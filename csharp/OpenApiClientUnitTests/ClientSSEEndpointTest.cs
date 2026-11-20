using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AlibabaCloud.OpenApiClient.Models;
using Darabonba.Models;
using Xunit;

namespace OpenApiClientUnitTests
{
    public class ClientSSEEndpointTest
    {
        [Theory]
        [InlineData(null, "aliyuncs.com", "ecs.cn-hangzhou.aliyuncs.com")]
        [InlineData("", "aliyuncs.alibaba", "ecs.cn-hangzhou.aliyuncs.alibaba")]
        [InlineData("oss-cn-hangzhou.aliyuncs.com", "aliyuncs.com", "oss-cn-hangzhou.aliyuncs.com")]
        [InlineData("oss-cn-hangzhou.aliyuncs.com", "aliyuncs.alibaba", "oss-cn-hangzhou.aliyuncs.alibaba")]
        [InlineData("custom.example.test", "aliyuncs.alibaba", "custom.example.test")]
        [InlineData("ecs.cn-hangzhou.aliyuncs.alibaba", "aliyuncs.alibaba", "ecs.cn-hangzhou.aliyuncs.alibaba")]
        public void CallSSEApiPreservesEndpointOverrideAndSuffix(string endpointOverride, string suffix, string expected)
        {
            // 经仅监听回环地址的代理捕获真实 SSE 请求，不解析或访问目标域名。
            using (var proxy = new RecordingProxy())
            {
                const string endpoint = "ecs.cn-hangzhou.aliyuncs.com";
                var client = new TestClient(new Config
                {
                    Endpoint = endpoint,
                    EndpointSuffix = suffix,
                    Protocol = "HTTP"
                });
                var parameters = new Params
                {
                    Action = "SSETestAPI", Version = "2022-06-01", Protocol = "HTTP",
                    Pathname = "/sse", Method = "GET", AuthType = "Anonymous",
                    Style = "ROA", ReqBodyType = "json", BodyType = "json"
                };
                var runtime = new RuntimeOptions
                {
                    HttpProxy = proxy.Url, ConnectTimeout = 2000, ReadTimeout = 2000
                };
                var responses = client.CallSSEApi(parameters,
                    new OpenApiRequest { EndpointOverride = endpointOverride }, runtime).ToList();
                proxy.WaitForRequest();
                Assert.Single(responses);
                Assert.Equal(200, responses[0].StatusCode);
                Assert.Equal("{\"ok\":true}", responses[0].Event.Data);
                Assert.Equal(expected, proxy.Host);
                Assert.Equal("GET http://" + expected + "/sse HTTP/1.1", proxy.RequestLine);
                Assert.Equal(endpoint, client.Endpoint);
            }
        }

        private sealed class RecordingProxy : IDisposable
        {
            private readonly TcpListener listener;
            private readonly Task worker;
            public string Host { get; private set; }
            public string RequestLine { get; private set; }
            public string Url { get; private set; }

            public RecordingProxy()
            {
                listener = new TcpListener(IPAddress.Loopback, 0);
                listener.Start();
                Url = "http://127.0.0.1:" + ((IPEndPoint)listener.LocalEndpoint).Port;
                worker = Task.Run((Action)Serve);
            }

            private void Serve()
            {
                using (var connection = listener.AcceptTcpClient())
                {
                    connection.ReceiveTimeout = 5000;
                    connection.SendTimeout = 5000;
                    using (var stream = connection.GetStream())
                    using (var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, true))
                    {
                        RequestLine = reader.ReadLine();
                        string line;
                        while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                        {
                            if (line.StartsWith("Host:", StringComparison.OrdinalIgnoreCase))
                                Host = line.Substring(5).Trim();
                        }
                        const string body = "data: {\"ok\":true}\n\n";
                        byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n" +
                            "Content-Type: text/event-stream\r\nConnection: close\r\n" +
                            "Content-Length: " + Encoding.UTF8.GetByteCount(body) + "\r\n\r\n" + body);
                        stream.Write(response, 0, response.Length);
                        stream.Flush();
                    }
                }
            }

            public void WaitForRequest()
            {
                Assert.True(worker.Wait(TimeSpan.FromSeconds(5)), "本地 SSE 代理未完成响应");
            }

            public void Dispose()
            {
                listener.Stop();
            }
        }
    }
}
