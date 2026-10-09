using System;
using System.Text;
using System.Threading.Tasks;
using AlibabaCloud.OpenApiClient.Models;
using Xunit;

namespace OpenApiClientUnitTests
{
    public class Ipv4OnlyTest
    {
        // Nothing listens here; an IPv4-only client must refuse the IPv6 literal before connecting.
        private const string Ipv6Endpoint = "[::1]:1";

        private static string Flatten(Exception ex)
        {
            var sb = new StringBuilder();
            for (var e = ex; e != null; e = e.InnerException)
            {
                sb.Append(e.Message).Append('\n');
            }
            return sb.ToString();
        }

        private static Params CreateParams(string style, string reqBodyType)
        {
            return new Params
            {
                Action = "TestAPI",
                Version = "2022-06-01",
                Protocol = "HTTP",
                Pathname = "/",
                Method = "POST",
                AuthType = "AK",
                Style = style,
                ReqBodyType = reqBodyType,
                BodyType = "json"
            };
        }

        [Theory]
        [InlineData("ACS3-HMAC-SHA256", null, "RPC", "formData")]
        [InlineData("v2", null, "RPC", "formData")]
        [InlineData("v2", null, "ROA", "json")]
        [InlineData("v2", null, "ROA", "formData")]
        [InlineData("ACS3-HMAC-SHA256", "v4", "RPC", "formData")]
        public async Task TestIpv4OnlyIsPassedToRuntime(string algorithm, string signatureVersion, string style, string reqBodyType)
        {
            // config.Ipv4Only, runtime.Ipv4Only, expected
            var settings = new[]
            {
                new bool?[] { null, null, false },
                new bool?[] { true, null, true },
                new bool?[] { null, true, true },
                new bool?[] { false, true, true },
            };
            foreach (var setting in settings)
            {
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = Ipv6Endpoint;
                config.SignatureAlgorithm = algorithm;
                config.SignatureVersion = signatureVersion;
                config.Ipv4Only = setting[0];
                var client = new TestClient(config);
                if (signatureVersion == "v4")
                {
                    client.SetGatewayClient(new AlibabaCloud.GatewayPop.Client());
                }
                var runtime = TestFixtures.CreateRuntimeOptions();
                runtime.ConnectTimeout = 1000;
                runtime.Ipv4Only = setting[1];
                var expected = setting[2].Value;
                var parameters = CreateParams(style, reqBodyType);

                var sync = Assert.ThrowsAny<Exception>(() => client.CallApi(parameters, TestFixtures.CreateOpenApiRequest(), runtime));
                Assert.True(expected == Flatten(sync).Contains("ipv4Only"), Flatten(sync));

                var async = await Assert.ThrowsAnyAsync<Exception>(() => client.CallApiAsync(parameters, TestFixtures.CreateOpenApiRequest(), runtime));
                Assert.True(expected == Flatten(async).Contains("ipv4Only"), Flatten(async));
            }
        }
    }
}
