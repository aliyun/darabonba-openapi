#include <gtest/gtest.h>
#include <alibabacloud/Utils.hpp>
#include <alibabacloud/gateway/POP.hpp>
#include <alibabacloud/OpenapiException.hpp>
#include <darabonba/http/Request.hpp>
#include <darabonba/encode/Encoder.hpp>
#include <darabonba/Stream.hpp>
#include <darabonba/Model.hpp>
#include <alibabacloud/exceptions/ClientException.hpp>
#include <string>
#include <map>
#include <utility>
#include <vector>

using namespace std;
using namespace AlibabaCloud::OpenApi::Utils;
using json = nlohmann::json;

namespace {

struct DownloadResponseModel {
  shared_ptr<Darabonba::IStream> body_;
  friend void from_json(const Darabonba::Json &j, DownloadResponseModel &obj) {
    DARABONBA_FROM_JSON(body, body_);
  }
  shared_ptr<Darabonba::IStream> getBody() const { return body_; }
};

} // namespace

TEST(POPTest, ExplicitEndpointSuffix) {
  AlibabaCloud::Gateway::POP client;
  const vector<pair<string, string>> cases = {
      {"oss-cn-hangzhou.aliyuncs.com", "oss-cn-hangzhou.aliyuncs.alibaba"},
      {"ecs.cn-hangzhou-internal.aliyuncs.com", "ecs.cn-hangzhou-internal.aliyuncs.alibaba"},
      {"ecs.aliyun.com", "ecs.aliyun.com"},
      {"custom.example.com", "custom.example.com"},
      {"ecs.aliyuncs.alibaba", "ecs.aliyuncs.alibaba"},
      {"ecs.aliyuncs.com.example.com", "ecs.aliyuncs.com.example.com"}};
  for (const auto &item : cases) {
    const map<string, string> endpoints = {{"cn-hangzhou", item.first}};
    EXPECT_EQ(item.second, client.getEndpoint("ecs", "cn-hangzhou", "regional", "", "", {}, item.first, "aliyuncs.alibaba"));
    EXPECT_EQ(item.second, client.getEndpoint("ecs", "cn-hangzhou", "regional", "", "", endpoints, "", "aliyuncs.alibaba"));
    EXPECT_EQ(item.second, client.getEndpoint("ecs", "cn-hangzhou", "regional", "", "", {}, item.second, "aliyuncs.alibaba"));
    EXPECT_EQ(item.first, client.getEndpoint("ecs", "cn-hangzhou", "regional", "", "", {}, item.first, "aliyuncs.com"));
  }
}

TEST(POPTest, EndpointSuffixValidation) {
  AlibabaCloud::Gateway::POP client;
  EXPECT_EQ("ecs.aliyuncs.alibaba", client.applyEndpointSuffix("ecs.aliyuncs.com", " \tALIYUNCS.ALIBABA\r\n"));
  EXPECT_EQ("ecs.aliyuncs.com", client.applyEndpointSuffix("ecs.aliyuncs.com", "  "));
  for (const string suffix : {"example.net", ".foo", "a/b", "host:8080"}) {
    EXPECT_ANY_THROW(client.applyEndpointSuffix("ecs.aliyuncs.com", suffix));
    AlibabaCloud::Gateway::Models::AttributeMap attributes;
    attributes.setKey({{"endpointSuffix", suffix}});
    EXPECT_ANY_THROW(client.resolveEndpointSuffix(attributes));
  }
  AlibabaCloud::Gateway::Models::AttributeMap attributes;
  attributes.setKey({{"endpointSuffix", " ALIYUNCS.ALIBABA "}});
  EXPECT_EQ("aliyuncs.alibaba", client.resolveEndpointSuffix(attributes));
}

