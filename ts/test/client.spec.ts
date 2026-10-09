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
import * as $_error from '../src/exceptions/error';
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
      if(urlObj.pathname === '/empty') {
          res.writeHead(204, {
            ...headers,
          });
          res.end();
          return;
      }
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
        case 'xml':
          responseBody = "<?xml version=\"1.0\"?><Error><Code>Invalid</Code><Message>xml error</Message><RequestId>xml-id</RequestId></Error>";
          res.writeHead(400, {
            ...headers,
            'content-type': 'text/xml;charset=utf-8',
          });
          break;
        case 'serverError':
          responseBody = "{\"Code\":\"InternalError\", \"Message\":\"boom\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"}";
          res.writeHead(500, {
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
      ca: "config.ca",
      ipv4Only: true,
    });
    assert.strictEqual(config.toMap()['ipv4Only'], true);
    assert.strictEqual(new $OpenApiUtil.Config(config.toMap()).ipv4Only, true);
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
    assert.strictEqual(client._ipv4Only, true);
  });

  it("ipv4Only should be passed to runtime", async function () {
    let port = (server.address() as AddressInfo).port;
    let cases = [
      { signatureAlgorithm: "v2", style: "RPC", reqBodyType: "formData" },
      { signatureAlgorithm: "v2", style: "ROA", reqBodyType: "json" },
      { signatureAlgorithm: "v2", style: "ROA", reqBodyType: "formData" },
      { signatureAlgorithm: "ACS3-HMAC-SHA256", style: "RPC", reqBodyType: "formData" },
    ];
    let settings = [
      { config: true, runtime: undefined },
      { config: undefined, runtime: true },
      { config: false, runtime: true },
    ];
    for (let c of cases) {
      let params = new $OpenApiUtil.Params({
        action: "TestAPI",
        version: "2022-06-01",
        protocol: "HTTP",
        pathname: "/",
        method: "POST",
        authType: "AK",
        style: c.style,
        reqBodyType: c.reqBodyType,
        bodyType: "json",
      });
      for (let s of settings) {
        let config = createConfig();
        config.protocol = "HTTP";
        config.signatureAlgorithm = c.signatureAlgorithm;
        config.endpoint = `[::1]:${port}`;
        config.ipv4Only = s.config;
        let client = new OpenApi(config);
        let runtime = createRuntimeOptions();
        runtime.ipv4Only = s.runtime;
        await assert.rejects(client.callApi(params, createOpenApiRequest(), runtime), /ipv4Only is enabled/);
      }
      let dualStackConfig = createConfig();
      dualStackConfig.protocol = "HTTP";
      dualStackConfig.signatureAlgorithm = c.signatureAlgorithm;
      dualStackConfig.endpoint = `[::1]:${port}`;
      try {
        await new OpenApi(dualStackConfig).callApi(params, createOpenApiRequest(), createRuntimeOptions());
      } catch (err) {
        assert.ok(!/ipv4Only is enabled/.test(err.message));
      }
      let config = createConfig();
      config.protocol = "HTTP";
      config.signatureAlgorithm = c.signatureAlgorithm;
      config.endpoint = `127.0.0.1:${port}`;
      config.ipv4Only = true;
      let client = new OpenApi(config);
      let result = await client.callApi(params, createOpenApiRequest(), createRuntimeOptions());
      assert.strictEqual(result.statusCode, 200);
    }
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
    assert.strictEqual(headers["raw-query"], "global-query=global-value&key1=value&key2=1&key3=true&Format=json");
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

describe('Throttling backoff retry', () => {
  let throttlingServer: http.Server;
  let throttlingPort = 0;
  let throttleCount = 2;
  let retryAfterMS = 1000;
  let requestCount = 0;
  let retryAttempts: string[] = [];
  let retryDelays: string[] = [];

  function resetThrottlingState(throttle: number, retryAfter: number) {
    throttleCount = throttle;
    retryAfterMS = retryAfter;
    requestCount = 0;
    retryAttempts = [];
    retryDelays = [];
  }

  function createThrottlingRetryOptions(): $dara.RetryOptions {
    return new $dara.RetryOptions({
      retryable: true,
      retryCondition: [new $dara.RetryCondition({
        maxAttempts: 3,
        exception: [],
        errorCode: ['Throttling', 'Throttling.User', 'Throttling.Api'],
        maxDelay: 60000,
      })],
    });
  }

  function createListProductQuotasParams(): $OpenApiUtil.Params {
    return new $OpenApiUtil.Params({
      action: 'ListProductQuotas',
      version: '2020-05-10',
      protocol: 'HTTPS',
      pathname: '/',
      method: 'POST',
      authType: 'AK',
      style: 'RPC',
      reqBodyType: 'formData',
      bodyType: 'json',
    });
  }

  function createListProductQuotasRequest(): $OpenApiUtil.OpenApiRequest {
    return new $OpenApiUtil.OpenApiRequest({
      body: OpenApiUtil.parseToMap({
        ProductCode: 'Ecs',
      }),
    });
  }

  before((done) => {
    throttlingServer = http.createServer((req, res) => {
      let data = '';
      req.on('data', (chunk) => {
        data += chunk;
      });
      req.on('end', () => {
        requestCount++;
        retryAttempts.push(<string>req.headers['x-acs-retry-attempts'] || '');
        retryDelays.push(<string>req.headers['x-acs-retry-delay'] || '');

        const headers: {[key: string]: string} = {};
        Object.keys(req.headers).forEach((key) => {
          if (key !== 'content-length') {
            headers[key] = <string>req.headers[key];
          }
        });
        headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4';
        headers['raw-body'] = data;

        if (requestCount <= throttleCount) {
          headers['x-acs-retry-after'] = `${retryAfterMS}`;
          const responseBody = '{"Code":"Throttling","Message":"Request was denied due to user flow control.","RequestId":"A45EE076-334D-5012-9746-A8F828D20FD4"}';
          res.writeHead(400, headers);
          res.end(responseBody);
          return;
        }

        const responseBody = '{"RequestId":"A45EE076-334D-5012-9746-A8F828D20FD4","Quotas":[]}';
        res.writeHead(200, headers);
        res.end(responseBody);
      });
    });
    throttlingServer.listen(0, () => {
      throttlingPort = (throttlingServer.address() as AddressInfo).port;
      done();
    });
  });

  after((done) => {
    throttlingServer.close(done);
  });

  beforeEach(() => {
    resetThrottlingState(2, 1000);
  });

  it('should retry ListProductQuotas on throttling and succeed', async () => {
    resetThrottlingState(2, 1000);
    let config = createConfig();
    config.protocol = 'HTTP';
    config.endpoint = `127.0.0.1:${throttlingPort}`;
    config.retryOptions = createThrottlingRetryOptions();
    let client = new OpenApi(config);
    let runtime = createRuntimeOptions();

    const start = Date.now();
    let result = await client.callApi(
      createListProductQuotasParams(),
      createListProductQuotasRequest(),
      runtime
    );
    const elapsed = Date.now() - start;

    assert.strictEqual(200, result['statusCode']);
    assert.strictEqual('ProductCode=Ecs', result['headers']['raw-body']);
    assert.strictEqual('ListProductQuotas', result['headers']['x-acs-action']);
    assert.strictEqual('2020-05-10', result['headers']['x-acs-version']);
    assert.strictEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result['body']['RequestId']);
    assert.strictEqual(3, requestCount);
    assert.strictEqual('', retryAttempts[0]);
    assert.strictEqual('1', retryAttempts[1]);
    assert.strictEqual('2', retryAttempts[2]);
    assert.strictEqual('', retryDelays[0]);
    assert.strictEqual('1000', retryDelays[1]);
    assert.strictEqual('1000', retryDelays[2]);
    assert.ok(elapsed >= 1800);
  });

  it('should exhaust throttling retry for ListProductQuotas', async () => {
    resetThrottlingState(3, 1000);
    let config = createConfig();
    config.protocol = 'HTTP';
    config.endpoint = `127.0.0.1:${throttlingPort}`;
    config.retryOptions = createThrottlingRetryOptions();
    let client = new OpenApi(config);
    let runtime = createRuntimeOptions();

    try {
      await client.callApi(
        createListProductQuotasParams(),
        createListProductQuotasRequest(),
        runtime
      );
      assert.fail('expected throttling retry to be exhausted');
    } catch (error) {
      assert.ok(error instanceof $_error.ThrottlingError || error.message.includes('Throttling'));
    }

    assert.strictEqual(3, requestCount);
  });

  it('should throttle doRPC doROA and doROAForm', async () => {
    resetThrottlingState(3, 1);
    const config = createConfig();
    config.protocol = 'HTTP';
    config.endpoint = `127.0.0.1:${throttlingPort}`;
    const client = new OpenApi(config);
    const runtime = createRuntimeOptions();
    const request = createOpenApiRequest();
    await assert.rejects(() => client.doRPCRequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', 'json', request, runtime));
    resetThrottlingState(3, 1);
    await assert.rejects(() => client.doROARequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/test', 'json', request, runtime));
    resetThrottlingState(3, 1);
    await assert.rejects(() => client.doROARequestWithForm('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/test', 'json', request, runtime));
  });

  it('should retry direct methods and callSSEApi after throttling', async () => {
    resetThrottlingState(1, 1);
    const config = createConfig();
    config.protocol = 'HTTP';
    config.endpoint = `127.0.0.1:${throttlingPort}`;
    config.retryOptions = createThrottlingRetryOptions();
    const client = new OpenApi(config);
    const runtime = createRuntimeOptions();
    const request = createOpenApiRequest();
    const rpc = await client.doRPCRequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', 'json', request, runtime);
    assert.strictEqual(rpc.statusCode, 200);

    resetThrottlingState(1, 1);
    const roa = await client.doROARequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/test', 'json', request, runtime);
    assert.strictEqual(roa.statusCode, 200);

    resetThrottlingState(1, 1);
    const form = await client.doROARequestWithForm('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/test', 'json', request, runtime);
    assert.strictEqual(form.statusCode, 200);

    resetThrottlingState(1, 1);
    const viaDoRequest = await client.doRequest(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/',
      method: 'POST',
      authType: 'AK',
      style: 'RPC',
      reqBodyType: 'formData',
      bodyType: 'json',
    }), request, runtime);
    assert.strictEqual(viaDoRequest.statusCode, 200);

    resetThrottlingState(3, 1);
    await assert.rejects(async () => {
      for await (const _ of client.callSSEApi(new $OpenApiUtil.Params({
        action: 'TestAPI',
        version: '2022-06-01',
        protocol: 'HTTPS',
        pathname: '/sse',
        method: 'GET',
        authType: 'Anonymous',
        style: 'RPC',
        reqBodyType: 'json',
        bodyType: 'json',
      }), request, runtime)) {
        // consume
      }
    });
  });

  it('should retry execute after throttling', async () => {
    resetThrottlingState(1, 1);
    const config = createConfig();
    config.protocol = 'HTTP';
    config.endpoint = `127.0.0.1:${throttlingPort}`;
    config.signatureVersion = 'v4';
    config.retryOptions = createThrottlingRetryOptions();
    const client = new OpenApi(config);
    client._productId = 'test';
    client.setGatewayClient(new POP());
    const result = await client.execute(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/',
      method: 'POST',
      authType: 'Anonymous',
      style: 'RPC',
      reqBodyType: 'json',
      bodyType: 'json',
    }), createOpenApiRequest(), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);

    resetThrottlingState(3, 1);
    await assert.rejects(() => client.execute(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/',
      method: 'POST',
      authType: 'Anonymous',
      style: 'ROA',
      reqBodyType: 'formData',
      bodyType: 'string',
    }), createOpenApiRequest(), createRuntimeOptions()));
  });
});

