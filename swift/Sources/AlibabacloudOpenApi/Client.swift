import Foundation
import Tea
import TeaUtils
import AlibabaCloudCredentials
import AlibabaCloudOpenApiUtil

open class Client {
    public var _endpoint: String?

    public var _regionId: String?

    public var _protocol: String?

    public var _method: String?

    public var _userAgent: String?

    public var _endpointRule: String?

    public var _endpointMap: [String: String]?

    public var _suffix: String?

    public var _readTimeout: Int?

    public var _connectTimeout: Int?

    public var _httpProxy: String?

    public var _httpsProxy: String?

    public var _socks5Proxy: String?

    public var _socks5NetWork: String?

    public var _noProxy: String?

    public var _network: String?

    public var _productId: String?

    public var _maxIdleConns: Int?

    public var _endpointType: String?

    public var _openPlatformEndpoint: String?

    public var _credential: AlibabaCloudCredentials.Client?

    public var _signatureVersion: String?

    public var _signatureAlgorithm: String?

    public var _headers: [String: String]?

    public var _globalParameters: GlobalParameters?

    public init(_ config: Config) throws {
        if (TeaUtils.Client.isUnset(config)) {
            throw Tea.ReuqestError([
                "code": "ParameterMissing",
                "message": "'config' can not be unset"
            ])
        }
        if (!TeaUtils.Client.empty(config.accessKeyId) && !TeaUtils.Client.empty(config.accessKeySecret)) {
            if (!TeaUtils.Client.empty(config.securityToken)) {
                config.type = "sts"
            }
            else {
                config.type = "access_key"
            }
            var credentialConfig: AlibabaCloudCredentials.Config = AlibabaCloudCredentials.Config([
                "accessKeyId": config.accessKeyId ?? "",
                "type": config.type ?? "",
                "accessKeySecret": config.accessKeySecret ?? ""
            ])
            credentialConfig.securityToken = config.securityToken
            self._credential = AlibabaCloudCredentials.Client(credentialConfig)
        }
        else if (!TeaUtils.Client.isUnset(config.credential)) {
            self._credential = config.credential
        }
        self._endpoint = config.endpoint
        self._endpointType = config.endpointType
        self._network = config.network
        self._suffix = config.suffix
        self._protocol = config.protocol_
        self._method = config.method
        self._regionId = config.regionId
        self._userAgent = config.userAgent
        self._readTimeout = config.readTimeout
        self._connectTimeout = config.connectTimeout
        self._httpProxy = config.httpProxy
        self._httpsProxy = config.httpsProxy
        self._noProxy = config.noProxy
        self._socks5Proxy = config.socks5Proxy
        self._socks5NetWork = config.socks5NetWork
        self._maxIdleConns = config.maxIdleConns
        self._signatureVersion = config.signatureVersion
        self._signatureAlgorithm = config.signatureAlgorithm
        self._globalParameters = config.globalParameters
    }

