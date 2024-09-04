package com.aliyun.teaopenapi.models;

import org.junit.Test;
import org.junit.Assert;

import java.io.ByteArrayInputStream;
import java.io.InputStream;
import java.util.HashMap;
import java.util.Map;

public class OpenApiRequestTest {

    @Test
    public void testSetterGetter() {
        OpenApiRequest request = new OpenApiRequest();
        // Arrange
        Map<String, String> expectedHeaders = new HashMap<>();
        expectedHeaders.put("Content-Type", "application/json");
        expectedHeaders.put("Authorization", "Bearer token");
        request.setHeaders(expectedHeaders);
        Assert.assertEquals(expectedHeaders, request.getHeaders());

        Map<String, String> expectedQuery = new HashMap<>();
        expectedQuery.put("param1", "value1");
        expectedQuery.put("param2", "value2");
        request.setQuery(expectedQuery);
        Assert.assertEquals(expectedQuery, request.getQuery());

        String bodyContent = "{\"key\":\"value\"}";
        Object expectedBody = bodyContent;
        request.setBody(expectedBody);
        Assert.assertEquals(expectedBody, request.getBody());

        InputStream expectedStream = new ByteArrayInputStream("Test Stream Content".getBytes());
        request.setStream(expectedStream);
        Assert.assertEquals(expectedStream, request.getStream());

        Map<String, String> expectedHostMap = new HashMap<>();
        expectedHostMap.put("host1", "value1");
        expectedHostMap.put("host2", "value2");
        request.setHostMap(expectedHostMap);
        Assert.assertEquals(expectedHostMap, request.getHostMap());

        String expectedEndpointOverride = "http://localhost:8080/api";
        request.setEndpointOverride(expectedEndpointOverride);
        Assert.assertEquals(expectedEndpointOverride, request.getEndpointOverride());
    }

}
