using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AlibabaCloud.OpenApiClient;
using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.TeaUtil.Models;
using Xunit;

public class SignatureV2Tests
{
    private static readonly InvalidOperationException SignatureFailure = new InvalidOperationException("signature failure");

    public static string FailSignature()
    {
        throw SignatureFailure;
    }

    [Fact]
    public void UtilFailureIsPropagated()
    {
        var invoke = typeof(Client).GetMethod("InvokeSignatureV2", BindingFlags.NonPublic | BindingFlags.Static);
        var failure = typeof(SignatureV2Tests).GetMethod("FailSignature", BindingFlags.Public | BindingFlags.Static);
        Assert.NotNull(invoke);
        var error = Assert.Throws<TargetInvocationException>(() => invoke.Invoke(null, new object[] { failure, new object[0] }));
        Assert.Same(SignatureFailure, error.InnerException);
    }

    [Fact]
    public void LegacyRpcSha1MatchesExistingUtilForSpecialAndEmptyValues()
    {
        var parameters = new Dictionary<string, string> {
            { "中文 +/%~", "空 格+/%*~雪" }, { "empty", "" }, { "null", null }, { "Action", "Test" }
        };
        string expected = AlibabaCloud.OpenApiUtil.Client.GetRPCSignature(parameters, "POST", "secret");
        var method = typeof(Client).GetMethod("SignRpcV2", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);
        string actual = (string)method.Invoke(null, new object[] { parameters, "POST", "secret", "HMAC-SHA1" });
        Assert.Equal(expected, actual);
    }

    private static string Sign(string text, string key, string method)
    {
        using (HMAC mac = method == "HMAC-SHA1" ? (HMAC)new HMACSHA1(Encoding.UTF8.GetBytes(key)) : new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            return Convert.ToBase64String(mac.ComputeHash(Encoding.UTF8.GetBytes(text)));
    }

    private static Dictionary<string, string> Query(string value)
    {
        var result = new Dictionary<string, string>();
        foreach (string part in value.TrimStart('?').Split('&'))
        {
            if (part.Length == 0) continue;
            string[] pair = part.Split(new[] { '=' }, 2);
            result[WebUtility.UrlDecode(pair[0])] = pair.Length == 1 ? "" : WebUtility.UrlDecode(pair[1]);
        }
        return result;
    }

    [Theory]
    [InlineData("RPC", null, null, "HMAC-SHA256")]
    [InlineData("RPC", "HMAC-SHA1", null, "HMAC-SHA1")]
    [InlineData("RPC", null, "HMAC-SHA1", "HMAC-SHA1")]
    [InlineData("RPC", "HMAC-SHA256", "HMAC-SHA1", "HMAC-SHA256")]
    [InlineData("RPC", null, "unsupported", "HMAC-SHA256")]
    [InlineData("RPC", null, "", "HMAC-SHA256")]
    [InlineData("ROA", null, null, "HMAC-SHA256")]
    [InlineData("ROA", "HMAC-SHA1", null, "HMAC-SHA1")]
    [InlineData("RPCAsync", null, null, "HMAC-SHA256")]
    [InlineData("RPCAsync", "HMAC-SHA1", null, "HMAC-SHA1")]
    [InlineData("RPCAsync", null, "HMAC-SHA1", "HMAC-SHA1")]
    [InlineData("RPCAsync", "HMAC-SHA256", "HMAC-SHA1", "HMAC-SHA256")]
    [InlineData("ROAAsync", null, null, "HMAC-SHA256")]
    [InlineData("ROAAsync", "HMAC-SHA1", null, "HMAC-SHA1")]
    [InlineData("ROAForm", null, null, "HMAC-SHA256")]
    [InlineData("ROAForm", "HMAC-SHA1", null, "HMAC-SHA1")]
    [InlineData("ROAFormAsync", null, null, "HMAC-SHA256")]
    [InlineData("ROAFormAsync", "HMAC-SHA1", null, "HMAC-SHA1")]
    public void ActualRequestsUseSelectedSignature(string style, string overrideHeader, string queryAlgorithm, string expected)
    {
        var socket = new TcpListener(IPAddress.Loopback, 0);
        socket.Start();
        int port = ((IPEndPoint)socket.LocalEndpoint).Port;
        socket.Stop();
        using (var listener = new HttpListener())
        {
            listener.Prefixes.Add("http://127.0.0.1:" + port + "/");
            listener.Start();
            var received = Task.Run(() => {
                var context = listener.GetContext();
                var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (string key in context.Request.Headers.AllKeys) headers[key] = context.Request.Headers[key];
                var query = context.Request.Url.Query;
                byte[] response = Encoding.UTF8.GetBytes("{}");
                context.Response.ContentType = "application/json";
                context.Response.OutputStream.Write(response, 0, response.Length);
                context.Response.Close();
                return Tuple.Create(query, headers);
            });
            var config = new Config { Endpoint = "127.0.0.1:" + port, Protocol = "HTTP", AccessKeyId = "ak", AccessKeySecret = "secret" };
            var request = new OpenApiRequest { Headers = new Dictionary<string, string>(), Query = new Dictionary<string, string>() };
            request.Query["中文 +/%~"] = "空 格+/%*~雪";
            request.Query["Empty"] = "";
            if (queryAlgorithm != null) request.Query["SignatureMethod"] = queryAlgorithm;
            if (overrideHeader != null)
            {
                config.GlobalParameters = new GlobalParameters { Headers = new Dictionary<string, string> { { "x-acs-signature-method", overrideHeader == "HMAC-SHA1" ? "HMAC-SHA256" : "HMAC-SHA1" } } };
                request.Headers["X-Acs-Signature-Method"] = overrideHeader;
            }
            var runtime = new RuntimeOptions { Autoretry = false, MaxAttempts = 1 };
            var client = new Client(config);
            if (style == "RPC") client.DoRPCRequest("Test", "2022-06-01", "HTTP", "GET", "AK", "json", request, runtime);
            else if (style == "RPCAsync") client.DoRPCRequestAsync("Test", "2022-06-01", "HTTP", "GET", "AK", "json", request, runtime).GetAwaiter().GetResult();
            else if (style == "ROA") client.DoROARequest("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", request, runtime);
            else if (style == "ROAAsync") client.DoROARequestAsync("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", request, runtime).GetAwaiter().GetResult();
            else if (style == "ROAForm") client.DoROARequestWithForm("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", request, runtime);
            else client.DoROARequestWithFormAsync("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", request, runtime).GetAwaiter().GetResult();
            Assert.True(received.Wait(TimeSpan.FromSeconds(5)));
            var parameters = Query(received.Result.Item1);
            var headers = received.Result.Item2;
            if (style.StartsWith("RPC", StringComparison.Ordinal))
            {
                Assert.Equal(expected, parameters["SignatureMethod"]);
                string signature = parameters["Signature"];
                parameters.Remove("Signature");
                string canonical = string.Join("&", parameters.OrderBy(pair => pair.Key, StringComparer.Ordinal).Select(pair => Uri.EscapeDataString(pair.Key) + "=" + Uri.EscapeDataString(pair.Value)));
                Assert.Equal(Sign("GET&%2F&" + Uri.EscapeDataString(canonical), "secret&", expected), signature);
            }
            else
            {
                Assert.Equal(expected, headers["x-acs-signature-method"]);
                string canonicalHeaders = string.Concat(headers.Where(pair => pair.Key.StartsWith("x-acs-", StringComparison.OrdinalIgnoreCase)).OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase).Select(pair => pair.Key.ToLowerInvariant() + ":" + pair.Value.Trim() + "\n"));
                string canonicalQuery = string.Join("&", parameters.OrderBy(pair => pair.Key, StringComparer.Ordinal).Select(pair => pair.Value.Length == 0 ? pair.Key : pair.Key + "=" + pair.Value));
                string resource = "/test" + (canonicalQuery.Length == 0 ? "" : "?" + canonicalQuery);
                string text = "GET\napplication/json\n\n\n" + headers["date"] + "\n" + canonicalHeaders + resource;
                Assert.Equal("acs ak:" + Sign(text, "secret", expected), headers["authorization"]);
            }
        }
    }

