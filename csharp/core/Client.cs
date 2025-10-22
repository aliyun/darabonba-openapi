// This file is auto-generated, don't edit it. Thanks.

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Darabonba.Utils;
using CredentialClient = Aliyun.Credentials.Client;
using SPIClient = AlibabaCloud.GatewaySpi.Client;
using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.GatewaySpi.Models;
using AlibabaCloud.OpenApiClient.Exceptions;
using System.Threading;
using Aliyun.Credentials.Models;

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
        protected CredentialClient _credential;
        protected string _signatureVersion;
        protected string _signatureAlgorithm;
        protected Dictionary<string, string> _headers;
        protected SPIClient _spi;
        protected GlobalParameters _globalParameters;
        protected string _key;
        protected string _cert;
        protected string _ca;
        protected bool? _disableHttp2;
        protected Darabonba.RetryPolicy.RetryOptions _retryOptions;
        protected string _tlsMinVersion;
        protected AttributeMap _attributeMap;

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Init client with Config</para>
        /// </description>
        /// 
        /// <param name="config">
        /// config contains the necessary information to create a client
        /// </param>
        public Client(AlibabaCloud.OpenApiClient.Models.Config config)
        {
            if (config.IsNull())
            {
                throw new ClientException
                {
                    Code = "ParameterMissing",
                    Message = "'config' can not be unset",
                };
            }
            if ((!config.AccessKeyId.IsNull() && config.AccessKeyId != "") && (!config.AccessKeySecret.IsNull() && config.AccessKeySecret != ""))
            {
                if (!config.SecurityToken.IsNull() && config.SecurityToken != "")
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
                this._credential = new CredentialClient(credentialConfig);
            }
            else if (!config.BearerToken.IsNull() && config.BearerToken != "")
            {
                Aliyun.Credentials.Models.Config cc = new Aliyun.Credentials.Models.Config
                {
                    Type = "bearer",
                    BearerToken = config.BearerToken,
                };
                this._credential = new CredentialClient(cc);
            }
            else if (!config.Credential.IsNull())
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
            this._retryOptions = config.RetryOptions;
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
        public Dictionary<string, object> DoRPCRequest(string action, string version, string protocol, string method, string authType, string bodyType, OpenApiRequest request, Darabonba.Models.RuntimeOptions runtime)
        {
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"key", (string)Darabonba.Core.GetDefaultValue(runtime.Key, _key)},
                {"cert", (string)Darabonba.Core.GetDefaultValue(runtime.Cert, _cert)},
                {"ca", (string)Darabonba.Core.GetDefaultValue(runtime.Ca, _ca)},
                {"readTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ReadTimeout, _readTimeout))},
                {"connectTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ConnectTimeout, _connectTimeout))},
                {"httpProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", (string)Darabonba.Core.GetDefaultValue(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5NetWork, _socks5NetWork)},
                {"maxIdleConns", (int?)(Darabonba.Core.GetDefaultValue(runtime.MaxIdleConns, _maxIdleConns))},
                {"retryOptions", _retryOptions},
                {"ignoreSSL", runtime.IgnoreSSL},
                {"tlsMinVersion", _tlsMinVersion},
            };

            Darabonba.RetryPolicy.RetryPolicyContext _retryPolicyContext = null;
            Darabonba.Request _lastRequest = null;
            Darabonba.Response _lastResponse = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retriesAttempted = 0;
            _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
            {
                RetriesAttempted = _retriesAttempted
            };
            while (Darabonba.Core.ShouldRetry((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext))
            {
                if (_retriesAttempted > 0)
                {
                    int backoffTime = Darabonba.Core.GetBackoffDelay((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext);
                    if (backoffTime > 0)
                    {
                        Thread.Sleep(backoffTime);
                    }
                }
                try
                {
                    Darabonba.Request request_ = new Darabonba.Request();
                    request_.Protocol = (string)Darabonba.Core.GetDefaultValue(_protocol, protocol);
                    request_.Method = method;
                    request_.Pathname = "/";
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!_globalParameters.IsNull())
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!globalParams.Queries.IsNull())
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!globalParams.Headers.IsNull())
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!runtime.ExtendsParameters.IsNull())
                    {
                        Darabonba.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!extendsParameters.Headers.IsNull())
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!extendsParameters.Queries.IsNull())
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    request_.Query = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"Action", action},
                            {"Format", "json"},
                            {"Version", version},
                            {"Timestamp", Utils.GetTimestamp()},
                            {"SignatureNonce", Utils.GetNonce()},
                        },
                        globalQueries,
                        extendsQueries,
                        request.Query
                    );
                    Dictionary<string, string> headers = GetRpcHeaders();
                    if (headers.IsNull())
                    {
                        // endpoint is setted in product client
                        request_.Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                        (
                            new Dictionary<string, string>()
                            {
                                {"host", _endpoint},
                                {"x-acs-version", version},
                                {"x-acs-action", action},
                                {"user-agent", Utils.GetUserAgent(_userAgent)},
                            },
                            globalHeaders,
                            extendsHeaders,
                            request.Headers
                        );
                    }
                    else
                    {
                        request_.Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                        (
                            new Dictionary<string, string>()
                            {
                                {"host", _endpoint},
                                {"x-acs-version", version},
                                {"x-acs-action", action},
                                {"user-agent", Utils.GetUserAgent(_userAgent)},
                            },
                            globalHeaders,
                            extendsHeaders,
                            request.Headers,
                            headers
                        );
                    }
                    if (!request.Body.IsNull())
                    {
                        Dictionary<string, object> m = (Dictionary<string, object>)(request.Body);
                        Dictionary<string, object> tmp = Darabonba.Core.ToObject(Utils.Query(m));
                        request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(Darabonba.Utils.FormUtils.ToFormString(tmp));
                        request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                    }
                    if (authType != "Anonymous")
                    {
                        if (_credential.IsNull())
                        {
                            throw new ClientException
                            {
                                Code = "InvalidCredentials",
                                Message = "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.",
                            };
                        }
                        CredentialModel credentialModel = this._credential.GetCredential();
                        if (!credentialModel.ProviderName.IsNull())
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string credentialType = credentialModel.Type;
                        if (credentialType == "bearer")
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Query["BearerToken"] = bearerToken;
                            request_.Query["SignatureType"] = "BEARERTOKEN";
                        }
                        else if (credentialType == "id_token")
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!securityToken.IsNull() && securityToken != "")
                            {
                                request_.Query["SecurityToken"] = securityToken;
                            }
                            request_.Query["SignatureMethod"] = "HMAC-SHA1";
                            request_.Query["SignatureVersion"] = "1.0";
                            request_.Query["AccessKeyId"] = accessKeyId;
                            Dictionary<string, object> t = null;
                            if (!request.Body.IsNull())
                            {
                                t = (Dictionary<string, object>)(request.Body);
                            }
                            Dictionary<string, string> signedParam = Darabonba.Utils.ConverterUtils.Merge<string>
                            (
                                request_.Query,
                                Utils.Query(t)
                            );
                            request_.Query["Signature"] = Utils.GetRPCSignature(signedParam, request_.Method, accessKeySecret);
                        }
                    }
                    _lastRequest = request_;
                    Darabonba.Response response_ = Darabonba.Core.DoAction(request_, runtime_);
                    _lastResponse = response_;

                    if ((response_.StatusCode >= 400) && (response_.StatusCode < 600))
                    {
                        object _res = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = (Dictionary<string, object>)(_res);
                        object requestId = Darabonba.Core.GetDefaultValue(err.Get("RequestId"), err.Get("requestId"));
                        object code = Darabonba.Core.GetDefaultValue(err.Get("Code"), err.Get("code"));
                        if (("" + code == "Throttling") || ("" + code == "Throttling.User") || ("" + code == "Throttling.Api"))
                        {
                            throw new ThrottlingException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                RetryAfter = Utils.GetThrottlingTimeLeft(response_.Headers),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                        else if ((response_.StatusCode >= 400) && (response_.StatusCode < 500))
                        {
                            throw new ClientException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                AccessDeniedDetail = GetAccessDeniedDetail(err),
                                RequestId = "" + requestId,
                            };
                        }
                        else
                        {
                            throw new ServerException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                    }
                    if (bodyType == "binary")
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                        return resp;
                    }
                    else if (bodyType == "byte")
                    {
                        byte[] byt = Darabonba.Utils.StreamUtils.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "string")
                    {
                        string _str = Darabonba.Utils.StreamUtils.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", _str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "json")
                    {
                        object obj = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = (Dictionary<string, object>)(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "array")
                    {
                        object arr = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
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
                    _retriesAttempted++;
                    _lastException = e;
                    _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
                    {
                        RetriesAttempted = _retriesAttempted,
                        Request = _lastRequest,
                        Response = _lastResponse,
                        Exception = _lastException
                    };
                }
            }

            throw Darabonba.Core.ThrowException(_retryPolicyContext);
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
        public async Task<Dictionary<string, object>> DoRPCRequestAsync(string action, string version, string protocol, string method, string authType, string bodyType, OpenApiRequest request, Darabonba.Models.RuntimeOptions runtime)
        {
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"key", (string)Darabonba.Core.GetDefaultValue(runtime.Key, _key)},
                {"cert", (string)Darabonba.Core.GetDefaultValue(runtime.Cert, _cert)},
                {"ca", (string)Darabonba.Core.GetDefaultValue(runtime.Ca, _ca)},
                {"readTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ReadTimeout, _readTimeout))},
                {"connectTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ConnectTimeout, _connectTimeout))},
                {"httpProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", (string)Darabonba.Core.GetDefaultValue(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5NetWork, _socks5NetWork)},
                {"maxIdleConns", (int?)(Darabonba.Core.GetDefaultValue(runtime.MaxIdleConns, _maxIdleConns))},
                {"retryOptions", _retryOptions},
                {"ignoreSSL", runtime.IgnoreSSL},
                {"tlsMinVersion", _tlsMinVersion},
            };

            Darabonba.RetryPolicy.RetryPolicyContext _retryPolicyContext = null;
            Darabonba.Request _lastRequest = null;
            Darabonba.Response _lastResponse = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retriesAttempted = 0;
            _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
            {
                RetriesAttempted = _retriesAttempted
            };
            while (Darabonba.Core.ShouldRetry((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext))
            {
                if (_retriesAttempted > 0)
                {
                    int backoffTime = Darabonba.Core.GetBackoffDelay((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext);
                    if (backoffTime > 0)
                    {
                        await Task.Delay(backoffTime);
                    }
                }
                try
                {
                    Darabonba.Request request_ = new Darabonba.Request();
                    request_.Protocol = (string)Darabonba.Core.GetDefaultValue(_protocol, protocol);
                    request_.Method = method;
                    request_.Pathname = "/";
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!_globalParameters.IsNull())
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!globalParams.Queries.IsNull())
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!globalParams.Headers.IsNull())
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!runtime.ExtendsParameters.IsNull())
                    {
                        Darabonba.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!extendsParameters.Headers.IsNull())
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!extendsParameters.Queries.IsNull())
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    request_.Query = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"Action", action},
                            {"Format", "json"},
                            {"Version", version},
                            {"Timestamp", Utils.GetTimestamp()},
                            {"SignatureNonce", Utils.GetNonce()},
                        },
                        globalQueries,
                        extendsQueries,
                        request.Query
                    );
                    Dictionary<string, string> headers = GetRpcHeaders();
                    if (headers.IsNull())
                    {
                        // endpoint is setted in product client
                        request_.Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                        (
                            new Dictionary<string, string>()
                            {
                                {"host", _endpoint},
                                {"x-acs-version", version},
                                {"x-acs-action", action},
                                {"user-agent", Utils.GetUserAgent(_userAgent)},
                            },
                            globalHeaders,
                            extendsHeaders,
                            request.Headers
                        );
                    }
                    else
                    {
                        request_.Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                        (
                            new Dictionary<string, string>()
                            {
                                {"host", _endpoint},
                                {"x-acs-version", version},
                                {"x-acs-action", action},
                                {"user-agent", Utils.GetUserAgent(_userAgent)},
                            },
                            globalHeaders,
                            extendsHeaders,
                            request.Headers,
                            headers
                        );
                    }
                    if (!request.Body.IsNull())
                    {
                        Dictionary<string, object> m = (Dictionary<string, object>)(request.Body);
                        Dictionary<string, object> tmp = Darabonba.Core.ToObject(Utils.Query(m));
                        request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(Darabonba.Utils.FormUtils.ToFormString(tmp));
                        request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                    }
                    if (authType != "Anonymous")
                    {
                        if (_credential.IsNull())
                        {
                            throw new ClientException
                            {
                                Code = "InvalidCredentials",
                                Message = "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.",
                            };
                        }
                        CredentialModel credentialModel = await this._credential.GetCredentialAsync();
                        if (!credentialModel.ProviderName.IsNull())
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string credentialType = credentialModel.Type;
                        if (credentialType == "bearer")
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Query["BearerToken"] = bearerToken;
                            request_.Query["SignatureType"] = "BEARERTOKEN";
                        }
                        else if (credentialType == "id_token")
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!securityToken.IsNull() && securityToken != "")
                            {
                                request_.Query["SecurityToken"] = securityToken;
                            }
                            request_.Query["SignatureMethod"] = "HMAC-SHA1";
                            request_.Query["SignatureVersion"] = "1.0";
                            request_.Query["AccessKeyId"] = accessKeyId;
                            Dictionary<string, object> t = null;
                            if (!request.Body.IsNull())
                            {
                                t = (Dictionary<string, object>)(request.Body);
                            }
                            Dictionary<string, string> signedParam = Darabonba.Utils.ConverterUtils.Merge<string>
                            (
                                request_.Query,
                                Utils.Query(t)
                            );
                            request_.Query["Signature"] = Utils.GetRPCSignature(signedParam, request_.Method, accessKeySecret);
                        }
                    }
                    _lastRequest = request_;
                    Darabonba.Response response_ = await Darabonba.Core.DoActionAsync(request_, runtime_);
                    _lastResponse = response_;

                    if ((response_.StatusCode >= 400) && (response_.StatusCode < 600))
                    {
                        object _res = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = (Dictionary<string, object>)(_res);
                        object requestId = Darabonba.Core.GetDefaultValue(err.Get("RequestId"), err.Get("requestId"));
                        object code = Darabonba.Core.GetDefaultValue(err.Get("Code"), err.Get("code"));
                        if (("" + code == "Throttling") || ("" + code == "Throttling.User") || ("" + code == "Throttling.Api"))
                        {
                            throw new ThrottlingException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                RetryAfter = Utils.GetThrottlingTimeLeft(response_.Headers),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                        else if ((response_.StatusCode >= 400) && (response_.StatusCode < 500))
                        {
                            throw new ClientException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                AccessDeniedDetail = GetAccessDeniedDetail(err),
                                RequestId = "" + requestId,
                            };
                        }
                        else
                        {
                            throw new ServerException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                    }
                    if (bodyType == "binary")
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                        return resp;
                    }
                    else if (bodyType == "byte")
                    {
                        byte[] byt = Darabonba.Utils.StreamUtils.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "string")
                    {
                        string _str = Darabonba.Utils.StreamUtils.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", _str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "json")
                    {
                        object obj = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = (Dictionary<string, object>)(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "array")
                    {
                        object arr = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
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
                    _retriesAttempted++;
                    _lastException = e;
                    _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
                    {
                        RetriesAttempted = _retriesAttempted,
                        Request = _lastRequest,
                        Response = _lastResponse,
                        Exception = _lastException
                    };
                }
            }

            throw Darabonba.Core.ThrowException(_retryPolicyContext);
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
        public Dictionary<string, object> DoROARequest(string action, string version, string protocol, string method, string authType, string pathname, string bodyType, OpenApiRequest request, Darabonba.Models.RuntimeOptions runtime)
        {
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"key", (string)Darabonba.Core.GetDefaultValue(runtime.Key, _key)},
                {"cert", (string)Darabonba.Core.GetDefaultValue(runtime.Cert, _cert)},
                {"ca", (string)Darabonba.Core.GetDefaultValue(runtime.Ca, _ca)},
                {"readTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ReadTimeout, _readTimeout))},
                {"connectTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ConnectTimeout, _connectTimeout))},
                {"httpProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", (string)Darabonba.Core.GetDefaultValue(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5NetWork, _socks5NetWork)},
                {"maxIdleConns", (int?)(Darabonba.Core.GetDefaultValue(runtime.MaxIdleConns, _maxIdleConns))},
                {"retryOptions", _retryOptions},
                {"ignoreSSL", runtime.IgnoreSSL},
                {"tlsMinVersion", _tlsMinVersion},
            };

            Darabonba.RetryPolicy.RetryPolicyContext _retryPolicyContext = null;
            Darabonba.Request _lastRequest = null;
            Darabonba.Response _lastResponse = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retriesAttempted = 0;
            _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
            {
                RetriesAttempted = _retriesAttempted
            };
            while (Darabonba.Core.ShouldRetry((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext))
            {
                if (_retriesAttempted > 0)
                {
                    int backoffTime = Darabonba.Core.GetBackoffDelay((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext);
                    if (backoffTime > 0)
                    {
                        Thread.Sleep(backoffTime);
                    }
                }
                try
                {
                    Darabonba.Request request_ = new Darabonba.Request();
                    request_.Protocol = (string)Darabonba.Core.GetDefaultValue(_protocol, protocol);
                    request_.Method = method;
                    request_.Pathname = pathname;
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!_globalParameters.IsNull())
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!globalParams.Queries.IsNull())
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!globalParams.Headers.IsNull())
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!runtime.ExtendsParameters.IsNull())
                    {
                        Darabonba.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!extendsParameters.Headers.IsNull())
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!extendsParameters.Queries.IsNull())
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    request_.Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"date", Utils.GetDateUTCString()},
                            {"host", _endpoint},
                            {"accept", "application/json"},
                            {"x-acs-signature-nonce", Utils.GetNonce()},
                            {"x-acs-signature-method", "HMAC-SHA1"},
                            {"x-acs-signature-version", "1.0"},
                            {"x-acs-version", version},
                            {"x-acs-action", action},
                            {"user-agent", Utils.GetUserAgent(_userAgent)},
                        },
                        globalHeaders,
                        extendsHeaders,
                        request.Headers
                    );
                    if (!request.Body.IsNull())
                    {
                        request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(Darabonba.Utils.JSONUtils.SerializeObject(request.Body));
                        request_.Headers["content-type"] = "application/json; charset=utf-8";
                    }
                    request_.Query = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        globalQueries,
                        extendsQueries
                    );
                    if (!request.Query.IsNull())
                    {
                        request_.Query = Darabonba.Utils.ConverterUtils.Merge<string>
                        (
                            request_.Query,
                            request.Query
                        );
                    }
                    if (authType != "Anonymous")
                    {
                        if (_credential.IsNull())
                        {
                            throw new ClientException
                            {
                                Code = "InvalidCredentials",
                                Message = "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.",
                            };
                        }
                        CredentialModel credentialModel = this._credential.GetCredential();
                        if (!credentialModel.ProviderName.IsNull())
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string credentialType = credentialModel.Type;
                        if (credentialType == "bearer")
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Headers["x-acs-bearer-token"] = bearerToken;
                            request_.Headers["x-acs-signature-type"] = "BEARERTOKEN";
                        }
                        else if (credentialType == "id_token")
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!securityToken.IsNull() && securityToken != "")
                            {
                                request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                                request_.Headers["x-acs-security-token"] = securityToken;
                            }
                            string stringToSign = Utils.GetStringToSign(request_);
                            request_.Headers["authorization"] = "acs " + accessKeyId + ":" + Utils.GetROASignature(stringToSign, accessKeySecret);
                        }
                    }
                    _lastRequest = request_;
                    Darabonba.Response response_ = Darabonba.Core.DoAction(request_, runtime_);
                    _lastResponse = response_;

                    if (response_.StatusCode == 204)
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                    if ((response_.StatusCode >= 400) && (response_.StatusCode < 600))
                    {
                        object _res = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = (Dictionary<string, object>)(_res);
                        string requestId = (string)Darabonba.Core.GetDefaultValue(err.Get("RequestId"), err.Get("requestId"));
                        requestId = (string)Darabonba.Core.GetDefaultValue(requestId, err.Get("requestid"));
                        string code = (string)Darabonba.Core.GetDefaultValue(err.Get("Code"), err.Get("code"));
                        if (("" + code == "Throttling") || ("" + code == "Throttling.User") || ("" + code == "Throttling.Api"))
                        {
                            throw new ThrottlingException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                RetryAfter = Utils.GetThrottlingTimeLeft(response_.Headers),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                        else if ((response_.StatusCode >= 400) && (response_.StatusCode < 500))
                        {
                            throw new ClientException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                AccessDeniedDetail = GetAccessDeniedDetail(err),
                                RequestId = "" + requestId,
                            };
                        }
                        else
                        {
                            throw new ServerException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                    }
                    if (bodyType == "binary")
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                        return resp;
                    }
                    else if (bodyType == "byte")
                    {
                        byte[] byt = Darabonba.Utils.StreamUtils.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "string")
                    {
                        string _str = Darabonba.Utils.StreamUtils.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", _str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "json")
                    {
                        object obj = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = (Dictionary<string, object>)(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "array")
                    {
                        object arr = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
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
                    _retriesAttempted++;
                    _lastException = e;
                    _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
                    {
                        RetriesAttempted = _retriesAttempted,
                        Request = _lastRequest,
                        Response = _lastResponse,
                        Exception = _lastException
                    };
                }
            }

            throw Darabonba.Core.ThrowException(_retryPolicyContext);
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
        public async Task<Dictionary<string, object>> DoROARequestAsync(string action, string version, string protocol, string method, string authType, string pathname, string bodyType, OpenApiRequest request, Darabonba.Models.RuntimeOptions runtime)
        {
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"key", (string)Darabonba.Core.GetDefaultValue(runtime.Key, _key)},
                {"cert", (string)Darabonba.Core.GetDefaultValue(runtime.Cert, _cert)},
                {"ca", (string)Darabonba.Core.GetDefaultValue(runtime.Ca, _ca)},
                {"readTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ReadTimeout, _readTimeout))},
                {"connectTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ConnectTimeout, _connectTimeout))},
                {"httpProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", (string)Darabonba.Core.GetDefaultValue(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5NetWork, _socks5NetWork)},
                {"maxIdleConns", (int?)(Darabonba.Core.GetDefaultValue(runtime.MaxIdleConns, _maxIdleConns))},
                {"retryOptions", _retryOptions},
                {"ignoreSSL", runtime.IgnoreSSL},
                {"tlsMinVersion", _tlsMinVersion},
            };

            Darabonba.RetryPolicy.RetryPolicyContext _retryPolicyContext = null;
            Darabonba.Request _lastRequest = null;
            Darabonba.Response _lastResponse = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retriesAttempted = 0;
            _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
            {
                RetriesAttempted = _retriesAttempted
            };
            while (Darabonba.Core.ShouldRetry((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext))
            {
                if (_retriesAttempted > 0)
                {
                    int backoffTime = Darabonba.Core.GetBackoffDelay((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext);
                    if (backoffTime > 0)
                    {
                        await Task.Delay(backoffTime);
                    }
                }
                try
                {
                    Darabonba.Request request_ = new Darabonba.Request();
                    request_.Protocol = (string)Darabonba.Core.GetDefaultValue(_protocol, protocol);
                    request_.Method = method;
                    request_.Pathname = pathname;
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!_globalParameters.IsNull())
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!globalParams.Queries.IsNull())
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!globalParams.Headers.IsNull())
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!runtime.ExtendsParameters.IsNull())
                    {
                        Darabonba.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!extendsParameters.Headers.IsNull())
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!extendsParameters.Queries.IsNull())
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    request_.Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"date", Utils.GetDateUTCString()},
                            {"host", _endpoint},
                            {"accept", "application/json"},
                            {"x-acs-signature-nonce", Utils.GetNonce()},
                            {"x-acs-signature-method", "HMAC-SHA1"},
                            {"x-acs-signature-version", "1.0"},
                            {"x-acs-version", version},
                            {"x-acs-action", action},
                            {"user-agent", Utils.GetUserAgent(_userAgent)},
                        },
                        globalHeaders,
                        extendsHeaders,
                        request.Headers
                    );
                    if (!request.Body.IsNull())
                    {
                        request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(Darabonba.Utils.JSONUtils.SerializeObject(request.Body));
                        request_.Headers["content-type"] = "application/json; charset=utf-8";
                    }
                    request_.Query = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        globalQueries,
                        extendsQueries
                    );
                    if (!request.Query.IsNull())
                    {
                        request_.Query = Darabonba.Utils.ConverterUtils.Merge<string>
                        (
                            request_.Query,
                            request.Query
                        );
                    }
                    if (authType != "Anonymous")
                    {
                        if (_credential.IsNull())
                        {
                            throw new ClientException
                            {
                                Code = "InvalidCredentials",
                                Message = "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.",
                            };
                        }
                        CredentialModel credentialModel = await this._credential.GetCredentialAsync();
                        if (!credentialModel.ProviderName.IsNull())
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string credentialType = credentialModel.Type;
                        if (credentialType == "bearer")
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Headers["x-acs-bearer-token"] = bearerToken;
                            request_.Headers["x-acs-signature-type"] = "BEARERTOKEN";
                        }
                        else if (credentialType == "id_token")
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!securityToken.IsNull() && securityToken != "")
                            {
                                request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                                request_.Headers["x-acs-security-token"] = securityToken;
                            }
                            string stringToSign = Utils.GetStringToSign(request_);
                            request_.Headers["authorization"] = "acs " + accessKeyId + ":" + Utils.GetROASignature(stringToSign, accessKeySecret);
                        }
                    }
                    _lastRequest = request_;
                    Darabonba.Response response_ = await Darabonba.Core.DoActionAsync(request_, runtime_);
                    _lastResponse = response_;

                    if (response_.StatusCode == 204)
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                    if ((response_.StatusCode >= 400) && (response_.StatusCode < 600))
                    {
                        object _res = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = (Dictionary<string, object>)(_res);
                        string requestId = (string)Darabonba.Core.GetDefaultValue(err.Get("RequestId"), err.Get("requestId"));
                        requestId = (string)Darabonba.Core.GetDefaultValue(requestId, err.Get("requestid"));
                        string code = (string)Darabonba.Core.GetDefaultValue(err.Get("Code"), err.Get("code"));
                        if (("" + code == "Throttling") || ("" + code == "Throttling.User") || ("" + code == "Throttling.Api"))
                        {
                            throw new ThrottlingException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                RetryAfter = Utils.GetThrottlingTimeLeft(response_.Headers),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                        else if ((response_.StatusCode >= 400) && (response_.StatusCode < 500))
                        {
                            throw new ClientException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                AccessDeniedDetail = GetAccessDeniedDetail(err),
                                RequestId = "" + requestId,
                            };
                        }
                        else
                        {
                            throw new ServerException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                    }
                    if (bodyType == "binary")
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                        return resp;
                    }
                    else if (bodyType == "byte")
                    {
                        byte[] byt = Darabonba.Utils.StreamUtils.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "string")
                    {
                        string _str = Darabonba.Utils.StreamUtils.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", _str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "json")
                    {
                        object obj = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = (Dictionary<string, object>)(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "array")
                    {
                        object arr = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
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
                    _retriesAttempted++;
                    _lastException = e;
                    _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
                    {
                        RetriesAttempted = _retriesAttempted,
                        Request = _lastRequest,
                        Response = _lastResponse,
                        Exception = _lastException
                    };
                }
            }

            throw Darabonba.Core.ThrowException(_retryPolicyContext);
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
        public Dictionary<string, object> DoROARequestWithForm(string action, string version, string protocol, string method, string authType, string pathname, string bodyType, OpenApiRequest request, Darabonba.Models.RuntimeOptions runtime)
        {
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"key", (string)Darabonba.Core.GetDefaultValue(runtime.Key, _key)},
                {"cert", (string)Darabonba.Core.GetDefaultValue(runtime.Cert, _cert)},
                {"ca", (string)Darabonba.Core.GetDefaultValue(runtime.Ca, _ca)},
                {"readTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ReadTimeout, _readTimeout))},
                {"connectTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ConnectTimeout, _connectTimeout))},
                {"httpProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", (string)Darabonba.Core.GetDefaultValue(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5NetWork, _socks5NetWork)},
                {"maxIdleConns", (int?)(Darabonba.Core.GetDefaultValue(runtime.MaxIdleConns, _maxIdleConns))},
                {"retryOptions", _retryOptions},
                {"ignoreSSL", runtime.IgnoreSSL},
                {"tlsMinVersion", _tlsMinVersion},
            };

            Darabonba.RetryPolicy.RetryPolicyContext _retryPolicyContext = null;
            Darabonba.Request _lastRequest = null;
            Darabonba.Response _lastResponse = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retriesAttempted = 0;
            _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
            {
                RetriesAttempted = _retriesAttempted
            };
            while (Darabonba.Core.ShouldRetry((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext))
            {
                if (_retriesAttempted > 0)
                {
                    int backoffTime = Darabonba.Core.GetBackoffDelay((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext);
                    if (backoffTime > 0)
                    {
                        Thread.Sleep(backoffTime);
                    }
                }
                try
                {
                    Darabonba.Request request_ = new Darabonba.Request();
                    request_.Protocol = (string)Darabonba.Core.GetDefaultValue(_protocol, protocol);
                    request_.Method = method;
                    request_.Pathname = pathname;
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!_globalParameters.IsNull())
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!globalParams.Queries.IsNull())
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!globalParams.Headers.IsNull())
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!runtime.ExtendsParameters.IsNull())
                    {
                        Darabonba.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!extendsParameters.Headers.IsNull())
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!extendsParameters.Queries.IsNull())
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    request_.Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"date", Utils.GetDateUTCString()},
                            {"host", _endpoint},
                            {"accept", "application/json"},
                            {"x-acs-signature-nonce", Utils.GetNonce()},
                            {"x-acs-signature-method", "HMAC-SHA1"},
                            {"x-acs-signature-version", "1.0"},
                            {"x-acs-version", version},
                            {"x-acs-action", action},
                            {"user-agent", Utils.GetUserAgent(_userAgent)},
                        },
                        globalHeaders,
                        extendsHeaders,
                        request.Headers
                    );
                    if (!request.Body.IsNull())
                    {
                        Dictionary<string, object> m = (Dictionary<string, object>)(request.Body);
                        request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(Utils.ToForm(m));
                        request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                    }
                    request_.Query = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        globalQueries,
                        extendsQueries
                    );
                    if (!request.Query.IsNull())
                    {
                        request_.Query = Darabonba.Utils.ConverterUtils.Merge<string>
                        (
                            request_.Query,
                            request.Query
                        );
                    }
                    if (authType != "Anonymous")
                    {
                        if (_credential.IsNull())
                        {
                            throw new ClientException
                            {
                                Code = "InvalidCredentials",
                                Message = "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.",
                            };
                        }
                        CredentialModel credentialModel = this._credential.GetCredential();
                        if (!credentialModel.ProviderName.IsNull())
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string credentialType = credentialModel.Type;
                        if (credentialType == "bearer")
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Headers["x-acs-bearer-token"] = bearerToken;
                            request_.Headers["x-acs-signature-type"] = "BEARERTOKEN";
                        }
                        else if (credentialType == "id_token")
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!securityToken.IsNull() && securityToken != "")
                            {
                                request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                                request_.Headers["x-acs-security-token"] = securityToken;
                            }
                            string stringToSign = Utils.GetStringToSign(request_);
                            request_.Headers["authorization"] = "acs " + accessKeyId + ":" + Utils.GetROASignature(stringToSign, accessKeySecret);
                        }
                    }
                    _lastRequest = request_;
                    Darabonba.Response response_ = Darabonba.Core.DoAction(request_, runtime_);
                    _lastResponse = response_;

                    if (response_.StatusCode == 204)
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                    if ((response_.StatusCode >= 400) && (response_.StatusCode < 600))
                    {
                        object _res = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = (Dictionary<string, object>)(_res);
                        string requestId = (string)Darabonba.Core.GetDefaultValue(err.Get("RequestId"), err.Get("requestId"));
                        string code = (string)Darabonba.Core.GetDefaultValue(err.Get("Code"), err.Get("code"));
                        if (("" + code == "Throttling") || ("" + code == "Throttling.User") || ("" + code == "Throttling.Api"))
                        {
                            throw new ThrottlingException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                RetryAfter = Utils.GetThrottlingTimeLeft(response_.Headers),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                        else if ((response_.StatusCode >= 400) && (response_.StatusCode < 500))
                        {
                            throw new ClientException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                AccessDeniedDetail = GetAccessDeniedDetail(err),
                                RequestId = "" + requestId,
                            };
                        }
                        else
                        {
                            throw new ServerException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                    }
                    if (bodyType == "binary")
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                        return resp;
                    }
                    else if (bodyType == "byte")
                    {
                        byte[] byt = Darabonba.Utils.StreamUtils.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "string")
                    {
                        string _str = Darabonba.Utils.StreamUtils.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", _str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "json")
                    {
                        object obj = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = (Dictionary<string, object>)(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "array")
                    {
                        object arr = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
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
                    _retriesAttempted++;
                    _lastException = e;
                    _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
                    {
                        RetriesAttempted = _retriesAttempted,
                        Request = _lastRequest,
                        Response = _lastResponse,
                        Exception = _lastException
                    };
                }
            }

            throw Darabonba.Core.ThrowException(_retryPolicyContext);
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
        public async Task<Dictionary<string, object>> DoROARequestWithFormAsync(string action, string version, string protocol, string method, string authType, string pathname, string bodyType, OpenApiRequest request, Darabonba.Models.RuntimeOptions runtime)
        {
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"key", (string)Darabonba.Core.GetDefaultValue(runtime.Key, _key)},
                {"cert", (string)Darabonba.Core.GetDefaultValue(runtime.Cert, _cert)},
                {"ca", (string)Darabonba.Core.GetDefaultValue(runtime.Ca, _ca)},
                {"readTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ReadTimeout, _readTimeout))},
                {"connectTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ConnectTimeout, _connectTimeout))},
                {"httpProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", (string)Darabonba.Core.GetDefaultValue(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5NetWork, _socks5NetWork)},
                {"maxIdleConns", (int?)(Darabonba.Core.GetDefaultValue(runtime.MaxIdleConns, _maxIdleConns))},
                {"retryOptions", _retryOptions},
                {"ignoreSSL", runtime.IgnoreSSL},
                {"tlsMinVersion", _tlsMinVersion},
            };

            Darabonba.RetryPolicy.RetryPolicyContext _retryPolicyContext = null;
            Darabonba.Request _lastRequest = null;
            Darabonba.Response _lastResponse = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retriesAttempted = 0;
            _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
            {
                RetriesAttempted = _retriesAttempted
            };
            while (Darabonba.Core.ShouldRetry((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext))
            {
                if (_retriesAttempted > 0)
                {
                    int backoffTime = Darabonba.Core.GetBackoffDelay((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext);
                    if (backoffTime > 0)
                    {
                        await Task.Delay(backoffTime);
                    }
                }
                try
                {
                    Darabonba.Request request_ = new Darabonba.Request();
                    request_.Protocol = (string)Darabonba.Core.GetDefaultValue(_protocol, protocol);
                    request_.Method = method;
                    request_.Pathname = pathname;
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!_globalParameters.IsNull())
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!globalParams.Queries.IsNull())
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!globalParams.Headers.IsNull())
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!runtime.ExtendsParameters.IsNull())
                    {
                        Darabonba.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!extendsParameters.Headers.IsNull())
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!extendsParameters.Queries.IsNull())
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    request_.Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"date", Utils.GetDateUTCString()},
                            {"host", _endpoint},
                            {"accept", "application/json"},
                            {"x-acs-signature-nonce", Utils.GetNonce()},
                            {"x-acs-signature-method", "HMAC-SHA1"},
                            {"x-acs-signature-version", "1.0"},
                            {"x-acs-version", version},
                            {"x-acs-action", action},
                            {"user-agent", Utils.GetUserAgent(_userAgent)},
                        },
                        globalHeaders,
                        extendsHeaders,
                        request.Headers
                    );
                    if (!request.Body.IsNull())
                    {
                        Dictionary<string, object> m = (Dictionary<string, object>)(request.Body);
                        request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(Utils.ToForm(m));
                        request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                    }
                    request_.Query = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        globalQueries,
                        extendsQueries
                    );
                    if (!request.Query.IsNull())
                    {
                        request_.Query = Darabonba.Utils.ConverterUtils.Merge<string>
                        (
                            request_.Query,
                            request.Query
                        );
                    }
                    if (authType != "Anonymous")
                    {
                        if (_credential.IsNull())
                        {
                            throw new ClientException
                            {
                                Code = "InvalidCredentials",
                                Message = "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.",
                            };
                        }
                        CredentialModel credentialModel = await this._credential.GetCredentialAsync();
                        if (!credentialModel.ProviderName.IsNull())
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string credentialType = credentialModel.Type;
                        if (credentialType == "bearer")
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Headers["x-acs-bearer-token"] = bearerToken;
                            request_.Headers["x-acs-signature-type"] = "BEARERTOKEN";
                        }
                        else if (credentialType == "id_token")
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!securityToken.IsNull() && securityToken != "")
                            {
                                request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                                request_.Headers["x-acs-security-token"] = securityToken;
                            }
                            string stringToSign = Utils.GetStringToSign(request_);
                            request_.Headers["authorization"] = "acs " + accessKeyId + ":" + Utils.GetROASignature(stringToSign, accessKeySecret);
                        }
                    }
                    _lastRequest = request_;
                    Darabonba.Response response_ = await Darabonba.Core.DoActionAsync(request_, runtime_);
                    _lastResponse = response_;

                    if (response_.StatusCode == 204)
                    {
                        return new Dictionary<string, object>
                        {
                            {"headers", response_.Headers},
                        };
                    }
                    if ((response_.StatusCode >= 400) && (response_.StatusCode < 600))
                    {
                        object _res = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> err = (Dictionary<string, object>)(_res);
                        string requestId = (string)Darabonba.Core.GetDefaultValue(err.Get("RequestId"), err.Get("requestId"));
                        string code = (string)Darabonba.Core.GetDefaultValue(err.Get("Code"), err.Get("code"));
                        if (("" + code == "Throttling") || ("" + code == "Throttling.User") || ("" + code == "Throttling.Api"))
                        {
                            throw new ThrottlingException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                RetryAfter = Utils.GetThrottlingTimeLeft(response_.Headers),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                        else if ((response_.StatusCode >= 400) && (response_.StatusCode < 500))
                        {
                            throw new ClientException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                AccessDeniedDetail = GetAccessDeniedDetail(err),
                                RequestId = "" + requestId,
                            };
                        }
                        else
                        {
                            throw new ServerException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                    }
                    if (bodyType == "binary")
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                        return resp;
                    }
                    else if (bodyType == "byte")
                    {
                        byte[] byt = Darabonba.Utils.StreamUtils.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "string")
                    {
                        string _str = Darabonba.Utils.StreamUtils.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", _str},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "json")
                    {
                        object obj = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = (Dictionary<string, object>)(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (bodyType == "array")
                    {
                        object arr = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
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
                    _retriesAttempted++;
                    _lastException = e;
                    _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
                    {
                        RetriesAttempted = _retriesAttempted,
                        Request = _lastRequest,
                        Response = _lastResponse,
                        Exception = _lastException
                    };
                }
            }

            throw Darabonba.Core.ThrowException(_retryPolicyContext);
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
        public Dictionary<string, object> DoRequest(Params params_, OpenApiRequest request, Darabonba.Models.RuntimeOptions runtime)
        {
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"key", (string)Darabonba.Core.GetDefaultValue(runtime.Key, _key)},
                {"cert", (string)Darabonba.Core.GetDefaultValue(runtime.Cert, _cert)},
                {"ca", (string)Darabonba.Core.GetDefaultValue(runtime.Ca, _ca)},
                {"readTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ReadTimeout, _readTimeout))},
                {"connectTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ConnectTimeout, _connectTimeout))},
                {"httpProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", (string)Darabonba.Core.GetDefaultValue(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5NetWork, _socks5NetWork)},
                {"maxIdleConns", (int?)(Darabonba.Core.GetDefaultValue(runtime.MaxIdleConns, _maxIdleConns))},
                {"retryOptions", _retryOptions},
                {"ignoreSSL", runtime.IgnoreSSL},
                {"tlsMinVersion", _tlsMinVersion},
            };

            Darabonba.RetryPolicy.RetryPolicyContext _retryPolicyContext = null;
            Darabonba.Request _lastRequest = null;
            Darabonba.Response _lastResponse = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retriesAttempted = 0;
            _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
            {
                RetriesAttempted = _retriesAttempted
            };
            while (Darabonba.Core.ShouldRetry((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext))
            {
                if (_retriesAttempted > 0)
                {
                    int backoffTime = Darabonba.Core.GetBackoffDelay((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext);
                    if (backoffTime > 0)
                    {
                        Thread.Sleep(backoffTime);
                    }
                }
                try
                {
                    Darabonba.Request request_ = new Darabonba.Request();
                    request_.Protocol = (string)Darabonba.Core.GetDefaultValue(_protocol, params_.Protocol);
                    request_.Method = params_.Method;
                    request_.Pathname = params_.Pathname;
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!_globalParameters.IsNull())
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!globalParams.Queries.IsNull())
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!globalParams.Headers.IsNull())
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!runtime.ExtendsParameters.IsNull())
                    {
                        Darabonba.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!extendsParameters.Headers.IsNull())
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!extendsParameters.Queries.IsNull())
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    request_.Query = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        globalQueries,
                        extendsQueries,
                        request.Query
                    );
                    // endpoint is setted in product client
                    request_.Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"host", _endpoint},
                            {"x-acs-version", params_.Version},
                            {"x-acs-action", params_.Action},
                            {"user-agent", Utils.GetUserAgent(_userAgent)},
                            {"x-acs-date", Utils.GetTimestamp()},
                            {"x-acs-signature-nonce", Utils.GetNonce()},
                            {"accept", "application/json"},
                        },
                        globalHeaders,
                        extendsHeaders,
                        request.Headers
                    );
                    if (params_.Style == "RPC")
                    {
                        Dictionary<string, string> headers = GetRpcHeaders();
                        if (!headers.IsNull())
                        {
                            request_.Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                            (
                                request_.Headers,
                                headers
                            );
                        }
                    }
                    string signatureAlgorithm = (string)Darabonba.Core.GetDefaultValue(_signatureAlgorithm, "ACS3-HMAC-SHA256");
                    byte[] hashedRequestPayload = Utils.Hash(Darabonba.Utils.BytesUtils.From("", "utf-8"), signatureAlgorithm);
                    if (!request.Stream.IsNull())
                    {
                        byte[] tmp = Darabonba.Utils.StreamUtils.ReadAsBytes(request.Stream);
                        hashedRequestPayload = Utils.Hash(tmp, signatureAlgorithm);
                        request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(tmp);
                        request_.Headers["content-type"] = "application/octet-stream";
                    }
                    else
                    {
                        if (!request.Body.IsNull())
                        {
                            if (params_.ReqBodyType == "byte")
                            {
                                byte[] byteObj = (byte[])(request.Body);
                                hashedRequestPayload = Utils.Hash(byteObj, signatureAlgorithm);
                                request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(byteObj);
                            }
                            else if (params_.ReqBodyType == "json")
                            {
                                string jsonObj = Darabonba.Utils.JSONUtils.SerializeObject(request.Body);
                                hashedRequestPayload = Utils.Hash(Darabonba.Utils.StringUtils.ToBytes(jsonObj, "utf8"), signatureAlgorithm);
                                request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(jsonObj);
                                request_.Headers["content-type"] = "application/json; charset=utf-8";
                            }
                            else
                            {
                                Dictionary<string, object> m = (Dictionary<string, object>)(request.Body);
                                string formObj = Utils.ToForm(m);
                                hashedRequestPayload = Utils.Hash(Darabonba.Utils.StringUtils.ToBytes(formObj, "utf8"), signatureAlgorithm);
                                request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(formObj);
                                request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                            }
                        }
                    }
                    request_.Headers["x-acs-content-sha256"] = Darabonba.Utils.BytesUtils.ToHex(hashedRequestPayload);
                    if (params_.AuthType != "Anonymous")
                    {
                        if (_credential.IsNull())
                        {
                            throw new ClientException
                            {
                                Code = "InvalidCredentials",
                                Message = "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.",
                            };
                        }
                        CredentialModel credentialModel = this._credential.GetCredential();
                        if (!credentialModel.ProviderName.IsNull())
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string authType = credentialModel.Type;
                        if (authType == "bearer")
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Headers["x-acs-bearer-token"] = bearerToken;
                            if (params_.Style == "RPC")
                            {
                                request_.Query["SignatureType"] = "BEARERTOKEN";
                            }
                            else
                            {
                                request_.Headers["x-acs-signature-type"] = "BEARERTOKEN";
                            }
                        }
                        else if (authType == "id_token")
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!securityToken.IsNull() && securityToken != "")
                            {
                                request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                                request_.Headers["x-acs-security-token"] = securityToken;
                            }
                            request_.Headers["Authorization"] = Utils.GetAuthorization(request_, signatureAlgorithm, Darabonba.Utils.BytesUtils.ToHex(hashedRequestPayload), accessKeyId, accessKeySecret);
                        }
                    }
                    _lastRequest = request_;
                    Darabonba.Response response_ = Darabonba.Core.DoAction(request_, runtime_);
                    _lastResponse = response_;

                    if ((response_.StatusCode >= 400) && (response_.StatusCode < 600))
                    {
                        Dictionary<string, object> err = new Dictionary<string, object>(){};
                        if (!response_.Headers.Get("content-type").IsNull() && response_.Headers.Get("content-type") == "text/xml;charset=utf-8")
                        {
                            string _str = Darabonba.Utils.StreamUtils.ReadAsString(response_.Body);
                            Dictionary<string, object> respMap = Darabonba.Utils.XmlUtils.ParseXml(_str, null);
                            err = (Dictionary<string, object>)(respMap.Get("Error"));
                        }
                        else
                        {
                            object _res = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                            err = (Dictionary<string, object>)(_res);
                        }
                        string requestId = (string)Darabonba.Core.GetDefaultValue(err.Get("RequestId"), err.Get("requestId"));
                        string code = (string)Darabonba.Core.GetDefaultValue(err.Get("Code"), err.Get("code"));
                        if (("" + code == "Throttling") || ("" + code == "Throttling.User") || ("" + code == "Throttling.Api"))
                        {
                            throw new ThrottlingException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                RetryAfter = Utils.GetThrottlingTimeLeft(response_.Headers),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                        else if ((response_.StatusCode >= 400) && (response_.StatusCode < 500))
                        {
                            throw new ClientException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                AccessDeniedDetail = GetAccessDeniedDetail(err),
                                RequestId = "" + requestId,
                            };
                        }
                        else
                        {
                            throw new ServerException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                    }
                    if (params_.BodyType == "binary")
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                        return resp;
                    }
                    else if (params_.BodyType == "byte")
                    {
                        byte[] byt = Darabonba.Utils.StreamUtils.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (params_.BodyType == "string")
                    {
                        string respStr = Darabonba.Utils.StreamUtils.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", respStr},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (params_.BodyType == "json")
                    {
                        object obj = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = (Dictionary<string, object>)(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (params_.BodyType == "array")
                    {
                        object arr = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else
                    {
                        string anything = Darabonba.Utils.StreamUtils.ReadAsString(response_.Body);
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
                    _retriesAttempted++;
                    _lastException = e;
                    _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
                    {
                        RetriesAttempted = _retriesAttempted,
                        Request = _lastRequest,
                        Response = _lastResponse,
                        Exception = _lastException
                    };
                }
            }

            throw Darabonba.Core.ThrowException(_retryPolicyContext);
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
        public async Task<Dictionary<string, object>> DoRequestAsync(Params params_, OpenApiRequest request, Darabonba.Models.RuntimeOptions runtime)
        {
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"key", (string)Darabonba.Core.GetDefaultValue(runtime.Key, _key)},
                {"cert", (string)Darabonba.Core.GetDefaultValue(runtime.Cert, _cert)},
                {"ca", (string)Darabonba.Core.GetDefaultValue(runtime.Ca, _ca)},
                {"readTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ReadTimeout, _readTimeout))},
                {"connectTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ConnectTimeout, _connectTimeout))},
                {"httpProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", (string)Darabonba.Core.GetDefaultValue(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5NetWork, _socks5NetWork)},
                {"maxIdleConns", (int?)(Darabonba.Core.GetDefaultValue(runtime.MaxIdleConns, _maxIdleConns))},
                {"retryOptions", _retryOptions},
                {"ignoreSSL", runtime.IgnoreSSL},
                {"tlsMinVersion", _tlsMinVersion},
            };

            Darabonba.RetryPolicy.RetryPolicyContext _retryPolicyContext = null;
            Darabonba.Request _lastRequest = null;
            Darabonba.Response _lastResponse = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retriesAttempted = 0;
            _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
            {
                RetriesAttempted = _retriesAttempted
            };
            while (Darabonba.Core.ShouldRetry((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext))
            {
                if (_retriesAttempted > 0)
                {
                    int backoffTime = Darabonba.Core.GetBackoffDelay((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext);
                    if (backoffTime > 0)
                    {
                        await Task.Delay(backoffTime);
                    }
                }
                try
                {
                    Darabonba.Request request_ = new Darabonba.Request();
                    request_.Protocol = (string)Darabonba.Core.GetDefaultValue(_protocol, params_.Protocol);
                    request_.Method = params_.Method;
                    request_.Pathname = params_.Pathname;
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!_globalParameters.IsNull())
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!globalParams.Queries.IsNull())
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!globalParams.Headers.IsNull())
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!runtime.ExtendsParameters.IsNull())
                    {
                        Darabonba.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!extendsParameters.Headers.IsNull())
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!extendsParameters.Queries.IsNull())
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    request_.Query = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        globalQueries,
                        extendsQueries,
                        request.Query
                    );
                    // endpoint is setted in product client
                    request_.Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                    (
                        new Dictionary<string, string>()
                        {
                            {"host", _endpoint},
                            {"x-acs-version", params_.Version},
                            {"x-acs-action", params_.Action},
                            {"user-agent", Utils.GetUserAgent(_userAgent)},
                            {"x-acs-date", Utils.GetTimestamp()},
                            {"x-acs-signature-nonce", Utils.GetNonce()},
                            {"accept", "application/json"},
                        },
                        globalHeaders,
                        extendsHeaders,
                        request.Headers
                    );
                    if (params_.Style == "RPC")
                    {
                        Dictionary<string, string> headers = GetRpcHeaders();
                        if (!headers.IsNull())
                        {
                            request_.Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                            (
                                request_.Headers,
                                headers
                            );
                        }
                    }
                    string signatureAlgorithm = (string)Darabonba.Core.GetDefaultValue(_signatureAlgorithm, "ACS3-HMAC-SHA256");
                    byte[] hashedRequestPayload = Utils.Hash(Darabonba.Utils.BytesUtils.From("", "utf-8"), signatureAlgorithm);
                    if (!request.Stream.IsNull())
                    {
                        byte[] tmp = Darabonba.Utils.StreamUtils.ReadAsBytes(request.Stream);
                        hashedRequestPayload = Utils.Hash(tmp, signatureAlgorithm);
                        request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(tmp);
                        request_.Headers["content-type"] = "application/octet-stream";
                    }
                    else
                    {
                        if (!request.Body.IsNull())
                        {
                            if (params_.ReqBodyType == "byte")
                            {
                                byte[] byteObj = (byte[])(request.Body);
                                hashedRequestPayload = Utils.Hash(byteObj, signatureAlgorithm);
                                request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(byteObj);
                            }
                            else if (params_.ReqBodyType == "json")
                            {
                                string jsonObj = Darabonba.Utils.JSONUtils.SerializeObject(request.Body);
                                hashedRequestPayload = Utils.Hash(Darabonba.Utils.StringUtils.ToBytes(jsonObj, "utf8"), signatureAlgorithm);
                                request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(jsonObj);
                                request_.Headers["content-type"] = "application/json; charset=utf-8";
                            }
                            else
                            {
                                Dictionary<string, object> m = (Dictionary<string, object>)(request.Body);
                                string formObj = Utils.ToForm(m);
                                hashedRequestPayload = Utils.Hash(Darabonba.Utils.StringUtils.ToBytes(formObj, "utf8"), signatureAlgorithm);
                                request_.Body = Darabonba.Utils.StreamUtils.BytesReadable(formObj);
                                request_.Headers["content-type"] = "application/x-www-form-urlencoded";
                            }
                        }
                    }
                    request_.Headers["x-acs-content-sha256"] = Darabonba.Utils.BytesUtils.ToHex(hashedRequestPayload);
                    if (params_.AuthType != "Anonymous")
                    {
                        if (_credential.IsNull())
                        {
                            throw new ClientException
                            {
                                Code = "InvalidCredentials",
                                Message = "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.",
                            };
                        }
                        CredentialModel credentialModel = await this._credential.GetCredentialAsync();
                        if (!credentialModel.ProviderName.IsNull())
                        {
                            request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName;
                        }
                        string authType = credentialModel.Type;
                        if (authType == "bearer")
                        {
                            string bearerToken = credentialModel.BearerToken;
                            request_.Headers["x-acs-bearer-token"] = bearerToken;
                            if (params_.Style == "RPC")
                            {
                                request_.Query["SignatureType"] = "BEARERTOKEN";
                            }
                            else
                            {
                                request_.Headers["x-acs-signature-type"] = "BEARERTOKEN";
                            }
                        }
                        else if (authType == "id_token")
                        {
                            string idToken = credentialModel.SecurityToken;
                            request_.Headers["x-acs-zero-trust-idtoken"] = idToken;
                        }
                        else
                        {
                            string accessKeyId = credentialModel.AccessKeyId;
                            string accessKeySecret = credentialModel.AccessKeySecret;
                            string securityToken = credentialModel.SecurityToken;
                            if (!securityToken.IsNull() && securityToken != "")
                            {
                                request_.Headers["x-acs-accesskey-id"] = accessKeyId;
                                request_.Headers["x-acs-security-token"] = securityToken;
                            }
                            request_.Headers["Authorization"] = Utils.GetAuthorization(request_, signatureAlgorithm, Darabonba.Utils.BytesUtils.ToHex(hashedRequestPayload), accessKeyId, accessKeySecret);
                        }
                    }
                    _lastRequest = request_;
                    Darabonba.Response response_ = await Darabonba.Core.DoActionAsync(request_, runtime_);
                    _lastResponse = response_;

                    if ((response_.StatusCode >= 400) && (response_.StatusCode < 600))
                    {
                        Dictionary<string, object> err = new Dictionary<string, object>(){};
                        if (!response_.Headers.Get("content-type").IsNull() && response_.Headers.Get("content-type") == "text/xml;charset=utf-8")
                        {
                            string _str = Darabonba.Utils.StreamUtils.ReadAsString(response_.Body);
                            Dictionary<string, object> respMap = Darabonba.Utils.XmlUtils.ParseXml(_str, null);
                            err = (Dictionary<string, object>)(respMap.Get("Error"));
                        }
                        else
                        {
                            object _res = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                            err = (Dictionary<string, object>)(_res);
                        }
                        string requestId = (string)Darabonba.Core.GetDefaultValue(err.Get("RequestId"), err.Get("requestId"));
                        string code = (string)Darabonba.Core.GetDefaultValue(err.Get("Code"), err.Get("code"));
                        if (("" + code == "Throttling") || ("" + code == "Throttling.User") || ("" + code == "Throttling.Api"))
                        {
                            throw new ThrottlingException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                RetryAfter = Utils.GetThrottlingTimeLeft(response_.Headers),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                        else if ((response_.StatusCode >= 400) && (response_.StatusCode < 500))
                        {
                            throw new ClientException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                AccessDeniedDetail = GetAccessDeniedDetail(err),
                                RequestId = "" + requestId,
                            };
                        }
                        else
                        {
                            throw new ServerException
                            {
                                StatusCode = response_.StatusCode,
                                Code = "" + code,
                                Message = "code: " + response_.StatusCode + ", " + Darabonba.Core.GetDefaultValue(err.Get("Message"), err.Get("message")) + " request id: " + requestId,
                                Description = "" + Darabonba.Core.GetDefaultValue(err.Get("Description"), err.Get("description")),
                                Data = err,
                                RequestId = "" + requestId,
                            };
                        }
                    }
                    if (params_.BodyType == "binary")
                    {
                        Dictionary<string, object> resp = new Dictionary<string, object>
                        {
                            {"body", response_.Body},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                        return resp;
                    }
                    else if (params_.BodyType == "byte")
                    {
                        byte[] byt = Darabonba.Utils.StreamUtils.ReadAsBytes(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", byt},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (params_.BodyType == "string")
                    {
                        string respStr = Darabonba.Utils.StreamUtils.ReadAsString(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", respStr},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (params_.BodyType == "json")
                    {
                        object obj = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        Dictionary<string, object> res = (Dictionary<string, object>)(obj);
                        return new Dictionary<string, object>
                        {
                            {"body", res},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else if (params_.BodyType == "array")
                    {
                        object arr = Darabonba.Utils.StreamUtils.ReadAsJSON(response_.Body);
                        return new Dictionary<string, object>
                        {
                            {"body", arr},
                            {"headers", response_.Headers},
                            {"statusCode", response_.StatusCode},
                        };
                    }
                    else
                    {
                        string anything = Darabonba.Utils.StreamUtils.ReadAsString(response_.Body);
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
                    _retriesAttempted++;
                    _lastException = e;
                    _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
                    {
                        RetriesAttempted = _retriesAttempted,
                        Request = _lastRequest,
                        Response = _lastResponse,
                        Exception = _lastException
                    };
                }
            }

            throw Darabonba.Core.ThrowException(_retryPolicyContext);
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
        public Dictionary<string, object> Execute(Params params_, OpenApiRequest request, Darabonba.Models.RuntimeOptions runtime)
        {
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"key", (string)Darabonba.Core.GetDefaultValue(runtime.Key, _key)},
                {"cert", (string)Darabonba.Core.GetDefaultValue(runtime.Cert, _cert)},
                {"ca", (string)Darabonba.Core.GetDefaultValue(runtime.Ca, _ca)},
                {"readTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ReadTimeout, _readTimeout))},
                {"connectTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ConnectTimeout, _connectTimeout))},
                {"httpProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", (string)Darabonba.Core.GetDefaultValue(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5NetWork, _socks5NetWork)},
                {"maxIdleConns", (int?)(Darabonba.Core.GetDefaultValue(runtime.MaxIdleConns, _maxIdleConns))},
                {"retryOptions", _retryOptions},
                {"ignoreSSL", runtime.IgnoreSSL},
                {"tlsMinVersion", _tlsMinVersion},
                {"disableHttp2", (bool?)Darabonba.Core.GetDefaultValue(_disableHttp2, false)},
            };

            Darabonba.RetryPolicy.RetryPolicyContext _retryPolicyContext = null;
            Darabonba.Request _lastRequest = null;
            Darabonba.Response _lastResponse = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retriesAttempted = 0;
            _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
            {
                RetriesAttempted = _retriesAttempted
            };
            while (Darabonba.Core.ShouldRetry((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext))
            {
                if (_retriesAttempted > 0)
                {
                    int backoffTime = Darabonba.Core.GetBackoffDelay((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext);
                    if (backoffTime > 0)
                    {
                        Thread.Sleep(backoffTime);
                    }
                }
                try
                {
                    Darabonba.Request request_ = new Darabonba.Request();
                    // spi = new Gateway();//Gateway implements SPI，这一步在产品 SDK 中实例化
                    Dictionary<string, string> headers = GetRpcHeaders();
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!_globalParameters.IsNull())
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!globalParams.Queries.IsNull())
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!globalParams.Headers.IsNull())
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!runtime.ExtendsParameters.IsNull())
                    {
                        Darabonba.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!extendsParameters.Headers.IsNull())
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!extendsParameters.Queries.IsNull())
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    InterceptorContext.InterceptorContextRequest requestContext = new InterceptorContext.InterceptorContextRequest
                    {
                        Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                        (
                            globalHeaders,
                            extendsHeaders,
                            request.Headers,
                            headers
                        ),
                        Query = Darabonba.Utils.ConverterUtils.Merge<string>
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
                        Protocol = (string)Darabonba.Core.GetDefaultValue(_protocol, params_.Protocol),
                        Method = (string)Darabonba.Core.GetDefaultValue(_method, params_.Method),
                        AuthType = params_.AuthType,
                        BodyType = params_.BodyType,
                        ReqBodyType = params_.ReqBodyType,
                        Style = params_.Style,
                        Credential = _credential,
                        SignatureVersion = _signatureVersion,
                        SignatureAlgorithm = _signatureAlgorithm,
                        UserAgent = Utils.GetUserAgent(_userAgent),
                    };
                    InterceptorContext.InterceptorContextConfiguration configurationContext = new InterceptorContext.InterceptorContextConfiguration
                    {
                        RegionId = _regionId,
                        Endpoint = (string)Darabonba.Core.GetDefaultValue(request.EndpointOverride, _endpoint),
                        EndpointRule = _endpointRule,
                        EndpointMap = _endpointMap,
                        EndpointType = _endpointType,
                        Network = _network,
                        Suffix = _suffix,
                    };
                    InterceptorContext interceptorContext = new InterceptorContext
                    {
                        Request = requestContext,
                        Configuration = configurationContext,
                    };
                    AttributeMap attributeMap = new AttributeMap();
                    if (!_attributeMap.IsNull())
                    {
                        attributeMap = _attributeMap;
                    }
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
                    Darabonba.Response response_ = Darabonba.Core.DoAction(request_, runtime_);
                    _lastResponse = response_;

                    InterceptorContext.InterceptorContextResponse responseContext = new InterceptorContext.InterceptorContextResponse
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
                    _retriesAttempted++;
                    _lastException = e;
                    _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
                    {
                        RetriesAttempted = _retriesAttempted,
                        Request = _lastRequest,
                        Response = _lastResponse,
                        Exception = _lastException
                    };
                }
            }

            throw Darabonba.Core.ThrowException(_retryPolicyContext);
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
        public async Task<Dictionary<string, object>> ExecuteAsync(Params params_, OpenApiRequest request, Darabonba.Models.RuntimeOptions runtime)
        {
            Dictionary<string, object> runtime_ = new Dictionary<string, object>
            {
                {"key", (string)Darabonba.Core.GetDefaultValue(runtime.Key, _key)},
                {"cert", (string)Darabonba.Core.GetDefaultValue(runtime.Cert, _cert)},
                {"ca", (string)Darabonba.Core.GetDefaultValue(runtime.Ca, _ca)},
                {"readTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ReadTimeout, _readTimeout))},
                {"connectTimeout", (int?)(Darabonba.Core.GetDefaultValue(runtime.ConnectTimeout, _connectTimeout))},
                {"httpProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpProxy, _httpProxy)},
                {"httpsProxy", (string)Darabonba.Core.GetDefaultValue(runtime.HttpsProxy, _httpsProxy)},
                {"noProxy", (string)Darabonba.Core.GetDefaultValue(runtime.NoProxy, _noProxy)},
                {"socks5Proxy", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5Proxy, _socks5Proxy)},
                {"socks5NetWork", (string)Darabonba.Core.GetDefaultValue(runtime.Socks5NetWork, _socks5NetWork)},
                {"maxIdleConns", (int?)(Darabonba.Core.GetDefaultValue(runtime.MaxIdleConns, _maxIdleConns))},
                {"retryOptions", _retryOptions},
                {"ignoreSSL", runtime.IgnoreSSL},
                {"tlsMinVersion", _tlsMinVersion},
                {"disableHttp2", (bool?)Darabonba.Core.GetDefaultValue(_disableHttp2, false)},
            };

            Darabonba.RetryPolicy.RetryPolicyContext _retryPolicyContext = null;
            Darabonba.Request _lastRequest = null;
            Darabonba.Response _lastResponse = null;
            Exception _lastException = null;
            long _now = System.DateTime.Now.Millisecond;
            int _retriesAttempted = 0;
            _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
            {
                RetriesAttempted = _retriesAttempted
            };
            while (Darabonba.Core.ShouldRetry((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext))
            {
                if (_retriesAttempted > 0)
                {
                    int backoffTime = Darabonba.Core.GetBackoffDelay((Darabonba.RetryPolicy.RetryOptions)runtime_["retryOptions"], _retryPolicyContext);
                    if (backoffTime > 0)
                    {
                        await Task.Delay(backoffTime);
                    }
                }
                try
                {
                    Darabonba.Request request_ = new Darabonba.Request();
                    // spi = new Gateway();//Gateway implements SPI，这一步在产品 SDK 中实例化
                    Dictionary<string, string> headers = GetRpcHeaders();
                    Dictionary<string, string> globalQueries = new Dictionary<string, string>(){};
                    Dictionary<string, string> globalHeaders = new Dictionary<string, string>(){};
                    if (!_globalParameters.IsNull())
                    {
                        GlobalParameters globalParams = _globalParameters;
                        if (!globalParams.Queries.IsNull())
                        {
                            globalQueries = globalParams.Queries;
                        }
                        if (!globalParams.Headers.IsNull())
                        {
                            globalHeaders = globalParams.Headers;
                        }
                    }
                    Dictionary<string, string> extendsHeaders = new Dictionary<string, string>(){};
                    Dictionary<string, string> extendsQueries = new Dictionary<string, string>(){};
                    if (!runtime.ExtendsParameters.IsNull())
                    {
                        Darabonba.Models.ExtendsParameters extendsParameters = runtime.ExtendsParameters;
                        if (!extendsParameters.Headers.IsNull())
                        {
                            extendsHeaders = extendsParameters.Headers;
                        }
                        if (!extendsParameters.Queries.IsNull())
                        {
                            extendsQueries = extendsParameters.Queries;
                        }
                    }
                    InterceptorContext.InterceptorContextRequest requestContext = new InterceptorContext.InterceptorContextRequest
                    {
                        Headers = Darabonba.Utils.ConverterUtils.Merge<string>
                        (
                            globalHeaders,
                            extendsHeaders,
                            request.Headers,
                            headers
                        ),
                        Query = Darabonba.Utils.ConverterUtils.Merge<string>
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
                        Protocol = (string)Darabonba.Core.GetDefaultValue(_protocol, params_.Protocol),
                        Method = (string)Darabonba.Core.GetDefaultValue(_method, params_.Method),
                        AuthType = params_.AuthType,
                        BodyType = params_.BodyType,
                        ReqBodyType = params_.ReqBodyType,
                        Style = params_.Style,
                        Credential = _credential,
                        SignatureVersion = _signatureVersion,
                        SignatureAlgorithm = _signatureAlgorithm,
                        UserAgent = Utils.GetUserAgent(_userAgent),
                    };
                    InterceptorContext.InterceptorContextConfiguration configurationContext = new InterceptorContext.InterceptorContextConfiguration
                    {
                        RegionId = _regionId,
                        Endpoint = (string)Darabonba.Core.GetDefaultValue(request.EndpointOverride, _endpoint),
                        EndpointRule = _endpointRule,
                        EndpointMap = _endpointMap,
                        EndpointType = _endpointType,
                        Network = _network,
                        Suffix = _suffix,
                    };
                    InterceptorContext interceptorContext = new InterceptorContext
                    {
                        Request = requestContext,
                        Configuration = configurationContext,
                    };
                    AttributeMap attributeMap = new AttributeMap();
                    if (!_attributeMap.IsNull())
                    {
                        attributeMap = _attributeMap;
                    }
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
                    Darabonba.Response response_ = await Darabonba.Core.DoActionAsync(request_, runtime_);
                    _lastResponse = response_;

                    InterceptorContext.InterceptorContextResponse responseContext = new InterceptorContext.InterceptorContextResponse
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
                    _retriesAttempted++;
                    _lastException = e;
                    _retryPolicyContext = new Darabonba.RetryPolicy.RetryPolicyContext
                    {
                        RetriesAttempted = _retriesAttempted,
                        Request = _lastRequest,
                        Response = _lastResponse,
                        Exception = _lastException
                    };
                }
            }

            throw Darabonba.Core.ThrowException(_retryPolicyContext);
        }

        public Dictionary<string, object> CallApi(Params params_, OpenApiRequest request, Darabonba.Models.RuntimeOptions runtime)
        {
            if (params_.IsNull())
            {
                throw new ClientException
                {
                    Code = "ParameterMissing",
                    Message = "'params' can not be unset",
                };
            }
            if (_signatureVersion.IsNull() || _signatureVersion != "v4")
            {
                if (_signatureAlgorithm.IsNull() || _signatureAlgorithm != "v2")
                {
                    return DoRequest(params_, request, runtime);
                }
                else if ((params_.Style == "ROA") && (params_.ReqBodyType == "json"))
                {
                    return DoROARequest(params_.Action, params_.Version, params_.Protocol, params_.Method, params_.AuthType, params_.Pathname, params_.BodyType, request, runtime);
                }
                else if (params_.Style == "ROA")
                {
                    return DoROARequestWithForm(params_.Action, params_.Version, params_.Protocol, params_.Method, params_.AuthType, params_.Pathname, params_.BodyType, request, runtime);
                }
                else
                {
                    return DoRPCRequest(params_.Action, params_.Version, params_.Protocol, params_.Method, params_.AuthType, params_.BodyType, request, runtime);
                }
            }
            else
            {
                return Execute(params_, request, runtime);
            }
        }

        public async Task<Dictionary<string, object>> CallApiAsync(Params params_, OpenApiRequest request, Darabonba.Models.RuntimeOptions runtime)
        {
            if (params_.IsNull())
            {
                throw new ClientException
                {
                    Code = "ParameterMissing",
                    Message = "'params' can not be unset",
                };
            }
            if (_signatureVersion.IsNull() || _signatureVersion != "v4")
            {
                if (_signatureAlgorithm.IsNull() || _signatureAlgorithm != "v2")
                {
                    return await DoRequestAsync(params_, request, runtime);
                }
                else if ((params_.Style == "ROA") && (params_.ReqBodyType == "json"))
                {
                    return await DoROARequestAsync(params_.Action, params_.Version, params_.Protocol, params_.Method, params_.AuthType, params_.Pathname, params_.BodyType, request, runtime);
                }
                else if (params_.Style == "ROA")
                {
                    return await DoROARequestWithFormAsync(params_.Action, params_.Version, params_.Protocol, params_.Method, params_.AuthType, params_.Pathname, params_.BodyType, request, runtime);
                }
                else
                {
                    return await DoRPCRequestAsync(params_.Action, params_.Version, params_.Protocol, params_.Method, params_.AuthType, params_.BodyType, request, runtime);
                }
            }
            else
            {
                return await ExecuteAsync(params_, request, runtime);
            }
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
            if (_credential.IsNull())
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
            if (_credential.IsNull())
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
            if (_credential.IsNull())
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
            if (_credential.IsNull())
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
            if (_credential.IsNull())
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
            if (_credential.IsNull())
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
            if (_credential.IsNull())
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
            if (_credential.IsNull())
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
            if (_credential.IsNull())
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
            if (_credential.IsNull())
            {
                return "";
            }
            string authType = this._credential.GetType();
            return authType;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>If the endpointRule and config.endpoint are empty, throw error</para>
        /// </description>
        /// 
        /// <param name="config">
        /// config contains the necessary information to create a client
        /// </param>
        public void CheckConfig(AlibabaCloud.OpenApiClient.Models.Config config)
        {
            if (_endpointRule.IsNull() && config.Endpoint.IsNull())
            {
                throw new ClientException
                {
                    Code = "ParameterMissing",
                    Message = "'config.endpoint' can not be empty",
                };
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
        public void SetGatewayClient(SPIClient spi)
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

        public Dictionary<string, object> GetAccessDeniedDetail(Dictionary<string, object> err)
        {
            Dictionary<string, object> accessDeniedDetail = null;
            if (!err.Get("AccessDeniedDetail").IsNull())
            {
                Dictionary<string, object> detail1 = (Dictionary<string, object>)(err.Get("AccessDeniedDetail"));
                accessDeniedDetail = detail1;
            }
            else if (!err.Get("accessDeniedDetail").IsNull())
            {
                Dictionary<string, object> detail2 = (Dictionary<string, object>)(err.Get("accessDeniedDetail"));
                accessDeniedDetail = detail2;
            }
            return accessDeniedDetail;
        }

    }
}

