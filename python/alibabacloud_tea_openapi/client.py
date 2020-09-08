# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
import time

from Tea.exceptions import TeaException, UnretryableException
from Tea.request import TeaRequest
from Tea.core import TeaCore

from alibabacloud_credentials.client import Client as CredentialClient
from alibabacloud_tea_util.client import Client as UtilClient
from alibabacloud_credentials import models as credential_models
from alibabacloud_openapi_util.client import Client as OpenApiUtilClient


class Client(object):
    """
    This is for OpenApi SDK
    """
    def __init__(self, config, _endpoint=None, _region_id=None, _protocol=None, _user_agent=None, _endpoint_rule=None,
                 _endpoint_map=None, _suffix=None, _read_timeout=None, _connect_timeout=None, _http_proxy=None, _https_proxy=None,
                 _socks_5proxy=None, _socks_5net_work=None, _no_proxy=None, _network=None, _product_id=None, _max_idle_conns=None,
                 _endpoint_type=None, _open_platform_endpoint=None, _credential=None):
        """
        Init client with Config

        @param config: config contains the necessary information to create a client
        """
        self._endpoint = _endpoint      # type: str
        self._region_id = _region_id    # type: str
        self._protocol = _protocol      # type: str
        self._user_agent = _user_agent  # type: str
        self._endpoint_rule = _endpoint_rule  # type: str
        self._endpoint_map = _endpoint_map  # type: dict
        self._suffix = _suffix          # type: str
        self._read_timeout = _read_timeout  # type: int
        self._connect_timeout = _connect_timeout  # type: int
        self._http_proxy = _http_proxy  # type: str
        self._https_proxy = _https_proxy  # type: str
        self._socks_5proxy = _socks_5proxy  # type: str
        self._socks_5net_work = _socks_5net_work  # type: str
        self._no_proxy = _no_proxy      # type: str
        self._network = _network        # type: str
        self._product_id = _product_id  # type: str
        self._max_idle_conns = _max_idle_conns  # type: int
        self._endpoint_type = _endpoint_type  # type: str
        self._open_platform_endpoint = _open_platform_endpoint  # type: str
        self._credential = _credential
        if UtilClient.is_unset(config):
            raise TeaException({
                "code": "ParameterMissing",
                "message": "'config' can not be unset"
            })
        if not UtilClient.empty(config.access_key_id) and not UtilClient.empty(config.access_key_secret):
            if not UtilClient.empty(config.security_token):
                config.type = "sts"
            else:
                config.type = "access_key"
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

    def do_rpcrequest(self, action, version, protocol, method, auth_type, body_type, request, runtime):
        """
        Encapsulate the request and invoke the network

        @type action: str
        @param action: api name

        @type version: str
        @param version: product version

        @type protocol: str
        @param protocol: http or https

        @type method: str
        @param method: e.g. GET

        @type auth_type: str
        @param auth_type: authorization type e.g. AK

        @type body_type: str
        @param body_type: response body type e.g. String

        @param request: object of OpenApiRequest

        @param runtime: which controls some details of call api, such as retry times

        @rtype: dict
        @return: the response
        """
        request.validate()
        runtime.validate()
        _runtime = {
            "timeouted": "retry",
            "readTimeout": UtilClient.default_number(runtime.read_timeout, self._read_timeout),
            "connectTimeout": UtilClient.default_number(runtime.connect_timeout, self._connect_timeout),
            "httpProxy": UtilClient.default_string(runtime.http_proxy, self._http_proxy),
            "httpsProxy": UtilClient.default_string(runtime.https_proxy, self._https_proxy),
            "noProxy": UtilClient.default_string(runtime.no_proxy, self._no_proxy),
            "maxIdleConns": UtilClient.default_number(runtime.max_idle_conns, self._max_idle_conns),
            "retry": {
                "retryable": runtime.autoretry,
                "maxAttempts": UtilClient.default_number(runtime.max_attempts, 3)
            },
            "backoff": {
                "policy": UtilClient.default_string(runtime.backoff_policy, "no"),
                "period": UtilClient.default_number(runtime.backoff_period, 1)
            },
            "ignoreSSL": runtime.ignore_ssl
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
                _request.pathname = "/"
                _request.query = TeaCore.merge({
                    "Action": action,
                    "Format": "json",
                    "Version": version,
                    "Timestamp": OpenApiUtilClient.get_timestamp(),
                    "SignatureNonce": UtilClient.get_nonce()
                }, request.query)
                # endpoint is setted in product client
                _request.headers = {
                    "host": self._endpoint,
                    "x-acs-version": version,
                    "x-acs-action": action,
                    "user-agent": self.get_user_agent()
                }
                if not UtilClient.is_unset(request.body):
                    m = UtilClient.assert_as_map(request.body)
                    tmp = UtilClient.anyify_map_value(OpenApiUtilClient.query(m))
                    _request.body = UtilClient.to_form_string(tmp)
                    _request.headers["content-type"] = "application/x-www-form-urlencoded"
                if not UtilClient.equal_string(auth_type, "Anonymous"):
                    access_key_id = self.get_access_key_id()
                    access_key_secret = self.get_access_key_secret()
                    security_token = self.get_security_token()
                    if not UtilClient.empty(security_token):
                        _request.query["SecurityToken"] = security_token
                    _request.query["SignatureMethod"] = "HMAC-SHA1"
                    _request.query["SignatureVersion"] = "1.0"
                    _request.query["AccessKeyId"] = access_key_id
                    t = None
                    if not UtilClient.is_unset(request.body):
                        t = UtilClient.assert_as_map(request.body)
                    signed_param = TeaCore.merge(_request.query,
                        OpenApiUtilClient.query(t))
                    _request.query["Signature"] = OpenApiUtilClient.get_rpcsignature(signed_param, _request.method, access_key_secret)
                _last_request = _request
                _response = TeaCore.do_action(_request, _runtime)
                if UtilClient.is_4xx(_response.status_code) or UtilClient.is_5xx(_response.status_code):
                    _res = UtilClient.read_as_json(_response.body)
                    err = UtilClient.assert_as_map(_res)
                    raise TeaException({
                        "code": "" + str(self.default_any(err.get('Code'), err.get('code'))) + "Error",
                        "message": "code: " + str(_response.status_code) + ", " + str(self.default_any(err.get('Message'), err.get('message'))) + " requestid: " + str(self.default_any(err.get('RequestId'), err.get('requestId'))) + "",
                        "data": err
                    })
                if UtilClient.equal_string(body_type, "binary"):
                    resp = {
                        "body": _response.body,
                        "headers": _response.headers
                    }
                    return resp
                elif UtilClient.equal_string(body_type, "byte"):
                    byt = UtilClient.read_as_bytes(_response.body)
                    return {
                        "body": byt,
                        "headers": _response.headers
                    }
                elif UtilClient.equal_string(body_type, "string"):
                    str = UtilClient.read_as_string(_response.body)
                    return {
                        "body": str,
                        "headers": _response.headers
                    }
                elif UtilClient.equal_string(body_type, "json"):
                    obj = UtilClient.read_as_json(_response.body)
                    res = UtilClient.assert_as_map(obj)
                    return {
                        "body": res,
                        "headers": _response.headers
                    }
                else:
                    return {
                        "headers": _response.headers
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

        @type action: str
        @param action: api name

        @type version: str
        @param version: product version

        @type protocol: str
        @param protocol: http or https

        @type method: str
        @param method: e.g. GET

        @type auth_type: str
        @param auth_type: authorization type e.g. AK

        @type pathname: str
        @param pathname: pathname of every api

        @type body_type: str
        @param body_type: response body type e.g. String

        @param request: object of OpenApiRequest

        @param runtime: which controls some details of call api, such as retry times

        @rtype: dict
        @return: the response
        """
        request.validate()
        runtime.validate()
        _runtime = {
            "timeouted": "retry",
            "readTimeout": UtilClient.default_number(runtime.read_timeout, self._read_timeout),
            "connectTimeout": UtilClient.default_number(runtime.connect_timeout, self._connect_timeout),
            "httpProxy": UtilClient.default_string(runtime.http_proxy, self._http_proxy),
            "httpsProxy": UtilClient.default_string(runtime.https_proxy, self._https_proxy),
            "noProxy": UtilClient.default_string(runtime.no_proxy, self._no_proxy),
            "maxIdleConns": UtilClient.default_number(runtime.max_idle_conns, self._max_idle_conns),
            "retry": {
                "retryable": runtime.autoretry,
                "maxAttempts": UtilClient.default_number(runtime.max_attempts, 3)
            },
            "backoff": {
                "policy": UtilClient.default_string(runtime.backoff_policy, "no"),
                "period": UtilClient.default_number(runtime.backoff_period, 1)
            },
            "ignoreSSL": runtime.ignore_ssl
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
                    "date": UtilClient.get_date_utcstring(),
                    "host": self._endpoint,
                    "accept": "application/json",
                    "x-acs-signature-nonce": UtilClient.get_nonce(),
                    "x-acs-signature-method": "HMAC-SHA1",
                    "x-acs-signature-version": "1.0",
                    "x-acs-version": version,
                    "x-acs-action": action,
                    "user-agent": UtilClient.get_user_agent(self._user_agent)
                }, request.headers)
                if not UtilClient.is_unset(request.body):
                    _request.body = UtilClient.to_jsonstring(request.body)
                    _request.headers["content-type"] = "application/json; charset=utf-8"
                if not UtilClient.is_unset(request.query):
                    _request.query = request.query
                if not UtilClient.equal_string(auth_type, "Anonymous"):
                    access_key_id = self._credential.get_access_key_id()
                    access_key_secret = self._credential.get_access_key_secret()
                    security_token = self._credential.get_security_token()
                    if not UtilClient.empty(security_token):
                        _request.headers["x-acs-accesskey-id"] = access_key_id
                        _request.headers["x-acs-security-token"] = security_token
                    string_to_sign = OpenApiUtilClient.get_string_to_sign(_request)
                    _request.headers["authorization"] = "acs " + str(access_key_id) + ":" + str(OpenApiUtilClient.get_roasignature(string_to_sign, access_key_secret)) + ""
                _last_request = _request
                _response = TeaCore.do_action(_request, _runtime)
                if UtilClient.equal_number(_response.status_code, 204):
                    return {
                        "headers": _response.headers
                    }
                if UtilClient.is_4xx(_response.status_code) or UtilClient.is_5xx(_response.status_code):
                    _res = UtilClient.read_as_json(_response.body)
                    err = UtilClient.assert_as_map(_res)
                    raise TeaException({
                        "code": "" + str(self.default_any(err.get('Code'), err.get('code'))) + "Error",
                        "message": "code: " + str(_response.status_code) + ", " + str(self.default_any(err.get('Message'), err.get('message'))) + " requestid: " + str(self.default_any(err.get('RequestId'), err.get('requestId'))) + "",
                        "data": err
                    })
                if UtilClient.equal_string(body_type, "binary"):
                    resp = {
                        "body": _response.body,
                        "headers": _response.headers
                    }
                    return resp
                elif UtilClient.equal_string(body_type, "byte"):
                    byt = UtilClient.read_as_bytes(_response.body)
                    return {
                        "body": byt,
                        "headers": _response.headers
                    }
                elif UtilClient.equal_string(body_type, "string"):
                    str = UtilClient.read_as_string(_response.body)
                    return {
                        "body": str,
                        "headers": _response.headers
                    }
                elif UtilClient.equal_string(body_type, "json"):
                    obj = UtilClient.read_as_json(_response.body)
                    res = UtilClient.assert_as_map(obj)
                    return {
                        "body": res,
                        "headers": _response.headers
                    }
                else:
                    return {
                        "headers": _response.headers
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

        @type action: str
        @param action: api name

        @type version: str
        @param version: product version

        @type protocol: str
        @param protocol: http or https

        @type method: str
        @param method: e.g. GET

        @type auth_type: str
        @param auth_type: authorization type e.g. AK

        @type pathname: str
        @param pathname: pathname of every api

        @type body_type: str
        @param body_type: response body type e.g. String

        @param request: object of OpenApiRequest

        @param runtime: which controls some details of call api, such as retry times

        @rtype: dict
        @return: the response
        """
        request.validate()
        runtime.validate()
        _runtime = {
            "timeouted": "retry",
            "readTimeout": UtilClient.default_number(runtime.read_timeout, self._read_timeout),
            "connectTimeout": UtilClient.default_number(runtime.connect_timeout, self._connect_timeout),
            "httpProxy": UtilClient.default_string(runtime.http_proxy, self._http_proxy),
            "httpsProxy": UtilClient.default_string(runtime.https_proxy, self._https_proxy),
            "noProxy": UtilClient.default_string(runtime.no_proxy, self._no_proxy),
            "maxIdleConns": UtilClient.default_number(runtime.max_idle_conns, self._max_idle_conns),
            "retry": {
                "retryable": runtime.autoretry,
                "maxAttempts": UtilClient.default_number(runtime.max_attempts, 3)
            },
            "backoff": {
                "policy": UtilClient.default_string(runtime.backoff_policy, "no"),
                "period": UtilClient.default_number(runtime.backoff_period, 1)
            },
            "ignoreSSL": runtime.ignore_ssl
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
                    "date": UtilClient.get_date_utcstring(),
                    "host": self._endpoint,
                    "accept": "application/json",
                    "x-acs-signature-nonce": UtilClient.get_nonce(),
                    "x-acs-signature-method": "HMAC-SHA1",
                    "x-acs-signature-version": "1.0",
                    "x-acs-version": version,
                    "x-acs-action": action,
                    "user-agent": UtilClient.get_user_agent(self._user_agent)
                }, request.headers)
                if not UtilClient.is_unset(request.body):
                    m = UtilClient.assert_as_map(request.body)
                    _request.body = OpenApiUtilClient.to_form(m)
                    _request.headers["content-type"] = "application/x-www-form-urlencoded"
                if not UtilClient.is_unset(request.query):
                    _request.query = request.query
                if not UtilClient.equal_string(auth_type, "Anonymous"):
                    access_key_id = self._credential.get_access_key_id()
                    access_key_secret = self._credential.get_access_key_secret()
                    security_token = self._credential.get_security_token()
                    if not UtilClient.empty(security_token):
                        _request.headers["x-acs-accesskey-id"] = access_key_id
                        _request.headers["x-acs-security-token"] = security_token
                    string_to_sign = OpenApiUtilClient.get_string_to_sign(_request)
                    _request.headers["authorization"] = "acs " + str(access_key_id) + ":" + str(OpenApiUtilClient.get_roasignature(string_to_sign, access_key_secret)) + ""
                _last_request = _request
                _response = TeaCore.do_action(_request, _runtime)
                if UtilClient.equal_number(_response.status_code, 204):
                    return {
                        "headers": _response.headers
                    }
                if UtilClient.is_4xx(_response.status_code) or UtilClient.is_5xx(_response.status_code):
                    _res = UtilClient.read_as_json(_response.body)
                    err = UtilClient.assert_as_map(_res)
                    raise TeaException({
                        "code": "" + str(self.default_any(err.get('Code'), err.get('code'))) + "Error",
                        "message": "code: " + str(_response.status_code) + ", " + str(self.default_any(err.get('Message'), err.get('message'))) + " requestid: " + str(self.default_any(err.get('RequestId'), err.get('requestId'))) + "",
                        "data": err
                    })
                if UtilClient.equal_string(body_type, "binary"):
                    resp = {
                        "body": _response.body,
                        "headers": _response.headers
                    }
                    return resp
                elif UtilClient.equal_string(body_type, "byte"):
                    byt = UtilClient.read_as_bytes(_response.body)
                    return {
                        "body": byt,
                        "headers": _response.headers
                    }
                elif UtilClient.equal_string(body_type, "string"):
                    str = UtilClient.read_as_string(_response.body)
                    return {
                        "body": str,
                        "headers": _response.headers
                    }
                elif UtilClient.equal_string(body_type, "json"):
                    obj = UtilClient.read_as_json(_response.body)
                    res = UtilClient.assert_as_map(obj)
                    return {
                        "body": res,
                        "headers": _response.headers
                    }
                else:
                    return {
                        "headers": _response.headers
                    }
            except Exception as e:
                if TeaCore.is_retryable(e):
                    _last_exception = e
                    continue
                raise e
        raise UnretryableException(_last_request, _last_exception)

    def get_user_agent(self):
        """
        Get user agent

        @rtype: str
        @return: user agent
        """
        user_agent = UtilClient.get_user_agent(self._user_agent)
        return user_agent

    def get_access_key_id(self):
        """
        Get accesskey id by using credential

        @rtype: str
        @return: accesskey id
        """
        if UtilClient.is_unset(self._credential):
            return ''
        access_key_id = self._credential.get_access_key_id()
        return access_key_id

    def get_access_key_secret(self):
        """
        Get accesskey secret by using credential

        @rtype: str
        @return: accesskey secret
        """
        if UtilClient.is_unset(self._credential):
            return ''
        secret = self._credential.get_access_key_secret()
        return secret

    def get_security_token(self):
        """
        Get security token by using credential

        @rtype: str
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
                "code": "ParameterMissing",
                "message": "'config.endpoint' can not be empty"
            })
