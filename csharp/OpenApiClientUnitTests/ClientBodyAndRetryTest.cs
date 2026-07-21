using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AlibabaCloud.OpenApiClient.Exceptions;
using AlibabaCloud.OpenApiClient.Models;
using Darabonba.Models;
using OpenApiClientUnitTests.Mock;
using Xunit;

namespace OpenApiClientUnitTests
{
    public class ClientBodyAndRetryTest
    {
        private const string RequestId = "A45EE076-334D-5012-9746-A8F828D20FD4";
        private const string JsonBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";

        [Fact]
        public void TestResponseBodyType()
        {
            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
                server.PathContent["/testArray"] = "array";
                server.PathContent["/testError"] = "error";
                server.PathContent["/testError1"] = "error1";
                server.PathContent["/testError2"] = "error2";

                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var request = TestFixtures.CreateOpenApiRequest();
                var parameters = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/test",
                    Method = "POST",
                    AuthType = "AK",
                    Style = "ROA",
                    ReqBodyType = "formData",
                    BodyType = "json"
                };

                var result = client.CallApi(parameters, request, runtime);
                var body = TestFixtures.GetBodyMap(result);
                Assert.Equal("test", body["AppId"]);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                parameters.BodyType = "array";
                parameters.Pathname = "/testArray";
                result = client.CallApi(parameters, request, runtime);
                var bodyArray = (List<object>)result["body"];
                Assert.Equal("AppId", bodyArray[0]);
                Assert.Equal("ClassId", bodyArray[1]);
                Assert.Equal("UserId", bodyArray[2]);

                parameters.BodyType = "string";
                parameters.Pathname = "/test";
                result = client.CallApi(parameters, request, runtime);
                Assert.Equal(JsonBody, TestFixtures.GetBodyString(result));

                parameters.BodyType = "binary";
                result = client.CallApi(parameters, request, runtime);
                var stream = (Stream)result["body"];
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    Assert.Equal(JsonBody, reader.ReadToEnd());
                }

                parameters.Pathname = "/testError";
                var ex = Assert.Throws<ClientException>(() => client.CallApi(parameters, request, runtime));
                Assert.Equal("code: 400, error message request id: " + RequestId, ex.Message);
                Assert.Equal(400, ex.StatusCode);
                Assert.Null(ex.AccessDeniedDetail == null || !ex.AccessDeniedDetail.ContainsKey("test") ? null : ex.AccessDeniedDetail["test"]);

                parameters.Pathname = "/testError1";
                ex = Assert.Throws<ClientException>(() => client.CallApi(parameters, request, runtime));
                Assert.Equal(400, ex.StatusCode);
                Assert.Null(ex.AccessDeniedDetail == null || !ex.AccessDeniedDetail.ContainsKey("test") ? null : ex.AccessDeniedDetail["test"]);