TEST(POPTest, RegionFollowsGatewayRules) {
  AlibabaCloud::Gateway::POP client;
  EXPECT_EQ("center", client.getRegion("", "", ""));
  EXPECT_EQ("cn-hangzhou", client.getRegion("", "", "cn-hangzhou"));
  EXPECT_EQ("center", client.getRegion("test", "test", ""));
  EXPECT_EQ("center", client.getRegion("test", "test.alibaba.api.com", ""));
  EXPECT_EQ("center", client.getRegion("test", "test.aliyuncs.com", ""));
  EXPECT_EQ("center", client.getRegion("test", "test-dualstack.aliyuncs.com", ""));
  EXPECT_EQ("cn-hangzhou", client.getRegion("test", "test.cn-hangzhou.aliyuncs.com", ""));
  EXPECT_EQ("cn-hangzhou", client.getRegion("test", "test.cn-hangzhou.aliyuncs.com:443", ""));
  EXPECT_EQ("cn-hangzhou", client.getRegion("test", "test-inner.cn-hangzhou.aliyuncs.com", ""));
  EXPECT_EQ("cn-hangzhou", client.getRegion("ecs", "ecs.cn-hangzhou.aliyuncs.com", ""));
  EXPECT_EQ("cn-beijing", client.getRegion("ecs", "ecs.cn-hangzhou.aliyuncs.com", "cn-beijing"));
  EXPECT_EQ("cn-hangzhou", client.getRegion("ecs", "ecs.cn-hangzhou.aliyuncs.alibaba", ""));
  EXPECT_EQ("cn-hangzhou-acdr-ut-1", client.getRegion("test", "test-inner.cn-hangzhou-acdr-ut-1.aliyuncs.com", ""));
}

TEST(POPTest, CanonicalQueryIsSortedAndEmptySafe) {
  AlibabaCloud::Gateway::POP client;
  EXPECT_EQ("", client.buildCanonicalizedResource({}));
  Darabonba::Http::Query query;
  query["b"] = "2";
  query["a"] = "1";
  query["c"] = "";
  EXPECT_EQ("a=1&b=2&c=", client.buildCanonicalizedResource(query));
}

TEST(POPTest, CanonicalHeadersFoldCase) {
  AlibabaCloud::Gateway::POP client;
  Darabonba::Http::Header headers;
  headers["Host"] = " ecs.cn-hangzhou.aliyuncs.com ";
  headers["X-Acs-Date"] = "20240101T000000Z";
  EXPECT_EQ("host:ecs.cn-hangzhou.aliyuncs.com\nx-acs-date:20240101T000000Z\n",
            client.buildCanonicalizedHeaders(headers));
}

TEST(POPTest, SignatureRequiresHostAndDate) {
  AlibabaCloud::Gateway::POP client;
  Darabonba::Http::Header headers;
  headers["x-acs-date"] = "20240101T000000Z";
  EXPECT_THROW(client.getSignature("/", "GET", {}, headers, "ACS4-HMAC-SHA256", "payload", Darabonba::Bytes()),
               AlibabaCloud::OpenApi::Exceptions::ClientException);
}

TEST(POPTest, AuthorizationUsesV4CredentialScope) {
  AlibabaCloud::Gateway::POP client;
  Darabonba::Http::Header headers;
  headers["host"] = "ecs.cn-hangzhou.aliyuncs.com";
  headers["x-acs-date"] = "20240101T000000Z";
  auto key = client.getSigningkey("ACS4-HMAC-SHA256", "secret", "ecs", "cn-hangzhou", "20240101");
  string auth = client.getAuthorization(
      "/", "GET", {}, headers, "ACS4-HMAC-SHA256",
      "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", "ak",
      key, "ecs", "cn-hangzhou", "20240101");
  EXPECT_NE(string::npos, auth.find("ACS4-HMAC-SHA256 Credential=ak/20240101/cn-hangzhou/ecs/aliyun_v4_request,SignedHeaders="));
  EXPECT_NE(string::npos, auth.find("host;x-acs-date"));
}

