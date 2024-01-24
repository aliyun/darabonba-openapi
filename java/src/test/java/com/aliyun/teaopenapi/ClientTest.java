package com.aliyun.teaopenapi;

import com.aliyun.tea.*;
import com.aliyun.teaopenapi.models.Config;
import com.aliyun.teaopenapi.models.GlobalParameters;
import com.aliyun.teaopenapi.models.OpenApiRequest;
import com.aliyun.teaopenapi.models.Params;
import com.aliyun.teautil.models.*;
import com.github.tomakehurst.wiremock.junit.WireMockRule;
import org.junit.Assert;
import org.junit.Rule;
import org.junit.Test;

import java.io.ByteArrayInputStream;
import java.util.List;
import java.util.Map;

import static com.github.tomakehurst.wiremock.client.WireMock.*;

public class ClientTest {
    @Rule
    public WireMockRule wireMock = new WireMockRule();

    @Test
    public void testConfig() throws Exception {
        GlobalParameters globalParameters = GlobalParameters.build(TeaConverter.buildMap(
                new TeaPair("headers", TeaConverter.buildMap(
                        new TeaPair("global-key", "global-value")
                )),
                new TeaPair("queries", TeaConverter.buildMap(
                        new TeaPair("global-query", "global-value")
                ))
        ));
        Config config = Config.build(TeaConverter.buildMap(
                new TeaPair("endpoint", "config.endpoint"),
                new TeaPair("endpointType", "public"),
                new TeaPair("network", "config.network"),
                new TeaPair("suffix", "config.suffix"),
                new TeaPair("protocol", "config.protocol"),
                new TeaPair("method", "config.method"),
                new TeaPair("regionId", "config.regionId"),
                new TeaPair("userAgent", "config.userAgent"),
                new TeaPair("readTimeout", 3000),
                new TeaPair("connectTimeout", 3000),
                new TeaPair("httpProxy", "config.httpProxy"),
                new TeaPair("httpsProxy", "config.httpsProxy"),
                new TeaPair("noProxy", "config.noProxy"),
                new TeaPair("socks5Proxy", "config.socks5Proxy"),
                new TeaPair("socks5NetWork", "config.socks5NetWork"),
                new TeaPair("maxIdleConns", 128),
                new TeaPair("signatureVersion", "config.signatureVersion"),
                new TeaPair("signatureAlgorithm", "config.signatureAlgorithm"),
                new TeaPair("globalParameters", globalParameters),
                new TeaPair("key", "config.key"),
                new TeaPair("cert", "config.cert"),
                new TeaPair("ca", "config.ca"),
                new TeaPair("disableHttp2", false)
        ));
        com.aliyun.credentials.models.Config creConfig = com.aliyun.credentials.models.Config.build(TeaConverter.buildMap(
                new TeaPair("accessKeyId", "accessKeyId"),
                new TeaPair("accessKeySecret", "accessKeySecret"),
                new TeaPair("securityToken", "securityToken"),
                new TeaPair("type", "sts")
        ));
        com.aliyun.credentials.Client credential = new com.aliyun.credentials.Client(creConfig);
        config.credential = credential;
        Client client = new Client(config);
        Assert.assertEquals("accessKeyId", client.getAccessKeyId());
        Assert.assertEquals("accessKeySecret", client.getAccessKeySecret());
        Assert.assertEquals("securityToken", client.getSecurityToken());
        Assert.assertEquals("sts", client.getType());
        config.accessKeyId = "ak";
        config.accessKeySecret = "secret";
        config.securityToken = "token";
        config.type = "sts";
        client = new Client(config);
        Assert.assertEquals("ak", client.getAccessKeyId());
        Assert.assertEquals("secret", client.getAccessKeySecret());
        Assert.assertEquals("token", client.getSecurityToken());
        Assert.assertEquals("sts", client.getType());
        Assert.assertNull(client._spi);
        Assert.assertNull(client._endpointRule);
        Assert.assertNull(client._endpointMap);
        Assert.assertNull(client._productId);
        Assert.assertEquals("config.endpoint", client._endpoint);
        Assert.assertEquals("public", client._endpointType);
        Assert.assertEquals("config.network", client._network);
        Assert.assertEquals("config.suffix", client._suffix);
        Assert.assertEquals("config.protocol", client._protocol);
        Assert.assertEquals("config.method", client._method);
        Assert.assertEquals("config.regionId", client._regionId);
        Assert.assertEquals("config.userAgent", client._userAgent);
        Assert.assertEquals(3000, (int) client._readTimeout);
        Assert.assertEquals(3000, (int) client._connectTimeout);
        Assert.assertEquals("config.httpProxy", client._httpProxy);
        Assert.assertEquals("config.httpsProxy", client._httpsProxy);
        Assert.assertEquals("config.noProxy", client._noProxy);
        Assert.assertEquals("config.socks5Proxy", client._socks5Proxy);
        Assert.assertEquals("config.socks5NetWork", client._socks5NetWork);
        Assert.assertEquals(128, (int) client._maxIdleConns);
        Assert.assertEquals("config.signatureVersion", client._signatureVersion);
        Assert.assertEquals("config.signatureAlgorithm", client._signatureAlgorithm);
        Assert.assertEquals("global-value", client._globalParameters.getHeaders().get("global-key"));
        Assert.assertEquals("global-value", client._globalParameters.getQueries().get("global-query"));
        Assert.assertEquals("config.key", client._key);
        Assert.assertEquals("config.cert", client._cert);
        Assert.assertEquals("config.ca", client._ca);
        Assert.assertEquals(false, client._disableHttp2);
    }