    @available(macOS 10.15, iOS 13, tvOS 13, watchOS 6, *)
    public func doRPCRequest(_ action: String, _ version: String, _ protocol_: String, _ method: String, _ authType: String, _ bodyType: String, _ request: OpenApiRequest, _ runtime: TeaUtils.RuntimeOptions) async throws -> [String: Any] {
        try request.validate()
        try runtime.validate()
        var _runtime: [String: Any] = [
            "timeouted": "retry",
            "readTimeout": TeaUtils.Client.defaultNumber(runtime.readTimeout, self._readTimeout),
            "connectTimeout": TeaUtils.Client.defaultNumber(runtime.connectTimeout, self._connectTimeout),
            "httpProxy": TeaUtils.Client.defaultString(runtime.httpProxy, self._httpProxy),
            "httpsProxy": TeaUtils.Client.defaultString(runtime.httpsProxy, self._httpsProxy),
            "noProxy": TeaUtils.Client.defaultString(runtime.noProxy, self._noProxy),
            "socks5Proxy": TeaUtils.Client.defaultString(runtime.socks5Proxy, self._socks5Proxy),
            "socks5NetWork": TeaUtils.Client.defaultString(runtime.socks5NetWork, self._socks5NetWork),
            "maxIdleConns": TeaUtils.Client.defaultNumber(runtime.maxIdleConns, self._maxIdleConns),
            "retry": [
                "retryable": Client.defaultAny(runtime.autoretry, false),
                "maxAttempts": TeaUtils.Client.defaultNumber(runtime.maxAttempts, 3)
            ],
            "backoff": [
                "policy": TeaUtils.Client.defaultString(runtime.backoffPolicy, "no"),
                "period": TeaUtils.Client.defaultNumber(runtime.backoffPeriod, 1)
            ],
            "ignoreSSL": Client.defaultAny(runtime.ignoreSSL, false)
        ]
        var _lastRequest: Tea.TeaRequest? = nil
        var _lastException: Tea.TeaError? = nil
        var _now: Int32 = Tea.TeaCore.timeNow()
        var _retryTimes: Int32 = 0
        while (Tea.TeaCore.allowRetry(_runtime["retry"], _retryTimes, _now)) {
            if (_retryTimes > 0) {
                var _backoffTime: Int32 = Tea.TeaCore.getBackoffTime(_runtime["backoff"], _retryTimes)
                if (_backoffTime > 0) {
                    Tea.TeaCore.sleep(_backoffTime)
                }
            }
            _retryTimes = _retryTimes + 1
            do {
                var _request: Tea.TeaRequest = Tea.TeaRequest()
                _request.protocol_ = TeaUtils.Client.defaultString(self._protocol, protocol_)
                _request.method = method as! String
                _request.pathname = "/"
                _request.query = Tea.TeaConverter.merge([
                    "Action": action as! String,
                    "Format": "json",
                    "Version": version as! String,
                    "Timestamp": AlibabaCloudOpenApiUtil.Client.getTimestamp(),
                    "SignatureNonce": TeaUtils.Client.getNonce()
                ], request.query ?? [:])
                var headers: [String: String] = try getRpcHeaders()
                if (TeaUtils.Client.isUnset(headers)) {
                    _request.headers = [
                        "host": self._endpoint ?? "",
                        "x-acs-version": version as! String,
                        "x-acs-action": action as! String,
                        "user-agent": getUserAgent()
                    ]
                }
                else {
                    _request.headers = Tea.TeaConverter.merge([
                        "host": self._endpoint ?? "",
                        "x-acs-version": version as! String,
                        "x-acs-action": action as! String,
                        "user-agent": getUserAgent()
                    ], headers)
                }
                if (!TeaUtils.Client.isUnset(request.body)) {
                    var m: [String: Any] = try TeaUtils.Client.assertAsMap(request.body)
                    var tmp: [String: Any] = TeaUtils.Client.anyifyMapValue(AlibabaCloudOpenApiUtil.Client.query(m))
                    _request.body = Tea.TeaCore.toReadable(TeaUtils.Client.toFormString(tmp))
                    _request.headers["content-type"] = "application/x-www-form-urlencoded";
                }
                if (!TeaUtils.Client.equalString(authType, "Anonymous")) {
                    var accessKeyId: String = try await getAccessKeyId()
                    var accessKeySecret: String = try await getAccessKeySecret()
                    var securityToken: String = try await getSecurityToken()
                    if (!TeaUtils.Client.empty(securityToken)) {
                        _request.query["SecurityToken"] = securityToken as! String;
                    }
                    _request.query["SignatureMethod"] = "HMAC-SHA1";
                    _request.query["SignatureVersion"] = "1.0";
                    _request.query["AccessKeyId"] = accessKeyId as! String;
                    var t: [String: Any]? = nil
                    if (!TeaUtils.Client.isUnset(request.body)) {
                        t = try TeaUtils.Client.assertAsMap(request.body)
                    }
                    var signedParam: [String: String] = Tea.TeaConverter.merge([:], _request.query, AlibabaCloudOpenApiUtil.Client.query(t))
                    _request.query["Signature"] = AlibabaCloudOpenApiUtil.Client.getRPCSignature(signedParam, _request.method, accessKeySecret);
                }
                _lastRequest = _request
                var _response: Tea.TeaResponse = try await Tea.TeaCore.doAction(_request, _runtime)
                if (TeaUtils.Client.is4xx(_response.statusCode) || TeaUtils.Client.is5xx(_response.statusCode)) {
                    var _res: Any = try await TeaUtils.Client.readAsJSON(_response.body)
                    var err: [String: Any] = try TeaUtils.Client.assertAsMap(_res)
                    var requestId: Any = Client.defaultAny(err["RequestId"], err["requestId"])
                    throw Tea.ReuqestError([
                        "code": Client.defaultAny(err["Code"], err["code"]),
                        "message": "code: \(_response.statusCode), \(Client.defaultAny(err["Message"], err["message"])) request id: \(requestId)",
                        "data": err
                    ])
                }
                if (TeaUtils.Client.equalString(bodyType, "binary")) {
                    var resp: [String: Any] = [
                        "body": _response.body,
                        "headers": _response.headers
                    ]
                    return resp as! [String: Any]
                }
                else if (TeaUtils.Client.equalString(bodyType, "byte")) {
                    var byt: [UInt8] = try await TeaUtils.Client.readAsBytes(_response.body)
                    return [
                        "body": byt as! [UInt8],
                        "headers": _response.headers
                    ]
                }
                else if (TeaUtils.Client.equalString(bodyType, "string")) {
                    var str: String = try await TeaUtils.Client.readAsString(_response.body)
                    return [
                        "body": str as! String,
                        "headers": _response.headers
                    ]
                }
                else if (TeaUtils.Client.equalString(bodyType, "json")) {
                    var obj: Any = try await TeaUtils.Client.readAsJSON(_response.body)
                    var res: [String: Any] = try TeaUtils.Client.assertAsMap(obj)
                    return [
                        "body": res as! [String: Any],
                        "headers": _response.headers
                    ]
                }
                else if (TeaUtils.Client.equalString(bodyType, "array")) {
                    var arr: Any = try await TeaUtils.Client.readAsJSON(_response.body)
                    return [
                        "body": arr as! Any,
                        "headers": _response.headers
                    ]
                }
                else {
                    return [
                        "headers": _response.headers
                    ]
                }
            }
            catch {
                if (Tea.TeaCore.isRetryable(error)) {
                    _lastException = error as! Tea.RetryableError
                    continue
                }
                throw error
            }
        }
        throw Tea.UnretryableError(_lastRequest, _lastException)
    }

