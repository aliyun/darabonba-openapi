import unittest
from io import BytesIO
from alibabacloud_tea_openapi.models import GlobalParameters, Config, OpenApiRequest, Params
from alibabacloud_credentials.client import Client as CredentialClient
from alibabacloud_credentials import models as credential_models
from Tea.exceptions import RequiredArgumentException


class TestGlobalParameters(unittest.TestCase):

    def test_initialization(self):
        params = GlobalParameters()
        self.assertIsNone(params.headers)
        self.assertIsNone(params.queries)
        headers = {"Content-Type": "application/json"}
        queries = {"test": "value"}
        params = GlobalParameters(headers=headers, queries=queries)
        self.assertEqual(params.headers, headers)
        self.assertEqual(params.queries, queries)

    def test_to_from_map(self):
        headers = {"header_key": "header_value"}
        queries = {"query_key": "query_value"}
        params = GlobalParameters(headers=headers, queries=queries)
        map_data = params.to_map()
        self.assertIn('headers', map_data)
        self.assertIn('queries', map_data)
        self.assertEqual(map_data['headers'], headers)
        self.assertEqual(map_data['queries'], queries)
        from_params = GlobalParameters()
        from_params.from_map(map_data)
        self.assertEqual(from_params.headers, headers)
        self.assertEqual(from_params.queries, queries)

    def test_validate(self):
        params = GlobalParameters()
        params.validate()


