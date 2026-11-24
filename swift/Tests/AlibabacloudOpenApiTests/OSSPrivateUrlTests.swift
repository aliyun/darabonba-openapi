import Foundation
import XCTest
import Tea
import TeaUtils
import TeaFileForm
@testable import AlibabacloudOpenApi

final class OSSPrivateUrlTests: XCTestCase {
    private let binary = Data((0..<131073).map { UInt8($0 % 256) })

    func testConfigAndRuntimePriority() throws {
        let unset = Config()
        XCTAssertNil(unset.ossPrivateUrl)
        XCTAssertNil(unset.toMap()["ossPrivateUrl"])
        for url in ["", "http://localhost:8080/prefix", "https://oss-private.example:8443/"] {
            let config = Config(["ossPrivateUrl": url, "readTimeout": 1234])
            XCTAssertEqual(config.ossPrivateUrl, url)
            XCTAssertEqual(Config(config.toMap()).ossPrivateUrl, url)
            XCTAssertEqual(Config(config.toMap()).readTimeout, 1234)
        }
        let client = try Client(Config(["ossPrivateUrl": "http://config.example/prefix"]))
        let runtime = RuntimeOptions()
        XCTAssertEqual(client.getOSSPrivateUrl(runtime), "http://config.example/prefix")
        runtime.ossPrivateUrl = ""
        XCTAssertEqual(client.getOSSPrivateUrl(runtime), "http://config.example/prefix")
        runtime.ossPrivateUrl = "https://runtime.example:8443/"
        XCTAssertEqual(client.getOSSPrivateUrl(runtime), runtime.ossPrivateUrl)
        XCTAssertEqual(try Client(Config()).getOSSPrivateUrl(RuntimeOptions()), "")
    }

    func testRequestPreservesSchemeHostPortAndPath() throws {
        let client = try Client(Config())
        for (url, scheme, host, path) in [
            ("http://localhost:8080", "http", "localhost:8080", "/bucket/"),
            ("https://oss-private.example:8443/prefix/", "https", "oss-private.example:8443", "/prefix/bucket/"),
            ("https://oss-private.example/prefix", "https", "oss-private.example", "/prefix/bucket/"),
            ("http://[::1]:8080/a%20b", "http", "[::1]:8080", "/a%20b/bucket/")
        ] {
            let request = try client.createOSSPrivateRequest(url, "bucket", "test-boundary")
            XCTAssertEqual(request.protocol_, scheme)
            XCTAssertEqual(request.method, "POST")
            XCTAssertEqual(request.headers["host"], host)
            XCTAssertEqual(request.pathname, path)
            XCTAssertFalse(request.headers["host"]!.hasPrefix("bucket."))
            XCTAssertNotNil(request.headers["date"])
            XCTAssertNotNil(request.headers["user-agent"])
        }
    }

    func testInvalidURLsFailBeforeTransport() async throws {
        let client = try Client(Config())
        for url in ["", "not a url", "//example.test", "ftp://example.test", "http:///missing-host",
                    "https://", "https://:443/", "http:/example.test", "https://user:pass@example.test",
                    "https://@example.test", "https://example.test/path@name", "https://example.test?",
                    "https://example.test?key=value", "https://example.test#", "https://example.test#fragment"] {
            do {
                _ = try await client.postOSSObjectWithURL(url, "bucket", [:], RuntimeOptions())
                XCTFail("应拒绝非法 URL：\(url)")
            } catch let error as Tea.ReuqestError {
                XCTAssertEqual(error.code, "InvalidOSSPrivateUrl", url)
            }
        }
    }

