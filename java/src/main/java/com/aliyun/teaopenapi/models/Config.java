// This file is auto-generated, don't edit it. Thanks.
package com.aliyun.teaopenapi.models;

import com.aliyun.tea.*;

/**
 * Model for initing client
 */
public class Config extends TeaModel {
    /**
     * <p>accesskey id</p>
     */
    @NameInMap("accessKeyId")
    public String accessKeyId;

    /**
     * <p>accesskey secret</p>
     */
    @NameInMap("accessKeySecret")
    public String accessKeySecret;

    /**
     * <p>security token</p>
     */
    @NameInMap("securityToken")
    public String securityToken;

    /**
     * <p>bearer token</p>
     */
    @NameInMap("bearerToken")
    public String bearerToken;

    /**
     * <p>http protocol</p>
     */
    @NameInMap("protocol")
    public String protocol;

    /**
     * <p>http method</p>
     */
    @NameInMap("method")
    public String method;

    /**
     * <p>region id</p>
     */
    @NameInMap("regionId")
    public String regionId;

    /**
     * <p>read timeout</p>
     */
    @NameInMap("readTimeout")
    public Integer readTimeout;

    /**
     * <p>connect timeout</p>
     */
    @NameInMap("connectTimeout")
    public Integer connectTimeout;

    /**
     * <p>http proxy</p>
     */
    @NameInMap("httpProxy")
    public String httpProxy;

    /**
     * <p>https proxy</p>
     */
    @NameInMap("httpsProxy")
    public String httpsProxy;

    /**
     * <p>credential</p>
     */
    @NameInMap("credential")
    public com.aliyun.credentials.Client credential;

    /**
     * <p>endpoint</p>
     */
    @NameInMap("endpoint")
    public String endpoint;

    /**
     * <p>proxy white list</p>
     */
    @NameInMap("noProxy")
    public String noProxy;

    /**
     * <p>max idle conns</p>
     */
    @NameInMap("maxIdleConns")
    public Integer maxIdleConns;

    /**
     * <p>network for endpoint</p>
     */
    @NameInMap("network")
    public String network;

    /**
     * <p>user agent</p>
     */
    @NameInMap("userAgent")
    public String userAgent;

    /**
     * <p>suffix for endpoint</p>
     */
    @NameInMap("suffix")
    public String suffix;

    /**
     * <p>socks5 proxy</p>
     */
    @NameInMap("socks5Proxy")
    public String socks5Proxy;

    /**
     * <p>socks5 network</p>
     */
    @NameInMap("socks5NetWork")
    public String socks5NetWork;

    /**
     * <p>endpoint type</p>
     */
    @NameInMap("endpointType")
    public String endpointType;

    /**
     * <p>OpenPlatform endpoint</p>
     */
    @NameInMap("openPlatformEndpoint")
    public String openPlatformEndpoint;

    /**
     * <p>credential type</p>
     */
    @NameInMap("type")
    @Deprecated
    public String type;

    /**
     * <p>Signature Version</p>
     */
    @NameInMap("signatureVersion")
    public String signatureVersion;

    /**
     * <p>Signature Algorithm</p>
     */
    @NameInMap("signatureAlgorithm")
    public String signatureAlgorithm;

    /**
     * <p>Global Parameters</p>
     */
    @NameInMap("globalParameters")
    public GlobalParameters globalParameters;

    /**
     * <p>privite key for client certificate</p>
     */
    @NameInMap("key")
    public String key;

    /**
     * <p>client certificate</p>
     */
    @NameInMap("cert")
    public String cert;

    /**
     * <p>server certificate</p>
     */
    @NameInMap("ca")
    public String ca;

    /**
     * <p>disable HTTP/2</p>
     */
    @NameInMap("disableHttp2")
    public Boolean disableHttp2;

    public static Config build(java.util.Map<String, ?> map) throws Exception {
        Config self = new Config();
        return TeaModel.build(map, self);
    }

    public Config setAccessKeyId(String accessKeyId) {
        this.accessKeyId = accessKeyId;
        return this;
    }
    public String getAccessKeyId() {
        return this.accessKeyId;
    }

    public Config setAccessKeySecret(String accessKeySecret) {
        this.accessKeySecret = accessKeySecret;
        return this;
    }
    public String getAccessKeySecret() {
        return this.accessKeySecret;
    }

    public Config setSecurityToken(String securityToken) {
        this.securityToken = securityToken;
        return this;
    }
    public String getSecurityToken() {
        return this.securityToken;
    }

    public Config setBearerToken(String bearerToken) {
        this.bearerToken = bearerToken;
        return this;
    }
    public String getBearerToken() {
        return this.bearerToken;
    }

