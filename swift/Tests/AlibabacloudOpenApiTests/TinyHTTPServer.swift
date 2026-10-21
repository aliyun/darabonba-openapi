import Foundation

final class TinyHTTPServer {
    private(set) var port: UInt16 = 0
    private var process: Process?
    private var scriptURL: URL?

    init() {
        let script = """
        from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer

        class Handler(BaseHTTPRequestHandler):
            protocol_version = "HTTP/1.0"

            def log_message(self, format, *args):
                return

            def do_GET(self):
                self.handle_any()

            def do_POST(self):
                self.handle_any()

            def do_PUT(self):
                self.handle_any()

            def do_DELETE(self):
                self.handle_any()

            def handle_any(self):
                te = (self.headers.get("Transfer-Encoding") or "").lower()
                if "chunked" in te:
                    while True:
                        line = self.rfile.readline()
                        if not line:
                            break
                        size = int(line.strip() or b"0", 16)
                        if size == 0:
                            self.rfile.readline()
                            break
                        self.rfile.read(size)
                        self.rfile.readline()
                else:
                    n = int(self.headers.get("Content-Length") or 0)
                    if n:
                        self.rfile.read(n)
                path = (self.path or "/").split("?", 1)[0]
                status = 200
                content_type = "application/json"
                body = b'{"AppId":"test","ClassId":"test","UserId":123}'
                if (self.headers.get("x-force-error") or "") == "1":
                    status = 500
                    body = b'{"Code":"InternalError","Message":"boom","RequestId":"rid"}'
                elif path.startswith("/empty"):
                    status = 204
                    body = b""
                elif path.startswith("/xml"):
                    status = 400
                    content_type = "text/xml;charset=utf-8"
                    body = b'<?xml version="1.0"?><Error><Code>Invalid</Code><Message>xml error</Message><RequestId>xml-id</RequestId></Error>'
                elif path.startswith("/server"):
                    status = 500
                    body = b'{"Code":"InternalError","Message":"boom","RequestId":"rid"}'
                elif path.startswith("/array"):
                    body = b'["AppId","ClassId","UserId"]'
                elif path.startswith("/error"):
                    status = 400
                    body = b'{"Code":"error code","Message":"error message","RequestId":"xml-id","Description":"desc","AccessDeniedDetail":{}}'
                elif path.startswith("/plain"):
                    content_type = "text/plain"
                    body = b"not-json"
                self.send_response(status)
                self.send_header("Content-Type", content_type)
                self.send_header("Content-Length", str(len(body)))
                self.send_header("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")
                self.send_header("Connection", "close")
                self.end_headers()
                if body:
                    self.wfile.write(body)

        httpd = ThreadingHTTPServer(("127.0.0.1", 0), Handler)
        print(httpd.server_address[1], flush=True)
        httpd.serve_forever()
        """
        let url = FileManager.default.temporaryDirectory.appendingPathComponent("openapi_tiny_http.py")
        try! script.write(to: url, atomically: true, encoding: .utf8)
        scriptURL = url

        let python = ["/usr/bin/python3", "/opt/homebrew/bin/python3", "/usr/local/bin/python3"]
            .first { FileManager.default.isExecutableFile(atPath: $0) } ?? "/usr/bin/python3"
        let proc = Process()
        proc.executableURL = URL(fileURLWithPath: python)
        proc.arguments = [url.path]
        let stdout = Pipe()
        proc.standardOutput = stdout
        proc.standardError = Pipe()
        try! proc.run()
        process = proc

        var data = Data()
        let handle = stdout.fileHandleForReading
        let deadline = Date().addingTimeInterval(5)
        while Date() < deadline && port == 0 {
            let chunk = handle.availableData
            if !chunk.isEmpty {
                data.append(chunk)
                if let text = String(data: data, encoding: .utf8),
                   let line = text.split(separator: "\n").first,
                   let parsed = UInt16(line) {
                    port = parsed
                    break
                }
            }
            Thread.sleep(forTimeInterval: 0.02)
        }
        precondition(port > 0, "tiny http server failed to publish a port")
        let readyDeadline = Date().addingTimeInterval(3)
        while Date() < readyDeadline {
            var input: InputStream?
            var output: OutputStream?
            Stream.getStreamsToHost(withName: "127.0.0.1", port: Int(port), inputStream: &input, outputStream: &output)
            output?.open()
            if output?.streamStatus == .open || output?.streamStatus == .writing {
                output?.close()
                input?.close()
                break
            }
            output?.close()
            input?.close()
            Thread.sleep(forTimeInterval: 0.05)
        }
    }

    deinit {
        stop()
    }

    func stop() {
        process?.terminate()
        process = nil
        if let scriptURL {
            try? FileManager.default.removeItem(at: scriptURL)
        }
        scriptURL = nil
    }
}
