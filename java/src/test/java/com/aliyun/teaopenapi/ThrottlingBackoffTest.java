package com.aliyun.teaopenapi;

import com.aliyun.tea.TeaConverter;
import com.aliyun.tea.TeaException;
import com.aliyun.tea.TeaPair;
import com.aliyun.tea.TeaRequest;
import com.aliyun.tea.TeaResponse;
import com.aliyun.tea.TeaRetryableException;
import com.aliyun.tea.TeaUnretryableException;
import com.aliyun.teaopenapi.models.Config;
import com.aliyun.teaopenapi.models.Params;
import com.aliyun.teautil.models.RuntimeOptions;
import com.github.tomakehurst.wiremock.junit.WireMockRule;
import com.github.tomakehurst.wiremock.stubbing.Scenario;
import org.junit.Assert;
import org.junit.Rule;
import org.junit.Test;

import static com.github.tomakehurst.wiremock.core.WireMockConfiguration.wireMockConfig;

import java.util.HashMap;
import java.util.Map;

import static com.github.tomakehurst.wiremock.client.WireMock.aResponse;
import static com.github.tomakehurst.wiremock.client.WireMock.anyUrl;
import static com.github.tomakehurst.wiremock.client.WireMock.findAll;
import static com.github.tomakehurst.wiremock.client.WireMock.post;
import static com.github.tomakehurst.wiremock.client.WireMock.postRequestedFor;
import static com.github.tomakehurst.wiremock.client.WireMock.stubFor;

public class ThrottlingBackoffTest {
    @Rule
    public WireMockRule wireMock = new WireMockRule(wireMockConfig().dynamicPort());

    @Test
    public void testGetThrottlingTimeLeft() {
        Assert.assertNull(Client.getThrottlingTimeLeft(null));
        Assert.assertNull(Client.getThrottlingTimeLeft(new HashMap<String, String>()));

        Map<String, String> headers = new HashMap<String, String>();
        headers.put("x-acs-retry-after", "");
        Assert.assertNull(Client.getThrottlingTimeLeft(headers));

        headers.put("x-acs-retry-after", "0");
        Assert.assertNull(Client.getThrottlingTimeLeft(headers));

        headers.put("x-acs-retry-after", "-5");
        Assert.assertNull(Client.getThrottlingTimeLeft(headers));

        headers.put("x-acs-retry-after", "invalid");
        Assert.assertNull(Client.getThrottlingTimeLeft(headers));

        headers.put("x-acs-retry-after", "2000");
        Assert.assertEquals(Long.valueOf(2000L), Client.getThrottlingTimeLeft(headers));

        headers.clear();
        headers.put("X-ACS-Retry-After", " 80 ");
        Assert.assertEquals(Long.valueOf(80L), Client.getThrottlingTimeLeft(headers));
    }

    @Test
    public void testExtractRetryAfterAndResolveBackoff() {
        Assert.assertNull(Client.extractRetryAfter(new RuntimeException("x")));
        Assert.assertNull(Client.extractRetryAfter(new TeaException(TeaConverter.buildMap(
                new TeaPair("code", "Throttling"),
                new TeaPair("message", "throttled")
        ))));

        Map<String, Object> data = new HashMap<String, Object>();
        data.put("retryAfter", 150);
        TeaException withAfter = new TeaException(TeaConverter.buildMap(
                new TeaPair("code", "Throttling"),
                new TeaPair("message", "throttled"),
                new TeaPair("data", data)
        ));
        Assert.assertEquals(Long.valueOf(150L), Client.extractRetryAfter(withAfter));

        Map<String, Object> backoff = new HashMap<String, Object>();
        backoff.put("policy", "fixed");
        backoff.put("period", 10000);
        Assert.assertEquals(150, Client.resolveBackoffTime(backoff, 1, withAfter));
        Assert.assertEquals(10000, Client.resolveBackoffTime(backoff, 1, new TeaException(TeaConverter.buildMap(
                new TeaPair("code", "Throttling"),
                new TeaPair("message", "throttled")
        ))));

        data.put("retryAfter", Long.valueOf(Integer.MAX_VALUE) + 10L);
        TeaException huge = new TeaException(TeaConverter.buildMap(
                new TeaPair("code", "Throttling"),
                new TeaPair("message", "throttled"),
                new TeaPair("data", data)
        ));
        Assert.assertEquals(Integer.MAX_VALUE, Client.resolveBackoffTime(backoff, 1, huge));
    }

