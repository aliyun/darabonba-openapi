using System.Collections.Generic;
using System.IO;
using System.Text;
using AlibabaCloud.OpenApiClient.Models;
using Xunit;

namespace OpenApiClientUnitTests.Models
{
    public class OpenApiRequestTest
    {
        [Fact]
        public void TestOpenApiRequest()
        {
            var req = new OpenApiRequest
            {
                Query = new Dictionary<string, string> { { "key", "value" } },
                Body = new Dictionary<string, object> { { "key", "value" } },
                Headers = new Dictionary<string, string> { { "key", "value" } },
                HostMap = new Dictionary<string, string> { { "key", "value" } },
                EndpointOverride = "test",
                Stream = new MemoryStream(Encoding.UTF8.GetBytes("test"))
            };

            Assert.Equal("value", req.Headers["key"]);
            Assert.Equal("value", req.Query["key"]);
            Assert.Equal("test", req.EndpointOverride);
            Assert.Equal("value", req.HostMap["key"]);

            req.Stream.Position = 0;
            using (var reader = new StreamReader(req.Stream, Encoding.UTF8, false, 1024, true))
            {
                Assert.Equal("test", reader.ReadToEnd());
            }
        }
    }

    public class ParamsTest
    {
        [Fact]
        public void TestParams()
        {
            var parameters = new Params
            {
                Action = "test",
                Version = "test",
                Protocol = "test",
                Pathname = "test",
                Method = "test",
                AuthType = "test",
                BodyType = "test",
                ReqBodyType = "test",
                Style = "test"
            };

            Assert.Equal("test", parameters.Action);
            Assert.Equal("test", parameters.Version);
            Assert.Equal("test", parameters.Protocol);
            Assert.Equal("test", parameters.Pathname);
            Assert.Equal("test", parameters.Method);
            Assert.Equal("test", parameters.AuthType);
            Assert.Equal("test", parameters.BodyType);
            Assert.Equal("test", parameters.ReqBodyType);
            Assert.Equal("test", parameters.Style);
        }
    }

    public class GlobalParametersTest
    {
        [Fact]
        public void TestGlobalParameters()
        {
            var gp = new GlobalParameters
            {
                Headers = new Dictionary<string, string> { { "h", "v" } },
                Queries = new Dictionary<string, string> { { "q", "v" } }
            };
            Assert.Equal("v", gp.Headers["h"]);
            Assert.Equal("v", gp.Queries["q"]);
        }
    }
}
