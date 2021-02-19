# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from __future__ import unicode_literals

import time

from Tea.exceptions import TeaException, UnretryableException
from Tea.request import TeaRequest
from Tea.core import TeaCore
from Tea.converter import TeaConverter

from alibabacloud_credentials.client import Client as CredentialClient
from alibabacloud_tea_util.client import Client as UtilClient
from alibabacloud_credentials import models as credential_models
from alibabacloud_openapi_util.client import Client as OpenApiUtilClient


class Client(object):
    """
    This is for OpenApi SDK
    """
    _endpoint = None  # type: unicode
    _region_id = None  # type: unicode
    _protocol = None  # type: unicode
    _user_agent = None  # type: unicode
    _endpoint_rule = None  # type: unicode
    _endpoint_map = None  # type: dict[unicode, unicode]
    _suffix = None  # type: unicode
    _read_timeout = None  # type: int
    _connect_timeout = None  # type: int
    _http_proxy = None  # type: unicode
    _https_proxy = None  # type: unicode
    _socks_5proxy = None  # type: unicode
    _socks_5net_work = None  # type: unicode
    _no_proxy = None  # type: unicode
    _network = None  # type: unicode
    _product_id = None  # type: unicode
    _max_idle_conns = None  # type: int
    _endpoint_type = None  # type: unicode
    _open_platform_endpoint = None  # type: unicode
    _credential = None  # type: CredentialClient
    _signature_algorithm = None  # type: unicode

    def __init__(self, config):
        """
        Init client with Config

        @param config: config contains the necessary information to create a client
        """
        if UtilClient.is_unset(config):
            raise TeaException({
                'code': 'ParameterMissing',
                'message': "'config' can not be unset"
            })
        if not UtilClient.empty(config.access_key_id) and not UtilClient.empty(config.access_key_secret):
            if not UtilClient.empty(config.security_token):
                config.type = 'sts'
            else:
                config.type = 'access_key'
            credential_config = credential_models.Config(
                access_key_id=config.access_key_id,
                type=config.type,
                access_key_secret=config.access_key_secret,
                security_token=config.security_token
            )
            self._credential = CredentialClient(credential_config)
        elif not UtilClient.is_unset(config.credential):
            self._credential = config.credential
        self._endpoint = config.endpoint
        self._protocol = config.protocol
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
        self._signature_algorithm = config.signature_algorithm

    def do_rpcrequest(self, action, version, protocol, method, auth_type, body_type, request, runtime):
        """
        Encapsulate the request and invoke the network

        @type action: unicode
        @param action: api name

        @type version: unicode
        @param version: product version

        @type protocol: unicode
        @param protocol: http or https

        @type method: unicode
        @param method: e.g. GET

        @type auth_type: unicode
        @param auth_type: authorization type e.g. AK

        @type body_type: unicode
        @param body_type: response body type e.g. String

        @param request: object of OpenApiRequest

        @param runtime: which controls some details of call api, such as retry times

        @rtype: dict
        @return: the response
        """
        request.validate()
        runtime.validate()
        _runtime = {
            'timeouted': 'retry',
            'readTimeout': UtilClient.default_number(runtime.read_timeout, self._read_timeout),
            'connectTimeout': UtilClient.default_number(runtime.connect_timeout, self._connect_timeout),
            'httpProxy': UtilClient.default_string(runtime.http_proxy, self._http_proxy),
            'httpsProxy': UtilClient.default_string(runtime.https_proxy, self._https_proxy),
            'noProxy': UtilClient.default_string(runtime.no_proxy, self._no_proxy),
            'maxIdleConns': UtilClient.default_number(runtime.max_idle_conns, self._max_idle_conns),
            'retry': {
                'retryable': runtime.autoretry,
                'maxAttempts': UtilClient.default_number(runtime.max_attempts, 3)
            },
            'backoff': {
                'policy': UtilClient.default_string(runtime.backoff_policy, 'no'),
                'period': UtilClient.default_number(runtime.backoff_period, 1)
            },
            'ignoreSSL': runtime.ignore_ssl
        }
        _last_request = None
        _last_exception = None
        _now = time.time()
        _retry_times = 0
        while TeaCore.allow_retry(_runtime.get('retry'), _retry_times, _now):
            if _retry_times > 0:
                _backoff_time = TeaCore.get_backoff_time(_runtime.get('backoff'), _retry_times)
                if _backoff_time > 0:
                    TeaCore.sleep(_backoff_time)
            _retry_times = _retry_times + 1
            try:
                _request = TeaRequest()
                _request.protocol = UtilClient.default_string(self._protocol, protocol)
                _request.method = method
                _request.pathname = '/'
                _request.query = TeaCore.merge({
                    'Action': action,
                    'Format': 'json',
                    'Version': version,
                    'Timestamp': OpenApiUtilClient.get_timestamp(),
                    'SignatureNonce': UtilClient.get_nonce()
                }, request.query)
                # endpoint is setted in product client
                _request.headers = {
                    'host': self._endpoint,
                    'x-acs-version': version,
                    'x-acs-action': action,
                    'user-agent': self.get_user_agent()
                }
                if not UtilClient.is_unset(request.body):
                    m = UtilClient.assert_as_map(request.body)
                    tmp = UtilClient.anyify_map_value(OpenApiUtilClient.query(m))
                    _request.body = UtilClient.to_form_string(tmp)
                    _request.headers['content-type'] = 'application/x-www-form-urlencoded'
                if not UtilClient.equal_string(auth_type, 'Anonymous'):
                    access_key_id = self.get_access_key_id()
                    access_key_secret = self.get_access_key_secret()
                    security_token = self.get_security_token()
                    if not UtilClient.empty(security_token):
                        _request.query['SecurityToken'] = security_token
                    _request.query['SignatureMethod'] = 'HMAC-SHA1'
                    _request.query['SignatureVersion'] = '1.0'
                    _request.query['AccessKeyId'] = access_key_id
                    t = None
                    if not UtilClient.is_unset(request.body):
                        t = UtilClient.assert_as_map(request.body)
                    signed_param = TeaCore.merge(_request.query,
                        OpenApiUtilClient.query(t))
                    _request.query['Signature'] = OpenApiUtilClient.get_rpcsignature(signed_param, _request.method, access_key_secret)
                _last_request = _request
                _response = TeaCore.do_action(_request, _runtime)
                if UtilClient.is_4xx(_response.status_code) or UtilClient.is_5xx(_response.status_code):
                    _res = UtilClient.read_as_json(_response.body)
                    err = UtilClient.assert_as_map(_res)
                    raise TeaException({
                        'code': '%s' % TeaConverter.to_unicode(self.default_any(err.get('Code'), err.get('code'))),
                        'message': 'code: %s, %s request id: %s' % (TeaConverter.to_unicode(_response.status_code), TeaConverter.to_unicode(self.default_any(err.get('Message'), err.get('message'))), TeaConverter.to_unicode(self.default_any(err.get('RequestId'), err.get('requestId')))),
                        'data': err
                    })
                if UtilClient.equal_string(body_type, 'binary'):
                    resp = {
                        'body': _response.body,
                        'headers': _response.headers
                    }
                    return resp
                elif UtilClient.equal_string(body_type, 'byte'):
                    byt = UtilClient.read_as_bytes(_response.body)
                    return {
                        'body': byt,
                        'headers': _response.headers
                    }
                elif UtilClient.equal_string(body_type, 'string'):
                    str = UtilClient.read_as_string(_response.body)
                    return {
                        'body': str,
                        'headers': _response.headers
                    }
                elif UtilClient.equal_string(body_type, 'json'):
                    obj = UtilClient.read_as_json(_response.body)
                    res = UtilClient.assert_as_map(obj)
                    return {
                        'body': res,
                        'headers': _response.headers
                    }
                elif UtilClient.equal_string(body_type, 'array'):
                    arr = UtilClient.read_as_json(_response.body)
                    return {
                        'body': arr,
                        'headers': _response.headers
                    }
                else:
                    return {
                        'headers': _response.headers
                    }
            except Exception as e:
                if TeaCore.is_retryable(e):
                    _last_exception = e
                    continue
                raise e
        raise UnretryableException(_last_request, _last_exception)

    def do_roarequest(self, action, version, protocol, method, auth_type, pathname, body_type, request, runtime):
        """
        Encapsulate the request and invoke the network

        @type action: unicode
        @param action: api name

        @type version: unicode
        @param version: product version

        @type protocol: unicode
        @param protocol: http or https

        @type method: unicode
        @param method: e.g. GET

        @type auth_type: unicode
        @param auth_type: authorization type e.g. AK

        @type pathname: unicode
        @param pathname: pathname of every api

        @type body_type: unicode
        @param body_type: response body type e.g. String

        @param request: object of OpenApiRequest

        @param runtime: which controls some details of call api, such as retry times

        @rtype: dict
        @return: the response
        """
        request.validate()
        runtime.validate()
        _runtime = {
            'timeouted': 'retry',
            'readTimeout': UtilClient.default_number(runtime.read_timeout, self._read_timeout),
            'connectTimeout': UtilClient.default_number(runtime.connect_timeout, self._connect_timeout),
            'httpProxy': UtilClient.default_string(runtime.http_proxy, self._http_proxy),
            'httpsProxy': UtilClient.default_string(runtime.https_proxy, self._https_proxy),
            'noProxy': UtilClient.default_string(runtime.no_proxy, self._no_proxy),
            'maxIdleConns': UtilClient.default_number(runtime.max_idle_conns, self._max_idle_conns),
            'retry': {
                'retryable': runtime.autoretry,
                'maxAttempts': UtilClient.default_number(runtime.max_attempts, 3)
            },
            'backoff': {
                'policy': UtilClient.default_string(runtime.backoff_policy, 'no'),
                'period': UtilClient.default_number(runtime.backoff_period, 1)
            },
            'ignoreSSL': runtime.ignore_ssl
        }
        _last_request = None
        _last_exception = None
        _now = time.time()
        _retry_times = 0
        while TeaCore.allow_retry(_runtime.get('retry'), _retry_times, _now):
            if _retry_times > 0:
                _backoff_time = TeaCore.get_backoff_time(_runtime.get('backoff'), _retry_times)
                if _backoff_time > 0:
                    TeaCore.sleep(_backoff_time)
            _retry_times = _retry_times + 1
            try:
                _request = TeaRequest()
                _request.protocol = UtilClient.default_string(self._protocol, protocol)
                _request.method = method
                _request.pathname = pathname
                _request.headers = TeaCore.merge({
                    'date': UtilClient.get_date_utcstring(),
                    'host': self._endpoint,
                    'accept': 'application/json',
                    'x-acs-signature-nonce': UtilClient.get_nonce(),
                    'x-acs-signature-method': 'HMAC-SHA1',
                    'x-acs-signature-version': '1.0',
                    'x-acs-version': version,
                    'x-acs-action': action,
                    'user-agent': UtilClient.get_user_agent(self._user_agent)
                }, request.headers)
                if not UtilClient.is_unset(request.body):
                    _request.body = UtilClient.to_jsonstring(request.body)
                    _request.headers['content-type'] = 'application/json; charset=utf-8'
                if not UtilClient.is_unset(request.query):
                    _request.query = request.query
                if not UtilClient.equal_string(auth_type, 'Anonymous'):
                    access_key_id = self.get_access_key_id()
                    access_key_secret = self.get_access_key_secret()
                    security_token = self.get_security_token()
                    if not UtilClient.empty(security_token):
                        _request.headers['x-acs-accesskey-id'] = access_key_id
                        _request.headers['x-acs-security-token'] = security_token
                    string_to_sign = OpenApiUtilClient.get_string_to_sign(_request)
                    _request.headers['authorization'] = 'acs %s:%s' % (TeaConverter.to_unicode(access_key_id), TeaConverter.to_unicode(OpenApiUtilClient.get_roasignature(string_to_sign, access_key_secret)))
                _last_request = _request
                _response = TeaCore.do_action(_request, _runtime)
                if UtilClient.equal_number(_response.status_code, 204):
                    return {
                        'headers': _response.headers
                    }
                if UtilClient.is_4xx(_response.status_code) or UtilClient.is_5xx(_response.status_code):
                    _res = UtilClient.read_as_json(_response.body)
                    err = UtilClient.assert_as_map(_res)
                    raise TeaException({
                        'code': '%s' % TeaConverter.to_unicode(self.default_any(err.get('Code'), err.get('code'))),
                        'message': 'code: %s, %s request id: %s' % (TeaConverter.to_unicode(_response.status_code), TeaConverter.to_unicode(self.default_any(err.get('Message'), err.get('message'))), TeaConverter.to_unicode(self.default_any(err.get('RequestId'), err.get('requestId')))),
                        'data': err
                    })
                if UtilClient.equal_string(body_type, 'binary'):
                    resp = {
                        'body': _response.body,
                        'headers': _response.headers
                    }
                    return resp
                elif UtilClient.equal_string(body_type, 'byte'):
                    byt = UtilClient.read_as_bytes(_response.body)
                    return {
                        'body': byt,
                        'headers': _response.headers
                    }
                elif UtilClient.equal_string(body_type, 'string'):
                    str = UtilClient.read_as_string(_response.body)
                    return {
                        'body': str,
                        'headers': _response.headers
                    }
                elif UtilClient.equal_string(body_type, 'json'):
                    obj = UtilClient.read_as_json(_response.body)
                    res = UtilClient.assert_as_map(obj)
                    return {
                        'body': res,
                        'headers': _response.headers
                    }
                elif UtilClient.equal_string(body_type, 'array'):
                    arr = UtilClient.read_as_json(_response.body)
                    return {
                        'body': arr,
                        'headers': _response.headers
                    }
                else:
                    return {
                        'headers': _response.headers
                    }
            except Exception as e:
                if TeaCore.is_retryable(e):
                    _last_exception = e
                    continue
                raise e
        raise UnretryableException(_last_request, _last_exception)

    def do_roarequest_with_form(self, action, version, protocol, method, auth_type, pathname, body_type, request, runtime):
        """
        Encapsulate the request and invoke the network with form body

        @type action: unicode
        @param action: api name

        @type version: unicode
        @param version: product version

        @type protocol: unicode
        @param protocol: http or https

        @type method: unicode
        @param method: e.g. GET

        @type auth_type: unicode
        @param auth_type: authorization type e.g. AK

        @type pathname: unicode
        @param pathname: pathname of every api

        @type body_type: unicode
        @param body_type: response body type e.g. String

        @param request: object of OpenApiRequest

        @param runtime: which controls some details of call api, such as retry times

        @rtype: dict
        @return: the response
        """
        request.validate()
        runtime.validate()
        _runtime = {
            'timeouted': 'retry',
            'readTimeout': UtilClient.default_number(runtime.read_timeout, self._read_timeout),
            'connectTimeout': UtilClient.default_number(runtime.connect_timeout, self._connect_timeout),
            'httpProxy': UtilClient.default_string(runtime.http_proxy, self._http_proxy),
            'httpsProxy': UtilClient.default_string(runtime.https_proxy, self._https_proxy),
            'noProxy': UtilClient.default_string(runtime.no_proxy, self._no_proxy),
            'maxIdleConns': UtilClient.default_number(runtime.max_idle_conns, self._max_idle_conns),
            'retry': {
                'retryable': runtime.autoretry,
                'maxAttempts': UtilClient.default_number(runtime.max_attempts, 3)
            },
            'backoff': {
                'policy': UtilClient.default_string(runtime.backoff_policy, 'no'),
                'period': UtilClient.default_number(runtime.backoff_period, 1)
            },
            'ignoreSSL': runtime.ignore_ssl
        }
        _last_request = None
        _last_exception = None
        _now = time.time()
        _retry_times = 0
        while TeaCore.allow_retry(_runtime.get('retry'), _retry_times, _now):
            if _retry_times > 0:
                _backoff_time = TeaCore.get_backoff_time(_runtime.get('backoff'), _retry_times)
                if _backoff_time > 0:
                    TeaCore.sleep(_backoff_time)
            _retry_times = _retry_times + 1
            try:
                _request = TeaRequest()
                _request.protocol = UtilClient.default_string(self._protocol, protocol)
                _request.method = method
                _request.pathname = pathname
                _request.headers = TeaCore.merge({
                    'date': UtilClient.get_date_utcstring(),
                    'host': self._endpoint,
                    'accept': 'application/json',
                    'x-acs-signature-nonce': UtilClient.get_nonce(),
                    'x-acs-signature-method': 'HMAC-SHA1',
                    'x-acs-signature-version': '1.0',
                    'x-acs-version': version,
                    'x-acs-action': action,
                    'user-agent': UtilClient.get_user_agent(self._user_agent)
                }, request.headers)
                if not UtilClient.is_unset(request.body):
                    m = UtilClient.assert_as_map(request.body)
                    _request.body = OpenApiUtilClient.to_form(m)
                    _request.headers['content-type'] = 'application/x-www-form-urlencoded'
                if not UtilClient.is_unset(request.query):
                    _request.query = request.query
                if not UtilClient.equal_string(auth_type, 'Anonymous'):
                    access_key_id = self.get_access_key_id()
                    access_key_secret = self.get_access_key_secret()
                    security_token = self.get_security_token()
                    if not UtilClient.empty(security_token):
                        _request.headers['x-acs-accesskey-id'] = access_key_id
                        _request.headers['x-acs-security-token'] = security_token
                    string_to_sign = OpenApiUtilClient.get_string_to_sign(_request)
                    _request.headers['authorization'] = 'acs %s:%s' % (TeaConverter.to_unicode(access_key_id), TeaConverter.to_unicode(OpenApiUtilClient.get_roasignature(string_to_sign, access_key_secret)))
                _last_request = _request
                _response = TeaCore.do_action(_request, _runtime)
                if UtilClient.equal_number(_response.status_code, 204):
                    return {
                        'headers': _response.headers
                    }
                if UtilClient.is_4xx(_response.status_code) or UtilClient.is_5xx(_response.status_code):
                    _res = UtilClient.read_as_json(_response.body)
                    err = UtilClient.assert_as_map(_res)
                    raise TeaException({
                        'code': '%s' % TeaConverter.to_unicode(self.default_any(err.get('Code'), err.get('code'))),
                        'message': 'code: %s, %s request id: %s' % (TeaConverter.to_unicode(_response.status_code), TeaConverter.to_unicode(self.default_any(err.get('Message'), err.get('message'))), TeaConverter.to_unicode(self.default_any(err.get('RequestId'), err.get('requestId')))),
                        'data': err
                    })
                if UtilClient.equal_string(body_type, 'binary'):
                    resp = {
                        'body': _response.body,
                        'headers': _response.headers
                    }
                    return resp
                elif UtilClient.equal_string(body_type, 'byte'):
                    byt = UtilClient.read_as_bytes(_response.body)
                    return {
                        'body': byt,
                        'headers': _response.headers
                    }
                elif UtilClient.equal_string(body_type, 'string'):
                    str = UtilClient.read_as_string(_response.body)
                    return {
                        'body': str,
                        'headers': _response.headers
                    }
                elif UtilClient.equal_string(body_type, 'json'):
                    obj = UtilClient.read_as_json(_response.body)
                    res = UtilClient.assert_as_map(obj)
                    return {
                        'body': res,
                        'headers': _response.headers
                    }
                elif UtilClient.equal_string(body_type, 'array'):
                    arr = UtilClient.read_as_json(_response.body)
                    return {
                        'body': arr,
                        'headers': _response.headers
                    }
                else:
                    return {
                        'headers': _response.headers
                    }
            except Exception as e:
                if TeaCore.is_retryable(e):
                    _last_exception = e
                    continue
                raise e
        raise UnretryableException(_last_request, _last_exception)

    def do_request(self, params, request, runtime):
        """
        Encapsulate the request and invoke the network

        @param action: api name

        @param version: product version

        @param protocol: http or https

        @param method: e.g. GET

        @param auth_type: authorization type e.g. AK

        @param body_type: response body type e.g. String

        @param request: object of OpenApiRequest

        @param runtime: which controls some details of call api, such as retry times

        @rtype: dict
        @return: the response
        """
        params.validate()
        request.validate()
        runtime.validate()
        _runtime = {
            'timeouted': 'retry',
            'readTimeout': UtilClient.default_number(runtime.read_timeout, self._read_timeout),
            'connectTimeout': UtilClient.default_number(runtime.connect_timeout, self._connect_timeout),
            'httpProxy': UtilClient.default_string(runtime.http_proxy, self._http_proxy),
            'httpsProxy': UtilClient.default_string(runtime.https_proxy, self._https_proxy),
            'noProxy': UtilClient.default_string(runtime.no_proxy, self._no_proxy),
            'maxIdleConns': UtilClient.default_number(runtime.max_idle_conns, self._max_idle_conns),
            'retry': {
                'retryable': runtime.autoretry,
                'maxAttempts': UtilClient.default_number(runtime.max_attempts, 3)
            },
            'backoff': {
                'policy': UtilClient.default_string(runtime.backoff_policy, 'no'),
                'period': UtilClient.default_number(runtime.backoff_period, 1)
            },
            'ignoreSSL': runtime.ignore_ssl
        }
        _last_request = None
        _last_exception = None
        _now = time.time()
        _retry_times = 0
        while TeaCore.allow_retry(_runtime.get('retry'), _retry_times, _now):
            if _retry_times > 0:
                _backoff_time = TeaCore.get_backoff_time(_runtime.get('backoff'), _retry_times)
                if _backoff_time > 0:
                    TeaCore.sleep(_backoff_time)
            _retry_times = _retry_times + 1
            try:
                _request = TeaRequest()
                _request.protocol = UtilClient.default_string(self._protocol, params.protocol)
                _request.method = params.method
                _request.pathname = OpenApiUtilClient.get_encode_path(params.pathname)
                _request.query = request.query
                # endpoint is setted in product client
                _request.headers = TeaCore.merge({
                    'host': self._endpoint,
                    'x-acs-version': params.version,
                    'x-acs-action': params.action,
                    'user-agent': self.get_user_agent(),
                    'x-acs-date': OpenApiUtilClient.get_timestamp(),
                    'x-acs-signature-nonce': UtilClient.get_nonce(),
                    'accept': 'application/json'
                }, request.headers)
                signature_algorithm = UtilClient.default_string(self._signature_algorithm, 'ACS3-HMAC-SHA256')
                hashed_request_payload = OpenApiUtilClient.hex_encode(OpenApiUtilClient.hash(UtilClient.to_bytes(''), signature_algorithm))
                if not UtilClient.is_unset(request.body):
                    if UtilClient.equal_string(params.req_body_type, 'json'):
                        json_obj = UtilClient.to_jsonstring(request.body)
                        hashed_request_payload = OpenApiUtilClient.hex_encode(OpenApiUtilClient.hash(UtilClient.to_bytes(json_obj), signature_algorithm))
                        _request.body = json_obj
                    else:
                        m = UtilClient.assert_as_map(request.body)
                        form_obj = OpenApiUtilClient.to_form(m)
                        hashed_request_payload = OpenApiUtilClient.hex_encode(OpenApiUtilClient.hash(UtilClient.to_bytes(form_obj), signature_algorithm))
                        _request.body = form_obj
                        _request.headers['content-type'] = 'application/x-www-form-urlencoded'
                if not UtilClient.is_unset(request.stream):
                    tmp = UtilClient.read_as_bytes(request.stream)
                    hashed_request_payload = OpenApiUtilClient.hex_encode(OpenApiUtilClient.hash(tmp, signature_algorithm))
                    _request.body = tmp
                _request.headers['x-acs-content-sha256'] = hashed_request_payload
                if not UtilClient.equal_string(params.auth_type, 'Anonymous'):
                    access_key_id = self.get_access_key_id()
                    access_key_secret = self.get_access_key_secret()
                    security_token = self.get_security_token()
                    if not UtilClient.empty(security_token):
                        _request.headers['x-acs-security-token'] = security_token
                    _request.headers['Authorization'] = OpenApiUtilClient.get_authorization(_request, signature_algorithm, hashed_request_payload, access_key_id, access_key_secret)
                _last_request = _request
                _response = TeaCore.do_action(_request, _runtime)
                if UtilClient.is_4xx(_response.status_code) or UtilClient.is_5xx(_response.status_code):
                    _res = UtilClient.read_as_json(_response.body)
                    err = UtilClient.assert_as_map(_res)
                    raise TeaException({
                        'code': '%s' % TeaConverter.to_unicode(self.default_any(err.get('Code'), err.get('code'))),
                        'message': 'code: %s, %s request id: %s' % (TeaConverter.to_unicode(_response.status_code), TeaConverter.to_unicode(self.default_any(err.get('Message'), err.get('message'))), TeaConverter.to_unicode(self.default_any(err.get('RequestId'), err.get('requestId')))),
                        'data': err
                    })
                if UtilClient.equal_string(params.body_type, 'binary'):
                    resp = {
                        'body': _response.body,
                        'headers': _response.headers
                    }
                    return resp
                elif UtilClient.equal_string(params.body_type, 'byte'):
                    byt = UtilClient.read_as_bytes(_response.body)
                    return {
                        'body': byt,
                        'headers': _response.headers
                    }
                elif UtilClient.equal_string(params.body_type, 'string'):
                    str = UtilClient.read_as_string(_response.body)
                    return {
                        'body': str,
                        'headers': _response.headers
                    }
                elif UtilClient.equal_string(params.body_type, 'json'):
                    obj = UtilClient.read_as_json(_response.body)
                    res = UtilClient.assert_as_map(obj)
                    return {
                        'body': res,
                        'headers': _response.headers
                    }
                elif UtilClient.equal_string(params.body_type, 'array'):
                    arr = UtilClient.read_as_json(_response.body)
                    return {
                        'body': arr,
                        'headers': _response.headers
                    }
                else:
                    return {
                        'headers': _response.headers
                    }
            except Exception as e:
                if TeaCore.is_retryable(e):
                    _last_exception = e
                    continue
                raise e
        raise UnretryableException(_last_request, _last_exception)

    def call_api(self, params, request, runtime):
        if UtilClient.is_unset(params):
            raise TeaException({
                'code': 'ParameterMissing',
                'message': "'params' can not be unset"
            })
        if UtilClient.is_unset(self._signature_algorithm) or not UtilClient.equal_string(self._signature_algorithm, 'v2'):
            return self.do_request(params, request, runtime)
        elif UtilClient.equal_string(params.style, 'ROA') and UtilClient.equal_string(params.req_body_type, 'json'):
            return self.do_roarequest(params.action, params.version, params.protocol, params.method, params.auth_type, params.pathname, params.body_type, request, runtime)
        elif UtilClient.equal_string(params.style, 'ROA'):
            return self.do_roarequest_with_form(params.action, params.version, params.protocol, params.method, params.auth_type, params.pathname, params.body_type, request, runtime)
        else:
            return self.do_rpcrequest(params.action, params.version, params.protocol, params.method, params.auth_type, params.body_type, request, runtime)

    def get_user_agent(self):
        """
        Get user agent

        @rtype: unicode
        @return: user agent
        """
        user_agent = UtilClient.get_user_agent(self._user_agent)
        return user_agent

    def get_access_key_id(self):
        """
        Get accesskey id by using credential

        @rtype: unicode
        @return: accesskey id
        """
        if UtilClient.is_unset(self._credential):
            return ''
        access_key_id = self._credential.get_access_key_id()
        return access_key_id

    def get_access_key_secret(self):
        """
        Get accesskey secret by using credential

        @rtype: unicode
        @return: accesskey secret
        """
        if UtilClient.is_unset(self._credential):
            return ''
        secret = self._credential.get_access_key_secret()
        return secret

    def get_security_token(self):
        """
        Get security token by using credential

        @rtype: unicode
        @return: security token
        """
        if UtilClient.is_unset(self._credential):
            return ''
        token = self._credential.get_security_token()
        return token

    @staticmethod
    def default_any(input_value, default_value):
        """
        If inputValue is not null, return it or return defaultValue

        @param input_value:  users input value

        @param default_value: default value

        @return: the final result
        """
        if UtilClient.is_unset(input_value):
            return default_value
        return input_value

    def check_config(self, config):
        """
        If the endpointRule and config.endpoint are empty, throw error

        @param config: config contains the necessary information to create a client
        """
        if UtilClient.empty(self._endpoint_rule) and UtilClient.empty(config.endpoint):
            raise TeaException({
                'code': 'ParameterMissing',
                'message': "'config.endpoint' can not be empty"
            })
