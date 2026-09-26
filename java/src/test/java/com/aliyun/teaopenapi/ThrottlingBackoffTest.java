package com.aliyun.teaopenapi;

import com.aliyun.tea.Tea;
import com.aliyun.tea.TeaConverter;
import com.aliyun.tea.TeaException;
import com.aliyun.tea.TeaPair;
import com.aliyun.tea.TeaRetryableException;
import com.aliyun.tea.TeaUnretryableException;
import com.aliyun.teaopenapi.models.Config;
import com.aliyun.teaopenapi.models.OpenApiRequest;
import com.aliyun.teaopenapi.models.Params;
import com.aliyun.teautil.models.RuntimeOptions;
import com.github.tomakehurst.wiremock.junit.WireMockRule;
import com.github.tomakehurst.wiremock.stubbing.Scenario;
import com.github.tomakehurst.wiremock.verification.LoggedRequest;
import org.junit.Assert;
import org.junit.Rule;
import org.junit.Test;

import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.nio.charset.Charset;
import java.util.List;
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
        Map<String, ?> result = newV2Client().callApi(rpcParams(), ClientTest.createOpenApiRequest(), runtime);
        long elapsed = System.currentTimeMillis() - start;
        Assert.assertEquals(200, result.get("statusCode"));
        Assert.assertEquals(2, findAll(postRequestedFor(anyUrl())).size());
        Assert.assertTrue("wait should follow retry-after (~80ms), was " + elapsed, elapsed >= 70 && elapsed < 2000);
    }

    @Test
    public void testCallApiFollowsDifferentRetryAfter() throws Exception {
        stubThrottlingThenOk("150", "x-acs-retry-after");
        long start = System.currentTimeMillis();
        Map<String, ?> result = newV2Client().callApi(rpcParams(), ClientTest.createOpenApiRequest(), retryRuntime());
        long elapsed = System.currentTimeMillis() - start;
        Assert.assertEquals(200, result.get("statusCode"));
        Assert.assertEquals(2, findAll(postRequestedFor(anyUrl())).size());
        Assert.assertTrue("wait should follow retry-after (~150ms), was " + elapsed, elapsed >= 130 && elapsed < 2000);
    }

    @Test
    public void testCallApiHeaderNameIsCaseInsensitive() throws Exception {
        stubThrottlingThenOk("80", "X-ACS-Retry-After");
        Map<String, ?> result = newV2Client().callApi(rpcParams(), ClientTest.createOpenApiRequest(), retryRuntime());
        Assert.assertEquals(200, result.get("statusCode"));
        Assert.assertEquals(2, findAll(postRequestedFor(anyUrl())).size());
    }

    @Test
    public void testCallApiNoRetryWhenAutoretryOff() throws Exception {
        stubAlwaysThrottling("80");
        RuntimeOptions runtime = retryRuntime();
        runtime.autoretry = false;
        try {
            newV2Client().callApi(rpcParams(), ClientTest.createOpenApiRequest(), runtime);
            Assert.fail("expected TeaException");
        } catch (TeaException e) {
            assertCompatibleHttpError(e, "Throttling");
        }
        Assert.assertEquals(1, findAll(postRequestedFor(anyUrl())).size());
    }

    @Test
    public void testCallApiNoRetryWithoutRetryAfterHeader() throws Exception {
        stubFor(post(anyUrl()).willReturn(aResponse().withStatus(400)
                .withBody("{\"Code\":\"InvalidParameter\",\"Message\":\"bad\",\"RequestId\":\"mock\"}")));
        try {
            newV2Client().callApi(rpcParams(), ClientTest.createOpenApiRequest(), retryRuntime());
            Assert.fail("expected TeaException");
        } catch (TeaException e) {
            assertCompatibleHttpError(e, "InvalidParameter");
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
    public void testCallApiExhaustedThrowsPlainTeaException() throws Exception {
        stubAlwaysThrottling("50");
        RuntimeOptions runtime = retryRuntime();
        runtime.maxAttempts = 1;
        try {
            newV2Client().callApi(rpcParams(), ClientTest.createOpenApiRequest(), runtime);
            Assert.fail("expected TeaException");
        } catch (TeaUnretryableException e) {
            Assert.fail("throttling exhaust must not wrap as TeaUnretryableException");
        } catch (TeaException e) {
            assertCompatibleHttpError(e, "Throttling");
        }
        Assert.assertEquals(2, findAll(postRequestedFor(anyUrl())).size());
    }

    @Test
    public void testDoRequestReplaysByteArrayStreamBody() throws Exception {
        stubThrottlingThenOk("50", "x-acs-retry-after");
        byte[] payload = "payload1".getBytes("UTF-8");
        OpenApiRequest request = OpenApiRequest.build(TeaConverter.buildMap(
                new TeaPair("stream", new ByteArrayInputStream(payload))
        ));
        Map<String, ?> result = newAcs3Client().callApi(rpcParams(), request, retryRuntime());
        Assert.assertEquals(200, result.get("statusCode"));
        assertRequestBodies(payload, payload);
    }

    @Test
    public void testDoRequestReplaysFileInputStreamBody() throws Exception {
        stubThrottlingThenOk("50", "x-acs-retry-after");
        File temp = File.createTempFile("tea-openapi-stream-", ".bin");
        temp.deleteOnExit();
        byte[] payload = "file-payload".getBytes(Charset.forName("UTF-8"));
        FileOutputStream out = new FileOutputStream(temp);
        try {
            out.write(payload);
        } finally {
            out.close();
        }
        FileInputStream stream = new FileInputStream(temp);
        try {
            OpenApiRequest request = OpenApiRequest.build(TeaConverter.buildMap(
                    new TeaPair("stream", stream)
            ));
            Map<String, ?> result = newAcs3Client().callApi(rpcParams(), request, retryRuntime());
            Assert.assertEquals(200, result.get("statusCode"));
            assertRequestBodies(payload, payload);
        } finally {
            stream.close();
        }
    }

    private void assertNoRetryForHeader(String retryAfter) throws Exception {
        stubAlwaysThrottling(retryAfter);
        try {
            newV2Client().callApi(rpcParams(), ClientTest.createOpenApiRequest(), retryRuntime());
            Assert.fail("expected TeaException for retry-after=" + retryAfter);
        } catch (TeaException e) {
            assertCompatibleHttpError(e, "Throttling");
        }
        Assert.assertEquals("retry-after=" + retryAfter, 1, findAll(postRequestedFor(anyUrl())).size());
    }

    private static void assertCompatibleHttpError(TeaException e, String code) {
        Assert.assertEquals(TeaException.class, e.getClass());
        Assert.assertFalse("HTTP error must not be Tea.isRetryable after it leaves Client", Tea.isRetryable(e));
        Assert.assertFalse(e instanceof TeaRetryableException);
        Assert.assertEquals(code, e.getCode());
        Map<String, Object> data = e.getData();
        if (data != null) {
            Assert.assertFalse("must not add retryAfter to exception data", data.containsKey("retryAfter"));
        }
    }

    private static void assertRequestBodies(byte[] first, byte[] second) {
        List<LoggedRequest> requests = findAll(postRequestedFor(anyUrl()));
        Assert.assertEquals(2, requests.size());
        Assert.assertArrayEquals(first, requests.get(0).getBody());
        Assert.assertArrayEquals(second, requests.get(1).getBody());
    }

    private Client newV2Client() throws Exception {
        Config config = ClientTest.createConfig();
        config.protocol = "HTTP";
        config.signatureAlgorithm = "v2";
        config.endpoint = "localhost:" + wireMock.port();
        return new Client(config);
    }

    private Client newAcs3Client() throws Exception {
        Config config = ClientTest.createConfig();
        config.protocol = "HTTP";
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
