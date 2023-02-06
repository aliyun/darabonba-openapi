// This file is auto-generated, don't edit it
/**
 * This is for OpenApi SDK
 */
import Util, * as $Util from '@alicloud/tea-util';
import Credential, * as $Credential from '@alicloud/credentials';
import OpenApiUtil from '@alicloud/openapi-util';
import SPI, * as $SPI from '@alicloud/gateway-spi';
import XML from '@alicloud/tea-xml';
import { Readable } from 'stream';
import * as $tea from '@alicloud/tea-typescript';

export class GlobalParameters extends $tea.Model {
  headers?: { [key: string]: string };
  queries?: { [key: string]: string };
  static names(): { [key: string]: string } {
    return {
      headers: 'headers',
      queries: 'queries',
    };
  }

  static types(): { [key: string]: any } {
    return {
      headers: { 'type': 'map', 'keyType': 'string', 'valueType': 'string' },
      queries: { 'type': 'map', 'keyType': 'string', 'valueType': 'string' },
    };
  }

  constructor(map?: { [key: string]: any }) {
    super(map);
  }
}

/**
 * Model for initing client
 */
export class Config extends $tea.Model {
  accessKeyId?: string;
  accessKeySecret?: string;
  securityToken?: string;
  protocol?: string;
  method?: string;
  regionId?: string;
  readTimeout?: number;
  connectTimeout?: number;
  httpProxy?: string;
  httpsProxy?: string;
  credential?: Credential;
  endpoint?: string;
  noProxy?: string;
  maxIdleConns?: number;
  network?: string;
  userAgent?: string;
  suffix?: string;
  socks5Proxy?: string;
  socks5NetWork?: string;
  endpointType?: string;
  openPlatformEndpoint?: string;
  type?: string;
  signatureVersion?: string;
  signatureAlgorithm?: string;
  globalParameters?: GlobalParameters;
  key?: string;
  cert?: string;
  ca?: string;
  static names(): { [key: string]: string } {
    return {
      accessKeyId: 'accessKeyId',
      accessKeySecret: 'accessKeySecret',
      securityToken: 'securityToken',
      protocol: 'protocol',
      method: 'method',
      regionId: 'regionId',
      readTimeout: 'readTimeout',
      connectTimeout: 'connectTimeout',
      httpProxy: 'httpProxy',
      httpsProxy: 'httpsProxy',
      credential: 'credential',
      endpoint: 'endpoint',
      noProxy: 'noProxy',
      maxIdleConns: 'maxIdleConns',
      network: 'network',
      userAgent: 'userAgent',
      suffix: 'suffix',
      socks5Proxy: 'socks5Proxy',
      socks5NetWork: 'socks5NetWork',
      endpointType: 'endpointType',
      openPlatformEndpoint: 'openPlatformEndpoint',
      type: 'type',
      signatureVersion: 'signatureVersion',
      signatureAlgorithm: 'signatureAlgorithm',
      globalParameters: 'globalParameters',
      key: 'key',
      cert: 'cert',
      ca: 'ca',
    };
  }

  static types(): { [key: string]: any } {
    return {
      accessKeyId: 'string',
      accessKeySecret: 'string',
      securityToken: 'string',
      protocol: 'string',
      method: 'string',
      regionId: 'string',
      readTimeout: 'number',
      connectTimeout: 'number',
      httpProxy: 'string',
      httpsProxy: 'string',
      credential: Credential,
      endpoint: 'string',
      noProxy: 'string',
      maxIdleConns: 'number',
      network: 'string',
      userAgent: 'string',
      suffix: 'string',
      socks5Proxy: 'string',
      socks5NetWork: 'string',
      endpointType: 'string',
      openPlatformEndpoint: 'string',
      type: 'string',
      signatureVersion: 'string',
      signatureAlgorithm: 'string',
      globalParameters: GlobalParameters,
      key: 'string',
      cert: 'string',
      ca: 'string',
    };
  }

  constructor(map?: { [key: string]: any }) {
    super(map);
  }
}

export class OpenApiRequest extends $tea.Model {
  headers?: { [key: string]: string };
  query?: { [key: string]: string };
  body?: any;
  stream?: Readable;
  hostMap?: { [key: string]: string };
  endpointOverride?: string;
  static names(): { [key: string]: string } {
    return {
      headers: 'headers',
      query: 'query',
      body: 'body',
      stream: 'stream',
      hostMap: 'hostMap',
      endpointOverride: 'endpointOverride',
    };
  }

  static types(): { [key: string]: any } {
    return {
      headers: { 'type': 'map', 'keyType': 'string', 'valueType': 'string' },
      query: { 'type': 'map', 'keyType': 'string', 'valueType': 'string' },
      body: 'any',
      stream: 'Readable',
      hostMap: { 'type': 'map', 'keyType': 'string', 'valueType': 'string' },
      endpointOverride: 'string',
    };
  }

  constructor(map?: { [key: string]: any }) {
    super(map);
  }
}

export class Params extends $tea.Model {
  action: string;
  version: string;
  protocol: string;
  pathname: string;
  method: string;
  authType: string;
  bodyType: string;
  reqBodyType: string;
  style?: string;
  static names(): { [key: string]: string } {
    return {
      action: 'action',
      version: 'version',
      protocol: 'protocol',
      pathname: 'pathname',
      method: 'method',
      authType: 'authType',
      bodyType: 'bodyType',
      reqBodyType: 'reqBodyType',
      style: 'style',
    };
  }

