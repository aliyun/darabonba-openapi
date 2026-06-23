using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using AlibabaCloud.OpenApiClient.Exceptions;
using AlibabaCloud.OpenApiClient.Models;
using Darabonba.Models;
using OpenApiClientUnitTests.Mock;
using Xunit;

namespace OpenApiClientUnitTests
{
    public class ClientTest
    {
        private const string RequestId = "A45EE076-334D-5012-9746-A8F828D20FD4";
        private const string InvalidCredentialsMessage =
            "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.";

        [Fact]
        public void TestCallApiForRPCWithV2Sign_AK_Form()
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
                    Pathname = "/",
                    Method = "POST",
                    AuthType = "AK",
                    Style = "RPC",
                    ReqBodyType = "formData",
                    BodyType = "json"
                };

                var result = client.CallApi(parameters, request, runtime);
                var headers = TestFixtures.GetHeaders(result);
                Assert.Equal("key1=value&key2=1&key3=True", headers["raw-body"]);
                TestFixtures.AssertQueryContains(headers["raw-query"],
                    "AccessKeyId=ak", "Action=TestAPI", "Format=json", "SecurityToken=token",
                    "SignatureMethod=HMAC-SHA1", "SignatureVersion=1.0", "Version=2022-06-01",
                    "extends-key=extends-value", "global-query=global-value", "key1=value", "key2=1", "key3=True");
                Assert.Contains("Signature=", headers["raw-query"]);
                Assert.Contains("SignatureNonce=", headers["raw-query"]);
                Assert.Contains("Timestamp=", headers["raw-query"]);
                Assert.Contains("TeaDSL/2 config.userAgent", TestFixtures.GetHeader(headers, "user-agent"));
                Assert.Equal("global-value", TestFixtures.GetHeader(headers, "global-key"));
                Assert.Equal("2022-06-01", TestFixtures.GetHeader(headers, "x-acs-version"));
                Assert.Equal("TestAPI", TestFixtures.GetHeader(headers, "x-acs-action"));
                Assert.Equal(RequestId, TestFixtures.GetHeader(headers, "x-acs-request-id"));

                var body = TestFixtures.GetBodyMap(result);
                Assert.Equal("test", body["AppId"]);
                Assert.Equal("test", body["ClassId"]);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                config = TestFixtures.CreateBearerTokenConfig();
                config.Protocol = "HTTP";
                config.SignatureAlgorithm = "v2";
                config.Endpoint = server.Endpoint;
                client = new TestClient(config);
                result = client.CallApi(parameters, request, runtime);
                headers = TestFixtures.GetHeaders(result);
                TestFixtures.AssertQueryContains(headers["raw-query"],
                    "Action=TestAPI", "BearerToken=token", "Format=json", "SignatureType=BEARERTOKEN", "Version=2022-06-01",
                    "extends-key=extends-value", "key1=value", "key2=1", "key3=True");

