package com.aliyun.teaopenapi;

import com.aliyun.tea.TeaException;
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

    @Test public void rpcAndRoaUseMatchingAlgorithms() throws Exception {
        stubFor(get(anyUrl()).willReturn(aResponse().withStatus(200).withHeader("Content-Type", "application/json").withBody("{}")));
        for (String style : new String[] { "RPC", "ROA" }) {
            for (String expected : new String[] { "HMAC-SHA256", "HMAC-SHA1" }) {
                wireMock.resetRequests();
                Config config = ClientTest.createConfig();
                config.protocol = "HTTP";
                config.endpoint = "localhost:" + wireMock.port();
                OpenApiRequest request = new OpenApiRequest();
                request.query = new HashMap<String, String>();
                request.query.put("中文 +/%~", "空 格+/%*~雪");
                request.query.put("Empty", "");
                request.headers = new HashMap<String, String>();
                if ("HMAC-SHA1".equals(expected)) {
                    config.globalParameters.headers.put("x-acs-signature-method", "HMAC-SHA256");
                    request.headers.put("X-Acs-Signature-Method", "HMAC-SHA1");
                }
                RuntimeOptions runtime = ClientTest.createRuntimeOptions();
                Client client = new Client(config);
                if ("RPC".equals(style)) client.doRPCRequest("Test", "2022-06-01", "HTTP", "GET", "AK", "json", request, runtime);
                else client.doROARequest("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", request, runtime);
                LoggedRequest captured = wireMock.findAll(getRequestedFor(urlPathEqualTo("RPC".equals(style) ? "/" : "/test"))).get(0);
                Map<String, String> params = query(captured);
                if ("RPC".equals(style)) {
                    Assert.assertEquals(expected, params.get("SignatureMethod"));
                    String actual = params.remove("Signature");
                    StringBuilder canonical = new StringBuilder();
                    for (Map.Entry<String, String> entry : params.entrySet()) {
                        if (entry.getValue().length() == 0) continue;
                        if (canonical.length() > 0) canonical.append('&');
                        canonical.append(encode(entry.getKey())).append('=').append(encode(entry.getValue()));
                    }
                    Assert.assertEquals(sign("GET&%2F&" + encode(canonical.toString()), "secret&", expected), actual);
                } else {
                    Assert.assertEquals(expected, captured.getHeader("x-acs-signature-method"));
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

    @Test public void conflictingHeadersFailBeforeRequest() throws Exception {
        Config config = ClientTest.createConfig();
        config.endpoint = "localhost:" + wireMock.port();
        OpenApiRequest request = new OpenApiRequest();
        request.headers = new HashMap<String, String>();
        request.headers.put("x-acs-signature-method", "HMAC-SHA1");
        request.headers.put("X-Acs-Signature-Method", "HMAC-SHA256");
        try {
            new Client(config).doRPCRequest("Test", "2022-06-01", "HTTP", "GET", "AK", "json", request, ClientTest.createRuntimeOptions());
            Assert.fail("Conflicting headers should be rejected");
        } catch (TeaException error) {
            Assert.assertEquals("InvalidSignatureAlgorithm", error.getCode());
        }
    }
}
