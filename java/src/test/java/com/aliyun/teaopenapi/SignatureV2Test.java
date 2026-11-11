package com.aliyun.teaopenapi;

import com.aliyun.teaopenapi.models.Config;
import com.aliyun.teaopenapi.models.OpenApiRequest;
import com.aliyun.teautil.models.RuntimeOptions;
import com.github.tomakehurst.wiremock.http.QueryParameter;
import com.github.tomakehurst.wiremock.junit.WireMockRule;
import com.github.tomakehurst.wiremock.verification.LoggedRequest;
import org.apache.commons.codec.binary.Base64;
import org.junit.Assert;
import org.junit.Rule;
import org.junit.Test;

import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;
import java.net.URLEncoder;
import java.util.HashMap;
import java.util.Map;
import java.util.TreeMap;

import static com.github.tomakehurst.wiremock.client.WireMock.*;

public class SignatureV2Test {
    @Rule public WireMockRule wireMock = new WireMockRule();

    private static final java.io.IOException SIGNATURE_FAILURE = new java.io.IOException("signature failure");

    public static String failSignature() throws java.io.IOException {
        throw SIGNATURE_FAILURE;
    }

    @Test public void utilFailureIsPropagated() throws Exception {
        java.lang.reflect.Method invoke = Client.class.getDeclaredMethod("invokeSignatureV2", java.lang.reflect.Method.class, Object[].class);
        invoke.setAccessible(true);
        try {
            invoke.invoke(null, new Object[] { SignatureV2Test.class.getMethod("failSignature"), new Object[0] });
            Assert.fail("The original signing exception should propagate");
        } catch (java.lang.reflect.InvocationTargetException error) {
            Assert.assertSame(SIGNATURE_FAILURE, error.getCause());
        }
    }

    private static String encode(String value) throws Exception {
        return URLEncoder.encode(value, "UTF-8").replace("+", "%20").replace("*", "%2A").replace("%7E", "~");
    }

    private static String sign(String text, String key, String method) throws Exception {
        String algorithm = "HMAC-SHA1".equals(method) ? "HmacSHA1" : "HmacSHA256";
        Mac mac = Mac.getInstance(algorithm);
        mac.init(new SecretKeySpec(key.getBytes("UTF-8"), algorithm));
        return Base64.encodeBase64String(mac.doFinal(text.getBytes("UTF-8")));
    }

    private static Map<String, String> query(LoggedRequest request) throws Exception {
        Map<String, String> result = new TreeMap<String, String>();
        for (Map.Entry<String, QueryParameter> entry : request.getQueryParams().entrySet()) result.put(java.net.URLDecoder.decode(entry.getKey(), "UTF-8"), entry.getValue().firstValue());
        return result;
    }

    private static void assertRpcSignature(LoggedRequest request, String expected) throws Exception {
        Map<String, String> params = query(request);
        Assert.assertEquals(expected, params.get("SignatureMethod"));
        String actual = params.remove("Signature");
        String body = request.getBodyAsString();
        if (body != null && !body.isEmpty()) {
            for (String part : body.split("&")) {
                String[] pair = part.split("=", 2);
                params.put(java.net.URLDecoder.decode(pair[0], "UTF-8"), pair.length > 1 ? java.net.URLDecoder.decode(pair[1], "UTF-8") : "");
            }
        }
        StringBuilder canonical = new StringBuilder();
        for (Map.Entry<String, String> entry : params.entrySet()) {
            if (entry.getValue().length() == 0) continue;
            if (canonical.length() > 0) canonical.append('&');
            canonical.append(encode(entry.getKey())).append('=').append(encode(entry.getValue()));
        }
        Assert.assertEquals(sign(request.getMethod().getName() + "&%2F&" + encode(canonical.toString()), "secret&", expected), actual);
    }