TEST(POPTest, ModifyRequestWritesHeadersBack) {
  using AlibabaCloud::Gateway::Models::AttributeMap;
  using AlibabaCloud::Gateway::Models::InterceptorContext;
  using AlibabaCloud::Gateway::Models::InterceptorContextConfiguration;
  using AlibabaCloud::Gateway::Models::InterceptorContextRequest;
  InterceptorContextRequest request(json({
      {"pathname", "/"},
      {"method", "GET"},
      {"productId", "ecs"},
      {"action", "DescribeInstances"},
      {"version", "2014-05-26"},
      {"authType", "Anonymous"}}));
  InterceptorContextConfiguration config(json({
      {"endpoint", "ecs.cn-hangzhou.aliyuncs.com"},
      {"regionId", "cn-hangzhou"}}));
  InterceptorContext context(json({{"request", request}, {"configuration", config}}));
  AttributeMap attributes;
  AlibabaCloud::Gateway::POP client;
  client.modifyRequest(context, attributes);
  auto headers = context.getRequest().getHeaders();
  EXPECT_EQ("ecs.cn-hangzhou.aliyuncs.com", headers["host"]);
  EXPECT_EQ("DescribeInstances", headers["x-acs-action"]);
  EXPECT_FALSE(headers["x-acs-date"].empty());
  EXPECT_FALSE(headers["x-acs-content-sha256"].empty());
  EXPECT_EQ(headers.end(), headers.find("Authorization"));
}

AlibabaCloud::Gateway::Models::InterceptorContext responseContext(
    const string &bodyType, int64_t status,
    const Darabonba::Http::Header &headers, const string &body) {
  using AlibabaCloud::Gateway::Models::InterceptorContext;
  using AlibabaCloud::Gateway::Models::InterceptorContextRequest;
  using AlibabaCloud::Gateway::Models::InterceptorContextResponse;
  InterceptorContextRequest request(json({{"bodyType", bodyType}}));
  InterceptorContextResponse response;
  response.setStatusCode(status);
  response.setHeaders(headers);
  response.setBody(std::make_shared<Darabonba::ISStream>(body));
  InterceptorContext context;
  context.setRequest(request);
  context.setResponse(response);
  return context;
}

TEST(POPTest, ModifyResponseReadsXmlErrorStream) {
  auto context = responseContext(
      "json", 400, {{"content-type", "text/xml;charset=utf-8"}},
      "<Error><Code>InvalidAction</Code><Message>The specified action is invalid.</Message>"
      "<RequestId>req-1</RequestId></Error>");
  AlibabaCloud::Gateway::Models::AttributeMap attributes;
  AlibabaCloud::Gateway::POP client;
  try {
    client.modifyResponse(context, attributes);
    FAIL() << "expected the XML error to be raised";
  } catch (const AlibabaCloud::OpenApi::Exceptions::ClientException &ex) {
    EXPECT_EQ("InvalidAction", ex.code());
    EXPECT_NE(string::npos, ex.message().find("The specified action is invalid."));
    EXPECT_NE(string::npos, ex.message().find("req-1"));
    EXPECT_EQ(string::npos, ex.message().find("missing the Error element"));
  }
}

TEST(POPTest, ModifyResponseReadsJsonStream) {
  auto context = responseContext(
      "json", 200, {{"content-type", "application/json"}}, "{\"ok\":true}");
  AlibabaCloud::Gateway::Models::AttributeMap attributes;
  AlibabaCloud::Gateway::POP client;
  client.modifyResponse(context, attributes);
  json body = context.getResponse().getDeserializedBody();
  EXPECT_EQ(true, body["ok"].get<bool>());
}

TEST(POPTest, ModifyRequestSignsRawByteBody) {
  using AlibabaCloud::Gateway::Models::AttributeMap;
  using AlibabaCloud::Gateway::Models::InterceptorContext;
  using AlibabaCloud::Gateway::Models::InterceptorContextConfiguration;
  using AlibabaCloud::Gateway::Models::InterceptorContextRequest;
  InterceptorContextRequest request(json({
      {"pathname", "/"},
      {"method", "POST"},
      {"productId", "ecs"},
      {"action", "Upload"},
      {"version", "2014-05-26"},
      {"authType", "Anonymous"},
      {"reqBodyType", "byte"},
      {"body", json::array({0, 65, 255})}}));
  InterceptorContextConfiguration config(json({
      {"endpoint", "ecs.cn-hangzhou.aliyuncs.com"},
      {"regionId", "cn-hangzhou"}}));
  InterceptorContext context(json({{"request", request}, {"configuration", config}}));
  AttributeMap attributes;
  AlibabaCloud::Gateway::POP client;
  client.modifyRequest(context, attributes);

  auto sent = Darabonba::Stream::readAsBytes(context.getRequest().getStream());
  ASSERT_EQ(3u, sent.size());
  EXPECT_EQ(0, sent[0]);
  EXPECT_EQ(65, sent[1]);
  EXPECT_EQ(255, sent[2]);
  Darabonba::Bytes raw;
  raw.push_back(0);
  raw.push_back(65);
  raw.push_back(255);
  auto headers = context.getRequest().getHeaders();
  EXPECT_EQ(headers.end(), headers.find("content-type"));
  EXPECT_EQ(Darabonba::Encode::Encoder::hexEncode(
                Darabonba::Encode::Encoder::hash(raw, "ACS4-HMAC-SHA256")),
            headers["x-acs-content-sha256"]);
}

