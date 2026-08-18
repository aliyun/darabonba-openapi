import XCTest
import Tea
@testable import AlibabacloudOpenApi

final class OpenApiErrorTests: XCTestCase {
    func testGetThrottlingTimeLeft() {
        XCTAssertNil(Client.getThrottlingTimeLeft(nil))
        XCTAssertNil(Client.getThrottlingTimeLeft([:]))
        XCTAssertNil(Client.getThrottlingTimeLeft(["x-acs-retry-after": "0"]))
        XCTAssertNil(Client.getThrottlingTimeLeft(["x-acs-retry-after": "-1"]))
        XCTAssertNil(Client.getThrottlingTimeLeft(["x-acs-retry-after": "abc"]))
        XCTAssertEqual(1200, Client.getThrottlingTimeLeft(["x-acs-retry-after": "1200"]))
        XCTAssertEqual(800, Client.getThrottlingTimeLeft(["X-Acs-Retry-After": "800"]))
        XCTAssertEqual(3, Client.getThrottlingTimeLeft(["Retry-After": "3"]))
    }

    func testContentTypeAndLooksLikeXML() {
        XCTAssertEqual("application/xml", Client.contentType(["Content-Type": "application/xml; charset=utf-8"]))
        XCTAssertEqual("text/xml", Client.contentType(["content-type": "text/xml;charset=utf-8"]))
        XCTAssertTrue(Client.looksLikeXML("application/xml"))
        XCTAssertTrue(Client.looksLikeXML("text/xml"))
        XCTAssertFalse(Client.looksLikeXML("application/json"))
        XCTAssertFalse(Client.looksLikeXML("text/plain"))
        XCTAssertEqual("", Client.contentType([:]))
        XCTAssertEqual("x", Client.headerValue(["X-Foo": "x"], "x-foo"))
    }

    func testParseErrorMapJSON() {
        let body = #"{"Code":"InvalidParameter","Message":"bad","RequestId":"rid-1"}"#.data(using: .utf8)
        let err = Client.parseErrorMap(statusCode: 400, headers: ["content-type": "application/json"], body: body)
        XCTAssertEqual("InvalidParameter", err["Code"] as? String)
        XCTAssertEqual("rid-1", err["RequestId"] as? String)
    }

    func testParseErrorMapJSONWithCharset() {
        let body = #"{"code":"Forbidden","message":"nope"}"#.data(using: .utf8)
        let err = Client.parseErrorMap(statusCode: 403, headers: ["Content-Type": "application/json; charset=utf-8"], body: body)
        XCTAssertEqual("Forbidden", err["code"] as? String)
    }

    func testParseErrorMapXMLVariants() {
        let xml = """
        <Error>
          <Code>InvalidAccessKeyId</Code>
          <Message>specified access key is not found</Message>
          <RequestId>xml-rid</RequestId>
        </Error>
        """
        let body = xml.data(using: .utf8)
        let types = [
            "text/xml;charset=utf-8",
            "text/xml; charset=utf-8",
            "application/xml",
            "APPLICATION/XML; charset=UTF-8"
        ]
        for mime in types {
            let err = Client.parseErrorMap(statusCode: 400, headers: ["content-type": mime], body: body)
            XCTAssertEqual("InvalidAccessKeyId", err["Code"] as? String, mime)
            XCTAssertEqual("xml-rid", err["RequestId"] as? String, mime)
            XCTAssertEqual(xml, err["rawBody"] as? String, mime)
        }

        let lower = """
        <error>
          <code>LowerError</code>
          <message>oops</message>
        </error>
        """
        let lowerErr = Client.parseErrorMap(
            statusCode: 400,
            headers: ["content-type": "application/xml"],
            body: lower.data(using: .utf8)
        )
        XCTAssertEqual("LowerError", lowerErr["code"] as? String)

        let root = """
        <Response>
          <Code>RootCode</Code>
        </Response>
        """
        let rootErr = Client.parseErrorMap(
            statusCode: 400,
            headers: ["content-type": "text/xml"],
            body: root.data(using: .utf8)
        )
        if let response = rootErr["Response"] as? [String: Any] {
            XCTAssertEqual("RootCode", response["Code"] as? String)
        } else {
            XCTAssertEqual("RootCode", rootErr["Code"] as? String)
        }
        XCTAssertEqual(root, rootErr["rawBody"] as? String)
    }

    func testParseErrorMapRawFallback() {
        let raw = "<html>proxy error</html>"
        let err = Client.parseErrorMap(
            statusCode: 502,
            headers: ["content-type": "text/html"],
            body: raw.data(using: .utf8)
        )
        XCTAssertEqual("FailedToParseResponse", err["code"] as? String)
        XCTAssertEqual(raw, err["message"] as? String)
        XCTAssertEqual(raw, err["rawBody"] as? String)
        XCTAssertEqual(502, err["statusCode"] as? Int)
    }

    func testParseErrorMapMalformedJSONFallsBack() {
        let err = Client.parseErrorMap(
            statusCode: 500,
            headers: ["content-type": "application/json"],
            body: "not-json".data(using: .utf8)
        )
        XCTAssertEqual("FailedToParseResponse", err["code"] as? String)
        XCTAssertEqual("not-json", err["rawBody"] as? String)
    }

