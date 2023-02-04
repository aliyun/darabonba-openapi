# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from Tea.model import TeaModel

from alibabacloud_credentials.client import Client as CredentialClient


class GlobalParameters(TeaModel):
    def __init__(self, headers=None, queries=None):
        self.headers = headers  # type: dict[str, str]
        self.queries = queries  # type: dict[str, str]

    def validate(self):
        pass

    def to_map(self):
        _map = super(GlobalParameters, self).to_map()
        if _map is not None:
            return _map

        result = dict()
        if self.headers is not None:
            result['headers'] = self.headers
        if self.queries is not None:
            result['queries'] = self.queries
        return result

    def from_map(self, m=None):
        m = m or dict()
        if m.get('headers') is not None:
            self.headers = m.get('headers')
        if m.get('queries') is not None:
            self.queries = m.get('queries')
        return self


class Config(TeaModel):
    """
    Model for initing client
    """
    def __init__(self, access_key_id=None, access_key_secret=None, security_token=None, protocol=None, method=None,
                 region_id=None, read_timeout=None, connect_timeout=None, http_proxy=None, https_proxy=None, credential=None,
                 endpoint=None, no_proxy=None, max_idle_conns=None, network=None, user_agent=None, suffix=None,
                 socks_5proxy=None, socks_5net_work=None, endpoint_type=None, open_platform_endpoint=None, type=None,
                 signature_version=None, signature_algorithm=None, global_parameters=None, key=None, cert=None, ca=None):
        # accesskey id
        self.access_key_id = access_key_id  # type: str
        # accesskey secret
        self.access_key_secret = access_key_secret  # type: str
        # security token
        self.security_token = security_token  # type: str
        # http protocol
        self.protocol = protocol  # type: str
        # http method
        self.method = method  # type: str
        # region id
        self.region_id = region_id  # type: str
        # read timeout
        self.read_timeout = read_timeout  # type: int
        # connect timeout
        self.connect_timeout = connect_timeout  # type: int
        # http proxy
        self.http_proxy = http_proxy  # type: str
        # https proxy
        self.https_proxy = https_proxy  # type: str
        # credential
        self.credential = credential  # type: CredentialClient
        # endpoint
        self.endpoint = endpoint  # type: str
        # proxy white list
        self.no_proxy = no_proxy  # type: str
        # max idle conns
        self.max_idle_conns = max_idle_conns  # type: int
        # network for endpoint
        self.network = network  # type: str
        # user agent
        self.user_agent = user_agent  # type: str
        # suffix for endpoint
        self.suffix = suffix  # type: str
        # socks5 proxy
        self.socks_5proxy = socks_5proxy  # type: str
        # socks5 network
        self.socks_5net_work = socks_5net_work  # type: str
        # endpoint type
        self.endpoint_type = endpoint_type  # type: str
        # OpenPlatform endpoint
        self.open_platform_endpoint = open_platform_endpoint  # type: str
        # credential type
        self.type = type  # type: str
        # Signature Version
        self.signature_version = signature_version  # type: str
        # Signature Algorithm
        self.signature_algorithm = signature_algorithm  # type: str
        # Global Parameters
        self.global_parameters = global_parameters  # type: GlobalParameters
        # privite key for client certificate
        self.key = key  # type: str
        # client certificate
        self.cert = cert  # type: str
        # server certificate
        self.ca = ca  # type: str

    def validate(self):
        if self.global_parameters:
            self.global_parameters.validate()

    def to_map(self):
        _map = super(Config, self).to_map()
        if _map is not None:
            return _map

        result = dict()
        if self.access_key_id is not None:
            result['accessKeyId'] = self.access_key_id
        if self.access_key_secret is not None:
            result['accessKeySecret'] = self.access_key_secret
        if self.security_token is not None:
            result['securityToken'] = self.security_token
        if self.protocol is not None:
            result['protocol'] = self.protocol
        if self.method is not None:
            result['method'] = self.method
        if self.region_id is not None:
            result['regionId'] = self.region_id
        if self.read_timeout is not None:
            result['readTimeout'] = self.read_timeout
        if self.connect_timeout is not None:
            result['connectTimeout'] = self.connect_timeout
        if self.http_proxy is not None:
            result['httpProxy'] = self.http_proxy
        if self.https_proxy is not None:
            result['httpsProxy'] = self.https_proxy
        if self.credential is not None:
            result['credential'] = self.credential
        if self.endpoint is not None:
            result['endpoint'] = self.endpoint
        if self.no_proxy is not None:
            result['noProxy'] = self.no_proxy
        if self.max_idle_conns is not None:
            result['maxIdleConns'] = self.max_idle_conns
        if self.network is not None:
            result['network'] = self.network
        if self.user_agent is not None:
            result['userAgent'] = self.user_agent
        if self.suffix is not None:
            result['suffix'] = self.suffix
        if self.socks_5proxy is not None:
            result['socks5Proxy'] = self.socks_5proxy
        if self.socks_5net_work is not None:
            result['socks5NetWork'] = self.socks_5net_work
        if self.endpoint_type is not None:
            result['endpointType'] = self.endpoint_type
        if self.open_platform_endpoint is not None:
            result['openPlatformEndpoint'] = self.open_platform_endpoint
        if self.type is not None:
            result['type'] = self.type
        if self.signature_version is not None:
            result['signatureVersion'] = self.signature_version
        if self.signature_algorithm is not None:
            result['signatureAlgorithm'] = self.signature_algorithm
        if self.global_parameters is not None:
            result['globalParameters'] = self.global_parameters.to_map()
        if self.key is not None:
            result['key'] = self.key
        if self.cert is not None:
            result['cert'] = self.cert
        if self.ca is not None:
            result['ca'] = self.ca
        return result

    def from_map(self, m=None):
        m = m or dict()
        if m.get('accessKeyId') is not None:
            self.access_key_id = m.get('accessKeyId')
        if m.get('accessKeySecret') is not None:
            self.access_key_secret = m.get('accessKeySecret')
        if m.get('securityToken') is not None:
            self.security_token = m.get('securityToken')
        if m.get('protocol') is not None:
            self.protocol = m.get('protocol')
        if m.get('method') is not None:
            self.method = m.get('method')
        if m.get('regionId') is not None:
            self.region_id = m.get('regionId')
        if m.get('readTimeout') is not None:
            self.read_timeout = m.get('readTimeout')
        if m.get('connectTimeout') is not None:
            self.connect_timeout = m.get('connectTimeout')
        if m.get('httpProxy') is not None:
            self.http_proxy = m.get('httpProxy')
        if m.get('httpsProxy') is not None:
            self.https_proxy = m.get('httpsProxy')
        if m.get('credential') is not None:
            self.credential = m.get('credential')
        if m.get('endpoint') is not None:
            self.endpoint = m.get('endpoint')
        if m.get('noProxy') is not None:
            self.no_proxy = m.get('noProxy')
        if m.get('maxIdleConns') is not None:
            self.max_idle_conns = m.get('maxIdleConns')
        if m.get('network') is not None:
            self.network = m.get('network')
        if m.get('userAgent') is not None:
            self.user_agent = m.get('userAgent')
        if m.get('suffix') is not None:
            self.suffix = m.get('suffix')
        if m.get('socks5Proxy') is not None:
            self.socks_5proxy = m.get('socks5Proxy')
        if m.get('socks5NetWork') is not None:
            self.socks_5net_work = m.get('socks5NetWork')
        if m.get('endpointType') is not None:
            self.endpoint_type = m.get('endpointType')
        if m.get('openPlatformEndpoint') is not None:
            self.open_platform_endpoint = m.get('openPlatformEndpoint')
        if m.get('type') is not None:
            self.type = m.get('type')
        if m.get('signatureVersion') is not None:
            self.signature_version = m.get('signatureVersion')
        if m.get('signatureAlgorithm') is not None:
            self.signature_algorithm = m.get('signatureAlgorithm')
        if m.get('globalParameters') is not None:
            temp_model = GlobalParameters()
            self.global_parameters = temp_model.from_map(m['globalParameters'])
        if m.get('key') is not None:
            self.key = m.get('key')
        if m.get('cert') is not None:
            self.cert = m.get('cert')
        if m.get('ca') is not None:
            self.ca = m.get('ca')
        return self