    @available(macOS 10.15, iOS 13, tvOS 13, watchOS 6, *)
    public func doROARequest(_ action: String, _ version: String, _ protocol_: String, _ method: String, _ authType: String, _ pathname: String, _ bodyType: String, _ request: OpenApiRequest, _ runtime: TeaUtils.RuntimeOptions) async throws -> [String: Any] {
        try request.validate()
        try runtime.validate()
        var _runtime: [String: Any] = [
            "timeouted": "retry",
            "readTimeout": TeaUtils.Client.defaultNumber(runtime.readTimeout, self._readTimeout),
            "connectTimeout": TeaUtils.Client.defaultNumber(runtime.connectTimeout, self._connectTimeout),
            "httpProxy": TeaUtils.Client.defaultString(runtime.httpProxy, self._httpProxy),
            "httpsProxy": TeaUtils.Client.defaultString(runtime.httpsProxy, self._httpsProxy),
            "noProxy": TeaUtils.Client.defaultString(runtime.noProxy, self._noProxy),
            "socks5Proxy": TeaUtils.Client.defaultString(runtime.socks5Proxy, self._socks5Proxy),
            "socks5NetWork": TeaUtils.Client.defaultString(runtime.socks5NetWork, self._socks5NetWork),
            "maxIdleConns": TeaUtils.Client.defaultNumber(runtime.maxIdleConns, self._maxIdleConns),
            "retry": [
                "retryable": Client.defaultAny(runtime.autoretry, false),
                "maxAttempts": TeaUtils.Client.defaultNumber(runtime.maxAttempts, 3)
            ],
            "backoff": [
                "policy": TeaUtils.Client.defaultString(runtime.backoffPolicy, "no"),
                "period": TeaUtils.Client.defaultNumber(runtime.backoffPeriod, 1)
            ],
            "ignoreSSL": Client.defaultAny(runtime.ignoreSSL, false)
        ]
        var _lastRequest: Tea.TeaRequest? = nil
        var _lastException: Tea.TeaError? = nil
        var _now: Int32 = Tea.TeaCore.timeNow()
        var _retryTimes: Int32 = 0
        while (Tea.TeaCore.allowRetry(_runtime["retry"], _retryTimes, _now)) {
            if (_retryTimes > 0) {
                var _backoffTime: Int32 = Tea.TeaCore.getBackoffTime(_runtime["backoff"], _retryTimes)
                if (_backoffTime > 0) {
                    Tea.TeaCore.sleep(_backoffTime)
                }
            }
            _retryTimes = _retryTimes + 1
            do {
                var _request: Tea.TeaRequest = Tea.TeaRequest()
                _request.protocol_ = TeaUtils.Client.defaultString(self._protocol, protocol_)
                _request.method = method as! String
                _request.pathname = pathname as! String
                _request.headers = Tea.TeaConverter.merge([
                    "date": TeaUtils.Client.getDateUTCString(),
                    "host": self._endpoint ?? "",
                    "accept": "application/json",
                    "x-acs-signature-nonce": TeaUtils.Client.getNonce(),
                    "x-acs-signature-method": "HMAC-SHA1",
                    "x-acs-signature-version": "1.0",
                    "x-acs-version": version as! String,
                    "x-acs-action": action as! String,
                    "user-agent": TeaUtils.Client.getUserAgent(self._userAgent)
                ], request.headers ?? [:])
                if (!TeaUtils.Client.isUnset(request.body)) {
                    _request.body = Tea.TeaCore.toReadable(TeaUtils.Client.toJSONString(request.body))
                    _request.headers["content-type"] = "application/json; charset=utf-8";
                }
                if (!TeaUtils.Client.isUnset(request.query)) {
                    _request.query = request.query ?? [:]
                }
                if (!TeaUtils.Client.equalString(authType, "Anonymous")) {
                    var accessKeyId: String = try await getAccessKeyId()
                    var accessKeySecret: String = try await getAccessKeySecret()
                    var securityToken: String = try await getSecurityToken()
                    if (!TeaUtils.Client.empty(securityToken)) {
                        _request.headers["x-acs-accesskey-id"] = accessKeyId as! String;
                        _request.headers["x-acs-security-token"] = securityToken as! String;
                    }
                    var stringToSign: String = AlibabaCloudOpenApiUtil.Client.getStringToSign(_request)
                    _request.headers["authorization"] = "acs " + accessKeyId + ":" + AlibabaCloudOpenApiUtil.Client.getROASignature(stringToSign, accessKeySecret);
                }
                _lastRequest = _request
                var _response: Tea.TeaResponse = try await Tea.TeaCore.doAction(_request, _runtime)
                if (TeaUtils.Client.equalNumber(_response.statusCode, 204)) {
                    return [
                        "headers": _response.headers
                    ]
                }
                if (TeaUtils.Client.is4xx(_response.statusCode) || TeaUtils.Client.is5xx(_response.statusCode)) {
                    var _res: Any = try await TeaUtils.Client.readAsJSON(_response.body)
                    var err: [String: Any] = try TeaUtils.Client.assertAsMap(_res)
                    var requestId: Any = Client.defaultAny(err["RequestId"], err["requestId"])
                    requestId = Client.defaultAny(requestId, err["requestid"])
                    throw Tea.ReuqestError([
                        "code": Client.defaultAny(err["Code"], err["code"]),
                        "message": "code: \(_response.statusCode), \(Client.defaultAny(err["Message"], err["message"])) request id: \(requestId)",
                        "data": err
                    ])
                }
                if (TeaUtils.Client.equalString(bodyType, "binary")) {
                    var resp: [String: Any] = [
                        "body": _response.body,
                        "headers": _response.headers
                    ]
                    return resp as! [String: Any]
                }
                else if (TeaUtils.Client.equalString(bodyType, "byte")) {
                    var byt: [UInt8] = try await TeaUtils.Client.readAsBytes(_response.body)
                    return [
                        "body": byt as! [UInt8],
                        "headers": _response.headers
                    ]
                }
                else if (TeaUtils.Client.equalString(bodyType, "string")) {
                    var str: String = try await TeaUtils.Client.readAsString(_response.body)
                    return [
                        "body": str as! String,
                        "headers": _response.headers
                    ]
                }
                else if (TeaUtils.Client.equalString(bodyType, "json")) {
                    var obj: Any = try await TeaUtils.Client.readAsJSON(_response.body)
                    var res: [String: Any] = try TeaUtils.Client.assertAsMap(obj)
                    return [
                        "body": res as! [String: Any],
                        "headers": _response.headers
                    ]
                }
                else if (TeaUtils.Client.equalString(bodyType, "array")) {
                    var arr: Any = try await TeaUtils.Client.readAsJSON(_response.body)
                    return [
                        "body": arr as! Any,
                        "headers": _response.headers
                    ]
                }
                else {
                    return [
                        "headers": _response.headers
                    ]
                }
            }
            catch {
                if (Tea.TeaCore.isRetryable(error)) {
                    _lastException = error as! Tea.RetryableError
                    continue
                }
                throw error
            }
        }
        throw Tea.UnretryableError(_lastRequest, _lastException)
    }