    @Test public void rpcAndRoaUseMatchingAlgorithms() throws Exception {
        stubFor(get(anyUrl()).willReturn(aResponse().withStatus(200).withHeader("Content-Type", "application/json").withBody("{}")));
        for (String style : new String[] { "RPC", "ROA", "ROA-form" }) {
            for (String requested : new String[] { "HMAC-SHA256", "HMAC-SHA1" }) {
                String expected = "HMAC-SHA256";
                wireMock.resetRequests();
                Config config = ClientTest.createConfig();
                config.protocol = "HTTP";
                config.endpoint = "localhost:" + wireMock.port();
                OpenApiRequest request = new OpenApiRequest();
                request.query = new HashMap<String, String>();
                request.query.put("中文 +/%~", "空 格+/%*~雪");
                request.query.put("Empty", "");
                request.headers = new HashMap<String, String>();
                if ("HMAC-SHA1".equals(requested)) {
                    config.globalParameters.headers.put("x-acs-signature-method", "HMAC-SHA256");
                    request.headers.put("x-acs-signature-method", "HMAC-SHA1");
                }
                RuntimeOptions runtime = ClientTest.createRuntimeOptions();
                Client client = new Client(config);
                if ("RPC".equals(style)) client.doRPCRequest("Test", "2022-06-01", "HTTP", "GET", "AK", "json", request, runtime);
                else if ("ROA".equals(style)) client.doROARequest("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", request, runtime);
                else client.doROARequestWithForm("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", request, runtime);
                LoggedRequest captured = wireMock.findAll(getRequestedFor(urlPathEqualTo("RPC".equals(style) ? "/" : "/test"))).get(0);
                Map<String, String> params = query(captured);
                if ("RPC".equals(style)) {
                    assertRpcSignature(captured, expected);
                } else {
                    Assert.assertEquals(requested, captured.getHeader("x-acs-signature-method"));
                    TreeMap<String, String> headers = new TreeMap<String, String>();
                    for (String key : captured.getAllHeaderKeys()) if (key.toLowerCase(java.util.Locale.ROOT).startsWith("x-acs-")) headers.put(key.toLowerCase(java.util.Locale.ROOT), captured.getHeader(key).trim());
                    StringBuilder canonical = new StringBuilder();
                    for (Map.Entry<String, String> entry : headers.entrySet()) canonical.append(entry.getKey()).append(':').append(entry.getValue()).append('\n');
                    StringBuilder resource = new StringBuilder("/test");
                    for (Map.Entry<String, String> entry : params.entrySet()) resource.append(resource.indexOf("?") < 0 ? '?' : '&').append(entry.getKey()).append(entry.getValue().length() == 0 ? "" : "=" + entry.getValue());
                    String text = "GET\napplication/json\n\n\n" + captured.getHeader("date") + "\n" + canonical + resource;
                    Assert.assertEquals("acs ak:" + sign(text, "secret", expected), captured.getHeader("authorization"));
                }
            }
        }
    }

    @Test public void rpcQuerySignatureMethodRemainsSdkControlled() throws Exception {
        stubFor(get(anyUrl()).willReturn(aResponse().withStatus(200).withHeader("Content-Type", "application/json").withBody("{}")));
        for (String scenario : new String[] { "global", "extended", "request", "header", "other", "empty" }) {
            wireMock.resetRequests();
            Config config = ClientTest.createConfig();
            config.protocol = "HTTP";
            config.endpoint = "localhost:" + wireMock.port();
            RuntimeOptions runtime = ClientTest.createRuntimeOptions();
            OpenApiRequest request = new OpenApiRequest();
            request.query = new HashMap<String, String>();
            request.headers = new HashMap<String, String>();
            if ("global".equals(scenario)) config.globalParameters.queries.put("SignatureMethod", "HMAC-SHA1");
            if ("extended".equals(scenario)) {
                config.globalParameters.queries.put("SignatureMethod", "ignored");
                runtime.extendsParameters.queries.put("SignatureMethod", "HMAC-SHA1");
            }
            if ("request".equals(scenario) || "header".equals(scenario)) request.query.put("SignatureMethod", "HMAC-SHA1");
            if ("header".equals(scenario)) request.headers.put("X-Acs-Signature-Method", "HMAC-SHA256");
            if ("other".equals(scenario)) request.query.put("SignatureMethod", "unsupported");
            if ("empty".equals(scenario)) request.query.put("SignatureMethod", "");
            new Client(config).doRPCRequest("Test", "2022-06-01", "HTTP", "GET", "AK", "json", request, runtime);
            LoggedRequest captured = wireMock.findAll(getRequestedFor(urlPathEqualTo("/"))).get(0);
            assertRpcSignature(captured, "HMAC-SHA256");
        }
    }

    @Test public void rpcBodySignatureMethodIsUnchanged() throws Exception {
        stubFor(post(anyUrl()).willReturn(aResponse().withStatus(200).withHeader("Content-Type", "application/json").withBody("{}")));
        Config config = ClientTest.createConfig();
        config.protocol = "HTTP";
        config.endpoint = "localhost:" + wireMock.port();
        OpenApiRequest request = new OpenApiRequest();
        Map<String, Object> body = new HashMap<String, Object>();
        body.put("SignatureMethod", "HMAC-SHA1");
        request.body = body;
        new Client(config).doRPCRequest("Test", "2022-06-01", "HTTP", "POST", "AK", "json", request, ClientTest.createRuntimeOptions());
        LoggedRequest captured = wireMock.findAll(postRequestedFor(urlPathEqualTo("/"))).get(0);
        assertRpcSignature(captured, "HMAC-SHA256");
        Assert.assertEquals("HMAC-SHA1", body.get("SignatureMethod"));
        Assert.assertTrue(captured.getBodyAsString().contains("SignatureMethod=HMAC-SHA1"));
    }

