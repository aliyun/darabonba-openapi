import XCTest
@testable import AlibabacloudOpenApi

class OpenApiRequestTests: XCTestCase {

    // MARK: - body 字段 fromMap 修复验证

    /// 测试：模拟产品包真实调用 OpenApiRequest(["query": ...])，body 不应被污染
    func testInit_withDictWithoutBody_bodyShouldBeNil() {
        let req = OpenApiRequest([
            "query": ["Action": "DescribeRegions"]
        ])

        XCTAssertNil(req.body,
                     "通过 init(dict) 创建且不含 body key 时，body 必须为 nil")
    }

    /// 测试：dict 中不包含 "body" key 时，body 应保持 nil，不被错误赋值
    func testFromMap_bodyKeyNotPresent_bodyShouldRemainNil() {
        let request = OpenApiRequest()
        let dict: [String: Any?] = [
            "headers": ["Content-Type": "application/json"],
            "query": ["page": "1"]
        ]
        request.fromMap(dict)

        XCTAssertNil(request.body, "当 dict 中不含 'body' key 时，body 应保持 nil")
        // 同时验证其他字段正常赋值
        XCTAssertEqual(request.headers, ["Content-Type": "application/json"])
        XCTAssertEqual(request.query, ["page": "1"])
    }

    /// 测试：dict 中包含 "body" key 且值非 nil 时，body 应被正确赋值
    func testFromMap_bodyKeyPresent_withNonNilValue() {
        let request = OpenApiRequest()
        let bodyContent: [String: Any] = ["key": "value", "number": 42]
        let dict: [String: Any?] = [
            "body": bodyContent
        ]
        request.fromMap(dict)

        XCTAssertNotNil(request.body, "当 dict 含 'body' key 且值非 nil 时，body 应被赋值")
        let result = request.body as? [String: Any]
        XCTAssertEqual(result?["key"] as? String, "value")
        XCTAssertEqual(result?["number"] as? Int, 42)
    }

    /// 测试：dict 中包含 "body" key 且值为字符串时，body 应被正确赋值
    func testFromMap_bodyKeyPresent_withStringValue() {
        let request = OpenApiRequest()
        let dict: [String: Any?] = [
            "body": "hello body"
        ]
        request.fromMap(dict)

        XCTAssertNotNil(request.body)
        XCTAssertEqual(request.body as? String, "hello body")
    }

    /// 测试：已有 body 值的 request，fromMap 传入不含 "body" 的 dict 后，body 不应被覆盖
    func testFromMap_existingBody_notOverwrittenWhenKeyAbsent() {
        let request = OpenApiRequest()
        request.body = "existing body"

        let dict: [String: Any?] = [
            "headers": ["X-Custom": "header"]
        ]
        request.fromMap(dict)

        XCTAssertEqual(request.body as? String, "existing body",
                       "当 dict 中不含 'body' key 时，已有的 body 值不应被覆盖")
    }

    /// 测试：空 dict 时所有字段保持默认值
    func testFromMap_emptyDict_allFieldsRemainNil() {
        let request = OpenApiRequest()
        let dict: [String: Any?] = [:]
        request.fromMap(dict)

        XCTAssertNil(request.headers)
        XCTAssertNil(request.query)
        XCTAssertNil(request.body)
        XCTAssertNil(request.stream)
        XCTAssertNil(request.hostMap)
        XCTAssertNil(request.endpointOverride)
    }

    /// 测试：传入 nil dict 时不崩溃，所有字段保持默认值
    func testFromMap_nilDict_noChange() {
        let request = OpenApiRequest()
        request.body = "should remain"
        request.fromMap(nil)

        XCTAssertEqual(request.body as? String, "should remain")
    }

    // MARK: - toMap 验证

    /// 测试：body 为 nil 时，toMap 结果中不应包含 "body" key
    func testToMap_bodyNil_shouldNotContainBodyKey() {
        let request = OpenApiRequest()
        request.headers = ["X-Key": "value"]
        // body 保持 nil

        let map = request.toMap()
        XCTAssertFalse(map.keys.contains("body"), "body 为 nil 时 toMap 不应包含 body key")
        XCTAssertNotNil(map["headers"])
    }

    /// 测试：body 非 nil 时，toMap 结果中应包含 "body" key
    func testToMap_bodyNonNil_shouldContainBodyKey() {
        let request = OpenApiRequest()
        request.body = ["data": "test"]

        let map = request.toMap()
        XCTAssertTrue(map.keys.contains("body"))
    }

    // MARK: - 往返测试 (round-trip)

    /// 测试：toMap -> fromMap 往返一致性
    func testRoundTrip_toMapThenFromMap() {
        let original = OpenApiRequest()
        original.headers = ["Authorization": "Bearer token"]
        original.query = ["limit": "10"]
        original.body = ["action": "test"]
        original.endpointOverride = "https://custom.endpoint.com"

        let map: [String: Any?] = original.toMap()
        let restored = OpenApiRequest()
        restored.fromMap(map)

        XCTAssertEqual(restored.headers, original.headers)
        XCTAssertEqual(restored.query, original.query)
        XCTAssertNotNil(restored.body)
        XCTAssertEqual((restored.body as? [String: String])?["action"], "test")
        XCTAssertEqual(restored.endpointOverride, original.endpointOverride)
    }
}