class TestConfig(unittest.TestCase):

    def setUp(self):
        self.global_params = GlobalParameters()
        self.credential_client = CredentialClient(credential_models.Config(
            bearer_token='token',
            type='bearer'
        ))

    def test_initialization(self):
        config = Config(
            access_key_id='ak',
            access_key_secret='secret',
            security_token='token',
            bearer_token='token',
            type='sts',
            endpoint='config.endpoint',
            endpoint_type='config.endpointType',
            network='config.network',
            suffix='config.suffix',
            protocol='config.protocol',
            method='config.method',
            region_id='config.regionId',
            user_agent='config.userAgent',
            read_timeout=3000,
            connect_timeout=3000,
            http_proxy='config.httpProxy',
            https_proxy='config.httpsProxy',
            no_proxy='config.noProxy',
            socks_5proxy='config.socks5Proxy',
            socks_5net_work='config.socks5NetWork',
            max_idle_conns=128,
            signature_version='config.signatureVersion',
            signature_algorithm='config.signatureAlgorithm',
            global_parameters=self.global_params,
            key='config.key',
            cert='config.cert',
            ca='config.ca',
            credential=self.credential_client,
            disable_http_2=True,
            tls_min_version='config.tlsMinVersion',
            open_platform_endpoint='config.openPlatformEndpoint',
        )
        self.assertEqual(config.access_key_id, 'ak')
        self.assertEqual(config.access_key_secret, 'secret')
        self.assertEqual(config.security_token, 'token')
        self.assertEqual(config.bearer_token, 'token')
        self.assertEqual(config.type, 'sts')
        self.assertEqual(config.endpoint, 'config.endpoint')
        self.assertEqual(config.endpoint_type, 'config.endpointType')
        self.assertEqual(config.network, 'config.network')
        self.assertEqual(config.suffix, 'config.suffix')
        self.assertEqual(config.protocol, 'config.protocol')
        self.assertEqual(config.method, 'config.method')
        self.assertEqual(config.region_id, 'config.regionId')
        self.assertEqual(config.user_agent, 'config.userAgent')
        self.assertEqual(config.read_timeout, 3000)
        self.assertEqual(config.connect_timeout, 3000)
        self.assertEqual(config.http_proxy, 'config.httpProxy')
        self.assertEqual(config.https_proxy, 'config.httpsProxy')
        self.assertEqual(config.no_proxy, 'config.noProxy')
        self.assertEqual(config.socks_5proxy, 'config.socks5Proxy')
        self.assertEqual(config.socks_5net_work, 'config.socks5NetWork')
        self.assertEqual(config.max_idle_conns, 128)
        self.assertEqual(config.signature_version, 'config.signatureVersion')
        self.assertEqual(config.signature_algorithm, 'config.signatureAlgorithm')
        self.assertEqual(config.global_parameters, self.global_params)
        self.assertEqual(config.key, 'config.key')
        self.assertEqual(config.cert, 'config.cert')
        self.assertEqual(config.ca, 'config.ca')
        self.assertEqual(config.credential, self.credential_client)
        self.assertEqual(config.disable_http_2, True)
        self.assertEqual(config.tls_min_version, 'config.tlsMinVersion')
        self.assertEqual(config.open_platform_endpoint, 'config.openPlatformEndpoint')

    def test_to_from_map(self):
        config = Config(
            access_key_id='ak',
            access_key_secret='secret',
            security_token='token',
            bearer_token='token',
            type='sts',
            endpoint='config.endpoint',
            endpoint_type='config.endpointType',
            network='config.network',
            suffix='config.suffix',
            protocol='config.protocol',
            method='config.method',
            region_id='config.regionId',
            user_agent='config.userAgent',
            read_timeout=3000,
            connect_timeout=3000,
            http_proxy='config.httpProxy',
            https_proxy='config.httpsProxy',
            no_proxy='config.noProxy',
            socks_5proxy='config.socks5Proxy',
            socks_5net_work='config.socks5NetWork',
            max_idle_conns=128,
            signature_version='config.signatureVersion',
            signature_algorithm='config.signatureAlgorithm',
            global_parameters=self.global_params,
            key='config.key',
            cert='config.cert',
            ca='config.ca',
            credential=self.credential_client,
            disable_http_2=True,
            tls_min_version='config.tlsMinVersion',
            open_platform_endpoint='config.openPlatformEndpoint',
        )
        map_data = config.to_map()
        self.assertEqual('ak', map_data['accessKeyId'])
        self.assertEqual('secret', map_data['accessKeySecret'])
        self.assertEqual('token', map_data['securityToken'])
        self.assertEqual('token', map_data['bearerToken'])
        self.assertEqual('sts', map_data['type'])
        self.assertEqual('config.endpoint', map_data['endpoint'])
        self.assertEqual('config.endpointType', map_data['endpointType'])
        self.assertEqual('config.network', map_data['network'])
        self.assertEqual('config.suffix', map_data['suffix'])
        self.assertEqual('config.protocol', map_data['protocol'])
        self.assertEqual('config.method', map_data['method'])
        self.assertEqual('config.regionId', map_data['regionId'])
        self.assertEqual('config.userAgent', map_data['userAgent'])
        self.assertEqual(3000, map_data['readTimeout'])
        self.assertEqual(3000, map_data['connectTimeout'])
        self.assertEqual('config.httpProxy', map_data['httpProxy'])
        self.assertEqual('config.httpsProxy', map_data['httpsProxy'])
        self.assertEqual('config.noProxy', map_data['noProxy'])
        self.assertEqual('config.socks5Proxy', map_data['socks5Proxy'])
        self.assertEqual('config.socks5NetWork', map_data['socks5NetWork'])
        self.assertEqual(128, map_data['maxIdleConns'])
        self.assertEqual('config.signatureVersion', map_data['signatureVersion'])
        self.assertEqual('config.signatureAlgorithm', map_data['signatureAlgorithm'])
        self.assertEqual({}, map_data['globalParameters'])
        self.assertEqual('config.key', map_data['key'])
        self.assertEqual('config.cert', map_data['cert'])
        self.assertEqual('config.ca', map_data['ca'])
        self.assertEqual(self.credential_client, map_data['credential'])
        self.assertEqual(True, map_data['disableHttp2'])
        self.assertEqual('config.tlsMinVersion', map_data['tlsMinVersion'])
        self.assertEqual('config.openPlatformEndpoint', map_data['openPlatformEndpoint'])
        config = Config()
        config.from_map(map_data)
        self.assertEqual(config.access_key_id, 'ak')
        self.assertEqual(config.access_key_secret, 'secret')
        self.assertEqual(config.security_token, 'token')
        self.assertEqual(config.bearer_token, 'token')
        self.assertEqual(config.type, 'sts')
        self.assertEqual(config.endpoint, 'config.endpoint')
        self.assertEqual(config.endpoint_type, 'config.endpointType')
        self.assertEqual(config.network, 'config.network')
        self.assertEqual(config.suffix, 'config.suffix')
        self.assertEqual(config.protocol, 'config.protocol')
        self.assertEqual(config.method, 'config.method')
        self.assertEqual(config.region_id, 'config.regionId')
        self.assertEqual(config.user_agent, 'config.userAgent')
        self.assertEqual(config.read_timeout, 3000)
        self.assertEqual(config.connect_timeout, 3000)
        self.assertEqual(config.http_proxy, 'config.httpProxy')
        self.assertEqual(config.https_proxy, 'config.httpsProxy')
        self.assertEqual(config.no_proxy, 'config.noProxy')
        self.assertEqual(config.socks_5proxy, 'config.socks5Proxy')
        self.assertEqual(config.socks_5net_work, 'config.socks5NetWork')
        self.assertEqual(config.max_idle_conns, 128)
        self.assertEqual(config.signature_version, 'config.signatureVersion')
        self.assertEqual(config.signature_algorithm, 'config.signatureAlgorithm')
        self.assertIsNotNone(config.global_parameters)
        self.assertEqual(config.key, 'config.key')
        self.assertEqual(config.cert, 'config.cert')
        self.assertEqual(config.ca, 'config.ca')
        self.assertEqual(config.credential, self.credential_client)
        self.assertEqual(config.disable_http_2, True)
        self.assertEqual(config.tls_min_version, 'config.tlsMinVersion')
        self.assertEqual(config.open_platform_endpoint, 'config.openPlatformEndpoint')

    def test_validate(self):
        config = Config()
        config.validate()
        config = Config(global_parameters=GlobalParameters())
        config.validate()


