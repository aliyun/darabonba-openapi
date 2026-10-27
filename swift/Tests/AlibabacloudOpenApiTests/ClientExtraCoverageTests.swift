import Foundation
import XCTest
import TeaUtils
@testable import AlibabacloudOpenApi

final class ClientExtraCoverageTests: XCTestCase {
    func testConstructorsHelpersAndModels() async throws {
        let empty = try Client(Config([
            "endpoint": "example.com"
        ]))
        let accessKeyId = try await empty.getAccessKeyId()
        let accessKeySecret = try await empty.getAccessKeySecret()
        let token = try await empty.getSecurityToken()
        let bearer = try await empty.getBearerToken()
        let type = try await empty.getType()
        XCTAssertEqual(accessKeyId, "")
        XCTAssertEqual(accessKeySecret, "")
        XCTAssertEqual(token, "")
        XCTAssertEqual(bearer, "")
        XCTAssertEqual(type, "")
        XCTAssertFalse(empty.getUserAgent().isEmpty)

        XCTAssertThrowsError(try empty.checkConfig(Config([:])))
        try empty.checkConfig(Config(["endpoint": "example.com"]))

        try empty.setRpcHeaders(["debug": "1"])
        XCTAssertEqual(try empty.getRpcHeaders()["debug"], "1")
        XCTAssertEqual(try empty.getRpcHeaders().count, 0)

        XCTAssertEqual(Client.defaultAny(nil, "fb") as? String, "fb")
        XCTAssertEqual(Client.defaultAny("keep", "fb") as? String, "keep")

        let bearerClient = try Client(Config([
            "bearerToken": "token",
            "endpoint": "example.com"
        ]))
        let bearerToken = try await bearerClient.getBearerToken()
        let bearerType = try await bearerClient.getType()
        XCTAssertEqual(bearerToken, "token")
        XCTAssertEqual(bearerType, "bearer")

        let ak = try Client(Config([
            "accessKeyId": "ak",
            "accessKeySecret": "sk",
            "endpoint": "example.com"
        ]))
        let akId = try await ak.getAccessKeyId()
        let akSecret = try await ak.getAccessKeySecret()
        let akType = try await ak.getType()
        XCTAssertEqual(akId, "ak")
        XCTAssertEqual(akSecret, "sk")
        XCTAssertEqual(akType, "access_key")

        let sts = try Client(Config([
            "accessKeyId": "ak",
            "accessKeySecret": "sk",
            "securityToken": "st",
            "endpoint": "example.com"
        ]))
        let stsType = try await sts.getType()
        let stsToken = try await sts.getSecurityToken()
        XCTAssertEqual(stsType, "sts")
        XCTAssertEqual(stsToken, "st")

        let fromCredConfig = Config(["endpoint": "example.com"])
        fromCredConfig.credential = sts._credential
        let fromCred = try Client(fromCredConfig)
        let fromCredId = try await fromCred.getAccessKeyId()
        XCTAssertEqual(fromCredId, "ak")

        let blank = Config()
        blank.credential = sts._credential
        XCTAssertNotNil(blank.toMap()["credential"])
        let loaded = Config()
        if let cred = sts._credential {
            loaded.fromMap(["credential": cred])
        }
        XCTAssertNotNil(loaded.credential)
        let emptyParams = Params()
        XCTAssertNil(emptyParams.action)

        let config = Config([
            "accessKeyId": "ak",
            "accessKeySecret": "sk",
            "securityToken": "st",
            "bearerToken": "bt",
            "protocol": "HTTP",
            "method": "POST",
            "regionId": "cn-hangzhou",
            "readTimeout": 1000,
            "connectTimeout": 1000,
            "httpProxy": "http://127.0.0.1",
            "httpsProxy": "https://127.0.0.1",
            "endpoint": "example.com",
            "noProxy": "localhost",
            "maxIdleConns": 10,
            "network": "public",
            "userAgent": "ua",
            "suffix": "sfx",
            "socks5Proxy": "socks5://127.0.0.1",
            "socks5NetWork": "tcp",
            "endpointType": "regional",
            "openPlatformEndpoint": "open.aliyun.com",
            "type": "access_key",
            "signatureVersion": "v4",
            "signatureAlgorithm": "ACS3-HMAC-SHA256",
            "key": "k",
            "cert": "c",
            "ca": "ca",
            "disableHttp2": true,
            "tlsMinVersion": "1.2"
        ])
        config.globalParameters = GlobalParameters([
            "headers": ["h": "1"],
            "queries": ["q": "2"]
        ])
        try config.validate()
        XCTAssertEqual(config.toMap()["endpoint"] as? String, "example.com")
        XCTAssertEqual(config.toMap()["protocol"] as? String, "HTTP")
        XCTAssertNotNil(config.toMap()["globalParameters"])
        let restoredConfig = Config()
        restoredConfig.fromMap(nil as [String: Any?]?)
        restoredConfig.fromMap(config.toMap())
        XCTAssertEqual(restoredConfig.endpoint, "example.com")
        XCTAssertEqual(restoredConfig.signatureVersion, "v4")
        restoredConfig.fromMap([
            "globalParameters": ["headers": ["h": "1"], "queries": ["q": "2"]] as [String: Any?]
        ])
        XCTAssertEqual(restoredConfig.globalParameters?.queries?["q"], "2")

        let params = Params([
            "action": "A",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/",
            "method": "POST",
            "authType": "AK",
            "bodyType": "json",
            "reqBodyType": "json",
            "style": "RPC"
        ])
        try params.validate()
        XCTAssertEqual(params.toMap()["action"] as? String, "A")
        XCTAssertEqual(params.toMap()["style"] as? String, "RPC")

        let global = GlobalParameters([
            "headers": ["h": "1"],
            "queries": ["q": "2"]
        ])
        try global.validate()
        XCTAssertEqual((global.toMap()["headers"] as? [String: String])?["h"], "1")
        let restoredGlobal = GlobalParameters()
        restoredGlobal.fromMap(nil)
        restoredGlobal.fromMap(global.toMap())
        XCTAssertEqual(restoredGlobal.queries?["q"], "2")
    }