    @available(macOS 10.15, iOS 13, tvOS 13, watchOS 6, *)
    public func doROARequestWithForm(_ action: String, _ version: String, _ protocol_: String, _ method: String, _ authType: String, _ pathname: String, _ bodyType: String, _ request: OpenApiRequest, _ runtime: TeaUtils.RuntimeOptions) async throws -> [String: Any] {
        try request.validate()
        try runtime.validate()
        var _runtime: [String: Any] = [
            "timeouted": "retry",
            "readTimeout": TeaUtils.Client.defaultNumber(runtime.readTimeout, self._readTimeout),
            "connectTimeout": TeaUtils.Client.defaultNumber(runtime.connectTimeout, self._connectTimeout),
            "httpProxy": TeaUtils.Client.defaultString(runtime.httpProxy, self._httpProxy),
            "httpsProxy": TeaUtils.Client.defaultString(runtime.httpsProxy, self._httpsProxy),
            "noProxy": TeaUtils.Client.defaultString(runtime.noProxy, self._noProxy),
            "socks5Proxy": TeaUtils.Client.defaultString(runtime.socks5Proxy, self._socks5Proxy),
            "socks5NetWork": TeaUtils.Client.defaultString(runtime.socks5NetWork, self._socks5NetWork),
            "maxIdleConns": TeaUtils.Client.defaultNumber(runtime.maxIdleConns, self._maxIdleConns),
            "retry": [
                "retryable": Client.defaultAny(runtime.autoretry, false),
                "maxAttempts": TeaUtils.Client.defaultNumber(runtime.maxAttempts, 3)
            ],
            "backoff": [
                "policy": TeaUtils.Client.defaultString(runtime.backoffPolicy, "no"),
                "period": TeaUtils.Client.defaultNumber(runtime.backoffPeriod, 1)
            ],
            "ignoreSSL": Client.defaultAny(runtime.ignoreSSL, false)
        ]
        var _lastRequest: Tea.TeaRequest? = nil
        var _lastException: Tea.TeaError? = nil
        var _now: Int32 = Tea.TeaCore.timeNow()
        var _retryTimes: Int32 = 0
        while (Tea.TeaCore.allowRetry(_runtime["retry"], _retryTimes, _now)) {
            if (_retryTimes > 0) {
                var _backoffTime: Int32 = Tea.TeaCore.getBackoffTime(_runtime["backoff"], _retryTimes)
                if (_backoffTime > 0) {
                    Tea.TeaCore.sleep(_backoffTime)
                }
            }
            _retryTimes = _retryTimes + 1
            do {
                var _request: Tea.TeaRequest = Tea.TeaRequest()
                _request.protocol_ = TeaUtils.Client.defaultString(self._protocol, protocol_)
                _request.method = method as! String
                _request.pathname = pathname as! String
                _request.headers = Tea.TeaConverter.merge([
                    "date": TeaUtils.Client.getDateUTCString(),
                    "host": self._endpoint ?? "",
                    "accept": "application/json",
                    "x-acs-signature-nonce": TeaUtils.Client.getNonce(),
                    "x-acs-signature-method": "HMAC-SHA1",
                    "x-acs-signature-version": "1.0",
                    "x-acs-version": version as! String,
                    "x-acs-action": action as! String,
                    "user-agent": TeaUtils.Client.getUserAgent(self._userAgent)
                ], request.headers ?? [:])
                if (!TeaUtils.Client.isUnset(request.body)) {
                    var m: [String: Any] = try TeaUtils.Client.assertAsMap(request.body)
                    _request.body = Tea.TeaCore.toReadable(AlibabaCloudOpenApiUtil.Client.toForm(m))
                    _request.headers["content-type"] = "application/x-www-form-urlencoded";
                }
                if (!TeaUtils.Client.isUnset(request.query)) {
                    _request.query = request.query ?? [:]
                }
                if (!TeaUtils.Client.equalString(authType, "Anonymous")) {
                    var accessKeyId: String = try await getAccessKeyId()
                    var accessKeySecret: String = try await getAccessKeySecret()
                    var securityToken: String = try await getSecurityToken()
                    if (!TeaUtils.Client.empty(securityToken)) {
                        _request.headers["x-acs-accesskey-id"] = accessKeyId as! String;
                        _request.headers["x-acs-security-token"] = securityToken as! String;
                    }
                    var stringToSign: String = AlibabaCloudOpenApiUtil.Client.getStringToSign(_request)
                    _request.headers["authorization"] = "acs " + accessKeyId + ":" + AlibabaCloudOpenApiUtil.Client.getROASignature(stringToSign, accessKeySecret);
                }
                _lastRequest = _request
                var _response: Tea.TeaResponse = try await Tea.TeaCore.doAction(_request, _runtime)
                if (TeaUtils.Client.equalNumber(_response.statusCode, 204)) {
                    return [
                        "headers": _response.headers
                    ]
                }
                if (TeaUtils.Client.is4xx(_response.statusCode) || TeaUtils.Client.is5xx(_response.statusCode)) {
                    var _res: Any = try await TeaUtils.Client.readAsJSON(_response.body)
                    var err: [String: Any] = try TeaUtils.Client.assertAsMap(_res)
                    throw Tea.ReuqestError([
                        "code": Client.defaultAny(err["Code"], err["code"]),
                        "message": "code: \(_response.statusCode), \(Client.defaultAny(err["Message"], err["message"])) request id: \(Client.defaultAny(err["RequestId"], err["requestId"]))",
                        "data": err
                    ])
                }
                if (TeaUtils.Client.equalString(bodyType, "binary")) {
                    var resp: [String: Any] = [
                        "body": _response.body,
                        "headers": _response.headers
                    ]
                    return resp as! [String: Any]
                }
                else if (TeaUtils.Client.equalString(bodyType, "byte")) {
                    var byt: [UInt8] = try await TeaUtils.Client.readAsBytes(_response.body)
                    return [
                        "body": byt as! [UInt8],
                        "headers": _response.headers
                    ]
                }
                else if (TeaUtils.Client.equalString(bodyType, "string")) {
                    var str: String = try await TeaUtils.Client.readAsString(_response.body)
                    return [
                        "body": str as! String,
                        "headers": _response.headers
                    ]
                }
                else if (TeaUtils.Client.equalString(bodyType, "json")) {
                    var obj: Any = try await TeaUtils.Client.readAsJSON(_response.body)
                    var res: [String: Any] = try TeaUtils.Client.assertAsMap(obj)
                    return [
                        "body": res as! [String: Any],
                        "headers": _response.headers
                    ]
                }
                else if (TeaUtils.Client.equalString(bodyType, "array")) {
                    var arr: Any = try await TeaUtils.Client.readAsJSON(_response.body)
                    return [
                        "body": arr as! Any,
                        "headers": _response.headers
                    ]
                }
                else {
                    return [
                        "headers": _response.headers
                    ]
                }
            }
            catch {
                if (Tea.TeaCore.isRetryable(error)) {
                    _lastException = error as! Tea.RetryableError
                    continue
                }
                throw error
            }
        }
        throw Tea.UnretryableError(_lastRequest, _lastException)
    }

