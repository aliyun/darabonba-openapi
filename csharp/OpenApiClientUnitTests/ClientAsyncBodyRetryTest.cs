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
    public class ClientAsyncBodyRetryTest
    {
        private const string RequestId = "A45EE076-334D-5012-9746-A8F828D20FD4";
        private const string JsonBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";

        [Fact]
        public async Task TestAsyncResponseBodyTypesAndAuth()
        {
            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
                server.PathContent["/testArray"] = "array";
                server.PathContent["/testError"] = "error";
                server.PathContent["/testError1"] = "error1";
                server.PathContent["/testError2"] = "error2";
                server.PathContent["/xml"] = "xml";
                server.PathContent["/server"] = "serverError";
                server.PathContent["/empty"] = "empty204";

                var runtime = TestFixtures.CreateRuntimeOptions();
                var request = TestFixtures.CreateOpenApiRequest();

                var v3Config = TestFixtures.CreateConfig();
                v3Config.Protocol = "HTTP";
                v3Config.Endpoint = server.Endpoint;
                var v3 = new TestClient(v3Config);
                var parameters = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/test",
                    Method = "POST", AuthType = "AK", Style = "ROA", ReqBodyType = "formData", BodyType = "json"
                };
                var result = await v3.CallApiAsync(parameters, request, runtime);
                Assert.Equal("test", TestFixtures.GetBodyMap(result)["AppId"]);

                parameters.BodyType = "array";
                parameters.Pathname = "/testArray";
                result = await v3.CallApiAsync(parameters, request, runtime);
                Assert.Equal("AppId", ((List<object>)result["body"])[0]);

                parameters.BodyType = "string";
                parameters.Pathname = "/test";
                result = await v3.CallApiAsync(parameters, request, runtime);
                Assert.Equal(JsonBody, TestFixtures.GetBodyString(result));

                parameters.BodyType = "binary";
                result = await v3.CallApiAsync(parameters, request, runtime);
                using (var reader = new StreamReader((Stream)result["body"], Encoding.UTF8))
                {
                    Assert.Equal(JsonBody, reader.ReadToEnd());
                }

                parameters.BodyType = "byte";
                result = await v3.CallApiAsync(parameters, request, runtime);
                Assert.NotNull(result["body"]);
                parameters.BodyType = "none";
                result = await v3.CallApiAsync(parameters, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                parameters.BodyType = "json";
                parameters.Pathname = "/testError";
                await Assert.ThrowsAsync<ClientException>(() => v3.CallApiAsync(parameters, request, runtime));
                parameters.Pathname = "/testError1";
                await Assert.ThrowsAsync<ClientException>(() => v3.CallApiAsync(parameters, request, runtime));
                parameters.Pathname = "/testError2";
                var ex = await Assert.ThrowsAsync<ClientException>(() => v3.CallApiAsync(parameters, request, runtime));
                Assert.Equal(1L, Convert.ToInt64(ex.AccessDeniedDetail["test"]));
                parameters.Pathname = "/xml";
                try { await v3.CallApiAsync(parameters, request, runtime); } catch (Exception) { }

                var v2Config = TestFixtures.CreateConfig();
                v2Config.Protocol = "HTTP";
                v2Config.SignatureAlgorithm = "v2";
                v2Config.Endpoint = server.Endpoint;
                var v2 = new TestClient(v2Config);
                var rpc = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/",
                    Method = "POST", AuthType = "AK", Style = "RPC", ReqBodyType = "formData", BodyType = "string"
                };
                result = await v2.CallApiAsync(rpc, request, runtime);
                Assert.Equal(JsonBody, TestFixtures.GetBodyString(result));
                rpc.BodyType = "binary";
                result = await v2.CallApiAsync(rpc, request, runtime);
                using (var reader = new StreamReader((Stream)result["body"], Encoding.UTF8))
                {
                    Assert.Equal(JsonBody, reader.ReadToEnd());
                }
                rpc.BodyType = "json";
                result = await v2.CallApiAsync(rpc, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                rpc.BodyType = "byte";
                result = await v2.CallApiAsync(rpc, request, runtime);
                Assert.NotNull(result["body"]);
                rpc.BodyType = "none";
                result = await v2.CallApiAsync(rpc, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                rpc.AuthType = "Anonymous";
                rpc.ReqBodyType = "json";
                rpc.BodyType = "json";
                result = await v2.CallApiAsync(rpc, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                var roa = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/test",
                    Method = "POST", AuthType = "AK", Style = "ROA", ReqBodyType = "formData", BodyType = "json"
                };
                result = await v2.CallApiAsync(roa, request, runtime);
                Assert.Equal("test", TestFixtures.GetBodyMap(result)["AppId"]);
                roa.BodyType = "array";
                roa.Pathname = "/testArray";
                result = await v2.CallApiAsync(roa, request, runtime);
                Assert.Equal("AppId", ((List<object>)result["body"])[0]);
                roa.BodyType = "string";
                roa.Pathname = "/test";
                result = await v2.CallApiAsync(roa, request, runtime);
                Assert.Equal(JsonBody, TestFixtures.GetBodyString(result));
                roa.BodyType = "binary";
                result = await v2.CallApiAsync(roa, request, runtime);
                using (var reader = new StreamReader((Stream)result["body"], Encoding.UTF8))
                {
                    Assert.Equal(JsonBody, reader.ReadToEnd());
                }
                roa.Pathname = "/testError";
                await Assert.ThrowsAsync<ClientException>(() => v2.CallApiAsync(roa, request, runtime));
                roa.Pathname = "/testError2";
                ex = await Assert.ThrowsAsync<ClientException>(() => v2.CallApiAsync(roa, request, runtime));
                Assert.Equal(1L, Convert.ToInt64(ex.AccessDeniedDetail["test"]));
                roa.ReqBodyType = "json";
                roa.Pathname = "/test";
                roa.BodyType = "json";
                roa.AuthType = "Anonymous";
                result = await v2.CallApiAsync(roa, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
            }
        }

        [Fact]
        public async Task TestAsyncRequestBodiesDirectAndRetry()
        {
            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
                server.PathContent["/testArray"] = "array";
                server.PathContent["/empty"] = "empty204";
                server.PathContent["/xml"] = "xml";
                server.PathContent["/server"] = "serverError";
                server.PathContent["/testError"] = "error";

                var runtime = TestFixtures.CreateRuntimeOptions();
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                var client = new TestClient(config);
                var parameters = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/test",
                    Method = "POST", AuthType = "AK", Style = "RPC", ReqBodyType = "formData", BodyType = "json"
                };
                var bodyMap = new Dictionary<string, object>
                {
                    { "key1", "value" },
                    { "key2", 1 },
                    { "key3", true }
                };
                var request = new OpenApiRequest { Body = AlibabaCloud.OpenApiClient.Utils.ParseToMap(bodyMap) };
                var result = await client.CallApiAsync(parameters, request, runtime);
                Assert.Equal("key1=value&key2=1&key3=True", TestFixtures.GetHeader(TestFixtures.GetHeaders(result), "raw-body"));

                parameters.ReqBodyType = "json";
                result = await client.CallApiAsync(parameters, request, runtime);
                Assert.Equal("{\"key1\":\"value\",\"key2\":1,\"key3\":true}", TestFixtures.GetHeader(TestFixtures.GetHeaders(result), "raw-body"));

                parameters.ReqBodyType = "byte";
                request = new OpenApiRequest { Body = Encoding.UTF8.GetBytes("test byte") };
                result = await client.CallApiAsync(parameters, request, runtime);
                Assert.Equal("test byte", TestFixtures.GetHeader(TestFixtures.GetHeaders(result), "raw-body"));

                parameters.ReqBodyType = "binary";
                request = new OpenApiRequest { Stream = new MemoryStream(Encoding.UTF8.GetBytes("test byte")) };
                result = await client.CallApiAsync(parameters, request, runtime);
                Assert.Equal("test byte", TestFixtures.GetHeader(TestFixtures.GetHeaders(result), "raw-body"));

                var openApiRequest = TestFixtures.CreateOpenApiRequest();
                foreach (var bodyType in new[] { "json", "string", "byte", "none", "binary", "array" })
                {
                    result = await client.DoRPCRequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", bodyType, openApiRequest, runtime);
                    Assert.NotNull(result);
                    result = await client.DoROARequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "AK", "/test", bodyType, openApiRequest, runtime);
                    Assert.NotNull(result);
                    result = await client.DoROARequestWithFormAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/test", bodyType, openApiRequest, runtime);
                    Assert.NotNull(result);
                }

                result = await client.DoRPCRequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "json", openApiRequest, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                await Assert.ThrowsAsync<ClientException>(() =>
                    client.DoROARequestAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/testError", "json", openApiRequest, runtime));
                await Assert.ThrowsAsync<ClientException>(() =>
                    client.DoROARequestWithFormAsync("TestAPI", "2022-06-01", "HTTPS", "POST", "Anonymous", "/testError", "json", openApiRequest, runtime));

                var doParams = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/test",
                    Method = "POST", AuthType = "AK", Style = "RPC", ReqBodyType = "json", BodyType = "json"
                };
                result = await client.DoRequestAsync(doParams, openApiRequest, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                doParams.Style = "ROA";
                doParams.ReqBodyType = "formData";
                result = await client.DoRequestAsync(doParams, openApiRequest, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                doParams.AuthType = "Anonymous";
                doParams.Style = "RPC";
                result = await client.DoRequestAsync(doParams, openApiRequest, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                doParams.BodyType = "array";
                doParams.Pathname = "/testArray";
                result = await client.DoRequestAsync(doParams, openApiRequest, runtime);
                Assert.NotNull(result["body"]);

                var xmlParams = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/xml",
                    Method = "POST", AuthType = "Anonymous", Style = "ROA", ReqBodyType = "json", BodyType = "json"
                };
                try { await client.DoRequestAsync(xmlParams, openApiRequest, runtime); } catch (Exception) { }

                var retryRuntime = TestFixtures.CreateRuntimeOptions();
                retryRuntime.Autoretry = true;
                retryRuntime.MaxAttempts = 3;
                retryRuntime.BackoffPolicy = "fix";
                retryRuntime.BackoffPeriod = 1;
                var v2Config = TestFixtures.CreateConfig();
                v2Config.Protocol = "HTTP";
                v2Config.SignatureAlgorithm = "v2";
                v2Config.Endpoint = server.Endpoint;
                var v2 = new TestClient(v2Config);
                var fail = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/server",
                    Method = "POST", AuthType = "Anonymous", Style = "ROA", ReqBodyType = "json", BodyType = "json"
                };
                await Assert.ThrowsAsync<ServerException>(() => v2.CallApiAsync(fail, openApiRequest, retryRuntime));
                fail.ReqBodyType = "formData";
                await Assert.ThrowsAsync<ServerException>(() => v2.CallApiAsync(fail, openApiRequest, retryRuntime));
                fail.Style = "RPC";
                fail.Pathname = "/";
                server.DefaultContent = "serverError";
                await Assert.ThrowsAsync<ServerException>(() => v2.CallApiAsync(fail, openApiRequest, retryRuntime));
                server.DefaultContent = "json";

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
                    Method = "POST", AuthType = "AK", Style = "RPC", ReqBodyType = "formData", BodyType = "json"
                };
                result = await execClient.ExecuteAsync(execParams, openApiRequest, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                execParams.BodyType = "string";
                result = await execClient.ExecuteAsync(execParams, openApiRequest, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                execParams.BodyType = "byte";
                result = await execClient.ExecuteAsync(execParams, openApiRequest, runtime);
                Assert.NotNull(result["body"]);
                execParams.BodyType = "none";
                result = await execClient.ExecuteAsync(execParams, openApiRequest, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                execParams.BodyType = "binary";
                result = await execClient.ExecuteAsync(execParams, openApiRequest, runtime);
                Assert.NotNull(result["body"]);
                execParams.BodyType = "array";
                execParams.Pathname = "/testArray";
                result = await execClient.ExecuteAsync(execParams, openApiRequest, runtime);
                Assert.NotNull(result["body"]);
                execParams.Pathname = "/server";
                execParams.BodyType = "json";
                try { await execClient.ExecuteAsync(execParams, openApiRequest, retryRuntime); } catch (Exception) { }

                var bearer = TestFixtures.CreateBearerTokenConfig();
                bearer.Protocol = "HTTP";
                bearer.Endpoint = server.Endpoint;
                var bearerClient = new TestClient(bearer);
                var bearerParams = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/test",
                    Method = "POST", AuthType = "AK", Style = "ROA", ReqBodyType = "json", BodyType = "json"
                };
                result = await bearerClient.CallApiAsync(bearerParams, openApiRequest, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                bearerParams.Style = "RPC";
                bearerParams.Pathname = "/";
                result = await bearerClient.CallApiAsync(bearerParams, openApiRequest, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
            }
        }

        [Fact]
        public async Task TestAsyncThrottlingAndV2AuthMatrix()
        {
            using (var server = new MockHttpServer())
            {
                server.ThrottlingMode = true;
                server.ThrottleCount = 2;
                server.RetryAfterMs = 1;

                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                config.RetryOptions = TestFixtures.CreateThrottlingRetryOptions();
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var request = new OpenApiRequest
                {
                    Body = AlibabaCloud.OpenApiClient.Utils.ParseToMap(new Dictionary<string, object> { { "ProductCode", "Ecs" } })
                };
                var parameters = new Params
                {
                    Action = "ListProductQuotas", Version = "2020-05-10", Protocol = "HTTPS", Pathname = "/",
                    Method = "POST", AuthType = "AK", Style = "RPC", ReqBodyType = "formData", BodyType = "json"
                };
                var result = await client.CallApiAsync(parameters, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                Assert.Equal(3, server.RequestCount);
            }

            using (var server = new MockHttpServer())
            {
                server.ThrottlingMode = true;
                server.ThrottleCount = 3;
                server.RetryAfterMs = 1;
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                config.RetryOptions = TestFixtures.CreateThrottlingRetryOptions();
                var client = new TestClient(config);
                var request = new OpenApiRequest
                {
                    Body = AlibabaCloud.OpenApiClient.Utils.ParseToMap(new Dictionary<string, object> { { "ProductCode", "Ecs" } })
                };
                var parameters = new Params
                {
                    Action = "ListProductQuotas", Version = "2020-05-10", Protocol = "HTTPS", Pathname = "/",
                    Method = "POST", AuthType = "AK", Style = "RPC", ReqBodyType = "formData", BodyType = "json"
                };
                var ex = await Assert.ThrowsAsync<ThrottlingException>(() => client.CallApiAsync(parameters, request, TestFixtures.CreateRuntimeOptions()));
                Assert.Equal("Throttling", ex.Code);
            }

            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
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
                var bearer = TestFixtures.CreateBearerTokenConfig();
                bearer.Protocol = "HTTP";
                bearer.SignatureAlgorithm = "v2";
                bearer.Endpoint = server.Endpoint;
                result = await new TestClient(bearer).CallApiAsync(rpc, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                var anon = TestFixtures.CreateAnonymousConfig();
                anon.Protocol = "HTTP";
                anon.SignatureAlgorithm = "v2";
                anon.Endpoint = server.Endpoint;
                await Assert.ThrowsAsync<ClientException>(() => new TestClient(anon).CallApiAsync(rpc, request, runtime));

                var roa = new Params
                {
                    Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTPS", Pathname = "/test",
                    Method = "POST", AuthType = "AK", Style = "ROA", ReqBodyType = "formData", BodyType = "json"
                };
                result = await v2.CallApiAsync(roa, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await new TestClient(bearer).CallApiAsync(roa, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                await Assert.ThrowsAsync<ClientException>(() => new TestClient(anon).CallApiAsync(roa, request, runtime));

                var v3Config = TestFixtures.CreateConfig();
                v3Config.Protocol = "HTTP";
                v3Config.Endpoint = server.Endpoint;
                var v3 = new TestClient(v3Config);
                result = await v3.CallApiAsync(rpc, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                result = await v3.CallApiAsync(roa, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                rpc.AuthType = "Anonymous";
                rpc.ReqBodyType = "json";
                result = await v3.CallApiAsync(rpc, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                roa.AuthType = "Anonymous";
                roa.ReqBodyType = "json";
                result = await v3.CallApiAsync(roa, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                var v3Bearer = TestFixtures.CreateBearerTokenConfig();
                v3Bearer.Protocol = "HTTP";
                v3Bearer.Endpoint = server.Endpoint;
                roa.AuthType = "AK";
                result = await new TestClient(v3Bearer).CallApiAsync(roa, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
                var v3Anon = TestFixtures.CreateAnonymousConfig();
                v3Anon.Protocol = "HTTP";
                v3Anon.Endpoint = server.Endpoint;
                await Assert.ThrowsAsync<ClientException>(() => new TestClient(v3Anon).CallApiAsync(roa, request, runtime));
            }
        }
    }
}
