// This file is auto-generated, don't edit it
/**
 * This is for OpenApi Util 
 */
import * as $tea from '@alicloud/tea-typescript';
import * as $dara from '@darabonba/typescript';
import Credential from '@alicloud/credentials';
import { Readable } from 'stream';
import querystring from 'querystring';
import crypto from 'crypto';
import os from 'os';


const PEM_BEGIN = "-----BEGIN PRIVATE KEY-----\n";
const PEM_END = "\n-----END PRIVATE KEY-----";
const DEFAULT_USER_AGENT = `AlibabaCloud (${os.platform()}; ${os.arch()}) Node.js/${process.version} Core/1.0.1 TeaDSL/2`;


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
 * @remarks
 * Model for initing client
 */
export class Config extends $tea.Model {
  /**
   * @remarks
   * accesskey id
   */
  accessKeyId?: string;
  /**
   * @remarks
   * accesskey secret
   */
  accessKeySecret?: string;
  /**
   * @remarks
   * security token
   */
  securityToken?: string;
  /**
   * @remarks
   * bearer token
   * 
   * @example
   * the-bearer-token
   */
  bearerToken?: string;
  /**
   * @remarks
   * http protocol
   * 
   * @example
   * http
   */
  protocol?: string;
  /**
   * @remarks
   * http method
   * 
   * @example
   * GET
   */
  method?: string;
  /**
   * @remarks
   * region id
   * 
   * @example
   * cn-hangzhou
   */
  regionId?: string;
  /**
   * @remarks
   * read timeout
   * 
   * @example
   * 10
   */
  readTimeout?: number;
  /**
   * @remarks
   * connect timeout
   * 
   * @example
   * 10
   */
  connectTimeout?: number;
  /**
   * @remarks
   * http proxy
   * 
   * @example
   * http://localhost
   */
  httpProxy?: string;
  /**
   * @remarks
   * https proxy
   * 
   * @example
   * https://localhost
   */
  httpsProxy?: string;
  /**
   * @remarks
   * credential
   */
  credential?: Credential;
  /**
   * @remarks
   * endpoint
   * 
   * @example
   * cs.aliyuncs.com
   */
  endpoint?: string;
  /**
   * @remarks
   * proxy white list
   * 
   * @example
   * http://localhost
   */
  noProxy?: string;
  /**
   * @remarks
   * max idle conns
   * 
   * @example
   * 3
   */
  maxIdleConns?: number;
  /**
   * @remarks
   * network for endpoint
   * 
   * @example
   * public
   */
  network?: string;
  /**
   * @remarks
   * user agent
   * 
   * @example
   * Alibabacloud/1
   */
  userAgent?: string;
  /**
   * @remarks
   * suffix for endpoint
   * 
   * @example
   * aliyun
   */
  suffix?: string;
  /**
   * @remarks
   * socks5 proxy
   */
  socks5Proxy?: string;
  /**
   * @remarks
   * socks5 network
   * 
   * @example
   * TCP
   */
  socks5NetWork?: string;
  /**
   * @remarks
   * endpoint type
   * 
   * @example
   * internal
   */
  endpointType?: string;
  /**
   * @remarks
   * OpenPlatform endpoint
   * 
   * @example
   * openplatform.aliyuncs.com
   */
  openPlatformEndpoint?: string;
  /**
   * @remarks
   * credential type
   * 
   * @example
   * access_key
   * 
   * @deprecated
   */
  type?: string;
  /**
   * @remarks
   * Signature Version
   * 
   * @example
   * v1
   */
  signatureVersion?: string;
  /**
   * @remarks
   * Signature Algorithm
   * 
   * @example
   * ACS3-HMAC-SHA256
   */
  signatureAlgorithm?: string;
  /**
   * @remarks
   * Global Parameters
   */
  globalParameters?: GlobalParameters;
  /**
   * @remarks
   * privite key for client certificate
   * 
   * @example
   * MIIEvQ
   */
  key?: string;
  /**
   * @remarks
   * client certificate
   * 
   * @example
   * -----BEGIN CERTIFICATE-----
   * xxx-----END CERTIFICATE-----
   */
  cert?: string;
  /**
   * @remarks
   * server certificate
   * 
   * @example
   * -----BEGIN CERTIFICATE-----
   * xxx-----END CERTIFICATE-----
   */
  ca?: string;
  /**
   * @remarks
   * disable HTTP/2
   * 
   * @example
   * false
   */
  disableHttp2?: boolean;
  tlsMinVersion?: string; 
  /**
   * @remarks
   * retry options
   */
  retryOptions?: $dara.RetryOptions;
  static names(): { [key: string]: string } {
    return {
      accessKeyId: 'accessKeyId',
      accessKeySecret: 'accessKeySecret',
      securityToken: 'securityToken',
      bearerToken: 'bearerToken',
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
      disableHttp2: 'disableHttp2',
      tlsMinVersion: 'tlsMinVersion',
      retryOptions: 'retryOptions',
    };
  }

