# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from __future__ import annotations
from darabonba.model import DaraModel 


class Params(DaraModel):
    def __init__(
        self, 
        action: str = None,
        version: str = None,
        protocol: str = None,
        pathname: str = None,
        method: str = None,
        auth_type: str = None,
        body_type: str = None,
        req_body_type: str = None,
        style: str = None,
    ):
        self.action = action
        self.version = version
        self.protocol = protocol
        self.pathname = pathname
        self.method = method
        self.auth_type = auth_type
        self.body_type = body_type
        self.req_body_type = req_body_type
        self.style = style

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
        _map = super().to_map()
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

    def from_map(self, m: dict = None):
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

