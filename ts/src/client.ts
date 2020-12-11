// This file is auto-generated, don't edit it
/**
 * This is for OpenApi SDK
 */
import Util, * as $Util from '@alicloud/tea-util';
import Credential, * as $Credential from '@alicloud/credentials';
import OpenApiUtil from '@alicloud/openapi-util';
import * as $tea from '@alicloud/tea-typescript';

/**
 * Model for initing client
 */
export class Config extends $tea.Model {
  accessKeyId?: string;
  accessKeySecret?: string;
  securityToken?: string;
  protocol?: string;
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
  static names(): { [key: string]: string } {
    return {
      accessKeyId: 'accessKeyId',
      accessKeySecret: 'accessKeySecret',
      securityToken: 'securityToken',
      protocol: 'protocol',
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
    };
  }

  static types(): { [key: string]: any } {
    return {
      accessKeyId: 'string',
      accessKeySecret: 'string',
      securityToken: 'string',
      protocol: 'string',
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
  static names(): { [key: string]: string } {
    return {
      headers: 'headers',
      query: 'query',
      body: 'body',
    };
  }

  static types(): { [key: string]: any } {
    return {
      headers: { 'type': 'map', 'keyType': 'string', 'valueType': 'string' },
      query: { 'type': 'map', 'keyType': 'string', 'valueType': 'string' },
      body: 'any',
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

  /**
   * Init client with Config
   * @param config config contains the necessary information to create a client
   */
  constructor(config: Config) {
    if (Util.isUnset($tea.toMap(config))) {
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
        securityToken: config.securityToken,
      });
      this._credential = new Credential(credentialConfig);
    } else if (!Util.isUnset(config.credential)) {
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
      readTimeout: Util.defaultNumber(runtime.readTimeout, this._readTimeout),
      connectTimeout: Util.defaultNumber(runtime.connectTimeout, this._connectTimeout),
      httpProxy: Util.defaultString(runtime.httpProxy, this._httpProxy),
      httpsProxy: Util.defaultString(runtime.httpsProxy, this._httpsProxy),
      noProxy: Util.defaultString(runtime.noProxy, this._noProxy),
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
        request_.query = {
          Action: action,
          Format: "json",
          Version: version,
          Timestamp: OpenApiUtil.getTimestamp(),
          SignatureNonce: Util.getNonce(),
          ...request.query,
        };
        // endpoint is setted in product client
        request_.headers = {
          host: this._endpoint,
          'x-acs-version': version,
          'x-acs-action': action,
          'user-agent': this.getUserAgent(),
        };
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
          throw $tea.newError({
            code: `${Client.defaultAny(err["Code"], err["code"])}`,
            message: `code: ${response_.statusCode}, ${Client.defaultAny(err["Message"], err["message"])} request id: ${Client.defaultAny(err["RequestId"], err["requestId"])}`,
            data: err,
          });
        }

        if (Util.equalString(bodyType, "binary")) {
          let resp = {
            body: response_.body,
            headers: response_.headers,
          };
          return resp;
        } else if (Util.equalString(bodyType, "byte")) {
          let byt = await Util.readAsBytes(response_.body);
          return {
            body: byt,
            headers: response_.headers,
          };
        } else if (Util.equalString(bodyType, "string")) {
          let str = await Util.readAsString(response_.body);
          return {
            body: str,
            headers: response_.headers,
          };
        } else if (Util.equalString(bodyType, "json")) {
          let obj = await Util.readAsJSON(response_.body);
          let res = Util.assertAsMap(obj);
          return {
            body: res,
            headers: response_.headers,
          };
        } else if (Util.equalString(bodyType, "array")) {
          let arr = await Util.readAsJSON(response_.body);
          return {
            body: arr,
            headers: response_.headers,
          };
        } else {
          return {
            headers: response_.headers,
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
      readTimeout: Util.defaultNumber(runtime.readTimeout, this._readTimeout),
      connectTimeout: Util.defaultNumber(runtime.connectTimeout, this._connectTimeout),
      httpProxy: Util.defaultString(runtime.httpProxy, this._httpProxy),
      httpsProxy: Util.defaultString(runtime.httpsProxy, this._httpsProxy),
      noProxy: Util.defaultString(runtime.noProxy, this._noProxy),
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
          ...request.headers,
        };
        if (!Util.isUnset(request.body)) {
          request_.body = new $tea.BytesReadable(Util.toJSONString(request.body));
          request_.headers["content-type"] = "application/json; charset=utf-8";
        }

        if (!Util.isUnset(request.query)) {
          request_.query = request.query;
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
          throw $tea.newError({
            code: `${Client.defaultAny(err["Code"], err["code"])}`,
            message: `code: ${response_.statusCode}, ${Client.defaultAny(err["Message"], err["message"])} request id: ${Client.defaultAny(err["RequestId"], err["requestId"])}`,
            data: err,
          });
        }

        if (Util.equalString(bodyType, "binary")) {
          let resp = {
            body: response_.body,
            headers: response_.headers,
          };
          return resp;
        } else if (Util.equalString(bodyType, "byte")) {
          let byt = await Util.readAsBytes(response_.body);
          return {
            body: byt,
            headers: response_.headers,
          };
        } else if (Util.equalString(bodyType, "string")) {
          let str = await Util.readAsString(response_.body);
          return {
            body: str,
            headers: response_.headers,
          };
        } else if (Util.equalString(bodyType, "json")) {
          let obj = await Util.readAsJSON(response_.body);
          let res = Util.assertAsMap(obj);
          return {
            body: res,
            headers: response_.headers,
          };
        } else if (Util.equalString(bodyType, "array")) {
          let arr = await Util.readAsJSON(response_.body);
          return {
            body: arr,
            headers: response_.headers,
          };
        } else {
          return {
            headers: response_.headers,
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
      readTimeout: Util.defaultNumber(runtime.readTimeout, this._readTimeout),
      connectTimeout: Util.defaultNumber(runtime.connectTimeout, this._connectTimeout),
      httpProxy: Util.defaultString(runtime.httpProxy, this._httpProxy),
      httpsProxy: Util.defaultString(runtime.httpsProxy, this._httpsProxy),
      noProxy: Util.defaultString(runtime.noProxy, this._noProxy),
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
          ...request.headers,
        };
        if (!Util.isUnset(request.body)) {
          let m = Util.assertAsMap(request.body);
          request_.body = new $tea.BytesReadable(OpenApiUtil.toForm(m));
          request_.headers["content-type"] = "application/x-www-form-urlencoded";
        }

        if (!Util.isUnset(request.query)) {
          request_.query = request.query;
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
          throw $tea.newError({
            code: `${Client.defaultAny(err["Code"], err["code"])}`,
            message: `code: ${response_.statusCode}, ${Client.defaultAny(err["Message"], err["message"])} request id: ${Client.defaultAny(err["RequestId"], err["requestId"])}`,
            data: err,
          });
        }

        if (Util.equalString(bodyType, "binary")) {
          let resp = {
            body: response_.body,
            headers: response_.headers,
          };
          return resp;
        } else if (Util.equalString(bodyType, "byte")) {
          let byt = await Util.readAsBytes(response_.body);
          return {
            body: byt,
            headers: response_.headers,
          };
        } else if (Util.equalString(bodyType, "string")) {
          let str = await Util.readAsString(response_.body);
          return {
            body: str,
            headers: response_.headers,
          };
        } else if (Util.equalString(bodyType, "json")) {
          let obj = await Util.readAsJSON(response_.body);
          let res = Util.assertAsMap(obj);
          return {
            body: res,
            headers: response_.headers,
          };
        } else if (Util.equalString(bodyType, "array")) {
          let arr = await Util.readAsJSON(response_.body);
          return {
            body: arr,
            headers: response_.headers,
          };
        } else {
          return {
            headers: response_.headers,
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

}