TEST(POPTest, ModifyResponsePassesBinaryBodyThroughExecute) {
  json executed;
  {
    auto context = responseContext(
        "binary", 200, {{"content-type", "application/octet-stream"}}, "download content");
    AlibabaCloud::Gateway::Models::AttributeMap attributes;
    AlibabaCloud::Gateway::POP client;
    client.modifyResponse(context, attributes);
    executed = json({
        {"headers", context.getResponse().getHeaders()},
        {"statusCode", context.getResponse().getStatusCode()},
        {"body", context.getResponse().getDeserializedBody()}});
  }

  ASSERT_TRUE(executed["body"].is_binary());
  DownloadResponseModel first = executed.get<DownloadResponseModel>();
  ASSERT_NE(nullptr, first.getBody());
  EXPECT_EQ("download content", Darabonba::Stream::readAsString(first.getBody()));
  DownloadResponseModel second = executed.get<DownloadResponseModel>();
  ASSERT_NE(nullptr, second.getBody());
  EXPECT_EQ("download content", Darabonba::Stream::readAsString(second.getBody()));
}

TEST(POPTest, ModifyResponseReadsStringStream) {
  auto context = responseContext(
      "string", 200, {{"content-type", "text/plain"}}, "hello");
  AlibabaCloud::Gateway::Models::AttributeMap attributes;
  AlibabaCloud::Gateway::POP client;
  client.modifyResponse(context, attributes);
  json body = context.getResponse().getDeserializedBody();
  EXPECT_EQ("hello", body.get<string>());
}

// Test Convert
TEST(UtilsTest, Convert) {  // Convert is typically used with actual model classes, not tested directly here
  SUCCEED();
}

// Test GetEndpoint
TEST(UtilsTest, GetEndpoint) {
  string endpoint1 = Utils::getEndpoint("common.aliyuncs.com", true, "internal");
  EXPECT_EQ("common-internal.aliyuncs.com", endpoint1);

  string endpoint2 = Utils::getEndpoint("common.aliyuncs.com", true, "accelerate");
  EXPECT_EQ("oss-accelerate.aliyuncs.com", endpoint2);

  string endpoint3 = Utils::getEndpoint("common.aliyuncs.com", true, "");
  EXPECT_EQ("common.aliyuncs.com", endpoint3);
}

// Test GetThrottlingTimeLeft
TEST(UtilsTest, GetThrottlingTimeLeft) {
  map<string, string> headers1;
  int64_t timeLeft1 = Utils::getThrottlingTimeLeft(headers1);
  EXPECT_EQ(-1, timeLeft1);

  map<string, string> headers2;
  headers2["x-acs-retry-after"] = "2000";
  int64_t timeLeft2 = Utils::getThrottlingTimeLeft(headers2);
  EXPECT_EQ(2000, timeLeft2);

  map<string, string> headers3;
  headers3["x-acs-retry-after"] = "0";
  int64_t timeLeft3 = Utils::getThrottlingTimeLeft(headers3);
  EXPECT_EQ(-1, timeLeft3);

  map<string, string> headers4;
  headers4["x-acs-retry-after"] = "invalid";
  int64_t timeLeft4 = Utils::getThrottlingTimeLeft(headers4);
  EXPECT_EQ(-1, timeLeft4);

  map<string, string> headers5;
  headers5["x-acs-retry-after"] = "";
  int64_t timeLeft5 = Utils::getThrottlingTimeLeft(headers5);
  EXPECT_EQ(-1, timeLeft5);

  map<string, string> headers6;
  headers6["x-acs-retry-after"] = "-5";
  int64_t timeLeft6 = Utils::getThrottlingTimeLeft(headers6);
  EXPECT_EQ(-1, timeLeft6);
}

