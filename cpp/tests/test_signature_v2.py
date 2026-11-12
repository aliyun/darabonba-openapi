import base64
import hashlib
import hmac
import queue
import subprocess
import sys
import threading
import urllib.parse
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer


class Handler(BaseHTTPRequestHandler):
    requests = queue.Queue()

    def do_GET(self):
        self.requests.put(("GET", self.path, {key.lower(): value for key, value in self.headers.items()}, ""))
        self.send_response(200)
        self.send_header("Content-Type", "application/json")
        self.end_headers()
        self.wfile.write(b"{}")

    def do_POST(self):
        if self.headers.get("Transfer-Encoding", "").lower() == "chunked":
            chunks = []
            while True:
                size = int(self.rfile.readline().strip().split(b";", 1)[0], 16)
                if not size:
                    self.rfile.readline()
                    break
                chunks.append(self.rfile.read(size))
                self.rfile.read(2)
            body = b"".join(chunks).decode()
        else:
            body = self.rfile.read(int(self.headers.get("Content-Length", "0"))).decode()
        self.requests.put(("POST", self.path, {key.lower(): value for key, value in self.headers.items()}, body))
        self.send_response(200)
        self.send_header("Content-Type", "application/json")
        self.end_headers()
        self.wfile.write(b"{}")

    def log_message(self, *args):
        pass


def sign(message, secret, algorithm):
    digest = hashlib.sha1 if algorithm == "HMAC-SHA1" else hashlib.sha256
    return base64.b64encode(hmac.new(secret.encode(), message.encode(), digest).digest()).decode()


def check_request(style, algorithm, method, path, headers, body, expected_header=None):
    parsed = urllib.parse.urlsplit(path)
    query = dict(urllib.parse.parse_qsl(parsed.query, keep_blank_values=True))
    if style == "RPC":
        assert query["SignatureMethod"] == algorithm
        signature = query.pop("Signature")
        query.update(urllib.parse.parse_qsl(body, keep_blank_values=True))
        canonical = "&".join(urllib.parse.quote(k, safe="-_.~") + "=" + urllib.parse.quote(v, safe="-_.~") for k, v in sorted(query.items()))
        expected = sign(method + "&%2F&" + urllib.parse.quote(canonical, safe="-_.~"), "secret&", algorithm)
        assert signature == expected
    else:
        assert headers["x-acs-signature-method"] == (expected_header or algorithm)
        canonical_headers = "".join(k + ":" + v.strip() + "\n" for k, v in sorted(headers.items()) if k.startswith("x-acs-"))
        resource = parsed.path + ("?" + "&".join(k + ("=" + v if v else "") for k, v in sorted(query.items())) if query else "")
        text = "GET\n" + headers.get("accept", "") + "\n" + headers.get("content-md5", "") + "\n" + headers.get("content-type", "") + "\n" + headers.get("date", "") + "\n" + canonical_headers + resource
        assert headers["authorization"] == "acs ak:" + sign(text, "secret", algorithm)


def main():
    executable = sys.argv[1]
    server = ThreadingHTTPServer(("127.0.0.1", 0), Handler)
    thread = threading.Thread(target=server.serve_forever, daemon=True)
    thread.start()
    endpoint = "127.0.0.1:" + str(server.server_port)
    try:
        for style in ("RPC", "ROA"):
            for scenario in ("default", "override"):
                subprocess.run([executable, endpoint, style, scenario], check=True, timeout=10)
                method, path, headers, body = Handler.requests.get(timeout=5)
                check_request(style, "HMAC-SHA256", method, path, headers, body, "HMAC-SHA1" if style == "ROA" and scenario == "override" else None)
        for scenario in ("querysha1", "headerwins", "invalidquery"):
            subprocess.run([executable, endpoint, "RPC", scenario], check=True, timeout=10)
            method, path, headers, body = Handler.requests.get(timeout=5)
            check_request("RPC", "HMAC-SHA256", method, path, headers, body)
        for scenario in ("bodysha1", "bodyconflict"):
            subprocess.run([executable, endpoint, "RPC", scenario], check=True, timeout=10)
            method, path, headers, body = Handler.requests.get(timeout=5)
            assert dict(urllib.parse.parse_qsl(body))["SignatureMethod"] == "HMAC-SHA1"
            check_request("RPC", "HMAC-SHA256", method, path, headers, body)
        for scenario in ("anonymous", "bearer"):
            subprocess.run([executable, endpoint, "ROA", scenario], check=True, timeout=10)
            _, _, headers, _ = Handler.requests.get(timeout=5)
            assert headers["x-acs-signature-method"] == "HMAC-SHA256"
    finally:
        server.shutdown()
        thread.join()
        server.server_close()


if __name__ == "__main__":
    main()
