import Foundation
import XCTest
import TeaUtils
@testable import AlibabacloudOpenApi

final class HTTPTests: XCTestCase {
    func testCallApiSendsACS3AndRPCSignatures() async throws {
        for algorithm in ["ACS3-HMAC-SHA256", "v2"] {
            let server = try HTTPTestServer()
            defer { server.stop() }

            let response = try await callApi(server, algorithm: algorithm, bodyType: "string")
            XCTAssertEqual(response["statusCode"] as? Int32, 200)
            let request = try XCTUnwrap(response["body"] as? String)
            XCTAssertTrue(request.hasPrefix("GET /?"))
            XCTAssertTrue(request.lowercased().contains("x-acs-action: describeregions\r\n"))
            XCTAssertFalse(request.contains("mock-secret"))
            if algorithm == "v2" {
                let target = try XCTUnwrap(request.split(separator: " ").dropFirst().first)
                let query = try XCTUnwrap(URLComponents(string: "http://localhost\(target)")?.queryItems)
                XCTAssertEqual(query.first { $0.name == "SignatureMethod" }?.value, "HMAC-SHA1")
                XCTAssertEqual(query.first { $0.name == "AccessKeyId" }?.value, "mock-id")
                let signature = try XCTUnwrap(query.first { $0.name == "Signature" }?.value)
                XCTAssertEqual(Data(base64Encoded: signature)?.count, 20)
            } else {
                XCTAssertNotNil(request.lowercased().range(
                    of: "authorization: acs3-hmac-sha256 credential=mock-id,signedheaders=[^\\r\\n]+,signature=[a-f0-9]{64}\\r\\n",
                    options: .regularExpression))
                XCTAssertTrue(request.lowercased().contains("x-acs-content-sha256:"))
            }
        }
    }

    func testCallApiParsesJSONResponse() async throws {
        let server = try HTTPTestServer(body: """
            {"RequestId":"mock-request","Regions":{"Region":[{"RegionId":"cn-hangzhou"}]}}
            """)
        defer { server.stop() }

        let response = try await callApi(server, bodyType: "json")
        XCTAssertEqual(response["statusCode"] as? Int32, 200)
        let body = try XCTUnwrap(response["body"] as? [String: Any])
        XCTAssertEqual(body["RequestId"] as? String, "mock-request")
        let regions = try XCTUnwrap(body["Regions"] as? [String: Any])
        XCTAssertEqual((regions["Region"] as? [[String: String]])?.first?["RegionId"], "cn-hangzhou")
    }

    private func callApi(_ server: HTTPTestServer, algorithm: String = "ACS3-HMAC-SHA256",
                         bodyType: String) async throws -> [String: Any] {
        let client = try Client(Config([
            "accessKeyId": "mock-id", "accessKeySecret": "mock-secret",
            "endpoint": "127.0.0.1:\(server.port)", "protocol": "http",
            "signatureAlgorithm": algorithm
        ]))
        return try await client.callApi(Params([
            "action": "DescribeRegions", "version": "2014-05-26", "protocol": "http",
            "method": "GET", "pathname": "/", "style": "RPC", "authType": "AK",
            "bodyType": bodyType, "reqBodyType": "formData"
        ]), OpenApiRequest(["query": ["RegionId": "cn-hangzhou"]]), RuntimeOptions([
            "autoretry": false, "connectTimeout": 2000, "readTimeout": 2000
        ]))
    }
}