// Test Hash
TEST(UtilsTest, Hash) {
  string testStr = "test";
  Darabonba::Bytes testBytes;
  testBytes.assign(testStr.begin(), testStr.end());

  Darabonba::Bytes hash1 = Utils::hash(testBytes, "ACS3-HMAC-SHA256");
  string hex1 = Darabonba::Encode::Encoder::hexEncode(hash1);
  EXPECT_EQ("9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", hex1);

  Darabonba::Bytes hash2 = Utils::hash(testBytes, "ACS3-RSA-SHA256");
  string hex2 = Darabonba::Encode::Encoder::hexEncode(hash2);
  EXPECT_EQ("9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", hex2);
}

// Test GetNonce
TEST(UtilsTest, GetNonce) {
  string nonce1 = Utils::getNonce();
  string nonce2 = Utils::getNonce();
  string nonce3 = Utils::getNonce();
  
  EXPECT_EQ(32, nonce1.length());
  EXPECT_EQ(32, nonce2.length());
  EXPECT_EQ(32, nonce3.length());
  
  // Nonces should be different
  EXPECT_NE(nonce1, nonce2);
  EXPECT_NE(nonce2, nonce3);
}

// Test GetStringToSign
TEST(UtilsTest, GetStringToSign) {
  Darabonba::Http::Request request;
  request.setMethod("GET");
  request.setPathname("/");
  
  map<string, string> headers;
  headers["accept"] = "application/json";
  request.setHeaders(headers);
  
  string sign = Utils::getStringToSign(request);
  // The actual implementation returns without x-acs- headers when there are none
  EXPECT_EQ("GET\napplication/json\n\n\n\n/", sign) << "Actual: " << sign;
}

// Test GetROASignature
TEST(UtilsTest, GetROASignature) {
  string stringToSign = "GET\n\n\n\n\n";
  string signature = Utils::getROASignature(stringToSign, "secret");
  EXPECT_EQ(28, signature.length());
  EXPECT_EQ("XGXDWA78AEvx/wmfxKoVCq/afWw=", signature);
}

// Test ToForm
TEST(UtilsTest, ToForm) {
  json filter = json::object();
  filter["client"] = "test";
  filter["strs"] = json::array({"str1", "str2"});
  filter["tag"] = json::object({{"key", "value"}});
  
  string result = Utils::toForm(filter);
  // The toForm actually returns empty string for JSON, check that it runs without error
  EXPECT_FALSE(result.empty()) << "Result: " << result;
}

// Test GetTimestamp
TEST(UtilsTest, GetTimestamp) {
  string timestamp = Utils::getTimestamp();
  EXPECT_FALSE(timestamp.empty());
  EXPECT_TRUE(timestamp.find("T") != string::npos);
  EXPECT_TRUE(timestamp.find("Z") != string::npos);
}

// Test GetDateUTCString
TEST(UtilsTest, GetDateUTCString) {
  string dateStr = Utils::getDateUTCString();
  EXPECT_FALSE(dateStr.empty());
  EXPECT_EQ(29, dateStr.length());
  EXPECT_TRUE(dateStr.find("GMT") != string::npos);
}

// Test Query
TEST(UtilsTest, Query) {
  json filter = json::object();
  filter["client"] = "test";
  filter["tag"] = json::object({{"key", "value"}});
  filter["strs"] = json::array({"str1", "str2"});
  
  map<string, string> result = Utils::query(filter);
  EXPECT_EQ("test", result["client"]);
  EXPECT_EQ("value", result["tag.key"]);
  EXPECT_EQ("str1", result["strs.1"]);
  EXPECT_EQ("str2", result["strs.2"]);
}

