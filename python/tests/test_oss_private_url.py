import asyncio
import threading
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from io import BytesIO
from urllib.parse import urlsplit

import pytest
from alibabacloud_tea_openapi.client import Client
from alibabacloud_tea_openapi.exceptions import ClientException
from alibabacloud_tea_openapi.utils_models import Config
from darabonba.runtime import RuntimeOptions
from darabonba.utils.form import FileField


BINARY = bytes([0, 1, 255, 10])
INVALID_URLS = [
    None, '', 'example.com', '//example.com', 'ftp://example.com',
    'https:///prefix', 'http://', 'http://example.com:99999',
    'http://user@example.com', 'http://user:password@example.com',
    'http://@example.com', 'http://example.com/?a=b', 'http://example.com?',
    'http://example.com/#fragment', 'http://example.com#',
]


class Gateway:
    """仅监听回环地址；HTTPS 代理只记录 CONNECT，不连接目标。"""

    def __init__(self):
        self.records = []
        self.status = 201
        gateway = self

        class Handler(BaseHTTPRequestHandler):
            def log_message(self, *args):
                pass

            def do_CONNECT(self):
                gateway.records.append({'method': 'CONNECT', 'target': self.path})
                self.respond(502, b'')

            def do_POST(self):
                if self.headers.get('Transfer-Encoding') == 'chunked':
                    chunks = []
                    while True:
                        size = int(self.rfile.readline().split(b';')[0], 16)
                        if not size:
                            self.rfile.readline()
                            break
                        chunks.append(self.rfile.read(size))
                        self.rfile.readline()
                    body = b''.join(chunks)
                else:
                    body = self.rfile.read(int(self.headers.get('Content-Length', 0)))
                gateway.records.append({
                    'method': self.command, 'target': self.path,
                    'path': urlsplit(self.path).path,
                    'host': self.headers.get('Host'), 'body': body,
                })
                xml = (b'<PostResponse><Key>dir/file.bin</Key></PostResponse>'
                       if gateway.status == 201 else
                       b'<Error><Code>AccessDenied</Code><Message>denied</Message>'
                       b'<RequestId>req-id</RequestId><HostId>host-id</HostId></Error>')
                self.respond(gateway.status, xml)

            def respond(self, status, body):
                self.send_response(status)
                self.send_header('Content-Type', 'application/xml')
                self.send_header('Content-Length', str(len(body)))
                self.end_headers()
                self.wfile.write(body)

        self.server = ThreadingHTTPServer(('127.0.0.1', 0), Handler)
        self.thread = threading.Thread(target=self.server.serve_forever, daemon=True)

    @property
    def host(self):
        return '127.0.0.1:%d' % self.server.server_port

    @property
    def url(self):
        return 'http://' + self.host

    def __enter__(self):
        self.thread.start()
        return self

    def __exit__(self, *args):
        self.server.shutdown()
        self.server.server_close()
        self.thread.join(timeout=5)


def form():
    return {
        'host': 'oss-cn-hangzhou.aliyuncs.com', 'OSSAccessKeyId': 'test-id',
        'policy': 'test-policy', 'Signature': 'test-signature',
        'key': 'dir/file.bin', 'success_action_status': '201',
        'file': FileField(filename='dir/file.bin', content=BytesIO(BINARY),
                          content_type='application/octet-stream'),
    }


def upload(client, url, async_mode=False, runtime=None):
    runtime = runtime or RuntimeOptions(read_timeout=2000, connect_timeout=2000)
    if async_mode:
        return asyncio.run(client.post_ossobject_with_url_async(url, 'test-bucket', form(), runtime))
    return client.post_ossobject_with_url(url, 'test-bucket', form(), runtime)


def assert_multipart(record):
    assert record['method'] == 'POST'
    for name in ['OSSAccessKeyId', 'policy', 'Signature', 'key', 'success_action_status', 'file']:
        assert ('name="%s"' % name).encode() in record['body']
    assert b'test-policy' in record['body']
    assert BINARY in record['body']


def test_runtime_priority_and_mapping():
    client = Client(Config(oss_private_url='http://config.example'))
    assert client.get_ossprivate_url(RuntimeOptions()) == 'http://config.example'
    assert client.get_ossprivate_url(RuntimeOptions(oss_private_url='')) == 'http://config.example'
    assert client.get_ossprivate_url(RuntimeOptions(oss_private_url='https://runtime.example')) == 'https://runtime.example'
    assert Client(Config()).get_ossprivate_url(RuntimeOptions()) is None
    values = {'ossPrivateUrl': 'https://pvl.example/proxy'}
    assert Config().from_map(values).to_map()['ossPrivateUrl'] == values['ossPrivateUrl']
    assert RuntimeOptions().from_map(values).to_map()['ossPrivateUrl'] == values['ossPrivateUrl']


@pytest.mark.parametrize('async_mode', [False, True])
def test_path_style_and_binary_multipart(async_mode):
    with Gateway() as gateway:
        client = Client(Config())
        for prefix in ['', '/', '/proxy', '/proxy/']:
            result = upload(client, gateway.url + prefix, async_mode)
            assert result['PostResponse']['Key'] == 'dir/file.bin'
            record = gateway.records[-1]
            assert record['host'] == gateway.host
            assert record['path'] == prefix.rstrip('/') + '/test-bucket/'
            assert_multipart(record)


@pytest.mark.parametrize('async_mode', [False, True])
def test_xml_error_and_invalid_urls(async_mode):
    with Gateway() as gateway:
        gateway.status = 403
        client = Client(Config())
        with pytest.raises(ClientException) as error:
            upload(client, gateway.url, async_mode)
        assert error.value.code == 'AccessDenied'
        assert error.value.message == 'denied'
        assert error.value.status_code == 403
        assert error.value.data['httpCode'] == 403
        assert error.value.data['requestId'] == 'req-id'
        assert error.value.data['hostId'] == 'host-id'
        for url in INVALID_URLS:
            with pytest.raises(ClientException) as invalid:
                upload(client, url, async_mode)
            assert invalid.value.code == 'InvalidOSSPrivateUrl', repr(url)
        assert len(gateway.records) == 1


def test_default_port_and_https_proxy_targets():
    with Gateway() as proxy:
        client = Client(Config())
        upload(client, 'http://pvl.example', runtime=RuntimeOptions(http_proxy=proxy.url, read_timeout=2000))
        assert proxy.records[-1]['target'] == 'http://pvl.example/test-bucket/'
        assert proxy.records[-1]['host'] == 'pvl.example'
        assert_multipart(proxy.records[-1])
        for url, target in [('https://pvl.example/', 'pvl.example:443'),
                            ('https://pvl.example:8443/proxy', 'pvl.example:8443')]:
            with pytest.raises(Exception):
                upload(client, url, runtime=RuntimeOptions(https_proxy=proxy.url, read_timeout=2000))
            assert proxy.records[-1] == {'method': 'CONNECT', 'target': target}
