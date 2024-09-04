package com.aliyun.teaopenapi.models;

import org.junit.Test;
import org.junit.Assert;

import java.util.HashMap;
import java.util.Map;

public class GlobalParametersTest {

    @Test
    public void testSetterGetter() {
        GlobalParameters globalParameters = new GlobalParameters();
        Map<String, String> expectedHeaders = new HashMap<>();
        expectedHeaders.put("Content-Type", "application/json");
        expectedHeaders.put("Authorization", "Bearer token");
        globalParameters.setHeaders(expectedHeaders);
        Assert.assertEquals(expectedHeaders, globalParameters.getHeaders());

        Map<String, String> expectedQueries = new HashMap<>();
        expectedQueries.put("param1", "value1");
        expectedQueries.put("param2", "value2");
        globalParameters.setQueries(expectedQueries);
        Assert.assertEquals(expectedQueries, globalParameters.getQueries());
    }
}