    @Test
    public void testThrowHttpErrorCompatibility() {
        TeaResponse response = new TeaResponse();
        response.headers.put("x-acs-retry-after", "80");
        Map<String, Object> body = new HashMap<String, Object>();
        body.put("Code", "Throttling");
        body.put("Message", "throttled");
        Map<String, Object> exceptionMap = TeaConverter.buildMap(
                new TeaPair("code", "Throttling"),
                new TeaPair("message", "throttled"),
                new TeaPair("data", body)
        );

        RuntimeOptions off = new RuntimeOptions();
        try {
            Client.throwHttpError(exceptionMap, response, off);
            Assert.fail("expected TeaException");
        } catch (TeaRetryableException e) {
            Assert.fail("autoretry off must not throw TeaRetryableException");
        } catch (TeaException e) {
            Assert.assertEquals("Throttling", e.getCode());
            Assert.assertFalse(e instanceof TeaRetryableException);
        }

        RuntimeOptions on = new RuntimeOptions();
        on.autoretry = true;
        response.headers.put("x-acs-retry-after", "0");
        try {
            Client.throwHttpError(exceptionMap, response, on);
            Assert.fail("expected TeaException");
        } catch (TeaRetryableException e) {
            Assert.fail("non-positive retry-after must not be retryable");
        } catch (TeaException e) {
            Assert.assertEquals("Throttling", e.getCode());
        }

        response.headers.remove("x-acs-retry-after");
        try {
            Client.throwHttpError(exceptionMap, response, on);
            Assert.fail("expected TeaException");
        } catch (TeaRetryableException e) {
            Assert.fail("4xx without header must not be retryable");
        } catch (TeaException e) {
            Assert.assertEquals("Throttling", e.getCode());
        }

        response.headers.put("x-acs-retry-after", "80");
        try {
            Client.throwHttpError(exceptionMap, response, on);
            Assert.fail("expected TeaRetryableException");
        } catch (TeaRetryableException e) {
            Assert.assertEquals("Throttling", e.getCode());
            Assert.assertEquals(80L, Long.parseLong(String.valueOf(e.getData().get("retryAfter"))));
        }
    }

    @Test
    public void testThrowAfterRetriesExhausted() {
        Map<String, Object> data = new HashMap<String, Object>();
        data.put("retryAfter", 80);
        TeaRetryableException throttling = new TeaRetryableException(TeaConverter.buildMap(
                new TeaPair("code", "Throttling"),
                new TeaPair("message", "throttled"),
                new TeaPair("data", data)
        ));
        RuntimeException exhausted = Client.retriesExhausted(new TeaRequest(), throttling);
        Assert.assertTrue(exhausted instanceof TeaException);
        Assert.assertFalse(exhausted instanceof TeaUnretryableException);
        Assert.assertEquals("Throttling", ((TeaException) exhausted).getCode());

        TeaRetryableException network = new TeaRetryableException(new RuntimeException("connect reset"));
        RuntimeException wrapped = Client.retriesExhausted(new TeaRequest(), network);
        Assert.assertTrue(wrapped instanceof TeaUnretryableException);
        Assert.assertEquals("connect reset", wrapped.getMessage());
    }

    @Test
    public void testCallApiFollowsRetryAfterThenSucceeds() throws Exception {
        stubThrottlingThenOk("80");
        RuntimeOptions runtime = retryRuntime();
        long start = System.currentTimeMillis();
        Map<String, ?> result = newClient().callApi(rpcParams(), ClientTest.createOpenApiRequest(), runtime);
        long elapsed = System.currentTimeMillis() - start;
        Assert.assertEquals(200, result.get("statusCode"));
        Assert.assertEquals(2, findAll(postRequestedFor(anyUrl())).size());
        Assert.assertTrue("wait should follow retry-after (~80ms), was " + elapsed, elapsed >= 70 && elapsed < 2000);
    }

    @Test
    public void testCallApiFollowsDifferentRetryAfter() throws Exception {
        stubThrottlingThenOk("150");
        long start = System.currentTimeMillis();
        Map<String, ?> result = newClient().callApi(rpcParams(), ClientTest.createOpenApiRequest(), retryRuntime());
        long elapsed = System.currentTimeMillis() - start;
        Assert.assertEquals(200, result.get("statusCode"));
        Assert.assertEquals(2, findAll(postRequestedFor(anyUrl())).size());
        Assert.assertTrue("wait should follow retry-after (~150ms), was " + elapsed, elapsed >= 130 && elapsed < 2000);
    }

