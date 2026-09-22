package com.aliyun.teaopenapi;

import com.aliyun.tea.TeaConverter;
import com.aliyun.tea.TeaException;
import com.aliyun.tea.TeaPair;
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

import java.util.Map;

import static com.github.tomakehurst.wiremock.client.WireMock.aResponse;
import static com.github.tomakehurst.wiremock.client.WireMock.anyUrl;
import static com.github.tomakehurst.wiremock.client.WireMock.findAll;
import static com.github.tomakehurst.wiremock.client.WireMock.post;
import static com.github.tomakehurst.wiremock.client.WireMock.postRequestedFor;
import static com.github.tomakehurst.wiremock.client.WireMock.stubFor;
import static com.github.tomakehurst.wiremock.core.WireMockConfiguration.wireMockConfig;

public class ThrottlingBackoffTest {
    @Rule
    public WireMockRule wireMock = new WireMockRule(wireMockConfig().dynamicPort());

    @Test
    public void testCallApiFollowsRetryAfterThenSucceeds() throws Exception {
        stubThrottlingThenOk("80", "x-acs-retry-after");
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
        stubThrottlingThenOk("150", "x-acs-retry-after");
        long start = System.currentTimeMillis();
        Map<String, ?> result = newClient().callApi(rpcParams(), ClientTest.createOpenApiRequest(), retryRuntime());
        long elapsed = System.currentTimeMillis() - start;
        Assert.assertEquals(200, result.get("statusCode"));
        Assert.assertEquals(2, findAll(postRequestedFor(anyUrl())).size());
        Assert.assertTrue("wait should follow retry-after (~150ms), was " + elapsed, elapsed >= 130 && elapsed < 2000);
    }

    @Test
    public void testCallApiHeaderNameIsCaseInsensitive() throws Exception {
        stubThrottlingThenOk("80", "X-ACS-Retry-After");
        Map<String, ?> result = newClient().callApi(rpcParams(), ClientTest.createOpenApiRequest(), retryRuntime());
        Assert.assertEquals(200, result.get("statusCode"));
        Assert.assertEquals(2, findAll(postRequestedFor(anyUrl())).size());
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
    public void testCallApiNoRetryWhenRetryAfterNotPositive() throws Exception {
        assertNoRetryForHeader("0");
        wireMock.resetAll();
        assertNoRetryForHeader("");
        wireMock.resetAll();
        assertNoRetryForHeader("invalid");
        wireMock.resetAll();
        assertNoRetryForHeader("-5");
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

    private void assertNoRetryForHeader(String retryAfter) throws Exception {
        stubAlwaysThrottling(retryAfter);
        try {
            newClient().callApi(rpcParams(), ClientTest.createOpenApiRequest(), retryRuntime());
            Assert.fail("expected TeaException for retry-after=" + retryAfter);
        } catch (TeaRetryableException e) {
            Assert.fail("non-positive/invalid retry-after must not be retryable: " + retryAfter);
        } catch (TeaException e) {
            Assert.assertEquals("Throttling", e.getCode());
        }
        Assert.assertEquals("retry-after=" + retryAfter, 1, findAll(postRequestedFor(anyUrl())).size());
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

    private static void stubThrottlingThenOk(String retryAfterMs, String headerName) {
        String scenario = "throttling-" + retryAfterMs + "-" + System.nanoTime();
        stubFor(post(anyUrl())
                .inScenario(scenario)
                .whenScenarioStateIs(Scenario.STARTED)
                .willSetStateTo("ok")
                .willReturn(aResponse().withStatus(400)
                        .withHeader(headerName, retryAfterMs)
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
