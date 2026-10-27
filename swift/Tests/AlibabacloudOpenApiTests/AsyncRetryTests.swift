import Foundation
import XCTest
import Tea
import TeaUtils
@testable import AlibabacloudOpenApi

final class AsyncRetryTests: XCTestCase {
    func testAllRequestPathsWaitBeforeRetryingTransportFailures() async throws {
        let server = TinyHTTPServer()
        defer { server.stop() }
        let client = try Client(Config([
            "accessKeyId": "ak", "accessKeySecret": "secret",
            "endpoint": "127.0.0.1:\(server.port)", "protocol": "HTTP",
            "signatureAlgorithm": "ACS3-HMAC-SHA256", "disableHttp2": true
        ]))
        try client.setGatewayClient(LocalHTTPGateway())
        let params = Params([
            "action": "TestAPI", "version": "2022-06-01", "protocol": "HTTP",
            "method": "POST", "pathname": "/", "style": "RPC",
            "authType": "Anonymous", "bodyType": "json", "reqBodyType": "json"
        ])
        let operations: [(String, (OpenApiRequest, RuntimeOptions) async throws -> [String: Any])] = [
            ("RPC", { request, runtime in
                try await client.doRPCRequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "json", request, runtime)
            }),
            ("ROA", { request, runtime in
                try await client.doROARequest("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/", "json", request, runtime)
            }),
            ("ROA form", { request, runtime in
                try await client.doROARequestWithForm("TestAPI", "2022-06-01", "HTTP", "POST", "Anonymous", "/", "json", request, runtime)
            }),
            ("ACS3", { request, runtime in
                try await client.doRequest(params, request, runtime)
            }),
            ("gateway", { request, runtime in
                try await client.execute(params, request, runtime)
            })
        ]
        for (name, operation) in operations {
            let request = OpenApiRequest([
                "headers": ["x-disconnect": "1"], "body": [:] as [String: Any]
            ])
            request.stream = TeaCore.toReadable("{}")
            let runtime = RuntimeOptions([
                "autoretry": true, "max_attempts": 1,
                "backoff_policy": "fixed", "backoff_period": 1,
                "connectTimeout": 1000, "readTimeout": 1000
            ])
            let started = DispatchTime.now().uptimeNanoseconds
            do {
                _ = try await operation(request, runtime)
                XCTFail("\(name) should exhaust its retries after the server disconnects")
            } catch let error as UnretryableError {
                XCTAssertTrue(error.couse is RetryableError, "\(name) should preserve the transport error")
            } catch {
                XCTFail("\(name) threw an unexpected error: \(error)")
            }
            let elapsed = DispatchTime.now().uptimeNanoseconds - started
            XCTAssertGreaterThanOrEqual(elapsed, 1_000_000_000, "\(name) should wait before retrying")
        }
    }
}