    func testMakeOpenApiErrorClientServerThrottling() {
        let client = Client.makeOpenApiError(
            statusCode: 400,
            headers: [:],
            err: ["Code": "InvalidParameter", "Message": "bad", "RequestId": "c1"]
        )
        XCTAssertTrue(client is ClientException)
        XCTAssertEqual("ClientException", client.getName())
        XCTAssertEqual(400, client.getStatusCode())
        XCTAssertEqual("c1", client.requestId)
        XCTAssertFalse(client.message?.contains("Optional") ?? true)

        let server = Client.makeOpenApiError(
            statusCode: 500,
            headers: [:],
            err: ["code": "InternalError", "message": "oops", "requestId": "s1"]
        )
        XCTAssertTrue(server is ServerException)
        XCTAssertEqual("ServerException", server.getName())
        XCTAssertTrue(Tea.TeaCore.isRetryable(server))

        let throttlingHeader = Client.makeOpenApiError(
            statusCode: 400,
            headers: ["x-acs-retry-after": "1500"],
            err: ["Code": "Throttling", "Message": "slow", "RequestId": "t1"]
        )
        XCTAssertTrue(throttlingHeader is ThrottlingException)
        XCTAssertEqual(1500, throttlingHeader.getRetryAfter())
        XCTAssertTrue(Tea.TeaCore.isRetryable(throttlingHeader))

        let throttling429 = Client.makeOpenApiError(
            statusCode: 429,
            headers: [:],
            err: ["Code": "TooManyRequests"]
        )
        XCTAssertTrue(throttling429 is ThrottlingException)

        let throttlingCode = Client.makeOpenApiError(
            statusCode: 400,
            headers: [:],
            err: ["Code": "Throttling.User"]
        )
        XCTAssertTrue(throttlingCode is ThrottlingException)
    }

    func testGetAccessDeniedDetail() throws {
        let client = try Client(Config([
            "accessKeyId": "ak",
            "accessKeySecret": "sk",
            "endpoint": "ecs.aliyuncs.com"
        ]))
        XCTAssertNil(client.getAccessDeniedDetail(nil))
        XCTAssertEqual(
            "ram:ListUsers",
            client.getAccessDeniedDetail(["AccessDeniedDetail": ["AuthAction": "ram:ListUsers"]])?["AuthAction"] as? String
        )
        XCTAssertEqual(
            "ImplicitDeny",
            client.getAccessDeniedDetail(["accessDeniedDetail": ["NoPermissionType": "ImplicitDeny"]])?["NoPermissionType"] as? String
        )
        XCTAssertNil(client.getAccessDeniedDetail(["foo": "bar"]))
    }

    func testStringify() {
        XCTAssertEqual("", Client.stringify(nil))
        XCTAssertEqual("", Client.stringify(NSNull()))
        XCTAssertEqual("abc", Client.stringify("abc"))
        XCTAssertEqual("12", Client.stringify(12))
    }

    func testMakeOpenApiErrorAccessDenied() {
        let err = Client.makeOpenApiError(
            statusCode: 403,
            headers: [:],
            err: [
                "Code": "Forbidden",
                "AccessDeniedDetail": ["AuthAction": "ram:ListUsers"]
            ]
        )
        XCTAssertTrue(err is ClientException)
        XCTAssertEqual("ram:ListUsers", err.accessDeniedDetail?["AuthAction"] as? String)
    }

    func testThrowIfError() throws {
        let client = try Client(Config([
            "accessKeyId": "ak",
            "accessKeySecret": "sk",
            "endpoint": "ecs.aliyuncs.com"
        ]))
        let ok = Tea.TeaResponse(statusCode: 200, headers: [:], body: nil)
        XCTAssertNoThrow(try client.throwIfError(ok))

        let clientErr = Tea.TeaResponse(
            statusCode: 400,
            headers: ["content-type": "application/json"],
            body: #"{"Code":"InvalidParameter","Message":"bad","RequestId":"rid"}"#.data(using: .utf8)
        )
        XCTAssertThrowsError(try client.throwIfError(clientErr)) { error in
            XCTAssertTrue(error is ClientException)
        }

        let serverErr = Tea.TeaResponse(
            statusCode: 500,
            headers: ["content-type": "application/json"],
            body: #"{"Code":"InternalError","Message":"oops"}"#.data(using: .utf8)
        )
        XCTAssertThrowsError(try client.throwIfError(serverErr)) { error in
            XCTAssertTrue(error is ServerException)
        }
    }

    func testExceptionHierarchy() {
        XCTAssertTrue(ClientException(["code": "x"]) is AlibabaCloudException)
        XCTAssertTrue(ServerException(["code": "x"]) is AlibabaCloudException)
        XCTAssertTrue(ThrottlingException(["code": "x"]) is AlibabaCloudException)
        XCTAssertEqual("AlibabaCloudException", AlibabaCloudException(["code": "x"]).getName())
    }
}