    func testCallApiPaths() async throws {
        let server = TinyHTTPServer()
        defer { server.stop() }

        let runtime = TeaUtils.RuntimeOptions([:])
        runtime.ignoreSSL = true
        runtime.connectTimeout = 5000
        runtime.readTimeout = 5000
        runtime.maxAttempts = 1
        let request = OpenApiRequest([
            "headers": ["for-test": "sdk"],
            "query": ["k": "v"],
            "body": ["key1": "value"]
        ])
        let config = Config([
            "accessKeyId": "ak",
            "accessKeySecret": "secret",
            "endpoint": "127.0.0.1:\(server.port)",
            "protocol": "HTTP",
            "signatureAlgorithm": "ACS3-HMAC-SHA256",
            "disableHttp2": true,
            "globalParameters": GlobalParameters([
                "headers": ["global-key": "global-value"],
                "queries": ["global-query": "global-value"]
            ]),
            "userAgent": "extra-ua",
            "readTimeout": 3000,
            "connectTimeout": 3000
        ])
        let client = try Client(config)
        client._globalParameters = GlobalParameters([
            "headers": ["global-key": "global-value"],
            "queries": ["global-query": "global-value"]
        ])
        let params = Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/test",
            "method": "POST",
            "authType": "Anonymous",
            "style": "RPC",
            "reqBodyType": "json",
            "bodyType": "json"
        ])
        func statusCode(_ result: [String: Any]) -> Int {
            if let n = result["statusCode"] as? Int { return n }
            if let n = result["statusCode"] as? Int32 { return Int(n) }
            if let n = result["statusCode"] as? Int64 { return Int(n) }
            if let n = result["statusCode"] as? NSNumber { return n.intValue }
            return 0
        }

        let jsonResult = try await client.callApi(params, request, runtime)
        XCTAssertEqual(statusCode(jsonResult), 200)

        params.bodyType = "none"
        let noneResult = try await client.callApi(params, request, runtime)
        XCTAssertEqual(statusCode(noneResult), 200)

        params.bodyType = "string"
        let stringResult = try await client.callApi(params, request, runtime)
        XCTAssertNotNil(stringResult["body"])

        params.bodyType = "byte"
        let byteResult = try await client.callApi(params, request, runtime)
        XCTAssertNotNil(byteResult["body"])

        params.bodyType = "array"
        params.pathname = "/array"
        let arrayResult = try await client.callApi(params, request, runtime)
        XCTAssertEqual(statusCode(arrayResult), 200)

        params.bodyType = "json"
        params.pathname = "/xml"
        do {
            _ = try await client.callApi(params, request, runtime)
            XCTFail("expected xml error")
        } catch {
            XCTAssertNotNil(error)
        }

        let v2 = try Client(Config([
            "accessKeyId": "ak",
            "accessKeySecret": "secret",
            "endpoint": "127.0.0.1:\(server.port)",
            "protocol": "HTTP",
            "signatureAlgorithm": "v2",
            "disableHttp2": true
        ]))
        let emptyParams = Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/empty",
            "method": "POST",
            "authType": "Anonymous",
            "style": "ROA",
            "reqBodyType": "json",
            "bodyType": "json"
        ])
        let emptyResult = try await v2.callApi(emptyParams, request, runtime)
        XCTAssertNotNil(emptyResult["headers"])
        emptyParams.reqBodyType = "formData"
        let formEmpty = try await v2.callApi(emptyParams, request, runtime)
        XCTAssertNotNil(formEmpty["headers"])

        let failParams = Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/server",
            "method": "POST",
            "authType": "Anonymous",
            "style": "ROA",
            "reqBodyType": "formData",
            "bodyType": "json"
        ])
        do {
            _ = try await v2.callApi(failParams, request, runtime)
            XCTFail("expected server error")
        } catch {
            XCTAssertNotNil(error)
        }

        let direct = try await client.doRPCRequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "json", request, runtime)
        XCTAssertEqual(statusCode(direct), 200)
        let rpcString = try await client.doRPCRequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "string", request, runtime)
        XCTAssertEqual(statusCode(rpcString), 200)
        let rpcNone = try await client.doRPCRequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "none", request, runtime)
        XCTAssertEqual(statusCode(rpcNone), 200)
        let roa = try await client.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/test", "json", request, runtime)
        XCTAssertEqual(statusCode(roa), 200)
        let roaString = try await client.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/test", "string", request, runtime)
        XCTAssertEqual(statusCode(roaString), 200)
        let roaForm = try await client.doROARequestWithForm("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/test", "json", request, runtime)
        XCTAssertEqual(statusCode(roaForm), 200)
        let roaFormNone = try await client.doROARequestWithForm("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/test", "none", request, runtime)
        XCTAssertEqual(statusCode(roaFormNone), 200)
        let doReq = try await client.doRequest(Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/test",
            "method": "POST",
            "authType": "Anonymous",
            "style": "RPC",
            "reqBodyType": "json",
            "bodyType": "json"
        ]), request, runtime)
        XCTAssertEqual(statusCode(doReq), 200)
        let doReqForm = try await client.doRequest(Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/test",
            "method": "POST",
            "authType": "AK",
            "style": "ROA",
            "reqBodyType": "formData",
            "bodyType": "json"
        ]), request, runtime)
        XCTAssertEqual(statusCode(doReqForm), 200)
        let rpcByte = try await client.doRPCRequest("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "byte", request, runtime)
        XCTAssertNotNil(rpcByte["body"])
        let rpcBinary = try await client.doRPCRequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "binary", request, runtime)
        XCTAssertNotNil(rpcBinary["body"])
        let roaByte = try await client.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "/test", "byte", request, runtime)
        XCTAssertNotNil(roaByte["body"])
        let roaNone = try await client.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/test", "none", request, runtime)
        XCTAssertEqual(statusCode(roaNone), 200)
        let formByte = try await client.doROARequestWithForm("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "/test", "byte", request, runtime)
        XCTAssertNotNil(formByte["body"])
        let formString = try await client.doROARequestWithForm("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/test", "string", request, runtime)
        XCTAssertNotNil(formString["body"])
        let akParams = Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/test",
            "method": "POST",
            "authType": "AK",
            "style": "RPC",
            "reqBodyType": "formData",
            "bodyType": "json"
        ])
        let akResult = try await client.callApi(akParams, request, runtime)
        XCTAssertEqual(statusCode(akResult), 200)
        let bearerClient = try Client(Config([
            "bearerToken": "token",
            "endpoint": "127.0.0.1:\(server.port)",
            "protocol": "HTTP",
            "disableHttp2": true
        ]))
        let bearerResult = try await bearerClient.callApi(Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/test",
            "method": "POST",
            "authType": "AK",
            "style": "ROA",
            "reqBodyType": "json",
            "bodyType": "json"
        ]), request, runtime)
        XCTAssertEqual(statusCode(bearerResult), 200)
        do {
            _ = try await client.callApi(Params([
                "action": "TestAPI",
                "version": "2022-06-01",
                "protocol": "HTTP",
                "pathname": "/error",
                "method": "POST",
                "authType": "Anonymous",
                "style": "ROA",
                "reqBodyType": "json",
                "bodyType": "json"
            ]), request, runtime)
            XCTFail("expected client error")
        } catch {
            XCTAssertNotNil(error)
        }

        let v2Rpc = Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/",
            "method": "POST",
            "authType": "Anonymous",
            "style": "RPC",
            "reqBodyType": "formData",
            "bodyType": "json"
        ])
        let v2RpcResult = try await v2.callApi(v2Rpc, request, runtime)
        XCTAssertEqual(statusCode(v2RpcResult), 200)
        v2Rpc.reqBodyType = "json"
        let v2RpcJson = try await v2.callApi(v2Rpc, request, runtime)
        XCTAssertEqual(statusCode(v2RpcJson), 200)
        v2Rpc.authType = "AK"
        v2Rpc.reqBodyType = "formData"
        let v2RpcAk = try await v2.callApi(v2Rpc, request, runtime)
        XCTAssertEqual(statusCode(v2RpcAk), 200)
        let v2RoaAk = try await v2.callApi(Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/test",
            "method": "POST",
            "authType": "AK",
            "style": "ROA",
            "reqBodyType": "json",
            "bodyType": "json"
        ]), request, runtime)
        XCTAssertEqual(statusCode(v2RoaAk), 200)
        let v2RoaFormAk = try await v2.callApi(Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/test",
            "method": "POST",
            "authType": "AK",
            "style": "ROA",
            "reqBodyType": "formData",
            "bodyType": "json"
        ]), request, runtime)
        XCTAssertEqual(statusCode(v2RoaFormAk), 200)
        let v2DoRpcAk = try await v2.doRPCRequest("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "json", request, runtime)
        XCTAssertEqual(statusCode(v2DoRpcAk), 200)
        let v2DoRoaAk = try await v2.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "/test", "json", request, runtime)
        XCTAssertEqual(statusCode(v2DoRoaAk), 200)
        let v2DoFormAk = try await v2.doROARequestWithForm("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "/test", "json", request, runtime)
        XCTAssertEqual(statusCode(v2DoFormAk), 200)

        let reqWithHost = OpenApiRequest([
            "headers": ["for-test": "sdk"],
            "query": ["k": "v"],
            "body": ["key1": "value"],
            "hostMap": ["region": "cn-hangzhou"],
            "endpointOverride": "127.0.0.1:\(server.port)"
        ])
        try reqWithHost.validate()
        XCTAssertEqual(reqWithHost.toMap()["endpointOverride"] as? String, "127.0.0.1:\(server.port)")
        XCTAssertEqual((reqWithHost.toMap()["hostMap"] as? [String: String])?["region"], "cn-hangzhou")
        reqWithHost.fromMap(nil)
        XCTAssertEqual(reqWithHost.endpointOverride, "127.0.0.1:\(server.port)")

        do {
            _ = try await client.callApi(Params([:]), request, runtime)
            XCTFail("expected params validation error")
        } catch {
            XCTAssertNotNil(error)
        }
    }
}
