package com.aliyun.teaopenapi;

import com.aliyun.tea.TeaException;
import com.aliyun.tea.TeaConverter;
import com.aliyun.tea.TeaPair;
import com.aliyun.teaopenapi.models.Config;
import com.aliyun.teaopenapi.models.OpenApiRequest;
import com.aliyun.teaopenapi.models.Params;
import com.aliyun.teautil.models.RuntimeOptions;
import com.github.tomakehurst.wiremock.junit.WireMockRule;
import org.junit.Assert;
import org.junit.Rule;
import org.junit.Test;

import java.util.HashMap;
import java.util.Map;

import static com.github.tomakehurst.wiremock.client.WireMock.aResponse;
import static com.github.tomakehurst.wiremock.client.WireMock.anyUrl;
import static com.github.tomakehurst.wiremock.client.WireMock.post;
import static com.github.tomakehurst.wiremock.client.WireMock.stubFor;
import static com.github.tomakehurst.wiremock.client.WireMock.urlMatching;

public class ClientExtraCoverageTest {
    @Rule
    public WireMockRule wireMock = new WireMockRule();

    @Test
    public void testConstructorsHelpersAndExceptions() throws Exception {
        try {
            new Client(null);
            Assert.fail();
        } catch (TeaException e) {
            Assert.assertEquals("ParameterMissing", e.getCode());
        }

        Client empty = new Client(Config.build(TeaConverter.buildMap(
                new TeaPair("endpoint", "example.com")
        )));
        Assert.assertEquals("", empty.getAccessKeyId());
        Assert.assertEquals("", empty.getAccessKeySecret());
        Assert.assertEquals("", empty.getSecurityToken());
        Assert.assertEquals("", empty.getBearerToken());
        Assert.assertEquals("", empty.getType());
        Assert.assertTrue(empty.getUserAgent().length() > 0);

        try {
            empty.checkConfig(new Config());
            Assert.fail();
        } catch (TeaException e) {
            Assert.assertEquals("ParameterMissing", e.getCode());
        }
        empty.checkConfig(Config.build(TeaConverter.buildMap(
                new TeaPair("endpoint", "example.com")
        )));

        Map<String, String> once = new HashMap<String, String>();
        once.put("debug", "1");
        empty.setRpcHeaders(once);
        Assert.assertEquals("1", empty.getRpcHeaders().get("debug"));
        Assert.assertNull(empty.getRpcHeaders());

        try {
            empty.callApi(null, ClientTest.createOpenApiRequest(), ClientTest.createRuntimeOptions());
            Assert.fail();
        } catch (TeaException e) {
            Assert.assertEquals("ParameterMissing", e.getCode());
        }

        Assert.assertEquals("fb", Client.defaultAny(null, "fb"));
        Assert.assertEquals("keep", Client.defaultAny("keep", "fb"));

        Config bearerConfig = Config.build(TeaConverter.buildMap(
                new TeaPair("bearerToken", "token"),
                new TeaPair("endpoint", "example.com")
        ));
        Client bearer = new Client(bearerConfig);
        Assert.assertEquals("token", bearer.getBearerToken());
        Assert.assertEquals("bearer", bearer.getType());
    }

