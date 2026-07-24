using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenApiClientUnitTests.Mock
{
    public class MockHttpServer : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly Thread _thread;
        private volatile bool _running;

        public int Port { get; }
        public string Endpoint => "127.0.0.1:" + Port;

        public string DefaultContent { get; set; } = "json";
        public Dictionary<string, string> PathContent { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public bool ThrottlingMode { get; set; }
        public int ThrottleCount { get; set; }
        public int RetryAfterMs { get; set; } = 1000;
        public int RequestCount { get; private set; }
        public List<string> RetryAttempts { get; } = new List<string>();
        public List<string> RetryDelays { get; } = new List<string>();

        public bool SseErrorMode { get; set; }
        public string SseErrorContentType { get; set; } = "application/json";

        public MockHttpServer()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            Port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();

            _listener = new HttpListener();
            _listener.Prefixes.Add("http://127.0.0.1:" + Port + "/");
            _listener.Start();
            _running = true;
            _thread = new Thread(ListenLoop)
            {
                IsBackground = true
            };
            _thread.Start();
        }

        private void ListenLoop()
        {
            while (_running)
            {
                try
                {
                    var context = _listener.GetContext();
                    Task.Run(() => HandleRequest(context));
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            var req = context.Request;
            var res = context.Response;

            if (ThrottlingMode)
            {
                HandleThrottling(req, res);
                return;
            }

            if (req.Url.AbsolutePath.Equals("/sse", StringComparison.OrdinalIgnoreCase))
            {
                if (SseErrorMode)
                {
                    HandleSseError(res);
                    return;
                }
                HandleSse(res);
                return;
            }

            CopyRequestHeaders(req, res);
            res.Headers["raw-query"] = req.Url.Query.TrimStart('?');
            string body = ReadBody(req);
            res.Headers["raw-body"] = body;
            res.Headers["x-acs-request-id"] = "A45EE076-334D-5012-9746-A8F828D20FD4";

            string content = DefaultContent;
            string path = req.Url.AbsolutePath;
            if (PathContent.ContainsKey(path))
            {
                content = PathContent[path];
            }

            string responseBody;
            int statusCode;
            switch (content)
            {
                case "array":
                    responseBody = "[\"AppId\", \"ClassId\", \"UserId\"]";
                    statusCode = 200;
                    break;
                case "error":
                    responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\", \"Description\":\"error description\", \"AccessDeniedDetail\":{}}";
                    statusCode = 400;
                    break;
                case "error1":
                    responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\", \"Description\":\"error description\", \"AccessDeniedDetail\":{}, \"accessDeniedDetail\":{\"test\": 1, \"test1\": \"str\"}}";
                    statusCode = 400;
                    break;
                case "error2":
                    responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\", \"Description\":\"error description\", \"accessDeniedDetail\":{\"test\": 1, \"test1\": \"str\"}}";
                    statusCode = 400;
                    break;
                case "serverError":
                    responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\", \"Description\":\"error description\", \"accessDeniedDetail\":{\"test\": 1, \"test1\": \"str\"}}";
                    statusCode = 500;
                    break;
                default:
                    responseBody = "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}";
                    statusCode = 200;
                    break;
            }

            WriteResponse(res, statusCode, responseBody);
        }

        private void HandleThrottling(HttpListenerRequest req, HttpListenerResponse res)
        {
            RequestCount++;
            RetryAttempts.Add(req.Headers["x-acs-retry-attempts"] ?? "");
            RetryDelays.Add(req.Headers["x-acs-retry-delay"] ?? "");

            CopyRequestHeaders(req, res);
            string body = ReadBody(req);
            res.Headers["raw-body"] = body;
            res.Headers["x-acs-request-id"] = "A45EE076-334D-5012-9746-A8F828D20FD4";

            if (RequestCount <= ThrottleCount)
            {
                res.Headers["x-acs-retry-after"] = RetryAfterMs.ToString();
                string throttleBody = "{\"Code\":\"Throttling\",\"Message\":\"Request was denied due to user flow control.\",\"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"}";
                WriteResponse(res, 400, throttleBody);
                return;
            }

            string successBody = "{\"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\",\"Quotas\":[]}";
            WriteResponse(res, 200, successBody);
        }

        private void HandleSseError(HttpListenerResponse res)
        {
            string body;
            string contentType = SseErrorContentType ?? "application/json";
            if (contentType.IndexOf("xml", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                body = "<Error><Code>SSE.Error</Code><Message>sse failed</Message><RequestId>A45EE076-334D-5012-9746-A8F828D20FD4</RequestId></Error>";
            }
            else
            {
                body = "{\"Code\":\"SSE.Error\", \"Message\":\"sse failed\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"}";
            }
            var bytes = Encoding.UTF8.GetBytes(body);
            res.StatusCode = 400;
            res.ContentType = contentType;
            res.Headers["content-type"] = contentType;
            res.ContentLength64 = bytes.Length;
            res.OutputStream.Write(bytes, 0, bytes.Length);
            res.OutputStream.Close();
        }

        private static void HandleSse(HttpListenerResponse res)
        {
            res.StatusCode = 200;
            res.ContentType = "text/event-stream";
            res.Headers["Cache-Control"] = "no-cache";
            res.Headers["Connection"] = "keep-alive";
            res.SendChunked = true;

            using (var writer = new StreamWriter(res.OutputStream, Encoding.UTF8))
            {
                for (int count = 0; count < 5; count++)
                {
                    writer.Write("data: {\"count\": " + count + "}\n");
                    writer.Write("event: flow\n");
                    writer.Write("id: sse-test\n");
                    writer.Write("retry: 3\n");
                    writer.Write(":heartbeat\n\n");
                    writer.Flush();
                    Thread.Sleep(100);
                }
            }
            res.OutputStream.Close();
        }

        private static void CopyRequestHeaders(HttpListenerRequest req, HttpListenerResponse res)
        {
            foreach (string key in req.Headers.AllKeys)
            {
                if (string.Equals(key, "Content-Length", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                res.Headers[key] = req.Headers[key];
            }
        }

        private static string ReadBody(HttpListenerRequest req)
        {
            if (!req.HasEntityBody)
            {
                return "";
            }
            using (var reader = new StreamReader(req.InputStream, req.ContentEncoding ?? Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private static void WriteResponse(HttpListenerResponse res, int statusCode, string body)
        {
            var bytes = Encoding.UTF8.GetBytes(body);
            res.StatusCode = statusCode;
            res.ContentType = "application/json";
            res.ContentLength64 = bytes.Length;
            res.OutputStream.Write(bytes, 0, bytes.Length);
            res.OutputStream.Close();
        }

        public void Dispose()
        {
            _running = false;
            try
            {
                _listener.Stop();
                _listener.Close();
            }
            catch
            {
                // ignore shutdown errors
            }
        }
    }
}
