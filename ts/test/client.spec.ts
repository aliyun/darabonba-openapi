'use strict';

import http from 'http';
import url from 'url';
import 'mocha';
import assert from 'assert';
import { AddressInfo } from 'net';
import { Readable } from 'stream';

import OpenApi, * as $OpenApi from "../src/client";
import OpenApiUtil, * as $OpenApiUtil from '../src/utils';
import Credential, * as $Credential from '@alicloud/credentials';
import * as $dara from '@darabonba/typescript';
import POP, * as $POP from '@alicloud/gateway-pop';

const server = http.createServer((req, res) => {
  if (req.headers['timeout'] === 'true') {
    const timeout = setTimeout(() => {
      res.writeHead(500, { 'Content-Type': 'text/plain' });
      res.end('Server Timeout');
    }, 5000);

    // 清除定时器，在请求结束时清除
    req.on('close', () => {
      clearTimeout(timeout);
    });
  } else {
    const urlObj = url.parse(req.url, false);
    let data = '';
    req.on('data', function (chunk) {
      data += chunk;
    });
    req.on('end', function () {
      let headers = {};
      let keys = Object.keys(req.headers);
      for (let index = 0; index < keys.length; index++) {
        let key = keys[index];
        headers[key] = <string>req.headers[key];
      }
      
      headers = {
        ...headers,
        'pathname': urlObj.pathname,
        'http-method': req.method,
        'raw-query': urlObj.query,
        'raw-body': data,
        'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4',
      };
      if(urlObj.pathname === '/sse') {
          res.writeHead(200, {
            ...headers,
          });
          let count = 0;
          const timer = setInterval(() => {
            if (count >= 5) {
              clearInterval(timer);
              res.end();
              return;
            }
            res.write(`data: ${JSON.stringify({ count: count })}\nevent: flow\nid: sse-test\nretry: 3\n:heartbeat\n\n`);
            count++;
          }, 100);
          return;
      }
      
      let responseBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";
      switch (headers['bodytype']) {
        case 'array':
          responseBody = "[\"AppId\", \"ClassId\", \"UserId\"]";
          res.writeHead(200, {
            ...headers,
          });
          break;
        case 'error':
          responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"" +
            ", \"Description\":\"error description\", \"AccessDeniedDetail\":{}}";
          res.writeHead(400, {
            ...headers,
          });
          break;
        case 'error1':
          responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"" +
            ", \"Description\":\"error description\", \"AccessDeniedDetail\":{}, \"accessDeniedDetail\":{\"test\": 0}}";
          res.writeHead(400, {
            ...headers,
          });
          break;
        case 'error2':
          responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"" +
            ", \"Description\":\"error description\", \"accessDeniedDetail\":{\"test\": 0}}";
          res.writeHead(400, {
            ...headers,
          });
          break;
        default:
          res.writeHead(200, {
            ...headers,
          });
      }
      res.end(responseBody);
    });
  }
});

class BytesReadable extends Readable {
  value: Buffer

  constructor(value: string | Buffer) {
    super();
    if (typeof value === 'string') {
      this.value = Buffer.from(value);
    } else if (Buffer.isBuffer(value)) {
      this.value = value;
    }
  }

  _read() {
    this.push(this.value);
    this.push(null);
  }
}

function createConfig(): $OpenApiUtil.Config {
  let globalParameters = new $OpenApiUtil.GlobalParameters({
    headers: {
      'global-key': "global-value",
    },
    queries: {
      'global-query': "global-value",
    },
  });
  let config = new $OpenApiUtil.Config({
    accessKeyId: "ak",
    accessKeySecret: "secret",
    securityToken: "token",
    type: "sts",
    userAgent: "config.userAgent",
    readTimeout: 3000,
    connectTimeout: 3000,
    maxIdleConns: 128,
    signatureVersion: "config.signatureVersion",
    signatureAlgorithm: "ACS3-HMAC-SHA256",
    globalParameters: globalParameters,
  });
  return config;
}

function createBearerTokenConfig(): $OpenApiUtil.Config {
  let creConfig = new $Credential.Config({
    bearerToken: "token",
    type: "bearer",
  });
  let credential = new Credential(creConfig);
  let config = new $OpenApiUtil.Config({
    credential: credential,
  });
  return config;
}

function createAnonymousConfig(): $OpenApiUtil.Config {
  return new $OpenApiUtil.Config();
}

function createRuntimeOptions(): $dara.RuntimeOptions {
  let extendsParameters = new $dara.ExtendsParameters({
    headers: {
      'extends-key': "extends-value",
    },
  });
  let runtime = new $dara.RuntimeOptions({
    readTimeout: 4000,
    connectTimeout: 4000,
    maxIdleConns: 100,
    autoretry: true,
    maxAttempts: 1,
    backoffPolicy: "no",
    backoffPeriod: 1,
    ignoreSSL: true,
    extendsParameters: extendsParameters,
  });
  return runtime;
}

function createOpenApiRequest(): $OpenApiUtil.OpenApiRequest {
  let query: { [key: string]: any } = {};
  query["key1"] = "value";
  query["key2"] = 1;
  query["key3"] = true;
  let body: { [key: string]: any } = {};
  body["key1"] = "value";
  body["key2"] = 1;
  body["key3"] = true;
  let headers: { [key: string]: string } = {
    'for-test': "sdk",
  };
  let req = new $OpenApiUtil.OpenApiRequest({
    headers: headers,
    query: OpenApiUtil.query(query),
    body: OpenApiUtil.parseToMap(body),
  });
  return req;
}