class OpenApiRequest(TeaModel):
    def __init__(self, headers=None, query=None, body=None, stream=None, host_map=None, endpoint_override=None):
        self.headers = headers  # type: dict[str, str]
        self.query = query  # type: dict[str, str]
        self.body = body  # type: any
        self.stream = stream  # type: READABLE
        self.host_map = host_map  # type: dict[str, str]
        self.endpoint_override = endpoint_override  # type: str

    def validate(self):
        pass

    def to_map(self):
        _map = super(OpenApiRequest, self).to_map()
        if _map is not None:
            return _map

        result = dict()
        if self.headers is not None:
            result['headers'] = self.headers
        if self.query is not None:
            result['query'] = self.query
        if self.body is not None:
            result['body'] = self.body
        if self.stream is not None:
            result['stream'] = self.stream
        if self.host_map is not None:
            result['hostMap'] = self.host_map
        if self.endpoint_override is not None:
            result['endpointOverride'] = self.endpoint_override
        return result

    def from_map(self, m=None):
        m = m or dict()
        if m.get('headers') is not None:
            self.headers = m.get('headers')
        if m.get('query') is not None:
            self.query = m.get('query')
        if m.get('body') is not None:
            self.body = m.get('body')
        if m.get('stream') is not None:
            self.stream = m.get('stream')
        if m.get('hostMap') is not None:
            self.host_map = m.get('hostMap')
        if m.get('endpointOverride') is not None:
            self.endpoint_override = m.get('endpointOverride')
        return self