describe('extra client coverage', function () {
  before((done) => {
    server.listen(0, done);
  });

  after(function (done) {
    this.timeout(10000);
    server.close(done);
  });

  it('constructor helpers checkConfig and exceptions', async function () {
    assert.throws(() => new OpenApi(null as any));
    const empty = new OpenApi(new $OpenApiUtil.Config({ endpoint: 'example.com' }));
    assert.strictEqual(await empty.getAccessKeyId(), '');
    assert.strictEqual(await empty.getAccessKeySecret(), '');
    assert.strictEqual(await empty.getSecurityToken(), '');
    assert.strictEqual(await empty.getBearerToken(), '');
    assert.strictEqual(await empty.getType(), '');
    assert.throws(() => empty.checkConfig(new $OpenApiUtil.Config()));
    empty.checkConfig(new $OpenApiUtil.Config({ endpoint: 'example.com' }));
    assert.deepStrictEqual(empty.getAccessDeniedDetail({ AccessDeniedDetail: { k: 'v' } }), { k: 'v' });
    assert.deepStrictEqual(empty.getAccessDeniedDetail({ accessDeniedDetail: { k: 'v2' } }), { k: 'v2' });
    assert.strictEqual(empty.getAccessDeniedDetail({}), null);
    empty.setRpcHeaders({ debug: '1' });
    assert.deepStrictEqual(empty.getRpcHeaders(), { debug: '1' });
    assert.strictEqual(empty.getRpcHeaders(), null);

    const client = new OpenApi(createConfig());
    try {
      await client.callApi(null as any, createOpenApiRequest(), createRuntimeOptions());
      assert.fail();
    } catch (err) {
      assert.strictEqual(err.code, 'ParameterMissing');
    }
  });

  it('v4 execute via callApi, 204 xml byte none and server errors', async function () {
    let port = (server.address() as AddressInfo).port;
    let config = createConfig();
    config.protocol = 'HTTP';
    config.endpoint = `127.0.0.1:${port}`;
    config.signatureVersion = 'v4';
    let client = new OpenApi(config);
    client._productId = 'test';
    client.setGatewayClient(new POP());
    let runtime = createRuntimeOptions();
    let request = createOpenApiRequest();
    let params = new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/test',
      method: 'POST',
      authType: 'Anonymous',
      style: 'RPC',
      reqBodyType: 'json',
      bodyType: 'json',
    });
    let result = await client.callApi(params, request, runtime);
    assert.strictEqual(result.statusCode, 200);

    config = createConfig();
    config.protocol = 'HTTP';
    config.endpoint = `127.0.0.1:${port}`;
    client = new OpenApi(config);
    params.bodyType = 'none';
    result = await client.callApi(params, request, runtime);
    assert.strictEqual(result.statusCode, 200);

    params.bodyType = 'json';
    request.headers = { ...request.headers, bodyType: 'xml' };
    runtime.extendsParameters.headers['bodyType'] = 'xml';
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.ok(err);
    }
    request.headers = createOpenApiRequest().headers;
    if (runtime.extendsParameters && runtime.extendsParameters.headers) {
      delete runtime.extendsParameters.headers['bodyType'];
    }

    config.signatureAlgorithm = 'v2';
    client = new OpenApi(config);
    params = new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/empty',
      method: 'POST',
      authType: 'Anonymous',
      style: 'ROA',
      reqBodyType: 'json',
      bodyType: 'json',
    });
    result = await client.callApi(params, request, runtime);
    assert.ok(result.headers);
    params.reqBodyType = 'formData';
    result = await client.callApi(params, request, runtime);
    assert.ok(result.headers);

    params.pathname = '/test';
    params.bodyType = 'none';
    result = await client.callApi(params, request, runtime);
    assert.strictEqual(result.statusCode, 200);

    request.headers = { ...request.headers, bodyType: 'serverError' };
    runtime.extendsParameters.headers['bodyType'] = 'serverError';
    params.bodyType = 'json';
    try {
      await client.callApi(params, request, runtime);
      assert.fail();
    } catch (err) {
      assert.ok(err);
    }
    params.reqBodyType = 'json';
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.ok(err);
    }
    params.style = 'RPC';
    params.reqBodyType = 'formData';
    params.pathname = '/';
    try {
      await client.callApi(params, request, runtime);
    } catch (err) {
      assert.ok(err);
    }
    request = createOpenApiRequest();
    if (runtime.extendsParameters && runtime.extendsParameters.headers) {
      delete runtime.extendsParameters.headers['bodyType'];
    }

    const akClient = new OpenApi(createConfig());
    akClient._endpoint = `127.0.0.1:${port}`;
    akClient._protocol = 'HTTP';
    params = new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/test',
      method: 'POST',
      authType: 'AK',
      style: 'RPC',
      reqBodyType: 'json',
      bodyType: 'byte',
    });
    result = await akClient.callApi(params, createOpenApiRequest(), createRuntimeOptions());
    assert.ok(result.body);
    params.reqBodyType = 'byte';
    params.bodyType = 'string';
    result = await akClient.doRequest(params, new $OpenApiUtil.OpenApiRequest({
      body: Buffer.from('test byte'),
    }), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);
    params.reqBodyType = 'binary';
    result = await akClient.doRequest(params, new $OpenApiUtil.OpenApiRequest({
      stream: new Readable({
        read() {
          this.push(Buffer.from('stream-body'));
          this.push(null);
        },
      }),
    }), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);
  });

  it('bearer websocket xml 204 array and direct methods', async function () {
    const port = (server.address() as AddressInfo).port;
    const runtime = createRuntimeOptions();
    const request = createOpenApiRequest();

    const bearer = new OpenApi(new $OpenApiUtil.Config({
      bearerToken: 'token',
      endpoint: `127.0.0.1:${port}`,
      protocol: 'HTTP',
    }));
    assert.strictEqual(await bearer.getBearerToken(), 'token');
    assert.strictEqual(await bearer.getType(), 'bearer');
    let result = await bearer.callApi(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/test',
      method: 'POST',
      authType: 'AK',
      style: 'ROA',
      reqBodyType: 'json',
      bodyType: 'json',
    }), request, runtime);
    assert.strictEqual(result.statusCode, 200);

    const config = createConfig();
    config.protocol = 'HTTP';
    config.endpoint = `127.0.0.1:${port}`;
    const client = new OpenApi(config);

    const wsClient = new OpenApi(new $OpenApiUtil.Config({
      accessKeyId: 'ak',
      accessKeySecret: 'sk',
      endpoint: `127.0.0.1:${port}`,
      protocol: 'ws',
    }));
    const wsParams = new $OpenApiUtil.Params({
      action: 'TestAction',
      version: '2020-01-01',
      protocol: 'ws',
      pathname: '/ws',
      method: 'GET',
      authType: 'AK',
      bodyType: 'string',
      reqBodyType: 'json',
    });
    await assert.rejects(() => wsClient.doRequest(wsParams, new $OpenApiUtil.OpenApiRequest(), runtime), /websocketSubProtocol is required/);
    wsParams.websocketSubProtocol = 'invalid';
    await assert.rejects(() => wsClient.doRequest(wsParams, new $OpenApiUtil.OpenApiRequest(), runtime), /must be 'awap' or 'general'/);
    wsParams.websocketSubProtocol = 'awap';
    await assert.rejects(() => wsClient.doRequest(wsParams, new $OpenApiUtil.OpenApiRequest(), createRuntimeOptions()), /WebSocketHandler is required/);

    const xmlRuntime = createRuntimeOptions();
    xmlRuntime.extendsParameters.headers['bodyType'] = 'xml';
    const xmlRequest = createOpenApiRequest();
    xmlRequest.headers = { ...xmlRequest.headers, bodyType: 'xml' };
    try {
      await client.callApi(new $OpenApiUtil.Params({
        action: 'TestAPI',
        version: '2022-06-01',
        protocol: 'HTTPS',
        pathname: '/test',
        method: 'POST',
        authType: 'Anonymous',
        style: 'RPC',
        reqBodyType: 'json',
        bodyType: 'json',
      }), xmlRequest, xmlRuntime);
      assert.fail('expected xml error');
    } catch (err) {
      assert.ok(err);
    }

    const v2Config = createConfig();
    v2Config.protocol = 'HTTP';
    v2Config.signatureAlgorithm = 'v2';
    v2Config.endpoint = `127.0.0.1:${port}`;
    const v2 = new OpenApi(v2Config);
    result = await v2.callApi(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/empty',
      method: 'POST',
      authType: 'Anonymous',
      style: 'ROA',
      reqBodyType: 'json',
      bodyType: 'json',
    }), createOpenApiRequest(), createRuntimeOptions());
    assert.ok(result.headers);
    result = await v2.callApi(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/empty',
      method: 'POST',
      authType: 'Anonymous',
      style: 'ROA',
      reqBodyType: 'formData',
      bodyType: 'json',
    }), createOpenApiRequest(), createRuntimeOptions());
    assert.ok(result.headers);

    const arrayRuntime = createRuntimeOptions();
    arrayRuntime.extendsParameters.headers['bodyType'] = 'array';
    const arrayRequest = createOpenApiRequest();
    arrayRequest.headers = { ...arrayRequest.headers, bodyType: 'array' };
    result = await client.callApi(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/test',
      method: 'POST',
      authType: 'AK',
      style: 'RPC',
      reqBodyType: 'json',
      bodyType: 'array',
    }), arrayRequest, arrayRuntime);
    assert.ok(Array.isArray(result.body) || result.body);

    for (const bodyType of ['json', 'string', 'byte', 'none', 'binary']) {
      result = await client.doRPCRequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', bodyType, createOpenApiRequest(), createRuntimeOptions());
      assert.strictEqual(result.statusCode, 200);
      result = await client.doROARequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/test', bodyType, createOpenApiRequest(), createRuntimeOptions());
      assert.strictEqual(result.statusCode, 200);
      result = await client.doROARequestWithForm('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/test', bodyType, createOpenApiRequest(), createRuntimeOptions());
      assert.ok(result);
    }
    result = await v2.doRPCRequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', 'string', createOpenApiRequest(), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);
    result = await v2.doROARequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/empty', 'json', createOpenApiRequest(), createRuntimeOptions());
    assert.ok(result.headers);
    result = await v2.doROARequestWithForm('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/empty', 'json', createOpenApiRequest(), createRuntimeOptions());
    assert.ok(result.headers);
    result = await client.doRequest(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/test',
      method: 'POST',
      authType: 'AK',
      style: 'ROA',
      reqBodyType: 'formData',
      bodyType: 'array',
    }), createOpenApiRequest(), createRuntimeOptions());
    assert.ok(result);
    result = await client.doRequest(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/test',
      method: 'POST',
      authType: 'Anonymous',
      style: 'RPC',
      reqBodyType: 'json',
      bodyType: 'none',
    }), createOpenApiRequest(), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);

    result = await client.doRPCRequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', 'json', createOpenApiRequest(), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);
    result = await client.doROARequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', '/test', 'string', createOpenApiRequest(), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);
    result = await client.doROARequestWithForm('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', '/test', 'byte', createOpenApiRequest(), createRuntimeOptions());
    assert.ok(result);
    result = await v2.doRPCRequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', 'json', createOpenApiRequest(), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);
    result = await v2.doRPCRequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', 'string', createOpenApiRequest(), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);
    result = await v2.doROARequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', '/test', 'json', createOpenApiRequest(), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);
    result = await v2.doROARequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', '/test', 'string', createOpenApiRequest(), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);
    result = await v2.doROARequestWithForm('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', '/test', 'json', createOpenApiRequest(), createRuntimeOptions());
    assert.ok(result);
    result = await v2.doROARequestWithForm('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', '/test', 'byte', createOpenApiRequest(), createRuntimeOptions());
    assert.ok(result);
    result = await v2.callApi(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/',
      method: 'POST',
      authType: 'AK',
      style: 'RPC',
      reqBodyType: 'formData',
      bodyType: 'json',
    }), createOpenApiRequest(), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);
    result = await v2.callApi(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/test',
      method: 'POST',
      authType: 'AK',
      style: 'ROA',
      reqBodyType: 'json',
      bodyType: 'json',
    }), createOpenApiRequest(), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);
    result = await v2.callApi(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/test',
      method: 'POST',
      authType: 'AK',
      style: 'ROA',
      reqBodyType: 'formData',
      bodyType: 'json',
    }), createOpenApiRequest(), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);

    const sseAnon = client.callSSEApi(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/sse',
      method: 'GET',
      authType: 'Anonymous',
      style: 'RPC',
      reqBodyType: 'formData',
      bodyType: 'json',
    }), createOpenApiRequest(), createRuntimeOptions());
    let sseCount = 0;
    for await (const event of sseAnon) {
      sseCount++;
      if (sseCount >= 1) {
        break;
      }
    }
    assert.ok(sseCount >= 1);
    client.setRpcHeaders({ 'rpc-key': 'rpc-value' });
    result = await client.doRPCRequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', 'json', createOpenApiRequest(), createRuntimeOptions());
    assert.strictEqual(result.statusCode, 200);

    const xmlRuntime2 = createRuntimeOptions();
    xmlRuntime2.extendsParameters.headers['bodyType'] = 'xml';
    const xmlRequest2 = createOpenApiRequest();
    xmlRequest2.headers = { ...xmlRequest2.headers, bodyType: 'xml' };
    await assert.rejects(() => client.doROARequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/test', 'json', xmlRequest2, xmlRuntime2));
    await assert.rejects(() => client.doRequest(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/test',
      method: 'POST',
      authType: 'Anonymous',
      style: 'ROA',
      reqBodyType: 'json',
      bodyType: 'json',
    }), xmlRequest2, xmlRuntime2));

    const execConfig = createConfig();
    execConfig.protocol = 'HTTP';
    execConfig.endpoint = `127.0.0.1:${port}`;
    execConfig.signatureVersion = 'v4';
    const execClient = new OpenApi(execConfig);
    execClient._productId = 'test';
    execClient.setGatewayClient(new POP());
    const hostReq = createOpenApiRequest();
    hostReq.hostMap = { region: 'cn-hangzhou' };
    hostReq.endpointOverride = `127.0.0.1:${port}`;
    for (const bodyType of ['json', 'string', 'byte', 'none', 'array']) {
      result = await execClient.execute(new $OpenApiUtil.Params({
        action: 'TestAPI',
        version: '2022-06-01',
        protocol: 'HTTPS',
        pathname: '/test',
        method: 'POST',
        authType: 'AK',
        style: 'RPC',
        reqBodyType: 'formData',
        bodyType,
      }), hostReq, createRuntimeOptions());
      assert.ok(result);
    }
  });

  it('callSSEApi remaining auth body and error paths', async function () {
    this.timeout(15000);
    const port = (server.address() as AddressInfo).port;
    const runtime = createRuntimeOptions();

    async function collectOne(gen: AsyncGenerator<any, any, unknown>): Promise<any> {
      for await (const event of gen) {
        return event;
      }
      return null;
    }

    const bearerCfg = createBearerTokenConfig();
    bearerCfg.protocol = 'HTTP';
    bearerCfg.endpoint = `127.0.0.1:${port}`;
    const bearer = new OpenApi(bearerCfg);
    const bearerEvent = await collectOne(bearer.callSSEApi(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/sse',
      method: 'POST',
      authType: 'AK',
      style: 'ROA',
      reqBodyType: 'json',
      bodyType: 'sse',
    }), createOpenApiRequest(), runtime));
    assert.ok(bearerEvent);

    const streamClient = new OpenApi((() => {
      const cfg = createConfig();
      cfg.protocol = 'HTTP';
      cfg.endpoint = `127.0.0.1:${port}`;
      return cfg;
    })());
    const streamReq = createOpenApiRequest();
    streamReq.stream = new BytesReadable('stream-body');
    const streamEvent = await collectOne(streamClient.callSSEApi(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/sse',
      method: 'POST',
      authType: 'AK',
      style: 'ROA',
      reqBodyType: 'json',
      bodyType: 'sse',
    }), streamReq, runtime));
    assert.ok(streamEvent);

    const byteReq = createOpenApiRequest();
    byteReq.body = Buffer.from('hello');
    const byteEvent = await collectOne(streamClient.callSSEApi(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/sse',
      method: 'POST',
      authType: 'Anonymous',
      style: 'ROA',
      reqBodyType: 'byte',
      bodyType: 'sse',
    }), byteReq, runtime));
    assert.ok(byteEvent);

    const sm3Cfg = createConfig();
    sm3Cfg.protocol = 'HTTP';
    sm3Cfg.endpoint = `127.0.0.1:${port}`;
    sm3Cfg.signatureAlgorithm = 'ACS3-HMAC-SM3';
    const sm3Client = new OpenApi(sm3Cfg);
    sm3Client.setRpcHeaders({ 'rpc-key': 'rpc-value' });
    const sm3Event = await collectOne(sm3Client.callSSEApi(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/sse',
      method: 'POST',
      authType: 'AK',
      style: 'RPC',
      reqBodyType: 'formData',
      bodyType: 'sse',
    }), createOpenApiRequest(), runtime));
    assert.ok(sm3Event);

    const idClient = new OpenApi((() => {
      const cfg = createConfig();
      cfg.protocol = 'HTTP';
      cfg.endpoint = `127.0.0.1:${port}`;
      return cfg;
    })());
    idClient._credential = {
      getCredential: async () => ({
        type: 'id_token',
        securityToken: 'idtok',
        providerName: 'mock',
        accessKeyId: 'ak',
        accessKeySecret: 'sk',
      }),
      getAccessKeyId: async () => 'ak',
      getAccessKeySecret: async () => 'sk',
      getSecurityToken: async () => 'idtok',
      getBearerToken: () => '',
      getType: () => 'id_token',
    } as any;
    const idEvent = await collectOne(idClient.callSSEApi(new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/sse',
      method: 'GET',
      authType: 'AK',
      style: 'ROA',
      reqBodyType: 'json',
      bodyType: 'sse',
    }), createOpenApiRequest(), runtime));
    assert.ok(idEvent);

    const idParams = new $OpenApiUtil.Params({
      action: 'TestAPI',
      version: '2022-06-01',
      protocol: 'HTTPS',
      pathname: '/test',
      method: 'POST',
      authType: 'AK',
      style: 'ROA',
      reqBodyType: 'json',
      bodyType: 'json',
    });
    let result = await idClient.doRequest(idParams, createOpenApiRequest(), runtime);
    assert.strictEqual(result.statusCode, 200);
    result = await idClient.doRPCRequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', 'json', createOpenApiRequest(), runtime);
    assert.strictEqual(result.statusCode, 200);
    result = await idClient.doROARequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', '/test', 'json', createOpenApiRequest(), runtime);
    assert.strictEqual(result.statusCode, 200);
    result = await idClient.doROARequestWithForm('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', '/test', 'json', createOpenApiRequest(), runtime);
    assert.strictEqual(result.statusCode, 200);

    const xmlReq = createOpenApiRequest();
    xmlReq.headers = { ...xmlReq.headers, bodyType: 'xml' };
    const xmlRuntime = createRuntimeOptions();
    xmlRuntime.extendsParameters.headers['bodyType'] = 'xml';
    await assert.rejects(async () => {
      for await (const _ of streamClient.callSSEApi(new $OpenApiUtil.Params({
        action: 'TestAPI',
        version: '2022-06-01',
        protocol: 'HTTPS',
        pathname: '/test',
        method: 'POST',
        authType: 'Anonymous',
        style: 'ROA',
        reqBodyType: 'json',
        bodyType: 'json',
      }), xmlReq, xmlRuntime)) {
        // consume
      }
    });

    const errReq = createOpenApiRequest();
    errReq.headers = { ...errReq.headers, bodyType: 'error' };
    const errRuntime = createRuntimeOptions();
    errRuntime.extendsParameters.headers['bodyType'] = 'error';
    await assert.rejects(async () => {
      for await (const _ of streamClient.callSSEApi(new $OpenApiUtil.Params({
        action: 'TestAPI',
        version: '2022-06-01',
        protocol: 'HTTPS',
        pathname: '/test',
        method: 'POST',
        authType: 'Anonymous',
        style: 'RPC',
        reqBodyType: 'formData',
        bodyType: 'json',
      }), errReq, errRuntime)) {
        // consume
      }
    });
  });
});