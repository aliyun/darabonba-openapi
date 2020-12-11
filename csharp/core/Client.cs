// This file is auto-generated, don't edit it. Thanks.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Tea;
using Tea.Utils;

using AlibabaCloud.OpenApiClient.Models;

namespace AlibabaCloud.OpenApiClient
{
    public class Client 
    {
        protected string _endpoint;
        protected string _regionId;
        protected string _protocol;
        protected string _userAgent;
        protected string _endpointRule;
        protected Dictionary<string, string> _endpointMap;
        protected string _suffix;
        protected int? _readTimeout;
        protected int? _connectTimeout;
        protected string _httpProxy;
        protected string _httpsProxy;
        protected string _socks5Proxy;
        protected string _socks5NetWork;
        protected string _noProxy;
        protected string _network;
        protected string _productId;
        protected int? _maxIdleConns;
        protected string _endpointType;
        protected string _openPlatformEndpoint;
        protected Aliyun.Credentials.Client _credential;

        /**
         * Init client with Config
         * @param config config contains the necessary information to create a client
         */
        public Client(Config config)
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(config.ToMap()))
            {
                throw new TeaException(new Dictionary<string, string>
                {
                    {"code", "ParameterMissing"},
                    {"message", "'config' can not be unset"},
                });
            }
            if (!AlibabaCloud.TeaUtil.Common.Empty(config.AccessKeyId) && !AlibabaCloud.TeaUtil.Common.Empty(config.AccessKeySecret))
            {
                if (!AlibabaCloud.TeaUtil.Common.Empty(config.SecurityToken))
                {
                    config.Type = "sts";
                }
                else
                {
                    config.Type = "access_key";
                }
                Aliyun.Credentials.Models.Config credentialConfig = new Aliyun.Credentials.Models.Config
                {
                    AccessKeyId = config.AccessKeyId,
                    Type = config.Type,
                    AccessKeySecret = config.AccessKeySecret,
                    SecurityToken = config.SecurityToken,
                };
                this._credential = new Aliyun.Credentials.Client(credentialConfig);
            }
            else if (!AlibabaCloud.TeaUtil.Common.IsUnset(config.Credential))
            {
                this._credential = config.Credential;
            }
            this._endpoint = config.Endpoint;
            this._protocol = config.Protocol;
            this._regionId = config.RegionId;
            this._userAgent = config.UserAgent;
            this._readTimeout = config.ReadTimeout;
            this._connectTimeout = config.ConnectTimeout;
            this._httpProxy = config.HttpProxy;
            this._httpsProxy = config.HttpsProxy;
            this._noProxy = config.NoProxy;
            this._socks5Proxy = config.Socks5Proxy;
            this._socks5NetWork = config.Socks5NetWork;
            this._maxIdleConns = config.MaxIdleConns;
        }

        /**
         * Encapsulate the request and invoke the network
         * @param action api name
         * @param version product version
         * @param protocol http or https
         * @param method e.g. GET
         * @param authType authorization type e.g. AK
         * @param bodyType response body type e.g. String
         * @param request object of OpenApiRequest
         * @param runtime which controls some details of call api, such as retry times
         * @return the response
         */
        public Dictionary<string, object> DoRPCRequest(string action, string version, string protocol, string method, string authType, string bodyType, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"maxIdleConns", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.MaxIdleConns, _maxIdleConns)},
                {"retry", new Dictionary<string, object>
                {
                    {"retryable", runtime.Autoretry},
                    {"maxAttempts", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.MaxAttempts, 3)},
                }},
                {"backoff", new Dictionary<string, object>
                {
                    {"policy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.BackoffPolicy, "no")},
                    {"period", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.BackoffPeriod, 1)},
                }},
                {"ignoreSSL", runtime.IgnoreSSL},
            };

            TeaRequest _lastRequest = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retryTimes = 0;
            while (TeaCore.AllowRetry((IDictionary) runtime_["retry"], _retryTimes, _now))
            {
                if (_retryTimes > 0)
                {
                    int backoffTime = TeaCore.GetBackoffTime((IDictionary)runtime_["backoff"], _retryTimes);
                    if (backoffTime > 0)
                    {
                        TeaCore.Sleep(backoffTime);
                    }
                }
                _retryTimes = _retryTimes + 1;
                try
                {
                    TeaRequest request_ = new TeaRequest();
                    request_.Protocol = AlibabaCloud.TeaUtil.Common.DefaultString(_protocol, protocol);
                    request_.Method = method;
                    request_.Pathname = "/";
                    request_.Query = TeaConverter.merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"Action", action},
                            {"Format", "json"},
                            {"Version", version},
                            {"Timestamp", AlibabaCloud.OpenApiUtil.Client.GetTimestamp()},
                            {"SignatureNonce", AlibabaCloud.TeaUtil.Common.GetNonce()},
                        },
                        request.Query
                    );
                    // endpoint is setted in product client
                    request_.Headers = new Dictionary<string, string>
                    {
                        {"host", _endpoint},
                        {"x-acs-version", version},
                        {"x-acs-action", action},
                        {"user-agent", GetUserAgent()},
                    };
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                    {
                        Dictionary<string, object> m = AlibabaCloud.TeaUtil.Common.AssertAsMap(request.Body);
                        Dictionary<string, object> tmp = AlibabaCloud.TeaUtil.Common.AnyifyMapValue(AlibabaCloud.OpenApiUtil.Client.Query(m));
                        request_.Body = TeaCore.BytesReadable(AlibabaCloud.TeaUtil.Common.ToFormString(tmp));
                        request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                    }
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(authType, "Anonymous"))
                    {
                        string accessKeyId = GetAccessKeyId();
                        string accessKeySecret = GetAccessKeySecret();
                        string securityToken = GetSecurityToken();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(securityToken))
                        {
                            request_.Query["SecurityToken"] = securityToken;
                        }
                        request_.Query["SignatureMethod"] = "HMAC-SHA1";
                        request_.Query["SignatureVersion"] = "1.0";
                        request_.Query["AccessKeyId"] = accessKeyId;
                        Dictionary<string, object> t = null;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                        {
                            t = AlibabaCloud.TeaUtil.Common.AssertAsMap(request.Body);
                        }
                        Dictionary<string, string> signedParam = TeaConverter.merge<string>
                        (
                            request_.Query,
                            AlibabaCloud.OpenApiUtil.Client.Query(t)
                        );
                        request_.Query["Signature"] = AlibabaCloud.OpenApiUtil.Client.GetRPCSignature(signedParam, request_.Method, accessKeySecret);
                    }
                    _lastRequest = request_;
                    TeaResponse response_ = TeaCore.DoAction(request_, runtime_);

                    if (AlibabaCloud.TeaUtil.Common.Is4xx(response_.StatusCode) || AlibabaCloud.TeaUtil.Common.Is5xx(response_.StatusCode))
                    {
                        object _res = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = AlibabaCloud.TeaUtil.Common.AssertAsMap(_res);
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + DefaultAny(err.Get("RequestId"), err.Get("requestId"))},
                            {"data", err},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                        };
                        return resp;
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "byte"))
                    {
                        byte[] byt = AlibabaCloud.TeaUtil.Common.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "json"))
                    {
                        object obj = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = AlibabaCloud.TeaUtil.Common.AssertAsMap(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                        };
                    }
                    else
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                }
                catch (Exception e)
                {
                    if (TeaCore.IsRetryable(e))
                    {
                        _lastException = e;
                        continue;
                    }
                    throw e;
                }
            }

            throw new TeaUnretryableException(_lastRequest, _lastException);
        }

        /**
         * Encapsulate the request and invoke the network
         * @param action api name
         * @param version product version
         * @param protocol http or https
         * @param method e.g. GET
         * @param authType authorization type e.g. AK
         * @param bodyType response body type e.g. String
         * @param request object of OpenApiRequest
         * @param runtime which controls some details of call api, such as retry times
         * @return the response
         */
        public async Task<Dictionary<string, object>> DoRPCRequestAsync(string action, string version, string protocol, string method, string authType, string bodyType, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"maxIdleConns", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.MaxIdleConns, _maxIdleConns)},
                {"retry", new Dictionary<string, object>
                {
                    {"retryable", runtime.Autoretry},
                    {"maxAttempts", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.MaxAttempts, 3)},
                }},
                {"backoff", new Dictionary<string, object>
                {
                    {"policy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.BackoffPolicy, "no")},
                    {"period", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.BackoffPeriod, 1)},
                }},
                {"ignoreSSL", runtime.IgnoreSSL},
            };

            TeaRequest _lastRequest = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retryTimes = 0;
            while (TeaCore.AllowRetry((IDictionary) runtime_["retry"], _retryTimes, _now))
            {
                if (_retryTimes > 0)
                {
                    int backoffTime = TeaCore.GetBackoffTime((IDictionary)runtime_["backoff"], _retryTimes);
                    if (backoffTime > 0)
                    {
                        TeaCore.Sleep(backoffTime);
                    }
                }
                _retryTimes = _retryTimes + 1;
                try
                {
                    TeaRequest request_ = new TeaRequest();
                    request_.Protocol = AlibabaCloud.TeaUtil.Common.DefaultString(_protocol, protocol);
                    request_.Method = method;
                    request_.Pathname = "/";
                    request_.Query = TeaConverter.merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"Action", action},
                            {"Format", "json"},
                            {"Version", version},
                            {"Timestamp", AlibabaCloud.OpenApiUtil.Client.GetTimestamp()},
                            {"SignatureNonce", AlibabaCloud.TeaUtil.Common.GetNonce()},
                        },
                        request.Query
                    );
                    // endpoint is setted in product client
                    request_.Headers = new Dictionary<string, string>
                    {
                        {"host", _endpoint},
                        {"x-acs-version", version},
                        {"x-acs-action", action},
                        {"user-agent", GetUserAgent()},
                    };
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                    {
                        Dictionary<string, object> m = AlibabaCloud.TeaUtil.Common.AssertAsMap(request.Body);
                        Dictionary<string, object> tmp = AlibabaCloud.TeaUtil.Common.AnyifyMapValue(AlibabaCloud.OpenApiUtil.Client.Query(m));
                        request_.Body = TeaCore.BytesReadable(AlibabaCloud.TeaUtil.Common.ToFormString(tmp));
                        request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                    }
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(authType, "Anonymous"))
                    {
                        string accessKeyId = await GetAccessKeyIdAsync();
                        string accessKeySecret = await GetAccessKeySecretAsync();
                        string securityToken = await GetSecurityTokenAsync();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(securityToken))
                        {
                            request_.Query["SecurityToken"] = securityToken;
                        }
                        request_.Query["SignatureMethod"] = "HMAC-SHA1";
                        request_.Query["SignatureVersion"] = "1.0";
                        request_.Query["AccessKeyId"] = accessKeyId;
                        Dictionary<string, object> t = null;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                        {
                            t = AlibabaCloud.TeaUtil.Common.AssertAsMap(request.Body);
                        }
                        Dictionary<string, string> signedParam = TeaConverter.merge<string>
                        (
                            request_.Query,
                            AlibabaCloud.OpenApiUtil.Client.Query(t)
                        );
                        request_.Query["Signature"] = AlibabaCloud.OpenApiUtil.Client.GetRPCSignature(signedParam, request_.Method, accessKeySecret);
                    }
                    _lastRequest = request_;
                    TeaResponse response_ = await TeaCore.DoActionAsync(request_, runtime_);

                    if (AlibabaCloud.TeaUtil.Common.Is4xx(response_.StatusCode) || AlibabaCloud.TeaUtil.Common.Is5xx(response_.StatusCode))
                    {
                        object _res = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = AlibabaCloud.TeaUtil.Common.AssertAsMap(_res);
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + DefaultAny(err.Get("RequestId"), err.Get("requestId"))},
                            {"data", err},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                        };
                        return resp;
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "byte"))
                    {
                        byte[] byt = AlibabaCloud.TeaUtil.Common.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "json"))
                    {
                        object obj = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = AlibabaCloud.TeaUtil.Common.AssertAsMap(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                        };
                    }
                    else
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                }
                catch (Exception e)
                {
                    if (TeaCore.IsRetryable(e))
                    {
                        _lastException = e;
                        continue;
                    }
                    throw e;
                }
            }

            throw new TeaUnretryableException(_lastRequest, _lastException);
        }

        /**
         * Encapsulate the request and invoke the network
         * @param action api name
         * @param version product version
         * @param protocol http or https
         * @param method e.g. GET
         * @param authType authorization type e.g. AK
         * @param pathname pathname of every api
         * @param bodyType response body type e.g. String
         * @param request object of OpenApiRequest
         * @param runtime which controls some details of call api, such as retry times
         * @return the response
         */
        public Dictionary<string, object> DoROARequest(string action, string version, string protocol, string method, string authType, string pathname, string bodyType, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"maxIdleConns", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.MaxIdleConns, _maxIdleConns)},
                {"retry", new Dictionary<string, object>
                {
                    {"retryable", runtime.Autoretry},
                    {"maxAttempts", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.MaxAttempts, 3)},
                }},
                {"backoff", new Dictionary<string, object>
                {
                    {"policy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.BackoffPolicy, "no")},
                    {"period", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.BackoffPeriod, 1)},
                }},
                {"ignoreSSL", runtime.IgnoreSSL},
            };

            TeaRequest _lastRequest = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retryTimes = 0;
            while (TeaCore.AllowRetry((IDictionary) runtime_["retry"], _retryTimes, _now))
            {
                if (_retryTimes > 0)
                {
                    int backoffTime = TeaCore.GetBackoffTime((IDictionary)runtime_["backoff"], _retryTimes);
                    if (backoffTime > 0)
                    {
                        TeaCore.Sleep(backoffTime);
                    }
                }
                _retryTimes = _retryTimes + 1;
                try
                {
                    TeaRequest request_ = new TeaRequest();
                    request_.Protocol = AlibabaCloud.TeaUtil.Common.DefaultString(_protocol, protocol);
                    request_.Method = method;
                    request_.Pathname = pathname;
                    request_.Headers = TeaConverter.merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"date", AlibabaCloud.TeaUtil.Common.GetDateUTCString()},
                            {"host", _endpoint},
                            {"accept", "application/json"},
                            {"x-acs-signature-nonce", AlibabaCloud.TeaUtil.Common.GetNonce()},
                            {"x-acs-signature-method", "HMAC-SHA1"},
                            {"x-acs-signature-version", "1.0"},
                            {"x-acs-version", version},
                            {"x-acs-action", action},
                            {"user-agent", AlibabaCloud.TeaUtil.Common.GetUserAgent(_userAgent)},
                        },
                        request.Headers
                    );
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                    {
                        request_.Body = TeaCore.BytesReadable(AlibabaCloud.TeaUtil.Common.ToJSONString(request.Body));
                        request_.Headers["content-type"] = "application/json; charset=utf-8";
                    }
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Query))
                    {
                        request_.Query = request.Query;
                    }
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(authType, "Anonymous"))
                    {
                        string accessKeyId = GetAccessKeyId();
                        string accessKeySecret = GetAccessKeySecret();
                        string securityToken = GetSecurityToken();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(securityToken))
                        {
                            request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                            request_.Headers["x-acs-security-token"] = securityToken;
                        }
                        string stringToSign = AlibabaCloud.OpenApiUtil.Client.GetStringToSign(request_);
                        request_.Headers["authorization"] = "acs " + accessKeyId + ":" + AlibabaCloud.OpenApiUtil.Client.GetROASignature(stringToSign, accessKeySecret);
                    }
                    _lastRequest = request_;
                    TeaResponse response_ = TeaCore.DoAction(request_, runtime_);

                    if (AlibabaCloud.TeaUtil.Common.EqualNumber(response_.StatusCode, 204))
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                    if (AlibabaCloud.TeaUtil.Common.Is4xx(response_.StatusCode) || AlibabaCloud.TeaUtil.Common.Is5xx(response_.StatusCode))
                    {
                        object _res = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = AlibabaCloud.TeaUtil.Common.AssertAsMap(_res);
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + DefaultAny(err.Get("RequestId"), err.Get("requestId"))},
                            {"data", err},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                        };
                        return resp;
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "byte"))
                    {
                        byte[] byt = AlibabaCloud.TeaUtil.Common.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "json"))
                    {
                        object obj = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = AlibabaCloud.TeaUtil.Common.AssertAsMap(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                        };
                    }
                    else
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                }
                catch (Exception e)
                {
                    if (TeaCore.IsRetryable(e))
                    {
                        _lastException = e;
                        continue;
                    }
                    throw e;
                }
            }

            throw new TeaUnretryableException(_lastRequest, _lastException);
        }

        /**
         * Encapsulate the request and invoke the network
         * @param action api name
         * @param version product version
         * @param protocol http or https
         * @param method e.g. GET
         * @param authType authorization type e.g. AK
         * @param pathname pathname of every api
         * @param bodyType response body type e.g. String
         * @param request object of OpenApiRequest
         * @param runtime which controls some details of call api, such as retry times
         * @return the response
         */
        public async Task<Dictionary<string, object>> DoROARequestAsync(string action, string version, string protocol, string method, string authType, string pathname, string bodyType, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"maxIdleConns", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.MaxIdleConns, _maxIdleConns)},
                {"retry", new Dictionary<string, object>
                {
                    {"retryable", runtime.Autoretry},
                    {"maxAttempts", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.MaxAttempts, 3)},
                }},
                {"backoff", new Dictionary<string, object>
                {
                    {"policy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.BackoffPolicy, "no")},
                    {"period", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.BackoffPeriod, 1)},
                }},
                {"ignoreSSL", runtime.IgnoreSSL},
            };

            TeaRequest _lastRequest = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retryTimes = 0;
            while (TeaCore.AllowRetry((IDictionary) runtime_["retry"], _retryTimes, _now))
            {
                if (_retryTimes > 0)
                {
                    int backoffTime = TeaCore.GetBackoffTime((IDictionary)runtime_["backoff"], _retryTimes);
                    if (backoffTime > 0)
                    {
                        TeaCore.Sleep(backoffTime);
                    }
                }
                _retryTimes = _retryTimes + 1;
                try
                {
                    TeaRequest request_ = new TeaRequest();
                    request_.Protocol = AlibabaCloud.TeaUtil.Common.DefaultString(_protocol, protocol);
                    request_.Method = method;
                    request_.Pathname = pathname;
                    request_.Headers = TeaConverter.merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"date", AlibabaCloud.TeaUtil.Common.GetDateUTCString()},
                            {"host", _endpoint},
                            {"accept", "application/json"},
                            {"x-acs-signature-nonce", AlibabaCloud.TeaUtil.Common.GetNonce()},
                            {"x-acs-signature-method", "HMAC-SHA1"},
                            {"x-acs-signature-version", "1.0"},
                            {"x-acs-version", version},
                            {"x-acs-action", action},
                            {"user-agent", AlibabaCloud.TeaUtil.Common.GetUserAgent(_userAgent)},
                        },
                        request.Headers
                    );
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                    {
                        request_.Body = TeaCore.BytesReadable(AlibabaCloud.TeaUtil.Common.ToJSONString(request.Body));
                        request_.Headers["content-type"] = "application/json; charset=utf-8";
                    }
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Query))
                    {
                        request_.Query = request.Query;
                    }
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(authType, "Anonymous"))
                    {
                        string accessKeyId = await GetAccessKeyIdAsync();
                        string accessKeySecret = await GetAccessKeySecretAsync();
                        string securityToken = await GetSecurityTokenAsync();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(securityToken))
                        {
                            request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                            request_.Headers["x-acs-security-token"] = securityToken;
                        }
                        string stringToSign = AlibabaCloud.OpenApiUtil.Client.GetStringToSign(request_);
                        request_.Headers["authorization"] = "acs " + accessKeyId + ":" + AlibabaCloud.OpenApiUtil.Client.GetROASignature(stringToSign, accessKeySecret);
                    }
                    _lastRequest = request_;
                    TeaResponse response_ = await TeaCore.DoActionAsync(request_, runtime_);

                    if (AlibabaCloud.TeaUtil.Common.EqualNumber(response_.StatusCode, 204))
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                    if (AlibabaCloud.TeaUtil.Common.Is4xx(response_.StatusCode) || AlibabaCloud.TeaUtil.Common.Is5xx(response_.StatusCode))
                    {
                        object _res = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = AlibabaCloud.TeaUtil.Common.AssertAsMap(_res);
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + DefaultAny(err.Get("RequestId"), err.Get("requestId"))},
                            {"data", err},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                        };
                        return resp;
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "byte"))
                    {
                        byte[] byt = AlibabaCloud.TeaUtil.Common.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "json"))
                    {
                        object obj = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = AlibabaCloud.TeaUtil.Common.AssertAsMap(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                        };
                    }
                    else
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                }
                catch (Exception e)
                {
                    if (TeaCore.IsRetryable(e))
                    {
                        _lastException = e;
                        continue;
                    }
                    throw e;
                }
            }

            throw new TeaUnretryableException(_lastRequest, _lastException);
        }

        /**
         * Encapsulate the request and invoke the network with form body
         * @param action api name
         * @param version product version
         * @param protocol http or https
         * @param method e.g. GET
         * @param authType authorization type e.g. AK
         * @param pathname pathname of every api
         * @param bodyType response body type e.g. String
         * @param request object of OpenApiRequest
         * @param runtime which controls some details of call api, such as retry times
         * @return the response
         */
        public Dictionary<string, object> DoROARequestWithForm(string action, string version, string protocol, string method, string authType, string pathname, string bodyType, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"maxIdleConns", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.MaxIdleConns, _maxIdleConns)},
                {"retry", new Dictionary<string, object>
                {
                    {"retryable", runtime.Autoretry},
                    {"maxAttempts", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.MaxAttempts, 3)},
                }},
                {"backoff", new Dictionary<string, object>
                {
                    {"policy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.BackoffPolicy, "no")},
                    {"period", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.BackoffPeriod, 1)},
                }},
                {"ignoreSSL", runtime.IgnoreSSL},
            };

            TeaRequest _lastRequest = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retryTimes = 0;
            while (TeaCore.AllowRetry((IDictionary) runtime_["retry"], _retryTimes, _now))
            {
                if (_retryTimes > 0)
                {
                    int backoffTime = TeaCore.GetBackoffTime((IDictionary)runtime_["backoff"], _retryTimes);
                    if (backoffTime > 0)
                    {
                        TeaCore.Sleep(backoffTime);
                    }
                }
                _retryTimes = _retryTimes + 1;
                try
                {
                    TeaRequest request_ = new TeaRequest();
                    request_.Protocol = AlibabaCloud.TeaUtil.Common.DefaultString(_protocol, protocol);
                    request_.Method = method;
                    request_.Pathname = pathname;
                    request_.Headers = TeaConverter.merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"date", AlibabaCloud.TeaUtil.Common.GetDateUTCString()},
                            {"host", _endpoint},
                            {"accept", "application/json"},
                            {"x-acs-signature-nonce", AlibabaCloud.TeaUtil.Common.GetNonce()},
                            {"x-acs-signature-method", "HMAC-SHA1"},
                            {"x-acs-signature-version", "1.0"},
                            {"x-acs-version", version},
                            {"x-acs-action", action},
                            {"user-agent", AlibabaCloud.TeaUtil.Common.GetUserAgent(_userAgent)},
                        },
                        request.Headers
                    );
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                    {
                        Dictionary<string, object> m = AlibabaCloud.TeaUtil.Common.AssertAsMap(request.Body);
                        request_.Body = TeaCore.BytesReadable(AlibabaCloud.OpenApiUtil.Client.ToForm(m));
                        request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                    }
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Query))
                    {
                        request_.Query = request.Query;
                    }
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(authType, "Anonymous"))
                    {
                        string accessKeyId = GetAccessKeyId();
                        string accessKeySecret = GetAccessKeySecret();
                        string securityToken = GetSecurityToken();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(securityToken))
                        {
                            request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                            request_.Headers["x-acs-security-token"] = securityToken;
                        }
                        string stringToSign = AlibabaCloud.OpenApiUtil.Client.GetStringToSign(request_);
                        request_.Headers["authorization"] = "acs " + accessKeyId + ":" + AlibabaCloud.OpenApiUtil.Client.GetROASignature(stringToSign, accessKeySecret);
                    }
                    _lastRequest = request_;
                    TeaResponse response_ = TeaCore.DoAction(request_, runtime_);

                    if (AlibabaCloud.TeaUtil.Common.EqualNumber(response_.StatusCode, 204))
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                    if (AlibabaCloud.TeaUtil.Common.Is4xx(response_.StatusCode) || AlibabaCloud.TeaUtil.Common.Is5xx(response_.StatusCode))
                    {
                        object _res = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = AlibabaCloud.TeaUtil.Common.AssertAsMap(_res);
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + DefaultAny(err.Get("RequestId"), err.Get("requestId"))},
                            {"data", err},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                        };
                        return resp;
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "byte"))
                    {
                        byte[] byt = AlibabaCloud.TeaUtil.Common.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "json"))
                    {
                        object obj = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = AlibabaCloud.TeaUtil.Common.AssertAsMap(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                        };
                    }
                    else
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                }
                catch (Exception e)
                {
                    if (TeaCore.IsRetryable(e))
                    {
                        _lastException = e;
                        continue;
                    }
                    throw e;
                }
            }

            throw new TeaUnretryableException(_lastRequest, _lastException);
        }

        /**
         * Encapsulate the request and invoke the network with form body
         * @param action api name
         * @param version product version
         * @param protocol http or https
         * @param method e.g. GET
         * @param authType authorization type e.g. AK
         * @param pathname pathname of every api
         * @param bodyType response body type e.g. String
         * @param request object of OpenApiRequest
         * @param runtime which controls some details of call api, such as retry times
         * @return the response
         */
        public async Task<Dictionary<string, object>> DoROARequestWithFormAsync(string action, string version, string protocol, string method, string authType, string pathname, string bodyType, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"maxIdleConns", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.MaxIdleConns, _maxIdleConns)},
                {"retry", new Dictionary<string, object>
                {
                    {"retryable", runtime.Autoretry},
                    {"maxAttempts", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.MaxAttempts, 3)},
                }},
                {"backoff", new Dictionary<string, object>
                {
                    {"policy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.BackoffPolicy, "no")},
                    {"period", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.BackoffPeriod, 1)},
                }},
                {"ignoreSSL", runtime.IgnoreSSL},
            };

            TeaRequest _lastRequest = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retryTimes = 0;
            while (TeaCore.AllowRetry((IDictionary) runtime_["retry"], _retryTimes, _now))
            {
                if (_retryTimes > 0)
                {
                    int backoffTime = TeaCore.GetBackoffTime((IDictionary)runtime_["backoff"], _retryTimes);
                    if (backoffTime > 0)
                    {
                        TeaCore.Sleep(backoffTime);
                    }
                }
                _retryTimes = _retryTimes + 1;
                try
                {
                    TeaRequest request_ = new TeaRequest();
                    request_.Protocol = AlibabaCloud.TeaUtil.Common.DefaultString(_protocol, protocol);
                    request_.Method = method;
                    request_.Pathname = pathname;
                    request_.Headers = TeaConverter.merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"date", AlibabaCloud.TeaUtil.Common.GetDateUTCString()},
                            {"host", _endpoint},
                            {"accept", "application/json"},
                            {"x-acs-signature-nonce", AlibabaCloud.TeaUtil.Common.GetNonce()},
                            {"x-acs-signature-method", "HMAC-SHA1"},
                            {"x-acs-signature-version", "1.0"},
                            {"x-acs-version", version},
                            {"x-acs-action", action},
                            {"user-agent", AlibabaCloud.TeaUtil.Common.GetUserAgent(_userAgent)},
                        },
                        request.Headers
                    );
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                    {
                        Dictionary<string, object> m = AlibabaCloud.TeaUtil.Common.AssertAsMap(request.Body);
                        request_.Body = TeaCore.BytesReadable(AlibabaCloud.OpenApiUtil.Client.ToForm(m));
                        request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                    }
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Query))
                    {
                        request_.Query = request.Query;
                    }
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(authType, "Anonymous"))
                    {
                        string accessKeyId = await GetAccessKeyIdAsync();
                        string accessKeySecret = await GetAccessKeySecretAsync();
                        string securityToken = await GetSecurityTokenAsync();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(securityToken))
                        {
                            request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                            request_.Headers["x-acs-security-token"] = securityToken;
                        }
                        string stringToSign = AlibabaCloud.OpenApiUtil.Client.GetStringToSign(request_);
                        request_.Headers["authorization"] = "acs " + accessKeyId + ":" + AlibabaCloud.OpenApiUtil.Client.GetROASignature(stringToSign, accessKeySecret);
                    }
                    _lastRequest = request_;
                    TeaResponse response_ = await TeaCore.DoActionAsync(request_, runtime_);

                    if (AlibabaCloud.TeaUtil.Common.EqualNumber(response_.StatusCode, 204))
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                    if (AlibabaCloud.TeaUtil.Common.Is4xx(response_.StatusCode) || AlibabaCloud.TeaUtil.Common.Is5xx(response_.StatusCode))
                    {
                        object _res = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = AlibabaCloud.TeaUtil.Common.AssertAsMap(_res);
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + DefaultAny(err.Get("RequestId"), err.Get("requestId"))},
                            {"data", err},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                        };
                        return resp;
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "byte"))
                    {
                        byte[] byt = AlibabaCloud.TeaUtil.Common.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "json"))
                    {
                        object obj = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = AlibabaCloud.TeaUtil.Common.AssertAsMap(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                        };
                    }
                    else
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                }
                catch (Exception e)
                {
                    if (TeaCore.IsRetryable(e))
                    {
                        _lastException = e;
                        continue;
                    }
                    throw e;
                }
            }

            throw new TeaUnretryableException(_lastRequest, _lastException);
        }

        /**
         * Get user agent
         * @return user agent
         */
        public string GetUserAgent()
        {
            string userAgent = AlibabaCloud.TeaUtil.Common.GetUserAgent(_userAgent);
            return userAgent;
        }

        /**
         * Get accesskey id by using credential
         * @return accesskey id
         */
        public string GetAccessKeyId()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string accessKeyId = this._credential.GetAccessKeyId();
            return accessKeyId;
        }

        /**
         * Get accesskey id by using credential
         * @return accesskey id
         */
        public async Task<string> GetAccessKeyIdAsync()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string accessKeyId = await this._credential.GetAccessKeyIdAsync();
            return accessKeyId;
        }

        /**
         * Get accesskey secret by using credential
         * @return accesskey secret
         */
        public string GetAccessKeySecret()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string secret = this._credential.GetAccessKeySecret();
            return secret;
        }

        /**
         * Get accesskey secret by using credential
         * @return accesskey secret
         */
        public async Task<string> GetAccessKeySecretAsync()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string secret = await this._credential.GetAccessKeySecretAsync();
            return secret;
        }

        /**
         * Get security token by using credential
         * @return security token
         */
        public string GetSecurityToken()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string token = this._credential.GetSecurityToken();
            return token;
        }

        /**
         * Get security token by using credential
         * @return security token
         */
        public async Task<string> GetSecurityTokenAsync()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string token = await this._credential.GetSecurityTokenAsync();
            return token;
        }

        /**
         * If inputValue is not null, return it or return defaultValue
         * @param inputValue  users input value
         * @param defaultValue default value
         * @return the final result
         */
        public static object DefaultAny(object inputValue, object defaultValue)
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(inputValue))
            {
                return defaultValue;
            }
            return inputValue;
        }

        /**
         * If the endpointRule and config.endpoint are empty, throw error
         * @param config config contains the necessary information to create a client
         */
        public void CheckConfig(Config config)
        {
            if (AlibabaCloud.TeaUtil.Common.Empty(_endpointRule) && AlibabaCloud.TeaUtil.Common.Empty(config.Endpoint))
            {
                throw new TeaException(new Dictionary<string, string>
                {
                    {"code", "ParameterMissing"},
                    {"message", "'config.endpoint' can not be empty"},
                });
            }
        }

    }
}
