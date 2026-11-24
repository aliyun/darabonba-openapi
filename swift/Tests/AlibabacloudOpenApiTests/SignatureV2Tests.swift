import Foundation
import XCTest
import Tea
import TeaUtils
#if canImport(CryptoKit)
import CryptoKit
#else
import CryptoSwift
#endif
@testable import AlibabacloudOpenApi

final class SignatureV2Tests: XCTestCase {
    func testV2AlgorithmsAndHeaders() async throws {
        for requestedAlgorithm in ["HMAC-SHA256", "HMAC-SHA1"] {
            for style in ["RPC", "ROA"] {
                for requestBodyType in ["json", "formData"] {
                    let server = try HTTPTestServer()
                    defer { server.stop() }
                    let config = Config(["accessKeyId": "ak", "accessKeySecret": "secret", "securityToken": "token", "endpoint": "127.0.0.1:\(server.port)", "protocol": "http", "signatureAlgorithm": "v2"])
                    let global = requestedAlgorithm == "HMAC-SHA1" ? ["x-acs-signature-method": "HMAC-SHA256"] : [:]
                    config.globalParameters = GlobalParameters(["headers": global])
                    let runtime = RuntimeOptions(["autoretry": false])
                    runtime.extendsParameters = ExtendsParameters(["headers": global])
                    let headers = requestedAlgorithm == "HMAC-SHA1" ? ["x-acs-signature-method": "HMAC-SHA1"] : [:]
                    let request = OpenApiRequest(["headers": headers, "query": ["special": "中文/%!~ +@", "empty": ""]])
                    let params = Params(["action": "TestAPI", "version": "2022-06-01", "protocol": "http", "pathname": "/test", "method": "GET", "authType": "AK", "style": style, "reqBodyType": requestBodyType, "bodyType": "string"])
                    let result = try await Client(config).callApi(params, request, runtime)
                    let captured = try XCTUnwrap(result["body"] as? String)
                    try verify(captured, style: style, algorithm: "HMAC-SHA256", expectedHeader: requestedAlgorithm == "HMAC-SHA1" ? "HMAC-SHA1" : "HMAC-SHA256")
                }
            }
        }
    }

    func testNonHmacAndInvalidMethods() async throws {
        for authType in ["AK", "Anonymous"] {
            let server = try HTTPTestServer()
            defer { server.stop() }
            let config = Config(["bearerToken": "token", "endpoint": "127.0.0.1:\(server.port)", "protocol": "http", "signatureAlgorithm": "v2"])
            let params = Params(["action": "TestAPI", "version": "2022-06-01", "protocol": "http", "pathname": "/test", "method": "GET", "authType": authType, "style": "ROA", "reqBodyType": "json", "bodyType": "string"])
            let result = try await Client(config).callApi(params, OpenApiRequest(["headers": ["x-acs-signature-method": "custom"]]), RuntimeOptions(["autoretry": false]))
            let request = try XCTUnwrap(result["body"] as? String)
            XCTAssertTrue(request.lowercased().contains("x-acs-signature-method: custom\r\n"))
        }

    }

    func testRpcQuerySignatureMethod() async throws {
        let cases: [([String: String], [String: String], [String: String], [String: String], String)] = [
            (["SignatureMethod": "HMAC-SHA1"], [:], [:], [:], "HMAC-SHA256"),
            ([:], ["SignatureMethod": "HMAC-SHA1"], [:], [:], "HMAC-SHA256"),
            ([:], [:], ["SignatureMethod": "HMAC-SHA1"], [:], "HMAC-SHA256"),
            ([:], [:], ["SignatureMethod": "HMAC-SHA1"], ["X-Acs-Signature-Method": "HMAC-SHA256"], "HMAC-SHA256")
        ]
        for (global, extended, query, headers, algorithm) in cases {
            let server = try HTTPTestServer()
            defer { server.stop() }
            let config = Config(["accessKeyId": "ak", "accessKeySecret": "secret", "securityToken": "token", "endpoint": "127.0.0.1:\(server.port)", "protocol": "http", "signatureAlgorithm": "v2"])
            config.globalParameters = GlobalParameters(["queries": global])
            let runtime = RuntimeOptions(["autoretry": false])
            runtime.extendsParameters = ExtendsParameters(["queries": extended])
            let params = Params(["action": "TestAPI", "version": "2022-06-01", "protocol": "http", "pathname": "/", "method": "GET", "authType": "AK", "style": "RPC", "reqBodyType": "json", "bodyType": "string"])
            let result = try await Client(config).callApi(params, OpenApiRequest(["query": query, "headers": headers]), runtime)
            try verify(try XCTUnwrap(result["body"] as? String), style: "RPC", algorithm: algorithm)
        }
    }

    func testRpcBodySignatureMethodPreservedWithSha256() async throws {
        let server = try HTTPTestServer()
        defer { server.stop() }
        let config = Config(["accessKeyId": "ak", "accessKeySecret": "secret", "endpoint": "127.0.0.1:\(server.port)", "protocol": "http", "signatureAlgorithm": "v2"])
        let params = Params(["action": "TestAPI", "version": "2022-06-01", "protocol": "http", "pathname": "/", "method": "POST", "authType": "AK", "style": "RPC", "reqBodyType": "formData", "bodyType": "string"])
        let input = ["SignatureMethod": "HMAC-SHA1"]
        let result = try await Client(config).callApi(params, OpenApiRequest(["body": input]), RuntimeOptions(["autoretry": false]))
        let request = try XCTUnwrap(result["body"] as? String)
        let transmittedBody = try requestBody(request)
        XCTAssertTrue(transmittedBody.contains("SignatureMethod=HMAC-SHA1"))
        XCTAssertEqual(input["SignatureMethod"], "HMAC-SHA1")
        try verify(request, style: "RPC", algorithm: "HMAC-SHA256", body: ["SignatureMethod": "HMAC-SHA1"])
    }

