// This file is auto-generated, don't edit it. Thanks.
package com.aliyun.teaopenapi;

import com.aliyun.tea.*;
import com.aliyun.tea.interceptor.InterceptorChain;
import com.aliyun.tea.interceptor.RuntimeOptionsInterceptor;
import com.aliyun.tea.interceptor.RequestInterceptor;
import com.aliyun.tea.interceptor.ResponseInterceptor;
import com.aliyun.teaopenapi.models.*;
import com.aliyun.teautil.*;
import com.aliyun.teautil.models.*;
import com.aliyun.credentials.*;
import com.aliyun.credentials.models.*;
import com.aliyun.openapiutil.*;
import com.aliyun.gateway.spi.*;
import com.aliyun.gateway.spi.models.*;

public class Client {

    private final static InterceptorChain interceptorChain = InterceptorChain.create();

    public String _endpoint;
    public String _regionId;
    public String _protocol;
    public String _method;
    public String _userAgent;
    public String _endpointRule;
    public java.util.Map<String, String> _endpointMap;
    public String _suffix;
    public Integer _readTimeout;
    public Integer _connectTimeout;
    public String _httpProxy;
    public String _httpsProxy;
    public String _socks5Proxy;
    public String _socks5NetWork;
    public String _noProxy;
    public String _network;
    public String _productId;
    public Integer _maxIdleConns;
    public String _endpointType;
    public String _openPlatformEndpoint;
    public com.aliyun.credentials.Client _credential;
    public String _signatureVersion;
    public String _signatureAlgorithm;
    public java.util.Map<String, String> _headers;
    public com.aliyun.gateway.spi.Client _spi;
    public GlobalParameters _globalParameters;
    /**
     * Init client with Config
     * @param config config contains the necessary information to create a client
     */
    public Client(com.aliyun.teaopenapi.models.Config config) throws Exception {
        if (com.aliyun.teautil.Common.isUnset(TeaModel.buildMap(config))) {
            throw new TeaException(TeaConverter.buildMap(
                new TeaPair("code", "ParameterMissing"),
                new TeaPair("message", "'config' can not be unset")
            ));
        }

        if (!com.aliyun.teautil.Common.empty(config.accessKeyId) && !com.aliyun.teautil.Common.empty(config.accessKeySecret)) {
            if (!com.aliyun.teautil.Common.empty(config.securityToken)) {
                config.type = "sts";
            } else {
                config.type = "access_key";
            }

            com.aliyun.credentials.models.Config credentialConfig = com.aliyun.credentials.models.Config.build(TeaConverter.buildMap(
                new TeaPair("accessKeyId", config.accessKeyId),
                new TeaPair("type", config.type),
                new TeaPair("accessKeySecret", config.accessKeySecret),
                new TeaPair("securityToken", config.securityToken)
            ));
            this._credential = new com.aliyun.credentials.Client(credentialConfig);
        } else if (!com.aliyun.teautil.Common.isUnset(config.credential)) {
            this._credential = config.credential;
        }

        this._endpoint = config.endpoint;
        this._endpointType = config.endpointType;
        this._network = config.network;
        this._suffix = config.suffix;
        this._protocol = config.protocol;
        this._method = config.method;
        this._regionId = config.regionId;
        this._userAgent = config.userAgent;
        this._readTimeout = config.readTimeout;
        this._connectTimeout = config.connectTimeout;
        this._httpProxy = config.httpProxy;
        this._httpsProxy = config.httpsProxy;
        this._noProxy = config.noProxy;
        this._socks5Proxy = config.socks5Proxy;
        this._socks5NetWork = config.socks5NetWork;
        this._maxIdleConns = config.maxIdleConns;
        this._signatureVersion = config.signatureVersion;
        this._signatureAlgorithm = config.signatureAlgorithm;
        this._globalParameters = config.globalParameters;
    }

