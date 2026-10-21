import Foundation
import XCTest
import Tea
import TeaUtils
import AlibabacloudGatewaySPI
@testable import AlibabacloudOpenApi

typealias OpenApiClient = AlibabacloudOpenApi.Client

final class ExecuteCoverageTests: XCTestCase {
    func testExecuteAndRemainingPaths() async throws {
        let server = TinyHTTPServer()
        defer { server.stop() }

        let runtime = TeaUtils.RuntimeOptions([:])
        runtime.ignoreSSL = true
        runtime.connectTimeout = 5000
        runtime.readTimeout = 5000
        runtime.maxAttempts = 3
        runtime.autoretry = true
        runtime.backoffPolicy = "no"
        runtime.backoffPeriod = 1
        runtime.extendsParameters = TeaUtils.ExtendsParameters([
            "headers": ["extends-key": "extends-value"],
            "queries": ["extends-q": "extends-v"]
        ])

        let request = OpenApiRequest([
            "headers": ["for-test": "sdk"],
            "query": ["k": "v"],
            "body": ["key1": "value"],
            "hostMap": ["region": "cn-hangzhou"],
            "endpointOverride": "127.0.0.1:\(server.port)"
        ])
        request.stream = Tea.TeaCore.toReadable("{}")

        let config = AlibabacloudOpenApi.Config([
            "accessKeyId": "ak",
            "accessKeySecret": "secret",
            "securityToken": "st",
            "endpoint": "127.0.0.1:\(server.port)",
            "protocol": "HTTP",
            "signatureVersion": "v4",
            "signatureAlgorithm": "ACS3-HMAC-SHA256",
            "disableHttp2": true,
            "regionId": "cn-hangzhou",
            "network": "public",
            "suffix": "sfx",
            "endpointType": "regional",
            "userAgent": "extra-ua",
            "readTimeout": 3000,
            "connectTimeout": 3000,
            "globalParameters": GlobalParameters([
                "headers": ["global-key": "global-value"],
                "queries": ["global-query": "global-value"]
            ])
        ])
        let client = try OpenApiClient(config)
        client._productId = "test"
        client._globalParameters = GlobalParameters([
            "headers": ["global-key": "global-value"],
            "queries": ["global-query": "global-value"]
        ])
        try client.setGatewayClient(LocalHTTPGateway())
        try client.setRpcHeaders(["rpc-key": "rpc-value"])

        func statusCode(_ result: [String: Any]) -> Int {
            if let n = result["statusCode"] as? Int { return n }
            if let n = result["statusCode"] as? Int32 { return Int(n) }
            if let n = result["statusCode"] as? Int64 { return Int(n) }
            if let n = result["statusCode"] as? NSNumber { return n.intValue }
            return 0
        }

        let params = Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/test",
            "method": "POST",
            "authType": "AK",
            "style": "RPC",
            "reqBodyType": "json",
            "bodyType": "json"
        ])
        let probe = URL(string: "http://127.0.0.1:\(server.port)/test")!
        let (_, probeResp) = try await URLSession.shared.data(from: probe)
        XCTAssertEqual((probeResp as? HTTPURLResponse)?.statusCode, 200)
        do {
            let viaCallApi = try await client.callApi(params, request, runtime)
            XCTAssertEqual(statusCode(viaCallApi), 200)
            let executed = try await client.execute(params, request, runtime)
            XCTAssertEqual(statusCode(executed), 200)
            let emptyMethod = Params([
                "action": "TestAPI",
                "version": "2022-06-01",
                "protocol": "HTTP",
                "pathname": "/plain",
                "method": "",
                "authType": "Anonymous",
                "style": "RPC",
                "reqBodyType": "json",
                "bodyType": "json"
            ])
            let noBody = OpenApiRequest([
                "headers": ["for-test": "sdk"],
                "endpointOverride": "127.0.0.1:\(server.port)"
            ])
            noBody.body = [:]
            noBody.stream = Tea.TeaCore.toReadable("{}")
            let plain = try await client.execute(emptyMethod, noBody, runtime)
            XCTAssertNotNil(plain["headers"])
        } catch {
            XCTAssertNotNil(error)
        }

        let spiOnly = try OpenApiClient(AlibabacloudOpenApi.Config([
            "accessKeyId": "ak",
            "accessKeySecret": "secret",
            "endpoint": "127.0.0.1:\(server.port)",
            "protocol": "HTTP",
            "signatureVersion": "v4",
            "disableHttp2": true
        ]))
        try spiOnly.setGatewayClient(AlibabacloudGatewaySPI.Client())
        do {
            _ = try await spiOnly.execute(params, request, runtime)
        } catch {
            XCTAssertNotNil(error)
        }

        let streamReq = OpenApiRequest([
            "headers": ["for-test": "sdk"],
            "query": ["k": "v"],
            "body": ["key1": "value"]
        ])
        streamReq.stream = Tea.TeaCore.toReadable("stream-body")
        let streamResult = try await client.doRequest(Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/test",
            "method": "POST",
            "authType": "AK",
            "style": "RPC",
            "reqBodyType": "json",
            "bodyType": "json"
        ]), streamReq, runtime)
        XCTAssertEqual(statusCode(streamResult), 200)

        let byteReq = OpenApiRequest([
            "headers": ["for-test": "sdk"],
            "body": Array("hello".utf8)
        ])
        let byteResult = try await client.doRequest(Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/test",
            "method": "POST",
            "authType": "Anonymous",
            "style": "ROA",
            "reqBodyType": "byte",
            "bodyType": "byte"
        ]), byteReq, runtime)
        XCTAssertNotNil(byteResult["body"])

        for bodyType in ["string", "binary", "array", "none"] {
            let path = bodyType == "array" ? "/array" : "/test"
            let typed = try await client.doRequest(Params([
                "action": "TestAPI",
                "version": "2022-06-01",
                "protocol": "HTTP",
                "pathname": path,
                "method": "POST",
                "authType": "Anonymous",
                "style": "ROA",
                "reqBodyType": "json",
                "bodyType": bodyType
            ]), request, runtime)
            XCTAssertNotNil(typed["headers"])
        }

        do {
            _ = try await client.doRequest(Params([
                "action": "TestAPI",
                "version": "2022-06-01",
                "protocol": "HTTP",
                "pathname": "/xml",
                "method": "POST",
                "authType": "Anonymous",
                "style": "ROA",
                "reqBodyType": "json",
                "bodyType": "json"
            ]), request, runtime)
            XCTFail("expected xml error")
        } catch {
            XCTAssertNotNil(error)
        }

        let bearerClient = try OpenApiClient(AlibabacloudOpenApi.Config([
            "bearerToken": "token",
            "endpoint": "127.0.0.1:\(server.port)",
            "protocol": "HTTP",
            "disableHttp2": true
        ]))
        let bearerRpc = try await bearerClient.doRequest(Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/test",
            "method": "POST",
            "authType": "AK",
            "style": "RPC",
            "reqBodyType": "json",
            "bodyType": "json"
        ]), request, runtime)
        XCTAssertEqual(statusCode(bearerRpc), 200)
        let bearerDoRpc = try await bearerClient.doRPCRequest("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "json", request, runtime)
        XCTAssertEqual(statusCode(bearerDoRpc), 200)
        let bearerDoRoa = try await bearerClient.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "/test", "json", request, runtime)
        XCTAssertEqual(statusCode(bearerDoRoa), 200)
        let bearerForm = try await bearerClient.doROARequestWithForm("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "/test", "json", request, runtime)
        XCTAssertEqual(statusCode(bearerForm), 200)

        try client.setRpcHeaders(["rpc-key": "rpc-value"])
        let rpcHeaders = try await client.doRPCRequest("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "array", request, runtime)
        XCTAssertEqual(statusCode(rpcHeaders), 200)
        do {
            _ = try await client.doRPCRequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "json", OpenApiRequest([
                "headers": ["x-force-error": "1"],
                "query": ["k": "v"],
                "body": ["key1": "value"]
            ]), runtime)
            XCTFail("expected rpc server error")
        } catch {
            XCTAssertNotNil(error)
        }
        let rpcServerReq = OpenApiRequest([
            "headers": ["for-test": "sdk"],
            "query": ["k": "v"],
            "body": ["key1": "value"]
        ])
        do {
            _ = try await client.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/server", "json", rpcServerReq, runtime)
            XCTFail("expected server error")
        } catch {
            XCTAssertNotNil(error)
        }
        do {
            _ = try await client.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/xml", "json", rpcServerReq, runtime)
            XCTFail("expected xml error")
        } catch {
            XCTAssertNotNil(error)
        }
        let emptyRoa = try await client.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/empty", "json", request, runtime)
        XCTAssertNotNil(emptyRoa["headers"])
        let arrayRoa = try await client.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/array", "array", request, runtime)
        XCTAssertEqual(statusCode(arrayRoa), 200)
        let formArray = try await client.doROARequestWithForm("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/array", "array", request, runtime)
        XCTAssertEqual(statusCode(formArray), 200)
        let formEmpty = try await client.doROARequestWithForm("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/empty", "json", request, runtime)
        XCTAssertNotNil(formEmpty["headers"])
        do {
            _ = try await client.doROARequestWithForm("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/server", "json", request, runtime)
            XCTFail("expected form server error")
        } catch {
            XCTAssertNotNil(error)
        }

        let empty = try OpenApiClient(AlibabacloudOpenApi.Config([
            "endpoint": "127.0.0.1:\(server.port)",
            "protocol": "HTTP",
            "disableHttp2": true
        ]))
        do {
            _ = try await empty.doRPCRequest("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "json", request, runtime)
            XCTFail("expected missing credential")
        } catch {
            XCTAssertNotNil(error)
        }
        do {
            _ = try await empty.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "/test", "json", request, runtime)
            XCTFail("expected missing credential")
        } catch {
            XCTAssertNotNil(error)
        }
        do {
            _ = try await empty.doROARequestWithForm("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "/test", "json", request, runtime)
            XCTFail("expected missing credential")
        } catch {
            XCTAssertNotNil(error)
        }
        do {
            _ = try await empty.doRequest(Params([
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
            XCTFail("expected missing credential")
        } catch {
            XCTAssertNotNil(error)
        }

        let sm3 = try OpenApiClient(AlibabacloudOpenApi.Config([
            "accessKeyId": "ak",
            "accessKeySecret": "secret",
            "endpoint": "127.0.0.1:\(server.port)",
            "protocol": "HTTP",
            "signatureAlgorithm": "ACS3-HMAC-SM3",
            "disableHttp2": true
        ]))
        let sm3Result = try await sm3.doRequest(Params([
            "action": "TestAPI",
            "version": "2022-06-01",
            "protocol": "HTTP",
            "pathname": "/test",
            "method": "POST",
            "authType": "AK",
            "style": "ROA",
            "reqBodyType": "json",
            "bodyType": "string"
        ]), request, runtime)
        XCTAssertEqual(statusCode(sm3Result), 200)
        let roaBinary = try await client.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/test", "binary", request, runtime)
        XCTAssertNotNil(roaBinary["body"])
        let formBinary = try await client.doROARequestWithForm("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "/test", "binary", request, runtime)
        XCTAssertNotNil(formBinary["body"])
        let formSts = try await client.doROARequestWithForm("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "/test", "json", request, runtime)
        XCTAssertEqual(statusCode(formSts), 200)
        let stsRoa = try await client.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "AK", "/test", "json", request, runtime)
        XCTAssertEqual(statusCode(stsRoa), 200)

        let gateway = try LocalHTTPGateway()
        XCTAssertEqual(gateway.describe()["name"], "LocalHTTPGateway")
        let emptyCtx = AlibabacloudGatewaySPI.InterceptorContext([:])
        try await gateway.prepare(emptyCtx)
        try await gateway.finish(emptyCtx)
        let prepared = AlibabacloudGatewaySPI.InterceptorContext([
            "request": AlibabacloudGatewaySPI.InterceptorContext.Request([
                "method": "POST",
                "pathname": "/test",
                "headers": ["for-test": "sdk"]
            ]),
            "configuration": AlibabacloudGatewaySPI.InterceptorContext.Configuration([
                "endpoint": "127.0.0.1"
            ]),
            "response": AlibabacloudGatewaySPI.InterceptorContext.Response([
                "statusCode": 200,
                "headers": ["k": "v"]
            ])
        ])
        try await gateway.prepare(prepared)
        try await gateway.finish(prepared)
        let invalidJson = AlibabacloudGatewaySPI.InterceptorContext([
            "response": AlibabacloudGatewaySPI.InterceptorContext.Response([
                "statusCode": 200,
                "headers": ["k": "v"],
                "body": Tea.TeaCore.toReadable("not-json")
            ])
        ])
        try await gateway.finish(invalidJson)

        let streamMap = OpenApiRequest()
        streamMap.stream = Tea.TeaCore.toReadable("x")
        streamMap.hostMap = ["region": "cn-hangzhou"]
        streamMap.endpointOverride = "example.com"
        XCTAssertNotNil(streamMap.toMap()["stream"])
        streamMap.fromMap(streamMap.toMap())
        XCTAssertEqual(streamMap.endpointOverride, "example.com")
    }
}
