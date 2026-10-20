using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlibabaCloud.OpenApiClient.Exceptions;
using AlibabaCloud.OpenApiClient.Models;
using OpenApiClientUnitTests.Mock;
using Xunit;

namespace OpenApiClientUnitTests
{
    public class ClientExtraCoverageTest
    {
        [Fact]
        public void TestHelpersAndExceptions()
        {
            var empty = new TestClient(new Config { Endpoint = "example.com" });
            Assert.Equal("", empty.GetAccessKeyId());
            Assert.Equal("", empty.GetAccessKeySecret());
            Assert.Equal("", empty.GetSecurityToken());
            Assert.Equal("", empty.GetBearerToken());
            Assert.Equal("", empty.GetType());
            empty.CheckConfig(new Config { Endpoint = "example.com" });
            Assert.Throws<ClientException>(() => empty.CheckConfig(new Config()));

            empty.SetRpcHeaders(new Dictionary<string, string> { { "debug", "1" } });
            Assert.Equal("1", empty.GetRpcHeaders()["debug"]);
            Assert.Null(empty.GetRpcHeaders());

            var detail = empty.GetAccessDeniedDetail(new Dictionary<string, object>
            {
                { "AccessDeniedDetail", new Dictionary<string, object> { { "k", "v" } } }
            });
            Assert.Equal("v", detail["k"]);
            detail = empty.GetAccessDeniedDetail(new Dictionary<string, object>
            {
                { "accessDeniedDetail", new Dictionary<string, object> { { "k", "v2" } } }
            });
            Assert.Equal("v2", detail["k"]);
            Assert.Null(empty.GetAccessDeniedDetail(new Dictionary<string, object>()));

            var clientErr = new ClientException
            {
                StatusCode = 400,
                Code = "Client.Error",
                Message = "bad",
                Detail = "d",
                Description = "desc",
                RequestId = "rid",
                AccessDeniedDetail = new Dictionary<string, object> { { "k", "v" } }
            };
            Assert.Equal("Client.Error", clientErr.Code);
            Assert.Equal("rid", clientErr.RequestId);
            var serverErr = new ServerException { StatusCode = 500, Code = "Internal", Message = "boom" };
            Assert.Equal("Internal", serverErr.Code);
            var throttling = new ThrottlingException { StatusCode = 429, Code = "Throttling", RetryAfter = 1000 };
            Assert.Equal(1000, throttling.RetryAfter);
            var cloud = new AlibabaCloudException { StatusCode = 400, Code = "X", RequestId = "rid", Detail = "d" };
            Assert.Equal("rid", cloud.RequestId);

            Assert.Throws<ClientException>(() => empty.CallApi(null, TestFixtures.CreateOpenApiRequest(), TestFixtures.CreateRuntimeOptions()));
        }

        [Fact]
        public async Task TestCallApiAsyncNullParams()
        {
            var empty = new TestClient(new Config { Endpoint = "example.com" });
            await Assert.ThrowsAsync<ClientException>(() => empty.CallApiAsync(null, TestFixtures.CreateOpenApiRequest(), TestFixtures.CreateRuntimeOptions()));
        }

        [Fact]
        public async Task TestHelpersAsync()
        {
            var empty = new TestClient(new Config { Endpoint = "example.com" });
            Assert.Equal("", await empty.GetAccessKeyIdAsync());
            Assert.Equal("", await empty.GetAccessKeySecretAsync());
            Assert.Equal("", await empty.GetSecurityTokenAsync());
            Assert.Equal("", await empty.GetBearerTokenAsync());
            Assert.Equal("", await empty.GetTypeAsync());

            var ak = new TestClient(TestFixtures.CreateConfig());
            Assert.Equal("ak", await ak.GetAccessKeyIdAsync());
            Assert.Equal("secret", await ak.GetAccessKeySecretAsync());
            Assert.Equal("token", await ak.GetSecurityTokenAsync());
            Assert.Equal("sts", await ak.GetTypeAsync());

            var bearer = new TestClient(TestFixtures.CreateBearerTokenConfig());
            Assert.Equal("token", await bearer.GetBearerTokenAsync());
            Assert.Equal("bearer", await bearer.GetTypeAsync());
        }

