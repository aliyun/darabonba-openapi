// This file is auto-generated, don't edit it. Thanks.
package com.aliyun.teaopenapi.models;

import com.aliyun.tea.*;

/**
 * <b>description</b> :
 * <p>Model for initing client</p>
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
     * 
     * <strong>example:</strong>
     * <p>a.txt</p>
     */
    @NameInMap("securityToken")
    public String securityToken;

    /**
     * <p>bearer token</p>
     * 
     * <strong>example:</strong>
     * <p>the-bearer-token</p>
     */
    @NameInMap("bearerToken")
    public String bearerToken;

    /**
     * <p>http protocol</p>
     * 
     * <strong>example:</strong>
     * <p>http</p>
     */
    @NameInMap("protocol")
    public String protocol;

    /**
     * <p>http method</p>
     * 
     * <strong>example:</strong>
     * <p>GET</p>
     */
    @NameInMap("method")
    public String method;

    /**
     * <p>region id</p>
     * 
     * <strong>example:</strong>
     * <p>cn-hangzhou</p>
     */
    @NameInMap("regionId")
    public String regionId;

    /**
     * <p>read timeout</p>
     * 
     * <strong>example:</strong>
     * <p>10</p>
     */
    @NameInMap("readTimeout")
    public Integer readTimeout;

    /**
     * <p>connect timeout</p>
     * 
     * <strong>example:</strong>
     * <p>10</p>
     */
    @NameInMap("connectTimeout")
    public Integer connectTimeout;

    /**
     * <p>http proxy</p>
     * 
     * <strong>example:</strong>
     * <p><a href="http://localhost">http://localhost</a></p>
     */
    @NameInMap("httpProxy")
    public String httpProxy;

    /**
     * <p>https proxy</p>
     * 
     * <strong>example:</strong>
     * <p><a href="https://localhost">https://localhost</a></p>
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
     * 
     * <strong>example:</strong>
     * <p>cs.aliyuncs.com</p>
     */
    @NameInMap("endpoint")
    public String endpoint;

    /**
     * <p>proxy white list</p>
     * 
     * <strong>example:</strong>
     * <p><a href="http://localhost">http://localhost</a></p>
     */
    @NameInMap("noProxy")
    public String noProxy;

    /**
     * <p>max idle conns</p>
     * 
     * <strong>example:</strong>
     * <p>3</p>
     */
    @NameInMap("maxIdleConns")
    public Integer maxIdleConns;

    /**
     * <p>network for endpoint</p>
     * 
     * <strong>example:</strong>
     * <p>public</p>
     */
    @NameInMap("network")
    public String network;

    /**
     * <p>user agent</p>
     * 
     * <strong>example:</strong>
     * <p>Alibabacloud/1</p>
     */
    @NameInMap("userAgent")
    public String userAgent;

    /**
     * <p>suffix for endpoint</p>
     * 
     * <strong>example:</strong>
     * <p>aliyun</p>
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
     * 
     * <strong>example:</strong>
     * <p>TCP</p>
     */
    @NameInMap("socks5NetWork")
    public String socks5NetWork;

    /**
     * <p>endpoint type</p>
     * 
     * <strong>example:</strong>
     * <p>internal</p>
     */
    @NameInMap("endpointType")
    public String endpointType;

    /**
     * <p>OpenPlatform endpoint</p>
     * 
     * <strong>example:</strong>
     * <p>openplatform.aliyuncs.com</p>
     */
    @NameInMap("openPlatformEndpoint")
    public String openPlatformEndpoint;

    /**
     * <p>credential type</p>
     * 
     * <strong>example:</strong>
     * <p>access_key</p>
     */
    @NameInMap("type")
    @Deprecated
    public String type;

    /**
     * <p>Signature Version</p>
     * 
     * <strong>example:</strong>
     * <p>v1</p>
     */
    @NameInMap("signatureVersion")
    public String signatureVersion;

    /**
     * <p>Signature Algorithm</p>
     * 
     * <strong>example:</strong>
     * <p>ACS3-HMAC-SHA256</p>
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
     * 
     * <strong>example:</strong>
     * <p>MIIEvQ</p>
     */
    @NameInMap("key")
    public String key;

    /**
     * <p>client certificate</p>
     * 
     * <strong>example:</strong>
     * <p>-----BEGIN CERTIFICATE-----
     * xxx-----END CERTIFICATE-----</p>
     */
    @NameInMap("cert")
    public String cert;

    /**
     * <p>server certificate</p>
     * 
     * <strong>example:</strong>
     * <p>-----BEGIN CERTIFICATE-----
     * xxx-----END CERTIFICATE-----</p>
     */
    @NameInMap("ca")
    public String ca;

    /**
     * <p>disable HTTP/2</p>
     * 
     * <strong>example:</strong>
     * <p>false</p>
     */
    @NameInMap("disableHttp2")
    public Boolean disableHttp2;

    /**
     * <p>TLS Minimum Version</p>
     * 
     * <strong>example:</strong>
     * <p>TLSv1, TLSv1.1, TLSv1.2, TLSv1.3</p>
     */
    @NameInMap("tlsMinVersion")
    public String tlsMinVersion;

    /**
     * <p>Enable usage data collection. If true, it means that you are aware of and confirm your authorization to Alibaba Cloud to collect your machine information. Currently, only the hostname is collected to obtain your call information.</p>
     */
    @NameInMap("enableUsageDataCollection")
    public Boolean enableUsageDataCollection;

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

    @Deprecated
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

    public Config setTlsMinVersion(String tlsMinVersion) {
        this.tlsMinVersion = tlsMinVersion;
        return this;
    }
    public String getTlsMinVersion() {
        return this.tlsMinVersion;
    }

    public Config setEnableUsageDataCollection(Boolean enableUsageDataCollection) {
        this.enableUsageDataCollection = enableUsageDataCollection;
        return this;
    }
    public Boolean getEnableUsageDataCollection() {
        return this.enableUsageDataCollection;
    }

}
