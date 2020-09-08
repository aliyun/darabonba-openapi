# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from Tea.model import TeaModel
from typing import Dict, Any


class Config(TeaModel):
    """
    Model for initing client
    """
    def __init__(self, access_key_id=None, access_key_secret=None, security_token=None, protocol=None,
                 region_id=None, read_timeout=None, connect_timeout=None, http_proxy=None, https_proxy=None, credential=None,
                 endpoint=None, no_proxy=None, max_idle_conns=None, network=None, user_agent=None, suffix=None,
                 socks_5proxy=None, socks_5net_work=None, endpoint_type=None, open_platform_endpoint=None, type=None):
        # accesskey id
        self.access_key_id = access_key_id  # type: str
        # accesskey secret
        self.access_key_secret = access_key_secret  # type: str
        # security token
        self.security_token = security_token  # type: str
        # http protocol
        self.protocol = protocol        # type: str
        # region id
        self.region_id = region_id      # type: str
        # read timeout
        self.read_timeout = read_timeout  # type: int
        # connect timeout
        self.connect_timeout = connect_timeout  # type: int
        # http proxy
        self.http_proxy = http_proxy    # type: str
        # https proxy
        self.https_proxy = https_proxy  # type: str
        # credential
        self.credential = credential
        # endpoint
        self.endpoint = endpoint        # type: str
        # proxy white list
        self.no_proxy = no_proxy        # type: str
        # max idle conns
        self.max_idle_conns = max_idle_conns  # type: int
        # network for endpoint
        self.network = network          # type: str
        # user agent
        self.user_agent = user_agent    # type: str
        # suffix for endpoint
        self.suffix = suffix            # type: str
        # socks5 proxy
        self.socks_5proxy = socks_5proxy  # type: str
        # socks5 network
        self.socks_5net_work = socks_5net_work  # type: str
        # endpoint type
        self.endpoint_type = endpoint_type  # type: str
        # OpenPlatform endpoint
        self.open_platform_endpoint = open_platform_endpoint  # type: str
        # credential type
        self.type = type                # type: str

    def validate(self):
        pass

    def to_map(self):
        result = {}
        result['accessKeyId'] = self.access_key_id
        result['accessKeySecret'] = self.access_key_secret
        result['securityToken'] = self.security_token
        result['protocol'] = self.protocol
        result['regionId'] = self.region_id
        result['readTimeout'] = self.read_timeout
        result['connectTimeout'] = self.connect_timeout
        result['httpProxy'] = self.http_proxy
        result['httpsProxy'] = self.https_proxy
        result['credential'] = self.credential
        result['endpoint'] = self.endpoint
        result['noProxy'] = self.no_proxy
        result['maxIdleConns'] = self.max_idle_conns
        result['network'] = self.network
        result['userAgent'] = self.user_agent
        result['suffix'] = self.suffix
        result['socks5Proxy'] = self.socks_5proxy
        result['socks5NetWork'] = self.socks_5net_work
        result['endpointType'] = self.endpoint_type
        result['openPlatformEndpoint'] = self.open_platform_endpoint
        result['type'] = self.type
        return result

    def from_map(self, map={}):
        self.access_key_id = map.get('accessKeyId')
        self.access_key_secret = map.get('accessKeySecret')
        self.security_token = map.get('securityToken')
        self.protocol = map.get('protocol')
        self.region_id = map.get('regionId')
        self.read_timeout = map.get('readTimeout')
        self.connect_timeout = map.get('connectTimeout')
        self.http_proxy = map.get('httpProxy')
        self.https_proxy = map.get('httpsProxy')
        self.credential = map.get('credential')
        self.endpoint = map.get('endpoint')
        self.no_proxy = map.get('noProxy')
        self.max_idle_conns = map.get('maxIdleConns')
        self.network = map.get('network')
        self.user_agent = map.get('userAgent')
        self.suffix = map.get('suffix')
        self.socks_5proxy = map.get('socks5Proxy')
        self.socks_5net_work = map.get('socks5NetWork')
        self.endpoint_type = map.get('endpointType')
        self.open_platform_endpoint = map.get('openPlatformEndpoint')
        self.type = map.get('type')
        return self


class OpenApiRequest(TeaModel):
    def __init__(self, headers=None, query=None, body=None):
        self.headers = headers          # type: Dict[str, str]
        self.query = query              # type: Dict[str, str]
        self.body = body                # type: Any

    def validate(self):
        pass

    def to_map(self):
        result = {}
        result['headers'] = self.headers
        result['query'] = self.query
        result['body'] = self.body
        return result

    def from_map(self, map={}):
        self.headers = map.get('headers')
        self.query = map.get('query')
        self.body = map.get('body')
        return self