                config = TestFixtures.CreateAnonymousConfig();
                config.Protocol = "HTTP";
                config.SignatureAlgorithm = "v2";
                config.Endpoint = server.Endpoint;
                client = new TestClient(config);
                var ex = Assert.Throws<ClientException>(() => client.CallApi(parameters, request, runtime));
                Assert.Equal("InvalidCredentials", ex.Code);
                Assert.Equal(InvalidCredentialsMessage, ex.Message);
            }
        }

        [Fact]
        public void TestCallApiForRPCWithV2Sign_Anonymous_JSON()
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
                    Pathname = "/",
                    Method = "POST",
                    AuthType = "Anonymous",
                    Style = "RPC",
                    ReqBodyType = "json",
                    BodyType = "json"
                };

                var result = client.CallApi(parameters, request, runtime);
                var headers = TestFixtures.GetHeaders(result);
                Assert.Equal("key1=value&key2=1&key3=True", headers["raw-body"]);
                TestFixtures.AssertQueryContains(headers["raw-query"],
                    "Action=TestAPI", "Format=json", "Version=2022-06-01",
                    "extends-key=extends-value", "global-query=global-value", "key1=value", "key2=1", "key3=True");
                Assert.Contains("SignatureNonce=", headers["raw-query"]);
                Assert.Contains("Timestamp=", headers["raw-query"]);
                Assert.Contains("TeaDSL/2 config.userAgent", TestFixtures.GetHeader(headers, "user-agent"));
                Assert.Equal("global-value", TestFixtures.GetHeader(headers, "global-key"));
                Assert.Equal("extends-value", TestFixtures.GetHeader(headers, "extends-key"));
                Assert.Equal(RequestId, TestFixtures.GetHeader(headers, "x-acs-request-id"));
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                client.SetRpcHeaders(new Dictionary<string, string> { { "extends-key", "test" } });
                result = client.CallApi(parameters, request, runtime);
                headers = TestFixtures.GetHeaders(result);
                Assert.Equal("test", headers["extends-key"]);
            }
        }

        [Fact]
        public void TestCallApiForROAWithV2Sign_HTTPS_AK_Form()
        {
            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
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
                var headers = TestFixtures.GetHeaders(result);
                Assert.Equal("key1=value&key2=1&key3=True", headers["raw-body"]);
                TestFixtures.AssertQueryContains(headers["raw-query"],
                    "extends-key=extends-value", "global-query=global-value", "key1=value", "key2=1", "key3=True");
                Assert.Contains("TeaDSL/2 config.userAgent", TestFixtures.GetHeader(headers, "user-agent"));
                Assert.Contains("acs ak:", TestFixtures.GetHeader(headers, "authorization"));
                Assert.Equal("sdk", TestFixtures.GetHeader(headers, "for-test"));
                Assert.Equal("application/json", TestFixtures.GetHeader(headers, "accept"));
                Assert.Equal("HMAC-SHA1", TestFixtures.GetHeader(headers, "x-acs-signature-method"));
                Assert.Equal("ak", TestFixtures.GetHeader(headers, "x-acs-accesskey-id"));
                Assert.Equal("token", TestFixtures.GetHeader(headers, "x-acs-security-token"));
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                client.SetRpcHeaders(new Dictionary<string, string> { { "extends-key", "test" } });
                result = client.CallApi(parameters, request, runtime);
                headers = TestFixtures.GetHeaders(result);
                Assert.Equal("extends-value", headers["extends-key"]);

                config = TestFixtures.CreateBearerTokenConfig();
                config.Protocol = "HTTP";
                config.SignatureAlgorithm = "v2";
                config.Endpoint = server.Endpoint;
                client = new TestClient(config);
                result = client.CallApi(parameters, request, runtime);
                headers = TestFixtures.GetHeaders(result);
                Assert.Equal("token", headers["x-acs-bearer-token"]);
                Assert.Equal("BEARERTOKEN", headers["x-acs-signature-type"]);

                config = TestFixtures.CreateAnonymousConfig();
                config.Protocol = "HTTP";
                config.SignatureAlgorithm = "v2";
                config.Endpoint = server.Endpoint;
                client = new TestClient(config);
                var ex = Assert.Throws<ClientException>(() => client.CallApi(parameters, request, runtime));
                Assert.Equal("InvalidCredentials", ex.Code);

                parameters = new Params
                {
                    Action = "TestAPI",
                    Version = "2022-06-01",
                    Protocol = "HTTPS",
                    Pathname = "/test",
                    Method = "POST",
                    AuthType = "AK",
                    Style = "ROA",
                    ReqBodyType = "json",
                    BodyType = "json"
                };
                config = TestFixtures.CreateBearerTokenConfig();
                config.Protocol = "HTTP";
                config.SignatureAlgorithm = "v2";
                config.Endpoint = server.Endpoint;
                client = new TestClient(config);
                result = client.CallApi(parameters, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                config = TestFixtures.CreateAnonymousConfig();
                config.Protocol = "HTTP";
                config.SignatureAlgorithm = "v2";
                config.Endpoint = server.Endpoint;
                client = new TestClient(config);
                Assert.Throws<ClientException>(() => client.CallApi(parameters, request, runtime));
            }
        }

        [Fact]
        public void TestCallApiForROAWithV2Sign_Anonymous_JSON()
        {
            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
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
                    AuthType = "Anonymous",
                    Style = "ROA",
                    ReqBodyType = "json",
                    BodyType = "json"
                };

                var result = client.CallApi(parameters, request, runtime);
                var headers = TestFixtures.GetHeaders(result);
                Assert.Equal("{\"key1\":\"value\",\"key2\":1,\"key3\":true}", headers["raw-body"]);
                Assert.Equal(RequestId, headers["x-acs-request-id"]);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
            }
        }

        [Fact]
        public void TestCallApiForRPCWithV3Sign_AK_Form()
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
                    Pathname = "/",
                    Method = "POST",
                    AuthType = "AK",
                    Style = "RPC",
                    ReqBodyType = "formData",
                    BodyType = "json"
                };

                var result = client.CallApi(parameters, request, runtime);
                var headers = TestFixtures.GetHeaders(result);
                Assert.Equal("key1=value&key2=1&key3=True", headers["raw-body"]);
                Assert.Equal("sdk", headers["for-test"]);
                Assert.Equal("ak", headers["x-acs-accesskey-id"]);
                Assert.Matches(new Regex("ACS3-HMAC-SHA256 Credential=ak,.+Signature=.+"), TestFixtures.GetHeader(headers, "Authorization"));
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                config = TestFixtures.CreateBearerTokenConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                client = new TestClient(config);
                result = client.CallApi(parameters, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                config = TestFixtures.CreateAnonymousConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                client = new TestClient(config);
                Assert.Throws<ClientException>(() => client.CallApi(parameters, request, runtime));
            }
        }

        [Fact]
        public void TestCallApiForRPCWithV3Sign_Anonymous_JSON()
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
                    Pathname = "/",
                    Method = "POST",
                    AuthType = "Anonymous",
                    Style = "RPC",
                    ReqBodyType = "json",
                    BodyType = "json"
                };

                var result = client.CallApi(parameters, request, runtime);
                var headers = TestFixtures.GetHeaders(result);
                Assert.Equal("{\"key1\":\"value\",\"key2\":1,\"key3\":true}", headers["raw-body"]);
                TestFixtures.AssertQueryContains(headers["raw-query"], "Format=json", "key1=value", "key2=1", "key3=True");
                Assert.Equal("TestAPI", TestFixtures.GetHeader(headers, "x-acs-action"));
                Assert.Equal("2022-06-01", TestFixtures.GetHeader(headers, "x-acs-version"));
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
            }
        }

        [Fact]
        public void TestCallApiForROAWithV3Sign_AK_Form()
        {
            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
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
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                config = TestFixtures.CreateBearerTokenConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                client = new TestClient(config);
                result = client.CallApi(parameters, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));

                config = TestFixtures.CreateAnonymousConfig();
                config.Protocol = "HTTP";
                config.Endpoint = server.Endpoint;
                client = new TestClient(config);
                Assert.Throws<ClientException>(() => client.CallApi(parameters, request, runtime));
            }
        }

        [Fact]
        public void TestCallApiForROAWithV3Sign_Anonymous_JSON()
        {
            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
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
                    AuthType = "Anonymous",
                    Style = "RPC",
                    ReqBodyType = "json",
                    BodyType = "json"
                };

                var result = client.CallApi(parameters, request, runtime);
                Assert.Equal(200, TestFixtures.GetStatusCode(result));
            }
        }
    }
}
