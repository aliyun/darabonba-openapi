package com.aliyun.teaopenapi;

import com.aliyun.tea.TeaException;
import com.aliyun.tea.TeaRequest;
import com.aliyun.tea.TeaRetryableException;
import com.aliyun.tea.TeaUnretryableException;
import com.aliyun.tea.interceptor.InterceptorContext;
import com.aliyun.tea.interceptor.RequestInterceptor;
import com.aliyun.tea.utils.AttributeMap;
import com.aliyun.teaopenapi.models.Config;
import com.aliyun.teautil.models.RuntimeOptions;
import com.sun.net.httpserver.HttpServer;
import com.sun.net.httpserver.HttpHandler;
import com.sun.net.httpserver.HttpExchange;
import org.junit.Test;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.net.InetSocketAddress;
import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.concurrent.atomic.AtomicInteger;

import static org.junit.Assert.*;

public class OSSPrivateUrlTest {
    // 与产品 Client 相同，经跨包子类访问 protected 辅助方法。
    private static class ProductClient extends Client {
        ProductClient(Config config) throws Exception { super(config); }
        String resolve(RuntimeOptions runtime) throws Exception { return getOSSPrivateUrl(runtime); }
        Map<String, ?> upload(String url) throws Exception {
            return upload(url, new RuntimeOptions().setReadTimeout(2000));
        }
        Map<String, ?> upload(String url, RuntimeOptions runtime) throws Exception {
            Map<String, Object> form = new HashMap<String, Object>();
            form.put("host", "oss-cn-hangzhou.aliyuncs.com");
            form.put("OSSAccessKeyId", "test-id");
            form.put("policy", "test-policy");
            form.put("Signature", "test-signature");
            form.put("key", "dir/file.bin");
            form.put("success_action_status", "201");
            com.aliyun.fileform.models.FileField file = new com.aliyun.fileform.models.FileField();
            file.filename = "dir/file.bin";
            file.contentType = "application/octet-stream";
            file.content = new ByteArrayInputStream(new byte[]{0, 1, (byte)255, 10});
            form.put("file", file);
            return postOSSObjectWithURL(url, "test-bucket", form, runtime);
        }
    }

    @Test public void runtimePriorityAndMapping() throws Exception {
        ProductClient client = new ProductClient(new Config().setOssPrivateUrl("http://config.example"));
        assertEquals("http://config.example", client.resolve(new RuntimeOptions()));
        assertEquals("http://config.example", client.resolve(new RuntimeOptions().setOssPrivateUrl("")));
        assertEquals("https://runtime.example", client.resolve(new RuntimeOptions().setOssPrivateUrl("https://runtime.example")));
        assertNull(new ProductClient(new Config()).resolve(new RuntimeOptions()));
        Map<String, Object> values = new HashMap<String, Object>();
        values.put("ossPrivateUrl", "https://pvl.example/proxy");
        assertEquals(values.get("ossPrivateUrl"), Config.build(values).getOssPrivateUrl());
        assertEquals(values.get("ossPrivateUrl"), RuntimeOptions.build(values).getOssPrivateUrl());
    }