    @available(macOS 10.15, iOS 13, tvOS 13, watchOS 6, *)
    public func doRequest(_ params: Params, _ request: OpenApiRequest, _ runtime: TeaUtils.RuntimeOptions) async throws -> [String: Any] {
        try params.validate()
        try request.validate()
        try runtime.validate()
        var _runtime: [String: Any] = [
            "timeouted": "retry",
            "readTimeout": TeaUtils.Client.defaultNumber(runtime.readTimeout, self._readTimeout),
            "connectTimeout": TeaUtils.Client.defaultNumber(runtime.connectTimeout, self._connectTimeout),
            "httpProxy": TeaUtils.Client.defaultString(runtime.httpProxy, self._httpProxy),
            "httpsProxy": TeaUtils.Client.defaultString(runtime.httpsProxy, self._httpsProxy),
            "noProxy": TeaUtils.Client.defaultString(runtime.noProxy, self._noProxy),
            "socks5Proxy": TeaUtils.Client.defaultString(runtime.socks5Proxy, self._socks5Proxy),
            "socks5NetWork": TeaUtils.Client.defaultString(runtime.socks5NetWork, self._socks5NetWork),
            "maxIdleConns": TeaUtils.Client.defaultNumber(runtime.maxIdleConns, self._maxIdleConns),
            "retry": [
                "retryable": Client.defaultAny(runtime.autoretry, false),
                "maxAttempts": TeaUtils.Client.defaultNumber(runtime.maxAttempts, 3)
            ],
            "backoff": [
                "policy": TeaUtils.Client.defaultString(runtime.backoffPolicy, "no"),
                "period": TeaUtils.Client.defaultNumber(runtime.backoffPeriod, 1)
            ],
            "ignoreSSL": Client.defaultAny(runtime.ignoreSSL, false)
        ]
        var _lastRequest: Tea.TeaRequest? = nil
        var _lastException: Tea.TeaError? = nil
        var _now: Int32 = Tea.TeaCore.timeNow()
        var _retryTimes: Int32 = 0
        while (Tea.TeaCore.allowRetry(_runtime["retry"], _retryTimes, _now)) {
            if (_retryTimes > 0) {
                var _backoffTime: Int32 = Tea.TeaCore.getBackoffTime(_runtime["backoff"], _retryTimes)
                if (_backoffTime > 0) {
                    Tea.TeaCore.sleep(_backoffTime)
                }
            }
            _retryTimes = _retryTimes + 1
            do {
                var _request: Tea.TeaRequest = Tea.TeaRequest()
                _request.protocol_ = TeaUtils.Client.defaultString(self._protocol, params.protocol_)
                _request.method = params.method ?? ""
                _request.pathname = params.pathname ?? ""
                var globalQueries: [String: String] = [:]
                var globalHeaders: [String: String] = [:]
                if (!TeaUtils.Client.isUnset(self._globalParameters)) {
                    var globalParams: GlobalParameters = self._globalParameters!
                    if (!TeaUtils.Client.isUnset(globalParams.queries)) {
                        globalQueries = globalParams.queries ?? [:]
                    }
                    if (!TeaUtils.Client.isUnset(globalParams.headers)) {
                        globalHeaders = globalParams.headers ?? [:]
                    }
                }
                _request.query = Tea.TeaConverter.merge([:], globalQueries, request.query ?? [:])
                _request.headers = Tea.TeaConverter.merge([
                    "host": self._endpoint ?? "",
                    "x-acs-version": params.version ?? "",
                    "x-acs-action": params.action ?? "",
                    "user-agent": getUserAgent(),
                    "x-acs-date": AlibabaCloudOpenApiUtil.Client.getTimestamp(),
                    "x-acs-signature-nonce": TeaUtils.Client.getNonce(),
                    "accept": "application/json"
                ], globalHeaders, request.headers ?? [:])
                if (TeaUtils.Client.equalString(params.style, "RPC")) {
                    var headers: [String: String] = try getRpcHeaders()
                    if (!TeaUtils.Client.isUnset(headers)) {
                        _request.headers = Tea.TeaConverter.merge([:], _request.headers, headers)
                    }
                }
                var signatureAlgorithm: String = TeaUtils.Client.defaultString(self._signatureAlgorithm, "ACS3-HMAC-SHA256")
                var hashedRequestPayload: String = AlibabaCloudOpenApiUtil.Client.hexEncode(AlibabaCloudOpenApiUtil.Client.hash(TeaUtils.Client.toBytes(""), signatureAlgorithm))
                if (!TeaUtils.Client.isUnset(request.stream)) {
                    var tmp: [UInt8] = try await TeaUtils.Client.readAsBytes(request.stream)
                    hashedRequestPayload = AlibabaCloudOpenApiUtil.Client.hexEncode(AlibabaCloudOpenApiUtil.Client.hash(tmp, signatureAlgorithm))
                    _request.body = Tea.TeaCore.toReadable(tmp as! [UInt8])
                    _request.headers["content-type"] = "application/octet-stream";
                }
                else {
                    if (!TeaUtils.Client.isUnset(request.body)) {
                        if (TeaUtils.Client.equalString(params.reqBodyType, "json")) {
                            var jsonObj: String = TeaUtils.Client.toJSONString(request.body)
                            hashedRequestPayload = AlibabaCloudOpenApiUtil.Client.hexEncode(AlibabaCloudOpenApiUtil.Client.hash(TeaUtils.Client.toBytes(jsonObj), signatureAlgorithm))
                            _request.body = Tea.TeaCore.toReadable(jsonObj as! String)
                            _request.headers["content-type"] = "application/json; charset=utf-8";
                        }
                        else {
                            var m: [String: Any] = try TeaUtils.Client.assertAsMap(request.body)
                            var formObj: String = AlibabaCloudOpenApiUtil.Client.toForm(m)
                            hashedRequestPayload = AlibabaCloudOpenApiUtil.Client.hexEncode(AlibabaCloudOpenApiUtil.Client.hash(TeaUtils.Client.toBytes(formObj), signatureAlgorithm))
                            _request.body = Tea.TeaCore.toReadable(formObj as! String)
                            _request.headers["content-type"] = "application/x-www-form-urlencoded";
                        }
                    }
                }
                _request.headers["x-acs-content-sha256"] = hashedRequestPayload as! String;
                if (!TeaUtils.Client.equalString(params.authType, "Anonymous")) {
                    var authType: String = try await getType()
                    if (TeaUtils.Client.equalString(authType, "bearer")) {
                        var bearerToken: String = try await getBearerToken()
                        _request.headers["x-acs-bearer-token"] = bearerToken as! String;
                    }
                    else {
                        var accessKeyId: String = try await getAccessKeyId()
                        var accessKeySecret: String = try await getAccessKeySecret()
                        var securityToken: String = try await getSecurityToken()
                        if (!TeaUtils.Client.empty(securityToken)) {
                            _request.headers["x-acs-accesskey-id"] = accessKeyId as! String;
                            _request.headers["x-acs-security-token"] = securityToken as! String;
                        }
                        _request.headers["Authorization"] = AlibabaCloudOpenApiUtil.Client.getAuthorization(_request, signatureAlgorithm, hashedRequestPayload, accessKeyId, accessKeySecret);
                    }
                }
                _lastRequest = _request
                var _response: Tea.TeaResponse = try await Tea.TeaCore.doAction(_request, _runtime)
                if (TeaUtils.Client.is4xx(_response.statusCode) || TeaUtils.Client.is5xx(_response.statusCode)) {
                    var _res: Any = try await TeaUtils.Client.readAsJSON(_response.body)
                    var err: [String: Any] = try TeaUtils.Client.assertAsMap(_res)
                    err["statusCode"] = _response.statusCode
                    throw Tea.ReuqestError([
                        "code": Client.defaultAny(err["Code"], err["code"]),
                        "message": "code: \(_response.statusCode), \(Client.defaultAny(err["Message"], err["message"])) request id: \(Client.defaultAny(err["RequestId"], err["requestId"]))",
                        "data": err
                    ])
                }
                if (TeaUtils.Client.equalString(params.bodyType, "binary")) {
                    var resp: [String: Any] = [
                        "body": _response.body,
                        "headers": _response.headers,
                        "statusCode": _response.statusCode
                    ]
                    return resp as! [String: Any]
                }
                else if (TeaUtils.Client.equalString(params.bodyType, "byte")) {
                    var byt: [UInt8] = try await TeaUtils.Client.readAsBytes(_response.body)
                    return [
                        "body": byt as! [UInt8],
                        "headers": _response.headers,
                        "statusCode": _response.statusCode
                    ]
                }
                else if (TeaUtils.Client.equalString(params.bodyType, "string")) {
                    var str: String = try await TeaUtils.Client.readAsString(_response.body)
                    return [
                        "body": str as! String,
                        "headers": _response.headers,
                        "statusCode": _response.statusCode
                    ]
                }
                else if (TeaUtils.Client.equalString(params.bodyType, "json")) {
                    var obj: Any = try await TeaUtils.Client.readAsJSON(_response.body)
                    var res: [String: Any] = try TeaUtils.Client.assertAsMap(obj)
                    return [
                        "body": res as! [String: Any],
                        "headers": _response.headers,
                        "statusCode": _response.statusCode
                    ]
                }
                else if (TeaUtils.Client.equalString(params.bodyType, "array")) {
                    var arr: Any = try await TeaUtils.Client.readAsJSON(_response.body)
                    return [
                        "body": arr as! Any,
                        "headers": _response.headers,
                        "statusCode": _response.statusCode
                    ]
                }
                else {
                    return [
                        "headers": _response.headers,
                        "statusCode": _response.statusCode
                    ]
                }
            }
            catch {
                if (Tea.TeaCore.isRetryable(error)) {
                    _lastException = error as! Tea.RetryableError
                    continue
                }
                throw error
            }
        }
        throw Tea.UnretryableError(_lastRequest, _lastException)
    }

