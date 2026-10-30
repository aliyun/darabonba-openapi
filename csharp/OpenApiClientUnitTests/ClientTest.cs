using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

        private static Dictionary<string, string> ParseParameters(string value)
        {
            var parameters = new Dictionary<string, string>();
            foreach (string item in (value ?? "").Split('&'))
            {
                if (item.Length == 0) continue;
                var pair = item.Split(new[] { '=' }, 2);
                parameters[System.Net.WebUtility.UrlDecode(pair[0])] = pair.Length == 2 ? System.Net.WebUtility.UrlDecode(pair[1]) : "";
            }
            return parameters;
        }

        private static string Hmac(string text, string secret, string algorithm)
        {
            using (HMAC hmac = algorithm == "HMAC-SHA1" ? (HMAC)new HMACSHA1(Encoding.UTF8.GetBytes(secret)) : new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(text)));
        }

        private static string ExpectedRpcSignature(string query, string body, string algorithm)
        {
            var parameters = ParseParameters(query);
            parameters.Remove("Signature");
            foreach (var pair in ParseParameters(body)) parameters[pair.Key] = pair.Value;
            string canonical = string.Join("&", parameters.OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .Select(pair => Uri.EscapeDataString(pair.Key) + "=" + Uri.EscapeDataString(pair.Value)));
            return Hmac("POST&%2F&" + Uri.EscapeDataString(canonical), "secret&", algorithm);
        }

        private static string ExpectedRoaSignature(Dictionary<string, string> headers, string query, string algorithm)
        {
            string canonicalHeaders = string.Concat(headers.Where(pair => pair.Key.StartsWith("x-acs-", StringComparison.OrdinalIgnoreCase))
                .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
                .Select(pair => pair.Key.ToLowerInvariant() + ":" + pair.Value.Trim() + "\n"));
            var parameters = ParseParameters(query);
            string canonicalQuery = string.Join("&", parameters.OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .Select(pair => pair.Value.Length == 0 ? pair.Key : pair.Key + "=" + pair.Value));
            string resource = "/test" + (canonicalQuery.Length == 0 ? "" : "?" + canonicalQuery);
            string text = "POST\n" + headers["accept"] + "\n\n" + headers["content-type"] + "\n" + headers["date"] + "\n" + canonicalHeaders + resource;
            return Hmac(text, "secret", algorithm);
        }

        [Theory]
        [InlineData(null, false, "HMAC-SHA256", 32)]
        [InlineData("HMAC-SHA1", false, "HMAC-SHA1", 20)]
        [InlineData("HMAC-SHA256", true, "HMAC-SHA1", 20)]
        public void TestCallApiForRPCWithV2Sign_AK_Form(string configuredAlgorithm, bool overrideHeader, string expectedAlgorithm, int signatureBytes)
        {
            using (var server = new MockHttpServer())
            {
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.SignatureAlgorithm = "v2";
                if (configuredAlgorithm != null) config.GlobalParameters.Headers["x-acs-signature-method"] = configuredAlgorithm;
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var request = TestFixtures.CreateOpenApiRequest();
                if (overrideHeader) request.Headers["X-Acs-Signature-Method"] = "HMAC-SHA1";
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
                    "SignatureMethod=" + expectedAlgorithm, "SignatureVersion=1.0", "Version=2022-06-01",
                    "extends-key=extends-value", "global-query=global-value", "key1=value", "key2=1", "key3=True");
                Assert.Contains("Signature=", headers["raw-query"]);
                var signaturePart = Array.Find(headers["raw-query"].Split('&'), part => part.StartsWith("Signature="));
                string signature = System.Net.WebUtility.UrlDecode(signaturePart.Substring("Signature=".Length));
                Assert.Equal(signatureBytes, Convert.FromBase64String(signature).Length);
                Assert.Equal(ExpectedRpcSignature(headers["raw-query"], headers["raw-body"], expectedAlgorithm), signature);
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

        [Theory]
        [InlineData(null, false, "HMAC-SHA256", 32)]
        [InlineData("HMAC-SHA1", false, "HMAC-SHA1", 20)]
        [InlineData("HMAC-SHA256", true, "HMAC-SHA1", 20)]
        public void TestCallApiForROAWithV2Sign_HTTPS_AK_Form(string configuredAlgorithm, bool overrideHeader, string expectedAlgorithm, int signatureBytes)
        {
            using (var server = new MockHttpServer())
            {
                server.PathContent["/test"] = "json";
                var config = TestFixtures.CreateConfig();
                config.Protocol = "HTTP";
                config.SignatureAlgorithm = "v2";
                if (configuredAlgorithm != null) config.GlobalParameters.Headers["x-acs-signature-method"] = configuredAlgorithm;
                config.Endpoint = server.Endpoint;
                var runtime = TestFixtures.CreateRuntimeOptions();
                var client = new TestClient(config);
                var request = TestFixtures.CreateOpenApiRequest();
                if (overrideHeader) request.Headers["X-Acs-Signature-Method"] = "HMAC-SHA1";
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
                Assert.Equal(expectedAlgorithm, TestFixtures.GetHeader(headers, "x-acs-signature-method"));
                Assert.Equal(signatureBytes, Convert.FromBase64String(TestFixtures.GetHeader(headers, "authorization").Substring("acs ak:".Length)).Length);
                Assert.Equal("acs ak:" + ExpectedRoaSignature(server.LastRequestHeaders, headers["raw-query"], expectedAlgorithm), TestFixtures.GetHeader(headers, "authorization"));
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
                if (!overrideHeader) Assert.Equal("HMAC-SHA1", TestFixtures.GetHeader(headers, "x-acs-signature-method"));

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

        [Theory]
        [InlineData("RPC")]
        [InlineData("ROA")]
        public void RejectConflictingSignatureMethodHeaders(string style)
        {
            var config = TestFixtures.CreateConfig();
            config.SignatureAlgorithm = "v2";
            config.Endpoint = "127.0.0.1";
            var request = TestFixtures.CreateOpenApiRequest();
            request.Headers["x-acs-signature-method"] = "HMAC-SHA1";
            request.Headers["X-Acs-Signature-Method"] = "HMAC-SHA256";
            var parameters = new Params { Action = "TestAPI", Version = "2022-06-01", Protocol = "HTTP", Pathname = "/", Method = "GET", AuthType = "AK", Style = style, ReqBodyType = "json", BodyType = "json" };
            Assert.Throws<ClientException>(() => new TestClient(config).CallApi(parameters, request, TestFixtures.CreateRuntimeOptions()));
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
                Assert.Equal("HMAC-SHA1", TestFixtures.GetHeader(headers, "x-acs-signature-method"));
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
