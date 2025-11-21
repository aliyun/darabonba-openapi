# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from __future__ import annotations

from typing import Dict, Generator, AsyncGenerator, Any

from alibabacloud_credentials import models as credential_models
from alibabacloud_credentials.client import Client as CredentialClient
from alibabacloud_gateway_spi import models as spi_models
from alibabacloud_gateway_spi.client import Client as SPIClient
from alibabacloud_tea_openapi import exceptions as main_exceptions
from alibabacloud_tea_openapi import models as main_models
from alibabacloud_tea_openapi import utils_models as open_api_util_models
from alibabacloud_tea_openapi.utils import Utils
from darabonba.core import DaraCore as DaraCore
from darabonba.core import DaraCore
from darabonba.exceptions import DaraException, UnretryableException
from darabonba.policy.retry import RetryOptions, RetryPolicyContext
from darabonba.request import DaraRequest
from darabonba.runtime import RuntimeOptions
from darabonba.utils.bytes import Bytes as DaraBytes
from darabonba.utils.form import Form as DaraForm
from darabonba.utils.stream import Stream as DaraStream
from darabonba.utils.xml import XML as DaraXML

"""
 * @remarks
 * This is for OpenApi SDK
"""
class Client:
    _endpoint: str = None
    _region_id: str = None
    _protocol: str = None
    _method: str = None
    _user_agent: str = None
    _endpoint_rule: str = None
    _endpoint_map: Dict[str, str] = None
    _suffix: str = None
    _read_timeout: int = None
    _connect_timeout: int = None
    _http_proxy: str = None
    _https_proxy: str = None
    _socks_5proxy: str = None
    _socks_5net_work: str = None
    _no_proxy: str = None
    _network: str = None
    _product_id: str = None
    _max_idle_conns: int = None
    _endpoint_type: str = None
    _open_platform_endpoint: str = None
    _credential: CredentialClient = None
    _signature_version: str = None
    _signature_algorithm: str = None
    _headers: Dict[str, str] = None
    _spi: SPIClient = None
    _global_parameters: open_api_util_models.GlobalParameters = None
    _key: str = None
    _cert: str = None
    _ca: str = None
    _disable_http_2: bool = None
    _retry_options: RetryOptions = None
    _tls_min_version: str = None
    _attribute_map: spi_models.AttributeMap = None

    def __init__(
        self,
        config: open_api_util_models.Config,
    ):
        if DaraCore.is_null(config):
            raise main_exceptions.ClientException(
                code = 'ParameterMissing',
                message = '\'config\' can not be unset'
            )
        if (not DaraCore.is_null(config.access_key_id) and config.access_key_id != '') and (not DaraCore.is_null(config.access_key_secret) and config.access_key_secret != ''):
            if not DaraCore.is_null(config.security_token) and config.security_token != '':
                config.type = 'sts'
            else:
                config.type = 'access_key'

            credential_config = credential_models.Config(
                access_key_id = config.access_key_id,
                type = config.type,
                access_key_secret = config.access_key_secret
            )
            credential_config.security_token = config.security_token
            self._credential = CredentialClient(credential_config)
        elif not DaraCore.is_null(config.bearer_token) and config.bearer_token != '':
            cc = credential_models.Config(
                type = 'bearer',
                bearer_token = config.bearer_token
            )
            self._credential = CredentialClient(cc)
        elif not DaraCore.is_null(config.credential):
            self._credential = config.credential
        self._endpoint = config.endpoint
        self._endpoint_type = config.endpoint_type
        self._network = config.network
        self._suffix = config.suffix
        self._protocol = config.protocol
        self._method = config.method
        self._region_id = config.region_id
        self._user_agent = config.user_agent
        self._read_timeout = config.read_timeout
        self._connect_timeout = config.connect_timeout
        self._http_proxy = config.http_proxy
        self._https_proxy = config.https_proxy
        self._no_proxy = config.no_proxy
        self._socks_5proxy = config.socks_5proxy
        self._socks_5net_work = config.socks_5net_work
        self._max_idle_conns = config.max_idle_conns
        self._signature_version = config.signature_version
        self._signature_algorithm = config.signature_algorithm
        self._global_parameters = config.global_parameters
        self._key = config.key
        self._cert = config.cert
        self._ca = config.ca
        self._disable_http_2 = config.disable_http_2
        self._retry_options = config.retry_options
        self._tls_min_version = config.tls_min_version

    """
     * @remarks
     * Encapsulate the request and invoke the network
     * 
     * @param action - api name
     * @param version - product version
     * @param protocol - http or https
     * @param method - e.g. GET
     * @param authType - authorization type e.g. AK
     * @param bodyType - response body type e.g. String
     * @param request - object of OpenApiRequest
     * @param runtime - which controls some details of call api, such as retry times
     * @returns the response
    """
    def do_rpcrequest(
        self,
        action: str,
        version: str,
        protocol: str,
        method: str,
        auth_type: str,
        body_type: str,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> dict:
        _runtime = {
            'key': runtime.key or self._key,
            'cert': runtime.cert or self._cert,
            'ca': runtime.ca or self._ca,
            'readTimeout': DaraCore.to_number(runtime.read_timeout or self._read_timeout),
            'connectTimeout': DaraCore.to_number(runtime.connect_timeout or self._connect_timeout),
            'httpProxy': runtime.http_proxy or self._http_proxy,
            'httpsProxy': runtime.https_proxy or self._https_proxy,
            'noProxy': runtime.no_proxy or self._no_proxy,
            'socks5Proxy': runtime.socks_5proxy or self._socks_5proxy,
            'socks5NetWork': runtime.socks_5net_work or self._socks_5net_work,
            'maxIdleConns': DaraCore.to_number(runtime.max_idle_conns or self._max_idle_conns),
            'retryOptions': self._retry_options,
            'ignoreSSL': runtime.ignore_ssl,
            'tlsMinVersion': self._tls_min_version,
        }
        _last_request = None
        _last_response = None
        _retries_attempted = 0
        _context = RetryPolicyContext(
            retries_attempted= _retries_attempted
        )
        while DaraCore.should_retry(_runtime.get('retryOptions'), _context):
            if _retries_attempted > 0:
                _backoff_time = DaraCore.get_backoff_time(_runtime.get('retryOptions'), _context)
                if _backoff_time > 0:
                    DaraCore.sleep(_backoff_time)
            _retries_attempted = _retries_attempted + 1
            try:
                _request = DaraRequest()
                _request.protocol = self._protocol or protocol
                _request.method = method
                _request.pathname = '/'
                global_queries = {}
                global_headers = {}
                if not DaraCore.is_null(self._global_parameters):
                    global_params = self._global_parameters
                    if not DaraCore.is_null(global_params.queries):
                        global_queries = global_params.queries
                    if not DaraCore.is_null(global_params.headers):
                        global_headers = global_params.headers
                extends_headers = {}
                extends_queries = {}
                if not DaraCore.is_null(runtime.extends_parameters):
                    extends_parameters = runtime.extends_parameters
                    if not DaraCore.is_null(extends_parameters.headers):
                        extends_headers = extends_parameters.headers
                    if not DaraCore.is_null(extends_parameters.queries):
                        extends_queries = extends_parameters.queries
                _request.query = DaraCore.merge({
                    'Action': action,
                    'Format': 'json',
                    'Version': version,
                    'Timestamp': Utils.get_timestamp(),
                    'SignatureNonce': Utils.get_nonce(),
                }, global_queries, extends_queries, request.query)
                headers = self.get_rpc_headers()
                if DaraCore.is_null(headers):
                    # endpoint is setted in product client
                    _request.headers = DaraCore.merge({
                        'host': self._endpoint,
                        'x-acs-version': version,
                        'x-acs-action': action,
                        'user-agent': Utils.get_user_agent(self._user_agent),
                    }, global_headers, extends_headers, request.headers)
                else:
                    _request.headers = DaraCore.merge({
                        'host': self._endpoint,
                        'x-acs-version': version,
                        'x-acs-action': action,
                        'user-agent': Utils.get_user_agent(self._user_agent),
                    }, global_headers, extends_headers, request.headers, headers)

                if not DaraCore.is_null(request.body):
                    m = request.body
                    tmp = Utils.query(m)
                    _request.body = DaraForm.to_form_string(tmp)
                    _request.headers["content-type"] = 'application/x-www-form-urlencoded'
                if auth_type != 'Anonymous':
                    if DaraCore.is_null(self._credential):
                        raise main_exceptions.ClientException(
                            code = f'InvalidCredentials',
                            message = f'Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.'
                        )
                    credential_model = self._credential.get_credential()
                    if not DaraCore.is_null(credential_model.provider_name):
                        _request.headers["x-acs-credentials-provider"] = credential_model.provider_name
                    credential_type = credential_model.type
                    if credential_type == 'bearer':
                        bearer_token = credential_model.bearer_token
                        _request.query["BearerToken"] = bearer_token
                        _request.query["SignatureType"] = 'BEARERTOKEN'
                    elif credential_type == 'id_token':
                        id_token = credential_model.security_token
                        _request.headers["x-acs-zero-trust-idtoken"] = id_token
                    else:
                        access_key_id = credential_model.access_key_id
                        access_key_secret = credential_model.access_key_secret
                        security_token = credential_model.security_token
                        if not DaraCore.is_null(security_token) and security_token != '':
                            _request.query["SecurityToken"] = security_token
                        _request.query["SignatureMethod"] = 'HMAC-SHA1'
                        _request.query["SignatureVersion"] = '1.0'
                        _request.query["AccessKeyId"] = access_key_id
                        t = None
                        if not DaraCore.is_null(request.body):
                            t = request.body
                        signed_param = DaraCore.merge({}, _request.query, Utils.query(t))
                        _request.query["Signature"] = Utils.get_rpcsignature(signed_param, _request.method, access_key_secret)

                _last_request = _request
                _response = DaraCore.do_action(_request, _runtime)
                _last_response = _response
                if (_response.status_code >= 400) and (_response.status_code < 600):
                    _res = DaraStream.read_as_json(_response.body)
                    err = _res
                    request_id = err.get("RequestId") or err.get("requestId")
                    code = err.get("Code") or err.get("code")
                    if (f'{code}' == 'Throttling') or (f'{code}' == 'Throttling.User') or (f'{code}' == 'Throttling.Api'):
                        raise main_exceptions.ThrottlingException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            retry_after = Utils.get_throttling_time_left(_response.headers),
                            data = err,
                            request_id = f'{request_id}'
                        )
                    elif (_response.status_code >= 400) and (_response.status_code < 500):
                        raise main_exceptions.ClientException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            access_denied_detail = self.get_access_denied_detail(err),
                            request_id = f'{request_id}'
                        )
                    else:
                        raise main_exceptions.ServerException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            request_id = f'{request_id}'
                        )

                if body_type == 'binary':
                    resp = {
                        'body': _response.body,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                    return resp
                elif body_type == 'byte':
                    byt = DaraStream.read_as_bytes(_response.body)
                    return {
                        'body': byt,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'string':
                    _str = DaraStream.read_as_string(_response.body)
                    return {
                        'body': _str,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'json':
                    obj = DaraStream.read_as_json(_response.body)
                    res = obj
                    return {
                        'body': res,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'array':
                    arr = DaraStream.read_as_json(_response.body)
                    return {
                        'body': arr,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                else:
                    return {
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }

            except Exception as e:
                _context = RetryPolicyContext(
                    retries_attempted= _retries_attempted,
                    http_request = _last_request,
                    http_response = _last_response,
                    exception = e
                )
                continue
        raise UnretryableException(_context)

    async def do_rpcrequest_async(
        self,
        action: str,
        version: str,
        protocol: str,
        method: str,
        auth_type: str,
        body_type: str,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> dict:
        _runtime = {
            'key': runtime.key or self._key,
            'cert': runtime.cert or self._cert,
            'ca': runtime.ca or self._ca,
            'readTimeout': DaraCore.to_number(runtime.read_timeout or self._read_timeout),
            'connectTimeout': DaraCore.to_number(runtime.connect_timeout or self._connect_timeout),
            'httpProxy': runtime.http_proxy or self._http_proxy,
            'httpsProxy': runtime.https_proxy or self._https_proxy,
            'noProxy': runtime.no_proxy or self._no_proxy,
            'socks5Proxy': runtime.socks_5proxy or self._socks_5proxy,
            'socks5NetWork': runtime.socks_5net_work or self._socks_5net_work,
            'maxIdleConns': DaraCore.to_number(runtime.max_idle_conns or self._max_idle_conns),
            'retryOptions': self._retry_options,
            'ignoreSSL': runtime.ignore_ssl,
            'tlsMinVersion': self._tls_min_version,
        }
        _last_request = None
        _last_response = None
        _retries_attempted = 0
        _context = RetryPolicyContext(
            retries_attempted= _retries_attempted
        )
        while DaraCore.should_retry(_runtime.get('retryOptions'), _context):
            if _retries_attempted > 0:
                _backoff_time = DaraCore.get_backoff_time(_runtime.get('retryOptions'), _context)
                if _backoff_time > 0:
                    DaraCore.sleep(_backoff_time)
            _retries_attempted = _retries_attempted + 1
            try:
                _request = DaraRequest()
                _request.protocol = self._protocol or protocol
                _request.method = method
                _request.pathname = '/'
                global_queries = {}
                global_headers = {}
                if not DaraCore.is_null(self._global_parameters):
                    global_params = self._global_parameters
                    if not DaraCore.is_null(global_params.queries):
                        global_queries = global_params.queries
                    if not DaraCore.is_null(global_params.headers):
                        global_headers = global_params.headers
                extends_headers = {}
                extends_queries = {}
                if not DaraCore.is_null(runtime.extends_parameters):
                    extends_parameters = runtime.extends_parameters
                    if not DaraCore.is_null(extends_parameters.headers):
                        extends_headers = extends_parameters.headers
                    if not DaraCore.is_null(extends_parameters.queries):
                        extends_queries = extends_parameters.queries
                _request.query = DaraCore.merge({
                    'Action': action,
                    'Format': 'json',
                    'Version': version,
                    'Timestamp': Utils.get_timestamp(),
                    'SignatureNonce': Utils.get_nonce(),
                }, global_queries, extends_queries, request.query)
                headers = self.get_rpc_headers()
                if DaraCore.is_null(headers):
                    # endpoint is setted in product client
                    _request.headers = DaraCore.merge({
                        'host': self._endpoint,
                        'x-acs-version': version,
                        'x-acs-action': action,
                        'user-agent': Utils.get_user_agent(self._user_agent),
                    }, global_headers, extends_headers, request.headers)
                else:
                    _request.headers = DaraCore.merge({
                        'host': self._endpoint,
                        'x-acs-version': version,
                        'x-acs-action': action,
                        'user-agent': Utils.get_user_agent(self._user_agent),
                    }, global_headers, extends_headers, request.headers, headers)

                if not DaraCore.is_null(request.body):
                    m = request.body
                    tmp = Utils.query(m)
                    _request.body = DaraForm.to_form_string(tmp)
                    _request.headers["content-type"] = 'application/x-www-form-urlencoded'
                if auth_type != 'Anonymous':
                    if DaraCore.is_null(self._credential):
                        raise main_exceptions.ClientException(
                            code = f'InvalidCredentials',
                            message = f'Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.'
                        )
                    credential_model = await self._credential.get_credential_async()
                    if not DaraCore.is_null(credential_model.provider_name):
                        _request.headers["x-acs-credentials-provider"] = credential_model.provider_name
                    credential_type = credential_model.type
                    if credential_type == 'bearer':
                        bearer_token = credential_model.bearer_token
                        _request.query["BearerToken"] = bearer_token
                        _request.query["SignatureType"] = 'BEARERTOKEN'
                    elif credential_type == 'id_token':
                        id_token = credential_model.security_token
                        _request.headers["x-acs-zero-trust-idtoken"] = id_token
                    else:
                        access_key_id = credential_model.access_key_id
                        access_key_secret = credential_model.access_key_secret
                        security_token = credential_model.security_token
                        if not DaraCore.is_null(security_token) and security_token != '':
                            _request.query["SecurityToken"] = security_token
                        _request.query["SignatureMethod"] = 'HMAC-SHA1'
                        _request.query["SignatureVersion"] = '1.0'
                        _request.query["AccessKeyId"] = access_key_id
                        t = None
                        if not DaraCore.is_null(request.body):
                            t = request.body
                        signed_param = DaraCore.merge({}, _request.query, Utils.query(t))
                        _request.query["Signature"] = Utils.get_rpcsignature(signed_param, _request.method, access_key_secret)

                _last_request = _request
                _response = await DaraCore.async_do_action(_request, _runtime)
                _last_response = _response
                if (_response.status_code >= 400) and (_response.status_code < 600):
                    _res = await DaraStream.read_as_json_async(_response.body)
                    err = _res
                    request_id = err.get("RequestId") or err.get("requestId")
                    code = err.get("Code") or err.get("code")
                    if (f'{code}' == 'Throttling') or (f'{code}' == 'Throttling.User') or (f'{code}' == 'Throttling.Api'):
                        raise main_exceptions.ThrottlingException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            retry_after = Utils.get_throttling_time_left(_response.headers),
                            data = err,
                            request_id = f'{request_id}'
                        )
                    elif (_response.status_code >= 400) and (_response.status_code < 500):
                        raise main_exceptions.ClientException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            access_denied_detail = self.get_access_denied_detail(err),
                            request_id = f'{request_id}'
                        )
                    else:
                        raise main_exceptions.ServerException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            request_id = f'{request_id}'
                        )

                if body_type == 'binary':
                    resp = {
                        'body': _response.body,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                    return resp
                elif body_type == 'byte':
                    byt = await DaraStream.read_as_bytes_async(_response.body)
                    return {
                        'body': byt,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'string':
                    _str = await DaraStream.read_as_string_async(_response.body)
                    return {
                        'body': _str,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'json':
                    obj = await DaraStream.read_as_json_async(_response.body)
                    res = obj
                    return {
                        'body': res,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'array':
                    arr = await DaraStream.read_as_json_async(_response.body)
                    return {
                        'body': arr,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                else:
                    return {
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }

            except Exception as e:
                _context = RetryPolicyContext(
                    retries_attempted= _retries_attempted,
                    http_request = _last_request,
                    http_response = _last_response,
                    exception = e
                )
                continue
        raise UnretryableException(_context)

    """
     * @remarks
     * Encapsulate the request and invoke the network
     * 
     * @param action - api name
     * @param version - product version
     * @param protocol - http or https
     * @param method - e.g. GET
     * @param authType - authorization type e.g. AK
     * @param pathname - pathname of every api
     * @param bodyType - response body type e.g. String
     * @param request - object of OpenApiRequest
     * @param runtime - which controls some details of call api, such as retry times
     * @returns the response
    """
    def do_roarequest(
        self,
        action: str,
        version: str,
        protocol: str,
        method: str,
        auth_type: str,
        pathname: str,
        body_type: str,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> dict:
        _runtime = {
            'key': runtime.key or self._key,
            'cert': runtime.cert or self._cert,
            'ca': runtime.ca or self._ca,
            'readTimeout': DaraCore.to_number(runtime.read_timeout or self._read_timeout),
            'connectTimeout': DaraCore.to_number(runtime.connect_timeout or self._connect_timeout),
            'httpProxy': runtime.http_proxy or self._http_proxy,
            'httpsProxy': runtime.https_proxy or self._https_proxy,
            'noProxy': runtime.no_proxy or self._no_proxy,
            'socks5Proxy': runtime.socks_5proxy or self._socks_5proxy,
            'socks5NetWork': runtime.socks_5net_work or self._socks_5net_work,
            'maxIdleConns': DaraCore.to_number(runtime.max_idle_conns or self._max_idle_conns),
            'retryOptions': self._retry_options,
            'ignoreSSL': runtime.ignore_ssl,
            'tlsMinVersion': self._tls_min_version,
        }
        _last_request = None
        _last_response = None
        _retries_attempted = 0
        _context = RetryPolicyContext(
            retries_attempted= _retries_attempted
        )
        while DaraCore.should_retry(_runtime.get('retryOptions'), _context):
            if _retries_attempted > 0:
                _backoff_time = DaraCore.get_backoff_time(_runtime.get('retryOptions'), _context)
                if _backoff_time > 0:
                    DaraCore.sleep(_backoff_time)
            _retries_attempted = _retries_attempted + 1
            try:
                _request = DaraRequest()
                _request.protocol = self._protocol or protocol
                _request.method = method
                _request.pathname = pathname
                global_queries = {}
                global_headers = {}
                if not DaraCore.is_null(self._global_parameters):
                    global_params = self._global_parameters
                    if not DaraCore.is_null(global_params.queries):
                        global_queries = global_params.queries
                    if not DaraCore.is_null(global_params.headers):
                        global_headers = global_params.headers
                extends_headers = {}
                extends_queries = {}
                if not DaraCore.is_null(runtime.extends_parameters):
                    extends_parameters = runtime.extends_parameters
                    if not DaraCore.is_null(extends_parameters.headers):
                        extends_headers = extends_parameters.headers
                    if not DaraCore.is_null(extends_parameters.queries):
                        extends_queries = extends_parameters.queries
                _request.headers = DaraCore.merge({
                    'date': Utils.get_date_utcstring(),
                    'host': self._endpoint,
                    'accept': 'application/json',
                    'x-acs-signature-nonce': Utils.get_nonce(),
                    'x-acs-signature-method': 'HMAC-SHA1',
                    'x-acs-signature-version': '1.0',
                    'x-acs-version': version,
                    'x-acs-action': action,
                    'user-agent': Utils.get_user_agent(self._user_agent),
                }, global_headers, extends_headers, request.headers)
                if not DaraCore.is_null(request.body):
                    _request.body = DaraCore.to_json_string(request.body)
                    _request.headers["content-type"] = 'application/json; charset=utf-8'
                _request.query = DaraCore.merge({}, global_queries, extends_queries)
                if not DaraCore.is_null(request.query):
                    _request.query = DaraCore.merge({}, _request.query, request.query)
                if auth_type != 'Anonymous':
                    if DaraCore.is_null(self._credential):
                        raise main_exceptions.ClientException(
                            code = f'InvalidCredentials',
                            message = f'Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.'
                        )
                    credential_model = self._credential.get_credential()
                    if not DaraCore.is_null(credential_model.provider_name):
                        _request.headers["x-acs-credentials-provider"] = credential_model.provider_name
                    credential_type = credential_model.type
                    if credential_type == 'bearer':
                        bearer_token = credential_model.bearer_token
                        _request.headers["x-acs-bearer-token"] = bearer_token
                        _request.headers["x-acs-signature-type"] = 'BEARERTOKEN'
                    elif credential_type == 'id_token':
                        id_token = credential_model.security_token
                        _request.headers["x-acs-zero-trust-idtoken"] = id_token
                    else:
                        access_key_id = credential_model.access_key_id
                        access_key_secret = credential_model.access_key_secret
                        security_token = credential_model.security_token
                        if not DaraCore.is_null(security_token) and security_token != '':
                            _request.headers["x-acs-accesskey-id"] = access_key_id
                            _request.headers["x-acs-security-token"] = security_token
                        string_to_sign = Utils.get_string_to_sign(_request)
                        _request.headers["authorization"] = f'acs {access_key_id}:{Utils.get_roasignature(string_to_sign, access_key_secret)}'

                _last_request = _request
                _response = DaraCore.do_action(_request, _runtime)
                _last_response = _response
                if _response.status_code == 204:
                    return {
                        'headers': _response.headers
                    }
                if (_response.status_code >= 400) and (_response.status_code < 600):
                    _res = DaraStream.read_as_json(_response.body)
                    err = _res
                    request_id = err.get("RequestId") or err.get("requestId")
                    request_id = request_id or err.get("requestid")
                    code = err.get("Code") or err.get("code")
                    if (f'{code}' == 'Throttling') or (f'{code}' == 'Throttling.User') or (f'{code}' == 'Throttling.Api'):
                        raise main_exceptions.ThrottlingException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            retry_after = Utils.get_throttling_time_left(_response.headers),
                            data = err,
                            request_id = f'{request_id}'
                        )
                    elif (_response.status_code >= 400) and (_response.status_code < 500):
                        raise main_exceptions.ClientException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            access_denied_detail = self.get_access_denied_detail(err),
                            request_id = f'{request_id}'
                        )
                    else:
                        raise main_exceptions.ServerException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            request_id = f'{request_id}'
                        )

                if body_type == 'binary':
                    resp = {
                        'body': _response.body,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                    return resp
                elif body_type == 'byte':
                    byt = DaraStream.read_as_bytes(_response.body)
                    return {
                        'body': byt,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'string':
                    _str = DaraStream.read_as_string(_response.body)
                    return {
                        'body': _str,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'json':
                    obj = DaraStream.read_as_json(_response.body)
                    res = obj
                    return {
                        'body': res,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'array':
                    arr = DaraStream.read_as_json(_response.body)
                    return {
                        'body': arr,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                else:
                    return {
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }

            except Exception as e:
                _context = RetryPolicyContext(
                    retries_attempted= _retries_attempted,
                    http_request = _last_request,
                    http_response = _last_response,
                    exception = e
                )
                continue
        raise UnretryableException(_context)

    async def do_roarequest_async(
        self,
        action: str,
        version: str,
        protocol: str,
        method: str,
        auth_type: str,
        pathname: str,
        body_type: str,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> dict:
        _runtime = {
            'key': runtime.key or self._key,
            'cert': runtime.cert or self._cert,
            'ca': runtime.ca or self._ca,
            'readTimeout': DaraCore.to_number(runtime.read_timeout or self._read_timeout),
            'connectTimeout': DaraCore.to_number(runtime.connect_timeout or self._connect_timeout),
            'httpProxy': runtime.http_proxy or self._http_proxy,
            'httpsProxy': runtime.https_proxy or self._https_proxy,
            'noProxy': runtime.no_proxy or self._no_proxy,
            'socks5Proxy': runtime.socks_5proxy or self._socks_5proxy,
            'socks5NetWork': runtime.socks_5net_work or self._socks_5net_work,
            'maxIdleConns': DaraCore.to_number(runtime.max_idle_conns or self._max_idle_conns),
            'retryOptions': self._retry_options,
            'ignoreSSL': runtime.ignore_ssl,
            'tlsMinVersion': self._tls_min_version,
        }
        _last_request = None
        _last_response = None
        _retries_attempted = 0
        _context = RetryPolicyContext(
            retries_attempted= _retries_attempted
        )
        while DaraCore.should_retry(_runtime.get('retryOptions'), _context):
            if _retries_attempted > 0:
                _backoff_time = DaraCore.get_backoff_time(_runtime.get('retryOptions'), _context)
                if _backoff_time > 0:
                    DaraCore.sleep(_backoff_time)
            _retries_attempted = _retries_attempted + 1
            try:
                _request = DaraRequest()
                _request.protocol = self._protocol or protocol
                _request.method = method
                _request.pathname = pathname
                global_queries = {}
                global_headers = {}
                if not DaraCore.is_null(self._global_parameters):
                    global_params = self._global_parameters
                    if not DaraCore.is_null(global_params.queries):
                        global_queries = global_params.queries
                    if not DaraCore.is_null(global_params.headers):
                        global_headers = global_params.headers
                extends_headers = {}
                extends_queries = {}
                if not DaraCore.is_null(runtime.extends_parameters):
                    extends_parameters = runtime.extends_parameters
                    if not DaraCore.is_null(extends_parameters.headers):
                        extends_headers = extends_parameters.headers
                    if not DaraCore.is_null(extends_parameters.queries):
                        extends_queries = extends_parameters.queries
                _request.headers = DaraCore.merge({
                    'date': Utils.get_date_utcstring(),
                    'host': self._endpoint,
                    'accept': 'application/json',
                    'x-acs-signature-nonce': Utils.get_nonce(),
                    'x-acs-signature-method': 'HMAC-SHA1',
                    'x-acs-signature-version': '1.0',
                    'x-acs-version': version,
                    'x-acs-action': action,
                    'user-agent': Utils.get_user_agent(self._user_agent),
                }, global_headers, extends_headers, request.headers)
                if not DaraCore.is_null(request.body):
                    _request.body = DaraCore.to_json_string(request.body)
                    _request.headers["content-type"] = 'application/json; charset=utf-8'
                _request.query = DaraCore.merge({}, global_queries, extends_queries)
                if not DaraCore.is_null(request.query):
                    _request.query = DaraCore.merge({}, _request.query, request.query)
                if auth_type != 'Anonymous':
                    if DaraCore.is_null(self._credential):
                        raise main_exceptions.ClientException(
                            code = f'InvalidCredentials',
                            message = f'Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.'
                        )
                    credential_model = await self._credential.get_credential_async()
                    if not DaraCore.is_null(credential_model.provider_name):
                        _request.headers["x-acs-credentials-provider"] = credential_model.provider_name
                    credential_type = credential_model.type
                    if credential_type == 'bearer':
                        bearer_token = credential_model.bearer_token
                        _request.headers["x-acs-bearer-token"] = bearer_token
                        _request.headers["x-acs-signature-type"] = 'BEARERTOKEN'
                    elif credential_type == 'id_token':
                        id_token = credential_model.security_token
                        _request.headers["x-acs-zero-trust-idtoken"] = id_token
                    else:
                        access_key_id = credential_model.access_key_id
                        access_key_secret = credential_model.access_key_secret
                        security_token = credential_model.security_token
                        if not DaraCore.is_null(security_token) and security_token != '':
                            _request.headers["x-acs-accesskey-id"] = access_key_id
                            _request.headers["x-acs-security-token"] = security_token
                        string_to_sign = Utils.get_string_to_sign(_request)
                        _request.headers["authorization"] = f'acs {access_key_id}:{Utils.get_roasignature(string_to_sign, access_key_secret)}'

                _last_request = _request
                _response = await DaraCore.async_do_action(_request, _runtime)
                _last_response = _response
                if _response.status_code == 204:
                    return {
                        'headers': _response.headers
                    }
                if (_response.status_code >= 400) and (_response.status_code < 600):
                    _res = await DaraStream.read_as_json_async(_response.body)
                    err = _res
                    request_id = err.get("RequestId") or err.get("requestId")
                    request_id = request_id or err.get("requestid")
                    code = err.get("Code") or err.get("code")
                    if (f'{code}' == 'Throttling') or (f'{code}' == 'Throttling.User') or (f'{code}' == 'Throttling.Api'):
                        raise main_exceptions.ThrottlingException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            retry_after = Utils.get_throttling_time_left(_response.headers),
                            data = err,
                            request_id = f'{request_id}'
                        )
                    elif (_response.status_code >= 400) and (_response.status_code < 500):
                        raise main_exceptions.ClientException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            access_denied_detail = self.get_access_denied_detail(err),
                            request_id = f'{request_id}'
                        )
                    else:
                        raise main_exceptions.ServerException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            request_id = f'{request_id}'
                        )

                if body_type == 'binary':
                    resp = {
                        'body': _response.body,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                    return resp
                elif body_type == 'byte':
                    byt = await DaraStream.read_as_bytes_async(_response.body)
                    return {
                        'body': byt,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'string':
                    _str = await DaraStream.read_as_string_async(_response.body)
                    return {
                        'body': _str,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'json':
                    obj = await DaraStream.read_as_json_async(_response.body)
                    res = obj
                    return {
                        'body': res,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'array':
                    arr = await DaraStream.read_as_json_async(_response.body)
                    return {
                        'body': arr,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                else:
                    return {
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }

            except Exception as e:
                _context = RetryPolicyContext(
                    retries_attempted= _retries_attempted,
                    http_request = _last_request,
                    http_response = _last_response,
                    exception = e
                )
                continue
        raise UnretryableException(_context)

    """
     * @remarks
     * Encapsulate the request and invoke the network with form body
     * 
     * @param action - api name
     * @param version - product version
     * @param protocol - http or https
     * @param method - e.g. GET
     * @param authType - authorization type e.g. AK
     * @param pathname - pathname of every api
     * @param bodyType - response body type e.g. String
     * @param request - object of OpenApiRequest
     * @param runtime - which controls some details of call api, such as retry times
     * @returns the response
    """
    def do_roarequest_with_form(
        self,
        action: str,
        version: str,
        protocol: str,
        method: str,
        auth_type: str,
        pathname: str,
        body_type: str,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> dict:
        _runtime = {
            'key': runtime.key or self._key,
            'cert': runtime.cert or self._cert,
            'ca': runtime.ca or self._ca,
            'readTimeout': DaraCore.to_number(runtime.read_timeout or self._read_timeout),
            'connectTimeout': DaraCore.to_number(runtime.connect_timeout or self._connect_timeout),
            'httpProxy': runtime.http_proxy or self._http_proxy,
            'httpsProxy': runtime.https_proxy or self._https_proxy,
            'noProxy': runtime.no_proxy or self._no_proxy,
            'socks5Proxy': runtime.socks_5proxy or self._socks_5proxy,
            'socks5NetWork': runtime.socks_5net_work or self._socks_5net_work,
            'maxIdleConns': DaraCore.to_number(runtime.max_idle_conns or self._max_idle_conns),
            'retryOptions': self._retry_options,
            'ignoreSSL': runtime.ignore_ssl,
            'tlsMinVersion': self._tls_min_version,
        }
        _last_request = None
        _last_response = None
        _retries_attempted = 0
        _context = RetryPolicyContext(
            retries_attempted= _retries_attempted
        )
        while DaraCore.should_retry(_runtime.get('retryOptions'), _context):
            if _retries_attempted > 0:
                _backoff_time = DaraCore.get_backoff_time(_runtime.get('retryOptions'), _context)
                if _backoff_time > 0:
                    DaraCore.sleep(_backoff_time)
            _retries_attempted = _retries_attempted + 1
            try:
                _request = DaraRequest()
                _request.protocol = self._protocol or protocol
                _request.method = method
                _request.pathname = pathname
                global_queries = {}
                global_headers = {}
                if not DaraCore.is_null(self._global_parameters):
                    global_params = self._global_parameters
                    if not DaraCore.is_null(global_params.queries):
                        global_queries = global_params.queries
                    if not DaraCore.is_null(global_params.headers):
                        global_headers = global_params.headers
                extends_headers = {}
                extends_queries = {}
                if not DaraCore.is_null(runtime.extends_parameters):
                    extends_parameters = runtime.extends_parameters
                    if not DaraCore.is_null(extends_parameters.headers):
                        extends_headers = extends_parameters.headers
                    if not DaraCore.is_null(extends_parameters.queries):
                        extends_queries = extends_parameters.queries
                _request.headers = DaraCore.merge({
                    'date': Utils.get_date_utcstring(),
                    'host': self._endpoint,
                    'accept': 'application/json',
                    'x-acs-signature-nonce': Utils.get_nonce(),
                    'x-acs-signature-method': 'HMAC-SHA1',
                    'x-acs-signature-version': '1.0',
                    'x-acs-version': version,
                    'x-acs-action': action,
                    'user-agent': Utils.get_user_agent(self._user_agent),
                }, global_headers, extends_headers, request.headers)
                if not DaraCore.is_null(request.body):
                    m = request.body
                    _request.body = Utils.to_form(m)
                    _request.headers["content-type"] = 'application/x-www-form-urlencoded'
                _request.query = DaraCore.merge({}, global_queries, extends_queries)
                if not DaraCore.is_null(request.query):
                    _request.query = DaraCore.merge({}, _request.query, request.query)
                if auth_type != 'Anonymous':
                    if DaraCore.is_null(self._credential):
                        raise main_exceptions.ClientException(
                            code = f'InvalidCredentials',
                            message = f'Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.'
                        )
                    credential_model = self._credential.get_credential()
                    if not DaraCore.is_null(credential_model.provider_name):
                        _request.headers["x-acs-credentials-provider"] = credential_model.provider_name
                    credential_type = credential_model.type
                    if credential_type == 'bearer':
                        bearer_token = credential_model.bearer_token
                        _request.headers["x-acs-bearer-token"] = bearer_token
                        _request.headers["x-acs-signature-type"] = 'BEARERTOKEN'
                    elif credential_type == 'id_token':
                        id_token = credential_model.security_token
                        _request.headers["x-acs-zero-trust-idtoken"] = id_token
                    else:
                        access_key_id = credential_model.access_key_id
                        access_key_secret = credential_model.access_key_secret
                        security_token = credential_model.security_token
                        if not DaraCore.is_null(security_token) and security_token != '':
                            _request.headers["x-acs-accesskey-id"] = access_key_id
                            _request.headers["x-acs-security-token"] = security_token
                        string_to_sign = Utils.get_string_to_sign(_request)
                        _request.headers["authorization"] = f'acs {access_key_id}:{Utils.get_roasignature(string_to_sign, access_key_secret)}'

                _last_request = _request
                _response = DaraCore.do_action(_request, _runtime)
                _last_response = _response
                if _response.status_code == 204:
                    return {
                        'headers': _response.headers
                    }
                if (_response.status_code >= 400) and (_response.status_code < 600):
                    _res = DaraStream.read_as_json(_response.body)
                    err = _res
                    request_id = err.get("RequestId") or err.get("requestId")
                    code = err.get("Code") or err.get("code")
                    if (f'{code}' == 'Throttling') or (f'{code}' == 'Throttling.User') or (f'{code}' == 'Throttling.Api'):
                        raise main_exceptions.ThrottlingException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            retry_after = Utils.get_throttling_time_left(_response.headers),
                            data = err,
                            request_id = f'{request_id}'
                        )
                    elif (_response.status_code >= 400) and (_response.status_code < 500):
                        raise main_exceptions.ClientException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            access_denied_detail = self.get_access_denied_detail(err),
                            request_id = f'{request_id}'
                        )
                    else:
                        raise main_exceptions.ServerException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            request_id = f'{request_id}'
                        )

                if body_type == 'binary':
                    resp = {
                        'body': _response.body,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                    return resp
                elif body_type == 'byte':
                    byt = DaraStream.read_as_bytes(_response.body)
                    return {
                        'body': byt,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'string':
                    _str = DaraStream.read_as_string(_response.body)
                    return {
                        'body': _str,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'json':
                    obj = DaraStream.read_as_json(_response.body)
                    res = obj
                    return {
                        'body': res,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'array':
                    arr = DaraStream.read_as_json(_response.body)
                    return {
                        'body': arr,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                else:
                    return {
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }

            except Exception as e:
                _context = RetryPolicyContext(
                    retries_attempted= _retries_attempted,
                    http_request = _last_request,
                    http_response = _last_response,
                    exception = e
                )
                continue
        raise UnretryableException(_context)

    async def do_roarequest_with_form_async(
        self,
        action: str,
        version: str,
        protocol: str,
        method: str,
        auth_type: str,
        pathname: str,
        body_type: str,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> dict:
        _runtime = {
            'key': runtime.key or self._key,
            'cert': runtime.cert or self._cert,
            'ca': runtime.ca or self._ca,
            'readTimeout': DaraCore.to_number(runtime.read_timeout or self._read_timeout),
            'connectTimeout': DaraCore.to_number(runtime.connect_timeout or self._connect_timeout),
            'httpProxy': runtime.http_proxy or self._http_proxy,
            'httpsProxy': runtime.https_proxy or self._https_proxy,
            'noProxy': runtime.no_proxy or self._no_proxy,
            'socks5Proxy': runtime.socks_5proxy or self._socks_5proxy,
            'socks5NetWork': runtime.socks_5net_work or self._socks_5net_work,
            'maxIdleConns': DaraCore.to_number(runtime.max_idle_conns or self._max_idle_conns),
            'retryOptions': self._retry_options,
            'ignoreSSL': runtime.ignore_ssl,
            'tlsMinVersion': self._tls_min_version,
        }
        _last_request = None
        _last_response = None
        _retries_attempted = 0
        _context = RetryPolicyContext(
            retries_attempted= _retries_attempted
        )
        while DaraCore.should_retry(_runtime.get('retryOptions'), _context):
            if _retries_attempted > 0:
                _backoff_time = DaraCore.get_backoff_time(_runtime.get('retryOptions'), _context)
                if _backoff_time > 0:
                    DaraCore.sleep(_backoff_time)
            _retries_attempted = _retries_attempted + 1
            try:
                _request = DaraRequest()
                _request.protocol = self._protocol or protocol
                _request.method = method
                _request.pathname = pathname
                global_queries = {}
                global_headers = {}
                if not DaraCore.is_null(self._global_parameters):
                    global_params = self._global_parameters
                    if not DaraCore.is_null(global_params.queries):
                        global_queries = global_params.queries
                    if not DaraCore.is_null(global_params.headers):
                        global_headers = global_params.headers
                extends_headers = {}
                extends_queries = {}
                if not DaraCore.is_null(runtime.extends_parameters):
                    extends_parameters = runtime.extends_parameters
                    if not DaraCore.is_null(extends_parameters.headers):
                        extends_headers = extends_parameters.headers
                    if not DaraCore.is_null(extends_parameters.queries):
                        extends_queries = extends_parameters.queries
                _request.headers = DaraCore.merge({
                    'date': Utils.get_date_utcstring(),
                    'host': self._endpoint,
                    'accept': 'application/json',
                    'x-acs-signature-nonce': Utils.get_nonce(),
                    'x-acs-signature-method': 'HMAC-SHA1',
                    'x-acs-signature-version': '1.0',
                    'x-acs-version': version,
                    'x-acs-action': action,
                    'user-agent': Utils.get_user_agent(self._user_agent),
                }, global_headers, extends_headers, request.headers)
                if not DaraCore.is_null(request.body):
                    m = request.body
                    _request.body = Utils.to_form(m)
                    _request.headers["content-type"] = 'application/x-www-form-urlencoded'
                _request.query = DaraCore.merge({}, global_queries, extends_queries)
                if not DaraCore.is_null(request.query):
                    _request.query = DaraCore.merge({}, _request.query, request.query)
                if auth_type != 'Anonymous':
                    if DaraCore.is_null(self._credential):
                        raise main_exceptions.ClientException(
                            code = f'InvalidCredentials',
                            message = f'Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.'
                        )
                    credential_model = await self._credential.get_credential_async()
                    if not DaraCore.is_null(credential_model.provider_name):
                        _request.headers["x-acs-credentials-provider"] = credential_model.provider_name
                    credential_type = credential_model.type
                    if credential_type == 'bearer':
                        bearer_token = credential_model.bearer_token
                        _request.headers["x-acs-bearer-token"] = bearer_token
                        _request.headers["x-acs-signature-type"] = 'BEARERTOKEN'
                    elif credential_type == 'id_token':
                        id_token = credential_model.security_token
                        _request.headers["x-acs-zero-trust-idtoken"] = id_token
                    else:
                        access_key_id = credential_model.access_key_id
                        access_key_secret = credential_model.access_key_secret
                        security_token = credential_model.security_token
                        if not DaraCore.is_null(security_token) and security_token != '':
                            _request.headers["x-acs-accesskey-id"] = access_key_id
                            _request.headers["x-acs-security-token"] = security_token
                        string_to_sign = Utils.get_string_to_sign(_request)
                        _request.headers["authorization"] = f'acs {access_key_id}:{Utils.get_roasignature(string_to_sign, access_key_secret)}'

                _last_request = _request
                _response = await DaraCore.async_do_action(_request, _runtime)
                _last_response = _response
                if _response.status_code == 204:
                    return {
                        'headers': _response.headers
                    }
                if (_response.status_code >= 400) and (_response.status_code < 600):
                    _res = await DaraStream.read_as_json_async(_response.body)
                    err = _res
                    request_id = err.get("RequestId") or err.get("requestId")
                    code = err.get("Code") or err.get("code")
                    if (f'{code}' == 'Throttling') or (f'{code}' == 'Throttling.User') or (f'{code}' == 'Throttling.Api'):
                        raise main_exceptions.ThrottlingException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            retry_after = Utils.get_throttling_time_left(_response.headers),
                            data = err,
                            request_id = f'{request_id}'
                        )
                    elif (_response.status_code >= 400) and (_response.status_code < 500):
                        raise main_exceptions.ClientException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            access_denied_detail = self.get_access_denied_detail(err),
                            request_id = f'{request_id}'
                        )
                    else:
                        raise main_exceptions.ServerException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            request_id = f'{request_id}'
                        )

                if body_type == 'binary':
                    resp = {
                        'body': _response.body,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                    return resp
                elif body_type == 'byte':
                    byt = await DaraStream.read_as_bytes_async(_response.body)
                    return {
                        'body': byt,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'string':
                    _str = await DaraStream.read_as_string_async(_response.body)
                    return {
                        'body': _str,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'json':
                    obj = await DaraStream.read_as_json_async(_response.body)
                    res = obj
                    return {
                        'body': res,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif body_type == 'array':
                    arr = await DaraStream.read_as_json_async(_response.body)
                    return {
                        'body': arr,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                else:
                    return {
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }

            except Exception as e:
                _context = RetryPolicyContext(
                    retries_attempted= _retries_attempted,
                    http_request = _last_request,
                    http_response = _last_response,
                    exception = e
                )
                continue
        raise UnretryableException(_context)

    """
     * @remarks
     * Encapsulate the request and invoke the network
     * 
     * @param action - api name
     * @param version - product version
     * @param protocol - http or https
     * @param method - e.g. GET
     * @param authType - authorization type e.g. AK
     * @param bodyType - response body type e.g. String
     * @param request - object of OpenApiRequest
     * @param runtime - which controls some details of call api, such as retry times
     * @returns the response
    """
    def do_request(
        self,
        params: open_api_util_models.Params,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> dict:
        _runtime = {
            'key': runtime.key or self._key,
            'cert': runtime.cert or self._cert,
            'ca': runtime.ca or self._ca,
            'readTimeout': DaraCore.to_number(runtime.read_timeout or self._read_timeout),
            'connectTimeout': DaraCore.to_number(runtime.connect_timeout or self._connect_timeout),
            'httpProxy': runtime.http_proxy or self._http_proxy,
            'httpsProxy': runtime.https_proxy or self._https_proxy,
            'noProxy': runtime.no_proxy or self._no_proxy,
            'socks5Proxy': runtime.socks_5proxy or self._socks_5proxy,
            'socks5NetWork': runtime.socks_5net_work or self._socks_5net_work,
            'maxIdleConns': DaraCore.to_number(runtime.max_idle_conns or self._max_idle_conns),
            'retryOptions': self._retry_options,
            'ignoreSSL': runtime.ignore_ssl,
            'tlsMinVersion': self._tls_min_version,
        }
        _last_request = None
        _last_response = None
        _retries_attempted = 0
        _context = RetryPolicyContext(
            retries_attempted= _retries_attempted
        )
        while DaraCore.should_retry(_runtime.get('retryOptions'), _context):
            if _retries_attempted > 0:
                _backoff_time = DaraCore.get_backoff_time(_runtime.get('retryOptions'), _context)
                if _backoff_time > 0:
                    DaraCore.sleep(_backoff_time)
            _retries_attempted = _retries_attempted + 1
            try:
                _request = DaraRequest()
                _request.protocol = self._protocol or params.protocol
                _request.method = params.method
                _request.pathname = params.pathname
                global_queries = {}
                global_headers = {}
                if not DaraCore.is_null(self._global_parameters):
                    global_params = self._global_parameters
                    if not DaraCore.is_null(global_params.queries):
                        global_queries = global_params.queries
                    if not DaraCore.is_null(global_params.headers):
                        global_headers = global_params.headers
                extends_headers = {}
                extends_queries = {}
                if not DaraCore.is_null(runtime.extends_parameters):
                    extends_parameters = runtime.extends_parameters
                    if not DaraCore.is_null(extends_parameters.headers):
                        extends_headers = extends_parameters.headers
                    if not DaraCore.is_null(extends_parameters.queries):
                        extends_queries = extends_parameters.queries
                _request.query = DaraCore.merge({}, global_queries, extends_queries, request.query)
                # endpoint is setted in product client
                _request.headers = DaraCore.merge({
                    'host': self._endpoint,
                    'x-acs-version': params.version,
                    'x-acs-action': params.action,
                    'user-agent': Utils.get_user_agent(self._user_agent),
                    'x-acs-date': Utils.get_timestamp(),
                    'x-acs-signature-nonce': Utils.get_nonce(),
                    'accept': 'application/json',
                }, global_headers, extends_headers, request.headers)
                if params.style == 'RPC':
                    headers = self.get_rpc_headers()
                    if not DaraCore.is_null(headers):
                        _request.headers = DaraCore.merge({}, _request.headers, headers)
                signature_algorithm = self._signature_algorithm or 'ACS3-HMAC-SHA256'
                hashed_request_payload = Utils.hash(DaraBytes.from_('', 'utf-8'), signature_algorithm)
                if not DaraCore.is_null(request.stream):
                    tmp = DaraStream.read_as_bytes(request.stream)
                    hashed_request_payload = Utils.hash(tmp, signature_algorithm)
                    _request.body = tmp
                    _request.headers["content-type"] = 'application/octet-stream'
                else:
                    if not DaraCore.is_null(request.body):
                        if params.req_body_type == 'byte':
                            byte_obj = bytes(request.body)
                            hashed_request_payload = Utils.hash(byte_obj, signature_algorithm)
                            _request.body = byte_obj
                        elif params.req_body_type == 'json':
                            json_obj = DaraCore.to_json_string(request.body)
                            hashed_request_payload = Utils.hash(json_obj.encode('utf-8'), signature_algorithm)
                            _request.body = json_obj
                            _request.headers["content-type"] = 'application/json; charset=utf-8'
                        else:
                            m = request.body
                            form_obj = Utils.to_form(m)
                            hashed_request_payload = Utils.hash(form_obj.encode('utf-8'), signature_algorithm)
                            _request.body = form_obj
                            _request.headers["content-type"] = 'application/x-www-form-urlencoded'


                _request.headers["x-acs-content-sha256"] = hashed_request_payload.hex()
                if params.auth_type != 'Anonymous':
                    if DaraCore.is_null(self._credential):
                        raise main_exceptions.ClientException(
                            code = f'InvalidCredentials',
                            message = f'Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.'
                        )
                    credential_model = self._credential.get_credential()
                    if not DaraCore.is_null(credential_model.provider_name):
                        _request.headers["x-acs-credentials-provider"] = credential_model.provider_name
                    auth_type = credential_model.type
                    if auth_type == 'bearer':
                        bearer_token = credential_model.bearer_token
                        _request.headers["x-acs-bearer-token"] = bearer_token
                        if params.style == 'RPC':
                            _request.query["SignatureType"] = 'BEARERTOKEN'
                        else:
                            _request.headers["x-acs-signature-type"] = 'BEARERTOKEN'

                    elif auth_type == 'id_token':
                        id_token = credential_model.security_token
                        _request.headers["x-acs-zero-trust-idtoken"] = id_token
                    else:
                        access_key_id = credential_model.access_key_id
                        access_key_secret = credential_model.access_key_secret
                        security_token = credential_model.security_token
                        if not DaraCore.is_null(security_token) and security_token != '':
                            _request.headers["x-acs-accesskey-id"] = access_key_id
                            _request.headers["x-acs-security-token"] = security_token
                        _request.headers["Authorization"] = Utils.get_authorization(_request, signature_algorithm, hashed_request_payload.hex(), access_key_id, access_key_secret)

                _last_request = _request
                _response = DaraCore.do_action(_request, _runtime)
                _last_response = _response
                if (_response.status_code >= 400) and (_response.status_code < 600):
                    err = {}
                    if not DaraCore.is_null(_response.headers.get("content-type")) and _response.headers.get("content-type") == 'text/xml;charset=utf-8':
                        _str = DaraStream.read_as_string(_response.body)
                        resp_map = DaraXML.parse_xml(_str, None)
                        err = resp_map.get("Error")
                    else:
                        _res = DaraStream.read_as_json(_response.body)
                        err = _res

                    request_id = err.get("RequestId") or err.get("requestId")
                    code = err.get("Code") or err.get("code")
                    if (f'{code}' == 'Throttling') or (f'{code}' == 'Throttling.User') or (f'{code}' == 'Throttling.Api'):
                        raise main_exceptions.ThrottlingException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            retry_after = Utils.get_throttling_time_left(_response.headers),
                            data = err,
                            request_id = f'{request_id}'
                        )
                    elif (_response.status_code >= 400) and (_response.status_code < 500):
                        raise main_exceptions.ClientException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            access_denied_detail = self.get_access_denied_detail(err),
                            request_id = f'{request_id}'
                        )
                    else:
                        raise main_exceptions.ServerException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            request_id = f'{request_id}'
                        )

                if params.body_type == 'binary':
                    resp = {
                        'body': _response.body,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                    return resp
                elif params.body_type == 'byte':
                    byt = DaraStream.read_as_bytes(_response.body)
                    return {
                        'body': byt,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif params.body_type == 'string':
                    resp_str = DaraStream.read_as_string(_response.body)
                    return {
                        'body': resp_str,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif params.body_type == 'json':
                    obj = DaraStream.read_as_json(_response.body)
                    res = obj
                    return {
                        'body': res,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif params.body_type == 'array':
                    arr = DaraStream.read_as_json(_response.body)
                    return {
                        'body': arr,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                else:
                    anything = DaraStream.read_as_string(_response.body)
                    return {
                        'body': anything,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }

            except Exception as e:
                _context = RetryPolicyContext(
                    retries_attempted= _retries_attempted,
                    http_request = _last_request,
                    http_response = _last_response,
                    exception = e
                )
                continue
        raise UnretryableException(_context)

    async def do_request_async(
        self,
        params: open_api_util_models.Params,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> dict:
        _runtime = {
            'key': runtime.key or self._key,
            'cert': runtime.cert or self._cert,
            'ca': runtime.ca or self._ca,
            'readTimeout': DaraCore.to_number(runtime.read_timeout or self._read_timeout),
            'connectTimeout': DaraCore.to_number(runtime.connect_timeout or self._connect_timeout),
            'httpProxy': runtime.http_proxy or self._http_proxy,
            'httpsProxy': runtime.https_proxy or self._https_proxy,
            'noProxy': runtime.no_proxy or self._no_proxy,
            'socks5Proxy': runtime.socks_5proxy or self._socks_5proxy,
            'socks5NetWork': runtime.socks_5net_work or self._socks_5net_work,
            'maxIdleConns': DaraCore.to_number(runtime.max_idle_conns or self._max_idle_conns),
            'retryOptions': self._retry_options,
            'ignoreSSL': runtime.ignore_ssl,
            'tlsMinVersion': self._tls_min_version,
        }
        _last_request = None
        _last_response = None
        _retries_attempted = 0
        _context = RetryPolicyContext(
            retries_attempted= _retries_attempted
        )
        while DaraCore.should_retry(_runtime.get('retryOptions'), _context):
            if _retries_attempted > 0:
                _backoff_time = DaraCore.get_backoff_time(_runtime.get('retryOptions'), _context)
                if _backoff_time > 0:
                    DaraCore.sleep(_backoff_time)
            _retries_attempted = _retries_attempted + 1
            try:
                _request = DaraRequest()
                _request.protocol = self._protocol or params.protocol
                _request.method = params.method
                _request.pathname = params.pathname
                global_queries = {}
                global_headers = {}
                if not DaraCore.is_null(self._global_parameters):
                    global_params = self._global_parameters
                    if not DaraCore.is_null(global_params.queries):
                        global_queries = global_params.queries
                    if not DaraCore.is_null(global_params.headers):
                        global_headers = global_params.headers
                extends_headers = {}
                extends_queries = {}
                if not DaraCore.is_null(runtime.extends_parameters):
                    extends_parameters = runtime.extends_parameters
                    if not DaraCore.is_null(extends_parameters.headers):
                        extends_headers = extends_parameters.headers
                    if not DaraCore.is_null(extends_parameters.queries):
                        extends_queries = extends_parameters.queries
                _request.query = DaraCore.merge({}, global_queries, extends_queries, request.query)
                # endpoint is setted in product client
                _request.headers = DaraCore.merge({
                    'host': self._endpoint,
                    'x-acs-version': params.version,
                    'x-acs-action': params.action,
                    'user-agent': Utils.get_user_agent(self._user_agent),
                    'x-acs-date': Utils.get_timestamp(),
                    'x-acs-signature-nonce': Utils.get_nonce(),
                    'accept': 'application/json',
                }, global_headers, extends_headers, request.headers)
                if params.style == 'RPC':
                    headers = self.get_rpc_headers()
                    if not DaraCore.is_null(headers):
                        _request.headers = DaraCore.merge({}, _request.headers, headers)
                signature_algorithm = self._signature_algorithm or 'ACS3-HMAC-SHA256'
                hashed_request_payload = Utils.hash(DaraBytes.from_('', 'utf-8'), signature_algorithm)
                if not DaraCore.is_null(request.stream):
                    tmp = await DaraStream.read_as_bytes_async(request.stream)
                    hashed_request_payload = Utils.hash(tmp, signature_algorithm)
                    _request.body = tmp
                    _request.headers["content-type"] = 'application/octet-stream'
                else:
                    if not DaraCore.is_null(request.body):
                        if params.req_body_type == 'byte':
                            byte_obj = bytes(request.body)
                            hashed_request_payload = Utils.hash(byte_obj, signature_algorithm)
                            _request.body = byte_obj
                        elif params.req_body_type == 'json':
                            json_obj = DaraCore.to_json_string(request.body)
                            hashed_request_payload = Utils.hash(json_obj.encode('utf-8'), signature_algorithm)
                            _request.body = json_obj
                            _request.headers["content-type"] = 'application/json; charset=utf-8'
                        else:
                            m = request.body
                            form_obj = Utils.to_form(m)
                            hashed_request_payload = Utils.hash(form_obj.encode('utf-8'), signature_algorithm)
                            _request.body = form_obj
                            _request.headers["content-type"] = 'application/x-www-form-urlencoded'


                _request.headers["x-acs-content-sha256"] = hashed_request_payload.hex()
                if params.auth_type != 'Anonymous':
                    if DaraCore.is_null(self._credential):
                        raise main_exceptions.ClientException(
                            code = f'InvalidCredentials',
                            message = f'Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.'
                        )
                    credential_model = await self._credential.get_credential_async()
                    if not DaraCore.is_null(credential_model.provider_name):
                        _request.headers["x-acs-credentials-provider"] = credential_model.provider_name
                    auth_type = credential_model.type
                    if auth_type == 'bearer':
                        bearer_token = credential_model.bearer_token
                        _request.headers["x-acs-bearer-token"] = bearer_token
                        if params.style == 'RPC':
                            _request.query["SignatureType"] = 'BEARERTOKEN'
                        else:
                            _request.headers["x-acs-signature-type"] = 'BEARERTOKEN'

                    elif auth_type == 'id_token':
                        id_token = credential_model.security_token
                        _request.headers["x-acs-zero-trust-idtoken"] = id_token
                    else:
                        access_key_id = credential_model.access_key_id
                        access_key_secret = credential_model.access_key_secret
                        security_token = credential_model.security_token
                        if not DaraCore.is_null(security_token) and security_token != '':
                            _request.headers["x-acs-accesskey-id"] = access_key_id
                            _request.headers["x-acs-security-token"] = security_token
                        _request.headers["Authorization"] = Utils.get_authorization(_request, signature_algorithm, hashed_request_payload.hex(), access_key_id, access_key_secret)

                _last_request = _request
                _response = await DaraCore.async_do_action(_request, _runtime)
                _last_response = _response
                if (_response.status_code >= 400) and (_response.status_code < 600):
                    err = {}
                    if not DaraCore.is_null(_response.headers.get("content-type")) and _response.headers.get("content-type") == 'text/xml;charset=utf-8':
                        _str = await DaraStream.read_as_string_async(_response.body)
                        resp_map = DaraXML.parse_xml(_str, None)
                        err = resp_map.get("Error")
                    else:
                        _res = await DaraStream.read_as_json_async(_response.body)
                        err = _res

                    request_id = err.get("RequestId") or err.get("requestId")
                    code = err.get("Code") or err.get("code")
                    if (f'{code}' == 'Throttling') or (f'{code}' == 'Throttling.User') or (f'{code}' == 'Throttling.Api'):
                        raise main_exceptions.ThrottlingException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            retry_after = Utils.get_throttling_time_left(_response.headers),
                            data = err,
                            request_id = f'{request_id}'
                        )
                    elif (_response.status_code >= 400) and (_response.status_code < 500):
                        raise main_exceptions.ClientException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            access_denied_detail = self.get_access_denied_detail(err),
                            request_id = f'{request_id}'
                        )
                    else:
                        raise main_exceptions.ServerException(
                            status_code = _response.status_code,
                            code = f'{code}',
                            message = f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {request_id}',
                            description = f'{err.get("Description") or err.get("description")}',
                            data = err,
                            request_id = f'{request_id}'
                        )

                if params.body_type == 'binary':
                    resp = {
                        'body': _response.body,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                    return resp
                elif params.body_type == 'byte':
                    byt = await DaraStream.read_as_bytes_async(_response.body)
                    return {
                        'body': byt,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif params.body_type == 'string':
                    resp_str = await DaraStream.read_as_string_async(_response.body)
                    return {
                        'body': resp_str,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif params.body_type == 'json':
                    obj = await DaraStream.read_as_json_async(_response.body)
                    res = obj
                    return {
                        'body': res,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                elif params.body_type == 'array':
                    arr = await DaraStream.read_as_json_async(_response.body)
                    return {
                        'body': arr,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }
                else:
                    anything = await DaraStream.read_as_string_async(_response.body)
                    return {
                        'body': anything,
                        'headers': _response.headers,
                        'statusCode': _response.status_code
                    }

            except Exception as e:
                _context = RetryPolicyContext(
                    retries_attempted= _retries_attempted,
                    http_request = _last_request,
                    http_response = _last_response,
                    exception = e
                )
                continue
        raise UnretryableException(_context)

    """
     * @remarks
     * Encapsulate the request and invoke the network
     * 
     * @param action - api name
     * @param version - product version
     * @param protocol - http or https
     * @param method - e.g. GET
     * @param authType - authorization type e.g. AK
     * @param bodyType - response body type e.g. String
     * @param request - object of OpenApiRequest
     * @param runtime - which controls some details of call api, such as retry times
     * @returns the response
    """
    def execute(
        self,
        params: open_api_util_models.Params,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> dict:
        _runtime = {
            'key': runtime.key or self._key,
            'cert': runtime.cert or self._cert,
            'ca': runtime.ca or self._ca,
            'readTimeout': DaraCore.to_number(runtime.read_timeout or self._read_timeout),
            'connectTimeout': DaraCore.to_number(runtime.connect_timeout or self._connect_timeout),
            'httpProxy': runtime.http_proxy or self._http_proxy,
            'httpsProxy': runtime.https_proxy or self._https_proxy,
            'noProxy': runtime.no_proxy or self._no_proxy,
            'socks5Proxy': runtime.socks_5proxy or self._socks_5proxy,
            'socks5NetWork': runtime.socks_5net_work or self._socks_5net_work,
            'maxIdleConns': DaraCore.to_number(runtime.max_idle_conns or self._max_idle_conns),
            'retryOptions': self._retry_options,
            'ignoreSSL': runtime.ignore_ssl,
            'tlsMinVersion': self._tls_min_version,
            'disableHttp2': bool(self._disable_http_2 or False),
        }
        _last_request = None
        _last_response = None
        _retries_attempted = 0
        _context = RetryPolicyContext(
            retries_attempted= _retries_attempted
        )
        while DaraCore.should_retry(_runtime.get('retryOptions'), _context):
            if _retries_attempted > 0:
                _backoff_time = DaraCore.get_backoff_time(_runtime.get('retryOptions'), _context)
                if _backoff_time > 0:
                    DaraCore.sleep(_backoff_time)
            _retries_attempted = _retries_attempted + 1
            try:
                _request = DaraRequest()
                # spi = new Gateway();//Gateway implements SPI，这一步在产品 SDK 中实例化
                headers = self.get_rpc_headers()
                global_queries = {}
                global_headers = {}
                if not DaraCore.is_null(self._global_parameters):
                    global_params = self._global_parameters
                    if not DaraCore.is_null(global_params.queries):
                        global_queries = global_params.queries
                    if not DaraCore.is_null(global_params.headers):
                        global_headers = global_params.headers
                extends_headers = {}
                extends_queries = {}
                if not DaraCore.is_null(runtime.extends_parameters):
                    extends_parameters = runtime.extends_parameters
                    if not DaraCore.is_null(extends_parameters.headers):
                        extends_headers = extends_parameters.headers
                    if not DaraCore.is_null(extends_parameters.queries):
                        extends_queries = extends_parameters.queries
                request_context = spi_models.InterceptorContextRequest(
                    headers = DaraCore.merge({}, global_headers, extends_headers, request.headers, headers),
                    query = DaraCore.merge({}, global_queries, extends_queries, request.query),
                    body = request.body,
                    stream = request.stream,
                    host_map = request.host_map,
                    pathname = params.pathname,
                    product_id = self._product_id,
                    action = params.action,
                    version = params.version,
                    protocol = self._protocol or params.protocol,
                    method = self._method or params.method,
                    auth_type = params.auth_type,
                    body_type = params.body_type,
                    req_body_type = params.req_body_type,
                    style = params.style,
                    credential = self._credential,
                    signature_version = self._signature_version,
                    signature_algorithm = self._signature_algorithm,
                    user_agent = Utils.get_user_agent(self._user_agent)
                )
                configuration_context = spi_models.InterceptorContextConfiguration(
                    region_id = self._region_id,
                    endpoint = request.endpoint_override or self._endpoint,
                    endpoint_rule = self._endpoint_rule,
                    endpoint_map = self._endpoint_map,
                    endpoint_type = self._endpoint_type,
                    network = self._network,
                    suffix = self._suffix
                )
                interceptor_context = spi_models.InterceptorContext(
                    request = request_context,
                    configuration = configuration_context
                )
                attribute_map = spi_models.AttributeMap()
                if not DaraCore.is_null(self._attribute_map):
                    attribute_map = self._attribute_map
                # 1. spi.modifyConfiguration(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                self._spi.modify_configuration(interceptor_context, attribute_map)
                # 2. spi.modifyRequest(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                self._spi.modify_request(interceptor_context, attribute_map)
                _request.protocol = interceptor_context.request.protocol
                _request.method = interceptor_context.request.method
                _request.pathname = interceptor_context.request.pathname
                _request.query = interceptor_context.request.query
                _request.body = interceptor_context.request.stream
                _request.headers = interceptor_context.request.headers
                _last_request = _request
                _response = DaraCore.do_action(_request, _runtime)
                _last_response = _response
                response_context = spi_models.InterceptorContextResponse(
                    status_code = _response.status_code,
                    headers = _response.headers,
                    body = _response.body
                )
                interceptor_context.response = response_context
                # 3. spi.modifyResponse(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                self._spi.modify_response(interceptor_context, attribute_map)
                return {
                    'headers': interceptor_context.response.headers,
                    'statusCode': interceptor_context.response.status_code,
                    'body': interceptor_context.response.deserialized_body
                }
            except Exception as e:
                _context = RetryPolicyContext(
                    retries_attempted= _retries_attempted,
                    http_request = _last_request,
                    http_response = _last_response,
                    exception = e
                )
                continue
        raise UnretryableException(_context)

    async def execute_async(
        self,
        params: open_api_util_models.Params,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> dict:
        _runtime = {
            'key': runtime.key or self._key,
            'cert': runtime.cert or self._cert,
            'ca': runtime.ca or self._ca,
            'readTimeout': DaraCore.to_number(runtime.read_timeout or self._read_timeout),
            'connectTimeout': DaraCore.to_number(runtime.connect_timeout or self._connect_timeout),
            'httpProxy': runtime.http_proxy or self._http_proxy,
            'httpsProxy': runtime.https_proxy or self._https_proxy,
            'noProxy': runtime.no_proxy or self._no_proxy,
            'socks5Proxy': runtime.socks_5proxy or self._socks_5proxy,
            'socks5NetWork': runtime.socks_5net_work or self._socks_5net_work,
            'maxIdleConns': DaraCore.to_number(runtime.max_idle_conns or self._max_idle_conns),
            'retryOptions': self._retry_options,
            'ignoreSSL': runtime.ignore_ssl,
            'tlsMinVersion': self._tls_min_version,
            'disableHttp2': bool(self._disable_http_2 or False),
        }
        _last_request = None
        _last_response = None
        _retries_attempted = 0
        _context = RetryPolicyContext(
            retries_attempted= _retries_attempted
        )
        while DaraCore.should_retry(_runtime.get('retryOptions'), _context):
            if _retries_attempted > 0:
                _backoff_time = DaraCore.get_backoff_time(_runtime.get('retryOptions'), _context)
                if _backoff_time > 0:
                    DaraCore.sleep(_backoff_time)
            _retries_attempted = _retries_attempted + 1
            try:
                _request = DaraRequest()
                # spi = new Gateway();//Gateway implements SPI，这一步在产品 SDK 中实例化
                headers = self.get_rpc_headers()
                global_queries = {}
                global_headers = {}
                if not DaraCore.is_null(self._global_parameters):
                    global_params = self._global_parameters
                    if not DaraCore.is_null(global_params.queries):
                        global_queries = global_params.queries
                    if not DaraCore.is_null(global_params.headers):
                        global_headers = global_params.headers
                extends_headers = {}
                extends_queries = {}
                if not DaraCore.is_null(runtime.extends_parameters):
                    extends_parameters = runtime.extends_parameters
                    if not DaraCore.is_null(extends_parameters.headers):
                        extends_headers = extends_parameters.headers
                    if not DaraCore.is_null(extends_parameters.queries):
                        extends_queries = extends_parameters.queries
                request_context = spi_models.InterceptorContextRequest(
                    headers = DaraCore.merge({}, global_headers, extends_headers, request.headers, headers),
                    query = DaraCore.merge({}, global_queries, extends_queries, request.query),
                    body = request.body,
                    stream = request.stream,
                    host_map = request.host_map,
                    pathname = params.pathname,
                    product_id = self._product_id,
                    action = params.action,
                    version = params.version,
                    protocol = self._protocol or params.protocol,
                    method = self._method or params.method,
                    auth_type = params.auth_type,
                    body_type = params.body_type,
                    req_body_type = params.req_body_type,
                    style = params.style,
                    credential = self._credential,
                    signature_version = self._signature_version,
                    signature_algorithm = self._signature_algorithm,
                    user_agent = Utils.get_user_agent(self._user_agent)
                )
                configuration_context = spi_models.InterceptorContextConfiguration(
                    region_id = self._region_id,
                    endpoint = request.endpoint_override or self._endpoint,
                    endpoint_rule = self._endpoint_rule,
                    endpoint_map = self._endpoint_map,
                    endpoint_type = self._endpoint_type,
                    network = self._network,
                    suffix = self._suffix
                )
                interceptor_context = spi_models.InterceptorContext(
                    request = request_context,
                    configuration = configuration_context
                )
                attribute_map = spi_models.AttributeMap()
                if not DaraCore.is_null(self._attribute_map):
                    attribute_map = self._attribute_map
                # 1. spi.modifyConfiguration(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                await self._spi.modify_configuration_async(interceptor_context, attribute_map)
                # 2. spi.modifyRequest(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                await self._spi.modify_request_async(interceptor_context, attribute_map)
                _request.protocol = interceptor_context.request.protocol
                _request.method = interceptor_context.request.method
                _request.pathname = interceptor_context.request.pathname
                _request.query = interceptor_context.request.query
                _request.body = interceptor_context.request.stream
                _request.headers = interceptor_context.request.headers
                _last_request = _request
                _response = await DaraCore.async_do_action(_request, _runtime)
                _last_response = _response
                response_context = spi_models.InterceptorContextResponse(
                    status_code = _response.status_code,
                    headers = _response.headers,
                    body = _response.body
                )
                interceptor_context.response = response_context
                # 3. spi.modifyResponse(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
                await self._spi.modify_response_async(interceptor_context, attribute_map)
                return {
                    'headers': interceptor_context.response.headers,
                    'statusCode': interceptor_context.response.status_code,
                    'body': interceptor_context.response.deserialized_body
                }
            except Exception as e:
                _context = RetryPolicyContext(
                    retries_attempted= _retries_attempted,
                    http_request = _last_request,
                    http_response = _last_response,
                    exception = e
                )
                continue
        raise UnretryableException(_context)

    def call_sseapi(
        self,
        params: open_api_util_models.Params,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> Generator[main_models.SSEResponse, None, None]:
        _runtime = {
            'key': runtime.key or self._key,
            'cert': runtime.cert or self._cert,
            'ca': runtime.ca or self._ca,
            'readTimeout': DaraCore.to_number(runtime.read_timeout or self._read_timeout),
            'connectTimeout': DaraCore.to_number(runtime.connect_timeout or self._connect_timeout),
            'httpProxy': runtime.http_proxy or self._http_proxy,
            'httpsProxy': runtime.https_proxy or self._https_proxy,
            'noProxy': runtime.no_proxy or self._no_proxy,
            'socks5Proxy': runtime.socks_5proxy or self._socks_5proxy,
            'socks5NetWork': runtime.socks_5net_work or self._socks_5net_work,
            'maxIdleConns': DaraCore.to_number(runtime.max_idle_conns or self._max_idle_conns),
            'retryOptions': self._retry_options,
            'ignoreSSL': runtime.ignore_ssl,
            'tlsMinVersion': self._tls_min_version,
        }
        _last_request = None
        _last_response = None
        _retries_attempted = 0
        _context = RetryPolicyContext(
            retries_attempted= _retries_attempted
        )
        while DaraCore.should_retry(_runtime.get('retryOptions'), _context):
            if _retries_attempted > 0:
                _backoff_time = DaraCore.get_backoff_time(_runtime.get('retryOptions'), _context)
                if _backoff_time > 0:
                    DaraCore.sleep(_backoff_time)
            _retries_attempted = _retries_attempted + 1
            try:
                _request = DaraRequest()
                _request.protocol = self._protocol or params.protocol
                _request.method = params.method
                _request.pathname = params.pathname
                global_queries = {}
                global_headers = {}
                if not DaraCore.is_null(self._global_parameters):
                    global_params = self._global_parameters
                    if not DaraCore.is_null(global_params.queries):
                        global_queries = global_params.queries
                    if not DaraCore.is_null(global_params.headers):
                        global_headers = global_params.headers
                extends_headers = {}
                extends_queries = {}
                if not DaraCore.is_null(runtime.extends_parameters):
                    extends_parameters = runtime.extends_parameters
                    if not DaraCore.is_null(extends_parameters.headers):
                        extends_headers = extends_parameters.headers
                    if not DaraCore.is_null(extends_parameters.queries):
                        extends_queries = extends_parameters.queries
                _request.query = DaraCore.merge({}, global_queries, extends_queries, request.query)
                # endpoint is setted in product client
                _request.headers = DaraCore.merge({
                    'host': self._endpoint,
                    'x-acs-version': params.version,
                    'x-acs-action': params.action,
                    'user-agent': Utils.get_user_agent(self._user_agent),
                    'x-acs-date': Utils.get_timestamp(),
                    'x-acs-signature-nonce': Utils.get_nonce(),
                    'accept': 'application/json',
                }, extends_headers, global_headers, request.headers)
                if params.style == 'RPC':
                    headers = self.get_rpc_headers()
                    if not DaraCore.is_null(headers):
                        _request.headers = DaraCore.merge({}, _request.headers, headers)
                signature_algorithm = self._signature_algorithm or 'ACS3-HMAC-SHA256'
                hashed_request_payload = Utils.hash(DaraBytes.from_('', 'utf-8'), signature_algorithm)
                if not DaraCore.is_null(request.stream):
                    tmp = DaraStream.read_as_bytes(request.stream)
                    hashed_request_payload = Utils.hash(tmp, signature_algorithm)
                    _request.body = tmp
                    _request.headers["content-type"] = 'application/octet-stream'
                else:
                    if not DaraCore.is_null(request.body):
                        if params.req_body_type == 'byte':
                            byte_obj = bytes(request.body)
                            hashed_request_payload = Utils.hash(byte_obj, signature_algorithm)
                            _request.body = byte_obj
                        elif params.req_body_type == 'json':
                            json_obj = DaraCore.to_json_string(request.body)
                            hashed_request_payload = Utils.hash(json_obj.encode('utf-8'), signature_algorithm)
                            _request.body = json_obj
                            _request.headers["content-type"] = 'application/json; charset=utf-8'
                        else:
                            m = request.body
                            form_obj = Utils.to_form(m)
                            hashed_request_payload = Utils.hash(form_obj.encode('utf-8'), signature_algorithm)
                            _request.body = form_obj
                            _request.headers["content-type"] = 'application/x-www-form-urlencoded'


                _request.headers["x-acs-content-sha256"] = hashed_request_payload.hex()
                if params.auth_type != 'Anonymous':
                    credential_model = self._credential.get_credential()
                    if not DaraCore.is_null(credential_model.provider_name):
                        _request.headers["x-acs-credentials-provider"] = credential_model.provider_name
                    auth_type = credential_model.type
                    if auth_type == 'bearer':
                        bearer_token = credential_model.bearer_token
                        _request.headers["x-acs-bearer-token"] = bearer_token
                    elif auth_type == 'id_token':
                        id_token = credential_model.security_token
                        _request.headers["x-acs-zero-trust-idtoken"] = id_token
                    else:
                        access_key_id = credential_model.access_key_id
                        access_key_secret = credential_model.access_key_secret
                        security_token = credential_model.security_token
                        if not DaraCore.is_null(security_token) and security_token != '':
                            _request.headers["x-acs-accesskey-id"] = access_key_id
                            _request.headers["x-acs-security-token"] = security_token
                        _request.headers["Authorization"] = Utils.get_authorization(_request, signature_algorithm, hashed_request_payload.hex(), access_key_id, access_key_secret)

                _last_request = _request
                _response = DaraCore.do_sse_action(_request, _runtime)
                _last_response = _response
                if (_response.status_code >= 400) and (_response.status_code < 600):
                    err = {}
                    if not DaraCore.is_null(_response.headers.get("content-type")) and _response.headers.get("content-type") == 'text/xml;charset=utf-8':
                        _str = DaraStream.read_as_string(_response.body)
                        resp_map = DaraXML.parse_xml(_str, None)
                        err = resp_map.get("Error")
                    else:
                        _res = DaraStream.read_as_json(_response.body)
                        err = _res

                    err["statusCode"] = _response.status_code
                    raise DaraException({
                        'code': f'{err.get("Code") or err.get("code")}',
                        'message': f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {err.get("RequestId") or err.get("requestId")}',
                        'data': err,
                        'description': f'{err.get("Description") or err.get("description")}',
                        'accessDeniedDetail': err.get("AccessDeniedDetail") or err.get("accessDeniedDetail")
                    })
                events = DaraStream.read_as_sse(_response.body)
                for event in events:
                    yield  main_models.SSEResponse(
                        status_code = _response.status_code,
                        headers = _response.headers,
                        event = event
                    )
                return
            except Exception as e:
                _context = RetryPolicyContext(
                    retries_attempted= _retries_attempted,
                    http_request = _last_request,
                    http_response = _last_response,
                    exception = e
                )
                continue
        raise UnretryableException(_context)

    async def call_sseapi_async(
        self,
        params: open_api_util_models.Params,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> AsyncGenerator[main_models.SSEResponse, None, None]:
        _runtime = {
            'key': runtime.key or self._key,
            'cert': runtime.cert or self._cert,
            'ca': runtime.ca or self._ca,
            'readTimeout': DaraCore.to_number(runtime.read_timeout or self._read_timeout),
            'connectTimeout': DaraCore.to_number(runtime.connect_timeout or self._connect_timeout),
            'httpProxy': runtime.http_proxy or self._http_proxy,
            'httpsProxy': runtime.https_proxy or self._https_proxy,
            'noProxy': runtime.no_proxy or self._no_proxy,
            'socks5Proxy': runtime.socks_5proxy or self._socks_5proxy,
            'socks5NetWork': runtime.socks_5net_work or self._socks_5net_work,
            'maxIdleConns': DaraCore.to_number(runtime.max_idle_conns or self._max_idle_conns),
            'retryOptions': self._retry_options,
            'ignoreSSL': runtime.ignore_ssl,
            'tlsMinVersion': self._tls_min_version,
        }
        _last_request = None
        _last_response = None
        _retries_attempted = 0
        _context = RetryPolicyContext(
            retries_attempted= _retries_attempted
        )
        while DaraCore.should_retry(_runtime.get('retryOptions'), _context):
            if _retries_attempted > 0:
                _backoff_time = DaraCore.get_backoff_time(_runtime.get('retryOptions'), _context)
                if _backoff_time > 0:
                    DaraCore.sleep(_backoff_time)
            _retries_attempted = _retries_attempted + 1
            try:
                _request = DaraRequest()
                _request.protocol = self._protocol or params.protocol
                _request.method = params.method
                _request.pathname = params.pathname
                global_queries = {}
                global_headers = {}
                if not DaraCore.is_null(self._global_parameters):
                    global_params = self._global_parameters
                    if not DaraCore.is_null(global_params.queries):
                        global_queries = global_params.queries
                    if not DaraCore.is_null(global_params.headers):
                        global_headers = global_params.headers
                extends_headers = {}
                extends_queries = {}
                if not DaraCore.is_null(runtime.extends_parameters):
                    extends_parameters = runtime.extends_parameters
                    if not DaraCore.is_null(extends_parameters.headers):
                        extends_headers = extends_parameters.headers
                    if not DaraCore.is_null(extends_parameters.queries):
                        extends_queries = extends_parameters.queries
                _request.query = DaraCore.merge({}, global_queries, extends_queries, request.query)
                # endpoint is setted in product client
                _request.headers = DaraCore.merge({
                    'host': self._endpoint,
                    'x-acs-version': params.version,
                    'x-acs-action': params.action,
                    'user-agent': Utils.get_user_agent(self._user_agent),
                    'x-acs-date': Utils.get_timestamp(),
                    'x-acs-signature-nonce': Utils.get_nonce(),
                    'accept': 'application/json',
                }, extends_headers, global_headers, request.headers)
                if params.style == 'RPC':
                    headers = self.get_rpc_headers()
                    if not DaraCore.is_null(headers):
                        _request.headers = DaraCore.merge({}, _request.headers, headers)
                signature_algorithm = self._signature_algorithm or 'ACS3-HMAC-SHA256'
                hashed_request_payload = Utils.hash(DaraBytes.from_('', 'utf-8'), signature_algorithm)
                if not DaraCore.is_null(request.stream):
                    tmp = await DaraStream.read_as_bytes_async(request.stream)
                    hashed_request_payload = Utils.hash(tmp, signature_algorithm)
                    _request.body = tmp
                    _request.headers["content-type"] = 'application/octet-stream'
                else:
                    if not DaraCore.is_null(request.body):
                        if params.req_body_type == 'byte':
                            byte_obj = bytes(request.body)
                            hashed_request_payload = Utils.hash(byte_obj, signature_algorithm)
                            _request.body = byte_obj
                        elif params.req_body_type == 'json':
                            json_obj = DaraCore.to_json_string(request.body)
                            hashed_request_payload = Utils.hash(json_obj.encode('utf-8'), signature_algorithm)
                            _request.body = json_obj
                            _request.headers["content-type"] = 'application/json; charset=utf-8'
                        else:
                            m = request.body
                            form_obj = Utils.to_form(m)
                            hashed_request_payload = Utils.hash(form_obj.encode('utf-8'), signature_algorithm)
                            _request.body = form_obj
                            _request.headers["content-type"] = 'application/x-www-form-urlencoded'


                _request.headers["x-acs-content-sha256"] = hashed_request_payload.hex()
                if params.auth_type != 'Anonymous':
                    credential_model = await self._credential.get_credential_async()
                    if not DaraCore.is_null(credential_model.provider_name):
                        _request.headers["x-acs-credentials-provider"] = credential_model.provider_name
                    auth_type = credential_model.type
                    if auth_type == 'bearer':
                        bearer_token = credential_model.bearer_token
                        _request.headers["x-acs-bearer-token"] = bearer_token
                    elif auth_type == 'id_token':
                        id_token = credential_model.security_token
                        _request.headers["x-acs-zero-trust-idtoken"] = id_token
                    else:
                        access_key_id = credential_model.access_key_id
                        access_key_secret = credential_model.access_key_secret
                        security_token = credential_model.security_token
                        if not DaraCore.is_null(security_token) and security_token != '':
                            _request.headers["x-acs-accesskey-id"] = access_key_id
                            _request.headers["x-acs-security-token"] = security_token
                        _request.headers["Authorization"] = Utils.get_authorization(_request, signature_algorithm, hashed_request_payload.hex(), access_key_id, access_key_secret)

                _last_request = _request
                _response = await DaraCore.async_do_sse_action(_request, _runtime)
                _last_response = _response
                if (_response.status_code >= 400) and (_response.status_code < 600):
                    err = {}
                    if not DaraCore.is_null(_response.headers.get("content-type")) and _response.headers.get("content-type") == 'text/xml;charset=utf-8':
                        _str = await DaraStream.read_as_string_async(_response.body)
                        resp_map = DaraXML.parse_xml(_str, None)
                        err = resp_map.get("Error")
                    else:
                        _res = await DaraStream.read_as_json_async(_response.body)
                        err = _res

                    err["statusCode"] = _response.status_code
                    raise DaraException({
                        'code': f'{err.get("Code") or err.get("code")}',
                        'message': f'code: {_response.status_code}, {err.get("Message") or err.get("message")} request id: {err.get("RequestId") or err.get("requestId")}',
                        'data': err,
                        'description': f'{err.get("Description") or err.get("description")}',
                        'accessDeniedDetail': err.get("AccessDeniedDetail") or err.get("accessDeniedDetail")
                    })
                events = DaraStream.read_as_sse_async(_response.body)
                async for event in events:
                    yield  main_models.SSEResponse(
                        status_code = _response.status_code,
                        headers = _response.headers,
                        event = event
                    )
                return
            except Exception as e:
                _context = RetryPolicyContext(
                    retries_attempted= _retries_attempted,
                    http_request = _last_request,
                    http_response = _last_response,
                    exception = e
                )
                continue
        raise UnretryableException(_context)

    def call_api(
        self,
        params: open_api_util_models.Params,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> dict:
        if DaraCore.is_null(params):
            raise main_exceptions.ClientException(
                code = 'ParameterMissing',
                message = '\'params\' can not be unset'
            )
        if DaraCore.is_null(self._signature_version) or self._signature_version != 'v4':
            if DaraCore.is_null(self._signature_algorithm) or self._signature_algorithm != 'v2':
                return self.do_request(params, request, runtime)
            elif (params.style == 'ROA') and (params.req_body_type == 'json'):
                return self.do_roarequest(params.action, params.version, params.protocol, params.method, params.auth_type, params.pathname, params.body_type, request, runtime)
            elif params.style == 'ROA':
                return self.do_roarequest_with_form(params.action, params.version, params.protocol, params.method, params.auth_type, params.pathname, params.body_type, request, runtime)
            else:
                return self.do_rpcrequest(params.action, params.version, params.protocol, params.method, params.auth_type, params.body_type, request, runtime)

        else:
            return self.execute(params, request, runtime)


    async def call_api_async(
        self,
        params: open_api_util_models.Params,
        request: open_api_util_models.OpenApiRequest,
        runtime: RuntimeOptions,
    ) -> dict:
        if DaraCore.is_null(params):
            raise main_exceptions.ClientException(
                code = 'ParameterMissing',
                message = '\'params\' can not be unset'
            )
        if DaraCore.is_null(self._signature_version) or self._signature_version != 'v4':
            if DaraCore.is_null(self._signature_algorithm) or self._signature_algorithm != 'v2':
                return await self.do_request_async(params, request, runtime)
            elif (params.style == 'ROA') and (params.req_body_type == 'json'):
                return await self.do_roarequest_async(params.action, params.version, params.protocol, params.method, params.auth_type, params.pathname, params.body_type, request, runtime)
            elif params.style == 'ROA':
                return await self.do_roarequest_with_form_async(params.action, params.version, params.protocol, params.method, params.auth_type, params.pathname, params.body_type, request, runtime)
            else:
                return await self.do_rpcrequest_async(params.action, params.version, params.protocol, params.method, params.auth_type, params.body_type, request, runtime)

        else:
            return await self.execute_async(params, request, runtime)


    def get_access_key_id(self) -> str:
        if DaraCore.is_null(self._credential):
            return ''
        access_key_id = self._credential.get_access_key_id()
        return access_key_id

    async def get_access_key_id_async(self) -> str:
        if DaraCore.is_null(self._credential):
            return ''
        access_key_id = await self._credential.get_access_key_id_async()
        return access_key_id

    def get_access_key_secret(self) -> str:
        if DaraCore.is_null(self._credential):
            return ''
        secret = self._credential.get_access_key_secret()
        return secret

    async def get_access_key_secret_async(self) -> str:
        if DaraCore.is_null(self._credential):
            return ''
        secret = await self._credential.get_access_key_secret_async()
        return secret

    def get_security_token(self) -> str:
        if DaraCore.is_null(self._credential):
            return ''
        token = self._credential.get_security_token()
        return token

    async def get_security_token_async(self) -> str:
        if DaraCore.is_null(self._credential):
            return ''
        token = await self._credential.get_security_token_async()
        return token

    def get_bearer_token(self) -> str:
        if DaraCore.is_null(self._credential):
            return ''
        token = self._credential.get_bearer_token()
        return token

    async def get_bearer_token_async(self) -> str:
        if DaraCore.is_null(self._credential):
            return ''
        token = self._credential.get_bearer_token()
        return token

    def get_type(self) -> str:
        if DaraCore.is_null(self._credential):
            return ''
        auth_type = self._credential.get_type()
        return auth_type

    async def get_type_async(self) -> str:
        if DaraCore.is_null(self._credential):
            return ''
        auth_type = self._credential.get_type()
        return auth_type

    def check_config(
        self,
        config: open_api_util_models.Config,
    ) -> None:
        if DaraCore.is_null(self._endpoint_rule) and DaraCore.is_null(config.endpoint):
            raise main_exceptions.ClientException(
                code = 'ParameterMissing',
                message = '\'config.endpoint\' can not be empty'
            )

    def set_gateway_client(
        self,
        spi: SPIClient,
    ) -> None:
        self._spi = spi

    def set_rpc_headers(
        self,
        headers: Dict[str, str],
    ) -> None:
        self._headers = headers

    def get_rpc_headers(self) -> Dict[str, str]:
        headers = self._headers
        self._headers = None
        return headers

    def default_any(
        input_value: Any,
        default_value: Any,
    ) -> Any:
        if DaraCore.is_null(input_value):
            return default_value
        return input_value

    def get_access_denied_detail(
        self,
        err: Dict[str, Any],
    ) -> Dict[str, Any]:
        access_denied_detail = None
        if not DaraCore.is_null(err.get("AccessDeniedDetail")):
            detail_1 = err.get("AccessDeniedDetail")
            access_denied_detail = detail_1
        elif not DaraCore.is_null(err.get("accessDeniedDetail")):
            detail_2 = err.get("accessDeniedDetail")
            access_denied_detail = detail_2
        return access_denied_detail