    [Theory]
    [InlineData("RPC")]
    [InlineData("ROA")]
    public void ConflictingHeadersAreRejected(string style)
    {
        var config = new Config { Endpoint = "127.0.0.1", Protocol = "HTTP", AccessKeyId = "ak", AccessKeySecret = "secret" };
        var request = new OpenApiRequest { Headers = new Dictionary<string, string> { { "x-acs-signature-method", "HMAC-SHA256" }, { "X-Acs-Signature-Method", "HMAC-SHA1" } } };
        var runtime = new RuntimeOptions { Autoretry = false, MaxAttempts = 1 };
        var client = new Client(config);
        var error = Assert.Throws<Tea.TeaException>(() => {
            if (style == "RPC") client.DoRPCRequest("Test", "2022-06-01", "HTTP", "GET", "AK", "json", request, runtime);
            else client.DoROARequest("Test", "2022-06-01", "HTTP", "GET", "AK", "/test", "json", request, runtime);
        });
        Assert.Contains("Conflicting signature V2 algorithms", error.Message);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NonHmacRoaKeepsOriginalDefault(bool bearer)
    {
        var socket = new TcpListener(IPAddress.Loopback, 0);
        socket.Start();
        int port = ((IPEndPoint)socket.LocalEndpoint).Port;
        socket.Stop();
        using (var listener = new HttpListener())
        {
            listener.Prefixes.Add("http://127.0.0.1:" + port + "/");
            listener.Start();
            var received = Task.Run(() => {
                var context = listener.GetContext();
                string method = context.Request.Headers["x-acs-signature-method"];
                byte[] response = Encoding.UTF8.GetBytes("{}");
                context.Response.ContentType = "application/json";
                context.Response.OutputStream.Write(response, 0, response.Length);
                context.Response.Close();
                return method;
            });
            var config = new Config { Endpoint = "127.0.0.1:" + port, Protocol = "HTTP" };
            if (bearer) config.BearerToken = "token";
            var runtime = new RuntimeOptions { Autoretry = false, MaxAttempts = 1 };
            new Client(config).DoROARequest("Test", "2022-06-01", "HTTP", "GET", bearer ? "AK" : "Anonymous", "/test", "json", new OpenApiRequest(), runtime);
            Assert.True(received.Wait(TimeSpan.FromSeconds(5)));
            Assert.Equal("HMAC-SHA1", received.Result);
        }
    }
}