    public static Config createConfig() throws Exception {
        GlobalParameters globalParameters = GlobalParameters.build(TeaConverter.buildMap(
                new TeaPair("headers", TeaConverter.buildMap(
                        new TeaPair("global-key", "global-value")
                )),
                new TeaPair("queries", TeaConverter.buildMap(
                        new TeaPair("global-query", "global-value")
                ))
        ));
        Config config = Config.build(TeaConverter.buildMap(
                new TeaPair("accessKeyId", "ak"),
                new TeaPair("accessKeySecret", "secret"),
                new TeaPair("securityToken", "token"),
                new TeaPair("type", "sts"),
                new TeaPair("userAgent", "config.userAgent"),
                new TeaPair("readTimeout", 3000),
                new TeaPair("connectTimeout", 3000),
                new TeaPair("maxIdleConns", 128),
                new TeaPair("signatureVersion", "config.signatureVersion"),
                new TeaPair("signatureAlgorithm", "ACS3-HMAC-SHA256"),
                new TeaPair("globalParameters", globalParameters),
                new TeaPair("disableHttp2", true)
        ));
        return config;
    }

    public static RuntimeOptions createRuntimeOptions() throws Exception {
        RuntimeOptions runtime = RuntimeOptions.build(TeaConverter.buildMap(
                new TeaPair("readTimeout", 4000),
                new TeaPair("connectTimeout", 4000),
                new TeaPair("maxIdleConns", 100),
                new TeaPair("autoretry", true),
                new TeaPair("maxAttempts", 1),
                new TeaPair("backoffPolicy", "no"),
                new TeaPair("backoffPeriod", 1),
                new TeaPair("ignoreSSL", true)
        ));
        return runtime;
    }

    public static OpenApiRequest createOpenApiRequest() throws Exception {
        java.util.Map<String, Object> query = new java.util.HashMap<>();
        query.put("key1", "value");
        query.put("key2", 1);
        query.put("key3", true);
        java.util.Map<String, Object> body = new java.util.HashMap<>();
        body.put("key1", "value");
        body.put("key2", 1);
        body.put("key3", true);
        java.util.Map<String, String> headers = TeaConverter.buildMap(
                new TeaPair("for-test", "sdk")
        );
        OpenApiRequest req = OpenApiRequest.build(TeaConverter.buildMap(
                new TeaPair("headers", headers),
                new TeaPair("query", com.aliyun.openapiutil.Client.query(query)),
                new TeaPair("body", com.aliyun.openapiutil.Client.parseToMap(body))
        ));
        return req;
    }