    @available(macOS 10.15, iOS 13, tvOS 13, watchOS 6, *)
    public func callApi(_ params: Params, _ request: OpenApiRequest, _ runtime: TeaUtils.RuntimeOptions) async throws -> [String: Any] {
        if (TeaUtils.Client.isUnset(params)) {
            throw Tea.ReuqestError([
                "code": "ParameterMissing",
                "message": "'params' can not be unset"
            ])
        }
        if (TeaUtils.Client.isUnset(self._signatureAlgorithm) || !TeaUtils.Client.equalString(self._signatureAlgorithm, "v2")) {
            return try await doRequest(params as! Params, request as! OpenApiRequest, runtime as! TeaUtils.RuntimeOptions)
        }
        else if (TeaUtils.Client.equalString(params.style, "ROA") && TeaUtils.Client.equalString(params.reqBodyType, "json")) {
            return try await doROARequest(params.action ?? "", params.version ?? "", params.protocol_ ?? "", params.method ?? "", params.authType ?? "", params.pathname ?? "", params.bodyType ?? "", request as! OpenApiRequest, runtime as! TeaUtils.RuntimeOptions)
        }
        else if (TeaUtils.Client.equalString(params.style, "ROA")) {
            return try await doROARequestWithForm(params.action ?? "", params.version ?? "", params.protocol_ ?? "", params.method ?? "", params.authType ?? "", params.pathname ?? "", params.bodyType ?? "", request as! OpenApiRequest, runtime as! TeaUtils.RuntimeOptions)
        }
        else {
            return try await doRPCRequest(params.action ?? "", params.version ?? "", params.protocol_ ?? "", params.method ?? "", params.authType ?? "", params.bodyType ?? "", request as! OpenApiRequest, runtime as! TeaUtils.RuntimeOptions)
        }
    }

