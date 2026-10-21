# -*- coding: utf-8 -*-
import asyncio
import os
import re
import unittest

import httpretty
from aioresponses import aioresponses
from darabonba.exceptions import TeaException
from darabonba.runtime import RuntimeOptions
from httpretty.core import HTTPrettyRequest

from alibabacloud_credentials import models as credential_models
from alibabacloud_credentials.client import Client as CredentialClient
from alibabacloud_gateway_pop.client import Client as PopClient
from alibabacloud_tea_openapi.client import Client as OpenApiClient
from alibabacloud_tea_openapi.exceptions import (
    AlibabaCloudException,
    ClientException,
    ServerException,
    ThrottlingException,
)
from alibabacloud_tea_openapi.models import Config, OpenApiRequest, Params, SSEResponse
from alibabacloud_tea_openapi.sm3 import Sm3, hash_sm3
from alibabacloud_tea_openapi.utils import Utils
from darabonba.event import Event as SSEEvent


class TestExtraCoverage(unittest.TestCase):
    def _client(self, **kwargs):
        config = Config(
            access_key_id='ak',
            access_key_secret='secret',
            endpoint=kwargs.get('endpoint', 'test.aliyuncs.com'),
            protocol=kwargs.get('protocol', 'HTTP'),
            signature_algorithm=kwargs.get('signature_algorithm', 'ACS3-HMAC-SHA256'),
            signature_version=kwargs.get('signature_version', 'config.signatureVersion'),
        )
        return OpenApiClient(config)

    def test_constructors_helpers_and_exceptions(self):
        with self.assertRaises(ClientException):
            OpenApiClient(None)

        os.environ['ALIBABA_CLOUD_USER_AGENT'] = 'from-env'
        try:
            client = OpenApiClient(Config(access_key_id='ak', access_key_secret='sk', endpoint='example.com'))
            self.assertEqual('from-env', client._user_agent)
        finally:
            os.environ.pop('ALIBABA_CLOUD_USER_AGENT', None)

        client = OpenApiClient(Config(endpoint='example.com'))
        self.assertEqual('', client.get_access_key_id())
        self.assertEqual('', client.get_access_key_secret())
        self.assertEqual('', client.get_security_token())
        self.assertEqual('', client.get_bearer_token())
        self.assertEqual('', client.get_type())
        loop = asyncio.get_event_loop()
        self.assertEqual('', loop.run_until_complete(client.get_access_key_id_async()))
        self.assertEqual('', loop.run_until_complete(client.get_access_key_secret_async()))
        self.assertEqual('', loop.run_until_complete(client.get_security_token_async()))
        self.assertEqual('', loop.run_until_complete(client.get_bearer_token_async()))
        self.assertEqual('', loop.run_until_complete(client.get_type_async()))

        with self.assertRaises(ClientException):
            client.check_config(Config())
        client.check_config(Config(endpoint='example.com'))

        self.assertEqual('fallback', OpenApiClient.default_any(None, 'fallback'))
        self.assertEqual('keep', OpenApiClient.default_any('keep', 'fallback'))

        self.assertEqual({'k': 'v'}, client.get_access_denied_detail({'AccessDeniedDetail': {'k': 'v'}}))
        self.assertEqual({'k': 'v2'}, client.get_access_denied_detail({'accessDeniedDetail': {'k': 'v2'}}))
        self.assertIsNone(client.get_access_denied_detail({}))

        client.set_rpc_headers({'debug': '1'})
        self.assertEqual({'debug': '1'}, client.get_rpc_headers())
        self.assertIsNone(client.get_rpc_headers())

        with self.assertRaises(ClientException) as ctx:
            client.call_api(None, OpenApiRequest(), RuntimeOptions())
        self.assertEqual('ParameterMissing', ctx.exception.code)

        cred = CredentialClient(credential_models.Config(
            access_key_id='ak', access_key_secret='sk', type='access_key'
        ))
        client = OpenApiClient(Config(credential=cred, endpoint='example.com'))
        loop = asyncio.get_event_loop()
        self.assertEqual('ak', loop.run_until_complete(client.get_access_key_id_async()))
        self.assertEqual('sk', loop.run_until_complete(client.get_access_key_secret_async()))
        self.assertEqual('access_key', loop.run_until_complete(client.get_type_async()))

        client_exc = ClientException(status_code=400, code='C', message='m', request_id='r')
        self.assertEqual('ClientException', client_exc.name)
        server_exc = ServerException(status_code=500, code='S', message='m')
        self.assertEqual('ServerException', server_exc.name)
        throttling = ThrottlingException(status_code=429, code='Throttling', retry_after=1000)
        self.assertEqual('ThrottlingException', throttling.name)
        self.assertEqual(1000, throttling.retry_after)
        cloud = AlibabaCloudException(code='X', message='y', request_id='rid')
        self.assertEqual('AlibabaCloudException', cloud.name)
        self.assertEqual('rid', cloud.request_id)

        sse = SSEResponse(headers={'k': 'v'}, status_code=200, event=SSEEvent(data='d', id='i', event='e'))
        sse.validate()
        mapped = sse.to_map()
        self.assertEqual(200, mapped['statusCode'])
        restored = SSEResponse().from_map(mapped)
        self.assertEqual(200, restored.status_code)

    def test_sm3_and_utils(self):
        digest = hash_sm3(b'test')
        self.assertEqual(
            '55e12e91650d2fec56ec74e1d3e4ddbfce2ef3a65890c2a19ecf88a307e76a23',
            digest.hex(),
        )
        long_msg = b'a' * 80
        self.assertEqual(32, len(hash_sm3(long_msg)))
        sm3 = Sm3(b'test')
        self.assertEqual(digest, sm3.digest())
        sm3 = Sm3(b'test')
        self.assertEqual(digest.hex(), sm3.hexdigest())
        sm3.update(b'')
        copied = sm3.copy()
        self.assertEqual(sm3.hexdigest(), copied.hexdigest())
        self.assertTrue(sm3.digest_size >= 4)

        headers = {}
        Utils.apply_retry_headers(headers, 0, 10)
        self.assertNotIn('x-acs-retry-attempts', headers)
        Utils.apply_retry_headers(headers, 2, 15)
        self.assertEqual('2', headers['x-acs-retry-attempts'])
        self.assertEqual('15', headers['x-acs-retry-delay'])

    @httpretty.activate
    def test_call_api_v4_byte_none_xml_and_204(self):
        def json_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, '{"AppId":"test","ClassId":"test","UserId":123}']

        def xml_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            headers['content-type'] = 'text/xml;charset=utf-8'
            body = '<?xml version="1.0"?><Error><Code>Invalid</Code><Message>xml error</Message><RequestId>xml-id</RequestId></Error>'
            return [400, headers, body]

        def empty_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            return [204, headers, '']

        def server_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            return [500, headers, '{"Code":"InternalError","Message":"boom","RequestId":"rid"}']

        httpretty.register_uri(httpretty.POST, 'http://test.aliyuncs.com/json', body=json_callback)
        httpretty.register_uri(httpretty.POST, 'http://test.aliyuncs.com/xml', body=xml_callback)
        httpretty.register_uri(httpretty.POST, 'http://test.aliyuncs.com/empty', body=empty_callback)
        httpretty.register_uri(httpretty.POST, 'http://test.aliyuncs.com/server', body=server_callback)

        request = OpenApiRequest(body={'key1': 'value'})
        runtime = RuntimeOptions(ignore_ssl=True)

        acs = self._client()
        json_params = Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/json', method='POST', auth_type='Anonymous',
            style='RPC', req_body_type='json', body_type='byte',
        )
        result = acs.call_api(json_params, request, runtime)
        self.assertIsNotNone(result['body'])

        json_params.body_type = 'binary'
        result = acs.call_api(json_params, request, runtime)
        self.assertIsNotNone(result['body'])

        json_params.body_type = 'none'
        result = acs.call_api(json_params, request, runtime)
        self.assertEqual(200, result['statusCode'])

        json_params.body_type = 'json'
        json_params.pathname = '/xml'
        with self.assertRaises(TeaException):
            acs.call_api(json_params, request, runtime)

        v2 = self._client(signature_algorithm='v2')
        roa = Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/empty', method='POST', auth_type='Anonymous',
            style='ROA', req_body_type='json', body_type='json',
        )
        result = v2.call_api(roa, request, runtime)
        self.assertIsNotNone(result.get('headers'))
        roa.req_body_type = 'formData'
        result = v2.call_api(roa, request, runtime)
        self.assertIsNotNone(result.get('headers'))

        roa.pathname = '/server'
        with self.assertRaises(TeaException):
            v2.call_api(roa, request, runtime)
        roa.req_body_type = 'json'
        with self.assertRaises(TeaException):
            v2.call_api(roa, request, runtime)
        rpc = Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/server', method='POST', auth_type='Anonymous',
            style='RPC', req_body_type='formData', body_type='json',
        )
        with self.assertRaises(TeaException):
            v2.call_api(rpc, request, runtime)

    @aioresponses()
    def test_async_v4_and_helpers(self, m):
        m.post(
            'http://test.aliyuncs.com/',
            status=200,
            payload={'AppId': 'test', 'ClassId': 'test', 'UserId': 123},
            headers={'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'},
        )
        client = self._client(signature_version='v4')
        client._product_id = 'test'
        client.set_gateway_client(PopClient())
        params = Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/', method='POST', auth_type='Anonymous',
            style='RPC', req_body_type='json', body_type='json',
        )
        loop = asyncio.get_event_loop()
        result = loop.run_until_complete(client.call_api_async(params, OpenApiRequest(body={'k': 'v'}), RuntimeOptions(ignore_ssl=True)))
        self.assertEqual(200, result['statusCode'])

        with self.assertRaises(ClientException):
            loop.run_until_complete(client.call_api_async(None, OpenApiRequest(), RuntimeOptions()))

    def _json_ok(self, request: HTTPrettyRequest, uri: str, headers: dict):
        headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
        return [200, headers, '{"AppId":"test","ClassId":"test","UserId":123}']

    @httpretty.activate
    def test_ak_v2_v3_execute_sse_and_body_types(self):
        httpretty.register_uri(httpretty.POST, 'http://test.aliyuncs.com/', body=self._json_ok)
        httpretty.register_uri(httpretty.POST, 'http://test.aliyuncs.com/test', body=self._json_ok)
        httpretty.register_uri(httpretty.GET, 'http://test.aliyuncs.com/sse', body=lambda req, uri, headers: [
            200, headers, 'id: sse-test\nevent: flow\ndata: {"count": 0}\n\n'
        ])
        httpretty.register_uri(httpretty.POST, 'http://test.aliyuncs.com/xml', body=lambda req, uri, headers: [
            400, {**headers, 'content-type': 'text/xml;charset=utf-8'},
            '<?xml version="1.0"?><Error><Code>Invalid</Code><Message>xml error</Message><RequestId>xml-id</RequestId></Error>'
        ])

        runtime = RuntimeOptions(ignore_ssl=True, autoretry=True, max_attempts=1)
        request = OpenApiRequest(query={'k': 'v'}, body={'key1': 'value'}, headers={'for-test': 'sdk'})
        ak = OpenApiClient(Config(
            access_key_id='ak', access_key_secret='secret', security_token='token', type='sts',
            endpoint='test.aliyuncs.com', protocol='HTTP',
        ))
        v2 = OpenApiClient(Config(
            access_key_id='ak', access_key_secret='secret', endpoint='test.aliyuncs.com',
            protocol='HTTP', signature_algorithm='v2',
        ))
        params = Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/', method='POST', auth_type='AK',
            style='RPC', req_body_type='formData', body_type='json',
        )
        result = v2.call_api(params, request, runtime)
        self.assertEqual(200, result['statusCode'])
        params.body_type = 'string'
        self.assertEqual(200, v2.call_api(params, request, runtime)['statusCode'])
        params.body_type = 'byte'
        self.assertIsNotNone(v2.call_api(params, request, runtime).get('body'))
        params.body_type = 'binary'
        self.assertIsNotNone(v2.call_api(params, request, runtime).get('body'))
        params.body_type = 'none'
        self.assertEqual(200, v2.call_api(params, request, runtime)['statusCode'])

        roa = Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/test', method='POST', auth_type='AK',
            style='ROA', req_body_type='json', body_type='json',
        )
        self.assertEqual(200, v2.call_api(roa, request, runtime)['statusCode'])
        roa.req_body_type = 'formData'
        self.assertEqual(200, v2.call_api(roa, request, runtime)['statusCode'])

        acs = Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/test', method='POST', auth_type='AK',
            style='RPC', req_body_type='json', body_type='json',
        )
        self.assertEqual(200, ak.call_api(acs, request, runtime)['statusCode'])
        acs.req_body_type = 'formData'
        self.assertEqual(200, ak.call_api(acs, request, runtime)['statusCode'])
        acs.req_body_type = 'byte'
        self.assertEqual(200, ak.call_api(acs, OpenApiRequest(body=b'test byte'), runtime)['statusCode'])
        acs.req_body_type = 'binary'
        self.assertEqual(200, ak.call_api(acs, OpenApiRequest(stream=b'stream-body'), runtime)['statusCode'])

        bearer = OpenApiClient(Config(
            credential=CredentialClient(credential_models.Config(bearer_token='token', type='bearer')),
            endpoint='test.aliyuncs.com', protocol='HTTP',
        ))
        self.assertEqual(200, bearer.call_api(Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/test', method='POST', auth_type='AK',
            style='ROA', req_body_type='json', body_type='json',
        ), request, runtime)['statusCode'])
        v2_bearer = OpenApiClient(Config(
            credential=CredentialClient(credential_models.Config(bearer_token='token', type='bearer')),
            endpoint='test.aliyuncs.com', protocol='HTTP', signature_algorithm='v2',
        ))
        self.assertEqual(200, v2_bearer.call_api(params, request, runtime)['statusCode'])

        exec_client = OpenApiClient(Config(
            access_key_id='ak', access_key_secret='secret', endpoint='test.aliyuncs.com',
            protocol='HTTP', signature_version='v4',
        ))
        exec_client._product_id = 'test'
        exec_client.set_gateway_client(PopClient())
        exec_client.set_rpc_headers({'rpc-exec': '1'})
        self.assertEqual(200, exec_client.execute(Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/', method='POST', auth_type='AK',
            style='RPC', req_body_type='json', body_type='json',
        ), request, runtime)['statusCode'])

        sse_params = Params(
            action='SSETestAPI', version='2022-06-01', protocol='HTTP',
            pathname='/sse', method='GET', auth_type='AK',
            style='ROA', req_body_type='formData', body_type='json',
        )
        events = list(ak.call_sseapi(sse_params, request, runtime))
        self.assertGreaterEqual(len(events), 1)

        xml_params = Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/xml', method='POST', auth_type='Anonymous',
            style='RPC', req_body_type='json', body_type='json',
        )
        with self.assertRaises(Exception):
            ak.call_api(xml_params, request, runtime)

        missing = OpenApiClient(Config(endpoint='test.aliyuncs.com', protocol='HTTP'))
        with self.assertRaises(Exception):
            missing.call_api(Params(
                action='TestAPI', version='2022-06-01', protocol='HTTPS',
                pathname='/test', method='POST', auth_type='AK',
                style='RPC', req_body_type='json', body_type='json',
            ), request, runtime)

    @aioresponses()
    def test_async_ak_and_sse(self, m):
        response_headers = {'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'}
        m.post(
            re.compile(r'http://test\.aliyuncs\.com'),
            status=200,
            body='{"AppId":"test","ClassId":"test","UserId":123}',
            headers=response_headers,
            repeat=True,
        )
        m.get(
            re.compile(r'http://test\.aliyuncs\.com/sse'),
            status=200,
            body='id: sse-test\nevent: flow\ndata: {"count": 0}\n\n',
            headers={'Content-Type': 'text/event-stream'},
            repeat=True,
        )
        runtime = RuntimeOptions(ignore_ssl=True)
        request = OpenApiRequest(body={'key1': 'value'})
        loop = asyncio.get_event_loop()
        v2 = OpenApiClient(Config(
            access_key_id='ak', access_key_secret='secret', endpoint='test.aliyuncs.com',
            protocol='HTTP', signature_algorithm='v2',
        ))
        result = loop.run_until_complete(v2.call_api_async(Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/', method='POST', auth_type='AK',
            style='RPC', req_body_type='formData', body_type='json',
        ), request, runtime))
        self.assertEqual(200, result['statusCode'])
        result = loop.run_until_complete(v2.call_api_async(Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/test', method='POST', auth_type='AK',
            style='ROA', req_body_type='json', body_type='string',
        ), request, runtime))
        self.assertEqual(200, result['statusCode'])

        ak = OpenApiClient(Config(
            access_key_id='ak', access_key_secret='secret', endpoint='test.aliyuncs.com', protocol='HTTP',
        ))
        result = loop.run_until_complete(ak.call_api_async(Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/test', method='POST', auth_type='AK',
            style='RPC', req_body_type='json', body_type='json',
        ), request, runtime))
        self.assertEqual(200, result['statusCode'])

        exec_client = OpenApiClient(Config(
            access_key_id='ak', access_key_secret='secret', endpoint='test.aliyuncs.com',
            protocol='HTTP', signature_version='v4',
        ))
        exec_client._product_id = 'test'
        exec_client.set_gateway_client(PopClient())
        result = loop.run_until_complete(exec_client.execute_async(Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/', method='POST', auth_type='Anonymous',
            style='RPC', req_body_type='json', body_type='json',
        ), request, runtime))
        self.assertEqual(200, result['statusCode'])

        async def collect_sse():
            events = []
            async for item in ak.call_sseapi_async(Params(
                action='SSETestAPI', version='2022-06-01', protocol='HTTP',
                pathname='/sse', method='GET', auth_type='AK',
                style='ROA', req_body_type='formData', body_type='json',
            ), request, runtime):
                events.append(item)
            return events

        events = loop.run_until_complete(collect_sse())
        self.assertGreaterEqual(len(events), 0)

        for body_type in ('json', 'string', 'byte', 'none'):
            result = loop.run_until_complete(ak.do_rpcrequest_async(
                'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', body_type, request, runtime,
            ))
            self.assertEqual(200, result['statusCode'])
            result = loop.run_until_complete(ak.do_roarequest_async(
                'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/test', body_type, request, runtime,
            ))
            self.assertEqual(200, result['statusCode'])
            result = loop.run_until_complete(ak.do_roarequest_with_form_async(
                'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/test', body_type, request, runtime,
            ))
            self.assertIsNotNone(result)
        result = loop.run_until_complete(ak.do_request_async(Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/test', method='POST', auth_type='AK',
            style='ROA', req_body_type='formData', body_type='json',
        ), request, runtime))
        self.assertEqual(200, result['statusCode'])

    @httpretty.activate
    def test_direct_do_rpc_roa_and_request(self):
        httpretty.register_uri(httpretty.POST, 'http://test.aliyuncs.com/', body=self._json_ok)
        httpretty.register_uri(httpretty.POST, 'http://test.aliyuncs.com/test', body=self._json_ok)
        runtime = RuntimeOptions(ignore_ssl=True)
        request = OpenApiRequest(body={'key1': 'value'})
        client = self._client()
        self.assertEqual(200, client.do_rpcrequest(
            'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', 'json', request, runtime,
        )['statusCode'])
        self.assertEqual(200, client.do_roarequest(
            'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/test', 'json', request, runtime,
        )['statusCode'])
        self.assertEqual(200, client.do_roarequest_with_form(
            'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/test', 'json', request, runtime,
        )['statusCode'])
        self.assertEqual(200, client.do_request(Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/test', method='POST', auth_type='Anonymous',
            style='RPC', req_body_type='json', body_type='none',
        ), request, runtime)['statusCode'])

        v2 = self._client(signature_algorithm='v2')
        self.assertEqual(200, v2.do_rpcrequest(
            'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', 'json', request, runtime,
        )['statusCode'])
        self.assertEqual(200, v2.do_rpcrequest(
            'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', 'string', request, runtime,
        )['statusCode'])
        self.assertEqual(200, v2.do_roarequest(
            'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', '/test', 'json', request, runtime,
        )['statusCode'])
        self.assertEqual(200, v2.do_roarequest(
            'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', '/test', 'string', request, runtime,
        )['statusCode'])
        self.assertEqual(200, v2.do_roarequest_with_form(
            'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', '/test', 'json', request, runtime,
        )['statusCode'])
        self.assertEqual(200, v2.do_roarequest_with_form(
            'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'AK', '/test', 'byte', request, runtime,
        )['statusCode'])
        self.assertEqual(200, v2.call_api(Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/', method='POST', auth_type='AK',
            style='RPC', req_body_type='formData', body_type='json',
        ), request, runtime)['statusCode'])
        self.assertEqual(200, v2.call_api(Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/test', method='POST', auth_type='AK',
            style='ROA', req_body_type='json', body_type='json',
        ), request, runtime)['statusCode'])
        self.assertEqual(200, v2.call_api(Params(
            action='TestAPI', version='2022-06-01', protocol='HTTPS',
            pathname='/test', method='POST', auth_type='AK',
            style='ROA', req_body_type='formData', body_type='json',
        ), request, runtime)['statusCode'])

        httpretty.register_uri(httpretty.POST, 'http://test.aliyuncs.com/xml', body=lambda req, uri, headers: [
            400, {**headers, 'content-type': 'text/xml;charset=utf-8'},
            '<?xml version="1.0"?><Error><Code>Invalid</Code><Message>xml error</Message><RequestId>xml-id</RequestId></Error>'
        ])
        xml_request = OpenApiRequest(body={'key1': 'value'})
        with self.assertRaises(Exception):
            client.do_roarequest(
                'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/xml', 'json', xml_request, runtime,
            )
        with self.assertRaises(Exception):
            client.do_roarequest_with_form(
                'TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/xml', 'json', xml_request, runtime,
            )
        with self.assertRaises(Exception):
            client.do_request(Params(
                action='TestAPI', version='2022-06-01', protocol='HTTPS',
                pathname='/xml', method='POST', auth_type='Anonymous',
                style='ROA', req_body_type='json', body_type='json',
            ), xml_request, runtime)

    @httpretty.activate
    def test_utils_flat_map_and_rpc_errors(self):
        self.assertEqual(1000, Utils._get_time_left('Other:1,TimeLeft:1000'))
        self.assertEqual(0, Utils._get_time_left(''))
        self.assertEqual({'a.b': '1', 'c.1': '2', 'c.2': '3'}, Utils.flat_map({'a': {'b': 1}, 'c': [2, 3]}))
        self.assertEqual(Utils.anyify_map_value({'k': 'v'})['k'], 'v')
        from alibabacloud_tea_openapi import utils as utils_mod
        self.assertEqual('abc', utils_mod.to_str(b'abc'))

        def server_callback(request, uri, headers):
            return [500, headers, '{"Code":"InternalError","Message":"boom","RequestId":"rid","Description":"d","Detail":"x"}']

        def client_callback(request, uri, headers):
            return [400, headers, '{"Code":"Invalid","Message":"bad","RequestId":"rid","AccessDeniedDetail":{"k":"v"}}']

        def throttle_callback(request, uri, headers):
            headers['x-acs-retry-after'] = '1000'
            return [429, headers, '{"Code":"Throttling","Message":"slow","RequestId":"rid"}']

        httpretty.register_uri(httpretty.POST, 'http://test.aliyuncs.com/', body=server_callback)
        httpretty.register_uri(httpretty.POST, 'http://test.aliyuncs.com/client-err', body=client_callback)
        httpretty.register_uri(httpretty.POST, 'http://test.aliyuncs.com/throttle', body=throttle_callback)
        client = self._client()
        runtime = RuntimeOptions(ignore_ssl=True, autoretry=False, max_attempts=1)
        request = OpenApiRequest(body={'key1': 'value'})
        with self.assertRaises(Exception):
            client.do_rpcrequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', 'json', request, runtime)
        with self.assertRaises(Exception):
            client.do_roarequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/client-err', 'json', request, runtime)
        with self.assertRaises(Exception):
            client.do_roarequest('TestAPI', '2022-06-01', 'HTTPS', 'POST', 'Anonymous', '/throttle', 'json', request, runtime)