    public java.util.Map<String, ?> doRPCRequest(String action, String version, String protocol, String method, String authType, String bodyType, OpenApiRequest request, RuntimeOptions runtime) throws Exception {
        TeaModel.validateParams(request, "request");
        java.util.Map<String, Object> runtime_ = TeaConverter.buildMap(
            new TeaPair("timeouted", "retry"),
            new TeaPair("readTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.readTimeout, _readTimeout)),
            new TeaPair("connectTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.connectTimeout, _connectTimeout)),
            new TeaPair("httpProxy", com.aliyun.teautil.Common.defaultString(runtime.httpProxy, _httpProxy)),
            new TeaPair("httpsProxy", com.aliyun.teautil.Common.defaultString(runtime.httpsProxy, _httpsProxy)),
            new TeaPair("noProxy", com.aliyun.teautil.Common.defaultString(runtime.noProxy, _noProxy)),
            new TeaPair("socks5Proxy", com.aliyun.teautil.Common.defaultString(runtime.socks5Proxy, _socks5Proxy)),
            new TeaPair("socks5NetWork", com.aliyun.teautil.Common.defaultString(runtime.socks5NetWork, _socks5NetWork)),
            new TeaPair("maxIdleConns", com.aliyun.teautil.Common.defaultNumber(runtime.maxIdleConns, _maxIdleConns)),
            new TeaPair("retry", TeaConverter.buildMap(
                new TeaPair("retryable", runtime.autoretry),
                new TeaPair("maxAttempts", com.aliyun.teautil.Common.defaultNumber(runtime.maxAttempts, 3))
            )),
            new TeaPair("backoff", TeaConverter.buildMap(
                new TeaPair("policy", com.aliyun.teautil.Common.defaultString(runtime.backoffPolicy, "no")),
                new TeaPair("period", com.aliyun.teautil.Common.defaultNumber(runtime.backoffPeriod, 1))
            )),
            new TeaPair("ignoreSSL", runtime.ignoreSSL),
            new TeaPair("keepAlive", runtime.keepAlive)
        );

        TeaRequest _lastRequest = null;
        Exception _lastException = null;
        TeaResponse _lastResponse = null;
        long _now = System.currentTimeMillis();
        int _retryTimes = 0;
        while (Tea.allowRetry((java.util.Map<String, Object>) runtime_.get("retry"), _retryTimes, _now)) {
            if (_retryTimes > 0) {
                int backoffTime = Tea.getBackoffTime(runtime_.get("backoff"), _retryTimes);
                if (backoffTime > 0) {
                    Tea.sleep(backoffTime);
                }
            }
            _retryTimes = _retryTimes + 1;
            try {
                TeaRequest request_ = new TeaRequest();
                request_.protocol = com.aliyun.teautil.Common.defaultString(_protocol, protocol);
                request_.method = method;
                request_.pathname = "/";
                request_.query = TeaConverter.merge(String.class,
                    TeaConverter.buildMap(
                        new TeaPair("Action", action),
                        new TeaPair("Format", "json"),
                        new TeaPair("Version", version),
                        new TeaPair("Timestamp", com.aliyun.openapiutil.Client.getTimestamp()),
                        new TeaPair("SignatureNonce", com.aliyun.teautil.Common.getNonce())
                    ),
                    request.query
                );
                java.util.Map<String, String> headers = this.getRpcHeaders();
                if (com.aliyun.teautil.Common.isUnset(headers)) {
                    // endpoint is setted in product client
                    request_.headers = TeaConverter.buildMap(
                        new TeaPair("host", _endpoint),
                        new TeaPair("x-acs-version", version),
                        new TeaPair("x-acs-action", action),
                        new TeaPair("user-agent", this.getUserAgent())
                    );
                } else {
                    request_.headers = TeaConverter.merge(String.class,
                        TeaConverter.buildMap(
                            new TeaPair("host", _endpoint),
                            new TeaPair("x-acs-version", version),
                            new TeaPair("x-acs-action", action),
                            new TeaPair("user-agent", this.getUserAgent())
                        ),
                        headers
                    );
                }

                if (!com.aliyun.teautil.Common.isUnset(request.body)) {
                    java.util.Map<String, Object> m = com.aliyun.teautil.Common.assertAsMap(request.body);
                    java.util.Map<String, Object> tmp = com.aliyun.teautil.Common.anyifyMapValue(com.aliyun.openapiutil.Client.query(m));
                    request_.body = Tea.toReadable(com.aliyun.teautil.Common.toFormString(tmp));
                    request_.headers.put("content-type", "application/x-www-form-urlencoded");
                }

                if (!com.aliyun.teautil.Common.equalString(authType, "Anonymous")) {
                    String accessKeyId = this.getAccessKeyId();
                    String accessKeySecret = this.getAccessKeySecret();
                    String securityToken = this.getSecurityToken();
                    if (!com.aliyun.teautil.Common.empty(securityToken)) {
                        request_.query.put("SecurityToken", securityToken);
                    }

                    request_.query.put("SignatureMethod", "HMAC-SHA1");
                    request_.query.put("SignatureVersion", "1.0");
                    request_.query.put("AccessKeyId", accessKeyId);
                    java.util.Map<String, Object> t = null;
                    if (!com.aliyun.teautil.Common.isUnset(request.body)) {
                        t = com.aliyun.teautil.Common.assertAsMap(request.body);
                    }

                    java.util.Map<String, String> signedParam = TeaConverter.merge(String.class,
                        request_.query,
                        com.aliyun.openapiutil.Client.query(t)
                    );
                    request_.query.put("Signature", com.aliyun.openapiutil.Client.getRPCSignature(signedParam, request_.method, accessKeySecret));
                }

                _lastRequest = request_;
                TeaResponse response_ = Tea.doAction(request_, runtime_, interceptorChain);
                _lastResponse = response_;

                if (com.aliyun.teautil.Common.is4xx(response_.statusCode) || com.aliyun.teautil.Common.is5xx(response_.statusCode)) {
                    Object _res = com.aliyun.teautil.Common.readAsJSON(response_.body);
                    java.util.Map<String, Object> err = com.aliyun.teautil.Common.assertAsMap(_res);
                    Object requestId = Client.defaultAny(err.get("RequestId"), err.get("requestId"));
                    throw new TeaException(TeaConverter.buildMap(
                        new TeaPair("code", "" + Client.defaultAny(err.get("Code"), err.get("code")) + ""),
                        new TeaPair("message", "code: " + response_.statusCode + ", " + Client.defaultAny(err.get("Message"), err.get("message")) + " request id: " + requestId + ""),
                        new TeaPair("data", err)
                    ));
                }

                if (com.aliyun.teautil.Common.equalString(bodyType, "binary")) {
                    java.util.Map<String, Object> resp = TeaConverter.buildMap(
                        new TeaPair("body", response_.body),
                        new TeaPair("headers", response_.headers)
                    );
                    return resp;
                } else if (com.aliyun.teautil.Common.equalString(bodyType, "byte")) {
                    byte[] byt = com.aliyun.teautil.Common.readAsBytes(response_.body);
                    return TeaConverter.buildMap(
                        new TeaPair("body", byt),
                        new TeaPair("headers", response_.headers)
                    );
                } else if (com.aliyun.teautil.Common.equalString(bodyType, "string")) {
                    String str = com.aliyun.teautil.Common.readAsString(response_.body);
                    return TeaConverter.buildMap(
                        new TeaPair("body", str),
                        new TeaPair("headers", response_.headers)
                    );
                } else if (com.aliyun.teautil.Common.equalString(bodyType, "json")) {
                    Object obj = com.aliyun.teautil.Common.readAsJSON(response_.body);
                    java.util.Map<String, Object> res = com.aliyun.teautil.Common.assertAsMap(obj);
                    return TeaConverter.buildMap(
                        new TeaPair("body", res),
                        new TeaPair("headers", response_.headers)
                    );
                } else if (com.aliyun.teautil.Common.equalString(bodyType, "array")) {
                    Object arr = com.aliyun.teautil.Common.readAsJSON(response_.body);
                    return TeaConverter.buildMap(
                        new TeaPair("body", arr),
                        new TeaPair("headers", response_.headers)
                    );
                } else {
                    return TeaConverter.buildMap(
                        new TeaPair("headers", response_.headers)
                    );
                }

            } catch (Exception e) {
                if (Tea.isRetryable(e)) {
                    _lastException = e;
                    continue;
                }
                throw e;
            } finally {
                if (!com.aliyun.teautil.Common.isUnset(_lastResponse)
                        && !com.aliyun.teautil.Common.isUnset(_lastResponse.response)
                        && !com.aliyun.teautil.Common.isUnset(_lastResponse.response.body())){
                    _lastResponse.response.close();
                }
            }
        }
        throw new TeaUnretryableException(_lastRequest, _lastException);
    }

    public java.util.Map<String, ?> doROARequest(String action, String version, String protocol, String method, String authType, String pathname, String bodyType, OpenApiRequest request, RuntimeOptions runtime) throws Exception {
        TeaModel.validateParams(request, "request");
        java.util.Map<String, Object> runtime_ = TeaConverter.buildMap(
            new TeaPair("timeouted", "retry"),
            new TeaPair("readTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.readTimeout, _readTimeout)),
            new TeaPair("connectTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.connectTimeout, _connectTimeout)),
            new TeaPair("httpProxy", com.aliyun.teautil.Common.defaultString(runtime.httpProxy, _httpProxy)),
            new TeaPair("httpsProxy", com.aliyun.teautil.Common.defaultString(runtime.httpsProxy, _httpsProxy)),
            new TeaPair("noProxy", com.aliyun.teautil.Common.defaultString(runtime.noProxy, _noProxy)),
            new TeaPair("socks5Proxy", com.aliyun.teautil.Common.defaultString(runtime.socks5Proxy, _socks5Proxy)),
            new TeaPair("socks5NetWork", com.aliyun.teautil.Common.defaultString(runtime.socks5NetWork, _socks5NetWork)),
            new TeaPair("maxIdleConns", com.aliyun.teautil.Common.defaultNumber(runtime.maxIdleConns, _maxIdleConns)),
            new TeaPair("retry", TeaConverter.buildMap(
                new TeaPair("retryable", runtime.autoretry),
                new TeaPair("maxAttempts", com.aliyun.teautil.Common.defaultNumber(runtime.maxAttempts, 3))
            )),
            new TeaPair("backoff", TeaConverter.buildMap(
                new TeaPair("policy", com.aliyun.teautil.Common.defaultString(runtime.backoffPolicy, "no")),
                new TeaPair("period", com.aliyun.teautil.Common.defaultNumber(runtime.backoffPeriod, 1))
            )),
            new TeaPair("ignoreSSL", runtime.ignoreSSL),
            new TeaPair("keepAlive", runtime.keepAlive)
        );

        TeaRequest _lastRequest = null;
        Exception _lastException = null;
        TeaResponse _lastResponse = null;
        long _now = System.currentTimeMillis();
        int _retryTimes = 0;
        while (Tea.allowRetry((java.util.Map<String, Object>) runtime_.get("retry"), _retryTimes, _now)) {
            if (_retryTimes > 0) {
                int backoffTime = Tea.getBackoffTime(runtime_.get("backoff"), _retryTimes);
                if (backoffTime > 0) {
                    Tea.sleep(backoffTime);
                }
            }
            _retryTimes = _retryTimes + 1;
            try {
                TeaRequest request_ = new TeaRequest();
                request_.protocol = com.aliyun.teautil.Common.defaultString(_protocol, protocol);
                request_.method = method;
                request_.pathname = pathname;
                request_.headers = TeaConverter.merge(String.class,
                    TeaConverter.buildMap(
                        new TeaPair("date", com.aliyun.teautil.Common.getDateUTCString()),
                        new TeaPair("host", _endpoint),
                        new TeaPair("accept", "application/json"),
                        new TeaPair("x-acs-signature-nonce", com.aliyun.teautil.Common.getNonce()),
                        new TeaPair("x-acs-signature-method", "HMAC-SHA1"),
                        new TeaPair("x-acs-signature-version", "1.0"),
                        new TeaPair("x-acs-version", version),
                        new TeaPair("x-acs-action", action),
                        new TeaPair("user-agent", com.aliyun.teautil.Common.getUserAgent(_userAgent))
                    ),
                    request.headers
                );
                if (!com.aliyun.teautil.Common.isUnset(request.body)) {
                    request_.body = Tea.toReadable(com.aliyun.teautil.Common.toJSONString(request.body));
                    request_.headers.put("content-type", "application/json; charset=utf-8");
                }

                if (!com.aliyun.teautil.Common.isUnset(request.query)) {
                    request_.query = request.query;
                }

                if (!com.aliyun.teautil.Common.equalString(authType, "Anonymous")) {
                    String accessKeyId = this.getAccessKeyId();
                    String accessKeySecret = this.getAccessKeySecret();
                    String securityToken = this.getSecurityToken();
                    if (!com.aliyun.teautil.Common.empty(securityToken)) {
                        request_.headers.put("x-acs-accesskey-id", accessKeyId);
                        request_.headers.put("x-acs-security-token", securityToken);
                    }

                    String stringToSign = com.aliyun.openapiutil.Client.getStringToSign(request_);
                    request_.headers.put("authorization", "acs " + accessKeyId + ":" + com.aliyun.openapiutil.Client.getROASignature(stringToSign, accessKeySecret) + "");
                }

                _lastRequest = request_;
                TeaResponse response_ = Tea.doAction(request_, runtime_, interceptorChain);
                _lastResponse = response_;

                if (com.aliyun.teautil.Common.equalNumber(response_.statusCode, 204)) {
                    return TeaConverter.buildMap(
                        new TeaPair("headers", response_.headers)
                    );
                }

                if (com.aliyun.teautil.Common.is4xx(response_.statusCode) || com.aliyun.teautil.Common.is5xx(response_.statusCode)) {
                    Object _res = com.aliyun.teautil.Common.readAsJSON(response_.body);
                    java.util.Map<String, Object> err = com.aliyun.teautil.Common.assertAsMap(_res);
                    Object requestId = Client.defaultAny(err.get("RequestId"), err.get("requestId"));
                    requestId = Client.defaultAny(requestId, err.get("requestid"));
                    throw new TeaException(TeaConverter.buildMap(
                        new TeaPair("code", "" + Client.defaultAny(err.get("Code"), err.get("code")) + ""),
                        new TeaPair("message", "code: " + response_.statusCode + ", " + Client.defaultAny(err.get("Message"), err.get("message")) + " request id: " + requestId + ""),
                        new TeaPair("data", err)
                    ));
                }

                if (com.aliyun.teautil.Common.equalString(bodyType, "binary")) {
                    java.util.Map<String, Object> resp = TeaConverter.buildMap(
                        new TeaPair("body", response_.body),
                        new TeaPair("headers", response_.headers)
                    );
                    return resp;
                } else if (com.aliyun.teautil.Common.equalString(bodyType, "byte")) {
                    byte[] byt = com.aliyun.teautil.Common.readAsBytes(response_.body);
                    return TeaConverter.buildMap(
                        new TeaPair("body", byt),
                        new TeaPair("headers", response_.headers)
                    );
                } else if (com.aliyun.teautil.Common.equalString(bodyType, "string")) {
                    String str = com.aliyun.teautil.Common.readAsString(response_.body);
                    return TeaConverter.buildMap(
                        new TeaPair("body", str),
                        new TeaPair("headers", response_.headers)
                    );
                } else if (com.aliyun.teautil.Common.equalString(bodyType, "json")) {
                    Object obj = com.aliyun.teautil.Common.readAsJSON(response_.body);
                    java.util.Map<String, Object> res = com.aliyun.teautil.Common.assertAsMap(obj);
                    return TeaConverter.buildMap(
                        new TeaPair("body", res),
                        new TeaPair("headers", response_.headers)
                    );
                } else if (com.aliyun.teautil.Common.equalString(bodyType, "array")) {
                    Object arr = com.aliyun.teautil.Common.readAsJSON(response_.body);
                    return TeaConverter.buildMap(
                        new TeaPair("body", arr),
                        new TeaPair("headers", response_.headers)
                    );
                } else {
                    return TeaConverter.buildMap(
                        new TeaPair("headers", response_.headers)
                    );
                }

            } catch (Exception e) {
                if (Tea.isRetryable(e)) {
                    _lastException = e;
                    continue;
                }
                throw e;
            } finally {
                if (!com.aliyun.teautil.Common.isUnset(_lastResponse)
                        && !com.aliyun.teautil.Common.isUnset(_lastResponse.response)
                        && !com.aliyun.teautil.Common.isUnset(_lastResponse.response.body())){
                    _lastResponse.response.close();
                }
            }
        }
        throw new TeaUnretryableException(_lastRequest, _lastException);
    }

    public java.util.Map<String, ?> doROARequestWithForm(String action, String version, String protocol, String method, String authType, String pathname, String bodyType, OpenApiRequest request, RuntimeOptions runtime) throws Exception {
        TeaModel.validateParams(request, "request");
        java.util.Map<String, Object> runtime_ = TeaConverter.buildMap(
            new TeaPair("timeouted", "retry"),
            new TeaPair("readTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.readTimeout, _readTimeout)),
            new TeaPair("connectTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.connectTimeout, _connectTimeout)),
            new TeaPair("httpProxy", com.aliyun.teautil.Common.defaultString(runtime.httpProxy, _httpProxy)),
            new TeaPair("httpsProxy", com.aliyun.teautil.Common.defaultString(runtime.httpsProxy, _httpsProxy)),
            new TeaPair("noProxy", com.aliyun.teautil.Common.defaultString(runtime.noProxy, _noProxy)),
            new TeaPair("socks5Proxy", com.aliyun.teautil.Common.defaultString(runtime.socks5Proxy, _socks5Proxy)),
            new TeaPair("socks5NetWork", com.aliyun.teautil.Common.defaultString(runtime.socks5NetWork, _socks5NetWork)),
            new TeaPair("maxIdleConns", com.aliyun.teautil.Common.defaultNumber(runtime.maxIdleConns, _maxIdleConns)),
            new TeaPair("retry", TeaConverter.buildMap(
                new TeaPair("retryable", runtime.autoretry),
                new TeaPair("maxAttempts", com.aliyun.teautil.Common.defaultNumber(runtime.maxAttempts, 3))
            )),
            new TeaPair("backoff", TeaConverter.buildMap(
                new TeaPair("policy", com.aliyun.teautil.Common.defaultString(runtime.backoffPolicy, "no")),
                new TeaPair("period", com.aliyun.teautil.Common.defaultNumber(runtime.backoffPeriod, 1))
            )),
            new TeaPair("ignoreSSL", runtime.ignoreSSL),
            new TeaPair("keepAlive", runtime.keepAlive)
        );

        TeaRequest _lastRequest = null;
        Exception _lastException = null;
        TeaResponse _lastResponse = null;
        long _now = System.currentTimeMillis();
        int _retryTimes = 0;
        while (Tea.allowRetry((java.util.Map<String, Object>) runtime_.get("retry"), _retryTimes, _now)) {
            if (_retryTimes > 0) {
                int backoffTime = Tea.getBackoffTime(runtime_.get("backoff"), _retryTimes);
                if (backoffTime > 0) {
                    Tea.sleep(backoffTime);
                }
            }
            _retryTimes = _retryTimes + 1;
            try {
                TeaRequest request_ = new TeaRequest();
                request_.protocol = com.aliyun.teautil.Common.defaultString(_protocol, protocol);
                request_.method = method;
                request_.pathname = pathname;
                request_.headers = TeaConverter.merge(String.class,
                    TeaConverter.buildMap(
                        new TeaPair("date", com.aliyun.teautil.Common.getDateUTCString()),
                        new TeaPair("host", _endpoint),
                        new TeaPair("accept", "application/json"),
                        new TeaPair("x-acs-signature-nonce", com.aliyun.teautil.Common.getNonce()),
                        new TeaPair("x-acs-signature-method", "HMAC-SHA1"),
                        new TeaPair("x-acs-signature-version", "1.0"),
                        new TeaPair("x-acs-version", version),
                        new TeaPair("x-acs-action", action),
                        new TeaPair("user-agent", com.aliyun.teautil.Common.getUserAgent(_userAgent))
                    ),
                    request.headers
                );
                if (!com.aliyun.teautil.Common.isUnset(request.body)) {
                    java.util.Map<String, Object> m = com.aliyun.teautil.Common.assertAsMap(request.body);
                    request_.body = Tea.toReadable(com.aliyun.openapiutil.Client.toForm(m));
                    request_.headers.put("content-type", "application/x-www-form-urlencoded");
                }

                if (!com.aliyun.teautil.Common.isUnset(request.query)) {
                    request_.query = request.query;
                }

                if (!com.aliyun.teautil.Common.equalString(authType, "Anonymous")) {
                    String accessKeyId = this.getAccessKeyId();
                    String accessKeySecret = this.getAccessKeySecret();
                    String securityToken = this.getSecurityToken();
                    if (!com.aliyun.teautil.Common.empty(securityToken)) {
                        request_.headers.put("x-acs-accesskey-id", accessKeyId);
                        request_.headers.put("x-acs-security-token", securityToken);
                    }

                    String stringToSign = com.aliyun.openapiutil.Client.getStringToSign(request_);
                    request_.headers.put("authorization", "acs " + accessKeyId + ":" + com.aliyun.openapiutil.Client.getROASignature(stringToSign, accessKeySecret) + "");
                }

                _lastRequest = request_;
                TeaResponse response_ = Tea.doAction(request_, runtime_, interceptorChain);
                _lastResponse = response_;

                if (com.aliyun.teautil.Common.equalNumber(response_.statusCode, 204)) {
                    return TeaConverter.buildMap(
                        new TeaPair("headers", response_.headers)
                    );
                }

                if (com.aliyun.teautil.Common.is4xx(response_.statusCode) || com.aliyun.teautil.Common.is5xx(response_.statusCode)) {
                    Object _res = com.aliyun.teautil.Common.readAsJSON(response_.body);
                    java.util.Map<String, Object> err = com.aliyun.teautil.Common.assertAsMap(_res);
                    throw new TeaException(TeaConverter.buildMap(
                        new TeaPair("code", "" + Client.defaultAny(err.get("Code"), err.get("code")) + ""),
                        new TeaPair("message", "code: " + response_.statusCode + ", " + Client.defaultAny(err.get("Message"), err.get("message")) + " request id: " + Client.defaultAny(err.get("RequestId"), err.get("requestId")) + ""),
                        new TeaPair("data", err)
                    ));
                }

                if (com.aliyun.teautil.Common.equalString(bodyType, "binary")) {
                    java.util.Map<String, Object> resp = TeaConverter.buildMap(
                        new TeaPair("body", response_.body),
                        new TeaPair("headers", response_.headers)
                    );
                    return resp;
                } else if (com.aliyun.teautil.Common.equalString(bodyType, "byte")) {
                    byte[] byt = com.aliyun.teautil.Common.readAsBytes(response_.body);
                    return TeaConverter.buildMap(
                        new TeaPair("body", byt),
                        new TeaPair("headers", response_.headers)
                    );
                } else if (com.aliyun.teautil.Common.equalString(bodyType, "string")) {
                    String str = com.aliyun.teautil.Common.readAsString(response_.body);
                    return TeaConverter.buildMap(
                        new TeaPair("body", str),
                        new TeaPair("headers", response_.headers)
                    );
                } else if (com.aliyun.teautil.Common.equalString(bodyType, "json")) {
                    Object obj = com.aliyun.teautil.Common.readAsJSON(response_.body);
                    java.util.Map<String, Object> res = com.aliyun.teautil.Common.assertAsMap(obj);
                    return TeaConverter.buildMap(
                        new TeaPair("body", res),
                        new TeaPair("headers", response_.headers)
                    );
                } else if (com.aliyun.teautil.Common.equalString(bodyType, "array")) {
                    Object arr = com.aliyun.teautil.Common.readAsJSON(response_.body);
                    return TeaConverter.buildMap(
                        new TeaPair("body", arr),
                        new TeaPair("headers", response_.headers)
                    );
                } else {
                    return TeaConverter.buildMap(
                        new TeaPair("headers", response_.headers)
                    );
                }

            } catch (Exception e) {
                if (Tea.isRetryable(e)) {
                    _lastException = e;
                    continue;
                }
                throw e;
            } finally {
                if (!com.aliyun.teautil.Common.isUnset(_lastResponse)
                        && !com.aliyun.teautil.Common.isUnset(_lastResponse.response)
                        && !com.aliyun.teautil.Common.isUnset(_lastResponse.response.body())){
                    _lastResponse.response.close();
                }
            }
        }
        throw new TeaUnretryableException(_lastRequest, _lastException);
    }

    public java.util.Map<String, ?> doRequest(Params params, OpenApiRequest request, RuntimeOptions runtime) throws Exception {
        TeaModel.validateParams(params, "params");
        TeaModel.validateParams(request, "request");
        java.util.Map<String, Object> runtime_ = TeaConverter.buildMap(
            new TeaPair("timeouted", "retry"),
            new TeaPair("readTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.readTimeout, _readTimeout)),
            new TeaPair("connectTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.connectTimeout, _connectTimeout)),
            new TeaPair("httpProxy", com.aliyun.teautil.Common.defaultString(runtime.httpProxy, _httpProxy)),
            new TeaPair("httpsProxy", com.aliyun.teautil.Common.defaultString(runtime.httpsProxy, _httpsProxy)),
            new TeaPair("noProxy", com.aliyun.teautil.Common.defaultString(runtime.noProxy, _noProxy)),
            new TeaPair("socks5Proxy", com.aliyun.teautil.Common.defaultString(runtime.socks5Proxy, _socks5Proxy)),
            new TeaPair("socks5NetWork", com.aliyun.teautil.Common.defaultString(runtime.socks5NetWork, _socks5NetWork)),
            new TeaPair("maxIdleConns", com.aliyun.teautil.Common.defaultNumber(runtime.maxIdleConns, _maxIdleConns)),
            new TeaPair("retry", TeaConverter.buildMap(
                new TeaPair("retryable", runtime.autoretry),
                new TeaPair("maxAttempts", com.aliyun.teautil.Common.defaultNumber(runtime.maxAttempts, 3))
            )),
            new TeaPair("backoff", TeaConverter.buildMap(
                new TeaPair("policy", com.aliyun.teautil.Common.defaultString(runtime.backoffPolicy, "no")),
                new TeaPair("period", com.aliyun.teautil.Common.defaultNumber(runtime.backoffPeriod, 1))
            )),
            new TeaPair("ignoreSSL", runtime.ignoreSSL),
            new TeaPair("keepAlive", runtime.keepAlive)
        );

        TeaRequest _lastRequest = null;
        Exception _lastException = null;
        TeaResponse _lastResponse = null;
        long _now = System.currentTimeMillis();
        int _retryTimes = 0;
        while (Tea.allowRetry((java.util.Map<String, Object>) runtime_.get("retry"), _retryTimes, _now)) {
            if (_retryTimes > 0) {
                int backoffTime = Tea.getBackoffTime(runtime_.get("backoff"), _retryTimes);
                if (backoffTime > 0) {
                    Tea.sleep(backoffTime);
                }
            }
            _retryTimes = _retryTimes + 1;
            try {
                TeaRequest request_ = new TeaRequest();
                request_.protocol = com.aliyun.teautil.Common.defaultString(_protocol, params.protocol);
                request_.method = params.method;
                request_.pathname = params.pathname;
                java.util.Map<String, String> globalQueries = new java.util.HashMap<>();
                java.util.Map<String, String> globalHeaders = new java.util.HashMap<>();
                if (!com.aliyun.teautil.Common.isUnset(TeaModel.buildMap(_globalParameters))) {
                    GlobalParameters globalParams = _globalParameters;
                    if (!com.aliyun.teautil.Common.isUnset(globalParams.queries)) {
                        globalQueries = globalParams.queries;
                    }

                    if (!com.aliyun.teautil.Common.isUnset(globalParams.headers)) {
                        globalHeaders = globalParams.headers;
                    }

                }

                request_.query = TeaConverter.merge(String.class,
                    globalQueries,
                    request.query
                );
                // endpoint is setted in product client
                request_.headers = TeaConverter.merge(String.class,
                    TeaConverter.buildMap(
                        new TeaPair("host", _endpoint),
                        new TeaPair("x-acs-version", params.version),
                        new TeaPair("x-acs-action", params.action),
                        new TeaPair("user-agent", this.getUserAgent()),
                        new TeaPair("x-acs-date", com.aliyun.openapiutil.Client.getTimestamp()),
                        new TeaPair("x-acs-signature-nonce", com.aliyun.teautil.Common.getNonce()),
                        new TeaPair("accept", "application/json")
                    ),
                    globalHeaders,
                    request.headers
                );
                if (com.aliyun.teautil.Common.equalString(params.style, "RPC")) {
                    java.util.Map<String, String> headers = this.getRpcHeaders();
                    if (!com.aliyun.teautil.Common.isUnset(headers)) {
                        request_.headers = TeaConverter.merge(String.class,
                            request_.headers,
                            headers
                        );
                    }

                }

                String signatureAlgorithm = com.aliyun.teautil.Common.defaultString(_signatureAlgorithm, "ACS3-HMAC-SHA256");
                String hashedRequestPayload = com.aliyun.openapiutil.Client.hexEncode(com.aliyun.openapiutil.Client.hash(com.aliyun.teautil.Common.toBytes(""), signatureAlgorithm));
                if (!com.aliyun.teautil.Common.isUnset(request.stream)) {
                    byte[] tmp = com.aliyun.teautil.Common.readAsBytes(request.stream);
                    hashedRequestPayload = com.aliyun.openapiutil.Client.hexEncode(com.aliyun.openapiutil.Client.hash(tmp, signatureAlgorithm));
                    request_.body = Tea.toReadable(tmp);
                    request_.headers.put("content-type", "application/octet-stream");
                } else {
                    if (!com.aliyun.teautil.Common.isUnset(request.body)) {
                        if (com.aliyun.teautil.Common.equalString(params.reqBodyType, "json")) {
                            String jsonObj = com.aliyun.teautil.Common.toJSONString(request.body);
                            hashedRequestPayload = com.aliyun.openapiutil.Client.hexEncode(com.aliyun.openapiutil.Client.hash(com.aliyun.teautil.Common.toBytes(jsonObj), signatureAlgorithm));
                            request_.body = Tea.toReadable(jsonObj);
                            request_.headers.put("content-type", "application/json; charset=utf-8");
                        } else {
                            java.util.Map<String, Object> m = com.aliyun.teautil.Common.assertAsMap(request.body);
                            String formObj = com.aliyun.openapiutil.Client.toForm(m);
                            hashedRequestPayload = com.aliyun.openapiutil.Client.hexEncode(com.aliyun.openapiutil.Client.hash(com.aliyun.teautil.Common.toBytes(formObj), signatureAlgorithm));
                            request_.body = Tea.toReadable(formObj);
                            request_.headers.put("content-type", "application/x-www-form-urlencoded");
                        }

                    }

                }

                request_.headers.put("x-acs-content-sha256", hashedRequestPayload);
                if (!com.aliyun.teautil.Common.equalString(params.authType, "Anonymous")) {
                    String authType = this.getType();
                    if (com.aliyun.teautil.Common.equalString(authType, "bearer")) {
                        String bearerToken = this.getBearerToken();
                        request_.headers.put("x-acs-bearer-token", bearerToken);
                    } else {
                        String accessKeyId = this.getAccessKeyId();
                        String accessKeySecret = this.getAccessKeySecret();
                        String securityToken = this.getSecurityToken();
                        if (!com.aliyun.teautil.Common.empty(securityToken)) {
                            request_.headers.put("x-acs-accesskey-id", accessKeyId);
                            request_.headers.put("x-acs-security-token", securityToken);
                        }

                        request_.headers.put("Authorization", com.aliyun.openapiutil.Client.getAuthorization(request_, signatureAlgorithm, hashedRequestPayload, accessKeyId, accessKeySecret));
                    }

                }

                _lastRequest = request_;
                TeaResponse response_ = Tea.doAction(request_, runtime_, interceptorChain);
                _lastResponse = response_;

                if (com.aliyun.teautil.Common.is4xx(response_.statusCode) || com.aliyun.teautil.Common.is5xx(response_.statusCode)) {
                    java.util.Map<String, Object> err = new java.util.HashMap<>();
                    if (!com.aliyun.teautil.Common.isUnset(response_.headers.get("content-type")) && com.aliyun.teautil.Common.equalString(response_.headers.get("content-type"), "text/xml;charset=utf-8")) {
                        String _str = com.aliyun.teautil.Common.readAsString(response_.body);
                        java.util.Map<String, Object> respMap = com.aliyun.teaxml.Client.parseXml(_str, null);
                        err = com.aliyun.teautil.Common.assertAsMap(respMap.get("Error"));
                    } else {
                        Object _res = com.aliyun.teautil.Common.readAsJSON(response_.body);
                        err = com.aliyun.teautil.Common.assertAsMap(_res);
                    }

                    err.put("statusCode", response_.statusCode);
                    throw new TeaException(TeaConverter.buildMap(
                        new TeaPair("code", "" + Client.defaultAny(err.get("Code"), err.get("code")) + ""),
                        new TeaPair("message", "code: " + response_.statusCode + ", " + Client.defaultAny(err.get("Message"), err.get("message")) + " request id: " + Client.defaultAny(err.get("RequestId"), err.get("requestId")) + ""),
                        new TeaPair("data", err)
                    ));
                }

                if (com.aliyun.teautil.Common.equalString(params.bodyType, "binary")) {
                    java.util.Map<String, Object> resp = TeaConverter.buildMap(
                        new TeaPair("body", response_.body),
                        new TeaPair("headers", response_.headers),
                        new TeaPair("statusCode", response_.statusCode)
                    );
                    return resp;
                } else if (com.aliyun.teautil.Common.equalString(params.bodyType, "byte")) {
                    byte[] byt = com.aliyun.teautil.Common.readAsBytes(response_.body);
                    return TeaConverter.buildMap(
                        new TeaPair("body", byt),
                        new TeaPair("headers", response_.headers),
                        new TeaPair("statusCode", response_.statusCode)
                    );
                } else if (com.aliyun.teautil.Common.equalString(params.bodyType, "string")) {
                    String str = com.aliyun.teautil.Common.readAsString(response_.body);
                    return TeaConverter.buildMap(
                        new TeaPair("body", str),
                        new TeaPair("headers", response_.headers),
                        new TeaPair("statusCode", response_.statusCode)
                    );
                } else if (com.aliyun.teautil.Common.equalString(params.bodyType, "json")) {
                    Object obj = com.aliyun.teautil.Common.readAsJSON(response_.body);
                    java.util.Map<String, Object> res = com.aliyun.teautil.Common.assertAsMap(obj);
                    return TeaConverter.buildMap(
                        new TeaPair("body", res),
                        new TeaPair("headers", response_.headers),
                        new TeaPair("statusCode", response_.statusCode)
                    );
                } else if (com.aliyun.teautil.Common.equalString(params.bodyType, "array")) {
                    Object arr = com.aliyun.teautil.Common.readAsJSON(response_.body);
                    return TeaConverter.buildMap(
                        new TeaPair("body", arr),
                        new TeaPair("headers", response_.headers),
                        new TeaPair("statusCode", response_.statusCode)
                    );
                } else {
                    return TeaConverter.buildMap(
                        new TeaPair("headers", response_.headers),
                        new TeaPair("statusCode", response_.statusCode)
                    );
                }

            } catch (Exception e) {
                if (Tea.isRetryable(e)) {
                    _lastException = e;
                    continue;
                }
                throw e;
            } finally {
                if (!com.aliyun.teautil.Common.isUnset(_lastResponse)
                        && !com.aliyun.teautil.Common.isUnset(_lastResponse.response)
                        && !com.aliyun.teautil.Common.isUnset(_lastResponse.response.body())){
                    _lastResponse.response.close();
                }
            }
        }
        throw new TeaUnretryableException(_lastRequest, _lastException);
    }

    public java.util.Map<String, ?> execute(Params params, OpenApiRequest request, RuntimeOptions runtime) throws Exception {
        TeaModel.validateParams(params, "params");
        TeaModel.validateParams(request, "request");
        java.util.Map<String, Object> runtime_ = TeaConverter.buildMap(
            new TeaPair("timeouted", "retry"),
            new TeaPair("readTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.readTimeout, _readTimeout)),
            new TeaPair("connectTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.connectTimeout, _connectTimeout)),
            new TeaPair("httpProxy", com.aliyun.teautil.Common.defaultString(runtime.httpProxy, _httpProxy)),
            new TeaPair("httpsProxy", com.aliyun.teautil.Common.defaultString(runtime.httpsProxy, _httpsProxy)),
            new TeaPair("noProxy", com.aliyun.teautil.Common.defaultString(runtime.noProxy, _noProxy)),
            new TeaPair("socks5Proxy", com.aliyun.teautil.Common.defaultString(runtime.socks5Proxy, _socks5Proxy)),
            new TeaPair("socks5NetWork", com.aliyun.teautil.Common.defaultString(runtime.socks5NetWork, _socks5NetWork)),
            new TeaPair("maxIdleConns", com.aliyun.teautil.Common.defaultNumber(runtime.maxIdleConns, _maxIdleConns)),
            new TeaPair("retry", TeaConverter.buildMap(
                new TeaPair("retryable", runtime.autoretry),
                new TeaPair("maxAttempts", com.aliyun.teautil.Common.defaultNumber(runtime.maxAttempts, 3))
            )),
            new TeaPair("backoff", TeaConverter.buildMap(
                new TeaPair("policy", com.aliyun.teautil.Common.defaultString(runtime.backoffPolicy, "no")),
                new TeaPair("period", com.aliyun.teautil.Common.defaultNumber(runtime.backoffPeriod, 1))
            )),
            new TeaPair("ignoreSSL", runtime.ignoreSSL),
            new TeaPair("keepAlive", runtime.keepAlive)
        );

        TeaRequest _lastRequest = null;
        Exception _lastException = null;
        long _now = System.currentTimeMillis();
        int _retryTimes = 0;
        while (Tea.allowRetry((java.util.Map<String, Object>) runtime_.get("retry"), _retryTimes, _now)) {
            if (_retryTimes > 0) {
                int backoffTime = Tea.getBackoffTime(runtime_.get("backoff"), _retryTimes);
                if (backoffTime > 0) {
                    Tea.sleep(backoffTime);
                }
            }
            _retryTimes = _retryTimes + 1;
            try {
                TeaRequest request_ = new TeaRequest();
                // spi = new Gateway();//Gateway implements SPI，这一步在产品 SDK 中实例化
                java.util.Map<String, String> headers = this.getRpcHeaders();
                java.util.Map<String, String> globalQueries = new java.util.HashMap<>();
                java.util.Map<String, String> globalHeaders = new java.util.HashMap<>();
                if (!com.aliyun.teautil.Common.isUnset(TeaModel.buildMap(_globalParameters))) {
                    GlobalParameters globalParams = _globalParameters;
                    if (!com.aliyun.teautil.Common.isUnset(globalParams.queries)) {
                        globalQueries = globalParams.queries;
                    }

                    if (!com.aliyun.teautil.Common.isUnset(globalParams.headers)) {
                        globalHeaders = globalParams.headers;
                    }

                }

                InterceptorContext.InterceptorContextRequest requestContext = InterceptorContext.InterceptorContextRequest.build(TeaConverter.buildMap(
                    new TeaPair("headers", TeaConverter.merge(String.class,
                        globalHeaders,
                        request.headers,
                        headers
                    )),
                    new TeaPair("query", TeaConverter.merge(String.class,
                        globalQueries,
                        request.query
                    )),
                    new TeaPair("body", request.body),
                    new TeaPair("stream", request.stream),
                    new TeaPair("hostMap", request.hostMap),
                    new TeaPair("pathname", params.pathname),
                    new TeaPair("productId", _productId),
                    new TeaPair("action", params.action),
                    new TeaPair("version", params.version),
                    new TeaPair("protocol", com.aliyun.teautil.Common.defaultString(_protocol, params.protocol)),
                    new TeaPair("method", com.aliyun.teautil.Common.defaultString(_method, params.method)),
                    new TeaPair("authType", params.authType),
                    new TeaPair("bodyType", params.bodyType),
                    new TeaPair("reqBodyType", params.reqBodyType),
                    new TeaPair("style", params.style),
                    new TeaPair("credential", _credential),
                    new TeaPair("signatureVersion", _signatureVersion),
                    new TeaPair("signatureAlgorithm", _signatureAlgorithm),
                    new TeaPair("userAgent", this.getUserAgent())
                ));
                InterceptorContext.InterceptorContextConfiguration configurationContext = InterceptorContext.InterceptorContextConfiguration.build(TeaConverter.buildMap(
                    new TeaPair("regionId", _regionId),
                    new TeaPair("endpoint", com.aliyun.teautil.Common.defaultString(request.endpointOverride, _endpoint)),
                    new TeaPair("endpointRule", _endpointRule),
                    new TeaPair("endpointMap", _endpointMap),
                    new TeaPair("endpointType", _endpointType),
                    new TeaPair("network", _network),
                    new TeaPair("suffix", _suffix)
                ));
                InterceptorContext interceptorContext = InterceptorContext.build(TeaConverter.buildMap(
                    new TeaPair("request", requestContext),
                    new TeaPair("configuration", configurationContext)
                ));
                AttributeMap attributeMap = new AttributeMap();
                // 1. spi.modifyConfiguration(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                _spi.modifyConfiguration(interceptorContext, attributeMap);
                // 2. spi.modifyRequest(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                _spi.modifyRequest(interceptorContext, attributeMap);
                request_.protocol = interceptorContext.request.protocol;
                request_.method = interceptorContext.request.method;
                request_.pathname = interceptorContext.request.pathname;
                request_.query = interceptorContext.request.query;
                request_.body = interceptorContext.request.stream;
                request_.headers = interceptorContext.request.headers;
                _lastRequest = request_;
                TeaResponse response_ = Tea.doAction(request_, runtime_, interceptorChain);

                InterceptorContext.InterceptorContextResponse responseContext = InterceptorContext.InterceptorContextResponse.build(TeaConverter.buildMap(
                    new TeaPair("statusCode", response_.statusCode),
                    new TeaPair("headers", response_.headers),
                    new TeaPair("body", response_.body)
                ));
                interceptorContext.response = responseContext;
                // 3. spi.modifyResponse(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                _spi.modifyResponse(interceptorContext, attributeMap);
                return TeaConverter.buildMap(
                    new TeaPair("headers", interceptorContext.response.headers),
                    new TeaPair("statusCode", interceptorContext.response.statusCode),
                    new TeaPair("body", interceptorContext.response.deserializedBody)
                );
            } catch (Exception e) {
                if (Tea.isRetryable(e)) {
                    _lastException = e;
                    continue;
                }
                throw e;
            }
        }
        throw new TeaUnretryableException(_lastRequest, _lastException);
    }

    public void addRuntimeOptionsInterceptor(RuntimeOptionsInterceptor interceptor) {
        interceptorChain.addRuntimeOptionsInterceptor(interceptor);
    }

    public void addRequestInterceptor(RequestInterceptor interceptor) {
        interceptorChain.addRequestInterceptor(interceptor);
    }

    public void addResponseInterceptor(ResponseInterceptor interceptor) {
        interceptorChain.addResponseInterceptor(interceptor);
    }

    public java.util.Map<String, ?> callApi(Params params, OpenApiRequest request, RuntimeOptions runtime) throws Exception {
        if (com.aliyun.teautil.Common.isUnset(TeaModel.buildMap(params))) {
            throw new TeaException(TeaConverter.buildMap(
                new TeaPair("code", "ParameterMissing"),
                new TeaPair("message", "'params' can not be unset")
            ));
        }

        if (com.aliyun.teautil.Common.isUnset(_signatureAlgorithm) || !com.aliyun.teautil.Common.equalString(_signatureAlgorithm, "v2")) {
            return this.doRequest(params, request, runtime);
        } else if (com.aliyun.teautil.Common.equalString(params.style, "ROA") && com.aliyun.teautil.Common.equalString(params.reqBodyType, "json")) {
            return this.doROARequest(params.action, params.version, params.protocol, params.method, params.authType, params.pathname, params.bodyType, request, runtime);
        } else if (com.aliyun.teautil.Common.equalString(params.style, "ROA")) {
            return this.doROARequestWithForm(params.action, params.version, params.protocol, params.method, params.authType, params.pathname, params.bodyType, request, runtime);
        } else {
            return this.doRPCRequest(params.action, params.version, params.protocol, params.method, params.authType, params.bodyType, request, runtime);
        }

    }

    /**
     * Get user agent
     * @return user agent
     */
    public String getUserAgent() throws Exception {
        String userAgent = com.aliyun.teautil.Common.getUserAgent(_userAgent);
        return userAgent;
    }

    /**
     * Get accesskey id by using credential
     * @return accesskey id
     */
    public String getAccessKeyId() throws Exception {
        if (com.aliyun.teautil.Common.isUnset(_credential)) {
            return "";
        }

        String accessKeyId = _credential.getAccessKeyId();
        return accessKeyId;
    }

    /**
     * Get accesskey secret by using credential
     * @return accesskey secret
     */
    public String getAccessKeySecret() throws Exception {
        if (com.aliyun.teautil.Common.isUnset(_credential)) {
            return "";
        }

        String secret = _credential.getAccessKeySecret();
        return secret;
    }

    /**
     * Get security token by using credential
     * @return security token
     */
    public String getSecurityToken() throws Exception {
        if (com.aliyun.teautil.Common.isUnset(_credential)) {
            return "";
        }

        String token = _credential.getSecurityToken();
        return token;
    }

    /**
     * Get bearer token by credential
     * @return bearer token
     */
    public String getBearerToken() throws Exception {
        if (com.aliyun.teautil.Common.isUnset(_credential)) {
            return "";
        }

        String token = _credential.getBearerToken();
        return token;
    }

    /**
     * Get credential type by credential
     * @return credential type e.g. access_key
     */
    public String getType() throws Exception {
        if (com.aliyun.teautil.Common.isUnset(_credential)) {
            return "";
        }

        String authType = _credential.getType();
        return authType;
    }

    /**
     * If inputValue is not null, return it or return defaultValue
     * @param inputValue  users input value
     * @param defaultValue default value
     * @return the final result
     */
    public static Object defaultAny(Object inputValue, Object defaultValue) throws Exception {
        if (com.aliyun.teautil.Common.isUnset(inputValue)) {
            return defaultValue;
        }

        return inputValue;
    }

    /**
     * If the endpointRule and config.endpoint are empty, throw error
     * @param config config contains the necessary information to create a client
     */
    public void checkConfig(com.aliyun.teaopenapi.models.Config config) throws Exception {
        if (com.aliyun.teautil.Common.empty(_endpointRule) && com.aliyun.teautil.Common.empty(config.endpoint)) {
            throw new TeaException(TeaConverter.buildMap(
                new TeaPair("code", "ParameterMissing"),
                new TeaPair("message", "'config.endpoint' can not be empty")
            ));
        }

    }

    /**
     * set RPC header for debug
     * @param headers headers for debug, this header can be used only once.
     */
    public void setRpcHeaders(java.util.Map<String, String> headers) throws Exception {
        this._headers = headers;
    }

    /**
     * get RPC header for debug
     */
    public java.util.Map<String, String> getRpcHeaders() throws Exception {
        java.util.Map<String, String> headers = _headers;
        this._headers = null;
        return headers;
    }
}
