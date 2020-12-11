// This file is auto-generated, don't edit it. Thanks.
package com.aliyun.teaopenapi;

import com.aliyun.tea.*;
import com.aliyun.teaopenapi.models.*;

public class Client {

    public String _endpoint;
    public String _regionId;
    public String _protocol;
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
    /**
     * Init client with Config
     * @param config config contains the necessary information to create a client
     */
    public Client(Config config) throws Exception {
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
        this._protocol = config.protocol;
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
    }

    public java.util.Map<String, ?> doRPCRequest(String action, String version, String protocol, String method, String authType, String bodyType, OpenApiRequest request, com.aliyun.teautil.models.RuntimeOptions runtime) throws Exception {
        TeaModel.validateParams(request, "request");
        java.util.Map<String, Object> runtime_ = TeaConverter.buildMap(
            new TeaPair("timeouted", "retry"),
            new TeaPair("readTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.readTimeout, _readTimeout)),
            new TeaPair("connectTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.connectTimeout, _connectTimeout)),
            new TeaPair("httpProxy", com.aliyun.teautil.Common.defaultString(runtime.httpProxy, _httpProxy)),
            new TeaPair("httpsProxy", com.aliyun.teautil.Common.defaultString(runtime.httpsProxy, _httpsProxy)),
            new TeaPair("noProxy", com.aliyun.teautil.Common.defaultString(runtime.noProxy, _noProxy)),
            new TeaPair("maxIdleConns", com.aliyun.teautil.Common.defaultNumber(runtime.maxIdleConns, _maxIdleConns)),
            new TeaPair("retry", TeaConverter.buildMap(
                new TeaPair("retryable", runtime.autoretry),
                new TeaPair("maxAttempts", com.aliyun.teautil.Common.defaultNumber(runtime.maxAttempts, 3))
            )),
            new TeaPair("backoff", TeaConverter.buildMap(
                new TeaPair("policy", com.aliyun.teautil.Common.defaultString(runtime.backoffPolicy, "no")),
                new TeaPair("period", com.aliyun.teautil.Common.defaultNumber(runtime.backoffPeriod, 1))
            )),
            new TeaPair("ignoreSSL", runtime.ignoreSSL)
        );

        TeaRequest _lastRequest = null;
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
                // endpoint is setted in product client
                request_.headers = TeaConverter.buildMap(
                    new TeaPair("host", _endpoint),
                    new TeaPair("x-acs-version", version),
                    new TeaPair("x-acs-action", action),
                    new TeaPair("user-agent", this.getUserAgent())
                );
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
                TeaResponse response_ = Tea.doAction(request_, runtime_);

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
                    continue;
                }
                throw new RuntimeException(e);
            }
        }

        throw new TeaUnretryableException(_lastRequest);
    }

    public java.util.Map<String, ?> doROARequest(String action, String version, String protocol, String method, String authType, String pathname, String bodyType, OpenApiRequest request, com.aliyun.teautil.models.RuntimeOptions runtime) throws Exception {
        TeaModel.validateParams(request, "request");
        java.util.Map<String, Object> runtime_ = TeaConverter.buildMap(
            new TeaPair("timeouted", "retry"),
            new TeaPair("readTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.readTimeout, _readTimeout)),
            new TeaPair("connectTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.connectTimeout, _connectTimeout)),
            new TeaPair("httpProxy", com.aliyun.teautil.Common.defaultString(runtime.httpProxy, _httpProxy)),
            new TeaPair("httpsProxy", com.aliyun.teautil.Common.defaultString(runtime.httpsProxy, _httpsProxy)),
            new TeaPair("noProxy", com.aliyun.teautil.Common.defaultString(runtime.noProxy, _noProxy)),
            new TeaPair("maxIdleConns", com.aliyun.teautil.Common.defaultNumber(runtime.maxIdleConns, _maxIdleConns)),
            new TeaPair("retry", TeaConverter.buildMap(
                new TeaPair("retryable", runtime.autoretry),
                new TeaPair("maxAttempts", com.aliyun.teautil.Common.defaultNumber(runtime.maxAttempts, 3))
            )),
            new TeaPair("backoff", TeaConverter.buildMap(
                new TeaPair("policy", com.aliyun.teautil.Common.defaultString(runtime.backoffPolicy, "no")),
                new TeaPair("period", com.aliyun.teautil.Common.defaultNumber(runtime.backoffPeriod, 1))
            )),
            new TeaPair("ignoreSSL", runtime.ignoreSSL)
        );

        TeaRequest _lastRequest = null;
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
                TeaResponse response_ = Tea.doAction(request_, runtime_);

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
                    continue;
                }
                throw new RuntimeException(e);
            }
        }

        throw new TeaUnretryableException(_lastRequest);
    }

    public java.util.Map<String, ?> doROARequestWithForm(String action, String version, String protocol, String method, String authType, String pathname, String bodyType, OpenApiRequest request, com.aliyun.teautil.models.RuntimeOptions runtime) throws Exception {
        TeaModel.validateParams(request, "request");
        java.util.Map<String, Object> runtime_ = TeaConverter.buildMap(
            new TeaPair("timeouted", "retry"),
            new TeaPair("readTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.readTimeout, _readTimeout)),
            new TeaPair("connectTimeout", com.aliyun.teautil.Common.defaultNumber(runtime.connectTimeout, _connectTimeout)),
            new TeaPair("httpProxy", com.aliyun.teautil.Common.defaultString(runtime.httpProxy, _httpProxy)),
            new TeaPair("httpsProxy", com.aliyun.teautil.Common.defaultString(runtime.httpsProxy, _httpsProxy)),
            new TeaPair("noProxy", com.aliyun.teautil.Common.defaultString(runtime.noProxy, _noProxy)),
            new TeaPair("maxIdleConns", com.aliyun.teautil.Common.defaultNumber(runtime.maxIdleConns, _maxIdleConns)),
            new TeaPair("retry", TeaConverter.buildMap(
                new TeaPair("retryable", runtime.autoretry),
                new TeaPair("maxAttempts", com.aliyun.teautil.Common.defaultNumber(runtime.maxAttempts, 3))
            )),
            new TeaPair("backoff", TeaConverter.buildMap(
                new TeaPair("policy", com.aliyun.teautil.Common.defaultString(runtime.backoffPolicy, "no")),
                new TeaPair("period", com.aliyun.teautil.Common.defaultNumber(runtime.backoffPeriod, 1))
            )),
            new TeaPair("ignoreSSL", runtime.ignoreSSL)
        );

        TeaRequest _lastRequest = null;
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
                TeaResponse response_ = Tea.doAction(request_, runtime_);

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
                    continue;
                }
                throw new RuntimeException(e);
            }
        }

        throw new TeaUnretryableException(_lastRequest);
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
    public void checkConfig(Config config) throws Exception {
        if (com.aliyun.teautil.Common.empty(_endpointRule) && com.aliyun.teautil.Common.empty(config.endpoint)) {
            throw new TeaException(TeaConverter.buildMap(
                new TeaPair("code", "ParameterMissing"),
                new TeaPair("message", "'config.endpoint' can not be empty")
            ));
        }

    }
}
