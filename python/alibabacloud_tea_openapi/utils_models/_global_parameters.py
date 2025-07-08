# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from __future__ import annotations
from darabonba.model import DaraModel 
from typing import Dict




class GlobalParameters(DaraModel):
    def __init__(
        self, 
        headers: Dict[str, str] = None,
        queries: Dict[str, str] = None,
    ):
        self.headers = headers
        self.queries = queries

    def validate(self):
        pass

    def to_map(self):
        _map = super().to_map()
        if _map is not None:
            return _map

        result = dict()
        if self.headers is not None:
            result['headers'] = self.headers
        if self.queries is not None:
            result['queries'] = self.queries
        return result

    def from_map(self, m: dict = None):
        m = m or dict()
        if m.get('headers') is not None:
            self.headers = m.get('headers')
        if m.get('queries') is not None:
            self.queries = m.get('queries')
        return self

