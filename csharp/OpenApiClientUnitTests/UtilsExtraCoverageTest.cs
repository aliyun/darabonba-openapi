using System;
using System.Collections.Generic;
using AlibabaCloud.OpenApiClient;
using AlibabaCloud.OpenApiClient.Models;
using Darabonba;
using Xunit;

namespace OpenApiClientUnitTests
{
    public class UtilsExtraCoverageTest
    {
        [Fact]
        public void TestEndpointNonceTimestampHashAndRetryHeaders()
        {
            Assert.Equal("ecs-internal.cn-hangzhou.aliyuncs.com", Utils.GetEndpoint("ecs.cn-hangzhou.aliyuncs.com", false, "internal"));
            Assert.Equal("oss-accelerate.aliyuncs.com", Utils.GetEndpoint("oss.aliyuncs.com", true, "accelerate"));
            Assert.Equal("ecs.cn-hangzhou.aliyuncs.com", Utils.GetEndpoint("ecs.cn-hangzhou.aliyuncs.com", false, "regional"));

            Assert.Equal("ecs.cn-hangzhou.aliyuncs.com", Utils.GetEndpointRules("ecs", "cn-hangzhou", "regional", "public", null));
            Assert.Equal("ecs-vpc.cn-hangzhou.aliyuncs.com", Utils.GetEndpointRules("ecs", "cn-hangzhou", "regional", "vpc", ""));
            Assert.Equal("ecs-inner.aliyuncs.com", Utils.GetEndpointRules("ecs", "cn-hangzhou", "central", "public", "inner"));
            Assert.Throws<ArgumentException>(() => Utils.GetEndpointRules("ecs", "", "regional", "public", null));

            Assert.False(string.IsNullOrEmpty(Utils.GetNonce()));
            Assert.Contains("T", Utils.GetTimestamp());
            Assert.Contains("GMT", Utils.GetDateUTCString());
            Assert.Contains("AlibabaCloud", Utils.GetUserAgent(null));
            Assert.Contains("custom-ua", Utils.GetUserAgent("custom-ua"));

            var raw = System.Text.Encoding.UTF8.GetBytes("payload");
            Assert.NotNull(Utils.Hash(raw, "ACS3-HMAC-SHA256"));
            Assert.NotNull(Utils.Hash(raw, "ACS3-RSA-SHA256"));
            Assert.Null(Utils.Hash(raw, "HMAC-SHA1"));

            var headers = new Dictionary<string, string>();
            Utils.ApplyRetryHeaders(headers, 0, 10);
            Assert.Empty(headers);
            Utils.ApplyRetryHeaders(null, 2, 10);
            Utils.ApplyRetryHeaders(headers, 2, 15);
            Assert.Equal("2", headers["x-acs-retry-attempts"]);
            Assert.Equal("15", headers["x-acs-retry-delay"]);
        }

