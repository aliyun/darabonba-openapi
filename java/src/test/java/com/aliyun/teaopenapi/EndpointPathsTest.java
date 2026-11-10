package com.aliyun.teaopenapi;

import com.aliyun.tea.TeaRequest;
import com.aliyun.tea.interceptor.InterceptorContext;
import com.aliyun.tea.interceptor.RequestInterceptor;
import com.aliyun.tea.utils.AttributeMap;
import com.aliyun.teaopenapi.models.Config;
import com.aliyun.teaopenapi.models.OpenApiRequest;
import com.aliyun.teaopenapi.models.Params;
import com.aliyun.teautil.models.RuntimeOptions;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.concurrent.atomic.AtomicReference;
import org.junit.Assert;
import org.junit.Test;

public class EndpointPathsTest {
    @Test
    public void suffixPriorityAndValidation() throws Exception {
        Config config = new Config();
        config.endpoint = "ecs.aliyuncs.com";
        Client client = new Client(config) {
            @Override String endpointSuffixFromEnv() { return " ALIYUNCS.ALIBABA "; }
        };
        java.lang.reflect.Method resolve = Client.class.getDeclaredMethod("endpointForRequest", OpenApiRequest.class);
        resolve.setAccessible(true);
        OpenApiRequest request = new OpenApiRequest();
        Assert.assertEquals("ecs.aliyuncs.alibaba", resolve.invoke(client, request));
        client._endpointSuffix = "aliyuncs.com";
        Assert.assertEquals("ecs.aliyuncs.com", resolve.invoke(client, request));
        client._attributeMap = new com.aliyun.gateway.spi.models.AttributeMap();
        client._attributeMap.key = new java.util.HashMap<String, String>();
        client._attributeMap.key.put("endpointSuffix", " ALIYUNCS.ALIBABA ");
        Assert.assertEquals("ecs.aliyuncs.alibaba", resolve.invoke(client, request));
        client._attributeMap.key.put("endpointSuffix", "  ");
        Assert.assertEquals("ecs.aliyuncs.com", resolve.invoke(client, request));
        client._endpointSuffix = "";
        Assert.assertEquals("ecs.aliyuncs.alibaba", resolve.invoke(client, request));
        client._attributeMap.key.put("endpointSuffix", "evil.com");
        try {
            resolve.invoke(client, request);
            Assert.fail("invalid suffix accepted");
        } catch (java.lang.reflect.InvocationTargetException expected) {
            Assert.assertTrue(expected.getCause() instanceof com.aliyun.tea.TeaException);
        }
        Client defaults = new Client(config) {
            @Override String endpointSuffixFromEnv() { return null; }
        };
        Assert.assertEquals("ecs.aliyuncs.com", resolve.invoke(defaults, request));
    }

    @Test
    public void rewritesHostBeforeSendingEveryLegacyRequest() throws Exception {
        final AtomicBoolean active = new AtomicBoolean(true);
        final AtomicReference<String> seen = new AtomicReference<String>();
        Config initial = new Config();
        initial.endpoint = "ecs.aliyuncs.com";
        new Client(initial).addRequestInterceptor(new RequestInterceptor() {
            @Override
            public TeaRequest modifyRequest(InterceptorContext context, AttributeMap attributes) {
                if (active.get()) {
                    seen.set(context.teaRequest().headers.get("host"));
                    throw new RuntimeException("stop-before-network");
                }
                return context.teaRequest();
            }
        });
        try {
            for (int path = 0; path < 5; path++) {
                for (String[] item : new String[][] {
                    {"ecs.aliyuncs.com", "", "ecs.aliyuncs.alibaba"},
                    {"ecs.aliyuncs.com", "oss-cn-hangzhou.aliyuncs.com", "oss-cn-hangzhou.aliyuncs.alibaba"},
                    {"ecs.aliyun.com", "", "ecs.aliyun.com"},
                    {"ecs.aliyuncs.alibaba", "", "ecs.aliyuncs.alibaba"}
                }) {
                    Config config = new Config();
                    config.endpoint = item[0];
                    config.endpointSuffix = "aliyuncs.alibaba";
                    Client client = new Client(config);
                    OpenApiRequest request = new OpenApiRequest();
                    request.endpointOverride = item[1];
                    RuntimeOptions runtime = new RuntimeOptions();
                    runtime.autoretry = false;
                    runtime.maxAttempts = 1;
                    Params params = new Params();
                    params.action = "Test"; params.version = "2020-01-01"; params.protocol = "HTTPS";
                    params.method = "GET"; params.pathname = "/"; params.authType = "Anonymous";
                    params.style = "ROA"; params.reqBodyType = "json"; params.bodyType = "string";
                    seen.set(null);
                    try {
                        switch (path) {
                            case 0: client.doRPCRequest("Test", "2020-01-01", "HTTPS", "GET", "Anonymous", "string", request, runtime); break;
                            case 1: client.doROARequest("Test", "2020-01-01", "HTTPS", "GET", "Anonymous", "/", "string", request, runtime); break;
                            case 2: client.doROARequestWithForm("Test", "2020-01-01", "HTTPS", "GET", "Anonymous", "/", "string", request, runtime); break;
                            case 3: client.doRequest(params, request, runtime); break;
                            default: client.callApi(params, request, runtime); break;
                        }
                        Assert.fail("expected transport interceptor");
                    } catch (Exception expected) {
                        Assert.assertEquals(item[2], seen.get());
                    }
                    Assert.assertEquals(item[0], client._endpoint);
                }
            }
        } finally {
            active.set(false);
        }
    }
}
