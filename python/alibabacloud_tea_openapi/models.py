# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from Tea.model import TeaModel
from typing import Dict, Any

from alibabacloud_credentials.client import Client as CredentialClient


class Config(TeaModel):
    """
    Model for initing client
    """
    def __init__(
        self,
        access_key_id: str = None,
        access_key_secret: str = None,
        security_token: str = None,
        protocol: str = None,
        region_id: str = None,
        read_timeout: int = None,
        connect_timeout: int = None,
        http_proxy: str = None,
        https_proxy: str = None,
        credential: CredentialClient = None,
        endpoint: str = None,
        no_proxy: str = None,
        max_idle_conns: int = None,
        network: str = None,
        user_agent: str = None,
        suffix: str = None,
        socks_5proxy: str = None,
        socks_5net_work: str = None,
        endpoint_type: str = None,
        open_platform_endpoint: str = None,
        type: str = None,
    ):
        # accesskey id
        self.access_key_id = access_key_id
        # accesskey secret
        self.access_key_secret = access_key_secret
        # security token
        self.security_token = security_token
        # http protocol
        self.protocol = protocol
        # region id
        self.region_id = region_id
        # read timeout
        self.read_timeout = read_timeout
        # connect timeout
        self.connect_timeout = connect_timeout
        # http proxy
        self.http_proxy = http_proxy
        # https proxy
        self.https_proxy = https_proxy
        # credential
        self.credential = credential
        # endpoint
        self.endpoint = endpoint
        # proxy white list
        self.no_proxy = no_proxy
        # max idle conns
        self.max_idle_conns = max_idle_conns
        # network for endpoint
        self.network = network
        # user agent
        self.user_agent = user_agent
        # suffix for endpoint
        self.suffix = suffix
        # socks5 proxy
        self.socks_5proxy = socks_5proxy
        # socks5 network
        self.socks_5net_work = socks_5net_work
        # endpoint type
        self.endpoint_type = endpoint_type
        # OpenPlatform endpoint
        self.open_platform_endpoint = open_platform_endpoint
        # credential type
        self.type = type

    def validate(self):
        pass

    def to_map(self):
        result = dict()
        if self.access_key_id is not None:
            result['accessKeyId'] = self.access_key_id
        if self.access_key_secret is not None:
            result['accessKeySecret'] = self.access_key_secret
        if self.security_token is not None:
            result['securityToken'] = self.security_token
        if self.protocol is not None:
            result['protocol'] = self.protocol
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
        return result

    def from_map(self, m: dict = None):
        m = m or dict()
        if m.get('accessKeyId') is not None:
            self.access_key_id = m.get('accessKeyId')
        if m.get('accessKeySecret') is not None:
            self.access_key_secret = m.get('accessKeySecret')
        if m.get('securityToken') is not None:
            self.security_token = m.get('securityToken')
        if m.get('protocol') is not None:
            self.protocol = m.get('protocol')
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
        return self


class OpenApiRequest(TeaModel):
    def __init__(
        self,
        headers: Dict[str, str] = None,
        query: Dict[str, str] = None,
        body: Any = None,
    ):
        self.headers = headers
        self.query = query
        self.body = body

    def validate(self):
        pass

    def to_map(self):
        result = dict()
        if self.headers is not None:
            result['headers'] = self.headers
        if self.query is not None:
            result['query'] = self.query
        if self.body is not None:
            result['body'] = self.body
        return result

    def from_map(self, m: dict = None):
        m = m or dict()
        if m.get('headers') is not None:
            self.headers = m.get('headers')
        if m.get('query') is not None:
            self.query = m.get('query')
        if m.get('body') is not None:
            self.body = m.get('body')
        return self