class Params(TeaModel):
    def __init__(self, action=None, version=None, protocol=None, pathname=None, method=None, auth_type=None,
                 body_type=None, req_body_type=None, style=None):
        self.action = action  # type: str
        self.version = version  # type: str
        self.protocol = protocol  # type: str
        self.pathname = pathname  # type: str
        self.method = method  # type: str
        self.auth_type = auth_type  # type: str
        self.body_type = body_type  # type: str
        self.req_body_type = req_body_type  # type: str
        self.style = style  # type: str

    def validate(self):
        self.validate_required(self.action, 'action')
        self.validate_required(self.version, 'version')
        self.validate_required(self.protocol, 'protocol')
        self.validate_required(self.pathname, 'pathname')
        self.validate_required(self.method, 'method')
        self.validate_required(self.auth_type, 'auth_type')
        self.validate_required(self.body_type, 'body_type')
        self.validate_required(self.req_body_type, 'req_body_type')

    def to_map(self):
        _map = super(Params, self).to_map()
        if _map is not None:
            return _map

        result = dict()
        if self.action is not None:
            result['action'] = self.action
        if self.version is not None:
            result['version'] = self.version
        if self.protocol is not None:
            result['protocol'] = self.protocol
        if self.pathname is not None:
            result['pathname'] = self.pathname
        if self.method is not None:
            result['method'] = self.method
        if self.auth_type is not None:
            result['authType'] = self.auth_type
        if self.body_type is not None:
            result['bodyType'] = self.body_type
        if self.req_body_type is not None:
            result['reqBodyType'] = self.req_body_type
        if self.style is not None:
            result['style'] = self.style
        return result

    def from_map(self, m=None):
        m = m or dict()
        if m.get('action') is not None:
            self.action = m.get('action')
        if m.get('version') is not None:
            self.version = m.get('version')
        if m.get('protocol') is not None:
            self.protocol = m.get('protocol')
        if m.get('pathname') is not None:
            self.pathname = m.get('pathname')
        if m.get('method') is not None:
            self.method = m.get('method')
        if m.get('authType') is not None:
            self.auth_type = m.get('authType')
        if m.get('bodyType') is not None:
            self.body_type = m.get('bodyType')
        if m.get('reqBodyType') is not None:
            self.req_body_type = m.get('reqBodyType')
        if m.get('style') is not None:
            self.style = m.get('style')
        return self