    @Test
    public void testCallApiForRPCWithV2Sign_AK_Form() throws Exception {
        StringBuilder requestBody = new StringBuilder();
        String responseBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";
        Config config = ClientTest.createConfig();
        RuntimeOptions runtime = ClientTest.createRuntimeOptions();
        config.protocol = "HTTP";
        config.signatureAlgorithm = "v2";
        config.endpoint = "localhost:" + wireMock.port();
        Client client = new Client(config);
        OpenApiRequest request = ClientTest.createOpenApiRequest();
        for (String key : ((java.util.Map<String, Object>) request.body).keySet()) {
            requestBody.append(key).append("=").append(((Map<?, ?>) request.body).get(key));
            requestBody.append("&");
        }
        Params params = Params.build(TeaConverter.buildMap(
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
        stubFor(post(anyUrl()).withRequestBody(equalTo(requestBody.deleteCharAt(requestBody.length() - 1).toString()))
                .withQueryParam("key1", equalTo("value"))
                .withQueryParam("key2", equalTo("1"))
                .withQueryParam("key3", equalTo("true"))
                .withQueryParam("global-query", equalTo("global-value"))
                .withQueryParam("Action", equalTo("TestAPI"))
                .withQueryParam("Version", equalTo("2022-06-01"))
                .withQueryParam("AccessKeyId", equalTo("ak"))
                .withQueryParam("SecurityToken", equalTo("token"))
                .withQueryParam("SignatureVersion", equalTo("1.0"))
                .withQueryParam("SignatureMethod", equalTo("HMAC-SHA1"))
                .withQueryParam("Format", equalTo("json"))
                .withQueryParam("Timestamp", matching(".+"))
                .withQueryParam("SignatureNonce", matching(".+"))
                .withQueryParam("Signature", matching(".+"))
                .withHeader("global-key", equalTo("global-value"))
                .withHeader("host", matching("localhost:[0-9]+"))
                .withHeader("x-acs-version", equalTo("2022-06-01"))
                .withHeader("x-acs-action", equalTo("TestAPI"))
                .withHeader("user-agent", matching("AlibabaCloud.+tea-util/0.2.21 TeaDSL/1 config.userAgent"))
                .withHeader("content-type", equalTo("application/x-www-form-urlencoded"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        Map<String, ?> result = client.callApi(params, request, runtime);
        Assert.assertEquals("A45EE076-334D-5012-9746-A8F828D20FD4", ((Map) result.get("headers")).get("x-acs-request-id"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("AppId"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("ClassId"));
        Assert.assertEquals(123L, ((Map) result.get("body")).get("UserId"));
        Assert.assertEquals(200, result.get("statusCode"));
    }

    @Test
    public void testCallApiForRPCWithV2Sign_Anonymous_JSON() throws Exception {
        StringBuilder requestBody = new StringBuilder();
        String responseBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";
        Config config = ClientTest.createConfig();
        RuntimeOptions runtime = ClientTest.createRuntimeOptions();
        config.protocol = "HTTP";
        config.signatureAlgorithm = "v2";
        config.endpoint = "localhost:" + wireMock.port();
        Client client = new Client(config);
        OpenApiRequest request = ClientTest.createOpenApiRequest();
        for (String key : ((java.util.Map<String, Object>) request.body).keySet()) {
            requestBody.append(key).append("=").append(((Map<?, ?>) request.body).get(key));
            requestBody.append("&");
        }
        Params params = Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "Anonymous"),
                new TeaPair("style", "RPC"),
                new TeaPair("reqBodyType", "json"),
                new TeaPair("bodyType", "json")
        ));
        stubFor(post(anyUrl()).withRequestBody(equalTo(requestBody.deleteCharAt(requestBody.length() - 1).toString()))
                .withQueryParam("key1", equalTo("value"))
                .withQueryParam("key2", equalTo("1"))
                .withQueryParam("key3", equalTo("true"))
                .withQueryParam("global-query", equalTo("global-value"))
                .withQueryParam("Action", equalTo("TestAPI"))
                .withQueryParam("Version", equalTo("2022-06-01"))
                .withQueryParam("Format", equalTo("json"))
                .withQueryParam("Timestamp", matching(".+"))
                .withQueryParam("SignatureNonce", matching(".+"))
                .withHeader("global-key", equalTo("global-value"))
                .withHeader("host", matching("localhost:[0-9]+"))
                .withHeader("x-acs-version", equalTo("2022-06-01"))
                .withHeader("x-acs-action", equalTo("TestAPI"))
                .withHeader("user-agent", matching("AlibabaCloud.+tea-util/0.2.21 TeaDSL/1 config.userAgent"))
                .withHeader("content-type", equalTo("application/x-www-form-urlencoded"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        Map<String, ?> result = client.callApi(params, request, runtime);
        Assert.assertEquals("A45EE076-334D-5012-9746-A8F828D20FD4", ((Map) result.get("headers")).get("x-acs-request-id"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("AppId"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("ClassId"));
        Assert.assertEquals(123L, ((Map) result.get("body")).get("UserId"));
        Assert.assertEquals(200, result.get("statusCode"));
    }

    @Test
    public void testCallApiForROAWithV2Sign_AK_Form() throws Exception {
        StringBuilder requestBody = new StringBuilder();
        String responseBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";
        Config config = ClientTest.createConfig();
        RuntimeOptions runtime = ClientTest.createRuntimeOptions();
        config.protocol = "HTTP";
        config.signatureAlgorithm = "v2";
        config.endpoint = "localhost:" + wireMock.port();
        Client client = new Client(config);
        OpenApiRequest request = ClientTest.createOpenApiRequest();
        for (String key : ((java.util.Map<String, Object>) request.body).keySet()) {
            requestBody.append(key).append("=").append(((Map<?, ?>) request.body).get(key));
            requestBody.append("&");
        }
        Params params = Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/test"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "AK"),
                new TeaPair("style", "ROA"),
                new TeaPair("reqBodyType", "formData"),
                new TeaPair("bodyType", "json")
        ));
        stubFor(post(urlMatching("/test\\?.+")).withRequestBody(equalTo(requestBody.deleteCharAt(requestBody.length() - 1).toString()))
                .withQueryParam("key1", equalTo("value"))
                .withQueryParam("key2", equalTo("1"))
                .withQueryParam("key3", equalTo("true"))
                .withQueryParam("global-query", equalTo("global-value"))
                .withHeader("global-key", equalTo("global-value"))
                .withHeader("for-test", matching("sdk"))
                .withHeader("date", matching(".+"))
                .withHeader("host", matching("localhost:[0-9]+"))
                .withHeader("accept", equalTo("application/json"))
                .withHeader("x-acs-signature-nonce", matching(".+"))
                .withHeader("x-acs-signature-method", equalTo("HMAC-SHA1"))
                .withHeader("x-acs-signature-version", equalTo("1.0"))
                .withHeader("x-acs-accesskey-id", equalTo("ak"))
                .withHeader("x-acs-security-token", equalTo("token"))
                .withHeader("authorization", matching("acs ak:.+"))
                .withHeader("x-acs-version", equalTo("2022-06-01"))
                .withHeader("x-acs-action", equalTo("TestAPI"))
                .withHeader("user-agent", matching("AlibabaCloud.+tea-util/0.2.21 TeaDSL/1 config.userAgent"))
                .withHeader("content-type", equalTo("application/x-www-form-urlencoded"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        Map<String, ?> result = client.callApi(params, request, runtime);
        Assert.assertEquals("A45EE076-334D-5012-9746-A8F828D20FD4", ((Map) result.get("headers")).get("x-acs-request-id"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("AppId"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("ClassId"));
        Assert.assertEquals(123L, ((Map) result.get("body")).get("UserId"));
        Assert.assertEquals(200, result.get("statusCode"));
    }

    @Test
    public void testCallApiForROAWithV2Sign_Anonymous_JSON() throws Exception {
        String requestBody = "{\"key1\":\"value\",\"key2\":1,\"key3\":true}";
        String responseBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";
        Config config = ClientTest.createConfig();
        RuntimeOptions runtime = ClientTest.createRuntimeOptions();
        config.protocol = "HTTP";
        config.signatureAlgorithm = "v2";
        config.endpoint = "localhost:" + wireMock.port();
        Client client = new Client(config);
        OpenApiRequest request = ClientTest.createOpenApiRequest();
        Params params = Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/test"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "Anonymous"),
                new TeaPair("style", "ROA"),
                new TeaPair("reqBodyType", "json"),
                new TeaPair("bodyType", "json")
        ));
        stubFor(post(urlMatching("/test\\?.+")).withRequestBody(equalToJson(requestBody))
                .withQueryParam("key1", equalTo("value"))
                .withQueryParam("key2", equalTo("1"))
                .withQueryParam("key3", equalTo("true"))
                .withQueryParam("global-query", equalTo("global-value"))
                .withHeader("global-key", equalTo("global-value"))
                .withHeader("for-test", matching("sdk"))
                .withHeader("date", matching(".+"))
                .withHeader("host", matching("localhost:[0-9]+"))
                .withHeader("accept", equalTo("application/json"))
                .withHeader("x-acs-signature-nonce", matching(".+"))
                .withHeader("x-acs-signature-method", equalTo("HMAC-SHA1"))
                .withHeader("x-acs-signature-version", equalTo("1.0"))
                .withHeader("x-acs-version", equalTo("2022-06-01"))
                .withHeader("x-acs-action", equalTo("TestAPI"))
                .withHeader("user-agent", matching("AlibabaCloud.+tea-util/0.2.21 TeaDSL/1 config.userAgent"))
                .withHeader("content-type", equalTo("application/json; charset=UTF-8"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        Map<String, ?> result = client.callApi(params, request, runtime);
        Assert.assertEquals("A45EE076-334D-5012-9746-A8F828D20FD4", ((Map) result.get("headers")).get("x-acs-request-id"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("AppId"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("ClassId"));
        Assert.assertEquals(123L, ((Map) result.get("body")).get("UserId"));
        Assert.assertEquals(200, result.get("statusCode"));
    }

    @Test
    public void testCallApiForRPCWithV3Sign_AK_Form() throws Exception {
        StringBuilder requestBody = new StringBuilder();
        String responseBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";
        Config config = ClientTest.createConfig();
        RuntimeOptions runtime = ClientTest.createRuntimeOptions();
        config.protocol = "HTTP";
        config.endpoint = "localhost:" + wireMock.port();
        Client client = new Client(config);
        OpenApiRequest request = ClientTest.createOpenApiRequest();
        for (String key : ((java.util.Map<String, Object>) request.body).keySet()) {
            requestBody.append(key).append("=").append(((Map<?, ?>) request.body).get(key));
            requestBody.append("&");
        }
        Params params = Params.build(TeaConverter.buildMap(
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
        stubFor(post(anyUrl()).withRequestBody(equalTo(requestBody.deleteCharAt(requestBody.length() - 1).toString()))
                .withQueryParam("key1", equalTo("value"))
                .withQueryParam("key2", equalTo("1"))
                .withQueryParam("key3", equalTo("true"))
                .withQueryParam("global-query", equalTo("global-value"))
                .withHeader("global-key", equalTo("global-value"))
                .withHeader("for-test", matching("sdk"))
                .withHeader("host", matching("localhost:[0-9]+"))
                .withHeader("x-acs-version", equalTo("2022-06-01"))
                .withHeader("x-acs-action", equalTo("TestAPI"))
                .withHeader("x-acs-date", matching(".+"))
                .withHeader("x-acs-signature-nonce", matching(".+"))
                .withHeader("x-acs-content-sha256", matching(".+"))
                .withHeader("accept", matching("application/json"))
                .withHeader("x-acs-accesskey-id", equalTo("ak"))
                .withHeader("x-acs-security-token", equalTo("token"))
                .withHeader("Authorization", matching("ACS3-HMAC-SHA256 Credential=ak,SignedHeaders=content-type;host;" +
                        "x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-date;x-acs-security-token;" +
                        "x-acs-signature-nonce;x-acs-version,Signature=.+"))
                .withHeader("user-agent", matching("AlibabaCloud.+tea-util/0.2.21 TeaDSL/1 config.userAgent"))
                .withHeader("content-type", equalTo("application/x-www-form-urlencoded"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        Map<String, ?> result = client.callApi(params, request, runtime);
        Assert.assertEquals("A45EE076-334D-5012-9746-A8F828D20FD4", ((Map) result.get("headers")).get("x-acs-request-id"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("AppId"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("ClassId"));
        Assert.assertEquals(123L, ((Map) result.get("body")).get("UserId"));
        Assert.assertEquals(200, result.get("statusCode"));
    }

    @Test
    public void testCallApiForRPCWithV3Sign_Anonymous_JSON() throws Exception {
        String requestBody = "{\"key1\":\"value\",\"key2\":1,\"key3\":true}";
        String responseBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";
        Config config = ClientTest.createConfig();
        RuntimeOptions runtime = ClientTest.createRuntimeOptions();
        config.protocol = "HTTP";
        config.endpoint = "localhost:" + wireMock.port();
        Client client = new Client(config);
        OpenApiRequest request = ClientTest.createOpenApiRequest();
        Params params = Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "Anonymous"),
                new TeaPair("style", "RPC"),
                new TeaPair("reqBodyType", "json"),
                new TeaPair("bodyType", "json")
        ));
        stubFor(post(anyUrl()).withRequestBody(equalToJson(requestBody))
                .withQueryParam("key1", equalTo("value"))
                .withQueryParam("key2", equalTo("1"))
                .withQueryParam("key3", equalTo("true"))
                .withQueryParam("global-query", equalTo("global-value"))
                .withHeader("global-key", equalTo("global-value"))
                .withHeader("for-test", matching("sdk"))
                .withHeader("host", matching("localhost:[0-9]+"))
                .withHeader("x-acs-version", equalTo("2022-06-01"))
                .withHeader("x-acs-action", equalTo("TestAPI"))
                .withHeader("x-acs-date", matching(".+"))
                .withHeader("x-acs-signature-nonce", matching(".+"))
                .withHeader("x-acs-content-sha256", matching(".+"))
                .withHeader("accept", matching("application/json"))
                .withHeader("user-agent", matching("AlibabaCloud.+tea-util/0.2.21 TeaDSL/1 config.userAgent"))
                .withHeader("content-type", equalTo("application/json; charset=UTF-8"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        Map<String, ?> result = client.callApi(params, request, runtime);
        Assert.assertEquals("A45EE076-334D-5012-9746-A8F828D20FD4", ((Map) result.get("headers")).get("x-acs-request-id"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("AppId"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("ClassId"));
        Assert.assertEquals(123L, ((Map) result.get("body")).get("UserId"));
        Assert.assertEquals(200, result.get("statusCode"));
    }

    @Test
    public void testCallApiForROAWithV3Sign_AK_Form() throws Exception {
        StringBuilder requestBody = new StringBuilder();
        String responseBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";
        Config config = ClientTest.createConfig();
        RuntimeOptions runtime = ClientTest.createRuntimeOptions();
        config.protocol = "HTTP";
        config.endpoint = "localhost:" + wireMock.port();
        Client client = new Client(config);
        OpenApiRequest request = ClientTest.createOpenApiRequest();
        for (String key : ((java.util.Map<String, Object>) request.body).keySet()) {
            requestBody.append(key).append("=").append(((Map<?, ?>) request.body).get(key));
            requestBody.append("&");
        }
        Params params = Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/test"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "AK"),
                new TeaPair("style", "ROA"),
                new TeaPair("reqBodyType", "formData"),
                new TeaPair("bodyType", "json")
        ));
        stubFor(post(urlMatching("/test\\?.+")).withRequestBody(equalTo(requestBody.deleteCharAt(requestBody.length() - 1).toString()))
                .withQueryParam("key1", equalTo("value"))
                .withQueryParam("key2", equalTo("1"))
                .withQueryParam("key3", equalTo("true"))
                .withQueryParam("global-query", equalTo("global-value"))
                .withHeader("global-key", equalTo("global-value"))
                .withHeader("for-test", matching("sdk"))
                .withHeader("host", matching("localhost:[0-9]+"))
                .withHeader("x-acs-version", equalTo("2022-06-01"))
                .withHeader("x-acs-action", equalTo("TestAPI"))
                .withHeader("x-acs-date", matching(".+"))
                .withHeader("x-acs-signature-nonce", matching(".+"))
                .withHeader("x-acs-content-sha256", matching(".+"))
                .withHeader("accept", matching("application/json"))
                .withHeader("x-acs-accesskey-id", equalTo("ak"))
                .withHeader("x-acs-security-token", equalTo("token"))
                .withHeader("Authorization", matching("ACS3-HMAC-SHA256 Credential=ak,SignedHeaders=content-type;host;" +
                        "x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-date;x-acs-security-token;" +
                        "x-acs-signature-nonce;x-acs-version,Signature=.+"))
                .withHeader("user-agent", matching("AlibabaCloud.+tea-util/0.2.21 TeaDSL/1 config.userAgent"))
                .withHeader("content-type", equalTo("application/x-www-form-urlencoded"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        Map<String, ?> result = client.callApi(params, request, runtime);
        Assert.assertEquals("A45EE076-334D-5012-9746-A8F828D20FD4", ((Map) result.get("headers")).get("x-acs-request-id"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("AppId"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("ClassId"));
        Assert.assertEquals(123L, ((Map) result.get("body")).get("UserId"));
        Assert.assertEquals(200, result.get("statusCode"));
    }

    @Test
    public void testCallApiForROAWithV3Sign_Anonymous_JSON() throws Exception {
        String requestBody = "{\"key1\":\"value\",\"key2\":1,\"key3\":true}";
        String responseBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";
        Config config = ClientTest.createConfig();
        RuntimeOptions runtime = ClientTest.createRuntimeOptions();
        config.protocol = "HTTP";
        config.endpoint = "localhost:" + wireMock.port();
        Client client = new Client(config);
        OpenApiRequest request = ClientTest.createOpenApiRequest();
        Params params = Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/test"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "Anonymous"),
                new TeaPair("style", "RPC"),
                new TeaPair("reqBodyType", "json"),
                new TeaPair("bodyType", "json")
        ));
        stubFor(post(urlMatching("/test\\?.+")).withRequestBody(equalToJson(requestBody))
                .withQueryParam("key1", equalTo("value"))
                .withQueryParam("key2", equalTo("1"))
                .withQueryParam("key3", equalTo("true"))
                .withQueryParam("global-query", equalTo("global-value"))
                .withHeader("global-key", equalTo("global-value"))
                .withHeader("for-test", matching("sdk"))
                .withHeader("host", matching("localhost:[0-9]+"))
                .withHeader("x-acs-version", equalTo("2022-06-01"))
                .withHeader("x-acs-action", equalTo("TestAPI"))
                .withHeader("x-acs-date", matching(".+"))
                .withHeader("x-acs-signature-nonce", matching(".+"))
                .withHeader("x-acs-content-sha256", matching(".+"))
                .withHeader("accept", matching("application/json"))
                .withHeader("user-agent", matching("AlibabaCloud.+tea-util/0.2.21 TeaDSL/1 config.userAgent"))
                .withHeader("content-type", equalTo("application/json; charset=UTF-8"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        Map<String, ?> result = client.callApi(params, request, runtime);
        Assert.assertEquals("A45EE076-334D-5012-9746-A8F828D20FD4", ((Map) result.get("headers")).get("x-acs-request-id"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("AppId"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("ClassId"));
        Assert.assertEquals(123L, ((Map) result.get("body")).get("UserId"));
        Assert.assertEquals(200, result.get("statusCode"));
    }

    @Test
    public void testResponseBodyType() throws Exception {
        StringBuilder requestBody = new StringBuilder();
        String responseBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";
        Config config = ClientTest.createConfig();
        RuntimeOptions runtime = ClientTest.createRuntimeOptions();
        config.protocol = "HTTP";
        config.endpoint = "localhost:" + wireMock.port();
        Client client = new Client(config);
        OpenApiRequest request = ClientTest.createOpenApiRequest();
        for (String key : ((java.util.Map<String, Object>) request.body).keySet()) {
            requestBody.append(key).append("=").append(((Map<?, ?>) request.body).get(key));
            requestBody.append("&");
        }
        Params params = Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/test"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "AK"),
                new TeaPair("style", "ROA"),
                new TeaPair("reqBodyType", "formData"),
                new TeaPair("bodyType", "json")
        ));
        stubFor(post(urlMatching("/test\\?.+"))
                .withRequestBody(equalTo(requestBody.deleteCharAt(requestBody.length() - 1).toString()))
                .withQueryParam("key1", equalTo("value"))
                .withQueryParam("key2", equalTo("1"))
                .withQueryParam("key3", equalTo("true"))
                .withQueryParam("global-query", equalTo("global-value"))
                .withHeader("content-type", equalTo("application/x-www-form-urlencoded"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        Map<String, ?> result = client.callApi(params, request, runtime);
        Assert.assertEquals("A45EE076-334D-5012-9746-A8F828D20FD4", ((Map) result.get("headers")).get("x-acs-request-id"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("AppId"));
        Assert.assertEquals("test", ((Map) result.get("body")).get("ClassId"));
        Assert.assertEquals(123L, ((Map) result.get("body")).get("UserId"));
        Assert.assertEquals(200, result.get("statusCode"));

        params.bodyType = "array";
        responseBody = "[\"AppId\", \"ClassId\", \"UserId\"]";
        stubFor(post(urlMatching("/test\\?.+"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        result = client.callApi(params, request, runtime);
        Assert.assertEquals("A45EE076-334D-5012-9746-A8F828D20FD4", ((Map) result.get("headers")).get("x-acs-request-id"));
        Assert.assertEquals("AppId", ((List) result.get("body")).get(0));
        Assert.assertEquals("ClassId", ((List) result.get("body")).get(1));
        Assert.assertEquals("UserId", ((List) result.get("body")).get(2));
        Assert.assertEquals(200, result.get("statusCode"));

        params.bodyType = "string";
        result = client.callApi(params, request, runtime);
        Assert.assertEquals("A45EE076-334D-5012-9746-A8F828D20FD4", ((Map) result.get("headers")).get("x-acs-request-id"));
        Assert.assertEquals("[\"AppId\", \"ClassId\", \"UserId\"]", result.get("body"));
        Assert.assertEquals(200, result.get("statusCode"));

        params.bodyType = "byte";
        stubFor(post(urlMatching("/test\\?.+"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody.getBytes("UTF-8"))
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        result = client.callApi(params, request, runtime);
        Assert.assertEquals("A45EE076-334D-5012-9746-A8F828D20FD4", ((Map) result.get("headers")).get("x-acs-request-id"));
        Assert.assertEquals("[\"AppId\", \"ClassId\", \"UserId\"]", new String((byte[]) result.get("body"), "UTF-8"));
        Assert.assertEquals(200, result.get("statusCode"));

        responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"" +
                ", \"Description\":\"error description\", \"AccessDeniedDetail\":{}}";
        stubFor(post(urlMatching("/test\\?.+"))
                .willReturn(aResponse().withStatus(400).withBody(responseBody.getBytes("UTF-8"))
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        try {
            result = client.callApi(params, request, runtime);
        } catch (TeaException e) {
            Assert.assertEquals("code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4", e.getMessage());
            Assert.assertEquals("error code", e.getCode());
            Assert.assertEquals(400, (int) e.getStatusCode());
            Assert.assertNull(e.getAccessDeniedDetail().get("test"));
        }

        responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"" +
                ", \"Description\":\"error description\", \"AccessDeniedDetail\":{}, \"accessDeniedDetail\":{\"test\": 0}}";
        stubFor(post(urlMatching("/test\\?.+"))
                .willReturn(aResponse().withStatus(400).withBody(responseBody.getBytes("UTF-8"))
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        try {
            result = client.callApi(params, request, runtime);
        } catch (TeaException e) {
            Assert.assertEquals("code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4", e.getMessage());
            Assert.assertEquals("error code", e.getCode());
            Assert.assertEquals(400, (int) e.getStatusCode());
            Assert.assertNull(e.getAccessDeniedDetail().get("test"));
        }

        responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"" +
                ", \"Description\":\"error description\", \"accessDeniedDetail\":{\"test\": 0}}";
        stubFor(post(urlMatching("/test\\?.+"))
                .willReturn(aResponse().withStatus(400).withBody(responseBody.getBytes("UTF-8"))
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        try {
            result = client.callApi(params, request, runtime);
        } catch (TeaException e) {
            Assert.assertEquals("code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4", e.getMessage());
            Assert.assertEquals("error code", e.getCode());
            Assert.assertEquals(400, (int) e.getStatusCode());
            Assert.assertEquals(0L, (long) e.getAccessDeniedDetail().get("test"));
        }

    }

    @Test
    public void testRequestBodyType() throws Exception {
        String responseBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";
        Config config = ClientTest.createConfig();
        RuntimeOptions runtime = ClientTest.createRuntimeOptions();
        config.protocol = "HTTP";
        config.endpoint = "localhost:" + wireMock.port();
        Client client = new Client(config);

        // formData
        Params params = Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/test"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "AK"),
                new TeaPair("style", "RPC"),
                new TeaPair("reqBodyType", "formData"),
                new TeaPair("bodyType", "json")
        ));
        java.util.Map<String, Object> body = new java.util.HashMap<>();
        body.put("key1", "value");
        body.put("key2", 1);
        body.put("key3", true);
        OpenApiRequest request = OpenApiRequest.build(TeaConverter.buildMap(
                new TeaPair("body", com.aliyun.openapiutil.Client.parseToMap(body))
        ));
        StringBuilder requestBody = new StringBuilder();
        for (String key : ((java.util.Map<String, Object>) request.body).keySet()) {
            requestBody.append(key).append("=").append(((Map<?, ?>) request.body).get(key));
            requestBody.append("&");
        }
        stubFor(post(urlMatching("/test\\?.+"))
                .withRequestBody(equalTo(requestBody.deleteCharAt(requestBody.length() - 1).toString()))
                .withHeader("content-type", equalTo("application/x-www-form-urlencoded"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)));
        Map<String, ?> result = client.callApi(params, request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));

        // json
        params.setReqBodyType("json");
        stubFor(post(urlMatching("/test\\?.+")).withRequestBody(equalToJson("{\"key1\":\"value\",\"key2\":1,\"key3\":true}"))
                .withHeader("content-type", equalTo("application/json; charset=UTF-8"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)));
        result = client.callApi(params, request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));

        // byte
        params.setReqBodyType("byte");
        request = OpenApiRequest.build(TeaConverter.buildMap(
                new TeaPair("body", com.aliyun.teautil.Common.toBytes("test byte"))
        ));
        stubFor(post(urlMatching("/test\\?.+")).withRequestBody(equalTo("test byte"))
                .withHeader("content-type", equalTo("application/json; charset=UTF-8;"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)));
        result = client.callApi(params, request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));

        // stream
        params.setReqBodyType("binary");
        request = OpenApiRequest.build(TeaConverter.buildMap(
                new TeaPair("stream", new ByteArrayInputStream("test byte".getBytes("UTF-8")))
        ));
        stubFor(post(urlMatching("/test\\?.+")).withRequestBody(equalTo("test byte"))
                .withHeader("content-type", equalTo("application/octet-stream"))
                .willReturn(aResponse().withStatus(200).withBody(responseBody)));
        result = client.callApi(params, request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));

    }
}
