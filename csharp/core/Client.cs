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
        protected string _method;
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
        protected string _signatureVersion;
        protected string _signatureAlgorithm;
        protected Dictionary<string, string> _headers;
        protected AlibabaCloud.GatewaySpi.Client _spi;
        protected GlobalParameters _globalParameters;
        protected string _key;
        protected string _cert;
        protected string _ca;
        protected bool? _disableHttp2;
        protected string _tlsMinVersion;

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Init client with Config</para>
        /// </description>
        /// 
        /// <param name="config">
        /// config contains the necessary information to create a client
        /// </param>
        public Client(Config config)
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(config))
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
                };
                credentialConfig.SecurityToken = config.SecurityToken;
                this._credential = new Aliyun.Credentials.Client(credentialConfig);
            }
            else if (!AlibabaCloud.TeaUtil.Common.Empty(config.BearerToken))
            {
                Aliyun.Credentials.Models.Config cc = new Aliyun.Credentials.Models.Config
                {
                    Type = "bearer",
                    BearerToken = config.BearerToken,
                };
                this._credential = new Aliyun.Credentials.Client(cc);
            }
            else if (!AlibabaCloud.TeaUtil.Common.IsUnset(config.Credential))
            {
                this._credential = config.Credential;
            }
            this._endpoint = config.Endpoint;
            this._endpointType = config.EndpointType;
            this._network = config.Network;
            this._suffix = config.Suffix;
            this._protocol = config.Protocol;
            this._method = config.Method;
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
            this._signatureVersion = config.SignatureVersion;
            this._signatureAlgorithm = config.SignatureAlgorithm;
            this._globalParameters = config.GlobalParameters;
            this._key = config.Key;
            this._cert = config.Cert;
            this._ca = config.Ca;
            this._disableHttp2 = config.DisableHttp2;
            this._tlsMinVersion = config.TlsMinVersion;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Encapsulate the request and invoke the network</para>
        /// </description>
        /// 
        /// <param name="action">
        /// api name
        /// </param>
        /// <param name="version">
        /// product version
        /// </param>
        /// <param name="protocol">
        /// http or https
        /// </param>
        /// <param name="method">
        /// e.g. GET
        /// </param>
        /// <param name="authType">
        /// authorization type e.g. AK
        /// </param>
        /// <param name="bodyType">
        /// response body type e.g. String
        /// </param>
        /// <param name="request">
        /// object of OpenApiRequest
        /// </param>
        /// <param name="runtime">
        /// which controls some details of call api, such as retry times
        /// </param>
        /// 
        /// <returns>
        /// the response
        /// </returns>
        public Dictionary<string, object> DoRPCRequest(string action, string version, string protocol, string method, string authType, string bodyType, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"key", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Key, _key)},
                {"cert", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Cert, _cert)},
                {"ca", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Ca, _ca)},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5NetWork, _socks5NetWork)},
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
                {"tlsMinVersion", _tlsMinVersion},
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
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(_globalParameters))
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Queries))
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Headers))
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(runtime.ExtendsParameters))
                    {
                        AlibabaCloud.TeaUtil.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Headers))
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Queries))
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
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
                        globalQueries,
                        extendsQueries,
                        request.Query
                    );
                    Dictionary<string, string> headers = GetRpcHeaders();
                    if (AlibabaCloud.TeaUtil.Common.IsUnset(headers))
                    {
                        // endpoint is setted in product client
                        request_.Headers = TeaConverter.merge<string>
                        (
                            new Dictionary<string, string>()
                            {
                                {"host", _endpoint},
                                {"x-acs-version", version},
                                {"x-acs-action", action},
                                {"user-agent", GetUserAgent()},
                            },
                            globalHeaders,
                            extendsHeaders
                        );
                    }
                    else
                    {
                        request_.Headers = TeaConverter.merge<string>
                        (
                            new Dictionary<string, string>()
                            {
                                {"host", _endpoint},
                                {"x-acs-version", version},
                                {"x-acs-action", action},
                                {"user-agent", GetUserAgent()},
                            },
                            globalHeaders,
                            extendsHeaders,
                            headers
                        );
                    }
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                    {
                        Dictionary<string, object> m = AlibabaCloud.TeaUtil.Common.AssertAsMap(request.Body);
                        Dictionary<string, object> tmp = AlibabaCloud.TeaUtil.Common.AnyifyMapValue(AlibabaCloud.OpenApiUtil.Client.Query(m));
                        request_.Body = TeaCore.BytesReadable(AlibabaCloud.TeaUtil.Common.ToFormString(tmp));
                        request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                    }
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(authType, "Anonymous"))
                    {
                        if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
                        {
                            throw new TeaException(new Dictionary<string, string>
                            {
                                {"code", "InvalidCredentials"},
                                {"message", "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details."},
                            });
                        }
                        Aliyun.Credentials.Models.CredentialModel credentialModel = this._credential.GetCredential();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(credentialModel.ProviderName))
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string credentialType = credentialModel.Type;
                        if (AlibabaCloud.TeaUtil.Common.EqualString(credentialType, "bearer"))
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Query["BearerToken"] = bearerToken;
                            request_.Query["SignatureType"] = "BEARERTOKEN";
                        }
                        else if (AlibabaCloud.TeaUtil.Common.EqualString(credentialType, "id_token"))
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
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
                    }
                    _lastRequest = request_;
                    TeaResponse response_ = TeaCore.DoAction(request_, runtime_);

                    if (AlibabaCloud.TeaUtil.Common.Is4xx(response_.StatusCode) || AlibabaCloud.TeaUtil.Common.Is5xx(response_.StatusCode))
                    {
                        object _res = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = AlibabaCloud.TeaUtil.Common.AssertAsMap(_res);
                        object requestId = DefaultAny(err.Get("RequestId"), err.Get("requestId"));
                        err["statusCode"] = response_.StatusCode;
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", "" + DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + requestId},
                            {"data", err},
                            {"description", "" + DefaultAny(err.Get("Description"), err.Get("description"))},
                            {"accessDeniedDetail", DefaultAny(err.Get("AccessDeniedDetail"), err.Get("accessDeniedDetail"))},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Encapsulate the request and invoke the network</para>
        /// </description>
        /// 
        /// <param name="action">
        /// api name
        /// </param>
        /// <param name="version">
        /// product version
        /// </param>
        /// <param name="protocol">
        /// http or https
        /// </param>
        /// <param name="method">
        /// e.g. GET
        /// </param>
        /// <param name="authType">
        /// authorization type e.g. AK
        /// </param>
        /// <param name="bodyType">
        /// response body type e.g. String
        /// </param>
        /// <param name="request">
        /// object of OpenApiRequest
        /// </param>
        /// <param name="runtime">
        /// which controls some details of call api, such as retry times
        /// </param>
        /// 
        /// <returns>
        /// the response
        /// </returns>
        public async Task<Dictionary<string, object>> DoRPCRequestAsync(string action, string version, string protocol, string method, string authType, string bodyType, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"key", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Key, _key)},
                {"cert", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Cert, _cert)},
                {"ca", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Ca, _ca)},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5NetWork, _socks5NetWork)},
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
                {"tlsMinVersion", _tlsMinVersion},
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
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(_globalParameters))
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Queries))
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Headers))
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(runtime.ExtendsParameters))
                    {
                        AlibabaCloud.TeaUtil.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Headers))
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Queries))
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
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
                        globalQueries,
                        extendsQueries,
                        request.Query
                    );
                    Dictionary<string, string> headers = GetRpcHeaders();
                    if (AlibabaCloud.TeaUtil.Common.IsUnset(headers))
                    {
                        // endpoint is setted in product client
                        request_.Headers = TeaConverter.merge<string>
                        (
                            new Dictionary<string, string>()
                            {
                                {"host", _endpoint},
                                {"x-acs-version", version},
                                {"x-acs-action", action},
                                {"user-agent", GetUserAgent()},
                            },
                            globalHeaders,
                            extendsHeaders
                        );
                    }
                    else
                    {
                        request_.Headers = TeaConverter.merge<string>
                        (
                            new Dictionary<string, string>()
                            {
                                {"host", _endpoint},
                                {"x-acs-version", version},
                                {"x-acs-action", action},
                                {"user-agent", GetUserAgent()},
                            },
                            globalHeaders,
                            extendsHeaders,
                            headers
                        );
                    }
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                    {
                        Dictionary<string, object> m = AlibabaCloud.TeaUtil.Common.AssertAsMap(request.Body);
                        Dictionary<string, object> tmp = AlibabaCloud.TeaUtil.Common.AnyifyMapValue(AlibabaCloud.OpenApiUtil.Client.Query(m));
                        request_.Body = TeaCore.BytesReadable(AlibabaCloud.TeaUtil.Common.ToFormString(tmp));
                        request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                    }
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(authType, "Anonymous"))
                    {
                        if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
                        {
                            throw new TeaException(new Dictionary<string, string>
                            {
                                {"code", "InvalidCredentials"},
                                {"message", "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details."},
                            });
                        }
                        Aliyun.Credentials.Models.CredentialModel credentialModel = await this._credential.GetCredentialAsync();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(credentialModel.ProviderName))
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string credentialType = credentialModel.Type;
                        if (AlibabaCloud.TeaUtil.Common.EqualString(credentialType, "bearer"))
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Query["BearerToken"] = bearerToken;
                            request_.Query["SignatureType"] = "BEARERTOKEN";
                        }
                        else if (AlibabaCloud.TeaUtil.Common.EqualString(credentialType, "id_token"))
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
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
                    }
                    _lastRequest = request_;
                    TeaResponse response_ = await TeaCore.DoActionAsync(request_, runtime_);

                    if (AlibabaCloud.TeaUtil.Common.Is4xx(response_.StatusCode) || AlibabaCloud.TeaUtil.Common.Is5xx(response_.StatusCode))
                    {
                        object _res = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = AlibabaCloud.TeaUtil.Common.AssertAsMap(_res);
                        object requestId = DefaultAny(err.Get("RequestId"), err.Get("requestId"));
                        err["statusCode"] = response_.StatusCode;
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", "" + DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + requestId},
                            {"data", err},
                            {"description", "" + DefaultAny(err.Get("Description"), err.Get("description"))},
                            {"accessDeniedDetail", DefaultAny(err.Get("AccessDeniedDetail"), err.Get("accessDeniedDetail"))},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Encapsulate the request and invoke the network</para>
        /// </description>
        /// 
        /// <param name="action">
        /// api name
        /// </param>
        /// <param name="version">
        /// product version
        /// </param>
        /// <param name="protocol">
        /// http or https
        /// </param>
        /// <param name="method">
        /// e.g. GET
        /// </param>
        /// <param name="authType">
        /// authorization type e.g. AK
        /// </param>
        /// <param name="pathname">
        /// pathname of every api
        /// </param>
        /// <param name="bodyType">
        /// response body type e.g. String
        /// </param>
        /// <param name="request">
        /// object of OpenApiRequest
        /// </param>
        /// <param name="runtime">
        /// which controls some details of call api, such as retry times
        /// </param>
        /// 
        /// <returns>
        /// the response
        /// </returns>
        public Dictionary<string, object> DoROARequest(string action, string version, string protocol, string method, string authType, string pathname, string bodyType, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"key", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Key, _key)},
                {"cert", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Cert, _cert)},
                {"ca", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Ca, _ca)},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5NetWork, _socks5NetWork)},
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
                {"tlsMinVersion", _tlsMinVersion},
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
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(_globalParameters))
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Queries))
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Headers))
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(runtime.ExtendsParameters))
                    {
                        AlibabaCloud.TeaUtil.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Headers))
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Queries))
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
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
                        globalHeaders,
                        extendsHeaders,
                        request.Headers
                    );
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                    {
                        request_.Body = TeaCore.BytesReadable(AlibabaCloud.TeaUtil.Common.ToJSONString(request.Body));
                        request_.Headers["content-type"] = "application/json; charset=utf-8";
                    }
                    request_.Query = TeaConverter.merge<string>
                    (
                        globalQueries,
                        extendsQueries
                    );
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Query))
                    {
                        request_.Query = TeaConverter.merge<string>
                        (
                            request_.Query,
                            request.Query
                        );
                    }
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(authType, "Anonymous"))
                    {
                        if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
                        {
                            throw new TeaException(new Dictionary<string, string>
                            {
                                {"code", "InvalidCredentials"},
                                {"message", "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details."},
                            });
                        }
                        Aliyun.Credentials.Models.CredentialModel credentialModel = this._credential.GetCredential();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(credentialModel.ProviderName))
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string credentialType = credentialModel.Type;
                        if (AlibabaCloud.TeaUtil.Common.EqualString(credentialType, "bearer"))
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Headers["x-acs-bearer-token"] = bearerToken;
                            request_.Headers["x-acs-signature-type"] = "BEARERTOKEN";
                        }
                        else if (AlibabaCloud.TeaUtil.Common.EqualString(credentialType, "id_token"))
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!AlibabaCloud.TeaUtil.Common.Empty(securityToken))
                            {
                                request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                                request_.Headers["x-acs-security-token"] = securityToken;
                            }
                            string stringToSign = AlibabaCloud.OpenApiUtil.Client.GetStringToSign(request_);
                            request_.Headers["authorization"] = "acs " + accessKeyId + ":" + AlibabaCloud.OpenApiUtil.Client.GetROASignature(stringToSign, accessKeySecret);
                        }
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
                        object requestId = DefaultAny(err.Get("RequestId"), err.Get("requestId"));
                        requestId = DefaultAny(requestId, err.Get("requestid"));
                        err["statusCode"] = response_.StatusCode;
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", "" + DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + requestId},
                            {"data", err},
                            {"description", "" + DefaultAny(err.Get("Description"), err.Get("description"))},
                            {"accessDeniedDetail", DefaultAny(err.Get("AccessDeniedDetail"), err.Get("accessDeniedDetail"))},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Encapsulate the request and invoke the network</para>
        /// </description>
        /// 
        /// <param name="action">
        /// api name
        /// </param>
        /// <param name="version">
        /// product version
        /// </param>
        /// <param name="protocol">
        /// http or https
        /// </param>
        /// <param name="method">
        /// e.g. GET
        /// </param>
        /// <param name="authType">
        /// authorization type e.g. AK
        /// </param>
        /// <param name="pathname">
        /// pathname of every api
        /// </param>
        /// <param name="bodyType">
        /// response body type e.g. String
        /// </param>
        /// <param name="request">
        /// object of OpenApiRequest
        /// </param>
        /// <param name="runtime">
        /// which controls some details of call api, such as retry times
        /// </param>
        /// 
        /// <returns>
        /// the response
        /// </returns>
        public async Task<Dictionary<string, object>> DoROARequestAsync(string action, string version, string protocol, string method, string authType, string pathname, string bodyType, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"key", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Key, _key)},
                {"cert", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Cert, _cert)},
                {"ca", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Ca, _ca)},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5NetWork, _socks5NetWork)},
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
                {"tlsMinVersion", _tlsMinVersion},
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
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(_globalParameters))
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Queries))
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Headers))
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(runtime.ExtendsParameters))
                    {
                        AlibabaCloud.TeaUtil.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Headers))
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Queries))
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
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
                        globalHeaders,
                        extendsHeaders,
                        request.Headers
                    );
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                    {
                        request_.Body = TeaCore.BytesReadable(AlibabaCloud.TeaUtil.Common.ToJSONString(request.Body));
                        request_.Headers["content-type"] = "application/json; charset=utf-8";
                    }
                    request_.Query = TeaConverter.merge<string>
                    (
                        globalQueries,
                        extendsQueries
                    );
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Query))
                    {
                        request_.Query = TeaConverter.merge<string>
                        (
                            request_.Query,
                            request.Query
                        );
                    }
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(authType, "Anonymous"))
                    {
                        if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
                        {
                            throw new TeaException(new Dictionary<string, string>
                            {
                                {"code", "InvalidCredentials"},
                                {"message", "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details."},
                            });
                        }
                        Aliyun.Credentials.Models.CredentialModel credentialModel = await this._credential.GetCredentialAsync();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(credentialModel.ProviderName))
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string credentialType = credentialModel.Type;
                        if (AlibabaCloud.TeaUtil.Common.EqualString(credentialType, "bearer"))
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Headers["x-acs-bearer-token"] = bearerToken;
                            request_.Headers["x-acs-signature-type"] = "BEARERTOKEN";
                        }
                        else if (AlibabaCloud.TeaUtil.Common.EqualString(credentialType, "id_token"))
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!AlibabaCloud.TeaUtil.Common.Empty(securityToken))
                            {
                                request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                                request_.Headers["x-acs-security-token"] = securityToken;
                            }
                            string stringToSign = AlibabaCloud.OpenApiUtil.Client.GetStringToSign(request_);
                            request_.Headers["authorization"] = "acs " + accessKeyId + ":" + AlibabaCloud.OpenApiUtil.Client.GetROASignature(stringToSign, accessKeySecret);
                        }
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
                        object requestId = DefaultAny(err.Get("RequestId"), err.Get("requestId"));
                        requestId = DefaultAny(requestId, err.Get("requestid"));
                        err["statusCode"] = response_.StatusCode;
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", "" + DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + requestId},
                            {"data", err},
                            {"description", "" + DefaultAny(err.Get("Description"), err.Get("description"))},
                            {"accessDeniedDetail", DefaultAny(err.Get("AccessDeniedDetail"), err.Get("accessDeniedDetail"))},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Encapsulate the request and invoke the network with form body</para>
        /// </description>
        /// 
        /// <param name="action">
        /// api name
        /// </param>
        /// <param name="version">
        /// product version
        /// </param>
        /// <param name="protocol">
        /// http or https
        /// </param>
        /// <param name="method">
        /// e.g. GET
        /// </param>
        /// <param name="authType">
        /// authorization type e.g. AK
        /// </param>
        /// <param name="pathname">
        /// pathname of every api
        /// </param>
        /// <param name="bodyType">
        /// response body type e.g. String
        /// </param>
        /// <param name="request">
        /// object of OpenApiRequest
        /// </param>
        /// <param name="runtime">
        /// which controls some details of call api, such as retry times
        /// </param>
        /// 
        /// <returns>
        /// the response
        /// </returns>
        public Dictionary<string, object> DoROARequestWithForm(string action, string version, string protocol, string method, string authType, string pathname, string bodyType, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"key", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Key, _key)},
                {"cert", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Cert, _cert)},
                {"ca", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Ca, _ca)},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5NetWork, _socks5NetWork)},
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
                {"tlsMinVersion", _tlsMinVersion},
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
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(_globalParameters))
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Queries))
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Headers))
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(runtime.ExtendsParameters))
                    {
                        AlibabaCloud.TeaUtil.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Headers))
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Queries))
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
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
                        globalHeaders,
                        extendsHeaders,
                        request.Headers
                    );
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                    {
                        Dictionary<string, object> m = AlibabaCloud.TeaUtil.Common.AssertAsMap(request.Body);
                        request_.Body = TeaCore.BytesReadable(AlibabaCloud.OpenApiUtil.Client.ToForm(m));
                        request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                    }
                    request_.Query = TeaConverter.merge<string>
                    (
                        globalQueries,
                        extendsQueries
                    );
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Query))
                    {
                        request_.Query = TeaConverter.merge<string>
                        (
                            request_.Query,
                            request.Query
                        );
                    }
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(authType, "Anonymous"))
                    {
                        if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
                        {
                            throw new TeaException(new Dictionary<string, string>
                            {
                                {"code", "InvalidCredentials"},
                                {"message", "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details."},
                            });
                        }
                        Aliyun.Credentials.Models.CredentialModel credentialModel = this._credential.GetCredential();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(credentialModel.ProviderName))
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string credentialType = credentialModel.Type;
                        if (AlibabaCloud.TeaUtil.Common.EqualString(credentialType, "bearer"))
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Headers["x-acs-bearer-token"] = bearerToken;
                            request_.Headers["x-acs-signature-type"] = "BEARERTOKEN";
                        }
                        else if (AlibabaCloud.TeaUtil.Common.EqualString(credentialType, "id_token"))
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!AlibabaCloud.TeaUtil.Common.Empty(securityToken))
                            {
                                request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                                request_.Headers["x-acs-security-token"] = securityToken;
                            }
                            string stringToSign = AlibabaCloud.OpenApiUtil.Client.GetStringToSign(request_);
                            request_.Headers["authorization"] = "acs " + accessKeyId + ":" + AlibabaCloud.OpenApiUtil.Client.GetROASignature(stringToSign, accessKeySecret);
                        }
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
                        err["statusCode"] = response_.StatusCode;
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", "" + DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + DefaultAny(err.Get("RequestId"), err.Get("requestId"))},
                            {"data", err},
                            {"description", "" + DefaultAny(err.Get("Description"), err.Get("description"))},
                            {"accessDeniedDetail", DefaultAny(err.Get("AccessDeniedDetail"), err.Get("accessDeniedDetail"))},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Encapsulate the request and invoke the network with form body</para>
        /// </description>
        /// 
        /// <param name="action">
        /// api name
        /// </param>
        /// <param name="version">
        /// product version
        /// </param>
        /// <param name="protocol">
        /// http or https
        /// </param>
        /// <param name="method">
        /// e.g. GET
        /// </param>
        /// <param name="authType">
        /// authorization type e.g. AK
        /// </param>
        /// <param name="pathname">
        /// pathname of every api
        /// </param>
        /// <param name="bodyType">
        /// response body type e.g. String
        /// </param>
        /// <param name="request">
        /// object of OpenApiRequest
        /// </param>
        /// <param name="runtime">
        /// which controls some details of call api, such as retry times
        /// </param>
        /// 
        /// <returns>
        /// the response
        /// </returns>
        public async Task<Dictionary<string, object>> DoROARequestWithFormAsync(string action, string version, string protocol, string method, string authType, string pathname, string bodyType, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"key", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Key, _key)},
                {"cert", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Cert, _cert)},
                {"ca", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Ca, _ca)},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5NetWork, _socks5NetWork)},
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
                {"tlsMinVersion", _tlsMinVersion},
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
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(_globalParameters))
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Queries))
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Headers))
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(runtime.ExtendsParameters))
                    {
                        AlibabaCloud.TeaUtil.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Headers))
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Queries))
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
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
                        globalHeaders,
                        extendsHeaders,
                        request.Headers
                    );
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                    {
                        Dictionary<string, object> m = AlibabaCloud.TeaUtil.Common.AssertAsMap(request.Body);
                        request_.Body = TeaCore.BytesReadable(AlibabaCloud.OpenApiUtil.Client.ToForm(m));
                        request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                    }
                    request_.Query = TeaConverter.merge<string>
                    (
                        globalQueries,
                        extendsQueries
                    );
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Query))
                    {
                        request_.Query = TeaConverter.merge<string>
                        (
                            request_.Query,
                            request.Query
                        );
                    }
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(authType, "Anonymous"))
                    {
                        if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
                        {
                            throw new TeaException(new Dictionary<string, string>
                            {
                                {"code", "InvalidCredentials"},
                                {"message", "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details."},
                            });
                        }
                        Aliyun.Credentials.Models.CredentialModel credentialModel = await this._credential.GetCredentialAsync();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(credentialModel.ProviderName))
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string credentialType = credentialModel.Type;
                        if (AlibabaCloud.TeaUtil.Common.EqualString(credentialType, "bearer"))
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Headers["x-acs-bearer-token"] = bearerToken;
                            request_.Headers["x-acs-signature-type"] = "BEARERTOKEN";
                        }
                        else if (AlibabaCloud.TeaUtil.Common.EqualString(credentialType, "id_token"))
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!AlibabaCloud.TeaUtil.Common.Empty(securityToken))
                            {
                                request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                                request_.Headers["x-acs-security-token"] = securityToken;
                            }
                            string stringToSign = AlibabaCloud.OpenApiUtil.Client.GetStringToSign(request_);
                            request_.Headers["authorization"] = "acs " + accessKeyId + ":" + AlibabaCloud.OpenApiUtil.Client.GetROASignature(stringToSign, accessKeySecret);
                        }
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
                        err["statusCode"] = response_.StatusCode;
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", "" + DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + DefaultAny(err.Get("RequestId"), err.Get("requestId"))},
                            {"data", err},
                            {"description", "" + DefaultAny(err.Get("Description"), err.Get("description"))},
                            {"accessDeniedDetail", DefaultAny(err.Get("AccessDeniedDetail"), err.Get("accessDeniedDetail"))},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(bodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Encapsulate the request and invoke the network</para>
        /// </description>
        /// 
        /// <param name="action">
        /// api name
        /// </param>
        /// <param name="version">
        /// product version
        /// </param>
        /// <param name="protocol">
        /// http or https
        /// </param>
        /// <param name="method">
        /// e.g. GET
        /// </param>
        /// <param name="authType">
        /// authorization type e.g. AK
        /// </param>
        /// <param name="bodyType">
        /// response body type e.g. String
        /// </param>
        /// <param name="request">
        /// object of OpenApiRequest
        /// </param>
        /// <param name="runtime">
        /// which controls some details of call api, such as retry times
        /// </param>
        /// 
        /// <returns>
        /// the response
        /// </returns>
        public Dictionary<string, object> DoRequest(Params params_, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            params_.Validate();
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"key", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Key, _key)},
                {"cert", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Cert, _cert)},
                {"ca", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Ca, _ca)},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5NetWork, _socks5NetWork)},
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
                {"tlsMinVersion", _tlsMinVersion},
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
                    request_.Protocol = AlibabaCloud.TeaUtil.Common.DefaultString(_protocol, params_.Protocol);
                    request_.Method = params_.Method;
                    request_.Pathname = params_.Pathname;
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(_globalParameters))
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Queries))
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Headers))
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(runtime.ExtendsParameters))
                    {
                        AlibabaCloud.TeaUtil.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Headers))
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Queries))
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    request_.Query = TeaConverter.merge<string>
                    (
                        globalQueries,
                        extendsQueries,
                        request.Query
                    );
                    // endpoint is setted in product client
                    request_.Headers = TeaConverter.merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"host", _endpoint},
                            {"x-acs-version", params_.Version},
                            {"x-acs-action", params_.Action},
                            {"user-agent", GetUserAgent()},
                            {"x-acs-date", AlibabaCloud.OpenApiUtil.Client.GetTimestamp()},
                            {"x-acs-signature-nonce", AlibabaCloud.TeaUtil.Common.GetNonce()},
                            {"accept", "application/json"},
                        },
                        globalHeaders,
                        extendsHeaders,
                        request.Headers
                    );
                    if (AlibabaCloud.TeaUtil.Common.EqualString(params_.Style, "RPC"))
                    {
                        Dictionary<string, string> headers = GetRpcHeaders();
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(headers))
                        {
                            request_.Headers = TeaConverter.merge<string>
                            (
                                request_.Headers,
                                headers
                            );
                        }
                    }
                    string signatureAlgorithm = AlibabaCloud.TeaUtil.Common.DefaultString(_signatureAlgorithm, "ACS3-HMAC-SHA256");
                    string hashedRequestPayload = AlibabaCloud.OpenApiUtil.Client.HexEncode(AlibabaCloud.OpenApiUtil.Client.Hash(AlibabaCloud.TeaUtil.Common.ToBytes(""), signatureAlgorithm));
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Stream))
                    {
                        byte[] tmp = AlibabaCloud.TeaUtil.Common.ReadAsBytes(request.Stream);
                        hashedRequestPayload = AlibabaCloud.OpenApiUtil.Client.HexEncode(AlibabaCloud.OpenApiUtil.Client.Hash(tmp, signatureAlgorithm));
                        request_.Body = TeaCore.BytesReadable(tmp);
                        request_.Headers["content-type"] = "application/octet-stream";
                    }
                    else
                    {
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                        {
                            if (AlibabaCloud.TeaUtil.Common.EqualString(params_.ReqBodyType, "byte"))
                            {
                                byte[] byteObj = AlibabaCloud.TeaUtil.Common.AssertAsBytes(request.Body);
                                hashedRequestPayload = AlibabaCloud.OpenApiUtil.Client.HexEncode(AlibabaCloud.OpenApiUtil.Client.Hash(byteObj, signatureAlgorithm));
                                request_.Body = TeaCore.BytesReadable(byteObj);
                            }
                            else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.ReqBodyType, "json"))
                            {
                                string jsonObj = AlibabaCloud.TeaUtil.Common.ToJSONString(request.Body);
                                hashedRequestPayload = AlibabaCloud.OpenApiUtil.Client.HexEncode(AlibabaCloud.OpenApiUtil.Client.Hash(AlibabaCloud.TeaUtil.Common.ToBytes(jsonObj), signatureAlgorithm));
                                request_.Body = TeaCore.BytesReadable(jsonObj);
                                request_.Headers["content-type"] = "application/json; charset=utf-8";
                            }
                            else
                            {
                                Dictionary<string, object> m = AlibabaCloud.TeaUtil.Common.AssertAsMap(request.Body);
                                string formObj = AlibabaCloud.OpenApiUtil.Client.ToForm(m);
                                hashedRequestPayload = AlibabaCloud.OpenApiUtil.Client.HexEncode(AlibabaCloud.OpenApiUtil.Client.Hash(AlibabaCloud.TeaUtil.Common.ToBytes(formObj), signatureAlgorithm));
                                request_.Body = TeaCore.BytesReadable(formObj);
                                request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                            }
                        }
                    }
                    request_.Headers["x-acs-content-sha256"] = hashedRequestPayload;
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(params_.AuthType, "Anonymous"))
                    {
                        if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
                        {
                            throw new TeaException(new Dictionary<string, string>
                            {
                                {"code", "InvalidCredentials"},
                                {"message", "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details."},
                            });
                        }
                        Aliyun.Credentials.Models.CredentialModel credentialModel = this._credential.GetCredential();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(credentialModel.ProviderName))
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string authType = credentialModel.Type;
                        if (AlibabaCloud.TeaUtil.Common.EqualString(authType, "bearer"))
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Headers["x-acs-bearer-token"] = bearerToken;
                            if (AlibabaCloud.TeaUtil.Common.EqualString(params_.Style, "RPC"))
                            {
                                request_.Query["SignatureType"] = "BEARERTOKEN";
                            }
                            else
                            {
                                request_.Headers["x-acs-signature-type"] = "BEARERTOKEN";
                            }
                        }
                        else if (AlibabaCloud.TeaUtil.Common.EqualString(authType, "id_token"))
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!AlibabaCloud.TeaUtil.Common.Empty(securityToken))
                            {
                                request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                                request_.Headers["x-acs-security-token"] = securityToken;
                            }
                            request_.Headers["Authorization"] = AlibabaCloud.OpenApiUtil.Client.GetAuthorization(request_, signatureAlgorithm, hashedRequestPayload, accessKeyId, accessKeySecret);
                        }
                    }
                    _lastRequest = request_;
                    TeaResponse response_ = TeaCore.DoAction(request_, runtime_);

                    if (AlibabaCloud.TeaUtil.Common.Is4xx(response_.StatusCode) || AlibabaCloud.TeaUtil.Common.Is5xx(response_.StatusCode))
                    {
                        Dictionary<string, object> err = new Dictionary<string, object>(){};
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(response_.Headers.Get("content-type")) && AlibabaCloud.TeaUtil.Common.EqualString(response_.Headers.Get("content-type"), "text/xml;charset=utf-8"))
                        {
                            string _str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                            Dictionary<string, object> respMap = AlibabaCloud.TeaXML.Client.ParseXml(_str, null);
                            err = AlibabaCloud.TeaUtil.Common.AssertAsMap(respMap.Get("Error"));
                        }
                        else
                        {
                            object _res = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                            err = AlibabaCloud.TeaUtil.Common.AssertAsMap(_res);
                        }
                        err["statusCode"] = response_.StatusCode;
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", "" + DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + DefaultAny(err.Get("RequestId"), err.Get("requestId"))},
                            {"data", err},
                            {"description", "" + DefaultAny(err.Get("Description"), err.Get("description"))},
                            {"accessDeniedDetail", DefaultAny(err.Get("AccessDeniedDetail"), err.Get("accessDeniedDetail"))},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(params_.BodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                        return resp;
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.BodyType, "byte"))
                    {
                        byte[] byt = AlibabaCloud.TeaUtil.Common.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.BodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.BodyType, "json"))
                    {
                        object obj = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = AlibabaCloud.TeaUtil.Common.AssertAsMap(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.BodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else
                    {
                        string anything = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", anything},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Encapsulate the request and invoke the network</para>
        /// </description>
        /// 
        /// <param name="action">
        /// api name
        /// </param>
        /// <param name="version">
        /// product version
        /// </param>
        /// <param name="protocol">
        /// http or https
        /// </param>
        /// <param name="method">
        /// e.g. GET
        /// </param>
        /// <param name="authType">
        /// authorization type e.g. AK
        /// </param>
        /// <param name="bodyType">
        /// response body type e.g. String
        /// </param>
        /// <param name="request">
        /// object of OpenApiRequest
        /// </param>
        /// <param name="runtime">
        /// which controls some details of call api, such as retry times
        /// </param>
        /// 
        /// <returns>
        /// the response
        /// </returns>
        public async Task<Dictionary<string, object>> DoRequestAsync(Params params_, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            params_.Validate();
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"key", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Key, _key)},
                {"cert", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Cert, _cert)},
                {"ca", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Ca, _ca)},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5NetWork, _socks5NetWork)},
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
                {"tlsMinVersion", _tlsMinVersion},
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
                    request_.Protocol = AlibabaCloud.TeaUtil.Common.DefaultString(_protocol, params_.Protocol);
                    request_.Method = params_.Method;
                    request_.Pathname = params_.Pathname;
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(_globalParameters))
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Queries))
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Headers))
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(runtime.ExtendsParameters))
                    {
                        AlibabaCloud.TeaUtil.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Headers))
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Queries))
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    request_.Query = TeaConverter.merge<string>
                    (
                        globalQueries,
                        extendsQueries,
                        request.Query
                    );
                    // endpoint is setted in product client
                    request_.Headers = TeaConverter.merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"host", _endpoint},
                            {"x-acs-version", params_.Version},
                            {"x-acs-action", params_.Action},
                            {"user-agent", GetUserAgent()},
                            {"x-acs-date", AlibabaCloud.OpenApiUtil.Client.GetTimestamp()},
                            {"x-acs-signature-nonce", AlibabaCloud.TeaUtil.Common.GetNonce()},
                            {"accept", "application/json"},
                        },
                        globalHeaders,
                        extendsHeaders,
                        request.Headers
                    );
                    if (AlibabaCloud.TeaUtil.Common.EqualString(params_.Style, "RPC"))
                    {
                        Dictionary<string, string> headers = GetRpcHeaders();
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(headers))
                        {
                            request_.Headers = TeaConverter.merge<string>
                            (
                                request_.Headers,
                                headers
                            );
                        }
                    }
                    string signatureAlgorithm = AlibabaCloud.TeaUtil.Common.DefaultString(_signatureAlgorithm, "ACS3-HMAC-SHA256");
                    string hashedRequestPayload = AlibabaCloud.OpenApiUtil.Client.HexEncode(AlibabaCloud.OpenApiUtil.Client.Hash(AlibabaCloud.TeaUtil.Common.ToBytes(""), signatureAlgorithm));
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Stream))
                    {
                        byte[] tmp = AlibabaCloud.TeaUtil.Common.ReadAsBytes(request.Stream);
                        hashedRequestPayload = AlibabaCloud.OpenApiUtil.Client.HexEncode(AlibabaCloud.OpenApiUtil.Client.Hash(tmp, signatureAlgorithm));
                        request_.Body = TeaCore.BytesReadable(tmp);
                        request_.Headers["content-type"] = "application/octet-stream";
                    }
                    else
                    {
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(request.Body))
                        {
                            if (AlibabaCloud.TeaUtil.Common.EqualString(params_.ReqBodyType, "byte"))
                            {
                                byte[] byteObj = AlibabaCloud.TeaUtil.Common.AssertAsBytes(request.Body);
                                hashedRequestPayload = AlibabaCloud.OpenApiUtil.Client.HexEncode(AlibabaCloud.OpenApiUtil.Client.Hash(byteObj, signatureAlgorithm));
                                request_.Body = TeaCore.BytesReadable(byteObj);
                            }
                            else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.ReqBodyType, "json"))
                            {
                                string jsonObj = AlibabaCloud.TeaUtil.Common.ToJSONString(request.Body);
                                hashedRequestPayload = AlibabaCloud.OpenApiUtil.Client.HexEncode(AlibabaCloud.OpenApiUtil.Client.Hash(AlibabaCloud.TeaUtil.Common.ToBytes(jsonObj), signatureAlgorithm));
                                request_.Body = TeaCore.BytesReadable(jsonObj);
                                request_.Headers["content-type"] = "application/json; charset=utf-8";
                            }
                            else
                            {
                                Dictionary<string, object> m = AlibabaCloud.TeaUtil.Common.AssertAsMap(request.Body);
                                string formObj = AlibabaCloud.OpenApiUtil.Client.ToForm(m);
                                hashedRequestPayload = AlibabaCloud.OpenApiUtil.Client.HexEncode(AlibabaCloud.OpenApiUtil.Client.Hash(AlibabaCloud.TeaUtil.Common.ToBytes(formObj), signatureAlgorithm));
                                request_.Body = TeaCore.BytesReadable(formObj);
                                request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                            }
                        }
                    }
                    request_.Headers["x-acs-content-sha256"] = hashedRequestPayload;
                    if (!AlibabaCloud.TeaUtil.Common.EqualString(params_.AuthType, "Anonymous"))
                    {
                        if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
                        {
                            throw new TeaException(new Dictionary<string, string>
                            {
                                {"code", "InvalidCredentials"},
                                {"message", "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details."},
                            });
                        }
                        Aliyun.Credentials.Models.CredentialModel credentialModel = await this._credential.GetCredentialAsync();
                        if (!AlibabaCloud.TeaUtil.Common.Empty(credentialModel.ProviderName))
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string authType = credentialModel.Type;
                        if (AlibabaCloud.TeaUtil.Common.EqualString(authType, "bearer"))
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Headers["x-acs-bearer-token"] = bearerToken;
                            if (AlibabaCloud.TeaUtil.Common.EqualString(params_.Style, "RPC"))
                            {
                                request_.Query["SignatureType"] = "BEARERTOKEN";
                            }
                            else
                            {
                                request_.Headers["x-acs-signature-type"] = "BEARERTOKEN";
                            }
                        }
                        else if (AlibabaCloud.TeaUtil.Common.EqualString(authType, "id_token"))
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!AlibabaCloud.TeaUtil.Common.Empty(securityToken))
                            {
                                request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                                request_.Headers["x-acs-security-token"] = securityToken;
                            }
                            request_.Headers["Authorization"] = AlibabaCloud.OpenApiUtil.Client.GetAuthorization(request_, signatureAlgorithm, hashedRequestPayload, accessKeyId, accessKeySecret);
                        }
                    }
                    _lastRequest = request_;
                    TeaResponse response_ = await TeaCore.DoActionAsync(request_, runtime_);

                    if (AlibabaCloud.TeaUtil.Common.Is4xx(response_.StatusCode) || AlibabaCloud.TeaUtil.Common.Is5xx(response_.StatusCode))
                    {
                        Dictionary<string, object> err = new Dictionary<string, object>(){};
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(response_.Headers.Get("content-type")) && AlibabaCloud.TeaUtil.Common.EqualString(response_.Headers.Get("content-type"), "text/xml;charset=utf-8"))
                        {
                            string _str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                            Dictionary<string, object> respMap = AlibabaCloud.TeaXML.Client.ParseXml(_str, null);
                            err = AlibabaCloud.TeaUtil.Common.AssertAsMap(respMap.Get("Error"));
                        }
                        else
                        {
                            object _res = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                            err = AlibabaCloud.TeaUtil.Common.AssertAsMap(_res);
                        }
                        err["statusCode"] = response_.StatusCode;
                        throw new TeaException(new Dictionary<string, object>
                        {
                            {"code", "" + DefaultAny(err.Get("Code"), err.Get("code"))},
                            {"message", "code: " + response_.StatusCode + ", " + DefaultAny(err.Get("Message"), err.Get("message")) + " request id: " + DefaultAny(err.Get("RequestId"), err.Get("requestId"))},
                            {"data", err},
                            {"description", "" + DefaultAny(err.Get("Description"), err.Get("description"))},
                            {"accessDeniedDetail", DefaultAny(err.Get("AccessDeniedDetail"), err.Get("accessDeniedDetail"))},
                        });
                    }
                    if (AlibabaCloud.TeaUtil.Common.EqualString(params_.BodyType, "binary"))
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                        return resp;
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.BodyType, "byte"))
                    {
                        byte[] byt = AlibabaCloud.TeaUtil.Common.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.BodyType, "string"))
                    {
                        string str = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.BodyType, "json"))
                    {
                        object obj = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = AlibabaCloud.TeaUtil.Common.AssertAsMap(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.BodyType, "array"))
                    {
                        object arr = AlibabaCloud.TeaUtil.Common.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else
                    {
                        string anything = AlibabaCloud.TeaUtil.Common.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", anything},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
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

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Encapsulate the request and invoke the network</para>
        /// </description>
        /// 
        /// <param name="action">
        /// api name
        /// </param>
        /// <param name="version">
        /// product version
        /// </param>
        /// <param name="protocol">
        /// http or https
        /// </param>
        /// <param name="method">
        /// e.g. GET
        /// </param>
        /// <param name="authType">
        /// authorization type e.g. AK
        /// </param>
        /// <param name="bodyType">
        /// response body type e.g. String
        /// </param>
        /// <param name="request">
        /// object of OpenApiRequest
        /// </param>
        /// <param name="runtime">
        /// which controls some details of call api, such as retry times
        /// </param>
        /// 
        /// <returns>
        /// the response
        /// </returns>
        public Dictionary<string, object> Execute(Params params_, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            params_.Validate();
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"key", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Key, _key)},
                {"cert", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Cert, _cert)},
                {"ca", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Ca, _ca)},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5NetWork, _socks5NetWork)},
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
                {"disableHttp2", DefaultAny(_disableHttp2.Value, false)},
                {"tlsMinVersion", _tlsMinVersion},
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
                    // spi = new Gateway();//Gateway implements SPI，这一步在产品 SDK 中实例化
                    Dictionary<string, string> headers = GetRpcHeaders();
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(_globalParameters))
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Queries))
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Headers))
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(runtime.ExtendsParameters))
                    {
                        AlibabaCloud.TeaUtil.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Headers))
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Queries))
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    AlibabaCloud.GatewaySpi.Models.InterceptorContext.InterceptorContextRequest requestContext = new AlibabaCloud.GatewaySpi.Models.InterceptorContext.InterceptorContextRequest
                    {
                        Headers = TeaConverter.merge<string>
                        (
                            globalHeaders,
                            extendsHeaders,
                            request.Headers,
                            headers
                        ),
                        Query = TeaConverter.merge<string>
                        (
                            globalQueries,
                            extendsQueries,
                            request.Query
                        ),
                        Body = request.Body,
                        Stream = request.Stream,
                        HostMap = request.HostMap,
                        Pathname = params_.Pathname,
                        ProductId = _productId,
                        Action = params_.Action,
                        Version = params_.Version,
                        Protocol = AlibabaCloud.TeaUtil.Common.DefaultString(_protocol, params_.Protocol),
                        Method = AlibabaCloud.TeaUtil.Common.DefaultString(_method, params_.Method),
                        AuthType = params_.AuthType,
                        BodyType = params_.BodyType,
                        ReqBodyType = params_.ReqBodyType,
                        Style = params_.Style,
                        Credential = _credential,
                        SignatureVersion = _signatureVersion,
                        SignatureAlgorithm = _signatureAlgorithm,
                        UserAgent = GetUserAgent(),
                    };
                    AlibabaCloud.GatewaySpi.Models.InterceptorContext.InterceptorContextConfiguration configurationContext = new AlibabaCloud.GatewaySpi.Models.InterceptorContext.InterceptorContextConfiguration
                    {
                        RegionId = _regionId,
                        Endpoint = AlibabaCloud.TeaUtil.Common.DefaultString(request.EndpointOverride, _endpoint),
                        EndpointRule = _endpointRule,
                        EndpointMap = _endpointMap,
                        EndpointType = _endpointType,
                        Network = _network,
                        Suffix = _suffix,
                    };
                    AlibabaCloud.GatewaySpi.Models.InterceptorContext interceptorContext = new AlibabaCloud.GatewaySpi.Models.InterceptorContext
                    {
                        Request = requestContext,
                        Configuration = configurationContext,
                    };
                    AlibabaCloud.GatewaySpi.Models.AttributeMap attributeMap = new AlibabaCloud.GatewaySpi.Models.AttributeMap();
                    // 1. spi.modifyConfiguration(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                    this._spi.ModifyConfiguration(interceptorContext, attributeMap);
                    // 2. spi.modifyRequest(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                    this._spi.ModifyRequest(interceptorContext, attributeMap);
                    request_.Protocol = interceptorContext.Request.Protocol;
                    request_.Method = interceptorContext.Request.Method;
                    request_.Pathname = interceptorContext.Request.Pathname;
                    request_.Query = interceptorContext.Request.Query;
                    request_.Body = interceptorContext.Request.Stream;
                    request_.Headers = interceptorContext.Request.Headers;
                    _lastRequest = request_;
                    TeaResponse response_ = TeaCore.DoAction(request_, runtime_);

                    AlibabaCloud.GatewaySpi.Models.InterceptorContext.InterceptorContextResponse responseContext = new AlibabaCloud.GatewaySpi.Models.InterceptorContext.InterceptorContextResponse
                    {
                        StatusCode = response_.StatusCode,
                        Headers = response_.Headers,
                        Body = response_.Body,
                    };
                    interceptorContext.Response = responseContext;
                    // 3. spi.modifyResponse(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                    this._spi.ModifyResponse(interceptorContext, attributeMap);
                    return new Dictionary<string, object>
                    {
                        {"headers", interceptorContext.Response.Headers},
                        {"statusCode", interceptorContext.Response.StatusCode},
                        {"body", interceptorContext.Response.DeserializedBody},
                    };
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

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Encapsulate the request and invoke the network</para>
        /// </description>
        /// 
        /// <param name="action">
        /// api name
        /// </param>
        /// <param name="version">
        /// product version
        /// </param>
        /// <param name="protocol">
        /// http or https
        /// </param>
        /// <param name="method">
        /// e.g. GET
        /// </param>
        /// <param name="authType">
        /// authorization type e.g. AK
        /// </param>
        /// <param name="bodyType">
        /// response body type e.g. String
        /// </param>
        /// <param name="request">
        /// object of OpenApiRequest
        /// </param>
        /// <param name="runtime">
        /// which controls some details of call api, such as retry times
        /// </param>
        /// 
        /// <returns>
        /// the response
        /// </returns>
        public async Task<Dictionary<string, object>> ExecuteAsync(Params params_, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            params_.Validate();
            request.Validate();
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"timeouted", "retry"},
                {"key", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Key, _key)},
                {"cert", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Cert, _cert)},
                {"ca", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Ca, _ca)},
                {"readTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ReadTimeout, _readTimeout)},
                {"connectTimeout", AlibabaCloud.TeaUtil.Common.DefaultNumber(runtime.ConnectTimeout, _connectTimeout)},
                {"httpProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", AlibabaCloud.TeaUtil.Common.DefaultString(runtime.Socks5NetWork, _socks5NetWork)},
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
                {"disableHttp2", DefaultAny(_disableHttp2.Value, false)},
                {"tlsMinVersion", _tlsMinVersion},
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
                    // spi = new Gateway();//Gateway implements SPI，这一步在产品 SDK 中实例化
                    Dictionary<string, string> headers = GetRpcHeaders();
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(_globalParameters))
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Queries))
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(globalParams.Headers))
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!AlibabaCloud.TeaUtil.Common.IsUnset(runtime.ExtendsParameters))
                    {
                        AlibabaCloud.TeaUtil.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Headers))
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!AlibabaCloud.TeaUtil.Common.IsUnset(extendsParameters.Queries))
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    AlibabaCloud.GatewaySpi.Models.InterceptorContext.InterceptorContextRequest requestContext = new AlibabaCloud.GatewaySpi.Models.InterceptorContext.InterceptorContextRequest
                    {
                        Headers = TeaConverter.merge<string>
                        (
                            globalHeaders,
                            extendsHeaders,
                            request.Headers,
                            headers
                        ),
                        Query = TeaConverter.merge<string>
                        (
                            globalQueries,
                            extendsQueries,
                            request.Query
                        ),
                        Body = request.Body,
                        Stream = request.Stream,
                        HostMap = request.HostMap,
                        Pathname = params_.Pathname,
                        ProductId = _productId,
                        Action = params_.Action,
                        Version = params_.Version,
                        Protocol = AlibabaCloud.TeaUtil.Common.DefaultString(_protocol, params_.Protocol),
                        Method = AlibabaCloud.TeaUtil.Common.DefaultString(_method, params_.Method),
                        AuthType = params_.AuthType,
                        BodyType = params_.BodyType,
                        ReqBodyType = params_.ReqBodyType,
                        Style = params_.Style,
                        Credential = _credential,
                        SignatureVersion = _signatureVersion,
                        SignatureAlgorithm = _signatureAlgorithm,
                        UserAgent = GetUserAgent(),
                    };
                    AlibabaCloud.GatewaySpi.Models.InterceptorContext.InterceptorContextConfiguration configurationContext = new AlibabaCloud.GatewaySpi.Models.InterceptorContext.InterceptorContextConfiguration
                    {
                        RegionId = _regionId,
                        Endpoint = AlibabaCloud.TeaUtil.Common.DefaultString(request.EndpointOverride, _endpoint),
                        EndpointRule = _endpointRule,
                        EndpointMap = _endpointMap,
                        EndpointType = _endpointType,
                        Network = _network,
                        Suffix = _suffix,
                    };
                    AlibabaCloud.GatewaySpi.Models.InterceptorContext interceptorContext = new AlibabaCloud.GatewaySpi.Models.InterceptorContext
                    {
                        Request = requestContext,
                        Configuration = configurationContext,
                    };
                    AlibabaCloud.GatewaySpi.Models.AttributeMap attributeMap = new AlibabaCloud.GatewaySpi.Models.AttributeMap();
                    // 1. spi.modifyConfiguration(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                    await this._spi.ModifyConfigurationAsync(interceptorContext, attributeMap);
                    // 2. spi.modifyRequest(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                    await this._spi.ModifyRequestAsync(interceptorContext, attributeMap);
                    request_.Protocol = interceptorContext.Request.Protocol;
                    request_.Method = interceptorContext.Request.Method;
                    request_.Pathname = interceptorContext.Request.Pathname;
                    request_.Query = interceptorContext.Request.Query;
                    request_.Body = interceptorContext.Request.Stream;
                    request_.Headers = interceptorContext.Request.Headers;
                    _lastRequest = request_;
                    TeaResponse response_ = await TeaCore.DoActionAsync(request_, runtime_);

                    AlibabaCloud.GatewaySpi.Models.InterceptorContext.InterceptorContextResponse responseContext = new AlibabaCloud.GatewaySpi.Models.InterceptorContext.InterceptorContextResponse
                    {
                        StatusCode = response_.StatusCode,
                        Headers = response_.Headers,
                        Body = response_.Body,
                    };
                    interceptorContext.Response = responseContext;
                    // 3. spi.modifyResponse(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                    await this._spi.ModifyResponseAsync(interceptorContext, attributeMap);
                    return new Dictionary<string, object>
                    {
                        {"headers", interceptorContext.Response.Headers},
                        {"statusCode", interceptorContext.Response.StatusCode},
                        {"body", interceptorContext.Response.DeserializedBody},
                    };
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

        public Dictionary<string, object> CallApi(Params params_, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(params_))
            {
                throw new TeaException(new Dictionary<string, string>
                {
                    {"code", "ParameterMissing"},
                    {"message", "'params' can not be unset"},
                });
            }
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_signatureAlgorithm) || !AlibabaCloud.TeaUtil.Common.EqualString(_signatureAlgorithm, "v2"))
            {
                return DoRequest(params_, request, runtime);
            }
            else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.Style, "ROA") && AlibabaCloud.TeaUtil.Common.EqualString(params_.ReqBodyType, "json"))
            {
                return DoROARequest(params_.Action, params_.Version, params_.Protocol, params_.Method, params_.AuthType, params_.Pathname, params_.BodyType, request, runtime);
            }
            else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.Style, "ROA"))
            {
                return DoROARequestWithForm(params_.Action, params_.Version, params_.Protocol, params_.Method, params_.AuthType, params_.Pathname, params_.BodyType, request, runtime);
            }
            else
            {
                return DoRPCRequest(params_.Action, params_.Version, params_.Protocol, params_.Method, params_.AuthType, params_.BodyType, request, runtime);
            }
        }

        public async Task<Dictionary<string, object>> CallApiAsync(Params params_, OpenApiRequest request, AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime)
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(params_))
            {
                throw new TeaException(new Dictionary<string, string>
                {
                    {"code", "ParameterMissing"},
                    {"message", "'params' can not be unset"},
                });
            }
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_signatureAlgorithm) || !AlibabaCloud.TeaUtil.Common.EqualString(_signatureAlgorithm, "v2"))
            {
                return await DoRequestAsync(params_, request, runtime);
            }
            else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.Style, "ROA") && AlibabaCloud.TeaUtil.Common.EqualString(params_.ReqBodyType, "json"))
            {
                return await DoROARequestAsync(params_.Action, params_.Version, params_.Protocol, params_.Method, params_.AuthType, params_.Pathname, params_.BodyType, request, runtime);
            }
            else if (AlibabaCloud.TeaUtil.Common.EqualString(params_.Style, "ROA"))
            {
                return await DoROARequestWithFormAsync(params_.Action, params_.Version, params_.Protocol, params_.Method, params_.AuthType, params_.Pathname, params_.BodyType, request, runtime);
            }
            else
            {
                return await DoRPCRequestAsync(params_.Action, params_.Version, params_.Protocol, params_.Method, params_.AuthType, params_.BodyType, request, runtime);
            }
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get user agent</para>
        /// </description>
        /// 
        /// <returns>
        /// user agent
        /// </returns>
        public string GetUserAgent()
        {
            string userAgent = AlibabaCloud.TeaUtil.Common.GetUserAgent(_userAgent);
            return userAgent;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get accesskey id by using credential</para>
        /// </description>
        /// 
        /// <returns>
        /// accesskey id
        /// </returns>
        public string GetAccessKeyId()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string accessKeyId = this._credential.GetAccessKeyId();
            return accessKeyId;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get accesskey id by using credential</para>
        /// </description>
        /// 
        /// <returns>
        /// accesskey id
        /// </returns>
        public async Task<string> GetAccessKeyIdAsync()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string accessKeyId = await this._credential.GetAccessKeyIdAsync();
            return accessKeyId;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get accesskey secret by using credential</para>
        /// </description>
        /// 
        /// <returns>
        /// accesskey secret
        /// </returns>
        public string GetAccessKeySecret()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string secret = this._credential.GetAccessKeySecret();
            return secret;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get accesskey secret by using credential</para>
        /// </description>
        /// 
        /// <returns>
        /// accesskey secret
        /// </returns>
        public async Task<string> GetAccessKeySecretAsync()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string secret = await this._credential.GetAccessKeySecretAsync();
            return secret;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get security token by using credential</para>
        /// </description>
        /// 
        /// <returns>
        /// security token
        /// </returns>
        public string GetSecurityToken()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string token = this._credential.GetSecurityToken();
            return token;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get security token by using credential</para>
        /// </description>
        /// 
        /// <returns>
        /// security token
        /// </returns>
        public async Task<string> GetSecurityTokenAsync()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string token = await this._credential.GetSecurityTokenAsync();
            return token;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get bearer token by credential</para>
        /// </description>
        /// 
        /// <returns>
        /// bearer token
        /// </returns>
        public string GetBearerToken()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string token = this._credential.GetBearerToken();
            return token;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get bearer token by credential</para>
        /// </description>
        /// 
        /// <returns>
        /// bearer token
        /// </returns>
        public async Task<string> GetBearerTokenAsync()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string token = this._credential.GetBearerToken();
            return token;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get credential type by credential</para>
        /// </description>
        /// 
        /// <returns>
        /// credential type e.g. access_key
        /// </returns>
        public string GetType()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string authType = this._credential.GetType();
            return authType;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get credential type by credential</para>
        /// </description>
        /// 
        /// <returns>
        /// credential type e.g. access_key
        /// </returns>
        public async Task<string> GetTypeAsync()
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(_credential))
            {
                return "";
            }
            string authType = this._credential.GetType();
            return authType;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>If inputValue is not null, return it or return defaultValue</para>
        /// </description>
        /// 
        /// <param name="inputValue">
        /// users input value
        /// </param>
        /// <param name="defaultValue">
        /// default value
        /// </param>
        /// 
        /// <returns>
        /// the final result
        /// </returns>
        public static object DefaultAny(object inputValue, object defaultValue)
        {
            if (AlibabaCloud.TeaUtil.Common.IsUnset(inputValue))
            {
                return defaultValue;
            }
            return inputValue;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>If the endpointRule and config.endpoint are empty, throw error</para>
        /// </description>
        /// 
        /// <param name="config">
        /// config contains the necessary information to create a client
        /// </param>
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

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>set gateway client</para>
        /// </description>
        /// 
        /// <param name="spi">
        /// .
        /// </param>
        public void SetGatewayClient(AlibabaCloud.GatewaySpi.Client spi)
        {
            this._spi = spi;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>set RPC header for debug</para>
        /// </description>
        /// 
        /// <param name="headers">
        /// headers for debug, this header can be used only once.
        /// </param>
        public void SetRpcHeaders(Dictionary<string, string> headers)
        {
            this._headers = headers;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>get RPC header for debug</para>
        /// </description>
        public Dictionary<string, string> GetRpcHeaders()
        {
            Dictionary<string, string> headers = _headers;
            this._headers = null;
            return headers;
        }

    }
}