    func testRequestBodyParsesChunkedAndContentLength() throws {
        let chunked = "POST / HTTP/1.1\r\nTransfer-Encoding: chunked\r\n\r\n9\r\nSignature\r\na\r\nMethod=HMA\r\n6\r\nC-SHA1\r\n0\r\n\r\n"
        XCTAssertEqual(try requestBody(chunked), "SignatureMethod=HMAC-SHA1")
        let unchunked = "POST / HTTP/1.1\r\nContent-Length: 3\r\n\r\nabc"
        XCTAssertEqual(try requestBody(unchunked), "abc")
    }

    private func requestBody(_ request: String) throws -> String {
        let boundary = try XCTUnwrap(request.range(of: "\r\n\r\n"))
        let header = request[..<boundary.lowerBound].lowercased()
        let body = Data(request[boundary.upperBound...].utf8)
        guard header.contains("transfer-encoding: chunked") else { return String(decoding: body, as: UTF8.self) }
        let newline = Data("\r\n".utf8)
        var position = 0
        var decoded = Data()
        while position < body.count {
            let lineEnd = try XCTUnwrap(body.range(of: newline, in: position..<body.count))
            let line = String(decoding: body[position..<lineEnd.lowerBound], as: UTF8.self)
            let sizeText = try XCTUnwrap(line.split(separator: ";", maxSplits: 1).first)
            let size = try XCTUnwrap(Int(sizeText, radix: 16))
            guard size >= 0 else { throw NSError(domain: "InvalidChunkedBody", code: 4) }
            position = lineEnd.upperBound
            if size == 0 { return String(decoding: decoded, as: UTF8.self) }
            guard size <= body.count - position, body.count - position - size >= 2 else {
                throw NSError(domain: "InvalidChunkedBody", code: 1)
            }
            decoded.append(body[position..<(position + size)])
            position += size
            guard body[position..<(position + 2)] == newline else { throw NSError(domain: "InvalidChunkedBody", code: 2) }
            position += 2
        }
        throw NSError(domain: "InvalidChunkedBody", code: 3)
    }

    private func verify(_ request: String, style: String, algorithm: String, body: [String: String] = [:], expectedHeader: String? = nil) throws {
        let lines = request.components(separatedBy: "\r\n")
        let first = lines[0].split(separator: " ")
        let components = try XCTUnwrap(URLComponents(string: "http://localhost" + String(first[1])))
        let items = components.queryItems ?? []
        let query = Dictionary(uniqueKeysWithValues: items.map { ($0.name, $0.value ?? "") })
        if style == "RPC" {
            XCTAssertEqual(query["SignatureMethod"], algorithm)
            var signed = query
            let signature = signed.removeValue(forKey: "Signature")
            signed.merge(body) { _, new in new }
            var canonical = ""
            var separator = ""
            for key in signed.keys.sorted() {
                canonical += separator
                if let value = signed[key], !value.isEmpty { canonical += encode(key) + "=" + encode(value) }
                separator = "&"
            }
            XCTAssertEqual(signature, try hmac(String(first[0]) + "&%2F&" + encode(canonical), key: "secret&", algorithm: algorithm))
        } else {
            var headers: [String: String] = [:]
            for line in lines.dropFirst() {
                let pair = line.split(separator: ":", maxSplits: 1, omittingEmptySubsequences: false)
                if pair.count == 2 { headers[String(pair[0]).lowercased()] = String(pair[1]).trimmingCharacters(in: .whitespaces) }
            }
            XCTAssertEqual(headers["x-acs-signature-method"], expectedHeader ?? algorithm)
            let canonicalHeaders = headers.keys.filter { $0.hasPrefix("x-acs-") }.sorted().map { $0 + ":" + (headers[$0] ?? "") + "\n" }.joined()
            let resource = components.path + (query.isEmpty ? "" : "?" + query.keys.sorted().map { key in query[key]!.isEmpty ? key : key + "=" + query[key]! }.joined(separator: "&"))
            let signed = ["GET", headers["accept"] ?? "", headers["content-md5"] ?? "", headers["content-type"] ?? "", headers["date"] ?? ""].joined(separator: "\n") + "\n" + canonicalHeaders + resource
            XCTAssertEqual(headers["authorization"], "acs ak:" + (try hmac(signed, key: "secret", algorithm: algorithm)))
        }
    }

    private func encode(_ value: String) -> String {
        let allowed = NSMutableCharacterSet.alphanumeric()
        allowed.addCharacters(in: "*-._ ")
        return (value.addingPercentEncoding(withAllowedCharacters: allowed as CharacterSet) ?? "").replacingOccurrences(of: " ", with: "%20").replacingOccurrences(of: "+", with: "%20").replacingOccurrences(of: "*", with: "%2A").replacingOccurrences(of: "%7E", with: "~")
    }

    private func hmac(_ value: String, key: String, algorithm: String) throws -> String {
        #if canImport(CryptoKit)
        let key = SymmetricKey(data: Data(key.utf8))
        if algorithm == "HMAC-SHA1" { return Data(HMAC<Insecure.SHA1>.authenticationCode(for: Data(value.utf8), using: key)).base64EncodedString() }
        return Data(HMAC<SHA256>.authenticationCode(for: Data(value.utf8), using: key)).base64EncodedString()
        #else
        return Data(try HMAC(key: key, variant: algorithm == "HMAC-SHA1" ? .sha1 : .sha2(.sha256)).authenticate(Array(value.utf8))).base64EncodedString()
        #endif
    }
}