    @Test public void roaNonAkPreservesDefaultAndUserHeaders() throws Exception {
        stubFor(get(anyUrl()).willReturn(aResponse().withStatus(200).withHeader("Content-Type", "application/json").withBody("{}")));
        for (String style : new String[] { "ROA", "ROA-form" }) {
            for (String auth : new String[] { "Anonymous", "Bearer" }) {
                for (String header : new String[] { null, "user-value" }) {
                    wireMock.resetRequests();
                    Config config = "Bearer".equals(auth) ? ClientTest.createBearerTokenConfig() : ClientTest.createConfig();
                    config.protocol = "HTTP";
                    config.endpoint = "localhost:" + wireMock.port();
                    OpenApiRequest request = new OpenApiRequest();
                    request.headers = new HashMap<String, String>();
                    if (header != null) request.headers.put("x-acs-signature-method", header);
                    Client client = new Client(config);
                    if ("ROA".equals(style)) client.doROARequest("Test", "2022-06-01", "HTTP", "GET", "Bearer".equals(auth) ? "AK" : "Anonymous", "/test", "json", request, ClientTest.createRuntimeOptions());
                    else client.doROARequestWithForm("Test", "2022-06-01", "HTTP", "GET", "Bearer".equals(auth) ? "AK" : "Anonymous", "/test", "json", request, ClientTest.createRuntimeOptions());
                    LoggedRequest captured = wireMock.findAll(getRequestedFor(urlPathEqualTo("/test"))).get(0);
                    Assert.assertEquals(header == null ? "HMAC-SHA1" : header, captured.getHeader("x-acs-signature-method"));
                    Assert.assertNull(captured.getHeader("authorization"));
                }
            }
        }
    }

    @Test public void roaAkPreservesGlobalAndRuntimeHeaders() throws Exception {
        stubFor(get(anyUrl()).willReturn(aResponse().withStatus(200).withHeader("Content-Type", "application/json").withBody("{}")));
        for (String style : new String[] { "ROA", "ROA-form" }) {
            for (String source : new String[] { "global", "runtime" }) {
                wireMock.resetRequests();
                Config config = ClientTest.createConfig();
                config.protocol = "HTTP";
                config.endpoint = "localhost:" + wireMock.port();
                RuntimeOptions runtime = ClientTest.createRuntimeOptions();
                if ("global".equals(source)) config.globalParameters.headers.put("x-acs-signature-method", "user-value");
                else runtime.extendsParameters.headers.put("x-acs-signature-method", "user-value");
                Client client = new Client(config);
                if ("ROA".equals(style)) client.doROARequest("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", new OpenApiRequest(), runtime);
                else client.doROARequestWithForm("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", new OpenApiRequest(), runtime);
                LoggedRequest captured = wireMock.findAll(getRequestedFor(urlPathEqualTo("/test"))).get(0);
                Assert.assertEquals("user-value", captured.getHeader("x-acs-signature-method"));
                Assert.assertTrue(captured.getHeader("authorization").startsWith("acs ak:"));
            }
        }
    }

    @Test public void roaAkKeepsHeaderKeyCaseInMergedRequest() throws Exception {
        stubFor(get(anyUrl()).willReturn(aResponse().withStatus(200).withHeader("Content-Type", "application/json").withBody("{}")));
        final Map<String, String> capturedHeaders = new HashMap<String, String>();
        Config config = ClientTest.createConfig();
        config.protocol = "HTTP";
        config.endpoint = "localhost:" + wireMock.port();
        Client client = new Client(config);
        client.addRequestInterceptor((context, attributes) -> {
            capturedHeaders.clear();
            capturedHeaders.putAll(context.teaRequest().headers);
            return context.teaRequest();
        });
        OpenApiRequest request = new OpenApiRequest();
        request.headers = new HashMap<String, String>();
        request.headers.put("X-Acs-Signature-Method", "HMAC-SHA1");
        for (String style : new String[] { "ROA", "ROA-form" }) {
            if ("ROA".equals(style)) client.doROARequest("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", request, ClientTest.createRuntimeOptions());
            else client.doROARequestWithForm("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", request, ClientTest.createRuntimeOptions());
            Assert.assertEquals("HMAC-SHA256", capturedHeaders.get("x-acs-signature-method"));
            Assert.assertEquals("HMAC-SHA1", capturedHeaders.get("X-Acs-Signature-Method"));
        }
        for (String source : new String[] { "global", "runtime", "request", "tree" }) {
            for (String style : new String[] { "ROA", "ROA-form" }) {
                RuntimeOptions runtime = ClientTest.createRuntimeOptions();
                request.headers = new HashMap<String, String>();
                config.globalParameters.headers.remove("x-acs-signature-method");
                if ("global".equals(source)) config.globalParameters.headers.put("x-acs-signature-method", null);
                else if ("runtime".equals(source)) runtime.extendsParameters.headers.put("x-acs-signature-method", null);
                else if ("request".equals(source)) request.headers.put("x-acs-signature-method", null);
                else {
                    request.headers = new TreeMap<String, String>(String.CASE_INSENSITIVE_ORDER);
                    request.headers.put("X-Acs-Signature-Method", "HMAC-SHA1");
                }
                if ("ROA".equals(style)) client.doROARequest("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", request, runtime);
                else client.doROARequestWithForm("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", request, runtime);
                Assert.assertEquals("HMAC-SHA256", capturedHeaders.get("x-acs-signature-method"));
                if ("tree".equals(source)) Assert.assertEquals("HMAC-SHA1", capturedHeaders.get("X-Acs-Signature-Method"));
            }
        }
    }

}
