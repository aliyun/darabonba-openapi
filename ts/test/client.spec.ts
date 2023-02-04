'use strict';

import http from 'http';
import url from 'url';
import 'mocha';
import assert from 'assert';
import { AddressInfo } from 'net';
import { Readable } from 'stream';

import OpenApi, * as $OpenApi from "../src/client";
import OpenApiUtil from '@alicloud/openapi-util';
import Util, * as $Util from '@alicloud/tea-util';
import Credential, * as $Credential from '@alicloud/credentials';
import * as $tea from '@alicloud/tea-typescript';

const server = http.createServer((req, res) => {
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
            default:
                res.writeHead(200, {
                    ...headers,
                });
        }
        res.end(responseBody);
    })
});

function createConfig(): $OpenApi.Config {
    let globalParameters = new $OpenApi.GlobalParameters({
        headers: {
            'global-key': "global-value",
        },
        queries: {
            'global-query': "global-value",
        },
    });
    let config = new $OpenApi.Config({
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

function createRuntimeOptions(): $Util.RuntimeOptions {
    let runtime = new $Util.RuntimeOptions({
        readTimeout: 4000,
        connectTimeout: 4000,
        maxIdleConns: 100,
        autoretry: true,
        maxAttempts: 1,
        backoffPolicy: "no",
        backoffPeriod: 1,
        ignoreSSL: true,
    });
    return runtime;
}

function createOpenApiRequest(): $OpenApi.OpenApiRequest {
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
    let req = new $OpenApi.OpenApiRequest({
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
        let globalParameters = new $OpenApi.GlobalParameters({
            headers: {
                'global-key': "global-value",
            },
            queries: {
                'global-query': "global-value",
            },
        });
        let config = new $OpenApi.Config({
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

        config.accessKeyId = "ak";
        config.accessKeySecret = "secret";
        config.securityToken = "token";
        config.type = "sts";
        client = new OpenApi(config);
        assert.strictEqual(await client.getAccessKeyId(), 'ak');
        assert.strictEqual(await client.getAccessKeySecret(), 'secret');
        assert.strictEqual(await client.getSecurityToken(), 'token');
        assert.strictEqual(await client.getType(), 'sts');
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
        let params = new $OpenApi.Params({
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
        assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/1 config.userAgent"));
        assert.strictEqual(headers["global-key"], "global-value");
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

    it("call api for RPC With V2Sign Anonymous JSON should ok", async function () {
        let config = createConfig();
        let runtime = createRuntimeOptions();
        config.protocol = "HTTP";
        config.signatureAlgorithm = "v2";
        let port = (server.address() as AddressInfo).port;
        config.endpoint = `127.0.0.1:${port}`;
        let client = new OpenApi(config);
        let request = createOpenApiRequest();
        let params = new $OpenApi.Params({
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
        assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/1 config.userAgent"));
        assert.strictEqual(headers["global-key"], "global-value");
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
        let params = new $OpenApi.Params({
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
        assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/1 config.userAgent"));
        assert.strictEqual(headers["x-acs-version"], "2022-06-01");
        assert.strictEqual(headers["x-acs-action"], "TestAPI");
        assert.strictEqual(headers["content-type"], "application/x-www-form-urlencoded");
        assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
        assert.strictEqual(headers["http-method"], "POST");
        assert.strictEqual(headers["pathname"], "/test");
        assert.strictEqual(headers["connection"], "keep-alive");
        assert.strictEqual(headers["for-test"], "sdk");
        assert.strictEqual(headers["global-key"], "global-value");
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
        let params = new $OpenApi.Params({
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
        assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/1 config.userAgent"));
        assert.strictEqual(headers["x-acs-version"], "2022-06-01");
        assert.strictEqual(headers["x-acs-action"], "TestAPI");
        assert.strictEqual(headers["content-type"], "application/json; charset=utf-8");
        assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
        assert.strictEqual(headers["http-method"], "POST");
        assert.strictEqual(headers["pathname"], "/test");
        assert.strictEqual(headers["connection"], "keep-alive");
        assert.strictEqual(headers["for-test"], "sdk");
        assert.strictEqual(headers["global-key"], "global-value");
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
        let params = new $OpenApi.Params({
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
        assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/1 config.userAgent"));
        assert.strictEqual(headers["x-acs-version"], "2022-06-01");
        assert.strictEqual(headers["x-acs-action"], "TestAPI");
        assert.strictEqual(headers["content-type"], "application/x-www-form-urlencoded");
        assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
        assert.strictEqual(headers["http-method"], "POST");
        assert.strictEqual(headers["pathname"], "/");
        assert.strictEqual(headers["connection"], "keep-alive");
        assert.strictEqual(headers["for-test"], "sdk");
        assert.strictEqual(headers["global-key"], "global-value");
        assert.ok(headers["x-acs-signature-nonce"].length > 0);
        assert.ok(headers["x-acs-date"].length > 0);
        assert.ok(headers["x-acs-content-sha256"].length > 0);
        assert.strictEqual(headers["accept"], "application/json");
        assert.strictEqual(headers["x-acs-accesskey-id"], "ak");
        assert.strictEqual(headers["x-acs-security-token"], "token");
        assert.ok(String(headers["authorization"]).startsWith("ACS3-HMAC-SHA256 Credential=ak," +
            "SignedHeaders=content-type;host;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-date;x-acs-security-token;" +
            "x-acs-signature-nonce;x-acs-version,Signature="));

        let body = result["body"];
        assert.strictEqual(body["AppId"], "test");
        assert.strictEqual(body["ClassId"], "test");
        assert.strictEqual(body["UserId"], 123);
        assert.strictEqual(result["statusCode"], 200);
    });

    it("call api for RPC With V3Sign Anonymous JSON should ok", async function () {
        let config = createConfig();
        let runtime = createRuntimeOptions();
        config.protocol = "HTTP";
        let port = (server.address() as AddressInfo).port;
        config.endpoint = `127.0.0.1:${port}`;
        let client = new OpenApi(config);
        let request = createOpenApiRequest();
        let params = new $OpenApi.Params({
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
        assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/1 config.userAgent"));
        assert.strictEqual(headers["x-acs-version"], "2022-06-01");
        assert.strictEqual(headers["x-acs-action"], "TestAPI");
        assert.strictEqual(headers["content-type"], "application/json; charset=utf-8");
        assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
        assert.strictEqual(headers["http-method"], "POST");
        assert.strictEqual(headers["pathname"], "/");
        assert.strictEqual(headers["connection"], "keep-alive");
        assert.strictEqual(headers["for-test"], "sdk");
        assert.strictEqual(headers["global-key"], "global-value");
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
        let params = new $OpenApi.Params({
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
        assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/1 config.userAgent"));
        assert.strictEqual(headers["x-acs-version"], "2022-06-01");
        assert.strictEqual(headers["x-acs-action"], "TestAPI");
        assert.strictEqual(headers["content-type"], "application/x-www-form-urlencoded");
        assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
        assert.strictEqual(headers["http-method"], "POST");
        assert.strictEqual(headers["pathname"], "/test");
        assert.strictEqual(headers["connection"], "keep-alive");
        assert.strictEqual(headers["for-test"], "sdk");
        assert.strictEqual(headers["global-key"], "global-value");
        assert.ok(headers["x-acs-signature-nonce"].length > 0);
        assert.ok(headers["x-acs-date"].length > 0);
        assert.ok(headers["x-acs-content-sha256"].length > 0);
        assert.strictEqual(headers["accept"], "application/json");
        assert.strictEqual(headers["x-acs-accesskey-id"], "ak");
        assert.strictEqual(headers["x-acs-security-token"], "token");
        assert.ok(String(headers["authorization"]).startsWith("ACS3-HMAC-SHA256 Credential=ak," +
            "SignedHeaders=content-type;host;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-date;x-acs-security-token;" +
            "x-acs-signature-nonce;x-acs-version,Signature="));

        let body = result["body"];
        assert.strictEqual(body["AppId"], "test");
        assert.strictEqual(body["ClassId"], "test");
        assert.strictEqual(body["UserId"], 123);
        assert.strictEqual(result["statusCode"], 200);
    });

    it("call api for ROA With V3Sign Anonymous JSON should ok", async function () {
        let config = createConfig();
        let runtime = createRuntimeOptions();
        config.protocol = "HTTP";
        let port = (server.address() as AddressInfo).port;
        config.endpoint = `127.0.0.1:${port}`;
        let client = new OpenApi(config);
        let request = createOpenApiRequest();
        let params = new $OpenApi.Params({
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
        assert.ok(String(headers["user-agent"]).endsWith("TeaDSL/1 config.userAgent"));
        assert.strictEqual(headers["x-acs-version"], "2022-06-01");
        assert.strictEqual(headers["x-acs-action"], "TestAPI");
        assert.strictEqual(headers["content-type"], "application/json; charset=utf-8");
        assert.strictEqual(headers["x-acs-request-id"], "A45EE076-334D-5012-9746-A8F828D20FD4");
        assert.strictEqual(headers["http-method"], "POST");
        assert.strictEqual(headers["pathname"], "/test");
        assert.strictEqual(headers["connection"], "keep-alive");
        assert.strictEqual(headers["for-test"], "sdk");
        assert.strictEqual(headers["global-key"], "global-value");
        assert.ok(headers["x-acs-signature-nonce"].length > 0);
        assert.ok(headers["x-acs-date"].length > 0);
        assert.strictEqual(headers["accept"], "application/json");

        let body = result["body"];
        assert.strictEqual(body["AppId"], "test");
        assert.strictEqual(body["ClassId"], "test");
        assert.strictEqual(body["UserId"], 123);
        assert.strictEqual(result["statusCode"], 200);
    });

    it("response body should ok", async function () {
        let config = createConfig();
        let runtime = createRuntimeOptions();
        config.protocol = "HTTP";
        let port = (server.address() as AddressInfo).port;
        config.endpoint = `127.0.0.1:${port}`;
        let client = new OpenApi(config);
        let request = createOpenApiRequest();
        let params = new $OpenApi.Params({
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
        params.bodyType = "string";
        result = await client.callApi(params, request, runtime);
        body = result["body"];
        assert.strictEqual(body, "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
        assert.strictEqual(result["statusCode"], 200);

        request.headers = {
            ...request.headers,
            bodyType: "byte"
        };
        params.bodyType = "byte";
        result = await client.callApi(params, request, runtime);
        body = result["body"];
        assert.ok(body instanceof Buffer);
        assert.strictEqual(body.toString(), "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}");
        assert.strictEqual(result["statusCode"], 200);

        request.headers = {
            ...request.headers,
            bodyType: "error"
        };
        try {
            await client.callApi(params, request, runtime);
        } catch (err) {
            assert.strictEqual(err.message, "error code: code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4");
            assert.strictEqual(err.statusCode, 400);
        }
    });

});