    @Test
    public void testCallApiNoRetryWhenAutoretryOff() throws Exception {
        stubAlwaysThrottling("80");
        RuntimeOptions runtime = retryRuntime();
        runtime.autoretry = false;
        try {
            newClient().callApi(rpcParams(), ClientTest.createOpenApiRequest(), runtime);
            Assert.fail("expected TeaException");
        } catch (TeaRetryableException e) {
            Assert.fail("must not retry when autoretry is false");
        } catch (TeaException e) {
            Assert.assertEquals("Throttling", e.getCode());
        }
        Assert.assertEquals(1, findAll(postRequestedFor(anyUrl())).size());
    }

    @Test
    public void testCallApiNoRetryWithoutRetryAfterHeader() throws Exception {
        stubFor(post(anyUrl()).willReturn(aResponse().withStatus(400)
                .withBody("{\"Code\":\"InvalidParameter\",\"Message\":\"bad\",\"RequestId\":\"mock\"}")));
        try {
            newClient().callApi(rpcParams(), ClientTest.createOpenApiRequest(), retryRuntime());
            Assert.fail("expected TeaException");
        } catch (TeaRetryableException e) {
            Assert.fail("business 4xx must not become retryable");
        } catch (TeaException e) {
            Assert.assertEquals("InvalidParameter", e.getCode());
        }
        Assert.assertEquals(1, findAll(postRequestedFor(anyUrl())).size());
    }

    @Test
    public void testCallApiExhaustedThrowsTeaException() throws Exception {
        stubAlwaysThrottling("50");
        RuntimeOptions runtime = retryRuntime();
        runtime.maxAttempts = 1;
        try {
            newClient().callApi(rpcParams(), ClientTest.createOpenApiRequest(), runtime);
            Assert.fail("expected TeaException");
        } catch (TeaUnretryableException e) {
            Assert.fail("throttling exhaust must not wrap as TeaUnretryableException");
        } catch (TeaException e) {
            Assert.assertEquals("Throttling", e.getCode());
            Assert.assertEquals(50L, Long.parseLong(String.valueOf(e.getData().get("retryAfter"))));
        }
        Assert.assertEquals(2, findAll(postRequestedFor(anyUrl())).size());
    }

    private Client newClient() throws Exception {
        Config config = ClientTest.createConfig();
        config.protocol = "HTTP";
        config.signatureAlgorithm = "v2";
        config.endpoint = "localhost:" + wireMock.port();
        return new Client(config);
    }

    private static RuntimeOptions retryRuntime() throws Exception {
        RuntimeOptions runtime = ClientTest.createRuntimeOptions();
        runtime.autoretry = true;
        runtime.maxAttempts = 1;
        runtime.backoffPolicy = "fixed";
        runtime.backoffPeriod = 10000;
        return runtime;
    }

    private static Params rpcParams() throws Exception {
        return Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "AK"),
                new TeaPair("style", "RPC"),
                new TeaPair("reqBodyType", "formData"),
                new TeaPair("bodyType", "json")
        ));
    }

    private static void stubThrottlingThenOk(String retryAfterMs) {
        String scenario = "throttling-" + retryAfterMs + "-" + System.nanoTime();
        stubFor(post(anyUrl())
                .inScenario(scenario)
                .whenScenarioStateIs(Scenario.STARTED)
                .willSetStateTo("ok")
                .willReturn(aResponse().withStatus(400)
                        .withHeader("x-acs-retry-after", retryAfterMs)
                        .withBody("{\"Code\":\"Throttling\",\"Message\":\"throttled\",\"RequestId\":\"mock\"}")));
        stubFor(post(anyUrl())
                .inScenario(scenario)
                .whenScenarioStateIs("ok")
                .willReturn(aResponse().withStatus(200)
                        .withBody("{\"AppId\":\"ok\"}")
                        .withHeader("x-acs-request-id", "ok")));
    }

    private static void stubAlwaysThrottling(String retryAfterMs) {
        stubFor(post(anyUrl()).willReturn(aResponse().withStatus(400)
                .withHeader("x-acs-retry-after", retryAfterMs)
                .withBody("{\"Code\":\"Throttling\",\"Message\":\"throttled\",\"RequestId\":\"mock\"}")));
    }
}