    @Test public void pathStyleMultipartXmlAndInvalidUrls() throws Exception {
        final List<String[]> records = Collections.synchronizedList(new ArrayList<String[]>());
        final AtomicInteger status = new AtomicInteger(201);
        // 仅监听回环地址；请求数据在测试线程中断言，避免网关线程吞掉断言。
        HttpServer server = HttpServer.create(new InetSocketAddress("127.0.0.1", 0), 0);
        String host = "127.0.0.1:" + server.getAddress().getPort();
        server.createContext("/", new HttpHandler() {
            public void handle(HttpExchange exchange) throws IOException {
                ByteArrayOutputStream body = new ByteArrayOutputStream();
                byte[] buffer = new byte[1024];
                int n;
                while ((n = exchange.getRequestBody().read(buffer)) != -1) { body.write(buffer, 0, n); }
                records.add(new String[]{exchange.getRequestMethod(), exchange.getRequestHeaders().getFirst("Host"),
                    exchange.getRequestURI().getRawPath(), body.toString("ISO-8859-1")});
                String xml = status.get() == 201 ? "<PostResponse><Key>dir/file.bin</Key></PostResponse>" :
                    "<Error><Code>AccessDenied</Code><Message>denied</Message><RequestId>req-id</RequestId><HostId>host-id</HostId></Error>";
                byte[] bytes = xml.getBytes("UTF-8");
                exchange.getResponseHeaders().set("Content-Type", "application/xml");
                exchange.sendResponseHeaders(status.get(), bytes.length);
                exchange.getResponseBody().write(bytes);
                exchange.close();
            }
        });
        server.start();
        // 既有 Client 共享静态拦截链，测试结束后必须让本测试拦截器失效。
        final AtomicBoolean active = new AtomicBoolean(true);
        try {
            ProductClient client = new ProductClient(new Config());
            final AtomicInteger attempts = new AtomicInteger();
            final TeaRetryableException retryError = new TeaRetryableException(new IOException("测试发送前失败"));
            client.addRequestInterceptor(new RequestInterceptor() {
                public TeaRequest modifyRequest(InterceptorContext context, AttributeMap attributes) {
                    if (active.get() && attempts.incrementAndGet() == 1) { throw retryError; }
                    return context.teaRequest();
                }
            });
            Map<String, ?> retried = client.upload("http://" + host, new RuntimeOptions()
                .setAutoretry(true).setMaxAttempts(1).setBackoffPolicy("fix").setBackoffPeriod(1).setReadTimeout(2000));
            assertEquals("dir/file.bin", ((Map<?, ?>)retried.get("PostResponse")).get("Key"));
            assertEquals(2, attempts.get());
            assertEquals(1, records.size());
            assertTrue(records.get(0)[3].contains(new String(new byte[]{0, 1, (byte)255, 10}, "ISO-8859-1")));
            records.clear();
            for (String prefix : new String[]{"", "/", "/proxy", "/proxy/"}) {
                Map<String, ?> result = client.upload("http://" + host + prefix);
                assertEquals("dir/file.bin", ((Map<?, ?>)result.get("PostResponse")).get("Key"));
                String[] record = records.get(records.size() - 1);
                assertEquals("POST", record[0]);
                assertEquals(host, record[1]);
                assertEquals(prefix.replaceAll("/$", "") + "/test-bucket/", record[2]);
                for (String field : new String[]{"OSSAccessKeyId", "policy", "Signature", "key", "success_action_status", "file"})
                    assertTrue(record[3].contains("name=\"" + field + "\""));
                assertTrue(record[3].contains("test-policy"));
                assertTrue(record[3].contains(new String(new byte[]{0, 1, (byte)255, 10}, "ISO-8859-1")));
            }
            status.set(403);
            try { client.upload("http://" + host); fail("应抛出 OSS 错误"); }
            catch (TeaException error) {
                assertEquals("AccessDenied", error.getCode());
                assertEquals(Integer.valueOf(403), error.getStatusCode());
                assertEquals(403, error.getData().get("httpCode"));
                assertEquals("req-id", error.getData().get("requestId"));
                assertEquals("host-id", error.getData().get("hostId"));
            }
            for (String url : new String[]{null, "", "example.com", "//example.com", "ftp://example.com", "https:///prefix", "http://",
                "http://user@example.com", "http://user:password@example.com", "http://@example.com", "http://example.com/?a=b",
                "http://example.com?", "http://example.com/#fragment", "http://example.com#", "http://example.com:bad", "http://[invalid"}) {
                try { client.upload(url); fail("非法地址不应上传: " + url); }
                catch (TeaException error) { assertEquals("InvalidOSSPrivateUrl", error.getCode()); }
            }
            assertEquals(5, records.size());
        } finally {
            active.set(false);
            server.stop(0);
        }
    }

    @Test public void retryExhaustionPreservesLastRequestAndCause() throws Exception {
        // 请求拦截器在真实 Tea.doAction 链上失败；验证耗尽边界，不访问公网。
        for (boolean autoretry : new boolean[]{false, true}) {
            final AtomicInteger attempts = new AtomicInteger();
            final TeaRetryableException cause = new TeaRetryableException(new IOException("测试重试耗尽"));
            final AtomicBoolean active = new AtomicBoolean(true);
            ProductClient client = new ProductClient(new Config());
            client.addRequestInterceptor(new RequestInterceptor() {
                public TeaRequest modifyRequest(InterceptorContext context, AttributeMap attributes) {
                    if (active.get()) {
                        attempts.incrementAndGet();
                        throw cause;
                    }
                    return context.teaRequest();
                }
            });
            try {
                client.upload("https://pvl.example/proxy", new RuntimeOptions().setAutoretry(autoretry).setMaxAttempts(1));
                fail("应报告重试耗尽");
            } catch (TeaUnretryableException error) {
                assertSame(cause, error.getCause());
                assertEquals("测试重试耗尽", error.getMessage());
                assertEquals(autoretry ? 2 : 1, attempts.get());
                assertEquals("https", error.getLastRequest().protocol);
                assertEquals("pvl.example", error.getLastRequest().headers.get("host"));
                assertEquals("/proxy/test-bucket/", error.getLastRequest().pathname);
            } finally {
                active.set(false);
            }
        }
    }
}