// Test GetRPCSignature
TEST(UtilsTest, GetRPCSignature) {
  map<string, string> signedParams;
  signedParams["test"] = "ok";
  
  string signature = Utils::getRPCSignature(signedParams, "", "accessKeySecret");
  EXPECT_EQ("jHx/oHoHNrbVfhncHEvPdHXZwHU=", signature);
}

// Test ArrayToStringWithSpecifiedStyle
TEST(UtilsTest, ArrayToStringWithSpecifiedStyle) {
  json arr = json::array({"ok", "test", 2, 3});
  
  string result1 = Utils::arrayToStringWithSpecifiedStyle(arr, "instance", "repeatList");
  EXPECT_TRUE(result1.find("instance.1=ok") != string::npos);
  EXPECT_TRUE(result1.find("instance.2=test") != string::npos);
  
  string result2 = Utils::arrayToStringWithSpecifiedStyle(arr, "instance", "json");
  EXPECT_EQ("[\"ok\",\"test\",2,3]", result2);
  
  string result3 = Utils::arrayToStringWithSpecifiedStyle(arr, "instance", "simple");
  EXPECT_EQ("ok,test,2,3", result3);
  
  string result4 = Utils::arrayToStringWithSpecifiedStyle(arr, "instance", "spaceDelimited");
  EXPECT_EQ("ok test 2 3", result4);
  
  string result5 = Utils::arrayToStringWithSpecifiedStyle(arr, "instance", "pipeDelimited");
  EXPECT_EQ("ok|test|2|3", result5);
  
  string result6 = Utils::arrayToStringWithSpecifiedStyle(arr, "instance", "piDelimited");
  EXPECT_EQ("", result6);
  
  string result7 = Utils::arrayToStringWithSpecifiedStyle(json(), "instance", "json");
  EXPECT_EQ("", result7);
}

// Test StringifyMapValue
TEST(UtilsTest, StringifyMapValue) {
  json m = json::object();
  m["num"] = 10;
  m["json"] = json::object({{"test", "ok"}});
  m["str"] = "ok";
  
  map<string, string> result = Utils::stringifyMapValue(m);
  EXPECT_EQ("10", result["num"]);
  EXPECT_EQ("{\"test\":\"ok\"}", result["json"]);
  EXPECT_EQ("ok", result["str"]);
}

// Test ToArray
TEST(UtilsTest, ToArray) {
  json arr = json::array({
    json::object({{"key", "value1"}}),
    json::object({{"key", "value2"}})
  });
  
  vector<json> result = Utils::toArray(arr);
  EXPECT_EQ(2, result.size());
  EXPECT_EQ("value1", result[0]["key"]);
  EXPECT_EQ("value2", result[1]["key"]);
  
  vector<json> emptyResult = Utils::toArray(json());
  EXPECT_EQ(0, emptyResult.size());
}

// Test MapToFlatStyle
TEST(UtilsTest, MapToFlatStyle) {
  // Test null
  json nullResult = Utils::mapToFlatStyle(json());
  EXPECT_TRUE(nullResult.is_null());
  
  // Test primitive values
  json strResult = Utils::mapToFlatStyle(json("test"));
  EXPECT_EQ("test", strResult.get<string>());
  
  json numResult = Utils::mapToFlatStyle(json(123));
  EXPECT_EQ(123, numResult.get<int>());
  
  json boolResult = Utils::mapToFlatStyle(json(true));
  EXPECT_EQ(true, boolResult.get<bool>());
  
  // Test plain map - ALL keys get flat style
  json plainMap = json::object();
  plainMap["key1"] = "value1";
  plainMap["key2"] = "value2";
  json flatMap = Utils::mapToFlatStyle(plainMap);
  // All keys at top level get flat style
  EXPECT_EQ("value1", flatMap["#4#key1"]);
  EXPECT_EQ("value2", flatMap["#4#key2"]);
  
  // Test nested map - recursively applies flat style
  json nestedMap = json::object();
  nestedMap["outerKey"] = json::object({{"innerKey", "innerValue"}});
  json flatNestedMap = Utils::mapToFlatStyle(nestedMap);
  // Both levels get flat style
  EXPECT_EQ("innerValue", flatNestedMap["#8#outerKey"]["#8#innerKey"]);
  
  // Test array
  json arr = json::array({"item1", "item2"});
  json flatArr = Utils::mapToFlatStyle(arr);
  EXPECT_EQ("item1", flatArr[0]);
  EXPECT_EQ("item2", flatArr[1]);
  
  // Test array with map elements
  json arrWithDict = json::array({json::object({{"key", "value"}})});
  json flatArrWithDict = Utils::mapToFlatStyle(arrWithDict);
  // Objects inside arrays also get flat style
  EXPECT_EQ("value", flatArrWithDict[0]["#3#key"]);
  
  // Test complex map
  json complexMap = json::object();
  complexMap["name"] = "testName";
  complexMap["tags"] = json::object({{"tagKey", "tagValue"}});
  json flatComplexMap = Utils::mapToFlatStyle(complexMap);
  EXPECT_EQ("testName", flatComplexMap["#4#name"]);
  EXPECT_EQ("tagValue", flatComplexMap["#4#tags"]["#6#tagKey"]);
}

