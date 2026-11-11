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
    [InlineData("RPC", "HMAC-SHA1", null, "HMAC-SHA256")]
    [InlineData("RPC", null, "HMAC-SHA1", "HMAC-SHA256")]
    [InlineData("RPC", "HMAC-SHA256", "HMAC-SHA1", "HMAC-SHA256")]
    [InlineData("ROA", null, null, "HMAC-SHA256")]
    [InlineData("ROA", "HMAC-SHA1", null, "HMAC-SHA256")]
    [InlineData("RPCBody", null, null, "HMAC-SHA256")]
    [InlineData("RPCBodyAsync", null, null, "HMAC-SHA256")]
    [InlineData("RPCAsync", null, null, "HMAC-SHA256")]
    [InlineData("RPCAsync", "HMAC-SHA1", null, "HMAC-SHA256")]
    [InlineData("RPCAsync", null, "HMAC-SHA1", "HMAC-SHA256")]
    [InlineData("RPCAsync", "HMAC-SHA256", "HMAC-SHA1", "HMAC-SHA256")]
    [InlineData("ROAAsync", null, null, "HMAC-SHA256")]
    [InlineData("ROAAsync", "HMAC-SHA1", null, "HMAC-SHA256")]
    [InlineData("ROAForm", null, null, "HMAC-SHA256")]
    [InlineData("ROAForm", "HMAC-SHA1", null, "HMAC-SHA256")]
    [InlineData("ROAFormAsync", null, null, "HMAC-SHA256")]
    [InlineData("ROAFormAsync", "HMAC-SHA1", null, "HMAC-SHA256")]
    public void ActualRequestsUseSha256AndPreserveUserFields(string style, string overrideHeader, string queryAlgorithm, string expected)
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
                var body = new System.IO.StreamReader(context.Request.InputStream).ReadToEnd();
                byte[] response = Encoding.UTF8.GetBytes("{}");
                context.Response.ContentType = "application/json";
                context.Response.OutputStream.Write(response, 0, response.Length);
                context.Response.Close();
                return Tuple.Create(query, headers, body);
            });
            var config = new Config { Endpoint = "127.0.0.1:" + port, Protocol = "HTTP", AccessKeyId = "ak", AccessKeySecret = "secret" };
            var request = new OpenApiRequest { Headers = new Dictionary<string, string>(), Query = new Dictionary<string, string>() };
            request.Query["中文 +/%~"] = "空 格+/%*~雪";
            request.Query["Empty"] = "";
            if (queryAlgorithm != null) request.Query["SignatureMethod"] = queryAlgorithm;
            if (style.StartsWith("RPCBody", StringComparison.Ordinal)) request.Body = new Dictionary<string, object> { { "SignatureMethod", "HMAC-SHA1" } };
            if (overrideHeader != null)
            {
                config.GlobalParameters = new GlobalParameters { Headers = new Dictionary<string, string> { { "x-acs-signature-method", overrideHeader == "HMAC-SHA1" ? "HMAC-SHA256" : "HMAC-SHA1" } } };
                request.Headers["x-acs-signature-method"] = overrideHeader;
            }
            var runtime = new RuntimeOptions { Autoretry = false, MaxAttempts = 1 };
            var client = new Client(config);
            if (style == "RPCBody") client.DoRPCRequest("Test", "2022-06-01", "HTTP", "POST", "AK", "json", request, runtime);
            else if (style == "RPCBodyAsync") client.DoRPCRequestAsync("Test", "2022-06-01", "HTTP", "POST", "AK", "json", request, runtime).GetAwaiter().GetResult();
            else if (style == "RPC") client.DoRPCRequest("Test", "2022-06-01", "HTTP", "GET", "AK", "json", request, runtime);
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
                foreach (var pair in Query(received.Result.Item3)) parameters[pair.Key] = pair.Value;
                string canonical = string.Join("&", parameters.OrderBy(pair => pair.Key, StringComparer.Ordinal).Select(pair => Uri.EscapeDataString(pair.Key) + "=" + Uri.EscapeDataString(pair.Value)));
                string method = style.StartsWith("RPCBody", StringComparison.Ordinal) ? "POST" : "GET";
                Assert.Equal(Sign(method + "&%2F&" + Uri.EscapeDataString(canonical), "secret&", expected), signature);
            }
            else
            {
                Assert.Equal(overrideHeader ?? "HMAC-SHA256", headers["x-acs-signature-method"]);
                string canonicalHeaders = string.Concat(headers.Where(pair => pair.Key.StartsWith("x-acs-", StringComparison.OrdinalIgnoreCase)).OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase).Select(pair => pair.Key.ToLowerInvariant() + ":" + pair.Value.Trim() + "\n"));
                string canonicalQuery = string.Join("&", parameters.OrderBy(pair => pair.Key, StringComparer.Ordinal).Select(pair => pair.Value.Length == 0 ? pair.Key : pair.Key + "=" + pair.Value));
                string resource = "/test" + (canonicalQuery.Length == 0 ? "" : "?" + canonicalQuery);
                string text = "GET\napplication/json\n\n\n" + headers["date"] + "\n" + canonicalHeaders + resource;
                Assert.Equal("acs ak:" + Sign(text, "secret", expected), headers["authorization"]);
            }
        }
    }

    [Theory]
    [InlineData(false, false, false, null)]
    [InlineData(false, false, true, null)]
    [InlineData(false, true, false, null)]
    [InlineData(false, true, true, null)]
    [InlineData(true, false, false, null)]
    [InlineData(true, false, true, null)]
    [InlineData(true, true, false, null)]
    [InlineData(true, true, true, null)]
    [InlineData(false, false, false, "custom")]
    [InlineData(true, true, true, "custom")]
    public void NonHmacRoaPreservesOriginalDefault(bool bearer, bool form, bool async, string overrideHeader)
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
            var request = new OpenApiRequest();
            if (overrideHeader != null) request.Headers = new Dictionary<string, string> { { "x-acs-signature-method", overrideHeader } };
            var client = new Client(config);
            string auth = bearer ? "AK" : "Anonymous";
            if (form && async) client.DoROARequestWithFormAsync("Test", "2022-06-01", "HTTP", "GET", auth, "/test", "json", request, runtime).GetAwaiter().GetResult();
            else if (form) client.DoROARequestWithForm("Test", "2022-06-01", "HTTP", "GET", auth, "/test", "json", request, runtime);
            else if (async) client.DoROARequestAsync("Test", "2022-06-01", "HTTP", "GET", auth, "/test", "json", request, runtime).GetAwaiter().GetResult();
            else client.DoROARequest("Test", "2022-06-01", "HTTP", "GET", auth, "/test", "json", request, runtime);
            Assert.True(received.Wait(TimeSpan.FromSeconds(5)));
            Assert.Equal(overrideHeader ?? "HMAC-SHA1", received.Result);
        }
    }
}