  static types(): { [key: string]: any } {
    return {
      action: 'string',
      version: 'string',
      protocol: 'string',
      pathname: 'string',
      method: 'string',
      authType: 'string',
      bodyType: 'string',
      reqBodyType: 'string',
      style: 'string',
    };
  }

  constructor(map?: { [key: string]: any }) {
    super(map);
  }
}


export default class Client {
  _endpoint: string;
  _regionId: string;
  _protocol: string;
  _method: string;
  _userAgent: string;
  _endpointRule: string;
  _endpointMap: {[key: string ]: string};
  _suffix: string;
  _readTimeout: number;
  _connectTimeout: number;
  _httpProxy: string;
  _httpsProxy: string;
  _socks5Proxy: string;
  _socks5NetWork: string;
  _noProxy: string;
  _network: string;
  _productId: string;
  _maxIdleConns: number;
  _endpointType: string;
  _openPlatformEndpoint: string;
  _credential: Credential;
  _signatureVersion: string;
  _signatureAlgorithm: string;
  _headers: {[key: string ]: string};
  _spi: SPI;
  _globalParameters: GlobalParameters;
  _key: string;
  _cert: string;
  _ca: string;

  /**
   * Init client with Config
   * @param config config contains the necessary information to create a client
   */
  constructor(config: Config) {
    if (Util.isUnset(config)) {
      throw $tea.newError({
        code: "ParameterMissing",
        message: "'config' can not be unset",
      });
    }

    if (!Util.empty(config.accessKeyId) && !Util.empty(config.accessKeySecret)) {
      if (!Util.empty(config.securityToken)) {
        config.type = "sts";
      } else {
        config.type = "access_key";
      }

      let credentialConfig = new $Credential.Config({
        accessKeyId: config.accessKeyId,
        type: config.type,
        accessKeySecret: config.accessKeySecret,
      });
      credentialConfig.securityToken = config.securityToken;
      this._credential = new Credential(credentialConfig);
    } else if (!Util.isUnset(config.credential)) {
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
    this._key = config.key;
    this._cert = config.cert;
    this._ca = config.ca;
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
  async doRPCRequest(action: string, version: string, protocol: string, method: string, authType: string, bodyType: string, request: OpenApiRequest, runtime: $Util.RuntimeOptions): Promise<{[key: string]: any}> {
    let _runtime: { [key: string]: any } = {
      timeouted: "retry",
      key: Util.defaultString(runtime.key, this._key),
      cert: Util.defaultString(runtime.cert, this._cert),
      ca: Util.defaultString(runtime.ca, this._ca),
      readTimeout: Util.defaultNumber(runtime.readTimeout, this._readTimeout),
      connectTimeout: Util.defaultNumber(runtime.connectTimeout, this._connectTimeout),
      httpProxy: Util.defaultString(runtime.httpProxy, this._httpProxy),
      httpsProxy: Util.defaultString(runtime.httpsProxy, this._httpsProxy),
      noProxy: Util.defaultString(runtime.noProxy, this._noProxy),
      socks5Proxy: Util.defaultString(runtime.socks5Proxy, this._socks5Proxy),
      socks5NetWork: Util.defaultString(runtime.socks5NetWork, this._socks5NetWork),
      maxIdleConns: Util.defaultNumber(runtime.maxIdleConns, this._maxIdleConns),
      retry: {
        retryable: runtime.autoretry,
        maxAttempts: Util.defaultNumber(runtime.maxAttempts, 3),
      },
      backoff: {
        policy: Util.defaultString(runtime.backoffPolicy, "no"),
        period: Util.defaultNumber(runtime.backoffPeriod, 1),
      },
      ignoreSSL: runtime.ignoreSSL,
    }

    let _lastRequest = null;
    let _now = Date.now();
    let _retryTimes = 0;
    while ($tea.allowRetry(_runtime['retry'], _retryTimes, _now)) {
      if (_retryTimes > 0) {
        let _backoffTime = $tea.getBackoffTime(_runtime['backoff'], _retryTimes);
        if (_backoffTime > 0) {
          await $tea.sleep(_backoffTime);
        }
      }

      _retryTimes = _retryTimes + 1;
      try {
        let request_ = new $tea.Request();
        request_.protocol = Util.defaultString(this._protocol, protocol);
        request_.method = method;
        request_.pathname = "/";
        let globalQueries : {[key: string ]: string} = { };
        let globalHeaders : {[key: string ]: string} = { };
        if (!Util.isUnset(this._globalParameters)) {
          let globalParams = this._globalParameters;
          if (!Util.isUnset(globalParams.queries)) {
            globalQueries = globalParams.queries;
          }

          if (!Util.isUnset(globalParams.headers)) {
            globalHeaders = globalParams.headers;
          }

        }

        request_.query = {
          Action: action,
          Format: "json",
          Version: version,
          Timestamp: OpenApiUtil.getTimestamp(),
          SignatureNonce: Util.getNonce(),
          ...globalQueries,
          ...request.query,
        };
        let headers = this.getRpcHeaders();
        if (Util.isUnset(headers)) {
          // endpoint is setted in product client
          request_.headers = {
            host: this._endpoint,
            'x-acs-version': version,
            'x-acs-action': action,
            'user-agent': this.getUserAgent(),
            ...globalHeaders,
          };
        } else {
          request_.headers = {
            host: this._endpoint,
            'x-acs-version': version,
            'x-acs-action': action,
            'user-agent': this.getUserAgent(),
            ...globalHeaders,
            ...headers,
          };
        }

        if (!Util.isUnset(request.body)) {
          let m = Util.assertAsMap(request.body);
          let tmp = Util.anyifyMapValue(OpenApiUtil.query(m));
          request_.body = new $tea.BytesReadable(Util.toFormString(tmp));
          request_.headers["content-type"] = "application/x-www-form-urlencoded";
        }

        if (!Util.equalString(authType, "Anonymous")) {
          let accessKeyId = await this.getAccessKeyId();
          let accessKeySecret = await this.getAccessKeySecret();
          let securityToken = await this.getSecurityToken();
          if (!Util.empty(securityToken)) {
            request_.query["SecurityToken"] = securityToken;
          }

          request_.query["SignatureMethod"] = "HMAC-SHA1";
          request_.query["SignatureVersion"] = "1.0";
          request_.query["AccessKeyId"] = accessKeyId;
          let t : {[key: string ]: any} = null;
          if (!Util.isUnset(request.body)) {
            t = Util.assertAsMap(request.body);
          }

          let signedParam = {
            ...request_.query,
            ...OpenApiUtil.query(t),
          };
          request_.query["Signature"] = OpenApiUtil.getRPCSignature(signedParam, request_.method, accessKeySecret);
        }

        _lastRequest = request_;
        let response_ = await $tea.doAction(request_, _runtime);

        if (Util.is4xx(response_.statusCode) || Util.is5xx(response_.statusCode)) {
          let _res = await Util.readAsJSON(response_.body);
          let err = Util.assertAsMap(_res);
          let requestId = Client.defaultAny(err["RequestId"], err["requestId"]);
          err["statusCode"] = response_.statusCode;
          throw $tea.newError({
            code: `${Client.defaultAny(err["Code"], err["code"])}`,
            message: `code: ${response_.statusCode}, ${Client.defaultAny(err["Message"], err["message"])} request id: ${requestId}`,
            data: err,
            description: `${Client.defaultAny(err["Description"], err["description"])}`,
            accessDeniedDetail: Client.defaultAny(err["AccessDeniedDetail"], err["accessDeniedDetail"]),
          });
        }

        if (Util.equalString(bodyType, "binary")) {
          let resp = {
            body: response_.body,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
          return resp;
        } else if (Util.equalString(bodyType, "byte")) {
          let byt = await Util.readAsBytes(response_.body);
          return {
            body: byt,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else if (Util.equalString(bodyType, "string")) {
          let str = await Util.readAsString(response_.body);
          return {
            body: str,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else if (Util.equalString(bodyType, "json")) {
          let obj = await Util.readAsJSON(response_.body);
          let res = Util.assertAsMap(obj);
          return {
            body: res,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else if (Util.equalString(bodyType, "array")) {
          let arr = await Util.readAsJSON(response_.body);
          return {
            body: arr,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else {
          return {
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        }

      } catch (ex) {
        if ($tea.isRetryable(ex)) {
          continue;
        }
        throw ex;
      }
    }

    throw $tea.newUnretryableError(_lastRequest);
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
  async doROARequest(action: string, version: string, protocol: string, method: string, authType: string, pathname: string, bodyType: string, request: OpenApiRequest, runtime: $Util.RuntimeOptions): Promise<{[key: string]: any}> {
    let _runtime: { [key: string]: any } = {
      timeouted: "retry",
      key: Util.defaultString(runtime.key, this._key),
      cert: Util.defaultString(runtime.cert, this._cert),
      ca: Util.defaultString(runtime.ca, this._ca),
      readTimeout: Util.defaultNumber(runtime.readTimeout, this._readTimeout),
      connectTimeout: Util.defaultNumber(runtime.connectTimeout, this._connectTimeout),
      httpProxy: Util.defaultString(runtime.httpProxy, this._httpProxy),
      httpsProxy: Util.defaultString(runtime.httpsProxy, this._httpsProxy),
      noProxy: Util.defaultString(runtime.noProxy, this._noProxy),
      socks5Proxy: Util.defaultString(runtime.socks5Proxy, this._socks5Proxy),
      socks5NetWork: Util.defaultString(runtime.socks5NetWork, this._socks5NetWork),
      maxIdleConns: Util.defaultNumber(runtime.maxIdleConns, this._maxIdleConns),
      retry: {
        retryable: runtime.autoretry,
        maxAttempts: Util.defaultNumber(runtime.maxAttempts, 3),
      },
      backoff: {
        policy: Util.defaultString(runtime.backoffPolicy, "no"),
        period: Util.defaultNumber(runtime.backoffPeriod, 1),
      },
      ignoreSSL: runtime.ignoreSSL,
    }

    let _lastRequest = null;
    let _now = Date.now();
    let _retryTimes = 0;
    while ($tea.allowRetry(_runtime['retry'], _retryTimes, _now)) {
      if (_retryTimes > 0) {
        let _backoffTime = $tea.getBackoffTime(_runtime['backoff'], _retryTimes);
        if (_backoffTime > 0) {
          await $tea.sleep(_backoffTime);
        }
      }

      _retryTimes = _retryTimes + 1;
      try {
        let request_ = new $tea.Request();
        request_.protocol = Util.defaultString(this._protocol, protocol);
        request_.method = method;
        request_.pathname = pathname;
        let globalQueries : {[key: string ]: string} = { };
        let globalHeaders : {[key: string ]: string} = { };
        if (!Util.isUnset(this._globalParameters)) {
          let globalParams = this._globalParameters;
          if (!Util.isUnset(globalParams.queries)) {
            globalQueries = globalParams.queries;
          }

          if (!Util.isUnset(globalParams.headers)) {
            globalHeaders = globalParams.headers;
          }

        }

        request_.headers = {
          date: Util.getDateUTCString(),
          host: this._endpoint,
          accept: "application/json",
          'x-acs-signature-nonce': Util.getNonce(),
          'x-acs-signature-method': "HMAC-SHA1",
          'x-acs-signature-version': "1.0",
          'x-acs-version': version,
          'x-acs-action': action,
          'user-agent': Util.getUserAgent(this._userAgent),
          ...globalHeaders,
          ...request.headers,
        };
        if (!Util.isUnset(request.body)) {
          request_.body = new $tea.BytesReadable(Util.toJSONString(request.body));
          request_.headers["content-type"] = "application/json; charset=utf-8";
        }

        request_.query = globalQueries;
        if (!Util.isUnset(request.query)) {
          request_.query = {
            ...request_.query,
            ...request.query,
          };
        }

        if (!Util.equalString(authType, "Anonymous")) {
          let accessKeyId = await this.getAccessKeyId();
          let accessKeySecret = await this.getAccessKeySecret();
          let securityToken = await this.getSecurityToken();
          if (!Util.empty(securityToken)) {
            request_.headers["x-acs-accesskey-id"] = accessKeyId;
            request_.headers["x-acs-security-token"] = securityToken;
          }

          let stringToSign = OpenApiUtil.getStringToSign(request_);
          request_.headers["authorization"] = `acs ${accessKeyId}:${OpenApiUtil.getROASignature(stringToSign, accessKeySecret)}`;
        }

        _lastRequest = request_;
        let response_ = await $tea.doAction(request_, _runtime);

        if (Util.equalNumber(response_.statusCode, 204)) {
          return {
            headers: response_.headers,
          };
        }

        if (Util.is4xx(response_.statusCode) || Util.is5xx(response_.statusCode)) {
          let _res = await Util.readAsJSON(response_.body);
          let err = Util.assertAsMap(_res);
          let requestId = Client.defaultAny(err["RequestId"], err["requestId"]);
          requestId = Client.defaultAny(requestId, err["requestid"]);
          err["statusCode"] = response_.statusCode;
          throw $tea.newError({
            code: `${Client.defaultAny(err["Code"], err["code"])}`,
            message: `code: ${response_.statusCode}, ${Client.defaultAny(err["Message"], err["message"])} request id: ${requestId}`,
            data: err,
            description: `${Client.defaultAny(err["Description"], err["description"])}`,
            accessDeniedDetail: Client.defaultAny(err["AccessDeniedDetail"], err["accessDeniedDetail"]),
          });
        }

        if (Util.equalString(bodyType, "binary")) {
          let resp = {
            body: response_.body,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
          return resp;
        } else if (Util.equalString(bodyType, "byte")) {
          let byt = await Util.readAsBytes(response_.body);
          return {
            body: byt,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else if (Util.equalString(bodyType, "string")) {
          let str = await Util.readAsString(response_.body);
          return {
            body: str,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else if (Util.equalString(bodyType, "json")) {
          let obj = await Util.readAsJSON(response_.body);
          let res = Util.assertAsMap(obj);
          return {
            body: res,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else if (Util.equalString(bodyType, "array")) {
          let arr = await Util.readAsJSON(response_.body);
          return {
            body: arr,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else {
          return {
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        }

      } catch (ex) {
        if ($tea.isRetryable(ex)) {
          continue;
        }
        throw ex;
      }
    }

    throw $tea.newUnretryableError(_lastRequest);
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
  async doROARequestWithForm(action: string, version: string, protocol: string, method: string, authType: string, pathname: string, bodyType: string, request: OpenApiRequest, runtime: $Util.RuntimeOptions): Promise<{[key: string]: any}> {
    let _runtime: { [key: string]: any } = {
      timeouted: "retry",
      key: Util.defaultString(runtime.key, this._key),
      cert: Util.defaultString(runtime.cert, this._cert),
      ca: Util.defaultString(runtime.ca, this._ca),
      readTimeout: Util.defaultNumber(runtime.readTimeout, this._readTimeout),
      connectTimeout: Util.defaultNumber(runtime.connectTimeout, this._connectTimeout),
      httpProxy: Util.defaultString(runtime.httpProxy, this._httpProxy),
      httpsProxy: Util.defaultString(runtime.httpsProxy, this._httpsProxy),
      noProxy: Util.defaultString(runtime.noProxy, this._noProxy),
      socks5Proxy: Util.defaultString(runtime.socks5Proxy, this._socks5Proxy),
      socks5NetWork: Util.defaultString(runtime.socks5NetWork, this._socks5NetWork),
      maxIdleConns: Util.defaultNumber(runtime.maxIdleConns, this._maxIdleConns),
      retry: {
        retryable: runtime.autoretry,
        maxAttempts: Util.defaultNumber(runtime.maxAttempts, 3),
      },
      backoff: {
        policy: Util.defaultString(runtime.backoffPolicy, "no"),
        period: Util.defaultNumber(runtime.backoffPeriod, 1),
      },
      ignoreSSL: runtime.ignoreSSL,
    }

    let _lastRequest = null;
    let _now = Date.now();
    let _retryTimes = 0;
    while ($tea.allowRetry(_runtime['retry'], _retryTimes, _now)) {
      if (_retryTimes > 0) {
        let _backoffTime = $tea.getBackoffTime(_runtime['backoff'], _retryTimes);
        if (_backoffTime > 0) {
          await $tea.sleep(_backoffTime);
        }
      }

      _retryTimes = _retryTimes + 1;
      try {
        let request_ = new $tea.Request();
        request_.protocol = Util.defaultString(this._protocol, protocol);
        request_.method = method;
        request_.pathname = pathname;
        let globalQueries : {[key: string ]: string} = { };
        let globalHeaders : {[key: string ]: string} = { };
        if (!Util.isUnset(this._globalParameters)) {
          let globalParams = this._globalParameters;
          if (!Util.isUnset(globalParams.queries)) {
            globalQueries = globalParams.queries;
          }

          if (!Util.isUnset(globalParams.headers)) {
            globalHeaders = globalParams.headers;
          }

        }

        request_.headers = {
          date: Util.getDateUTCString(),
          host: this._endpoint,
          accept: "application/json",
          'x-acs-signature-nonce': Util.getNonce(),
          'x-acs-signature-method': "HMAC-SHA1",
          'x-acs-signature-version': "1.0",
          'x-acs-version': version,
          'x-acs-action': action,
          'user-agent': Util.getUserAgent(this._userAgent),
          ...globalHeaders,
          ...request.headers,
        };
        if (!Util.isUnset(request.body)) {
          let m = Util.assertAsMap(request.body);
          request_.body = new $tea.BytesReadable(OpenApiUtil.toForm(m));
          request_.headers["content-type"] = "application/x-www-form-urlencoded";
        }

        request_.query = globalQueries;
        if (!Util.isUnset(request.query)) {
          request_.query = {
            ...request_.query,
            ...request.query,
          };
        }

        if (!Util.equalString(authType, "Anonymous")) {
          let accessKeyId = await this.getAccessKeyId();
          let accessKeySecret = await this.getAccessKeySecret();
          let securityToken = await this.getSecurityToken();
          if (!Util.empty(securityToken)) {
            request_.headers["x-acs-accesskey-id"] = accessKeyId;
            request_.headers["x-acs-security-token"] = securityToken;
          }

          let stringToSign = OpenApiUtil.getStringToSign(request_);
          request_.headers["authorization"] = `acs ${accessKeyId}:${OpenApiUtil.getROASignature(stringToSign, accessKeySecret)}`;
        }

        _lastRequest = request_;
        let response_ = await $tea.doAction(request_, _runtime);

        if (Util.equalNumber(response_.statusCode, 204)) {
          return {
            headers: response_.headers,
          };
        }

        if (Util.is4xx(response_.statusCode) || Util.is5xx(response_.statusCode)) {
          let _res = await Util.readAsJSON(response_.body);
          let err = Util.assertAsMap(_res);
          err["statusCode"] = response_.statusCode;
          throw $tea.newError({
            code: `${Client.defaultAny(err["Code"], err["code"])}`,
            message: `code: ${response_.statusCode}, ${Client.defaultAny(err["Message"], err["message"])} request id: ${Client.defaultAny(err["RequestId"], err["requestId"])}`,
            data: err,
            description: `${Client.defaultAny(err["Description"], err["description"])}`,
            accessDeniedDetail: Client.defaultAny(err["AccessDeniedDetail"], err["accessDeniedDetail"]),
          });
        }

        if (Util.equalString(bodyType, "binary")) {
          let resp = {
            body: response_.body,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
          return resp;
        } else if (Util.equalString(bodyType, "byte")) {
          let byt = await Util.readAsBytes(response_.body);
          return {
            body: byt,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else if (Util.equalString(bodyType, "string")) {
          let str = await Util.readAsString(response_.body);
          return {
            body: str,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else if (Util.equalString(bodyType, "json")) {
          let obj = await Util.readAsJSON(response_.body);
          let res = Util.assertAsMap(obj);
          return {
            body: res,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else if (Util.equalString(bodyType, "array")) {
          let arr = await Util.readAsJSON(response_.body);
          return {
            body: arr,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else {
          return {
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        }

      } catch (ex) {
        if ($tea.isRetryable(ex)) {
          continue;
        }
        throw ex;
      }
    }

    throw $tea.newUnretryableError(_lastRequest);
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
  async doRequest(params: Params, request: OpenApiRequest, runtime: $Util.RuntimeOptions): Promise<{[key: string]: any}> {
    let _runtime: { [key: string]: any } = {
      timeouted: "retry",
      key: Util.defaultString(runtime.key, this._key),
      cert: Util.defaultString(runtime.cert, this._cert),
      ca: Util.defaultString(runtime.ca, this._ca),
      readTimeout: Util.defaultNumber(runtime.readTimeout, this._readTimeout),
      connectTimeout: Util.defaultNumber(runtime.connectTimeout, this._connectTimeout),
      httpProxy: Util.defaultString(runtime.httpProxy, this._httpProxy),
      httpsProxy: Util.defaultString(runtime.httpsProxy, this._httpsProxy),
      noProxy: Util.defaultString(runtime.noProxy, this._noProxy),
      socks5Proxy: Util.defaultString(runtime.socks5Proxy, this._socks5Proxy),
      socks5NetWork: Util.defaultString(runtime.socks5NetWork, this._socks5NetWork),
      maxIdleConns: Util.defaultNumber(runtime.maxIdleConns, this._maxIdleConns),
      retry: {
        retryable: runtime.autoretry,
        maxAttempts: Util.defaultNumber(runtime.maxAttempts, 3),
      },
      backoff: {
        policy: Util.defaultString(runtime.backoffPolicy, "no"),
        period: Util.defaultNumber(runtime.backoffPeriod, 1),
      },
      ignoreSSL: runtime.ignoreSSL,
    }

    let _lastRequest = null;
    let _now = Date.now();
    let _retryTimes = 0;
    while ($tea.allowRetry(_runtime['retry'], _retryTimes, _now)) {
      if (_retryTimes > 0) {
        let _backoffTime = $tea.getBackoffTime(_runtime['backoff'], _retryTimes);
        if (_backoffTime > 0) {
          await $tea.sleep(_backoffTime);
        }
      }

      _retryTimes = _retryTimes + 1;
      try {
        let request_ = new $tea.Request();
        request_.protocol = Util.defaultString(this._protocol, params.protocol);
        request_.method = params.method;
        request_.pathname = params.pathname;
        let globalQueries : {[key: string ]: string} = { };
        let globalHeaders : {[key: string ]: string} = { };
        if (!Util.isUnset(this._globalParameters)) {
          let globalParams = this._globalParameters;
          if (!Util.isUnset(globalParams.queries)) {
            globalQueries = globalParams.queries;
          }

          if (!Util.isUnset(globalParams.headers)) {
            globalHeaders = globalParams.headers;
          }

        }

        request_.query = {
          ...globalQueries,
          ...request.query,
        };
        // endpoint is setted in product client
        request_.headers = {
          host: this._endpoint,
          'x-acs-version': params.version,
          'x-acs-action': params.action,
          'user-agent': this.getUserAgent(),
          'x-acs-date': OpenApiUtil.getTimestamp(),
          'x-acs-signature-nonce': Util.getNonce(),
          accept: "application/json",
          ...globalHeaders,
          ...request.headers,
        };
        if (Util.equalString(params.style, "RPC")) {
          let headers = this.getRpcHeaders();
          if (!Util.isUnset(headers)) {
            request_.headers = {
              ...request_.headers,
              ...headers,
            };
          }

        }

        let signatureAlgorithm = Util.defaultString(this._signatureAlgorithm, "ACS3-HMAC-SHA256");
        let hashedRequestPayload = OpenApiUtil.hexEncode(OpenApiUtil.hash(Util.toBytes(""), signatureAlgorithm));
        if (!Util.isUnset(request.stream)) {
          let tmp = await Util.readAsBytes(request.stream);
          hashedRequestPayload = OpenApiUtil.hexEncode(OpenApiUtil.hash(tmp, signatureAlgorithm));
          request_.body = new $tea.BytesReadable(tmp);
          request_.headers["content-type"] = "application/octet-stream";
        } else {
          if (!Util.isUnset(request.body)) {
            if (Util.equalString(params.reqBodyType, "json")) {
              let jsonObj = Util.toJSONString(request.body);
              hashedRequestPayload = OpenApiUtil.hexEncode(OpenApiUtil.hash(Util.toBytes(jsonObj), signatureAlgorithm));
              request_.body = new $tea.BytesReadable(jsonObj);
              request_.headers["content-type"] = "application/json; charset=utf-8";
            } else {
              let m = Util.assertAsMap(request.body);
              let formObj = OpenApiUtil.toForm(m);
              hashedRequestPayload = OpenApiUtil.hexEncode(OpenApiUtil.hash(Util.toBytes(formObj), signatureAlgorithm));
              request_.body = new $tea.BytesReadable(formObj);
              request_.headers["content-type"] = "application/x-www-form-urlencoded";
            }

          }

        }

        request_.headers["x-acs-content-sha256"] = hashedRequestPayload;
        if (!Util.equalString(params.authType, "Anonymous")) {
          let authType = await this.getType();
          if (Util.equalString(authType, "bearer")) {
            let bearerToken = await this.getBearerToken();
            request_.headers["x-acs-bearer-token"] = bearerToken;
          } else {
            let accessKeyId = await this.getAccessKeyId();
            let accessKeySecret = await this.getAccessKeySecret();
            let securityToken = await this.getSecurityToken();
            if (!Util.empty(securityToken)) {
              request_.headers["x-acs-accesskey-id"] = accessKeyId;
              request_.headers["x-acs-security-token"] = securityToken;
            }

            request_.headers["Authorization"] = OpenApiUtil.getAuthorization(request_, signatureAlgorithm, hashedRequestPayload, accessKeyId, accessKeySecret);
          }

        }

        _lastRequest = request_;
        let response_ = await $tea.doAction(request_, _runtime);

        if (Util.is4xx(response_.statusCode) || Util.is5xx(response_.statusCode)) {
          let err : {[key: string ]: any} = { };
          if (!Util.isUnset(response_.headers["content-type"]) && Util.equalString(response_.headers["content-type"], "text/xml;charset=utf-8")) {
            let _str = await Util.readAsString(response_.body);
            let respMap = XML.parseXml(_str, null);
            err = Util.assertAsMap(respMap["Error"]);
          } else {
            let _res = await Util.readAsJSON(response_.body);
            err = Util.assertAsMap(_res);
          }

          err["statusCode"] = response_.statusCode;
          throw $tea.newError({
            code: `${Client.defaultAny(err["Code"], err["code"])}`,
            message: `code: ${response_.statusCode}, ${Client.defaultAny(err["Message"], err["message"])} request id: ${Client.defaultAny(err["RequestId"], err["requestId"])}`,
            data: err,
            description: `${Client.defaultAny(err["Description"], err["description"])}`,
            accessDeniedDetail: Client.defaultAny(err["AccessDeniedDetail"], err["accessDeniedDetail"]),
          });
        }

        if (Util.equalString(params.bodyType, "binary")) {
          let resp = {
            body: response_.body,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
          return resp;
        } else if (Util.equalString(params.bodyType, "byte")) {
          let byt = await Util.readAsBytes(response_.body);
          return {
            body: byt,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else if (Util.equalString(params.bodyType, "string")) {
          let str = await Util.readAsString(response_.body);
          return {
            body: str,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else if (Util.equalString(params.bodyType, "json")) {
          let obj = await Util.readAsJSON(response_.body);
          let res = Util.assertAsMap(obj);
          return {
            body: res,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else if (Util.equalString(params.bodyType, "array")) {
          let arr = await Util.readAsJSON(response_.body);
          return {
            body: arr,
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        } else {
          return {
            headers: response_.headers,
            statusCode: response_.statusCode,
          };
        }

      } catch (ex) {
        if ($tea.isRetryable(ex)) {
          continue;
        }
        throw ex;
      }
    }

    throw $tea.newUnretryableError(_lastRequest);
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
  async execute(params: Params, request: OpenApiRequest, runtime: $Util.RuntimeOptions): Promise<{[key: string]: any}> {
    let _runtime: { [key: string]: any } = {
      timeouted: "retry",
      key: Util.defaultString(runtime.key, this._key),
      cert: Util.defaultString(runtime.cert, this._cert),
      ca: Util.defaultString(runtime.ca, this._ca),
      readTimeout: Util.defaultNumber(runtime.readTimeout, this._readTimeout),
      connectTimeout: Util.defaultNumber(runtime.connectTimeout, this._connectTimeout),
      httpProxy: Util.defaultString(runtime.httpProxy, this._httpProxy),
      httpsProxy: Util.defaultString(runtime.httpsProxy, this._httpsProxy),
      noProxy: Util.defaultString(runtime.noProxy, this._noProxy),
      socks5Proxy: Util.defaultString(runtime.socks5Proxy, this._socks5Proxy),
      socks5NetWork: Util.defaultString(runtime.socks5NetWork, this._socks5NetWork),
      maxIdleConns: Util.defaultNumber(runtime.maxIdleConns, this._maxIdleConns),
      retry: {
        retryable: runtime.autoretry,
        maxAttempts: Util.defaultNumber(runtime.maxAttempts, 3),
      },
      backoff: {
        policy: Util.defaultString(runtime.backoffPolicy, "no"),
        period: Util.defaultNumber(runtime.backoffPeriod, 1),
      },
      ignoreSSL: runtime.ignoreSSL,
    }

    let _lastRequest = null;
    let _now = Date.now();
    let _retryTimes = 0;
    while ($tea.allowRetry(_runtime['retry'], _retryTimes, _now)) {
      if (_retryTimes > 0) {
        let _backoffTime = $tea.getBackoffTime(_runtime['backoff'], _retryTimes);
        if (_backoffTime > 0) {
          await $tea.sleep(_backoffTime);
        }
      }

      _retryTimes = _retryTimes + 1;
      try {
        let request_ = new $tea.Request();
        // spi = new Gateway();//Gateway implements SPI，这一步在产品 SDK 中实例化
        let headers = this.getRpcHeaders();
        let globalQueries : {[key: string ]: string} = { };
        let globalHeaders : {[key: string ]: string} = { };
        if (!Util.isUnset(this._globalParameters)) {
          let globalParams = this._globalParameters;
          if (!Util.isUnset(globalParams.queries)) {
            globalQueries = globalParams.queries;
          }

          if (!Util.isUnset(globalParams.headers)) {
            globalHeaders = globalParams.headers;
          }

        }

        let requestContext = new $SPI.InterceptorContextRequest({
          headers: {
            ...globalHeaders,
            ...request.headers,
            ...headers,
          },
          query: {
            ...globalQueries,
            ...request.query,
          },
          body: request.body,
          stream: request.stream,
          hostMap: request.hostMap,
          pathname: params.pathname,
          productId: this._productId,
          action: params.action,
          version: params.version,
          protocol: Util.defaultString(this._protocol, params.protocol),
          method: Util.defaultString(this._method, params.method),
          authType: params.authType,
          bodyType: params.bodyType,
          reqBodyType: params.reqBodyType,
          style: params.style,
          credential: this._credential,
          signatureVersion: this._signatureVersion,
          signatureAlgorithm: this._signatureAlgorithm,
          userAgent: this.getUserAgent(),
        });
        let configurationContext = new $SPI.InterceptorContextConfiguration({
          regionId: this._regionId,
          endpoint: Util.defaultString(request.endpointOverride, this._endpoint),
          endpointRule: this._endpointRule,
          endpointMap: this._endpointMap,
          endpointType: this._endpointType,
          network: this._network,
          suffix: this._suffix,
        });
        let interceptorContext = new $SPI.InterceptorContext({
          request: requestContext,
          configuration: configurationContext,
        });
        let attributeMap = new $SPI.AttributeMap({ });
        // 1. spi.modifyConfiguration(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
        await this._spi.modifyConfiguration(interceptorContext, attributeMap);
        // 2. spi.modifyRequest(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
        await this._spi.modifyRequest(interceptorContext, attributeMap);
        request_.protocol = interceptorContext.request.protocol;
        request_.method = interceptorContext.request.method;
        request_.pathname = interceptorContext.request.pathname;
        request_.query = interceptorContext.request.query;
        request_.body = interceptorContext.request.stream;
        request_.headers = interceptorContext.request.headers;
        _lastRequest = request_;
        let response_ = await $tea.doAction(request_, _runtime);

        let responseContext = new $SPI.InterceptorContextResponse({
          statusCode: response_.statusCode,
          headers: response_.headers,
          body: response_.body,
        });
        interceptorContext.response = responseContext;
        // 3. spi.modifyResponse(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
        await this._spi.modifyResponse(interceptorContext, attributeMap);
        return {
          headers: interceptorContext.response.headers,
          statusCode: interceptorContext.response.statusCode,
          body: interceptorContext.response.deserializedBody,
        };
      } catch (ex) {
        if ($tea.isRetryable(ex)) {
          continue;
        }
        throw ex;
      }
    }

    throw $tea.newUnretryableError(_lastRequest);
  }

  async callApi(params: Params, request: OpenApiRequest, runtime: $Util.RuntimeOptions): Promise<{[key: string]: any}> {
    if (Util.isUnset(params)) {
      throw $tea.newError({
        code: "ParameterMissing",
        message: "'params' can not be unset",
      });
    }

    if (Util.isUnset(this._signatureAlgorithm) || !Util.equalString(this._signatureAlgorithm, "v2")) {
      return await this.doRequest(params, request, runtime);
    } else if (Util.equalString(params.style, "ROA") && Util.equalString(params.reqBodyType, "json")) {
      return await this.doROARequest(params.action, params.version, params.protocol, params.method, params.authType, params.pathname, params.bodyType, request, runtime);
    } else if (Util.equalString(params.style, "ROA")) {
      return await this.doROARequestWithForm(params.action, params.version, params.protocol, params.method, params.authType, params.pathname, params.bodyType, request, runtime);
    } else {
      return await this.doRPCRequest(params.action, params.version, params.protocol, params.method, params.authType, params.bodyType, request, runtime);
    }

  }

  /**
   * Get user agent
   * @return user agent
   */
  getUserAgent(): string {
    let userAgent = Util.getUserAgent(this._userAgent);
    return userAgent;
  }

  /**
   * Get accesskey id by using credential
   * @return accesskey id
   */
  async getAccessKeyId(): Promise<string> {
    if (Util.isUnset(this._credential)) {
      return "";
    }

    let accessKeyId = await this._credential.getAccessKeyId();
    return accessKeyId;
  }

  /**
   * Get accesskey secret by using credential
   * @return accesskey secret
   */
  async getAccessKeySecret(): Promise<string> {
    if (Util.isUnset(this._credential)) {
      return "";
    }

    let secret = await this._credential.getAccessKeySecret();
    return secret;
  }

  /**
   * Get security token by using credential
   * @return security token
   */
  async getSecurityToken(): Promise<string> {
    if (Util.isUnset(this._credential)) {
      return "";
    }

    let token = await this._credential.getSecurityToken();
    return token;
  }

  /**
   * Get bearer token by credential
   * @return bearer token
   */
  async getBearerToken(): Promise<string> {
    if (Util.isUnset(this._credential)) {
      return "";
    }

    let token = this._credential.getBearerToken();
    return token;
  }

  /**
   * Get credential type by credential
   * @return credential type e.g. access_key
   */
  async getType(): Promise<string> {
    if (Util.isUnset(this._credential)) {
      return "";
    }

    let authType = this._credential.getType();
    return authType;
  }

  /**
   * If inputValue is not null, return it or return defaultValue
   * @param inputValue  users input value
   * @param defaultValue default value
   * @return the final result
   */
  static defaultAny(inputValue: any, defaultValue: any): any {
    if (Util.isUnset(inputValue)) {
      return defaultValue;
    }

    return inputValue;
  }

  /**
   * If the endpointRule and config.endpoint are empty, throw error
   * @param config config contains the necessary information to create a client
   */
  checkConfig(config: Config): void {
    if (Util.empty(this._endpointRule) && Util.empty(config.endpoint)) {
      throw $tea.newError({
        code: "ParameterMissing",
        message: "'config.endpoint' can not be empty",
      });
    }

  }

  /**
   * set gateway client
   * @param spi.
   */
  setGatewayClient(spi: SPI): void {
    this._spi = spi;
  }

  /**
   * set RPC header for debug
   * @param headers headers for debug, this header can be used only once.
   */
  setRpcHeaders(headers: {[key: string ]: string}): void {
    this._headers = headers;
  }

  /**
   * get RPC header for debug
   */
  getRpcHeaders(): {[key: string ]: string} {
    let headers : {[key: string ]: string} = this._headers;
    this._headers = null;
    return headers;
  }

}