class TestOpenApiRequest(unittest.TestCase):

    def test_initialization(self):
        f = BytesIO()
        f.write(bytes('test', 'utf-8'))
        f.seek(0)
        request = OpenApiRequest(
            headers={"Content-Type": "text/plain"},
            query={"test": "query"},
            body='test',
            stream=f,
            host_map={"test": "test"},
            endpoint_override='http://localhost:8080/api'
        )
        self.assertEqual({"Content-Type": "text/plain"}, request.headers)
        self.assertEqual({"test": "query"}, request.query)
        self.assertEqual('test', request.body)
        self.assertEqual(b'test', request.stream.read())
        self.assertEqual({"test": "test"}, request.host_map)
        self.assertEqual('http://localhost:8080/api', request.endpoint_override)

    def test_to_from_map(self):
        f = BytesIO()
        f.write(bytes('test', 'utf-8'))
        f.seek(0)
        request = OpenApiRequest(
            headers={"Content-Type": "text/plain"},
            query={"test": "query"},
            body='test',
            stream=f,
            host_map={"test": "test"},
            endpoint_override='http://localhost:8080/api'
        )
        map_data = request.to_map()
        self.assertEqual('query', map_data['query']['test'])
        self.assertEqual('text/plain', map_data['headers']['Content-Type'])
        self.assertEqual('test', map_data['body'])
        self.assertEqual(b'test', map_data['stream'].read())
        self.assertEqual({"test": "test"}, map_data['hostMap'])
        self.assertEqual('http://localhost:8080/api', map_data['endpointOverride'])
        request = OpenApiRequest()
        request.from_map(map_data)
        self.assertEqual({"Content-Type": "text/plain"}, request.headers)
        self.assertEqual({"test": "query"}, request.query)
        self.assertEqual('test', request.body)
        self.assertEqual({"test": "test"}, request.host_map)
        self.assertEqual('http://localhost:8080/api', request.endpoint_override)

    def test_validate(self):
        request = OpenApiRequest()
        request.validate()


class TestParams(unittest.TestCase):

    def test_initialization_and_validation(self):
        params = Params()
        with self.assertRaises(RequiredArgumentException):
            params.validate()

        params = Params(
            action="action",
            version="version",
            protocol="protocol",
            pathname="pathname",
            method="method",
            auth_type="auth_type",
            body_type="body_type",
            req_body_type="req_body_type",
            style='style'
        )
        params.validate()
        self.assertEqual("action", params.action)
        self.assertEqual("version", params.version)
        self.assertEqual("protocol", params.protocol)
        self.assertEqual("pathname", params.pathname)
        self.assertEqual("method", params.method)
        self.assertEqual("auth_type", params.auth_type)
        self.assertEqual("body_type", params.body_type)
        self.assertEqual("req_body_type", params.req_body_type)
        self.assertEqual("style", params.style)

    def test_to_from_map(self):
        params = Params(
            action="action",
            version="version",
            protocol="protocol",
            pathname="pathname",
            method="method",
            auth_type="auth_type",
            body_type="body_type",
            req_body_type="req_body_type",
            style='style'
        )

        map_data = params.to_map()
        self.assertEqual('action', map_data['action'])
        self.assertEqual('version', map_data['version'])
        self.assertEqual('protocol', map_data['protocol'])
        self.assertEqual('pathname', map_data['pathname'])
        self.assertEqual('method', map_data['method'])
        self.assertEqual('auth_type', map_data['authType'])
        self.assertEqual('body_type', map_data['bodyType'])
        self.assertEqual('req_body_type', map_data['reqBodyType'])
        self.assertEqual('style', map_data['style'])

        params = Params()
        params.from_map(map_data)
        self.assertEqual("action", params.action)
        self.assertEqual("version", params.version)
        self.assertEqual("protocol", params.protocol)
        self.assertEqual("pathname", params.pathname)
        self.assertEqual("method", params.method)
        self.assertEqual("auth_type", params.auth_type)
        self.assertEqual("body_type", params.body_type)
        self.assertEqual("req_body_type", params.req_body_type)
        self.assertEqual("style", params.style)
