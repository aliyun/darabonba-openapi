package com.aliyun.teaopenapi.models;

import org.junit.Assert;
import org.junit.Test;

public class ConfigTest {

    @Test
    public void testSetterGetter() {
        Config config = new Config();
        config.setAccessKeyId("test");
        Assert.assertEquals("test", config.getAccessKeyId());
        config.setAccessKeySecret("test");
        Assert.assertEquals("test", config.getAccessKeySecret());
        config.setSecurityToken("test");
        Assert.assertEquals("test", config.getSecurityToken());
        config.setBearerToken("test");
        Assert.assertEquals("test", config.getBearerToken());
        config.setProtocol("test");
        Assert.assertEquals("test", config.getProtocol());
        config.setMethod("test");
        Assert.assertEquals("test", config.getMethod());
        config.setRegionId("test");
        Assert.assertEquals("test", config.getRegionId());
        config.setReadTimeout(100);
        Assert.assertEquals(100, (int) config.getReadTimeout());
        config.setConnectTimeout(100);
        Assert.assertEquals(100, (int) config.getConnectTimeout());
        config.setHttpProxy("test");
        Assert.assertEquals("test", config.getHttpProxy());
        config.setEndpoint("test");
        Assert.assertEquals("test", config.getEndpoint());
        config.setHttpsProxy("test");
        Assert.assertEquals("test", config.getHttpsProxy());
        com.aliyun.credentials.Client credential = new com.aliyun.credentials.Client(); // 假设Client类有默认构造函数
        config.setCredential(credential);
        Assert.assertNotNull(config.getCredential());
        config.setEndpoint("test");
        Assert.assertEquals("test", config.getEndpoint());
        config.setNoProxy("test");
        Assert.assertEquals("test", config.getNoProxy());
        config.setMaxIdleConns(100);
        Assert.assertEquals(100, (int) config.getMaxIdleConns());
        config.setNetwork("test");
        Assert.assertEquals("test", config.getNetwork());
        config.setUserAgent("test");
        Assert.assertEquals("test", config.getUserAgent());
        config.setSuffix("test");
        Assert.assertEquals("test", config.getSuffix());
        config.setSocks5Proxy("test");
        Assert.assertEquals("test", config.getSocks5Proxy());
        config.setSocks5NetWork("test");
        Assert.assertEquals("test", config.getSocks5NetWork());
        config.setEndpointType("test");
        Assert.assertEquals("test", config.getEndpointType());
        config.setOpenPlatformEndpoint("test");
        Assert.assertEquals("test", config.getOpenPlatformEndpoint());
        config.setType("test");
        Assert.assertEquals("test", config.getType());
        config.setSignatureVersion("test");
        Assert.assertEquals("test", config.getSignatureVersion());
        config.setSignatureAlgorithm("test");
        Assert.assertEquals("test", config.getSignatureAlgorithm());
        GlobalParameters globalParameters = new GlobalParameters();
        config.setGlobalParameters(globalParameters);
        Assert.assertNotNull(config.getGlobalParameters());
        config.setKey("test");
        Assert.assertEquals("test", config.getKey());
        config.setCert("test");
        Assert.assertEquals("test", config.getCert());
        config.setCa("test");
        Assert.assertEquals("test", config.getCa());
        config.setDisableHttp2(true);
        Assert.assertTrue(config.getDisableHttp2());
        config.setTlsMinVersion("TLSv1.2");
        Assert.assertEquals("TLSv1.2", config.getTlsMinVersion());
    }
}
