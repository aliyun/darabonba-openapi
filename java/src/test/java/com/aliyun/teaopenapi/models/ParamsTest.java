package com.aliyun.teaopenapi.models;

import org.junit.Test;
import org.junit.Assert;

public class ParamsTest {

    @Test
    public void testSetterGetter() {
        Params params = new Params();
        params.setAction("testAction");
        Assert.assertEquals("testAction", params.getAction());
        params.setVersion("1.0");
        Assert.assertEquals("1.0", params.getVersion());
        params.setProtocol("https");
        Assert.assertEquals("https", params.getProtocol());
        params.setPathname("/path/to/resource");
        Assert.assertEquals("/path/to/resource", params.getPathname());
        params.setMethod("GET");
        Assert.assertEquals("GET", params.getMethod());
        params.setAuthType("AK");
        Assert.assertEquals("AK", params.getAuthType());
        params.setBodyType("json");
        Assert.assertEquals("json", params.getBodyType());
        params.setReqBodyType("json");
        Assert.assertEquals("json", params.getReqBodyType());
        params.setStyle("ROA");
        Assert.assertEquals("ROA", params.getStyle());
    }
}