    public Config setProtocol(String protocol) {
        this.protocol = protocol;
        return this;
    }
    public String getProtocol() {
        return this.protocol;
    }

    public Config setMethod(String method) {
        this.method = method;
        return this;
    }
    public String getMethod() {
        return this.method;
    }

    public Config setRegionId(String regionId) {
        this.regionId = regionId;
        return this;
    }
    public String getRegionId() {
        return this.regionId;
    }

    public Config setReadTimeout(Integer readTimeout) {
        this.readTimeout = readTimeout;
        return this;
    }
    public Integer getReadTimeout() {
        return this.readTimeout;
    }

    public Config setConnectTimeout(Integer connectTimeout) {
        this.connectTimeout = connectTimeout;
        return this;
    }
    public Integer getConnectTimeout() {
        return this.connectTimeout;
    }

    public Config setHttpProxy(String httpProxy) {
        this.httpProxy = httpProxy;
        return this;
    }
    public String getHttpProxy() {
        return this.httpProxy;
    }

    public Config setHttpsProxy(String httpsProxy) {
        this.httpsProxy = httpsProxy;
        return this;
    }
    public String getHttpsProxy() {
        return this.httpsProxy;
    }

    public Config setCredential(com.aliyun.credentials.Client credential) {
        this.credential = credential;
        return this;
    }
    public com.aliyun.credentials.Client getCredential() {
        return this.credential;
    }

    public Config setEndpoint(String endpoint) {
        this.endpoint = endpoint;
        return this;
    }
    public String getEndpoint() {
        return this.endpoint;
    }

    public Config setNoProxy(String noProxy) {
        this.noProxy = noProxy;
        return this;
    }
    public String getNoProxy() {
        return this.noProxy;
    }

    public Config setMaxIdleConns(Integer maxIdleConns) {
        this.maxIdleConns = maxIdleConns;
        return this;
    }
    public Integer getMaxIdleConns() {
        return this.maxIdleConns;
    }

    public Config setNetwork(String network) {
        this.network = network;
        return this;
    }
    public String getNetwork() {
        return this.network;
    }

    public Config setUserAgent(String userAgent) {
        this.userAgent = userAgent;
        return this;
    }
    public String getUserAgent() {
        return this.userAgent;
    }

    public Config setSuffix(String suffix) {
        this.suffix = suffix;
        return this;
    }
    public String getSuffix() {
        return this.suffix;
    }

    public Config setSocks5Proxy(String socks5Proxy) {
        this.socks5Proxy = socks5Proxy;
        return this;
    }
    public String getSocks5Proxy() {
        return this.socks5Proxy;
    }

    public Config setSocks5NetWork(String socks5NetWork) {
        this.socks5NetWork = socks5NetWork;
        return this;
    }
    public String getSocks5NetWork() {
        return this.socks5NetWork;
    }

    public Config setEndpointType(String endpointType) {
        this.endpointType = endpointType;
        return this;
    }
    public String getEndpointType() {
        return this.endpointType;
    }

    public Config setOpenPlatformEndpoint(String openPlatformEndpoint) {
        this.openPlatformEndpoint = openPlatformEndpoint;
        return this;
    }
    public String getOpenPlatformEndpoint() {
        return this.openPlatformEndpoint;
    }

    public Config setType(String type) {
        this.type = type;
        return this;
    }
    public String getType() {
        return this.type;
    }

    public Config setSignatureVersion(String signatureVersion) {
        this.signatureVersion = signatureVersion;
        return this;
    }
    public String getSignatureVersion() {
        return this.signatureVersion;
    }

    public Config setSignatureAlgorithm(String signatureAlgorithm) {
        this.signatureAlgorithm = signatureAlgorithm;
        return this;
    }
    public String getSignatureAlgorithm() {
        return this.signatureAlgorithm;
    }

    public Config setGlobalParameters(GlobalParameters globalParameters) {
        this.globalParameters = globalParameters;
        return this;
    }
    public GlobalParameters getGlobalParameters() {
        return this.globalParameters;
    }

    public Config setKey(String key) {
        this.key = key;
        return this;
    }
    public String getKey() {
        return this.key;
    }

    public Config setCert(String cert) {
        this.cert = cert;
        return this;
    }
    public String getCert() {
        return this.cert;
    }

    public Config setCa(String ca) {
        this.ca = ca;
        return this;
    }
    public String getCa() {
        return this.ca;
    }

    public Config setDisableHttp2(Boolean disableHttp2) {
        this.disableHttp2 = disableHttp2;
        return this;
    }
    public Boolean getDisableHttp2() {
        return this.disableHttp2;
    }

}