describe('$openapi', function () {

  before((done) => {
    server.listen(0, done);
  });

  after(function (done) {
    this.timeout(10000);
    server.close(done);
  });

  it('config should ok', async function () {
    let globalParameters = new $OpenApiUtil.GlobalParameters({
      headers: {
        'global-key': "global-value",
      },
      queries: {
        'global-query': "global-value",
      },
    });
    let config = new $OpenApiUtil.Config({
      endpoint: "config.endpoint",
      endpointType: "public",
      network: "config.network",
      suffix: "config.suffix",
      protocol: "config.protocol",
      method: "config.method",
      regionId: "config.regionId",
      userAgent: "config.userAgent",
      readTimeout: 3000,
      connectTimeout: 3000,
      httpProxy: "config.httpProxy",
      httpsProxy: "config.httpsProxy",
      noProxy: "config.noProxy",
      socks5Proxy: "config.socks5Proxy",
      socks5NetWork: "config.socks5NetWork",
      maxIdleConns: 128,
      signatureVersion: "config.signatureVersion",
      signatureAlgorithm: "config.signatureAlgorithm",
      globalParameters: globalParameters,
      key: "config.key",
      cert: "config.cert",
      ca: "config.ca"
    });
    let creConfig = new $Credential.Config({
      accessKeyId: "accessKeyId",
      accessKeySecret: "accessKeySecret",
      securityToken: "securityToken",
      type: "sts",
    });
    let credential = new Credential(creConfig);
    config.credential = credential;
    let client = new OpenApi(config);
    assert.strictEqual(await client.getAccessKeyId(), 'accessKeyId');
    assert.strictEqual(await client.getAccessKeySecret(), 'accessKeySecret');
    assert.strictEqual(await client.getSecurityToken(), 'securityToken');
    assert.strictEqual(await client.getType(), 'sts');

    creConfig = new $Credential.Config({
      bearerToken: "token",
      type: "bearer",
    });
    credential = new Credential(creConfig);
    config.credential = credential;
    client = new OpenApi(config);
    assert.strictEqual(await client.getAccessKeyId(), '');
    assert.strictEqual(await client.getAccessKeySecret(), '');
    assert.strictEqual(await client.getSecurityToken(), '');
    assert.strictEqual(await client.getBearerToken(), 'token');
    assert.strictEqual(await client.getType(), 'bearer');

    config.accessKeyId = "ak";
    config.accessKeySecret = "secret";
    config.securityToken = "token";
    config.type = "sts";
    client = new OpenApi(config);
    assert.strictEqual(await client.getAccessKeyId(), 'ak');
    assert.strictEqual(await client.getAccessKeySecret(), 'secret');
    assert.strictEqual(await client.getSecurityToken(), 'token');
    assert.strictEqual(await client.getType(), 'sts');

    config.bearerToken = "token";
    config.accessKeyId = undefined;
    config.accessKeySecret = undefined;
    config.securityToken = undefined;
    config.type = "bearer";
    client = new OpenApi(config);
    assert.strictEqual(await client.getAccessKeyId(), '');
    assert.strictEqual(await client.getAccessKeySecret(), '');
    assert.strictEqual(await client.getSecurityToken(), '');
    assert.strictEqual(await client.getBearerToken(), 'token');
    assert.strictEqual(await client.getType(), 'bearer');

    config.bearerToken = undefined;
    config.accessKeyId = "ak";
    config.accessKeySecret = "secret";
    config.securityToken = undefined;
    config.type = "access_key";
    client = new OpenApi(config);
    assert.strictEqual(await client.getAccessKeyId(), 'ak');
    assert.strictEqual(await client.getAccessKeySecret(), 'secret');
    assert.strictEqual(await client.getSecurityToken(), undefined);
    assert.strictEqual(await client.getBearerToken(), undefined);
    assert.strictEqual(await client.getType(), 'access_key');

    assert.strictEqual(client._spi, undefined);
    assert.strictEqual(client._endpointRule, undefined);
    assert.strictEqual(client._endpointMap, undefined);
    assert.strictEqual(client._productId, undefined);
    assert.strictEqual(client._endpoint, 'config.endpoint');
    assert.strictEqual(client._endpointType, 'public');
    assert.strictEqual(client._network, 'config.network');
    assert.strictEqual(client._suffix, 'config.suffix');
    assert.strictEqual(client._protocol, 'config.protocol');
    assert.strictEqual(client._method, 'config.method');
    assert.strictEqual(client._regionId, 'config.regionId');
    assert.strictEqual(client._userAgent, 'config.userAgent');
    assert.strictEqual(client._readTimeout, 3000);
    assert.strictEqual(client._connectTimeout, 3000);
    assert.strictEqual(client._httpProxy, 'config.httpProxy');
    assert.strictEqual(client._httpsProxy, 'config.httpsProxy');
    assert.strictEqual(client._noProxy, 'config.noProxy');
    assert.strictEqual(client._socks5Proxy, 'config.socks5Proxy');
    assert.strictEqual(client._socks5NetWork, 'config.socks5NetWork');
    assert.strictEqual(client._maxIdleConns, 128);
    assert.strictEqual(client._signatureVersion, 'config.signatureVersion');
    assert.strictEqual(client._signatureAlgorithm, 'config.signatureAlgorithm');
    assert.strictEqual(client._globalParameters.toMap().headers['global-key'], 'global-value');
    assert.strictEqual(client._globalParameters.toMap().queries['global-query'], 'global-value');
    assert.strictEqual(client._key, 'config.key');
    assert.strictEqual(client._cert, 'config.cert');
    assert.strictEqual(client._ca, 'config.ca');
  });

  it("call api for RPC With V2Sign AK Form should ok", async function () {
    let config = createConfig();
    let runtime = createRuntimeOptions();
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    let port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    let client = new OpenApi(config);
    let request = createOpenApiRequest();
    let params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/",
      method: "POST",
      authType: "AK",
      style: "RPC",
      reqBodyType: "formData",
      bodyType: "json",
    });
    let result = await client.callApi(params, request, runtime);
    let headers = result["headers"];
    assert.strictEqual(headers["raw-body"], "key1=value&key2=1&key3=true");
    let regexp = /Action=TestAPI&Format=json&Version=2022-06-01&Timestamp=.+&SignatureNonce=.+&global-query=global-value&key1=value&key2=1&key3=true&SecurityToken=token&SignatureMethod=HMAC-SHA1&SignatureVersion=1\.0&AccessKeyId=ak&Signature=.+/;
    assert.ok(regexp.test(headers["raw-query"]));
    assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/2 config.userAgent"));
    assert.strictEqual(headers["global-key"], "global-value");
    assert.strictEqual(headers["extends-key"], "extends-value");
    assert.strictEqual(headers["x-acs-version"], "2022-06-01");
    assert.strictEqual(headers["x-acs-action"], "TestAPI");
    assert.strictEqual(headers["content-type"], "application/x-www-form-urlencoded");
    assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
    assert.strictEqual(headers["http-method"], "POST");
    assert.strictEqual(headers["pathname"], "/");
    assert.strictEqual(headers["connection"], "keep-alive");

    let body = result["body"];
    assert.strictEqual(body["AppId"], "test");
    assert.strictEqual(body["ClassId"], "test");
    assert.strictEqual(body["UserId"], 123);
    assert.strictEqual(result["statusCode"], 200);

    // bearer token
    config = createBearerTokenConfig();
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    config.endpoint = `127.0.0.1:${port}`;
    client = new OpenApi(config);
    result = await client.callApi(params, request, runtime);
    headers = result["headers"];
    regexp = /Action=TestAPI&Format=json&Version=2022-06-01&Timestamp=.+&SignatureNonce=.+&key1=value&key2=1&key3=true&BearerToken=token&SignatureType=BEARERTOKEN/;
    assert.ok(regexp.test(headers["raw-query"]));
    // assert.strictEqual(headers["authorization"], "bearer token");

    // Anonymous error
    config = createAnonymousConfig();
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    config.endpoint = `127.0.0.1:${port}`;
    client = new OpenApi(config);
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "InvalidCredentials: Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.");
      assert.strictEqual(err.code, "InvalidCredentials");
    }
  });

  it("call api for RPC With V2Sign Anonymous JSON should ok", async function () {
    let config = createConfig();
    let runtime = createRuntimeOptions();
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    let port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    let client = new OpenApi(config);
    let request = createOpenApiRequest();
    let params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/",
      method: "POST",
      authType: "Anonymous",
      style: "RPC",
      reqBodyType: "json",
      bodyType: "json",
    });
    let result = await client.callApi(params, request, runtime);
    let headers = result["headers"];
    assert.strictEqual(headers["raw-body"], "key1=value&key2=1&key3=true");
    let regexp = /Action=TestAPI&Format=json&Version=2022-06-01&Timestamp=.+&SignatureNonce=.+&global-query=global-value&key1=value&key2=1&key3=true/;
    assert.ok(regexp.test(headers["raw-query"]));
    assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/2 config.userAgent"));
    assert.strictEqual(headers["global-key"], "global-value");
    assert.strictEqual(headers["extends-key"], "extends-value");
    assert.strictEqual(headers["x-acs-version"], "2022-06-01");
    assert.strictEqual(headers["x-acs-action"], "TestAPI");
    assert.strictEqual(headers["content-type"], "application/x-www-form-urlencoded");
    assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
    assert.strictEqual(headers["http-method"], "POST");
    assert.strictEqual(headers["pathname"], "/");
    assert.strictEqual(headers["connection"], "keep-alive");

    let body = result["body"];
    assert.strictEqual(body["AppId"], "test");
    assert.strictEqual(body["ClassId"], "test");
    assert.strictEqual(body["UserId"], 123);
    assert.strictEqual(result["statusCode"], 200);
  });

  it("call api for ROA With V2Sign AK Form should ok", async function () {
    let config = createConfig();
    let runtime = createRuntimeOptions();
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    let port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    let client = new OpenApi(config);
    let request = createOpenApiRequest();
    let params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/test",
      method: "POST",
      authType: "AK",
      style: "ROA",
      reqBodyType: "formData",
      bodyType: "json",
    });
    let result = await client.callApi(params, request, runtime);
    let headers = result["headers"];
    assert.strictEqual(headers["raw-body"], "key1=value&key2=1&key3=true");
    assert.strictEqual(headers["raw-query"], "global-query=global-value&key1=value&key2=1&key3=true");
    assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/2 config.userAgent"));
    assert.strictEqual(headers["x-acs-version"], "2022-06-01");
    assert.strictEqual(headers["x-acs-action"], "TestAPI");
    assert.strictEqual(headers["content-type"], "application/x-www-form-urlencoded");
    assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
    assert.strictEqual(headers["http-method"], "POST");
    assert.strictEqual(headers["pathname"], "/test");
    assert.strictEqual(headers["connection"], "keep-alive");
    assert.strictEqual(headers["for-test"], "sdk");
    assert.strictEqual(headers["global-key"], "global-value");
    assert.strictEqual(headers["extends-key"], "extends-value");
    assert.ok(headers["x-acs-signature-nonce"].length > 0);
    assert.ok(headers["date"].length > 0);
    assert.strictEqual(headers["accept"], "application/json");
    assert.strictEqual(headers["x-acs-signature-method"], "HMAC-SHA1");
    assert.strictEqual(headers["x-acs-signature-version"], "1.0");
    assert.strictEqual(headers["x-acs-accesskey-id"], "ak");
    assert.strictEqual(headers["x-acs-security-token"], "token");
    assert.ok(String(headers["authorization"]).startsWith("acs ak:"));

    let body = result["body"];
    assert.strictEqual(body["AppId"], "test");
    assert.strictEqual(body["ClassId"], "test");
    assert.strictEqual(body["UserId"], 123);
    assert.strictEqual(result["statusCode"], 200);

    // bearer token
    config = createBearerTokenConfig();
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    config.endpoint = `127.0.0.1:${port}`;
    client = new OpenApi(config);
    result = await client.callApi(params, request, runtime);
    headers = result["headers"];
    assert.strictEqual(headers["x-acs-version"], "2022-06-01");
    assert.strictEqual(headers["x-acs-action"], "TestAPI");
    assert.strictEqual(headers["content-type"], "application/x-www-form-urlencoded");
    assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
    assert.strictEqual(headers["accept"], "application/json");
    assert.strictEqual(headers["x-acs-bearer-token"], "token");
    assert.strictEqual(headers["x-acs-signature-type"], "BEARERTOKEN");
    // assert.strictEqual(headers["authorization"], "bearer token");

    // Anonymous error
    config = createAnonymousConfig();
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    config.endpoint = `127.0.0.1:${port}`;
    client = new OpenApi(config);
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "InvalidCredentials: Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.");
      assert.strictEqual(err.code, "InvalidCredentials");
    }

    config = createBearerTokenConfig();
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    config.endpoint = `127.0.0.1:${port}`;
    client = new OpenApi(config);
    params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/test",
      method: "POST",
      authType: "AK",
      style: "ROA",
      reqBodyType: "json",
      bodyType: "json",
    });
    result = await client.callApi(params, request, runtime);
    headers = result["headers"];
    assert.strictEqual(headers["x-acs-version"], "2022-06-01");
    assert.strictEqual(headers["x-acs-action"], "TestAPI");
    assert.strictEqual(headers["content-type"], "application/json; charset=utf-8");
    assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
    assert.strictEqual(headers["accept"], "application/json");
    assert.strictEqual(headers["x-acs-bearer-token"], "token");
    assert.strictEqual(headers["x-acs-signature-type"], "BEARERTOKEN");
    // assert.strictEqual(headers["authorization"], "bearer token");

    // Anonymous error
    config = createAnonymousConfig();
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    config.endpoint = `127.0.0.1:${port}`;
    client = new OpenApi(config);
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "InvalidCredentials: Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.");
      assert.strictEqual(err.code, "InvalidCredentials");
    }
  });

  it("call api for ROA With V2Sign Anonymous JSON should ok", async function () {
    let config = createConfig();
    let runtime = createRuntimeOptions();
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    let port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    let client = new OpenApi(config);
    let request = createOpenApiRequest();
    let params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/test",
      method: "POST",
      authType: "Anonymous",
      style: "ROA",
      reqBodyType: "json",
      bodyType: "json",
    });
    let result = await client.callApi(params, request, runtime);
    let headers = result["headers"];
    assert.strictEqual(headers["raw-body"], "{\"key1\":\"value\",\"key2\":1,\"key3\":true}");
    assert.strictEqual(headers["raw-query"], "global-query=global-value&key1=value&key2=1&key3=true");
    assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/2 config.userAgent"));
    assert.strictEqual(headers["x-acs-version"], "2022-06-01");
    assert.strictEqual(headers["x-acs-action"], "TestAPI");
    assert.strictEqual(headers["content-type"], "application/json; charset=utf-8");
    assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
    assert.strictEqual(headers["http-method"], "POST");
    assert.strictEqual(headers["pathname"], "/test");
    assert.strictEqual(headers["connection"], "keep-alive");
    assert.strictEqual(headers["for-test"], "sdk");
    assert.strictEqual(headers["global-key"], "global-value");
    assert.strictEqual(headers["extends-key"], "extends-value");
    assert.ok(headers["x-acs-signature-nonce"].length > 0);
    assert.ok(headers["date"].length > 0);
    assert.strictEqual(headers["accept"], "application/json");
    assert.strictEqual(headers["x-acs-signature-method"], "HMAC-SHA1");
    assert.strictEqual(headers["x-acs-signature-version"], "1.0");

    let body = result["body"];
    assert.strictEqual(body["AppId"], "test");
    assert.strictEqual(body["ClassId"], "test");
    assert.strictEqual(body["UserId"], 123);
    assert.strictEqual(result["statusCode"], 200);
  });

  it("call api for RPC With V3Sign AK Form should ok", async function () {
    let config = createConfig();
    let runtime = createRuntimeOptions();
    config.protocol = "HTTP";
    let port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    let client = new OpenApi(config);
    let request = createOpenApiRequest();
    let params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/",
      method: "POST",
      authType: "AK",
      style: "RPC",
      reqBodyType: "formData",
      bodyType: "json",
    });
    let result = await client.callApi(params, request, runtime);
    let headers = result["headers"];
    assert.strictEqual(headers["raw-body"], "key1=value&key2=1&key3=true");
    assert.strictEqual(headers["raw-query"], "global-query=global-value&key1=value&key2=1&key3=true");
    assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/2 config.userAgent"));
    assert.strictEqual(headers["x-acs-version"], "2022-06-01");
    assert.strictEqual(headers["x-acs-action"], "TestAPI");
    assert.strictEqual(headers["content-type"], "application/x-www-form-urlencoded");
    assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
    assert.strictEqual(headers["http-method"], "POST");
    assert.strictEqual(headers["pathname"], "/");
    assert.strictEqual(headers["connection"], "keep-alive");
    assert.strictEqual(headers["for-test"], "sdk");
    assert.strictEqual(headers["global-key"], "global-value");
    assert.strictEqual(headers["extends-key"], "extends-value");
    assert.ok(headers["x-acs-signature-nonce"].length > 0);
    assert.ok(headers["x-acs-date"].length > 0);
    assert.ok(headers["x-acs-content-sha256"].length > 0);
    assert.strictEqual(headers["accept"], "application/json");
    assert.strictEqual(headers["x-acs-accesskey-id"], "ak");
    assert.strictEqual(headers["x-acs-security-token"], "token");
    assert.ok(String(headers["authorization"]).startsWith("ACS3-HMAC-SHA256 Credential=ak," +
      "SignedHeaders=content-type;host;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-credentials-provider;x-acs-date;x-acs-security-token;" +
      "x-acs-signature-nonce;x-acs-version,Signature="));

    let body = result["body"];
    assert.strictEqual(body["AppId"], "test");
    assert.strictEqual(body["ClassId"], "test");
    assert.strictEqual(body["UserId"], 123);
    assert.strictEqual(result["statusCode"], 200);

    // bearer token
    config = createBearerTokenConfig();
    config.protocol = "HTTP";
    config.endpoint = `127.0.0.1:${port}`;
    client = new OpenApi(config);
    result = await client.callApi(params, request, runtime);
    headers = result["headers"];
    assert.strictEqual(headers["x-acs-version"], "2022-06-01");
    assert.strictEqual(headers["x-acs-action"], "TestAPI");
    assert.strictEqual(headers["content-type"], "application/x-www-form-urlencoded");
    assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
    assert.strictEqual(headers["accept"], "application/json");
    assert.strictEqual(headers["x-acs-bearer-token"], "token");
    const regexp = /SignatureType=BEARERTOKEN/;
    assert.ok(regexp.test(headers["raw-query"]));
    // assert.strictEqual(headers["authorization"], "bearer token");

    // Anonymous error
    config = createAnonymousConfig();
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    config.endpoint = `127.0.0.1:${port}`;
    client = new OpenApi(config);
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "InvalidCredentials: Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.");
      assert.strictEqual(err.code, "InvalidCredentials");
    }
  });

  it("call api for RPC With V3Sign Anonymous JSON should ok", async function () {
    let config = createConfig();
    let runtime = createRuntimeOptions();
    config.protocol = "HTTP";
    let port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    let client = new OpenApi(config);
    let request = createOpenApiRequest();
    let params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/",
      method: "POST",
      authType: "Anonymous",
      style: "RPC",
      reqBodyType: "json",
      bodyType: "json",
    });
    let result = await client.callApi(params, request, runtime);
    let headers = result["headers"];
    assert.strictEqual(headers["raw-body"], "{\"key1\":\"value\",\"key2\":1,\"key3\":true}");
    assert.strictEqual(headers["raw-query"], "global-query=global-value&key1=value&key2=1&key3=true");
    assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/2 config.userAgent"));
    assert.strictEqual(headers["x-acs-version"], "2022-06-01");
    assert.strictEqual(headers["x-acs-action"], "TestAPI");
    assert.strictEqual(headers["content-type"], "application/json; charset=utf-8");
    assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
    assert.strictEqual(headers["http-method"], "POST");
    assert.strictEqual(headers["pathname"], "/");
    assert.strictEqual(headers["connection"], "keep-alive");
    assert.strictEqual(headers["for-test"], "sdk");
    assert.strictEqual(headers["global-key"], "global-value");
    assert.strictEqual(headers["extends-key"], "extends-value");
    assert.ok(headers["x-acs-signature-nonce"].length > 0);
    assert.ok(headers["x-acs-date"].length > 0);
    assert.strictEqual(headers["accept"], "application/json");

    let body = result["body"];
    assert.strictEqual(body["AppId"], "test");
    assert.strictEqual(body["ClassId"], "test");
    assert.strictEqual(body["UserId"], 123);
    assert.strictEqual(result["statusCode"], 200);
  });

  it("call api for ROA With V3Sign AK Form should ok", async function () {
    let config = createConfig();
    let runtime = createRuntimeOptions();
    config.protocol = "HTTP";
    let port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    let client = new OpenApi(config);
    let request = createOpenApiRequest();
    let params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/test",
      method: "POST",
      authType: "AK",
      style: "ROA",
      reqBodyType: "formData",
      bodyType: "json",
    });
    let result = await client.callApi(params, request, runtime);
    let headers = result["headers"];
    assert.strictEqual(headers["raw-body"], "key1=value&key2=1&key3=true");
    assert.strictEqual(headers["raw-query"], "global-query=global-value&key1=value&key2=1&key3=true");
    assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/2 config.userAgent"));
    assert.strictEqual(headers["x-acs-version"], "2022-06-01");
    assert.strictEqual(headers["x-acs-action"], "TestAPI");
    assert.strictEqual(headers["content-type"], "application/x-www-form-urlencoded");
    assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
    assert.strictEqual(headers["http-method"], "POST");
    assert.strictEqual(headers["pathname"], "/test");
    assert.strictEqual(headers["connection"], "keep-alive");
    assert.strictEqual(headers["for-test"], "sdk");
    assert.strictEqual(headers["global-key"], "global-value");
    assert.strictEqual(headers["extends-key"], "extends-value");
    assert.ok(headers["x-acs-signature-nonce"].length > 0);
    assert.ok(headers["x-acs-date"].length > 0);
    assert.ok(headers["x-acs-content-sha256"].length > 0);
    assert.strictEqual(headers["accept"], "application/json");
    assert.strictEqual(headers["x-acs-accesskey-id"], "ak");
    assert.strictEqual(headers["x-acs-security-token"], "token");
    assert.ok(String(headers["authorization"]).startsWith("ACS3-HMAC-SHA256 Credential=ak," +
      "SignedHeaders=content-type;host;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-credentials-provider;x-acs-date;x-acs-security-token;" +
      "x-acs-signature-nonce;x-acs-version,Signature="));

    let body = result["body"];
    assert.strictEqual(body["AppId"], "test");
    assert.strictEqual(body["ClassId"], "test");
    assert.strictEqual(body["UserId"], 123);
    assert.strictEqual(result["statusCode"], 200);

    // bearer token
    config = createBearerTokenConfig();
    config.protocol = "HTTP";
    config.endpoint = `127.0.0.1:${port}`;
    client = new OpenApi(config);
    result = await client.callApi(params, request, runtime);
    headers = result["headers"];
    assert.strictEqual(headers["x-acs-version"], "2022-06-01");
    assert.strictEqual(headers["x-acs-action"], "TestAPI");
    assert.strictEqual(headers["content-type"], "application/x-www-form-urlencoded");
    assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
    assert.strictEqual(headers["accept"], "application/json");
    assert.strictEqual(headers["x-acs-bearer-token"], "token");
    assert.strictEqual(headers["x-acs-signature-type"], "BEARERTOKEN");
    // assert.strictEqual(headers["authorization"], "bearer token");

    // Anonymous error
    config = createAnonymousConfig();
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    config.endpoint = `127.0.0.1:${port}`;
    client = new OpenApi(config);
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "InvalidCredentials: Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.");
      assert.strictEqual(err.code, "InvalidCredentials");
    }
  });

  it("call api for ROA With V3Sign Anonymous JSON should ok", async function () {
    let config = createConfig();
    let runtime = createRuntimeOptions();
    config.protocol = "HTTP";
    let port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    let client = new OpenApi(config);
    let request = createOpenApiRequest();
    let params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/test",
      method: "POST",
      authType: "Anonymous",
      style: "ROA",
      reqBodyType: "json",
      bodyType: "json",
    });
    let result = await client.callApi(params, request, runtime);
    let headers = result["headers"];
    assert.strictEqual(headers["raw-body"], "{\"key1\":\"value\",\"key2\":1,\"key3\":true}");
    assert.strictEqual(headers["raw-query"], "global-query=global-value&key1=value&key2=1&key3=true");
    assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/2 config.userAgent"));
    assert.strictEqual(headers["x-acs-version"], "2022-06-01");
    assert.strictEqual(headers["x-acs-action"], "TestAPI");
    assert.strictEqual(headers["content-type"], "application/json; charset=utf-8");
    assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
    assert.strictEqual(headers["http-method"], "POST");
    assert.strictEqual(headers["pathname"], "/test");
    assert.strictEqual(headers["connection"], "keep-alive");
    assert.strictEqual(headers["for-test"], "sdk");
    assert.strictEqual(headers["global-key"], "global-value");
    assert.strictEqual(headers["extends-key"], "extends-value");
    assert.ok(headers["x-acs-signature-nonce"].length > 0);
    assert.ok(headers["x-acs-date"].length > 0);
    assert.strictEqual(headers["accept"], "application/json");

    let body = result["body"];
    assert.strictEqual(body["AppId"], "test");
    assert.strictEqual(body["ClassId"], "test");
    assert.strictEqual(body["UserId"], 123);
    assert.strictEqual(result["statusCode"], 200);
  });

  it("call api for SSE With V3Sign AK should ok", async function () {
    let config = createConfig();
    let runtime = createRuntimeOptions();
    config.protocol = "HTTP";
    let port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    let client = new OpenApi(config);
    let request = createOpenApiRequest();
    let params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/sse",
      method: "POST",
      authType: "AK",
      style: "ROA",
      reqBodyType: "json",
      bodyType: "sse",
    });
    let result = client.callSSEApi(params, request, runtime);
    const events: $dara.SSEEvent[] = [];
    for await (const event of result) {
      let headers = event["headers"];
      assert.strictEqual(headers["raw-body"], "{\"key1\":\"value\",\"key2\":1,\"key3\":true}");
      assert.strictEqual(headers["raw-query"], "global-query=global-value&key1=value&key2=1&key3=true");
      assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/2 config.userAgent"));
      assert.strictEqual(headers["x-acs-version"], "2022-06-01");
      assert.strictEqual(headers["x-acs-action"], "TestAPI");
      assert.strictEqual(headers["content-type"], "application/json; charset=utf-8");
      assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
      assert.strictEqual(headers["http-method"], "POST");
      assert.strictEqual(headers["pathname"], "/sse");
      assert.strictEqual(headers["connection"], "keep-alive");
      assert.strictEqual(headers["for-test"], "sdk");
      assert.strictEqual(headers["global-key"], "global-value");
      assert.strictEqual(headers["extends-key"], "extends-value");
      assert.ok(headers["x-acs-signature-nonce"].length > 0);
      assert.ok(headers["x-acs-date"].length > 0);
      assert.strictEqual(headers["accept"], "application/json");
      events.push(event.event);
    }
    assert.strictEqual(events.length, 5);

    assert.deepStrictEqual([new $dara.SSEEvent({
      data: '{"count":0}',
      event: 'flow',
      id: 'sse-test',
      retry: 3,
    }), new $dara.SSEEvent({
      data: '{"count":1}',
      event: 'flow',
      id: 'sse-test',
      retry: 3,
    }), new $dara.SSEEvent({
      data: '{"count":2}',
      event: 'flow',
      id: 'sse-test',
      retry: 3,
    }), new $dara.SSEEvent({
      data: '{"count":3}',
      event: 'flow',
      id: 'sse-test',
      retry: 3,
    }), new $dara.SSEEvent({
      data: '{"count":4}',
      event: 'flow',
      id: 'sse-test',
      retry: 3,
    })], events);
  });

  it("response body should ok", async function () {
    let config = createConfig();
    let runtime = createRuntimeOptions();
    let extendsParameters = new $dara.ExtendsParameters();
    extendsParameters.headers = runtime.extendsParameters?.headers || {};
    extendsParameters.headers['bodyType'] = 'json';
    runtime.extendsParameters = extendsParameters;
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    let port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    let client = new OpenApi(config);
    let request = createOpenApiRequest();
    let params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/test",
      method: "POST",
      authType: "AK",
      style: "ROA",
      reqBodyType: "formData",
      bodyType: "json",
    });
    let result = await client.callApi(params, request, runtime);
    let body = result["body"];
    assert.strictEqual(body["AppId"], "test");
    assert.strictEqual(body["ClassId"], "test");
    assert.strictEqual(body["UserId"], 123);
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "array"
    };
    extendsParameters.headers['bodyType'] = 'array';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "array";
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.strictEqual(body[0], "AppId");
    assert.strictEqual(body[1], "ClassId");
    assert.strictEqual(body[2], "UserId");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "string"
    };
    extendsParameters.headers['bodyType'] = 'string';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "string";
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.strictEqual(body, "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "byte"
    };
    extendsParameters.headers['bodyType'] = 'byte';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "byte";
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.ok(body instanceof Buffer);
    assert.strictEqual(body.toString(), "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "binary"
    };
    extendsParameters.headers['bodyType'] = 'binary';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "binary";
    result = await client.callApi(params, request, runtime);
    body = await $dara.Stream.readAsBytes(result["body"]);
    assert.ok(body instanceof Buffer);
    assert.strictEqual(body.toString(), "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "error"
    };
    extendsParameters.headers['bodyType'] = 'error';
    runtime.extendsParameters = extendsParameters;
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "error code: code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4");
      assert.strictEqual(err.statusCode, 400);
      assert.strictEqual(err.accessDeniedDetail["test"], undefined);
    }

    request.headers = {
      ...request.headers,
      bodyType: "error1"
    };
    extendsParameters.headers['bodyType'] = 'error1';
    runtime.extendsParameters = extendsParameters;
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "error code: code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4");
      assert.strictEqual(err.statusCode, 400);
      assert.strictEqual(err.accessDeniedDetail["test"], undefined);
    }

    request.headers = {
      ...request.headers,
      bodyType: "error2"
    };
    extendsParameters.headers['bodyType'] = 'error2';
    runtime.extendsParameters = extendsParameters;
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "error code: code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4");
      assert.strictEqual(err.statusCode, 400);
      assert.strictEqual(err.accessDeniedDetail["test"], 0);
    }

    port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    client = new OpenApi(config);
    params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/test",
      method: "POST",
      authType: "AK",
      style: "ROA",
      reqBodyType: "json",
      bodyType: "json",
    });
    request.headers = {
      ...request.headers,
      bodyType: "json"
    };
    extendsParameters.headers['bodyType'] = 'json';
    runtime.extendsParameters = extendsParameters;
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.strictEqual(body["AppId"], "test");
    assert.strictEqual(body["ClassId"], "test");
    assert.strictEqual(body["UserId"], 123);
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "array"
    };
    extendsParameters.headers['bodyType'] = 'array';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "array";
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.strictEqual(body[0], "AppId");
    assert.strictEqual(body[1], "ClassId");
    assert.strictEqual(body[2], "UserId");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "string"
    };
    extendsParameters.headers['bodyType'] = 'string';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "string";
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.strictEqual(body, "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "byte"
    };
    extendsParameters.headers['bodyType'] = 'byte';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "byte";
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.ok(body instanceof Buffer);
    assert.strictEqual(body.toString(), "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "binary"
    };
    extendsParameters.headers['bodyType'] = 'binary';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "binary";
    result = await client.callApi(params, request, runtime);
    body = await $dara.Stream.readAsBytes(result["body"]);
    assert.ok(body instanceof Buffer);
    assert.strictEqual(body.toString(), "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "error"
    };
    extendsParameters.headers['bodyType'] = 'error';
    runtime.extendsParameters = extendsParameters;
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "error code: code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4");
      assert.strictEqual(err.statusCode, 400);
      assert.strictEqual(err.accessDeniedDetail["test"], undefined);
    }

    request.headers = {
      ...request.headers,
      bodyType: "error1"
    };
    extendsParameters.headers['bodyType'] = 'error1';
    runtime.extendsParameters = extendsParameters;
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "error code: code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4");
      assert.strictEqual(err.statusCode, 400);
      assert.strictEqual(err.accessDeniedDetail["test"], undefined);
    }

    request.headers = {
      ...request.headers,
      bodyType: "error2"
    };
    extendsParameters.headers['bodyType'] = 'error2';
    runtime.extendsParameters = extendsParameters;
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "error code: code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4");
      assert.strictEqual(err.statusCode, 400);
      assert.strictEqual(err.accessDeniedDetail["test"], 0);
    }

    port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    client = new OpenApi(config);
    params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/test",
      method: "POST",
      authType: "AK",
      style: "RPC",
      reqBodyType: "json",
      bodyType: "json",
    });
    request.headers = {
      ...request.headers,
      bodyType: "json"
    };
    extendsParameters.headers['bodyType'] = 'json';
    runtime.extendsParameters = extendsParameters;
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.strictEqual(body["AppId"], "test");
    assert.strictEqual(body["ClassId"], "test");
    assert.strictEqual(body["UserId"], 123);
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "array"
    };
    extendsParameters.headers['bodyType'] = 'array';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "array";
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.strictEqual(body[0], "AppId");
    assert.strictEqual(body[1], "ClassId");
    assert.strictEqual(body[2], "UserId");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "string"
    };
    extendsParameters.headers['bodyType'] = 'string';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "string";
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.strictEqual(body, "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "byte"
    };
    extendsParameters.headers['bodyType'] = 'byte';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "byte";
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.ok(body instanceof Buffer);
    assert.strictEqual(body.toString(), "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "binary"
    };
    extendsParameters.headers['bodyType'] = 'binary';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "binary";
    result = await client.callApi(params, request, runtime);
    body = await $dara.Stream.readAsBytes(result["body"]);
    assert.ok(body instanceof Buffer);
    assert.strictEqual(body.toString(), "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "error"
    };
    extendsParameters.headers['bodyType'] = 'error';
    runtime.extendsParameters = extendsParameters;
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "error code: code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4");
      assert.strictEqual(err.statusCode, 400);
      assert.strictEqual(err.accessDeniedDetail["test"], undefined);
    }

    request.headers = {
      ...request.headers,
      bodyType: "error1"
    };
    extendsParameters.headers['bodyType'] = 'error1';
    runtime.extendsParameters = extendsParameters;
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "error code: code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4");
      assert.strictEqual(err.statusCode, 400);
      assert.strictEqual(err.accessDeniedDetail["test"], undefined);
    }

    request.headers = {
      ...request.headers,
      bodyType: "error2"
    };
    extendsParameters.headers['bodyType'] = 'error2';
    runtime.extendsParameters = extendsParameters;
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "error code: code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4");
      assert.strictEqual(err.statusCode, 400);
      assert.strictEqual(err.accessDeniedDetail["test"], 0);
    }

    port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    config.signatureAlgorithm = undefined;
    client = new OpenApi(config);
    params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/test",
      method: "POST",
      authType: "AK",
      style: "RPC",
      reqBodyType: "json",
      bodyType: "json",
    });
    request.headers = {
      ...request.headers,
      bodyType: "json"
    };
    extendsParameters.headers['bodyType'] = 'json';
    runtime.extendsParameters = extendsParameters;
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.strictEqual(body["AppId"], "test");
    assert.strictEqual(body["ClassId"], "test");
    assert.strictEqual(body["UserId"], 123);
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "array"
    };
    extendsParameters.headers['bodyType'] = 'array';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "array";
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.strictEqual(body[0], "AppId");
    assert.strictEqual(body[1], "ClassId");
    assert.strictEqual(body[2], "UserId");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "string"
    };
    extendsParameters.headers['bodyType'] = 'string';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "string";
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.strictEqual(body, "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "byte"
    };
    extendsParameters.headers['bodyType'] = 'byte';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "byte";
    result = await client.callApi(params, request, runtime);
    body = result["body"];
    assert.ok(body instanceof Buffer);
    assert.strictEqual(body.toString(), "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "binary"
    };
    extendsParameters.headers['bodyType'] = 'binary';
    runtime.extendsParameters = extendsParameters;
    params.bodyType = "binary";
    result = await client.callApi(params, request, runtime);
    body = await $dara.Stream.readAsBytes(result["body"]);
    assert.ok(body instanceof Buffer);
    assert.strictEqual(body.toString(), "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
    assert.strictEqual(result["statusCode"], 200);

    request.headers = {
      ...request.headers,
      bodyType: "error"
    };
    extendsParameters.headers['bodyType'] = 'error';
    runtime.extendsParameters = extendsParameters;
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "error code: code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4");
      assert.strictEqual(err.statusCode, 400);
      assert.strictEqual(err.accessDeniedDetail["test"], undefined);
    }

    request.headers = {
      ...request.headers,
      bodyType: "error1"
    };
    extendsParameters.headers['bodyType'] = 'error1';
    runtime.extendsParameters = extendsParameters;
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "error code: code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4");
      assert.strictEqual(err.statusCode, 400);
      assert.strictEqual(err.accessDeniedDetail["test"], undefined);
    }

    request.headers = {
      ...request.headers,
      bodyType: "error2"
    };
    extendsParameters.headers['bodyType'] = 'error2';
    runtime.extendsParameters = extendsParameters;
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.strictEqual(err.message, "error code: code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4");
      assert.strictEqual(err.statusCode, 400);
      assert.strictEqual(err.accessDeniedDetail["test"], 0);
    }

  });

  it("request body should ok", async function () {
    let config = createConfig();
    let runtime = createRuntimeOptions();
    config.protocol = "HTTP";
    let port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    let client = new OpenApi(config);
    // formData
    let params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/test",
      method: "POST",
      authType: "AK",
      style: "ROA",
      reqBodyType: "formData",
      bodyType: "json",
    });
    let body: { [key: string]: any } = {};
    body["key1"] = "value";
    body["key2"] = 1;
    body["key3"] = true;
    let request = new $OpenApiUtil.OpenApiRequest({
      body: OpenApiUtil.parseToMap(body),
    });
    let result = await client.callApi(params, request, runtime);
    let headers = result["headers"];
    assert.strictEqual(headers["raw-body"], "key1=value&key2=1&key3=true");
    assert.strictEqual(result["statusCode"], 200);

    // json
    params.reqBodyType = "json";
    result = await client.callApi(params, request, runtime);
    headers = result["headers"];
    assert.strictEqual(headers["raw-body"], "{\"key1\":\"value\",\"key2\":1,\"key3\":true}");
    assert.strictEqual(result["statusCode"], 200);

    // byte
    params.reqBodyType = "byte";
    let byteBody = Buffer.from("test byte");
    request = new $OpenApiUtil.OpenApiRequest({
      body: byteBody,
    });
    result = await client.callApi(params, request, runtime);
    headers = result["headers"];
    assert.strictEqual(headers["raw-body"], "test byte");
    assert.strictEqual(result["statusCode"], 200);

    // stream
    params.reqBodyType = "binary";
    request = new $OpenApiUtil.OpenApiRequest({
      stream: new BytesReadable(byteBody),
    });
    result = await client.callApi(params, request, runtime);
    headers = result["headers"];
    assert.strictEqual(headers["raw-body"], "test byte");
    assert.strictEqual(result["statusCode"], 200);
  });

  it("retry should ok", async function () {
    let config = createConfig();
    let runtime = createRuntimeOptions();
    runtime.autoretry = true;
    runtime.maxAttempts = 1;
    runtime.backoffPolicy = "fix";
    runtime.backoffPeriod = 1;
    runtime.connectTimeout = 100;
    runtime.readTimeout = 10;
    let extendsParameters = new $dara.ExtendsParameters();
    extendsParameters.headers = runtime.extendsParameters?.headers || {};
    extendsParameters.headers['timeout'] = 'true';
    runtime.extendsParameters = extendsParameters;
    config.protocol = "HTTP";
    config.signatureAlgorithm = "v2";
    let port = (server.address() as AddressInfo).port;
    config.endpoint = `127.0.0.1:${port}`;
    let client = new OpenApi(config);

    let request = createOpenApiRequest();
    let params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/timeout",
      method: "POST",
      authType: "AK",
      style: "RPC",
      reqBodyType: "formData",
      bodyType: "json",
    });
    try {
      await client.callApi(params, request, runtime);
      assert.fail();
    } catch (error) {
      assert.ok(error.message.indexOf('ReadTimeout(10)') !== -1);
    }

    params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/timeout",
      method: "POST",
      authType: "AK",
      style: "ROA",
      reqBodyType: "formData",
      bodyType: "json",
    });
    try {
      await client.callApi(params, request, runtime);
      assert.fail();
    } catch (error) {
      assert.ok(error.message.indexOf('ReadTimeout(10)') !== -1);
    }

    params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/timeout",
      method: "POST",
      authType: "AK",
      style: "ROA",
      reqBodyType: "json",
      bodyType: "json",
    });
    try {
      await client.callApi(params, request, runtime);
      assert.fail();
    } catch (error) {
      assert.ok(error.message.indexOf('ReadTimeout(10)') !== -1);
    }

    config.signatureAlgorithm = undefined;
    client = new OpenApi(config);
    params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/timeout",
      method: "POST",
      authType: "AK",
      style: "ROA",
      reqBodyType: "json",
      bodyType: "json",
    });
    try {
      await client.callApi(params, request, runtime);
      assert.fail();
    } catch (error) {
      assert.ok(error.message.indexOf('ReadTimeout(10)') !== -1);
    }

    client._productId = "test";
    client.setGatewayClient(new POP());
    params = new $OpenApiUtil.Params({
      action: "TestAPI",
      version: "2022-06-01",
      protocol: "HTTPS",
      pathname: "/timeout",
      method: "POST",
      authType: "AK",
      style: "ROA",
      reqBodyType: "json",
      bodyType: "json",
    });
    try {
      await client.execute(params, request, runtime);
      assert.fail();
    } catch (error) {
      assert.ok(error.message.indexOf('ReadTimeout(10)') !== -1);
    }

  });

});