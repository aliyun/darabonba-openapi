using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AlibabaCloud.OpenApiClient.Exceptions;
using AlibabaCloud.OpenApiClient.Models;
using Darabonba.Models;
using Xunit;

namespace OpenApiClientUnitTests.PrivateLink
{
    public class OSSPrivateUrlTest
    {
        private static readonly byte[] Binary = { 0, 1, 255, 10 };

        // 经跨命名空间子类调用真实 protected 上传方法。
        private sealed class ProductClient : AlibabaCloud.OpenApiClient.Client
        {
            public ProductClient(Config config) : base(config) { }
            public string Resolve(RuntimeOptions runtime) { return GetOSSPrivateUrl(runtime); }
            public Dictionary<string, object> Upload(string url)
            {
                return PostOSSObjectWithURL(url, "test-bucket", Form(), new RuntimeOptions { ReadTimeout = 2000 });
            }
            public Task<Dictionary<string, object>> UploadAsync(string url)
            {
                return PostOSSObjectWithURLAsync(url, "test-bucket", Form(), new RuntimeOptions { ReadTimeout = 2000 });
            }
        }

        private static Dictionary<string, object> Form()
        {
            return new Dictionary<string, object>
            {
                { "host", "oss-cn-hangzhou.aliyuncs.com" }, { "OSSAccessKeyId", "test-id" },
                { "policy", "test-policy" }, { "Signature", "test-signature" },
                { "key", "dir/file.bin" }, { "success_action_status", "201" },
                { "file", new FileField { Filename = "dir/file.bin", ContentType = "application/octet-stream", Content = new MemoryStream(Binary) } },
            };
        }

        private sealed class Record
        {
            public string Method, Host, Path, Body;
        }

        // 仅监听回环地址，记录实际 multipart 请求；不使用真实凭据。
        private sealed class Gateway : IDisposable
        {
            private readonly HttpListener listener = new HttpListener();
            private readonly Task worker;
            public readonly List<Record> Records = new List<Record>();
            public int Status = 201;
            public readonly string Host;
            public string Url { get { return "http://" + Host; } }

            public Gateway()
            {
                var reservation = new TcpListener(IPAddress.Loopback, 0);
                reservation.Start();
                Host = "127.0.0.1:" + ((IPEndPoint)reservation.LocalEndpoint).Port;
                reservation.Stop();
                listener.Prefixes.Add(Url + "/");
                listener.Start();
                worker = Task.Run((Action)Serve);
            }

            private void Serve()
            {
                while (listener.IsListening)
                {
                    HttpListenerContext context;
                    try { context = listener.GetContext(); }
                    catch (HttpListenerException) { return; }
                    catch (ObjectDisposedException) { return; }
                    using (var body = new MemoryStream())
                    {
                        context.Request.InputStream.CopyTo(body);
                        Records.Add(new Record
                        {
                            Method = context.Request.HttpMethod, Host = context.Request.Headers["Host"],
                            Path = context.Request.Url.AbsolutePath,
                            Body = Encoding.GetEncoding(28591).GetString(body.ToArray()),
                        });
                    }
                    var xml = Status == 201 ? "<PostResponse><Key>dir/file.bin</Key></PostResponse>" :
                        "<Error><Code>AccessDenied</Code><Message>denied</Message><RequestId>req-id</RequestId><HostId>host-id</HostId></Error>";
                    var bytes = Encoding.UTF8.GetBytes(xml);
                    context.Response.StatusCode = Status;
                    context.Response.ContentType = "application/xml";
                    context.Response.ContentLength64 = bytes.Length;
                    context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                    context.Response.Close();
                }
            }

            public void Dispose()
            {
                listener.Close();
                Assert.True(worker.Wait(TimeSpan.FromSeconds(5)), "本地上传网关未结束");
            }
        }

        [Fact]
        public void RuntimePriorityAndMapping()
        {
            var client = new ProductClient(new Config { OssPrivateUrl = "http://config.example" });
            Assert.Equal("http://config.example", client.Resolve(new RuntimeOptions()));
            Assert.Equal("http://config.example", client.Resolve(new RuntimeOptions { OssPrivateUrl = "" }));
            Assert.Equal("https://runtime.example", client.Resolve(new RuntimeOptions { OssPrivateUrl = "https://runtime.example" }));
            Assert.Null(new ProductClient(new Config()).Resolve(new RuntimeOptions()));
            var map = new Dictionary<string, object> { { "ossPrivateUrl", "https://pvl.example/proxy" } };
            Assert.Equal(map["ossPrivateUrl"], Darabonba.Model.ToObject<Config>(map).OssPrivateUrl);
            Assert.Equal(map["ossPrivateUrl"], Darabonba.Model.ToObject<RuntimeOptions>(map).ToMap()["ossPrivateUrl"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task PathStyleMultipartAndXmlErrors(bool asyncMode)
        {
            using (var gateway = new Gateway())
            {
                var client = new ProductClient(new Config());
                foreach (var prefix in new[] { "", "/", "/proxy", "/proxy/" })
                {
                    var result = asyncMode ? await client.UploadAsync(gateway.Url + prefix) : client.Upload(gateway.Url + prefix);
                    Assert.Equal("dir/file.bin", ((Dictionary<string, object>)result["PostResponse"])["Key"]);
                    var record = gateway.Records.Last();
                    Assert.Equal("POST", record.Method);
                    Assert.Equal(gateway.Host, record.Host);
                    Assert.Equal(prefix.TrimEnd('/') + "/test-bucket/", record.Path);
                    foreach (var name in new[] { "OSSAccessKeyId", "policy", "Signature", "key", "success_action_status", "file" })
                        Assert.Contains("name=\"" + name + "\"", record.Body);
                    Assert.Contains("test-policy", record.Body);
                    Assert.Contains(Encoding.GetEncoding(28591).GetString(Binary), record.Body);
                }
                gateway.Status = 403;
                var error = asyncMode ? await Assert.ThrowsAsync<ClientException>(() => client.UploadAsync(gateway.Url)) :
                    Assert.Throws<ClientException>(() => client.Upload(gateway.Url));
                Assert.Equal("AccessDenied", error.Code);
                Assert.Equal("denied", error.Message);
                Assert.Equal(403, error.StatusCode);
                Assert.Equal(403, error.Data["httpCode"]);
                Assert.Equal("req-id", error.Data["requestId"]);
                Assert.Equal("host-id", error.Data["hostId"]);
                foreach (var url in new[] { null, "", "example.com", "//example.com", "ftp://example.com", "https:///prefix", "http://",
                    "http://example.com:99999", "http://user@example.com", "http://user:password@example.com", "http://@example.com",
                    "http://example.com/?a=b", "http://example.com?", "http://example.com/#fragment", "http://example.com#" })
                {
                    var invalid = asyncMode ? await Assert.ThrowsAsync<ClientException>(() => client.UploadAsync(url)) :
                        Assert.Throws<ClientException>(() => client.Upload(url));
                    Assert.Equal("InvalidOSSPrivateUrl", invalid.Code);
                }
                Assert.Equal(5, gateway.Records.Count);
            }
        }
    }
}
