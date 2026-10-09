package com.aliyun.teaopenapi;

import com.aliyun.tea.TeaConverter;
import com.aliyun.tea.TeaPair;
import com.aliyun.teaopenapi.models.Config;
import com.aliyun.teaopenapi.models.OpenApiRequest;
import com.aliyun.teaopenapi.models.Params;
import com.aliyun.teautil.models.RuntimeOptions;
import org.junit.Assert;
import org.junit.Test;

public class Ipv4OnlyTest {
    // Nothing listens here; an IPv4-only client must refuse the IPv6 literal before connecting.
    private static final String IPV6_ENDPOINT = "[::1]:1";
    private static final String REFUSED = "ipv4Only is enabled";

    private interface Call {
        void invoke(Client client, Params params, OpenApiRequest request, RuntimeOptions runtime) throws Exception;
    }

    private static String messages(Throwable e) {
        StringBuilder sb = new StringBuilder();
        for (Throwable t = e; t != null; t = t.getCause()) {
            sb.append(t.getMessage()).append('\n');
        }
        return sb.toString();
    }

    private static Params params(String style, String reqBodyType) throws Exception {
        return Params.build(TeaConverter.buildMap(
                new TeaPair("action", "TestAPI"),
                new TeaPair("version", "2022-06-01"),
                new TeaPair("protocol", "HTTP"),
                new TeaPair("pathname", "/"),
                new TeaPair("method", "POST"),
                new TeaPair("authType", "AK"),
                new TeaPair("style", style),
                new TeaPair("reqBodyType", reqBodyType),
                new TeaPair("bodyType", "json")
        ));
    }

    private static void setGatewayStub(Client client) throws Exception {
        client._productId = "test";
        client.setGatewayClient(new com.aliyun.gateway.spi.Client() {
            @Override
            public void modifyConfiguration(com.aliyun.gateway.spi.models.InterceptorContext context,
                                            com.aliyun.gateway.spi.models.AttributeMap attributeMap) {
            }

            @Override
            public void modifyRequest(com.aliyun.gateway.spi.models.InterceptorContext context,
                                      com.aliyun.gateway.spi.models.AttributeMap attributeMap) {
                if (context.request.headers == null) {
                    context.request.headers = new java.util.HashMap<String, String>();
                }
                context.request.headers.put("host", context.configuration.endpoint);
            }

            @Override
            public void modifyResponse(com.aliyun.gateway.spi.models.InterceptorContext context,
                                       com.aliyun.gateway.spi.models.AttributeMap attributeMap) {
            }
        });
    }

    private static void assertIpv4Only(String signatureAlgorithm, String style, String reqBodyType,
                                       boolean gateway, Call call) throws Exception {
        // config.ipv4Only, runtime.ipv4Only, expected
        Object[][] settings = {
                {null, null, false},
                {true, null, true},
                {null, true, true},
                {false, true, true},
                {true, false, false},
        };
        for (Object[] setting : settings) {
            Config config = ClientTest.createConfig();
            config.protocol = "HTTP";
            config.endpoint = IPV6_ENDPOINT;
            config.signatureAlgorithm = signatureAlgorithm;
            config.ipv4Only = (Boolean) setting[0];
            Client client = new Client(config);
            Assert.assertEquals(setting[0], client._ipv4Only);
            if (gateway) {
                setGatewayStub(client);
            }
            RuntimeOptions runtime = ClientTest.createRuntimeOptions();
            runtime.connectTimeout = 1000;
            runtime.ipv4Only = (Boolean) setting[1];
            boolean expected = (Boolean) setting[2];
            try {
                call.invoke(client, params(style, reqBodyType), ClientTest.createOpenApiRequest(), runtime);
                Assert.fail("expected the request to fail");
            } catch (Exception e) {
                String all = messages(e);
                Assert.assertEquals(all, expected, all.contains(REFUSED));
            }
        }
    }

    private static final Call CALL_API = new Call() {
        @Override
        public void invoke(Client client, Params params, OpenApiRequest request, RuntimeOptions runtime) throws Exception {
            client.callApi(params, request, runtime);
        }
    };

    @Test
    public void testDoRPCRequest() throws Exception {
        assertIpv4Only("v2", "RPC", "formData", false, CALL_API);
    }

    @Test
    public void testDoROARequest() throws Exception {
        assertIpv4Only("v2", "ROA", "json", false, CALL_API);
    }

    @Test
    public void testDoROARequestWithForm() throws Exception {
        assertIpv4Only("v2", "ROA", "formData", false, CALL_API);
    }

    @Test
    public void testDoRequest() throws Exception {
        assertIpv4Only("ACS3-HMAC-SHA256", "RPC", "formData", false, CALL_API);
    }

    @Test
    public void testExecute() throws Exception {
        assertIpv4Only("ACS3-HMAC-SHA256", "RPC", "formData", true, new Call() {
            @Override
            public void invoke(Client client, Params params, OpenApiRequest request, RuntimeOptions runtime) throws Exception {
                client.execute(params, request, runtime);
            }
        });
    }
}