    public func getUserAgent() -> String {
        var userAgent: String = TeaUtils.Client.getUserAgent(self._userAgent)
        return userAgent as! String
    }

    @available(macOS 10.15, iOS 13, tvOS 13, watchOS 6, *)
    public func getAccessKeyId() async throws -> String {
        if (TeaUtils.Client.isUnset(self._credential)) {
            return ""
        }
        var accessKeyId: String = try await self._credential!.getAccessKeyId()
        return accessKeyId as! String
    }

    @available(macOS 10.15, iOS 13, tvOS 13, watchOS 6, *)
    public func getAccessKeySecret() async throws -> String {
        if (TeaUtils.Client.isUnset(self._credential)) {
            return ""
        }
        var secret: String = try await self._credential!.getAccessKeySecret()
        return secret as! String
    }

    @available(macOS 10.15, iOS 13, tvOS 13, watchOS 6, *)
    public func getSecurityToken() async throws -> String {
        if (TeaUtils.Client.isUnset(self._credential)) {
            return ""
        }
        var token: String = try await self._credential!.getSecurityToken()
        return token as! String
    }

    @available(macOS 10.15, iOS 13, tvOS 13, watchOS 6, *)
    public func getBearerToken() async throws -> String {
        if (TeaUtils.Client.isUnset(self._credential)) {
            return ""
        }
        var token: String = self._credential!.getBearerToken()
        return token as! String
    }

    @available(macOS 10.15, iOS 13, tvOS 13, watchOS 6, *)
    public func getType() async throws -> String {
        if (TeaUtils.Client.isUnset(self._credential)) {
            return ""
        }
        var authType: String = self._credential!.getType()
        return authType as! String
    }

    public static func defaultAny(_ inputValue: Any?, _ defaultValue: Any?) -> Any {
        if (TeaUtils.Client.isUnset(inputValue)) {
            return defaultValue as! Any
        }
        return inputValue as! Any
    }

    public func checkConfig(_ config: Config) throws -> Void {
        if (TeaUtils.Client.empty(self._endpointRule) && TeaUtils.Client.empty(config.endpoint)) {
            throw Tea.ReuqestError([
                "code": "ParameterMissing",
                "message": "'config.endpoint' can not be empty"
            ])
        }
    }

    public func setRpcHeaders(_ headers: [String: String]) throws -> Void {
        self._headers = headers
    }

    public func getRpcHeaders() throws -> [String: String] {
        var headers: [String: String] = self._headers ?? [:]
        self._headers = nil
        return headers as! [String: String]
    }
}