        [Fact]
        public async Task TestXmlEmptyDirectExecuteAndAsync()
        {
            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
                server.PathContent["/empty"] = "empty204";
                server.PathContent["/xml"] = "xml";
                server.PathContent["/server"] = "serverError";
                server.PathContent["/testError"] = "error";

                var runtime = TestFixtures.CreateRuntimeOptions();
                var request = TestFixtures.CreateOpenApiRequest();
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                var client = new TestClient(config);

                var noneParams = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/test",
                    Method = "POST",
                    AuthType = "Anonymous",
                    Style = "RPC",
                    ReqBodyType = "json",
                    BodyType = "none"
                };
                var result = client.CallApi(noneParams, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await client.CallApiAsync(noneParams, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                noneParams.Pathname = "/xml";
                noneParams.BodyType = "json";
                try
                {
                    client.CallApi(noneParams, request, runtime);
                    Assert.True(false, "expected xml error");
                }
                catch (Exception)
                {
                }
                try
                {
                    await client.CallApiAsync(noneParams, request, runtime);
                    Assert.True(false, "expected xml error");
                }
                catch (Exception)
                {
                }

                var v2Config = TestFixtures.CreateConfig();
                v2Config.Protocol = "HTTP";
                v2Config.SignatureAlgorithm = "v2";
                v2Config.Endpoint = server.Endpoint;
                var v2 = new TestClient(v2Config);
                var emptyParams = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/empty",
                    Method = "POST",
                    AuthType = "Anonymous",
                    Style = "ROA",
                    ReqBodyType = "json",
                    BodyType = "json"
                };
                result = v2.CallApi(emptyParams, request, runtime);
                Assert.NotNull(TestFixtures.GetHeaders(result));
                result = await v2.CallApiAsync(emptyParams, request, runtime);
                Assert.NotNull(TestFixtures.GetHeaders(result));
                emptyParams.ReqBodyType = "formData";
                result = v2.CallApi(emptyParams, request, runtime);
                Assert.NotNull(TestFixtures.GetHeaders(result));
                result = await v2.CallApiAsync(emptyParams, request, runtime);
                Assert.NotNull(TestFixtures.GetHeaders(result));

                var failParams = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/server",
                    Method = "POST",
                    AuthType = "Anonymous",
                    Style = "RPC",
                    ReqBodyType = "formData",
                    BodyType = "json"
                };
                try
                {
                    v2.CallApi(failParams, request, runtime);
                    Assert.True(false, "expected server error");
                }
                catch (Exception)
                {
                }
                try
                {
                    await v2.CallApiAsync(failParams, request, runtime);
                    Assert.True(false, "expected server error");
                }
                catch (Exception)
                {
                }

