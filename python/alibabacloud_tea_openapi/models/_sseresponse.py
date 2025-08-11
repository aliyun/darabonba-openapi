# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from __future__ import annotations

from typing import Dict

from darabonba.event import Event as SSEEvent
from darabonba.model import DaraModel



class SSEResponse(DaraModel):
    def __init__(
        self, *,
        headers: Dict[str, str] = None,
        status_code: int = None,
        event: SSEEvent = None,
    ):
        self.headers = headers
        # HTTP Status Code
        self.status_code = status_code
        self.event = event

    def validate(self):
        self.validate_required(self.headers, 'headers')
        self.validate_required(self.status_code, 'status_code')
        self.validate_required(self.event, 'event')

    def to_map(self):
        result = dict()
        _map = super().to_map()
        if _map is not None:
            result = _map
        if self.headers is not None:
            result['headers'] = self.headers

        if self.status_code is not None:
            result['statusCode'] = self.status_code

        if self.event is not None:
            result['event'] = self.event.to_map()

        return result

    def from_map(self, m: dict = None):
        m = m or dict()
        if m.get('headers') is not None:
            self.headers = m.get('headers')

        if m.get('statusCode') is not None:
            self.status_code = m.get('statusCode')

        if m.get('event') is not None:
            temp_model = SSEEvent()
            self.event = temp_model.from_map(m.get('event'))

        return self