                parameters.Pathname = "/testError2";
                ex = Assert.Throws<ClientException>(() => client.CallApi(parameters, request, runtime));
                Assert.Equal(400, ex.StatusCode);
                Assert.NotNull(ex.AccessDeniedDetail);
                Assert.Equal(1L, Convert.ToInt64(ex.AccessDeniedDetail["test"]));
                Assert.Equal("str", ex.AccessDeniedDetail["test1"]);
            }
        }

        [Fact]
        public void TestRequestBodyType()
        {
            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var parameters = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/test",
                    Method = "POST",
                    AuthType = "AK",
                    Style = "RPC",
                    ReqBodyType = "formData",
                    BodyType = "json"
                };
                var bodyMap = new Dictionary<string, object>
                {
                    { "key1", "value" },
                    { "key2", 1 },
                    { "key3", true }
                };
                var request = new OpenApiRequest
                {
                    Body = AlibabaCloud.OpenApiClient.Utils.ParseToMap(bodyMap)
                };

                var result = client.CallApi(parameters, request, runtime);
                var headers = TestFixtures.GetHeaders(result);
                Assert.Equal("key1=value&key2=1&key3=True", TestFixtures.GetHeader(headers, "raw-body"));

                parameters.ReqBodyType = "json";
                result = client.CallApi(parameters, request, runtime);
                headers = TestFixtures.GetHeaders(result);
                Assert.Equal("{\"key1\":\"value\",\"key2\":1,\"key3\":true}", TestFixtures.GetHeader(headers, "raw-body"));

                parameters.ReqBodyType = "byte";
                request = new OpenApiRequest
                {
                    Body = Encoding.UTF8.GetBytes("test byte")
                };
                result = client.CallApi(parameters, request, runtime);
                headers = TestFixtures.GetHeaders(result);
                Assert.Equal("test byte", TestFixtures.GetHeader(headers, "raw-body"));

                parameters.ReqBodyType = "binary";
                request = new OpenApiRequest
                {
                    Stream = new MemoryStream(Encoding.UTF8.GetBytes("test byte"))
                };
                result = client.CallApi(parameters, request, runtime);
                headers = TestFixtures.GetHeaders(result);
                Assert.Equal("test byte", TestFixtures.GetHeader(headers, "raw-body"));
            }
        }

        [Fact]
        public void TestResponseBodyTypeRPC()
        {
            using (var server = new MockHttpServer())
            {
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.SignatureAlgorithm = "v2";
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var request = TestFixtures.CreateOpenApiRequest();
                var parameters = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/test",
                    Method = "POST",
                    AuthType = "AK",
                    Style = "RPC",
                    ReqBodyType = "formData",
                    BodyType = "string"
                };

                var result = client.CallApi(parameters, request, runtime);
                Assert.Equal(JsonBody, TestFixtures.GetBodyString(result));

                parameters.BodyType = "binary";
                result = client.CallApi(parameters, request, runtime);
                using (var reader = new StreamReader((Stream)result["body"], Encoding.UTF8))
                {
                    Assert.Equal(JsonBody, reader.ReadToEnd());
                }
            }
        }

        [Fact]
        public void TestResponseBodyTypeROA()
        {
            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
                server.PathContent["/testArray"] = "array";
                server.PathContent["/testError"] = "error";
                server.PathContent["/testError1"] = "error1";
                server.PathContent["/testError2"] = "error2";

                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.SignatureAlgorithm = "v2";
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var request = TestFixtures.CreateOpenApiRequest();
                var parameters = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/test",
                    Method = "POST",
                    AuthType = "AK",
                    Style = "ROA",
                    ReqBodyType = "formData",
                    BodyType = "json"
                };

                var result = client.CallApi(parameters, request, runtime);
                Assert.Equal("test", TestFixtures.GetBodyMap(result)["AppId"]);

                parameters.BodyType = "array";
                parameters.Pathname = "/testArray";
                result = client.CallApi(parameters, request, runtime);
                var bodyArray = (List<object>)result["body"];
                Assert.Equal("AppId", bodyArray[0]);

                parameters.BodyType = "string";
                parameters.Pathname = "/test";
                result = client.CallApi(parameters, request, runtime);
                Assert.Equal(JsonBody, TestFixtures.GetBodyString(result));

                parameters.BodyType = "binary";
                result = client.CallApi(parameters, request, runtime);
                using (var reader = new StreamReader((Stream)result["body"], Encoding.UTF8))
                {
                    Assert.Equal(JsonBody, reader.ReadToEnd());
                }

                parameters.Pathname = "/testError";
                Assert.Throws<ClientException>(() => client.CallApi(parameters, request, runtime));

                parameters.Pathname = "/testError2";
                var ex = Assert.Throws<ClientException>(() => client.CallApi(parameters, request, runtime));
                Assert.Equal(1L, Convert.ToInt64(ex.AccessDeniedDetail["test"]));
            }
        }

        [Fact]
        public void TestRetryWithError()
        {
            using (var server = new MockHttpServer())
            {
                server.DefaultContent = "serverError";
                server.PathContent["/test"] = "serverError";

                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.SignatureAlgorithm = "v2";
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                runtime.Autoretry = true;
                runtime.MaxAttempts = 3;
                runtime.BackoffPolicy = "fix";
                runtime.BackoffPeriod = 1;

                var client = new TestClient(config);
                var request = TestFixtures.CreateOpenApiRequest();
                var parameters = new Params
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

                var ex = Assert.Throws<ServerException>(() => client.CallApi(parameters, request, runtime));
                Assert.Equal("code: 500, error message request id: " + RequestId, ex.Message);

                parameters.Pathname = "/test";
                parameters.Style = "ROA";
                ex = Assert.Throws<ServerException>(() => client.CallApi(parameters, request, runtime));
                Assert.Equal("code: 500, error message request id: " + RequestId, ex.Message);

                parameters.ReqBodyType = "json";
                ex = Assert.Throws<ServerException>(() => client.CallApi(parameters, request, runtime));

                parameters.ReqBodyType = "formData";
                parameters.Style = "RPC";
                client = new TestClient(config);
                client.ProductId = "test";
                client.SetGatewayClient(new AlibabaCloud.GatewayPop.Client());
                Assert.ThrowsAny<Exception>(() => client.Execute(parameters, request, runtime));

                parameters.BodyType = "binary";
                Assert.ThrowsAny<Exception>(() => client.Execute(parameters, request, runtime));
            }
        }

        [Fact]
        public void TestCallSSeApiWithV3Sign_AK_Form()
        {
            using (var server = new MockHttpServer())
            {
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var request = TestFixtures.CreateOpenApiRequest();
                var parameters = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/sse",
                    Method = "GET",
                    AuthType = "AK",
                    Style = "ROA",
                    ReqBodyType = "formData",
                    BodyType = "json"
                };

                var events = new List<SSEResponse>();
                foreach (var resp in client.CallSSEApi(parameters, request, runtime))
                {
                    events.Add(resp);
                }

                Assert.Equal(5, events.Count);
                for (int i = 0; i < events.Count; i++)
                {
                    Assert.Equal("{\"count\": " + i + "}", events[i].Event.Data);
                    Assert.Equal("sse-test", events[i].Event.Id);
                    Assert.Equal("flow", events[i].Event.Event);
                }
            }
        }

