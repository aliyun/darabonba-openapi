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
        self.requests.put((self.path, {key.lower(): value for key, value in self.headers.items()}))
        self.send_response(200)
        self.send_header("Content-Type", "application/json")
        self.end_headers()
        self.wfile.write(b"{}")

    def log_message(self, *args):
        pass


def sign(message, secret, algorithm):
    digest = hashlib.sha1 if algorithm == "HMAC-SHA1" else hashlib.sha256
    return base64.b64encode(hmac.new(secret.encode(), message.encode(), digest).digest()).decode()


def check_request(style, algorithm, path, headers):
    parsed = urllib.parse.urlsplit(path)
    query = dict(urllib.parse.parse_qsl(parsed.query, keep_blank_values=True))
    if style == "RPC":
        assert query["SignatureMethod"] == algorithm
        signature = query.pop("Signature")
        canonical = "&".join(urllib.parse.quote(k, safe="-_.~") + "=" + urllib.parse.quote(v, safe="-_.~") for k, v in sorted(query.items()))
        expected = sign("GET&%2F&" + urllib.parse.quote(canonical, safe="-_.~"), "secret&", algorithm)
        assert signature == expected
    else:
        assert headers["x-acs-signature-method"] == algorithm
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
            for scenario, algorithm in (("default", "HMAC-SHA256"), ("override", "HMAC-SHA1")):
                subprocess.run([executable, endpoint, style, scenario], check=True, timeout=10)
                path, headers = Handler.requests.get(timeout=5)
                check_request(style, algorithm, path, headers)
            for scenario in ("conflict", "invalid"):
                result = subprocess.run([executable, endpoint, style, scenario], timeout=10, capture_output=True)
                assert result.returncode != 0, (style, scenario)
                assert b"InvalidSignatureAlgorithm" in result.stderr, (style, scenario, result.stderr)
        for scenario, algorithm in (("querysha1", "HMAC-SHA1"), ("headerwins", "HMAC-SHA256")):
            subprocess.run([executable, endpoint, "RPC", scenario], check=True, timeout=10)
            path, headers = Handler.requests.get(timeout=5)
            check_request("RPC", algorithm, path, headers)
        for scenario in ("anonymous", "bearer"):
            subprocess.run([executable, endpoint, "ROA", scenario], check=True, timeout=10)
            _, headers = Handler.requests.get(timeout=5)
            assert headers["x-acs-signature-method"] == "HMAC-SHA1"
    finally:
        server.shutdown()
        thread.join()
        server.server_close()


if __name__ == "__main__":
    main()
