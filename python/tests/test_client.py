# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
import unittest
import re
import httpretty
from Tea.exceptions import TeaException
from httpretty.core import HTTPrettyRequest

from alibabacloud_tea_openapi import models as open_api_models
from alibabacloud_credentials import models as credential_models
from alibabacloud_credentials.client import Client as CredentialClient
from alibabacloud_tea_openapi.client import Client as OpenApiClient
from alibabacloud_tea_util import models as util_models
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
            global_parameters=global_parameters
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
        config.access_key_id = 'ak'
        config.access_key_secret = 'secret'
        config.security_token = 'token'
        config.type = 'sts'
        client = OpenApiClient(config)
        self.assertEqual('ak', client.get_access_key_id())
        self.assertEqual('secret', client.get_access_key_secret())
        self.assertEqual('token', client.get_security_token())
        self.assertEqual('sts', client.get_type())
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
            global_parameters=global_parameters
        )
        return config

    def create_runtime_options(self) -> util_models.RuntimeOptions:
        runtime = util_models.RuntimeOptions(
            read_timeout=4000,
            connect_timeout=4000,
            max_idle_conns=100,
            autoretry=True,
            max_attempts=1,
            backoff_policy='no',
            backoff_period=1,
            ignore_ssl=True
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
        config.endpoint = 'test.aliyuncs.com'
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
            assert 'test.aliyuncs.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent',
                                        request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/json but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.aliyuncs.com",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_rpcwith_v2sign_anonymous_json(self):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.aliyuncs.com'
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
            assert 'TestAPI' == request.querystring['Action'][0]
            assert '2022-06-01' == request.querystring['Version'][0]
            assert 'json' == request.querystring['Format'][0]
            assert None is not request.querystring['Timestamp'][0]
            assert None is not request.querystring['SignatureNonce'][0]
            assert 'global-value' == request.headers.get('global-key')
            assert 'test.aliyuncs.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent',
                                        request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/json but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.aliyuncs.com",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_roawith_v2sign_ak_form(self):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.aliyuncs.com'
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
            assert 'global-value' == request.headers.get('global-key')
            assert 'sdk' == request.headers.get('for-test')
            assert 'test.aliyuncs.com' == request.headers.get('host')
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
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/json but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.aliyuncs.com/test",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_roawith_v2sign_anonymous_json(self):
        requestBody = '{"key1": "value", "key2": 1, "key3": true}'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.signature_algorithm = 'v2'
        config.endpoint = 'test.aliyuncs.com'
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
            assert 'global-value' == request.headers.get('global-key')
            assert 'sdk' == request.headers.get('for-test')
            assert 'test.aliyuncs.com' == request.headers.get('host')
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
            httpretty.POST, "http://test.aliyuncs.com/test",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_rpcwith_v3sign_ak_form(self):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.aliyuncs.com'
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
            assert 'sdk' == request.headers.get('for-test')
            assert 'global-value' == request.headers.get('global-key')
            assert 'test.aliyuncs.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert 'ak' == request.headers.get('x-acs-accesskey-id')
            assert 'token' == request.headers.get('x-acs-security-token')
            assert None is not request.headers.get('x-acs-date')
            assert None is not request.headers.get('x-acs-signature-nonce')
            assert None is not request.headers.get('x-acs-content-sha256')

            assert None is not re.match(
                'ACS3-HMAC-SHA256 Credential=ak,SignedHeaders=accept;content-type;for-test;global-key;' +
                'host;user-agent;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-date;x-acs-security-token;' +
                'x-acs-signature-nonce;x-acs-version,Signature=.+', request.headers.get('Authorization'))
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/json but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.aliyuncs.com",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_rpcwith_v3sign_anonymous_json(self):
        requestBody = '{"key1": "value", "key2": 1, "key3": true}'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.aliyuncs.com'
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
            assert 'sdk' == request.headers.get('for-test')
            assert 'global-value' == request.headers.get('global-key')
            assert 'test.aliyuncs.com' == request.headers.get('host')
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
            httpretty.POST, "http://test.aliyuncs.com",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_roawith_v3sign_ak_form(self):
        requestBody = 'key1=value&key2=1&key3=True'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.aliyuncs.com'
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
            assert 'sdk' == request.headers.get('for-test')
            assert 'global-value' == request.headers.get('global-key')
            assert 'test.aliyuncs.com' == request.headers.get('host')
            assert '2022-06-01' == request.headers.get('x-acs-version')
            assert 'TestAPI' == request.headers.get('x-acs-action')
            assert 'application/json' == request.headers.get('accept')
            assert 'ak' == request.headers.get('x-acs-accesskey-id')
            assert 'token' == request.headers.get('x-acs-security-token')
            assert None is not request.headers.get('x-acs-date')
            assert None is not request.headers.get('x-acs-signature-nonce')
            assert None is not request.headers.get('x-acs-content-sha256')

            assert None is not re.match(
                'ACS3-HMAC-SHA256 Credential=ak,SignedHeaders=accept;content-type;for-test;global-key;' +
                'host;user-agent;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-date;x-acs-security-token;' +
                'x-acs-signature-nonce;x-acs-version,Signature=.+', request.headers.get('Authorization'))
            assert None is not re.match('AlibabaCloud.+TeaDSL/1 config.userAgent', request.headers.get('user-agent'))
            content_type = request.headers.get('content-type')
            assert request.body.decode('utf-8') == requestBody, 'unexpected body: {}'.format(request.body)
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/json but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            return [200, headers, responseBody]

        httpretty.register_uri(
            httpretty.POST, "http://test.aliyuncs.com/test",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))

    @httpretty.activate(allow_net_connect=False)
    def test_call_api_for_roawith_v3sign_anonymous_json(self):
        requestBody = '{"key1": "value", "key2": 1, "key3": true}'
        responseBody = '{"AppId":"test", "ClassId":"test", "UserId":123}'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.aliyuncs.com'
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
            assert 'sdk' == request.headers.get('for-test')
            assert 'global-value' == request.headers.get('global-key')
            assert 'test.aliyuncs.com' == request.headers.get('host')
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
            httpretty.POST, "http://test.aliyuncs.com/test",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))

    @httpretty.activate(allow_net_connect=False)
    def test_response_body_type(self):
        requestBody = 'key1=value&key2=1&key3=True'
        config = self.create_config()
        runtime = self.create_runtime_options()
        config.protocol = 'HTTP'
        config.endpoint = 'test.aliyuncs.com'
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
            assert 'sdk' == request.headers.get('for-test')
            assert 'global-value' == request.headers.get('global-key')
            assert 'test.aliyuncs.com' == request.headers.get('host')
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
            assert content_type == 'application/x-www-form-urlencoded', 'expected application/json but received Content-Type: {}'.format(
                content_type)
            headers['x-acs-request-id'] = 'A45EE076-334D-5012-9746-A8F828D20FD4'
            if request.headers.get('type') == 'array':
                return [200, headers, '["AppId", "ClassId", "UserId"]']
            elif request.headers.get('type') == 'error':
                return [400, headers, '{"Code":"error code", "Message":"error message", '
                                      '"RequestId":"A45EE076-334D-5012-9746-A8F828D20FD4"}']
            return [200, headers, '{"AppId":"test", "ClassId":"test", "UserId":123}']

        httpretty.register_uri(
            httpretty.POST, "http://test.aliyuncs.com/test",
            body=request_callback)
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('test', result.get('body').get('AppId'))
        self.assertEqual('test', result.get('body').get('ClassId'))
        self.assertEqual(123, result.get('body').get('UserId'))

        params.body_type = 'array'
        request.headers.update({'type': 'array'})
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('AppId', result.get('body')[0])
        self.assertEqual('ClassId', result.get('body')[1])
        self.assertEqual('UserId', result.get('body')[2])

        params.body_type = 'string'
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('["AppId", "ClassId", "UserId"]', result.get('body'))

        params.body_type = 'byte'
        result = client.call_api(params, request, runtime)
        self.assertEqual('A45EE076-334D-5012-9746-A8F828D20FD4', result.get("headers").get("x-acs-request-id"))
        self.assertEqual('["AppId", "ClassId", "UserId"]', result.get('body').decode('utf-8'))

        request.headers.update({'type': 'error'})
        try:
            client.call_api(params, request, runtime)
        except TeaException as e:
            self.assertEqual('code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4', e.message)
            self.assertEqual('error code', e.code)
