using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AlibabaCloud.OpenApiClient.Exceptions;
using AlibabaCloud.OpenApiClient.Models;
using OpenApiClientUnitTests.Mock;
using Xunit;

namespace OpenApiClientUnitTests
{
    public class ClientAsyncCoverageTest
    {
        [Fact]
        public async Task TestCallApiAsyncV2AndV3Paths()
        {
            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
                server.PathContent["/testArray"] = "array";
                server.PathContent["/testError"] = "error";
                server.PathContent["/testError2"] = "error2";
                server.PathContent["/empty"] = "empty204";
                server.PathContent["/xml"] = "xml";
                server.PathContent["/server"] = "serverError";

                var runtime = TestFixtures.CreateRuntimeOptions();
                var request = TestFixtures.CreateOpenApiRequest();

                var v2Config = TestFixtures.CreateConfig();
                v2Config.Protocol = "HTTP";
                v2Config.SignatureAlgorithm = "v2";
                v2Config.Endpoint = server.Endpoint;
                var v2 = new TestClient(v2Config);

                var rpc = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/",
                    Method = "POST", AuthType = "AK", Style = "RPC", ReqBodyType = "formData", BodyType = "json"
                };
                var result = await v2.CallApiAsync(rpc, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                rpc.AuthType = "Anonymous";
                rpc.ReqBodyType = "json";
                result = await v2.CallApiAsync(rpc, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                var bearerConfig = TestFixtures.CreateBearerTokenConfig();
                bearerConfig.Protocol = "HTTP";
                bearerConfig.SignatureAlgorithm = "v2";
                bearerConfig.Endpoint = server.Endpoint;
                var bearer = new TestClient(bearerConfig);
                rpc.AuthType = "AK";
                rpc.ReqBodyType = "formData";
                result = await bearer.CallApiAsync(rpc, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                var anon = new TestClient(new AlibabaCloud.OpenApiClient.Models.Config
                {
                    Protocol = "HTTP",
                    SignatureAlgorithm = "v2",
                    Endpoint = server.Endpoint
                });
                await Assert.ThrowsAsync<ClientException>(() => anon.CallApiAsync(rpc, request, runtime));

                var roa = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/test",
                    Method = "POST", AuthType = "AK", Style = "ROA", ReqBodyType = "formData", BodyType = "json"
                };
                result = await v2.CallApiAsync(roa, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                roa.ReqBodyType = "json";
                roa.AuthType = "Anonymous";
                result = await v2.CallApiAsync(roa, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await bearer.CallApiAsync(roa, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                var v3Config = TestFixtures.CreateConfig();
                v3Config.Protocol = "HTTP";
                v3Config.Endpoint = server.Endpoint;
                var v3 = new TestClient(v3Config);
                var v3Params = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/",
                    Method = "POST", AuthType = "AK", Style = "RPC", ReqBodyType = "formData", BodyType = "json"
                };
                result = await v3.CallApiAsync(v3Params, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                v3Params.AuthType = "Anonymous";
                v3Params.ReqBodyType = "json";
                result = await v3.CallApiAsync(v3Params, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                v3Params.Style = "ROA";
                v3Params.Pathname = "/test";
                v3Params.ReqBodyType = "formData";
                v3Params.AuthType = "AK";
                result = await v3.CallApiAsync(v3Params, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                v3Params.AuthType = "Anonymous";
                v3Params.ReqBodyType = "json";
                result = await v3.CallApiAsync(v3Params, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                var bearerV3 = TestFixtures.CreateBearerTokenConfig();
                bearerV3.Protocol = "HTTP";
                bearerV3.Endpoint = server.Endpoint;
                result = await new TestClient(bearerV3).CallApiAsync(v3Params, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
            }
        }

        [Fact]
        public async Task TestAsyncBodyTypesErrorsExecuteAndDirect()
        {
            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
                server.PathContent["/testArray"] = "array";
                server.PathContent["/testError"] = "error";
                server.PathContent["/testError2"] = "error2";
                server.PathContent["/empty"] = "empty204";
                server.PathContent["/xml"] = "xml";
                server.PathContent["/server"] = "serverError";

                var runtime = TestFixtures.CreateRuntimeOptions();
                var request = TestFixtures.CreateOpenApiRequest();
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                var client = new TestClient(config);

                var parameters = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/test",
                    Method = "POST", AuthType = "AK", Style = "ROA", ReqBodyType = "formData", BodyType = "json"
                };
                var result = await client.CallApiAsync(parameters, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                parameters.BodyType = "array";
                parameters.Pathname = "/testArray";
                result = await client.CallApiAsync(parameters, request, runtime);
                Assert.NotNull(result["body"]);
                parameters.BodyType = "string";
                parameters.Pathname = "/test";
                result = await client.CallApiAsync(parameters, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                parameters.BodyType = "binary";
                result = await client.CallApiAsync(parameters, request, runtime);
                Assert.NotNull(result["body"]);
                parameters.BodyType = "byte";
                result = await client.CallApiAsync(parameters, request, runtime);
                Assert.NotNull(result["body"]);
                parameters.BodyType = "none";
                result = await client.CallApiAsync(parameters, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                parameters.BodyType = "json";
                parameters.Pathname = "/testError";
                await Assert.ThrowsAsync<ClientException>(() => client.CallApiAsync(parameters, request, runtime));
                parameters.Pathname = "/testError2";
                await Assert.ThrowsAsync<ClientException>(() => client.CallApiAsync(parameters, request, runtime));
                parameters.Pathname = "/xml";
                try { await client.CallApiAsync(parameters, request, runtime); } catch (Exception) { }

                parameters.ReqBodyType = "json";
                parameters.Pathname = "/test";
                result = await client.CallApiAsync(parameters, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                parameters.ReqBodyType = "byte";
                result = await client.DoRequestAsync(parameters, new OpenApiRequest { Body = Encoding.UTF8.GetBytes("test byte") }, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                parameters.ReqBodyType = "binary";
                result = await client.DoRequestAsync(parameters, new OpenApiRequest { Stream = new MemoryStream(Encoding.UTF8.GetBytes("stream")) }, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                result = await client.DoRPCRequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "string", request, runtime);
                Assert.NotNull(result);
                result = await client.DoROARequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/test", "byte", request, runtime);
                Assert.NotNull(result);
                result = await client.DoROARequestWithFormAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/test", "none", request, runtime);
                Assert.NotNull(result);

                var execConfig = TestFixtures.CreateConfig();
                execConfig.Protocol = "HTTP";
                execConfig.Endpoint = server.Endpoint;
                execConfig.SignatureVersion = "v4";
                var execClient = new TestClient(execConfig);
                execClient.ProductId = "test";
                execClient.SetGatewayClient(new AlibabaCloud.GatewayPop.Client());
                var execParams = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/test",
                    Method = "POST", AuthType = "Anonymous", Style = "RPC", ReqBodyType = "formData", BodyType = "json"
                };
                result = await execClient.ExecuteAsync(execParams, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                execParams.BodyType = "string";
                result = await execClient.ExecuteAsync(execParams, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                var retryRuntime = TestFixtures.CreateRuntimeOptions();
                retryRuntime.Autoretry = true;
                retryRuntime.MaxAttempts = 2;
                retryRuntime.BackoffPolicy = "fix";
                retryRuntime.BackoffPeriod = 1;
                var fail = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/server",
                    Method = "POST", AuthType = "Anonymous", Style = "ROA", ReqBodyType = "json", BodyType = "json"
                };
                var v2Config = TestFixtures.CreateConfig();
                v2Config.Protocol = "HTTP";
                v2Config.SignatureAlgorithm = "v2";
                v2Config.Endpoint = server.Endpoint;
                await Assert.ThrowsAsync<ServerException>(() => new TestClient(v2Config).CallApiAsync(fail, request, retryRuntime));
                fail.ReqBodyType = "formData";
                await Assert.ThrowsAsync<ServerException>(() => new TestClient(v2Config).CallApiAsync(fail, request, retryRuntime));
            }
        }
    }
}
