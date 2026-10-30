# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
import unittest
import asyncio
import re
import httpretty
from Tea.exceptions import TeaException
from httpretty.core import HTTPrettyRequest
from aioresponses import aioresponses

from alibabacloud_gateway_pop.client import Client as PopClient
from alibabacloud_tea_openapi import models as open_api_models
from alibabacloud_credentials import models as credential_models
from alibabacloud_credentials.client import Client as CredentialClient
from alibabacloud_tea_openapi.client import Client as OpenApiClient
from alibabacloud_tea_util import models as util_models
from alibabacloud_tea_util.client import Client as UtilClient
from alibabacloud_openapi_util.client import Client as OpenApiUtilClient


class TestClient(unittest.TestCase):

    def test_config(self):
        global_parameters = open_api_models.GlobalParameters(
            headers={
                'global-key': 'global-value'
            },
            queries={
                'global-query': 'global-value'
            }
        )
        config = open_api_models.Config(
            endpoint='config.endpoint',
            endpoint_type='regional',
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
            global_parameters=global_parameters,
            key='config.key',
            cert='config.cert',
            ca='config.ca',
            disable_http_2=True,
            tls_min_version='config.tlsMinVersion'
        )
        cre_config = credential_models.Config(
            access_key_id='accessKeyId',
            access_key_secret='accessKeySecret',
            security_token='securityToken',
            type='sts'
        )
        credential = CredentialClient(cre_config)
        config.credential = credential
        client = OpenApiClient(config)
        self.assertEqual('accessKeyId', client.get_access_key_id())
        self.assertEqual('accessKeySecret', client.get_access_key_secret())
        self.assertEqual('securityToken', client.get_security_token())
        self.assertEqual('sts', client.get_type())

        cre_config = credential_models.Config(
            bearer_token='token',
            type='bearer'
        )
        credential = CredentialClient(cre_config)
        config.credential = credential
        client = OpenApiClient(config)
        self.assertIsNone(client.get_access_key_id())
        self.assertIsNone(client.get_access_key_secret())
        self.assertIsNone(client.get_security_token())
        self.assertEqual('token', client.get_bearer_token())
        self.assertEqual('bearer', client.get_type())

        config.access_key_id = 'ak'
        config.access_key_secret = 'secret'
        config.security_token = 'token'
        config.type = 'sts'
        client = OpenApiClient(config)
        self.assertEqual('ak', client.get_access_key_id())
        self.assertEqual('secret', client.get_access_key_secret())
        self.assertEqual('token', client.get_security_token())
        self.assertEqual('sts', client.get_type())

        config.bearer_token = 'token'
        config.type = 'bearer'
        config.access_key_id = ''
        config.access_key_secret = ''
        config.security_token = ''
        client = OpenApiClient(config)
        self.assertIsNone(client.get_access_key_id())
        self.assertIsNone(client.get_access_key_secret())
        self.assertIsNone(client.get_security_token())
        self.assertEqual('token', client.get_bearer_token())
        self.assertEqual('bearer', client.get_type())

        config.bearer_token = ''
        config.type = ''
        config.access_key_id = ''
        config.access_key_secret = ''
        config.security_token = ''
        config.credential = None
        client = OpenApiClient(config)
        self.assertEqual('', client.get_access_key_id())
        self.assertEqual('', client.get_access_key_secret())
        self.assertEqual('', client.get_security_token())
        self.assertEqual('', client.get_bearer_token())
        self.assertEqual('', client.get_type())

        self.assertIsNone(client._spi)
        self.assertIsNone(client._endpoint_rule)
        self.assertIsNone(client._endpoint_map)
        self.assertIsNone(client._product_id)
        self.assertEqual("config.endpoint", client._endpoint)
        self.assertEqual("regional", client._endpoint_type)
        self.assertEqual("config.network", client._network)
        self.assertEqual("config.suffix", client._suffix)
        self.assertEqual("config.protocol", client._protocol)
        self.assertEqual("config.method", client._method)
        self.assertEqual("config.regionId", client._region_id)
        self.assertEqual("config.userAgent", client._user_agent)
        self.assertEqual(3000, client._read_timeout)
        self.assertEqual(3000, client._connect_timeout)
        self.assertEqual("config.httpProxy", client._http_proxy)
        self.assertEqual("config.httpsProxy", client._https_proxy)
        self.assertEqual("config.noProxy", client._no_proxy)
        self.assertEqual("config.socks5Proxy", client._socks_5proxy)
        self.assertEqual("config.socks5NetWork", client._socks_5net_work)
        self.assertEqual(128, client._max_idle_conns)
        self.assertEqual("config.signatureVersion", client._signature_version)
        self.assertEqual("config.signatureAlgorithm", client._signature_algorithm)
        self.assertEqual("global-value", client._global_parameters.headers.get("global-key"))
        self.assertEqual("global-value", client._global_parameters.queries.get("global-query"))
        self.assertEqual("config.key", client._key)
        self.assertEqual("config.cert", client._cert)
        self.assertEqual("config.ca", client._ca)
        self.assertEqual(True, client._disable_http_2)
        self.assertEqual("config.tlsMinVersion", client._tls_min_version)

    def create_config(self) -> open_api_models.Config:
        global_parameters = open_api_models.GlobalParameters(
            headers={
                'global-key': 'global-value'
            },
            queries={
                'global-query': 'global-value'
            }
        )
        config = open_api_models.Config(
            access_key_id='ak',
            access_key_secret='secret',
            security_token='token',
            type='sts',
            user_agent='config.userAgent',
            read_timeout=3000,
            connect_timeout=3000,
            max_idle_conns=128,
            signature_version='config.signatureVersion',
            signature_algorithm='ACS3-HMAC-SHA256',
            global_parameters=global_parameters,
            tls_min_version='TLSv1.2',
        )
        return config

    def create_bearer_token_config(self) -> open_api_models.Config:
        cre_config = credential_models.Config(
            bearer_token='token',
            type='bearer'
        )
        credential = CredentialClient(cre_config)
        config = open_api_models.Config(
            credential=credential
        )
        return config
    
    def create_anonymous_config(self) -> open_api_models.Config:
        return open_api_models.Config()

    def create_runtime_options(self) -> util_models.RuntimeOptions:
        extends_parameters = util_models.ExtendsParameters(
            headers={
                'extends-key': 'extends-value'
            },
            queries={
                'extends-key': 'extends-value'
            }
        )
        runtime = util_models.RuntimeOptions(
            read_timeout=4000,
            connect_timeout=4000,
            max_idle_conns=100,
            autoretry=True,
            max_attempts=1,
            backoff_policy='no',
            backoff_period=1,
            ignore_ssl=True,
            extends_parameters=extends_parameters
        )
        return runtime

    def create_open_api_request(self) -> open_api_models.OpenApiRequest:
        query = {}
        query['key1'] = 'value'
        query['key2'] = 1
        query['key3'] = True
        body = {}
        body['key1'] = 'value'
        body['key2'] = 1
        body['key3'] = True
        headers = {
            'for-test': 'sdk'
        }
        req = open_api_models.OpenApiRequest(
            headers=headers,
            query=OpenApiUtilClient.query(query),
            body=OpenApiUtilClient.parse_to_map(body)
        )
        return req

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_rpcwith_v2sign_ak_form(self):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/',
            method='POST',
            auth_type='AK',
            style='RPC',
            req_body_type='formData',
            body_type='json'
        )

        def request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert 'value' == request.querystring['key1'][0]
            assert '1' == request.querystring['key2'][0]
            assert 'True' == request.querystring['key3'][0]
            assert 'global-value' == request.querystring['global-query'][0]
            assert 'extends-value' == request.querystring['extends-key'][0]
            assert 'TestAPI' == request.querystring['Action'][0]
            assert '2022-06-01' == request.querystring['Version'][0]
            assert 'ak' == request.querystring['AccessKeyId'][0]
            assert 'token' == request.querystring['SecurityToken'][0]
            assert '1.0' == request.querystring['SignatureVersion'][0]
            assert 'HMAC-SHA1' == request.querystring['SignatureMethod'][0]
            assert 'json' == request.querystring['Format'][0]
            assert None is not request.querystring['Timestamp'][0]
            assert None is not request.querystring['SignatureNonce'][0]
            assert None is not request.querystring['Signature'][0]
            assert 'global-value' == request.headers.get('global-key')
            assert 'extends-value' == request.headers.get('extends-key')
            assert 'test.alibabacloud.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent',
                                        request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

        # bearer token
        config = self.create_bearer_token_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test1.alibabacloud.com'
        client = OpenApiClient(config)

        def bearer_request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert 'TestAPI' == request.querystring['Action'][0]
            assert '2022-06-01' == request.querystring['Version'][0]
            assert 'json' == request.querystring['Format'][0]
            assert None is not request.querystring['Timestamp'][0]
            assert None is not request.querystring['SignatureNonce'][0]
            assert 'token' == request.querystring['BearerToken'][0]
            assert 'BEARERTOKEN' == request.querystring['SignatureType'][0]
            # assert 'bearer token' == request.headers.get('authorization')
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test1.alibabacloud.com",
            body=bearer_request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual(200, result.get('statusCode'))

        # anonymous error
        config = self.create_anonymous_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        try:
            client.call_api(params, request, runtime)
        except TeaException as e:
            self.assertEqual('Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.', e.message)
            self.assertEqual('InvalidCredentials', e.code)

    @aioresponses()
    def test_call_api_for_rpcwith_v2sign_ak_form_async(self, m):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/',
            method='POST',
            auth_type='AK',
            style='RPC',
            req_body_type='formData',
            body_type='json'
        )

        def request_callback(url, **request):
            assert None is not re.match(
                '.+AccessKeyId=ak&Action=TestAPI&Format=json&SecurityToken=token&Signature=.+&SignatureMethod=HMAC-SHA1&SignatureNonce=.+&SignatureVersion=1.0&Timestamp=.+&Version=2022-06-01&extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
                str(url))
            assert 'global-value' == request['headers'].get('global-key')
            assert 'extends-value' == request['headers'].get('extends-key')
            assert 'test.alibabacloud.com' == request['headers'].get('host')
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent',
                                        request['headers'].get('user-agent'))
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)

        responseHeaders = {'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'}
        m.post(re.compile(r'http://test\.alibabacloud\.com/.*'),
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

        # bearer token
        config = self.create_bearer_token_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test1.alibabacloud.com'
        client = OpenApiClient(config)

        def bearer_request_callback(url, **request):
            assert None is not re.match(
                '.+Action=TestAPI&BearerToken=token&Format=json&SignatureNonce=.+&SignatureType=BEARERTOKEN&Timestamp=.+&Version=2022-06-01&extends-key=extends-value&key1=value&key2=1&key3=True',
                str(url))
            # assert 'bearer token' == request.headers.get('authorization')
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)

        responseHeaders = {'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'}
        m.post(re.compile(r'http://test1\.alibabacloud\.com/.*'),
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=bearer_request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual(200, result.get('statusCode'))

        # anonymous error
        config = self.create_anonymous_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        try:
            loop.run_until_complete(client.call_api_async(params, request, runtime))
            assert False
        except TeaException as e:
            self.assertEqual('Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.', e.message)
            self.assertEqual('InvalidCredentials', e.code)

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_rpcwith_v2sign_anonymous_json(self):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/',
            method='POST',
            auth_type='Anonymous',
            style='RPC',
            req_body_type='json',
            body_type='json'
        )

        def request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert 'value' == request.querystring['key1'][0]
            assert '1' == request.querystring['key2'][0]
            assert 'True' == request.querystring['key3'][0]
            assert 'global-value' == request.querystring['global-query'][0]
            assert 'extends-value' == request.querystring['extends-key'][0]
            assert 'TestAPI' == request.querystring['Action'][0]
            assert '2022-06-01' == request.querystring['Version'][0]
            assert 'json' == request.querystring['Format'][0]
            assert None is not request.querystring['Timestamp'][0]
            assert None is not request.querystring['SignatureNonce'][0]
            assert 'global-value' == request.headers.get('global-key')
            assert 'extends-value' == request.headers.get('extends-key')
            assert 'test.alibabacloud.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent',
                                        request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

    @aioresponses()
    def test_call_api_for_rpcwith_v2sign_anonymous_json_async(self, m):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/',
            method='POST',
            auth_type='Anonymous',
            style='RPC',
            req_body_type='json',
            body_type='json'
        )

        def request_callback(url, **request):
            assert None is not re.match(
                '.+Action=TestAPI&Format=json&SignatureNonce=.+&Timestamp=.+&Version=2022-06-01&extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
                str(url))
            assert 'global-value' == request['headers'].get('global-key')
            assert 'extends-value' == request['headers'].get('extends-key')
            assert 'test.alibabacloud.com' == request['headers'].get('host')
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent',
                                        request['headers'].get('user-agent'))
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)

        responseHeaders = {'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'}
        m.post(re.compile(r'http://test\.alibabacloud\.com/.*'),
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_roawith_v2sign_ak_form(self):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test',
            method='POST',
            auth_type='AK',
            style='ROA',
            req_body_type='formData',
            body_type='json'
        )

        def request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert 'value' == request.querystring['key1'][0]
            assert '1' == request.querystring['key2'][0]
            assert 'True' == request.querystring['key3'][0]
            assert 'global-value' == request.querystring['global-query'][0]
            assert 'extends-value' == request.querystring['extends-key'][0]
            assert 'global-value' == request.headers.get('global-key')
            assert 'extends-value' == request.headers.get('extends-key')
            assert 'sdk' == request.headers.get('for-test')
            assert 'test.alibabacloud.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert 'HMAC-SHA1' == request.headers.get('x-acs-signature-method')
            assert '1.0' == request.headers.get('x-acs-signature-version')
            assert 'ak' == request.headers.get('x-acs-accesskey-id')
            assert 'token' == request.headers.get('x-acs-security-token')
            assert None is not request.headers.get('date')
            assert None is not request.headers.get('x-acs-signature-nonce')

            assert None is not re.match('acs ak:.+', request.headers.get('authorization'))
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com/test",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

        # bearer token
        config = self.create_bearer_token_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test1.alibabacloud.com'
        client = OpenApiClient(config)

        def bearer_request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert 'token' == request.headers.get('x-acs-bearer-token')
            assert 'BEARERTOKEN' == request.headers.get('x-acs-signature-type')
            # assert 'bearer token' == request.headers.get('authorization')
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test1.alibabacloud.com/test",
            body=bearer_request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual(200, result.get('statusCode'))

        # anonymous error
        config = self.create_anonymous_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        try:
            client.call_api(params, request, runtime)
        except TeaException as e:
            self.assertEqual('Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.', e.message)
            self.assertEqual('InvalidCredentials', e.code)

        requestBody = '{"key1":"value","key2":1,"key3":true}'
        # bearer token
        config = self.create_bearer_token_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test2.alibabacloud.com'
        client = OpenApiClient(config)
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test',
            method='POST',
            auth_type='AK',
            style='ROA',
            req_body_type='json',
            body_type='json'
        )

        def bearer_json_request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert 'token' == request.headers.get('x-acs-bearer-token')
            assert 'BEARERTOKEN' == request.headers.get('x-acs-signature-type')
            # assert 'bearer token' == request.headers.get('authorization')
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/json; charset=utf-8', 'expected application/json but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test2.alibabacloud.com/test",
            body=bearer_json_request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual(200, result.get('statusCode'))

        # anonymous error
        config = self.create_anonymous_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        try:
            client.call_api(params, request, runtime)
        except TeaException as e:
            self.assertEqual('Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.', e.message)
            self.assertEqual('InvalidCredentials', e.code)

    @aioresponses()
    def test_call_api_for_roawith_v2sign_ak_form_async(self, m):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test',
            method='POST',
            auth_type='AK',
            style='ROA',
            req_body_type='formData',
            body_type='json'
        )

        def request_callback(url, **request):
            assert 'http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True' == str(url)
            assert 'global-value' == request['headers'].get('global-key')
            assert 'extends-value' == request['headers'].get('extends-key')
            assert 'sdk' == request['headers'].get('for-test')
            assert 'test.alibabacloud.com' == request['headers'].get('host')
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert 'application/json' == request['headers'].get('accept')
            assert 'HMAC-SHA1' == request['headers'].get('x-acs-signature-method')
            assert '1.0' == request['headers'].get('x-acs-signature-version')
            assert 'ak' == request['headers'].get('x-acs-accesskey-id')
            assert 'token' == request['headers'].get('x-acs-security-token')
            assert None is not request['headers'].get('date')
            assert None is not request['headers'].get('x-acs-signature-nonce')

            assert None is not re.match('acs ak:.+', request['headers'].get('authorization'))
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request['headers'].get('user-agent'))
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)

        responseHeaders = {'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'}
        m.post('http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

        # bearer token
        config = self.create_bearer_token_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test1.alibabacloud.com'
        client = OpenApiClient(config)

        def bearer_request_callback(url, **request):
            assert 'http://test1.alibabacloud.com/test?extends-key=extends-value&key1=value&key2=1&key3=True' == str(url)
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert 'application/json' == request['headers'].get('accept')
            assert 'token' == request['headers'].get('x-acs-bearer-token')
            assert 'BEARERTOKEN' == request['headers'].get('x-acs-signature-type')
            # assert 'bearer token' == request.headers.get('authorization')
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)

        m.post('http://test1.alibabacloud.com/test?extends-key=extends-value&key1=value&key2=1&key3=True',
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=bearer_request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual(200, result.get('statusCode'))

        # anonymous error
        config = self.create_anonymous_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        try:
            loop.run_until_complete(client.call_api_async(params, request, runtime))
            assert False
        except TeaException as e:
            self.assertEqual('Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.', e.message)
            self.assertEqual('InvalidCredentials', e.code)

        requestBody = '{"key1":"value","key2":1,"key3":true}'
        # bearer token
        config = self.create_bearer_token_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test2.alibabacloud.com'
        client = OpenApiClient(config)
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test',
            method='POST',
            auth_type='AK',
            style='ROA',
            req_body_type='json',
            body_type='json'
        )

        def bearer_json_request_callback(url, **request):
            assert 'http://test2.alibabacloud.com/test?extends-key=extends-value&key1=value&key2=1&key3=True' == str(url)
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert 'application/json' == request['headers'].get('accept')
            assert 'token' == request['headers'].get('x-acs-bearer-token')
            assert 'BEARERTOKEN' == request['headers'].get('x-acs-signature-type')
            # assert 'bearer token' == request.headers.get('authorization')
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/json; charset=utf-8', 'expected application/json but received Content-Type: {}'.format(
                content_type)

        m.post('http://test2.alibabacloud.com/test?extends-key=extends-value&key1=value&key2=1&key3=True',
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=bearer_json_request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual(200, result.get('statusCode'))

        # anonymous error
        config = self.create_anonymous_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        try:
            loop.run_until_complete(client.call_api_async(params, request, runtime))
            assert False
        except TeaException as e:
            self.assertEqual('Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.', e.message)
            self.assertEqual('InvalidCredentials', e.code)

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_roawith_v2sign_anonymous_json(self):
        requestBody = '{"key1":"value","key2":1,"key3":true}'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test',
            method='POST',
            auth_type='Anonymous',
            style='ROA',
            req_body_type='json',
            body_type='json'
        )

        def request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert 'value' == request.querystring['key1'][0]
            assert '1' == request.querystring['key2'][0]
            assert 'True' == request.querystring['key3'][0]
            assert 'global-value' == request.querystring['global-query'][0]
            assert 'extends-value' == request.querystring['extends-key'][0]
            assert 'global-value' == request.headers.get('global-key')
            assert 'extends-value' == request.headers.get('extends-key')
            assert 'sdk' == request.headers.get('for-test')
            assert 'test.alibabacloud.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert 'HMAC-SHA1' == request.headers.get('x-acs-signature-method')
            assert '1.0' == request.headers.get('x-acs-signature-version')
            assert None is not request.headers.get('date')
            assert None is not request.headers.get('x-acs-signature-nonce')
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/json; charset=utf-8', 'expected application/json but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com/test",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

    @aioresponses()
    def test_call_api_for_roawith_v2sign_anonymous_json_async(self, m):
        requestBody = '{"key1":"value","key2":1,"key3":true}'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test',
            method='POST',
            auth_type='Anonymous',
            style='ROA',
            req_body_type='json',
            body_type='json'
        )

        def request_callback(url, **request):
            assert 'http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True' == str(url)
            assert 'global-value' == request['headers'].get('global-key')
            assert 'extends-value' == request['headers'].get('extends-key')
            assert 'sdk' == request['headers'].get('for-test')
            assert 'test.alibabacloud.com' == request['headers'].get('host')
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert 'application/json' == request['headers'].get('accept')
            assert 'HMAC-SHA1' == request['headers'].get('x-acs-signature-method')
            assert '1.0' == request['headers'].get('x-acs-signature-version')
            assert None is not request['headers'].get('date')
            assert None is not request['headers'].get('x-acs-signature-nonce')
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request['headers'].get('user-agent'))
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/json; charset=utf-8', 'expected application/json but received Content-Type: {}'.format(
                content_type)

        responseHeaders = {'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'}
        m.post('http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_rpcwith_v3sign_ak_form(self):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/',
            method='POST',
            auth_type='AK',
            style='RPC',
            req_body_type='formData',
            body_type='json'
        )

        def request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert 'value' == request.querystring['key1'][0]
            assert '1' == request.querystring['key2'][0]
            assert 'True' == request.querystring['key3'][0]
            assert 'global-value' == request.querystring['global-query'][0]
            assert 'extends-value' == request.querystring['extends-key'][0]
            assert 'sdk' == request.headers.get('for-test')
            assert 'global-value' == request.headers.get('global-key')
            assert 'extends-value' == request.headers.get('extends-key')
            assert 'test.alibabacloud.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert 'ak' == request.headers.get('x-acs-accesskey-id')
            assert 'token' == request.headers.get('x-acs-security-token')
            assert 'static_sts' == request.headers.get('x-acs-credentials-provider')
            assert None is not request.headers.get('x-acs-date')
            assert None is not request.headers.get('x-acs-signature-nonce')
            assert None is not request.headers.get('x-acs-content-sha256')

            assert None is not re.match(
                'ACS3-HMAC-SHA256 Credential=ak,SignedHeaders=accept;content-type;extends-key;for-test;global-key;' +
                'host;user-agent;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-credentials-provider;x-acs-date;x-acs-security-token;' +
                'x-acs-signature-nonce;x-acs-version,Signature=.+', request.headers.get('Authorization'))
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

        # bearer token
        config = self.create_bearer_token_config()
        config.protocol = 'HTTP'
        config.endpoint = 'test1.alibabacloud.com'
        client = OpenApiClient(config)

        def bearer_request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert 'token' == request.headers.get('x-acs-bearer-token')
            assert 'BEARERTOKEN' == request.querystring['SignatureType'][0]
            # assert 'bearer token' == request.headers.get('authorization')
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test1.alibabacloud.com",
            body=bearer_request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual(200, result.get('statusCode'))

        # anonymous error
        config = self.create_anonymous_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        try:
            client.call_api(params, request, runtime)
        except TeaException as e:
            self.assertEqual('Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.', e.message)
            self.assertEqual('InvalidCredentials', e.code)

    @aioresponses()
    def test_call_api_for_rpcwith_v3sign_ak_form_async(self, m):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/',
            method='POST',
            auth_type='AK',
            style='RPC',
            req_body_type='formData',
            body_type='json'
        )

        def request_callback(url, **request):
            assert 'http://test.alibabacloud.com/?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True' == str(url)
            assert 'sdk' == request['headers'].get('for-test')
            assert 'global-value' == request['headers'].get('global-key')
            assert 'extends-value' == request['headers'].get('extends-key')
            assert 'test.alibabacloud.com' == request['headers'].get('host')
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert 'application/json' == request['headers'].get('accept')
            assert 'ak' == request['headers'].get('x-acs-accesskey-id')
            assert 'token' == request['headers'].get('x-acs-security-token')
            assert 'static_sts' == request['headers'].get('x-acs-credentials-provider')
            assert None is not request['headers'].get('x-acs-date')
            assert None is not request['headers'].get('x-acs-signature-nonce')
            assert None is not request['headers'].get('x-acs-content-sha256')

            assert None is not re.match(
                'ACS3-HMAC-SHA256 Credential=ak,SignedHeaders=accept;content-type;extends-key;for-test;global-key;' +
                'host;user-agent;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-credentials-provider;x-acs-date;x-acs-security-token;' +
                'x-acs-signature-nonce;x-acs-version,Signature=.+', request['headers'].get('Authorization'))
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request['headers'].get('user-agent'))
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)

        responseHeaders = {'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'}
        m.post('http://test.alibabacloud.com/?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

        # bearer token
        config = self.create_bearer_token_config()
        config.protocol = 'HTTP'
        config.endpoint = 'test1.alibabacloud.com'
        client = OpenApiClient(config)

        def bearer_request_callback(url, **request):
            assert 'http://test1.alibabacloud.com/?SignatureType=BEARERTOKEN&extends-key=extends-value&key1=value&key2=1&key3=True' == str(url)
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert 'application/json' == request['headers'].get('accept')
            assert 'token' == request['headers'].get('x-acs-bearer-token')
            # assert 'bearer token' == request.headers.get('authorization')
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)

        responseHeaders = {'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'}
        m.post('http://test1.alibabacloud.com/?SignatureType=BEARERTOKEN&extends-key=extends-value&key1=value&key2=1&key3=True',
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=bearer_request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual(200, result.get('statusCode'))

        # anonymous error
        config = self.create_anonymous_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        try:
            loop.run_until_complete(client.call_api_async(params, request, runtime))
            assert False
        except TeaException as e:
            self.assertEqual('Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.', e.message)
            self.assertEqual('InvalidCredentials', e.code)

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_rpcwith_v3sign_anonymous_json(self):
        requestBody = '{"key1":"value","key2":1,"key3":true}'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/',
            method='POST',
            auth_type='Anonymous',
            style='RPC',
            req_body_type='json',
            body_type='json'
        )

        def request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert 'value' == request.querystring['key1'][0]
            assert '1' == request.querystring['key2'][0]
            assert 'True' == request.querystring['key3'][0]
            assert 'global-value' == request.querystring['global-query'][0]
            assert 'extends-value' == request.querystring['extends-key'][0]
            assert 'sdk' == request.headers.get('for-test')
            assert 'global-value' == request.headers.get('global-key')
            assert 'extends-value' == request.headers.get('extends-key')
            assert 'test.alibabacloud.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert None is not request.headers.get('x-acs-date')
            assert None is not request.headers.get('x-acs-signature-nonce')
            assert None is not request.headers.get('x-acs-content-sha256')

            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/json; charset=utf-8', 'expected application/json but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

    @aioresponses()
    def test_call_api_for_rpcwith_v3sign_anonymous_json_async(self, m):
        requestBody = '{"key1":"value","key2":1,"key3":true}'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/',
            method='POST',
            auth_type='Anonymous',
            style='RPC',
            req_body_type='json',
            body_type='json'
        )

        def request_callback(url, **request):
            assert 'http://test.alibabacloud.com/?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True' == str(url)
            assert 'sdk' == request['headers'].get('for-test')
            assert 'global-value' == request['headers'].get('global-key')
            assert 'extends-value' == request['headers'].get('extends-key')
            assert 'test.alibabacloud.com' == request['headers'].get('host')
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert 'application/json' == request['headers'].get('accept')
            assert None is not request['headers'].get('x-acs-date')
            assert None is not request['headers'].get('x-acs-signature-nonce')
            assert None is not request['headers'].get('x-acs-content-sha256')

            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request['headers'].get('user-agent'))
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/json; charset=utf-8', 'expected application/json but received Content-Type: {}'.format(
                content_type)

        responseHeaders = {'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'}
        m.post('http://test.alibabacloud.com/?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_roawith_v3sign_ak_form(self):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test',
            method='POST',
            auth_type='AK',
            style='ROA',
            req_body_type='formData',
            body_type='json'
        )

        def request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert 'value' == request.querystring['key1'][0]
            assert '1' == request.querystring['key2'][0]
            assert 'True' == request.querystring['key3'][0]
            assert 'global-value' == request.querystring['global-query'][0]
            assert 'extends-value' == request.querystring['extends-key'][0]
            assert 'sdk' == request.headers.get('for-test')
            assert 'global-value' == request.headers.get('global-key')
            assert 'extends-value' == request.headers.get('extends-key')
            assert 'test.alibabacloud.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert 'ak' == request.headers.get('x-acs-accesskey-id')
            assert 'token' == request.headers.get('x-acs-security-token')
            assert 'static_sts' == request.headers.get('x-acs-credentials-provider')
            assert None is not request.headers.get('x-acs-date')
            assert None is not request.headers.get('x-acs-signature-nonce')
            assert None is not request.headers.get('x-acs-content-sha256')

            assert None is not re.match(
                'ACS3-HMAC-SHA256 Credential=ak,SignedHeaders=accept;content-type;extends-key;for-test;global-key;' +
                'host;user-agent;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-credentials-provider;x-acs-date;x-acs-security-token;' +
                'x-acs-signature-nonce;x-acs-version,Signature=.+', request.headers.get('Authorization'))
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com/test",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

        # bearer token
        config = self.create_bearer_token_config()
        config.protocol = 'HTTP'
        config.endpoint = 'test1.alibabacloud.com'
        client = OpenApiClient(config)

        def bearer_request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert 'token' == request.headers.get('x-acs-bearer-token')
            assert 'BEARERTOKEN' == request.headers.get('x-acs-signature-type')
            # assert 'bearer token' == request.headers.get('authorization')
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test1.alibabacloud.com/test",
            body=bearer_request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual(200, result.get('statusCode'))

        # anonymous error
        config = self.create_anonymous_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        try:
            client.call_api(params, request, runtime)
        except TeaException as e:
            self.assertEqual('Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.', e.message)
            self.assertEqual('InvalidCredentials', e.code)

    @aioresponses()
    def test_call_api_for_roawith_v3sign_ak_form_async(self, m):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test',
            method='POST',
            auth_type='AK',
            style='ROA',
            req_body_type='formData',
            body_type='json'
        )

        def request_callback(url, **request):
            assert 'http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True' == str(url)
            assert 'sdk' == request['headers'].get('for-test')
            assert 'global-value' == request['headers'].get('global-key')
            assert 'extends-value' == request['headers'].get('extends-key')
            assert 'test.alibabacloud.com' == request['headers'].get('host')
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert 'application/json' == request['headers'].get('accept')
            assert 'ak' == request['headers'].get('x-acs-accesskey-id')
            assert 'token' == request['headers'].get('x-acs-security-token')
            assert 'static_sts' == request['headers'].get('x-acs-credentials-provider')
            assert None is not request['headers'].get('x-acs-date')
            assert None is not request['headers'].get('x-acs-signature-nonce')
            assert None is not request['headers'].get('x-acs-content-sha256')

            assert None is not re.match(
                'ACS3-HMAC-SHA256 Credential=ak,SignedHeaders=accept;content-type;extends-key;for-test;global-key;' +
                'host;user-agent;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-credentials-provider;x-acs-date;x-acs-security-token;' +
                'x-acs-signature-nonce;x-acs-version,Signature=.+', request['headers'].get('Authorization'))
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request['headers'].get('user-agent'))
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)

        responseHeaders = {'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'}
        m.post('http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

        # bearer token
        config = self.create_bearer_token_config()
        config.protocol = 'HTTP'
        config.endpoint = 'test1.alibabacloud.com'
        client = OpenApiClient(config)

        def bearer_request_callback(url, **request):
            assert 'http://test1.alibabacloud.com/test?extends-key=extends-value&key1=value&key2=1&key3=True' == str(url)
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert 'application/json' == request['headers'].get('accept')
            assert 'token' == request['headers'].get('x-acs-bearer-token')
            assert 'BEARERTOKEN' == request['headers'].get('x-acs-signature-type')
            # assert 'bearer token' == request.headers.get('authorization')
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)

        m.post('http://test1.alibabacloud.com/test?extends-key=extends-value&key1=value&key2=1&key3=True',
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=bearer_request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual(200, result.get('statusCode'))

        # anonymous error
        config = self.create_anonymous_config()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        try:
            loop.run_until_complete(client.call_api_async(params, request, runtime))
            assert False
        except TeaException as e:
            self.assertEqual('Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.', e.message)
            self.assertEqual('InvalidCredentials', e.code)

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_roawith_v3sign_anonymous_json(self):
        requestBody = '{"key1":"value","key2":1,"key3":true}'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test',
            method='POST',
            auth_type='Anonymous',
            style='ROA',
            req_body_type='json',
            body_type='json'
        )

        def request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert 'value' == request.querystring['key1'][0]
            assert '1' == request.querystring['key2'][0]
            assert 'True' == request.querystring['key3'][0]
            assert 'global-value' == request.querystring['global-query'][0]
            assert 'extends-value' == request.querystring['extends-key'][0]
            assert 'sdk' == request.headers.get('for-test')
            assert 'global-value' == request.headers.get('global-key')
            assert 'extends-value' == request.headers.get('extends-key')
            assert 'test.alibabacloud.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert None is not request.headers.get('x-acs-date')
            assert None is not request.headers.get('x-acs-signature-nonce')
            assert None is not request.headers.get('x-acs-content-sha256')

            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/json; charset=utf-8', 'expected application/json but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com/test",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

    @aioresponses()
    def test_call_api_for_roawith_v3sign_anonymous_json_async(self, m):
        requestBody = '{"key1":"value","key2":1,"key3":true}'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test',
            method='POST',
            auth_type='Anonymous',
            style='ROA',
            req_body_type='json',
            body_type='json'
        )

        def request_callback(url, **request):
            assert 'http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True' == str(url)
            assert 'sdk' == request['headers'].get('for-test')
            assert 'global-value' == request['headers'].get('global-key')
            assert 'extends-value' == request['headers'].get('extends-key')
            assert 'test.alibabacloud.com' == request['headers'].get('host')
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert 'application/json' == request['headers'].get('accept')
            assert None is not request['headers'].get('x-acs-date')
            assert None is not request['headers'].get('x-acs-signature-nonce')
            assert None is not request['headers'].get('x-acs-content-sha256')

            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request['headers'].get('user-agent'))
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/json; charset=utf-8', 'expected application/json but received Content-Type: {}'.format(
                content_type)

        responseHeaders = {'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'}
        m.post('http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

    @httpretty.activate(allow_net_connect=False)
    def test_response_body_type(self):
        requestBody = 'key1=value&key2=1&key3=True'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        request.headers['type'] = 'json'
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test',
            method='POST',
            auth_type='AK',
            style='ROA',
            req_body_type='formData',
            body_type='json'
        )

        def request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert 'value' == request.querystring['key1'][0]
            assert '1' == request.querystring['key2'][0]
            assert 'True' == request.querystring['key3'][0]
            assert 'global-value' == request.querystring['global-query'][0]
            assert 'extends-value' == request.querystring['extends-key'][0]
            assert 'sdk' == request.headers.get('for-test')
            assert 'global-value' == request.headers.get('global-key')
            assert 'extends-value' == request.headers.get('extends-key')
            assert 'test.alibabacloud.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert 'ak' == request.headers.get('x-acs-accesskey-id')
            assert 'token' == request.headers.get('x-acs-security-token')
            assert None is not request.headers.get('x-acs-date')
            assert None is not request.headers.get('x-acs-signature-nonce')
            assert None is not request.headers.get('x-acs-content-sha256')

            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            if request.headers.get('type') == 'array':
                return [200, headers, '["AppId", "ClassId", "UserId"]']
            elif request.headers.get('type') == 'error':
                return [400, headers, '{"Code":"error code", "Message":"error message", '
                                      '"RequestId":"A45EE076-334D-5012-9746-A8F828D20FD4", '
                                      '"Description":"error description", "AccessDeniedDetail":{}}']
            elif request.headers.get('type') == 'error1':
                return [400, headers, '{"Code":"error code", "Message":"error message", '
                                      '"RequestId":"A45EE076-334D-5012-9746-A8F828D20FD4", '
                                      '"Description":"error description", "AccessDeniedDetail":{}, "accessDeniedDetail":{"test": 0}}']
            elif request.headers.get('type') == 'error2':
                return [400, headers, '{"Code":"error code", "Message":"error message", '
                                      '"RequestId":"A45EE076-334D-5012-9746-A8F828D20FD4", '
                                      '"Description":"error description", "accessDeniedDetail":{"test": 0}}']
            return [200, headers, '{"AppId":"test", "ClassId":"test", "UserId":123}']

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com/test",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

        params.body_type = 'array'
        request.headers.update({'type': 'array'})
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('AppId', result.get('body')[0])
        self.assertEqual('ClassId', result.get('body')[1])
        self.assertEqual('UserId', result.get('body')[2])
        self.assertEqual(200, result.get('statusCode'))

        params.body_type = 'string'
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('["AppId", "ClassId", "UserId"]', result.get('body'))
        self.assertEqual(200, result.get('statusCode'))

        params.body_type = 'byte'
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('["AppId", "ClassId", "UserId"]', result.get('body').decode('utf-8'))
        self.assertEqual(200, result.get('statusCode'))

        request.headers.update({'type': 'error'})
        try:
            client.call_api(params, request, runtime)
        except TeaException as e:
            self.assertEqual('code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4', e.message)
            self.assertEqual('error code', e.code)
            self.assertFalse('test' in e.accessDeniedDetail)

        request.headers.update({'type': 'error1'})
        try:
            client.call_api(params, request, runtime)
        except TeaException as e:
            self.assertEqual('code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4', e.message)
            self.assertEqual('error code', e.code)
            self.assertFalse('test' in e.accessDeniedDetail)

        request.headers.update({'type': 'error2'})
        try:
            client.call_api(params, request, runtime)
        except TeaException as e:
            self.assertEqual('code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4', e.message)
            self.assertEqual('error code', e.code)
            self.assertEqual(0, e.accessDeniedDetail['test'])

    @aioresponses()
    def test_response_body_type_async(self, m):
        requestBody = 'key1=value&key2=1&key3=True'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        request = self.create_open_api_request()
        request.headers['type'] = 'json'
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test',
            method='POST',
            auth_type='AK',
            style='ROA',
            req_body_type='formData',
            body_type='json'
        )

        def request_callback(url, **request):
            assert 'http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True' == str(url)
            assert 'sdk' == request['headers'].get('for-test')
            assert 'global-value' == request['headers'].get('global-key')
            assert 'extends-value' == request['headers'].get('extends-key')
            assert 'test.alibabacloud.com' == request['headers'].get('host')
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert 'application/json' == request['headers'].get('accept')
            assert 'ak' == request['headers'].get('x-acs-accesskey-id')
            assert 'token' == request['headers'].get('x-acs-security-token')
            assert None is not request['headers'].get('x-acs-date')
            assert None is not request['headers'].get('x-acs-signature-nonce')
            assert None is not request['headers'].get('x-acs-content-sha256')

            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request['headers'].get('user-agent'))
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)

        responseHeaders = {'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'}
        m.post('http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body='{"AppId":"test", "ClassId":"test", "UserId":123}',
               status=200,
               headers=responseHeaders,
               callback=request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

        m.post('http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body='["AppId", "ClassId", "UserId"]',
               status=200,
               headers=responseHeaders,
               callback=request_callback)

        params.body_type = 'array'
        request.headers.update({'type': 'array'})

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('AppId', result.get('body')[0])
        self.assertEqual('ClassId', result.get('body')[1])
        self.assertEqual('UserId', result.get('body')[2])
        self.assertEqual(200, result.get('statusCode'))

        m.post('http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body='["AppId", "ClassId", "UserId"]',
               status=200,
               headers=responseHeaders,
               callback=request_callback)
        params.body_type = 'string'

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('["AppId", "ClassId", "UserId"]', result.get('body'))
        self.assertEqual(200, result.get('statusCode'))

        m.post('http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body='["AppId", "ClassId", "UserId"]',
               status=200,
               headers=responseHeaders,
               callback=request_callback)
        params.body_type = 'byte'

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('["AppId", "ClassId", "UserId"]', result.get('body').decode('utf-8'))
        self.assertEqual(200, result.get('statusCode'))

        m.post('http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body='{"Code":"error code", "Message":"error message", '
                    '"RequestId":"A45EE076-334D-5012-9746-A8F828D20FD4", '
                    '"Description":"error description", "AccessDeniedDetail":{}}',
               status=400,
               headers=responseHeaders,
               callback=request_callback)
        request.headers.update({'type': 'error'})

        try:
            loop.run_until_complete(client.call_api_async(params, request, runtime))
            assert False
        except TeaException as e:
            self.assertEqual('code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4', e.message)
            self.assertEqual('error code', e.code)
            self.assertFalse('test' in e.accessDeniedDetail)

        m.post('http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body='{"Code":"error code", "Message":"error message", '
                    '"RequestId":"A45EE076-334D-5012-9746-A8F828D20FD4", '
                    '"Description":"error description", "AccessDeniedDetail":{}, "accessDeniedDetail":{"test": 0}}',
               status=400,
               headers=responseHeaders,
               callback=request_callback)
        request.headers.update({'type': 'error1'})

        try:
            loop.run_until_complete(client.call_api_async(params, request, runtime))
            assert False
        except TeaException as e:
            self.assertEqual('code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4', e.message)
            self.assertEqual('error code', e.code)
            self.assertFalse('test' in e.accessDeniedDetail)

        m.post('http://test.alibabacloud.com/test?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body='{"Code":"error code", "Message":"error message", '
                    '"RequestId":"A45EE076-334D-5012-9746-A8F828D20FD4", '
                    '"Description":"error description", "accessDeniedDetail":{"test": 0}}',
               status=400,
               headers=responseHeaders,
               callback=request_callback)
        request.headers.update({'type': 'error2'})

        try:
            loop.run_until_complete(client.call_api_async(params, request, runtime))
            assert False
        except TeaException as e:
            self.assertEqual('code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4', e.message)
            self.assertEqual('error code', e.code)
            self.assertEqual(0, e.accessDeniedDetail['test'])

    @httpretty.activate(allow_net_connect=False)
    def test_request_body_type(self):
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        # formData
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test1',
            method='POST',
            auth_type='AK',
            style='ROA',
            req_body_type='formData',
            body_type='json'
        )
        body = {}
        body['key1'] = 'value'
        body['key2'] = 1
        body['key3'] = True
        request = open_api_models.OpenApiRequest(
            body=OpenApiUtilClient.parse_to_map(body)
        )

        def request_callback_1(request: HTTPrettyRequest, uri: str, headers: dict):
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == 'key1=value&key2=1&key3=True', 'unexpected body: {}'.format(
                request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)
            return [200, headers, '{"AppId":"test", "ClassId":"test", "UserId":123}']

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com/test1",
            body=request_callback_1)
        result = client.call_api(params, request, runtime)
        self.assertEqual(200, result.get('statusCode'))

        # json
        params.pathname = '/test2'
        params.req_body_type = 'json'

        def request_callback_2(request: HTTPrettyRequest, uri: str, headers: dict):
            content_type = request.headers.get('content-type')
            assert request.body.decode(
                'utf-8') == '{"key1":"value","key2":1,"key3":true}', 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/json; charset=utf-8', 'expected application/json but received Content-Type: {}'.format(
                content_type)
            return [200, headers, '{"AppId":"test", "ClassId":"test", "UserId":123}']

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com/test2",
            body=request_callback_2)
        result = client.call_api(params, request, runtime)
        self.assertEqual(200, result.get('statusCode'))

        # byte
        params.pathname = '/test3'
        params.req_body_type = 'byte'
        byte_body = UtilClient.to_bytes('test byte')
        request = open_api_models.OpenApiRequest(
            body=byte_body
        )

        def request_callback_3(request: HTTPrettyRequest, uri: str, headers: dict):
            content_type = request.headers.get('content-type')
            assert request.body == UtilClient.to_bytes('test byte'), 'unexpected body: {}'.format(request.body)
            assert content_type is None, 'expected text/plain but received Content-Type: {}'.format(
                content_type)
            return [200, headers, '{"AppId":"test", "ClassId":"test", "UserId":123}']

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com/test3",
            body=request_callback_3)
        result = client.call_api(params, request, runtime)
        self.assertEqual(200, result.get('statusCode'))

        # stream
        params.pathname = '/test4'
        params.req_body_type = 'binary'
        request = open_api_models.OpenApiRequest(
            stream=byte_body
        )

        def request_callback_4(request: HTTPrettyRequest, uri: str, headers: dict):
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == 'test byte', 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/octet-stream', 'expected application/octet-stream but received Content-Type: {}'.format(
                content_type)
            return [200, headers, '{"AppId":"test", "ClassId":"test", "UserId":123}']

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com/test4",
            body=request_callback_4)
        result = client.call_api(params, request, runtime)
        self.assertEqual(200, result.get('statusCode'))

    @aioresponses()
    def test_request_body_type_async(self, m):
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        # formData
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/test1',
            method='POST',
            auth_type='AK',
            style='ROA',
            req_body_type='formData',
            body_type='json'
        )
        body = {}
        body['key1'] = 'value'
        body['key2'] = 1
        body['key3'] = True
        request = open_api_models.OpenApiRequest(
            body=OpenApiUtilClient.parse_to_map(body)
        )

        def request_callback_1(url, **request):
            assert 'http://test.alibabacloud.com/test1?extends-key=extends-value&global-query=global-value' == str(url)
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == 'key1=value&key2=1&key3=True', 'unexpected body: {}'.format(
                request['data'])
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/x-www-form-urlencoded but received Content-Type: {}'.format(
                content_type)

        m.post('http://test.alibabacloud.com/test1?extends-key=extends-value&global-query=global-value',
               body='{"AppId":"test", "ClassId":"test", "UserId":123}',
               status=200,
               callback=request_callback_1)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual(200, result.get('statusCode'))

        # json
        params.pathname = '/test2'
        params.req_body_type = 'json'

        def request_callback_2(url, **request):
            assert 'http://test.alibabacloud.com/test2?extends-key=extends-value&global-query=global-value' == str(url)
            content_type = request['headers'].get('content-type')
            assert request['data'].decode(
                'utf-8') == '{"key1":"value","key2":1,"key3":true}', 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/json; charset=utf-8', 'expected application/json but received Content-Type: {}'.format(
                content_type)

        m.post('http://test.alibabacloud.com/test2?extends-key=extends-value&global-query=global-value',
               body='{"AppId":"test", "ClassId":"test", "UserId":123}',
               status=200,
               callback=request_callback_2)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual(200, result.get('statusCode'))

        # byte
        params.pathname = '/test3'
        params.req_body_type = 'byte'
        byte_body = UtilClient.to_bytes('test byte')
        request = open_api_models.OpenApiRequest(
            body=byte_body
        )

        def request_callback_3(url, **request):
            assert 'http://test.alibabacloud.com/test3?extends-key=extends-value&global-query=global-value' == str(url)
            content_type = request['headers'].get('content-type')
            assert request['data'] == UtilClient.to_bytes('test byte'), 'unexpected body: {}'.format(request['data'])
            assert content_type is None, 'expected text/plain but received Content-Type: {}'.format(
                content_type)

        m.post('http://test.alibabacloud.com/test3?extends-key=extends-value&global-query=global-value',
               body='{"AppId":"test", "ClassId":"test", "UserId":123}',
               status=200,
               callback=request_callback_3)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual(200, result.get('statusCode'))

        # stream
        params.pathname = '/test4'
        params.req_body_type = 'binary'
        request = open_api_models.OpenApiRequest(
            stream=byte_body
        )

        def request_callback_4(url, **request):
            assert 'http://test.alibabacloud.com/test4?extends-key=extends-value&global-query=global-value' == str(url)
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == 'test byte', 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/octet-stream', 'expected application/octet-stream but received Content-Type: {}'.format(
                content_type)

        m.post('http://test.alibabacloud.com/test4?extends-key=extends-value&global-query=global-value',
               body='{"AppId":"test", "ClassId":"test", "UserId":123}',
               status=200,
               callback=request_callback_4)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.call_api_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual(200, result.get('statusCode'))

    @httpretty.activate
    def test_execute(self):
        requestBody = '{"key1":"value","key2":1,"key3":true}'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        client._product_id = 'test'
        gateway_client = PopClient()
        client.set_gateway_client(gateway_client)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/',
            method='POST',
            auth_type='Anonymous',
            style='RPC',
            req_body_type='json',
            body_type='json'
        )

        def request_callback(request: HTTPrettyRequest, uri: str, headers: dict):
            assert 'value' == request.querystring['key1'][0]
            assert '1' == request.querystring['key2'][0]
            assert 'True' == request.querystring['key3'][0]
            assert 'global-value' == request.querystring['global-query'][0]
            assert 'extends-value' == request.querystring['extends-key'][0]
            assert 'sdk' == request.headers.get('for-test')
            assert 'global-value' == request.headers.get('global-key')
            assert 'extends-value' == request.headers.get('extends-key')
            assert 'test.alibabacloud.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert None is not request.headers.get('x-acs-date')
            assert None is not request.headers.get('x-acs-signature-nonce')
            assert None is not request.headers.get('x-acs-content-sha256')

            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/json; charset=utf-8', 'expected application/json but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.alibabacloud.com",
            body=request_callback)
        result = client.execute(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))

    @aioresponses()
    def test_execute_async(self, m):
        requestBody = '{"key1":"value","key2":1,"key3":true}'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.alibabacloud.com'
        client = OpenApiClient(config)
        client._product_id = 'test'
        gateway_client = PopClient()
        client.set_gateway_client(gateway_client)
        request = self.create_open_api_request()
        params = open_api_models.Params(
            action='TestAPI',
            version='2022-06-01',
            protocol='HTTPS',
            pathname='/',
            method='POST',
            auth_type='Anonymous',
            style='RPC',
            req_body_type='json',
            body_type='json'
        )

        def request_callback(url, **request):
            assert 'http://test.alibabacloud.com/?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True' == str(url)
            assert 'sdk' == request['headers'].get('for-test')
            assert 'global-value' == request['headers'].get('global-key')
            assert 'extends-value' == request['headers'].get('extends-key')
            assert 'test.alibabacloud.com' == request['headers'].get('host')
            assert '2022-06-01' == request['headers'].get('x-acs-version')
            assert 'TestAPI' == request['headers'].get('x-acs-action')
            assert 'application/json' == request['headers'].get('accept')
            assert None is not request['headers'].get('x-acs-date')
            assert None is not request['headers'].get('x-acs-signature-nonce')
            assert None is not request['headers'].get('x-acs-content-sha256')

            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request['headers'].get('user-agent'))
            content_type = request['headers'].get('content-type')
            assert request['data'].decode('utf-8') == requestBody, 'unexpected body: {}'.format(request['data'])
            assert content_type == 'application/json; charset=utf-8', 'expected application/json but received Content-Type: {}'.format(
                content_type)

        responseHeaders = {'x-acs-request-id': 'A45EE076-334D-5012-9746-A8F828D20FD4'}
        m.post('http://test.alibabacloud.com/?extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=True',
               body=responseBody,
               status=200,
               headers=responseHeaders,
               callback=request_callback)

        loop = asyncio.get_event_loop()
        task = asyncio.ensure_future(
            client.execute_async(params, request, runtime)
        )
        loop.run_until_complete(task)
        result = task.result()
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))
        self.assertEqual(200, result.get('statusCode'))