    func testHTTPUploadPreservesMultipartBinaryWithDefaultOptions() async throws {
        let expectedBinary = binary
        // The server checks the full body before sending the response awaited below.
        let server = try HTTPTestServer(body: "<Upload><ETag>mock-etag</ETag></Upload>", contentType: "application/xml") { headers, content in
            XCTAssertTrue(headers.hasPrefix("POST /prefix/bucket/ HTTP/1.1"))
            XCTAssertTrue(headers.lowercased().contains("host: 127.0.0.1:"))
            let type = headers.components(separatedBy: "\r\n").first { $0.lowercased().hasPrefix("content-type:") } ?? ""
            let boundary = type.components(separatedBy: "boundary=").last ?? ""
            XCTAssertFalse(boundary.isEmpty)
            XCTAssertTrue(content.starts(with: Data("--\(boundary)\r\n".utf8)))
            XCTAssertTrue(content.suffix(Data("--\(boundary)--\r\n".utf8).count).elementsEqual(Data("--\(boundary)--\r\n".utf8)))
            let keyPart = try? XCTUnwrap(content.range(of: Data("name=\"key\"\r\n\r\nobject.bin\r\n".utf8)))
            let filePart = try? XCTUnwrap(content.range(of: Data("name=\"file\"; filename=\"dir/file.bin\"".utf8)))
            if let keyPart = keyPart, let filePart = filePart {
                XCTAssertLessThan(keyPart.lowerBound, filePart.lowerBound)
            }
            let marker = Data("Content-Type: application/octet-stream\r\n\r\n".utf8)
            if let start = content.range(of: marker), let end = content.range(of: Data("\r\n--\(boundary)".utf8), in: start.upperBound..<content.count) {
                XCTAssertEqual(Data(content[start.upperBound..<end.lowerBound]), expectedBinary)
            } else {
                XCTFail("未收到完整二进制文件字段")
            }
        }
        defer { server.stop() }
        let client = try Client(Config(["ossPrivateUrl": "http://127.0.0.1:\(server.port)/prefix", "readTimeout": 2000]))
        let file = FileField(["filename": "dir/file.bin", "contentType": "application/octet-stream", "content": InputStream(data: binary)])
        let response = try await client.postOSSObjectWithURL(client.getOSSPrivateUrl(RuntimeOptions()), "bucket", ["key": "object.bin", "file": file], RuntimeOptions())
        XCTAssertEqual(response["ETag"] as? String, "mock-etag")
    }

    func testXMLRootAndNestedErrorsPreserveDetails() async throws {
        let xml = "<Error><Code>AccessDenied</Code><Message>denied</Message><RequestId>request-id</RequestId><HostId>host-id</HostId></Error>"
        for (status, body) in [(403, xml), (500, "<Envelope>\(xml)</Envelope>"), (400, "<Error><Code>Missing</Code></Error>")] {
            let server = try HTTPTestServer(body: body, statusCode: status, contentType: "application/xml")
            defer { server.stop() }
            let client = try Client(Config())
            do {
                _ = try await client.postOSSObjectWithURL("http://127.0.0.1:\(server.port)", "bucket", ["key": "object"], RuntimeOptions())
                XCTFail("应抛出 OSS XML 错误")
            } catch let error as Tea.ReuqestError {
                XCTAssertEqual(error.statusCode, status)
                XCTAssertEqual(error.data?["httpCode"] as? Int32, Int32(status))
                XCTAssertEqual(error.code, status == 400 ? "Missing" : "AccessDenied")
                XCTAssertEqual(error.message, status == 400 ? "" : "denied")
                XCTAssertEqual(error.data?["requestId"] as? String, status == 400 ? "" : "request-id")
                XCTAssertEqual(error.data?["hostId"] as? String, status == 400 ? "" : "host-id")
            }
        }
    }

    func testUploadWaitsBeforeRetryingTransportFailure() async throws {
        let server = TinyHTTPServer()
        defer { server.stop() }
        let client = try Client(Config())
        let runtime = RuntimeOptions(["autoretry": true, "max_attempts": 1, "backoff_policy": "fixed", "backoff_period": 1,
                                      "connectTimeout": 1000, "readTimeout": 1000])
        let started = DispatchTime.now().uptimeNanoseconds
        do {
            _ = try await client.postOSSObjectWithURL("http://127.0.0.1:\(server.port)/disconnect-pvl", "bucket", ["key": "object"], runtime)
            XCTFail("断开连接应耗尽重试次数")
        } catch let error as UnretryableError {
            XCTAssertTrue(error.couse is RetryableError)
        }
        XCTAssertGreaterThanOrEqual(DispatchTime.now().uptimeNanoseconds - started, 1_000_000_000)
    }
}