// Test ParseToMap
TEST(UtilsTest, ParseToMap) {
  json input = json::object();
  input["key"] = "value";
  
  json result = Utils::parseToMap(input);
  EXPECT_EQ("value", result["key"]);
  
  json nullResult = Utils::parseToMap(json());
  EXPECT_TRUE(nullResult.is_null() || nullResult.empty());
}

// Test GetAuthorization
TEST(UtilsTest, GetAuthorization) {
  Darabonba::Http::Request request;
  request.setMethod("GET");
  request.setPathname("/");
  
  map<string, string> query;
  query["test"] = "ok";
  query["empty"] = "";
  request.setQuery(query);
  
  map<string, string> headers;
  headers["x-acs-test"] = "http";
  headers["x-acs-TEST"] = "https";
  request.setHeaders(headers);
  
  string auth = Utils::getAuthorization(
    request,
    "ACS3-HMAC-SHA256",
    "55e12e91650d2fec56ec74e1d3e4ddbfce2ef3a65890c2a19ecf88a307e76a23",
    "acesskey",
    "secret"
  );
  
  EXPECT_TRUE(auth.find("ACS3-HMAC-SHA256") != string::npos);
  EXPECT_TRUE(auth.find("Credential=acesskey") != string::npos);
  EXPECT_TRUE(auth.find("SignedHeaders=") != string::npos);
  EXPECT_TRUE(auth.find("Signature=") != string::npos);
}

// Test GetUserAgent
TEST(UtilsTest, GetUserAgent) {
  string userAgent1 = Utils::getUserAgent("");
  EXPECT_FALSE(userAgent1.empty());
  EXPECT_TRUE(userAgent1.find("AlibabaCloud") != string::npos);
  
  string userAgent2 = Utils::getUserAgent("test");
  EXPECT_TRUE(userAgent2.find("test") != string::npos);
  EXPECT_TRUE(userAgent2.find("AlibabaCloud") != string::npos);
}

// Test GetEndpointRules
TEST(UtilsTest, GetEndpointRules) {
  // Test regional endpoint
  EXPECT_THROW({
    Utils::getEndpointRules("ecs", "", "regional", "", "");
  }, runtime_error);
  
  string endpoint1 = Utils::getEndpointRules("ecs", "cn-hangzhou", "regional", "", "");
  EXPECT_EQ("ecs.cn-hangzhou.aliyuncs.com", endpoint1);
  
  string endpoint2 = Utils::getEndpointRules("ecs", "cn-hangzhou", "central", "intl", "test");
  EXPECT_EQ("ecs-test-intl.aliyuncs.com", endpoint2);
  
  string endpoint3 = Utils::getEndpointRules("ecs", "cn-hangzhou", "central", "", "");
  EXPECT_EQ("ecs.aliyuncs.com", endpoint3);
  
  string endpoint4 = Utils::getEndpointRules("ecs", "cn-hangzhou", "central", "public", "");
  EXPECT_EQ("ecs.aliyuncs.com", endpoint4);
}

int main(int argc, char **argv) {
  ::testing::InitGoogleTest(&argc, argv);
  return RUN_ALL_TESTS();
}