#if NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
        [Fact]
        public async Task TestCallAsyncSSEApiWithV3Sign_AK_Form()
        {
            using (var server = new MockHttpServer())
            {
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var request = TestFixtures.CreateOpenApiRequest();
                var parameters = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/sse",
                    Method = "GET",
                    AuthType = "AK",
                    Style = "ROA",
                    ReqBodyType = "formData",
                    BodyType = "json"
                };

                var events = new List<SSEResponse>();
                await foreach (var resp in client.CallAsyncSSEApi(parameters, request, runtime))
                {
                    events.Add(resp);
                }

                Assert.Equal(5, events.Count);
                for (int i = 0; i < events.Count; i++)
                {
                    Assert.Equal("{\"count\": " + i + "}", events[i].Event.Data);
                    Assert.Equal("sse-test", events[i].Event.Id);
                    Assert.Equal("flow", events[i].Event.Event);
                }
            }
        }

        [Fact]
        public async Task TestCallAsyncSSEApi_JsonBody_And_RpcStyle()
        {
            using (var server = new MockHttpServer())
            {
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var request = new OpenApiRequest
                {
                    Body = new Dictionary<string, object> { { "k", "v" } }
                };
                var parameters = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/sse",
                    Method = "POST",
                    AuthType = "AK",
                    Style = "RPC",
                    ReqBodyType = "json",
                    BodyType = "json"
                };

                var events = new List<SSEResponse>();
                await foreach (var resp in client.CallAsyncSSEApi(parameters, request, runtime))
                {
                    events.Add(resp);
                }
                Assert.Equal(5, events.Count);
            }
        }

        [Fact]
        public async Task TestCallAsyncSSEApi_ByteBody_Anonymous()
        {
            using (var server = new MockHttpServer())
            {
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var request = new OpenApiRequest
                {
                    Body = Encoding.UTF8.GetBytes("raw-bytes")
                };
                var parameters = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/sse",
                    Method = "POST",
                    AuthType = "Anonymous",
                    Style = "ROA",
                    ReqBodyType = "byte",
                    BodyType = "byte"
                };

                var events = new List<SSEResponse>();
                await foreach (var resp in client.CallAsyncSSEApi(parameters, request, runtime))
                {
                    events.Add(resp);
                }
                Assert.Equal(5, events.Count);
            }
        }

        [Fact]
        public async Task TestCallAsyncSSEApi_StreamBody_Bearer()
        {
            using (var server = new MockHttpServer())
            {
                var config = TestFixtures.CreateBearerTokenConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var request = new OpenApiRequest
                {
                    Stream = new MemoryStream(Encoding.UTF8.GetBytes("stream-body"))
                };
                var parameters = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/sse",
                    Method = "POST",
                    AuthType = "AK",
                    Style = "ROA",
                    ReqBodyType = "json",
                    BodyType = "json"
                };

                var events = new List<SSEResponse>();
                await foreach (var resp in client.CallAsyncSSEApi(parameters, request, runtime))
                {
                    events.Add(resp);
                }
                Assert.Equal(5, events.Count);
            }
        }

        [Fact]
        public async Task TestCallAsyncSSEApi_ErrorJson()
        {
            using (var server = new MockHttpServer())
            {
                server.SseErrorMode = true;
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var request = TestFixtures.CreateOpenApiRequest();
                var parameters = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/sse",
                    Method = "GET",
                    AuthType = "AK",
                    Style = "ROA",
                    ReqBodyType = "formData",
                    BodyType = "json"
                };

                await Assert.ThrowsAsync<Darabonba.Exceptions.DaraException>(async () =>
                {
                    await foreach (var _ in client.CallAsyncSSEApi(parameters, request, runtime))
                    {
                    }
                });
            }
        }

        [Fact]
        public async Task TestCallAsyncSSEApi_ErrorXml()
        {
            using (var server = new MockHttpServer())
            {
                server.SseErrorMode = true;
                server.SseErrorContentType = "text/xml;charset=utf-8";
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var request = TestFixtures.CreateOpenApiRequest();
                var parameters = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/sse",
                    Method = "GET",
                    AuthType = "AK",
                    Style = "ROA",
                    ReqBodyType = "formData",
                    BodyType = "json"
                };

                var ex = await Assert.ThrowsAsync<Darabonba.Exceptions.DaraException>(async () =>
                {
                    await foreach (var _ in client.CallAsyncSSEApi(parameters, request, runtime))
                    {
                    }
                });
                Assert.Contains("sse failed", ex.Message);
            }
        }