        [Fact]
        public void TestQueryFormArrayParseConvertAndSignatures()
        {
            Assert.Equal("", Utils.ToForm(null));
            var form = Utils.ToForm(new Dictionary<string, object>
            {
                { "Name", "aliyun" },
                { "Ids", new List<object> { "1", "2" } },
                { "Empty", "" }
            });
            Assert.Contains("Name=aliyun", form);
            Assert.Contains("Ids.1=1", form);

            var query = Utils.Query(new Dictionary<string, object>
            {
                { "Region", "cn-hangzhou" },
                { "Ids", new List<object> { "a", "b" } },
                { "Bytes", System.Text.Encoding.UTF8.GetBytes("ok") }
            });
            Assert.Equal("cn-hangzhou", query["Region"]);
            Assert.Equal("a", query["Ids.1"]);
            Assert.Equal("ok", query["Bytes"]);

            Assert.Equal("", Utils.ArrayToStringWithSpecifiedStyle(null, "Ids", "json"));
            Assert.Equal("1,2", Utils.ArrayToStringWithSpecifiedStyle(new List<object> { "1", "2" }, "Ids", "simple"));
            Assert.Equal("1 2", Utils.ArrayToStringWithSpecifiedStyle(new List<object> { "1", "2" }, "Ids", "spaceDelimited"));
            Assert.Equal("1|2", Utils.ArrayToStringWithSpecifiedStyle(new List<object> { "1", "2" }, "Ids", "pipeDelimited"));
            Assert.Contains("1", Utils.ArrayToStringWithSpecifiedStyle(new List<object> { "1", "2" }, "Ids", "json"));
            Assert.Contains("Ids.1=1", Utils.ArrayToStringWithSpecifiedStyle(new List<object> { "1", "2" }, "Ids", "repeatList"));
            Assert.Equal("", Utils.ArrayToStringWithSpecifiedStyle(new List<object> { "1" }, "Ids", "unknown"));

            Assert.Null(Utils.ParseToMap(null));
            var paramsMap = Utils.ParseToMap(new Params { Action = "DescribeRegions", Version = "2014-05-26" });
            Assert.Equal("DescribeRegions", paramsMap["action"]);

            Utils.Convert(null, new Params());
            Utils.Convert(new Params { Action = "A" }, null);
            var dest = new Params();
            Utils.Convert(new Params { Action = "DescribeRegions", Version = "2014-05-26" }, dest);
            Assert.NotNull(dest);

            Assert.Null(Utils.ToArray("not-a-list"));
            Assert.Null(Utils.ToArray(null));
            var array = Utils.ToArray(new List<Params>
            {
                new Params { Action = "A" },
                null
            });
            Assert.Single(array);
            Assert.Equal("A", array[0]["action"]);

            var nested = Utils.MapToFlatStyle(new Dictionary<string, object>
            {
                { "Name", "aliyun" },
                { "Tags", new List<object> { "a", "b" } }
            });
            Assert.NotNull(nested);
            Assert.Null(Utils.MapToFlatStyle(null));

            var request = new Request
            {
                Method = "POST",
                Pathname = "/test",
                Headers = new Dictionary<string, string>
                {
                    { "accept", "application/json" },
                    { "content-type", "application/json" },
                    { "content-md5", "md5" },
                    { "date", "Mon, 11 Aug 2025 00:00:00 GMT" },
                    { "host", "ecs.aliyuncs.com" },
                    { "x-acs-action", "DescribeRegions" }
                },
                Query = new Dictionary<string, string>
                {
                    { "RegionId", "cn-hangzhou" },
                    { "Empty", "" },
                    { "Skip", null }
                }
            };
            var stringToSign = Utils.GetStringToSign(request);
            Assert.Contains("POST", stringToSign);
            Assert.Contains("x-acs-action", stringToSign);
            Assert.False(string.IsNullOrEmpty(Utils.GetROASignature(stringToSign, "secret")));
            Assert.False(string.IsNullOrEmpty(Utils.GetRPCSignature(new Dictionary<string, string>
            {
                { "Action", "DescribeRegions" },
                { "Version", "2014-05-26" }
            }, "POST", "secret")));

            var auth = Utils.GetAuthorization(request, "ACS3-HMAC-SHA256", "payload", "ak", "secret");
            Assert.Contains("ACS3-HMAC-SHA256 Credential=ak", auth);
            request.Pathname = "";
            request.Query = new Dictionary<string, string>();
            var authEmpty = Utils.GetAuthorization(request, "ACS3-HMAC-SHA256", "payload", "ak", "secret");
            Assert.Contains("SignedHeaders=", authEmpty);

            request.Headers["Host"] = "ecs.aliyuncs.com";
            request.Headers["host"] = "ecs.aliyuncs.com";
            request.Headers["X-Acs-Action"] = "DescribeRegions";
            request.Query["Space"] = "a b";
            request.Query["Star"] = "a*b";
            request.Query["Tilde"] = "a~b";
            var authDup = Utils.GetAuthorization(request, "ACS3-HMAC-SHA256", "payload", "ak", "secret");
            Assert.Contains("ACS3-HMAC-SHA256 Credential=ak", authDup);

            var withStream = new OpenApiRequest
            {
                Stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes("x")),
                Body = new Dictionary<string, object>
                {
                    { "keep", "v" },
                    { "nested", new List<object> { "a", null, new Dictionary<string, object> { { "k", "v" } } } },
                    { "nil", null }
                }
            };
            var destReq = new OpenApiRequest();
            Utils.Convert(withStream, destReq);
            Assert.NotNull(destReq);

            var querySpecial = Utils.Query(new Dictionary<string, object>
            {
                { "Empty", "" },
                { "Nested", new Dictionary<string, object> { { "k", "v" } } },
                { "List", new List<object> { 1, 2 } },
                { "Model", new Params { Action = "DescribeRegions", Version = "2014-05-26" } },
                { "Skip", null }
            });
            Assert.Equal("v", querySpecial["Nested.k"]);
            Assert.Equal("DescribeRegions", querySpecial["Model.action"]);

            var jsonStyle = Utils.ArrayToStringWithSpecifiedStyle(new List<object>
            {
                new Dictionary<string, object>
                {
                    { "keep", "v" },
                    { "nil", null },
                    { "model", new Params { Action = "A" } }
                }
            }, "Ids", "json");
            Assert.Contains("keep", jsonStyle);

            var flatModel = Utils.MapToFlatStyle(new Params { Action = "A", Version = "v" });
            Assert.NotNull(flatModel);
            var flatList = Utils.MapToFlatStyle(new List<object>
            {
                new Dictionary<string, object> { { "Name", "aliyun" } },
                "plain"
            });
            Assert.NotNull(flatList);
        }
    }
}
