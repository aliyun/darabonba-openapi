# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from __future__ import annotations
from darabonba.model import DaraModel 
from typing import Dict, Any, BinaryIO


class OpenApiRequest(DaraModel):
    def __init__(
        self, 
        headers: Dict[str, str] = None,
        query: Dict[str, str] = None,
        body: Any = None,
        stream: BinaryIO = None,
        host_map: Dict[str, str] = None,
        endpoint_override: str = None,
    ):
        self.headers = headers
        self.query = query
        self.body = body
        self.stream = stream
        self.host_map = host_map
        self.endpoint_override = endpoint_override

    def validate(self):
        pass

    def to_map(self):
        _map = super().to_map()
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

    def from_map(self, m: dict = None):
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