#endif

        [Fact]
        public void TestThrottlingBackoffRetry_ListProductQuotas()
        {
            using (var server = new MockHttpServer())
            {
                server.ThrottlingMode = true;
                server.ThrottleCount = 2;
                server.RetryAfterMs = 1000;

                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                config.RetryOptions = TestFixtures.CreateThrottlingRetryOptions();
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);

                var request = new OpenApiRequest
                {
                    Body = AlibabaCloud.OpenApiClient.Utils.ParseToMap(new Dictionary<string, object>
                    {
                        { "ProductCode", "Ecs" }
                    })
                };
                var parameters = new Params
                {
                    Action = "ListProductQuotas",
                    Version = "2020-05-10",
                    Protocol = "HTTPS",
                    Pathname = "/",
                    Method = "POST",
                    AuthType = "AK",
                    Style = "RPC",
                    ReqBodyType = "formData",
                    BodyType = "json"
                };

                var stopwatch = Stopwatch.StartNew();
                var result = client.CallApi(parameters, request, runtime);
                stopwatch.Stop();

                Assert.Equal(3, server.RequestCount);
                Assert.Equal("", server.RetryAttempts[0]);
                Assert.Equal("1", server.RetryAttempts[1]);
                Assert.Equal("2", server.RetryAttempts[2]);
                Assert.Equal("", server.RetryDelays[0]);
                Assert.Equal("100", server.RetryDelays[1]);
                Assert.Equal("100", server.RetryDelays[2]);
                Assert.True(stopwatch.ElapsedMilliseconds >= 150, "expected throttling backoff delay, elapsed " + stopwatch.Elapsed);

                var headers = TestFixtures.GetHeaders(result);
                Assert.Equal("ProductCode=Ecs", headers["raw-body"]);
                Assert.Equal("ListProductQuotas", headers["x-acs-action"]);
                Assert.Equal("2020-05-10", headers["x-acs-version"]);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                Assert.Equal(RequestId, TestFixtures.GetBodyMap(result)["RequestId"]);
            }
        }

        [Fact]
        public void TestThrottlingBackoffRetry_ListProductQuotasExhausted()
        {
            using (var server = new MockHttpServer())
            {
                server.ThrottlingMode = true;
                server.ThrottleCount = 3;
                server.RetryAfterMs = 1000;

                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                config.RetryOptions = TestFixtures.CreateThrottlingRetryOptions();
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);

                var request = new OpenApiRequest
                {
                    Body = AlibabaCloud.OpenApiClient.Utils.ParseToMap(new Dictionary<string, object>
                    {
                        { "ProductCode", "Ecs" }
                    })
                };
                var parameters = new Params
                {
                    Action = "ListProductQuotas",
                    Version = "2020-05-10",
                    Protocol = "HTTPS",
                    Pathname = "/",
                    Method = "POST",
                    AuthType = "AK",
                    Style = "RPC",
                    ReqBodyType = "formData",
                    BodyType = "json"
                };

                var ex = Assert.Throws<ThrottlingException>(() => client.CallApi(parameters, request, runtime));
                Assert.Equal("Throttling", ex.Code);
                Assert.Equal(3, server.RequestCount);
            }
        }
    }
}