  static types(): { [key: string]: any } {
    return {
      accessKeyId: 'string',
      accessKeySecret: 'string',
      securityToken: 'string',
      bearerToken: 'string',
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
      disableHttp2: 'boolean',
      tlsMinVersion: 'string',
      retryOptions: $dara.RetryOptions,
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

function replaceRepeatList(target: { [key: string]: string }, repeat: any[], prefix: string) {
  if (prefix) {
    prefix = prefix + '.';
  }
  for (var i = 0; i < repeat.length; i++) {
    var item = repeat[i];
    let key = prefix + (i + 1);
    if (typeof item === 'undefined' || item == null) {
      continue;
    }
    if (Array.isArray(item)) {
      replaceRepeatList(target, item, key);
    } else if (item instanceof Object) {
      flatMap(target, item, key);
    } else {
      target[key] = item.toString();
    }
  }
}

function flatMap(target: { [key: string]: any }, params: { [key: string]: any }, prefix: string = '') {
  if (prefix) {
    prefix = prefix + '.';
  }
  params = toMap(params);
  let keys = Object.keys(params);
  for (let i = 0; i < keys.length; i++) {
    let key = keys[i];
    let value = params[key];
    key = prefix + key;
    if (typeof value === 'undefined' || value == null) {
      continue;
    }

    if (Array.isArray(value)) {
      replaceRepeatList(target, value, key);
    } else if (value instanceof Object) {
      flatMap(target, value, key);
    } else {
      target[key] = value.toString();
    }
  }
  return target;
}

function filter(value: string): string {
  return value.replace(/[\t\n\r\f]/g, ' ');
}

function getCanonicalizedHeaders(headers: { [key: string]: string }): string {
  const prefix = 'x-acs-';
  const keys = Object.keys(headers);

  const canonicalizedKeys = [];
  for (let i = 0; i < keys.length; i++) {
    const key = keys[i];
    if (key.startsWith(prefix)) {
      canonicalizedKeys.push(key);
    }
  }

  canonicalizedKeys.sort();

  var result = '';
  for (let i = 0; i < canonicalizedKeys.length; i++) {
    const key = canonicalizedKeys[i];
    result += `${key}:${filter(headers[key]).trim()}\n`;
  }

  return result;
}

function getCanonicalizedResource(uriPattern: string, query: { [key: string]: string }): string {
  const keys = !query ? [] : Object.keys(query).sort();

  if (keys.length === 0) {
    return uriPattern;
  }

  var result = [];
  for (var i = 0; i < keys.length; i++) {
    const key = keys[i];
    result.push(`${key}=${query[key]}`);
  }

  return `${uriPattern}?${result.join('&')}`;
}

function getAuthorizationQueryString(query: { [key: string]: string }): string {
  let canonicalQueryArray = [];
  const keys = !query ? [] : Object.keys(query).sort();
  for (let i = 0; i < keys.length; i++) {
    const key = keys[i];
    let param = key + '='
    if (typeof query[key] !== 'undefined' && query[key] !== null) {
      param = param + encode(query[key])
    }
    canonicalQueryArray.push(param)
  }
  return canonicalQueryArray.join('&');
}

function getAuthorizationHeaders(header: { [key: string]: string }): {} {
  let canonicalheaders = "";
  let tmp = {};
  const keys = !header ? [] : Object.keys(header);
  for (let i = 0; i < keys.length; i++) {
    const key = keys[i];
    const lowerKey = keys[i].toLowerCase();
    if (lowerKey.startsWith("x-acs-") || lowerKey === "host" || lowerKey === "content-type") {
      if (tmp[lowerKey]) {
        tmp[lowerKey].push((header[key] || "").trim());
      } else {
        tmp[lowerKey] = [(header[key] || "").trim()];
      }
    }
  }
  var hsKeys = Object.keys(tmp).sort();
  for (let i = 0; i < hsKeys.length; i++) {
    const hsKey = hsKeys[i];
    let listSort = tmp[hsKey].sort();
    canonicalheaders += `${hsKey}:${listSort.join(",")}\n`;
  }

  return { canonicalheaders, hsKeys };
}

function encode(str: string) {
  var result = encodeURIComponent(str);

  return result.replace(/!/g, '%21')
    .replace(/'/g, '%27')
    .replace(/\(/g, '%28')
    .replace(/\)/g, '%29')
    .replace(/\*/g, '%2A');
}

function normalize(params: { [key: string]: any }) {
  var list = [];
  var flated: { [key: string]: string } = {};
  flatMap(flated, params);
  var keys = Object.keys(flated).sort();
  for (let i = 0; i < keys.length; i++) {
    var key = keys[i];
    var value = flated[key];
    list.push([encode(key), encode(value)]);
  }
  return list;
}

function canonicalize(normalized: any[]) {
  var fields = [];
  for (var i = 0; i < normalized.length; i++) {
    var [key, value] = normalized[i];
    fields.push(key + '=' + value);
  }
  return fields.join('&');
}

function isModelClass(t: any): boolean {
  if (!t) {
    return false;
  }
  return typeof t.types === 'function' && typeof t.names === 'function';
}

function isObjectOrArray(t: any): boolean {
  return Array.isArray(t) || (t instanceof Object && typeof t !== 'function');
}

function getTimeLeft(rateLimit: string | null): number | null {
  if (rateLimit) {
    const pairs = rateLimit.split(',');
    for (const pair of pairs) {
      const kv = pair.split(':');
      if (kv.length === 2) {
        const key = kv[0].trim();
        const value = kv[1].trim();
        if (key === 'TimeLeft') {
          const timeLeftValue = parseInt(value, 10);
          if (isNaN(timeLeftValue)) {
            return null;
          }
          return timeLeftValue;
        }
      }
    }
  }
  return null;
}

function toMap(input: any) {
  if (!isObjectOrArray(input)) {
    return null;
  } else if (input instanceof $tea.Model) {
    return $tea.toMap(input);
  } else if (input && input.toMap && typeof input.toMap === 'function') {
    // 解决跨版本 Model 不互认的问题
    return input.toMap();
  } else if (Array.isArray(input)) {
    const result = [];
    input.forEach((value) => {
      if (isObjectOrArray(value)) {
        result.push(toMap(value));
      } else {
        result.push(value);
      }
    });

    return result;
  } else if (input instanceof Object) {
    const result = {};
    Object.entries(input).forEach(([key, value]) => {
      if (isObjectOrArray(value)) {
        result[key] = toMap(value);
      } else {
        result[key] = value;
      }
    });

    return result;
  }
}

export default class Client {

  /**
   * Convert all params of body other than type of readable into content 
   * @param body source Model
   * @param content target Model
   * @return void
   */
  static convert(input: $tea.Model, output: $tea.Model): void {
    if (!output) {
      return;
    }
    let inputModel = Object.assign({}, input);
    let constructor = <any>output.constructor;
    let types = constructor.types();
    // let constructor = <any>output.constructor;
    for (let key of Object.keys(constructor.names())) {
      if (inputModel[key] !== null && inputModel[key] !== undefined) {
        if (isModelClass(types[key])) {
          output[key] = new types[key](output[key]);
          Client.convert(inputModel[key], output[key]);
        } else if (types[key] && types[key].type === 'array') {
          output[key] = inputModel[key].map(function (d) {
            if (isModelClass(types[key].itemType)) {
              var item = new types[key].itemType({});
              Client.convert(d, item);
              return item;
            }
            return d;
          });
        } else if (types[key] && types[key].type === 'map') {
          output[key] = {};
          Object.keys(inputModel[key]).map(function (d) {
            if (isModelClass(types[key].valueType)) {
              var item = new types[key].valueType({});
              Client.convert(inputModel[key][d], item);
              output[key][d] = item;
            } else {
              output[key][d] = inputModel[key][d];
            }
          });
        } else {
          output[key] = inputModel[key];
        }
      }
    }
  }

  /**
   * If endpointType is internal, use internal endpoint
   * If serverUse is true and endpointType is accelerate, use accelerate endpoint
   * Default return endpoint
   * @param serverUse whether use accelerate endpoint
   * @param endpointType value must be internal or accelerate
   * @return the final endpoint
   */
  static getEndpoint(endpoint: string, serverUse: boolean, endpointType: string): string {
    if (endpointType == "internal") {
      let strs = endpoint.split(".");
      strs[0] += "-internal";
      endpoint = strs.join(".")
    }
    if (serverUse && endpointType == "accelerate") {
      return "oss-accelerate.aliyuncs.com"
    }

    return endpoint
  }

  /**
   * Get throttling param
   * @param the response headers
   * @return time left
   */
  static getThrottlingTimeLeft(headers: {[key: string ]: string}): number { 
    const rateLimitForUserApi = headers["x-ratelimit-user-api"];
    const rateLimitForUser = headers["x-ratelimit-user"];
    const timeLeftForUserApi = getTimeLeft(rateLimitForUserApi);
    const timeLeftForUser = getTimeLeft(rateLimitForUser);

    if (timeLeftForUserApi > timeLeftForUser) {
        return timeLeftForUserApi;
    } else {
        return timeLeftForUser;
    }
  }

  /**
   * Hash the raw data with signatureAlgorithm
   * @param raw hashing data
   * @param signatureAlgorithm the autograph method
   * @return hashed bytes
   */
  static hash(raw: Buffer, signatureAlgorithm: string): Buffer {
    if (signatureAlgorithm === "ACS3-HMAC-SHA256" || signatureAlgorithm === "ACS3-RSA-SHA256") {
      const obj = crypto.createHash('sha256');
      obj.update(raw);
      return obj.digest();
    } else if (signatureAlgorithm == "ACS3-HMAC-SM3") {
      const obj = crypto.createHash('sm3');
      obj.update(raw);
      return obj.digest();
    }
  }

  /**
   * Generate a nonce string
   * @return the nonce string
   */
  static getNonce(): string {
    let counter = 0;
    let last;
    const machine = os.hostname();
    const pid = process.pid;

    var val = Math.floor(Math.random() * 1000000000000);
    if (val === last) {
      counter++;
    } else {
      counter = 0;
    }

    last = val;

    var uid = `${machine}${pid}${val}${counter}`;
    var shasum = crypto.createHash('md5');
    shasum.update(uid);
    return shasum.digest('hex');
  }

  /**
   * Get the string to be signed according to request
   * @param request  which contains signed messages
   * @return the signed string
   */
  static getStringToSign(request: $tea.Request): string {
    const method = request.method;
    const accept = request.headers['accept'];
    const contentMD5 = request.headers['content-md5'] || '';
    const contentType = request.headers['content-type'] || '';
    const date = request.headers['date'] || '';
    const header = `${method}\n${accept}\n${contentMD5}\n${contentType}\n${date}\n`;
    const canonicalizedHeaders = getCanonicalizedHeaders(request.headers);
    const canonicalizedResource = getCanonicalizedResource(request.pathname, request.query);

    return `${header}${canonicalizedHeaders}${canonicalizedResource}`;
  }

  /**
   * Get signature according to stringToSign, secret
   * @param stringToSign  the signed string
   * @param secret accesskey secret
   * @return the signature
   */
  static getROASignature(stringToSign: string, secret: string): string {
    const utf8Buff = Buffer.from(stringToSign, 'utf8');
    return crypto.createHmac('sha1', secret).update(utf8Buff).digest('base64')
  }

  /**
   * Parse filter into a form string
   * @param filter object
   * @return the string
   */
  static toForm(filter: {[key: string]: any}): string {
    if (!filter) {
      return '';
    }
    let target = {};
    flatMap(target, filter);
    return $dara.Form.toFormString(target);
  }

  /**
   * Get timestamp
   * @return the timestamp string
   */
  static getTimestamp(): string {
    let date = new Date();
    let YYYY = date.getUTCFullYear();
    let MM =`${date.getUTCMonth() + 1}`.padStart(2, '0');
    let DD =`${date.getUTCDate()}`.padStart(2, '0');
    let HH =`${date.getUTCHours()}`.padStart(2, '0');
    let mm =`${date.getUTCMinutes()}`.padStart(2, '0');
    let ss =`${date.getUTCSeconds()}`.padStart(2, '0');
    return `${YYYY}-${MM}-${DD}T${HH}:${mm}:${ss}Z`;
  }

  /**
   * Get UTC string
   * @return the UTC string
   */
  static getDateUTCString(): string {
    const now = new Date();
    return now.toUTCString();
  }

  /**
   * Parse filter into a object which's type is map[string]string
   * @param filter query param
   * @return the object
   */
  static query(filter: {[key: string]: any}): {[key: string ]: string} {
    if (!filter) {
      return {};
    }
    let ret: { [key: string]: string } = {};
    flatMap(ret, filter);
    return ret;
  }

  /**
   * Get signature according to signedParams, method and secret
   * @param signedParams params which need to be signed
   * @param method http method e.g. GET
   * @param secret AccessKeySecret
   * @return the signature
   */
  static getRPCSignature(signedParams: {[key: string ]: string}, method: string, secret: string): string {
    var normalized = normalize(signedParams);
    var canonicalized = canonicalize(normalized);
    var stringToSign = `${method}&${encode('/')}&${encode(canonicalized)}`;
    const key = secret + '&';
    return crypto.createHmac('sha1', key).update(stringToSign).digest('base64');
  }

  /**
   * Parse array into a string with specified style
   * @param array the array
   * @param prefix the prefix string
   * @style specified style e.g. repeatList
   * @return the string
   */
  static arrayToStringWithSpecifiedStyle(array: any, prefix: string, style: string): string {
    if (!array) {
      return '';
    }
    if (style === 'repeatList') {
      let target = {};
      replaceRepeatList(target, array, prefix);
      return querystring.stringify(target, '&&');
    } else if (style === 'json') {
      return JSON.stringify(toMap(array));
    } else if (style === 'simple') {
      return array.join(',');
    } else if (style === 'spaceDelimited') {
      return array.join(' ');
    } else if (style === 'pipeDelimited') {
      return array.join('|');
    } else {
      return '';
    }
  }

  static stringifyMapValue(m: { [key: string]: any }): { [key: string]: string } {
    if (!m) {
      return m;
    }

    const result: { [key: string]: string } = {};
    for (const [key, value] of Object.entries(m)) {
      if (typeof value === 'undefined' || value === null) {
        continue;
      }
      result[key] = String(value);
    }
    return result;
  }


  static toArray(input: any): { [key: string]: any }[] {
    if (!(input instanceof Array)) {
      return null;
    }
    let ret = [];
    input.forEach((model) => {
      if (!model) {
        return;
      }
      ret.push($tea.toMap(model));
    })
    return ret;
  }

  static getEndpointRules(product: string, regionId: string, endpointType: string, network: string, suffix: string): string {
    let result;
    if (network && network.length && network != "public") {
      network = "-" + network;
    } else {
      network = "";
    }
    suffix = suffix || "";
    if (suffix.length) {
      suffix = "-" + suffix;
    }
    if (endpointType == "regional") {
      if (!regionId || !regionId.length) {
        throw new Error("RegionId is empty, please set a valid RegionId");
      }
      result = `${product}${suffix}${network}.${regionId}.aliyuncs.com`;
    } else {
      result = `${product}${suffix}${network}.aliyuncs.com`;
    }
    return result;
  }

  /**
   * Transform input as map.
   */
  static parseToMap(input: any): {[key: string ]: any} {
    return toMap(input);
  }

  /**
   * Get the authorization 
   * @param request request params
   * @param signatureAlgorithm the autograph method
   * @param payload the hashed request
   * @param accessKey the accessKey string
   * @param accessKeySecret the accessKeySecret string
   * @return authorization string
   */
  static getAuthorization(request: $tea.Request, signatureAlgorithm: string, payload: string, accessKey: string, accessKeySecret: string): string {
    const canonicalURI = (request.pathname || "").replace("+", "%20").replace("*", "%2A").replace("%7E", "~");
    const method = request.method;
    const canonicalQueryString = getAuthorizationQueryString(request.query);
    const tuple = getAuthorizationHeaders(request.headers);
    const canonicalheaders = tuple["canonicalheaders"];
    const signedHeaders = tuple["hsKeys"];

    const canonicalRequest = method + "\n" + canonicalURI + "\n" + canonicalQueryString + "\n" + canonicalheaders + "\n" +
      signedHeaders.join(";") + "\n" + payload;
    let raw = Buffer.from(canonicalRequest);
    const stringToSign = signatureAlgorithm + "\n" + Client.hash(raw, signatureAlgorithm).toString("hex");
    const signature = Client.signatureMethod(accessKeySecret, stringToSign, signatureAlgorithm).toString("hex");
    const auth = `${signatureAlgorithm} Credential=${accessKey},SignedHeaders=${signedHeaders.join(';')},Signature=${signature}`;

    return auth;
  }

  static getUserAgent(userAgent: string): string {
    if (!userAgent || !userAgent.length) {
      return DEFAULT_USER_AGENT;
    }
    return DEFAULT_USER_AGENT + " " + userAgent;
  }

  static signatureMethod(secret: string, source: string, signatureAlgorithm: string): Buffer {
    if (signatureAlgorithm === "ACS3-HMAC-SHA256") {
      const obj = crypto.createHmac('sha256', secret);
      obj.update(source);
      return obj.digest();
    } else if (signatureAlgorithm === "ACS3-HMAC-SM3") {
      const obj = crypto.createHmac('sm3', secret);
      obj.update(source);
      return obj.digest();
    } else if (signatureAlgorithm === "ACS3-RSA-SHA256") {

      if (!secret.startsWith(PEM_BEGIN)) {
        secret = PEM_BEGIN + secret;
      }
      if (!secret.endsWith(PEM_END)) {
        secret = secret + PEM_END;
      }
      
      var signerObject = crypto.createSign("RSA-SHA256");
      signerObject.update(source);
      var signature = signerObject.sign({ key: secret, padding: crypto.constants.RSA_PKCS1_PADDING });
      return signature;
    }
  }

}