    @Test
    public void testCallApiXmlEmptyNoneDirectAndExecute() throws Exception {
        String jsonBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";
        stubFor(post(urlMatching("/test.*"))
                .willReturn(aResponse().withStatus(200).withBody(jsonBody)
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        stubFor(post(urlMatching("/empty.*"))
                .willReturn(aResponse().withStatus(204)
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        stubFor(post(urlMatching("/xml.*"))
                .willReturn(aResponse().withStatus(400)
                        .withHeader("Content-Type", "text/xml;charset=utf-8")
                        .withBody("<?xml version=\"1.0\"?><Error><Code>Invalid</Code><Message>xml error</Message><RequestId>xml-id</RequestId></Error>")));
        stubFor(post(urlMatching("/server.*"))
                .willReturn(aResponse().withStatus(500).withBody(
                        "{\"Code\":\"InternalError\",\"Message\":\"boom\",\"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"}")));

        Config config = ClientTest.createConfig();
        config.protocol = "HTTP";
        config.endpoint = "localhost:" + wireMock.port();
        Client client = new Client(config);
        RuntimeOptions runtime = ClientTest.createRuntimeOptions();
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
                new TeaPair("bodyType", "none")
        ));
        Map<String, ?> result = client.callApi(params, request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));

        params.bodyType = "binary";
        result = client.callApi(params, request, runtime);
        Assert.assertNotNull(result.get("body"));

        params.bodyType = "json";
        params.pathname = "/xml";
        try {
            client.callApi(params, request, runtime);
            Assert.fail();
        } catch (Exception e) {
            Assert.assertNotNull(e.getMessage());
        }

        Config v2Config = ClientTest.createConfig();
        v2Config.protocol = "HTTP";
        v2Config.signatureAlgorithm = "v2";
        v2Config.endpoint = "localhost:" + wireMock.port();
        Client v2 = new Client(v2Config);
        Params emptyParams = Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/empty"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "Anonymous"),
                new TeaPair("style", "ROA"),
                new TeaPair("reqBodyType", "json"),
                new TeaPair("bodyType", "json")
        ));
        result = v2.callApi(emptyParams, request, runtime);
        Assert.assertNotNull(result.get("headers"));
        emptyParams.reqBodyType = "formData";
        result = v2.callApi(emptyParams, request, runtime);
        Assert.assertNotNull(result.get("headers"));

        Params failParams = Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/server"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "Anonymous"),
                new TeaPair("style", "ROA"),
                new TeaPair("reqBodyType", "json"),
                new TeaPair("bodyType", "json")
        ));
        try {
            v2.callApi(failParams, request, runtime);
            Assert.fail();
        } catch (Exception e) {
            Assert.assertNotNull(e.getMessage());
        }
        failParams.reqBodyType = "formData";
        try {
            v2.callApi(failParams, request, runtime);
            Assert.fail();
        } catch (Exception e) {
            Assert.assertNotNull(e.getMessage());
        }
        failParams.style = "RPC";
        failParams.pathname = "/";
        stubFor(post(anyUrl())
                .willReturn(aResponse().withStatus(500).withBody(
                        "{\"Code\":\"InternalError\",\"Message\":\"boom\",\"RequestId\":\"rid\"}")));
        try {
            v2.callApi(failParams, request, runtime);
        } catch (Exception e) {
            Assert.assertNotNull(e.getMessage());
        }

        stubFor(post(anyUrl())
                .willReturn(aResponse().withStatus(200).withBody(jsonBody)
                        .withHeader("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")));
        result = client.doRPCRequest("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "json", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = client.doRPCRequest("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "string", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = client.doRPCRequest("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "byte", request, runtime);
        Assert.assertNotNull(result.get("body"));
        result = client.doRPCRequest("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "none", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = client.doROARequest("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/test", "json", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = client.doROARequest("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/test", "string", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = client.doROARequestWithForm("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/test", "json", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = client.doROARequestWithForm("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/test", "none", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = client.doRequest(Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/test"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "Anonymous"),
                new TeaPair("style", "RPC"),
                new TeaPair("reqBodyType", "json"),
                new TeaPair("bodyType", "string")
        )), request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = client.doRequest(Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/test"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "AK"),
                new TeaPair("style", "ROA"),
                new TeaPair("reqBodyType", "formData"),
                new TeaPair("bodyType", "json")
        )), request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));

        Params v2Rpc = Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "Anonymous"),
                new TeaPair("style", "RPC"),
                new TeaPair("reqBodyType", "formData"),
                new TeaPair("bodyType", "json")
        ));
        result = v2.callApi(v2Rpc, request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        v2Rpc.reqBodyType = "json";
        result = v2.callApi(v2Rpc, request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));

        client._productId = "test";
        client.setGatewayClient(new com.aliyun.gateway.pop.Client());
        result = client.execute(Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/test"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "Anonymous"),
                new TeaPair("style", "RPC"),
                new TeaPair("reqBodyType", "json"),
                new TeaPair("bodyType", "json")
        )), request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = client.doRPCRequest("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "json", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = client.doROARequest("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "/test", "byte", request, runtime);
        Assert.assertNotNull(result.get("body"));
        result = client.doROARequestWithForm("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "/test", "string", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = client.execute(Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/test"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "AK"),
                new TeaPair("style", "RPC"),
                new TeaPair("reqBodyType", "formData"),
                new TeaPair("bodyType", "string")
        )), request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));

        result = v2.doRPCRequest("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "json", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = v2.doRPCRequest("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "string", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = v2.doROARequest("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "/test", "json", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = v2.doROARequest("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "/test", "string", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = v2.doROARequestWithForm("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "/test", "json", request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = v2.doROARequestWithForm("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "/test", "byte", request, runtime);
        Assert.assertNotNull(result.get("body"));
        result = v2.callApi(Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "AK"),
                new TeaPair("style", "RPC"),
                new TeaPair("reqBodyType", "formData"),
                new TeaPair("bodyType", "json")
        )), request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = v2.callApi(Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/test"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "AK"),
                new TeaPair("style", "ROA"),
                new TeaPair("reqBodyType", "json"),
                new TeaPair("bodyType", "json")
        )), request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
        result = v2.callApi(Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTPS"),
                new TeaPair("pathname", "/test"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "AK"),
                new TeaPair("style", "ROA"),
                new TeaPair("reqBodyType", "formData"),
                new TeaPair("bodyType", "json")
        )), request, runtime);
        Assert.assertEquals(200, result.get("statusCode"));
    }
}