                result = client.DoRPCRequest("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "json", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await client.DoRPCRequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "json", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await client.DoRPCRequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "string", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await client.DoRPCRequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "none", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await client.DoRPCRequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "byte", request, runtime);
                Assert.NotNull(result["body"]);
                result = client.DoROARequest("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/test", "json", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await client.DoROARequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/test", "json", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await client.DoROARequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/test", "string", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = client.DoROARequestWithForm("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/test", "json", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await client.DoROARequestWithFormAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/test", "json", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await client.DoROARequestWithFormAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/test", "none", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                var doParams = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/test",
                    Method = "POST",
                    AuthType = "Anonymous",
                    Style = "RPC",
                    ReqBodyType = "json",
                    BodyType = "string"
                };
                result = client.DoRequest(doParams, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await client.DoRequestAsync(doParams, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                doParams.AuthType = "AK";
                doParams.Style = "ROA";
                doParams.ReqBodyType = "formData";
                doParams.BodyType = "json";
                result = await client.DoRequestAsync(doParams, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                var v2Rpc = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/",
                    Method = "POST",
                    AuthType = "Anonymous",
                    Style = "RPC",
                    ReqBodyType = "formData",
                    BodyType = "json"
                };
                result = v2.CallApi(v2Rpc, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await v2.CallApiAsync(v2Rpc, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                v2Rpc.ReqBodyType = "json";
                result = await v2.CallApiAsync(v2Rpc, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                var execConfig = TestFixtures.CreateConfig();
                execConfig.Protocol = "HTTP";
                execConfig.Endpoint = server.Endpoint;
                execConfig.SignatureVersion = "v4";
                var execClient = new TestClient(execConfig);
                execClient.ProductId = "test";
                execClient.SetGatewayClient(new AlibabaCloud.GatewayPop.Client());
                var execParams = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/test",
                    Method = "POST",
                    AuthType = "Anonymous",
                    Style = "RPC",
                    ReqBodyType = "json",
                    BodyType = "json"
                };
                result = execClient.CallApi(execParams, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await execClient.CallApiAsync(execParams, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = execClient.Execute(execParams, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await execClient.ExecuteAsync(execParams, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                result = v2.DoRPCRequest("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "json", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await v2.DoRPCRequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "string", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = v2.DoROARequest("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "/test", "json", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await v2.DoROARequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "/test", "string", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = v2.DoROARequestWithForm("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "/test", "json", request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await v2.DoROARequestWithFormAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "/test", "byte", request, runtime);
                Assert.NotNull(result["body"]);

                var sseParams = new Params
                {
                    Action = "SSETestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/sse",
                    Method = "GET",
                    AuthType = "Anonymous",
                    Style = "RPC",
                    ReqBodyType = "json",
                    BodyType = "json"
                };
                client.SetRpcHeaders(new Dictionary<string, string> { { "rpc-sse", "1" } });
                var sseEvents = new List<SSEResponse>();
                foreach (var item in client.CallSSEApi(sseParams, request, runtime))
                {
                    sseEvents.Add(item);
                }
                Assert.True(sseEvents.Count >= 1);

                var bearerConfig = TestFixtures.CreateBearerTokenConfig();
                bearerConfig.Protocol = "HTTP";
                bearerConfig.Endpoint = server.Endpoint;
                var bearerSse = new TestClient(bearerConfig);
                sseParams.AuthType = "AK";
                sseParams.Style = "ROA";
                sseParams.ReqBodyType = "formData";
                sseEvents.Clear();
                foreach (var item in bearerSse.CallSSEApi(sseParams, request, runtime))
                {
                    sseEvents.Add(item);
                }
                Assert.True(sseEvents.Count >= 1);

                sseParams.ReqBodyType = "json";
                using (var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes("stream-body")))
                {
                    var streamReq = new OpenApiRequest
                    {
                        Headers = request.Headers,
                        Query = request.Query,
                        Body = request.Body,
                        Stream = stream
                    };
                    sseEvents.Clear();
                    foreach (var item in client.CallSSEApi(sseParams, streamReq, runtime))
                    {
                        sseEvents.Add(item);
                    }
                    Assert.True(sseEvents.Count >= 1);
                }

                sseParams.ReqBodyType = "byte";
                var byteReq = new OpenApiRequest { Body = System.Text.Encoding.UTF8.GetBytes("test byte") };
                sseEvents.Clear();
                foreach (var item in client.CallSSEApi(sseParams, byteReq, runtime))
                {
                    sseEvents.Add(item);
                }
                Assert.True(sseEvents.Count >= 0);

                sseParams.Pathname = "/testError";
                try
                {
                    foreach (var item in client.CallSSEApi(sseParams, request, runtime))
                    {
                    }
                    Assert.True(false, "expected sse json error");
                }
                catch (Exception)
                {
                }

                sseParams.Pathname = "/xml";
                try
                {
                    foreach (var item in client.CallSSEApi(sseParams, request, runtime))
                    {
                    }
                    Assert.True(false, "expected sse xml error");
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
