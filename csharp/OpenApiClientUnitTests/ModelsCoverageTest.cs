using System.Collections.Generic;
using System.IO;
using System.Text;
using AlibabaCloud.OpenApiClient.Models;
using Xunit;

namespace OpenApiClientUnitTests
{
    public class ModelsCoverageTest
    {
        [Fact]
        public void TestGlobalParametersCopyToMapFromMap()
        {
            var empty = GlobalParameters.FromMap(new Dictionary<string, object>());
            Assert.Null(empty.Headers);
            Assert.Null(empty.Queries);

            var emptyHeaders = GlobalParameters.FromMap(new Dictionary<string, object>
            {
                { "headers", new Dictionary<string, string>() },
                { "queries", new Dictionary<string, string>() }
            });
            Assert.Null(emptyHeaders.Headers);

            var original = new GlobalParameters
            {
                Headers = new Dictionary<string, string> { { "h", "1" } },
                Queries = new Dictionary<string, string> { { "q", "2" } }
            };
            var mapped = original.ToMap();
            Assert.Equal("1", ((Dictionary<string, string>)mapped["headers"])["h"]);
            Assert.Equal("2", ((Dictionary<string, string>)mapped["queries"])["q"]);

            var restored = GlobalParameters.FromMap(mapped);
            Assert.Equal("1", restored.Headers["h"]);
            Assert.Equal("2", restored.Queries["q"]);

            var copy = original.Copy();
            Assert.Equal("1", copy.Headers["h"]);
            var copyNoStream = original.CopyWithoutStream();
            Assert.Equal("2", copyNoStream.Queries["q"]);
        }

        [Fact]
        public void TestOpenApiRequestCopyToMapFromMap()
        {
            var empty = OpenApiRequest.FromMap(new Dictionary<string, object>());
            Assert.Null(empty.Headers);

            var emptyMaps = OpenApiRequest.FromMap(new Dictionary<string, object>
            {
                { "headers", new Dictionary<string, string>() },
                { "query", new Dictionary<string, string>() },
                { "hostMap", new Dictionary<string, string>() }
            });
            Assert.Null(emptyMaps.Headers);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("payload")))
            {
                var original = new OpenApiRequest
                {
                    Headers = new Dictionary<string, string> { { "for-test", "sdk" } },
                    Query = new Dictionary<string, string> { { "k", "v" } },
                    Body = new Dictionary<string, object> { { "b", "1" } },
                    Stream = stream,
                    HostMap = new Dictionary<string, string> { { "region", "cn-hangzhou" } },
                    EndpointOverride = "override.example.com"
                };
                var mapped = original.ToMap();
                Assert.Equal("sdk", ((Dictionary<string, string>)mapped["headers"])["for-test"]);
                Assert.Equal("v", ((Dictionary<string, string>)mapped["query"])["k"]);
                Assert.Equal("cn-hangzhou", ((Dictionary<string, string>)mapped["hostMap"])["region"]);
                Assert.Equal("override.example.com", mapped["endpointOverride"]);
                Assert.NotNull(mapped["stream"]);
                Assert.NotNull(mapped["body"]);

                var restored = OpenApiRequest.FromMap(mapped);
                Assert.Equal("sdk", restored.Headers["for-test"]);
                Assert.Equal("v", restored.Query["k"]);
                Assert.Equal("override.example.com", restored.EndpointOverride);
                Assert.Equal("cn-hangzhou", restored.HostMap["region"]);
                Assert.NotNull(restored.Stream);

                var copy = original.Copy();
                Assert.Equal("sdk", copy.Headers["for-test"]);
                var copyNoStream = original.CopyWithoutStream();
                Assert.Equal("override.example.com", copyNoStream.EndpointOverride);
            }
        }

        [Fact]
        public void TestParamsCopyToMapFromMapAndConfigEndpoint()
        {
            var empty = Params.FromMap(new Dictionary<string, object>());
            Assert.Null(empty.Action);

            var original = new Params
            {
                Action = "DescribeRegions",
                Version = "2014-05-26",
                Protocol = "HTTPS",
                Pathname = "/",
                Method = "POST",
                AuthType = "AK",
                BodyType = "json",
                ReqBodyType = "formData",
                Style = "RPC"
            };
            var mapped = original.ToMap();
            Assert.Equal("DescribeRegions", mapped["action"]);
            Assert.Equal("RPC", mapped["style"]);
            Assert.Equal("formData", mapped["reqBodyType"]);

            var restored = Params.FromMap(mapped);
            Assert.Equal("DescribeRegions", restored.Action);
            Assert.Equal("2014-05-26", restored.Version);
            Assert.Equal("HTTPS", restored.Protocol);
            Assert.Equal("/", restored.Pathname);
            Assert.Equal("POST", restored.Method);
            Assert.Equal("AK", restored.AuthType);
            Assert.Equal("json", restored.BodyType);
            Assert.Equal("formData", restored.ReqBodyType);
            Assert.Equal("RPC", restored.Style);

            var copy = original.Copy();
            Assert.Equal("DescribeRegions", copy.Action);
            var copyNoStream = original.CopyWithoutStream();
            Assert.Equal("RPC", copyNoStream.Style);

            var config = new Config
            {
                OpenPlatformEndpoint = "open.aliyun.com",
                Endpoint = "example.com"
            };
            Assert.Equal("open.aliyun.com", config.OpenPlatformEndpoint);
            Assert.Equal("example.com", config.Endpoint);
        }
    }
}
