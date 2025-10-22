# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from __future__ import annotations

from typing import Dict, Any

from darabonba.exceptions import ResponseException

class AlibabaCloudException(ResponseException):
    def __init__(
        self, *,
        retry_after: int = None,
        data: Dict[str, Any] = None,
        access_denied_detail: Dict[str, Any] = None,
        stack: str = None,
        status_code: int = None,
        code: str = None,
        message: str = None,
        description: str = None,
        request_id: str = None,
    ):
        super().__init__(
            status_code = status_code,
            retry_after = retry_after,
            description = description,
            data = data,
            access_denied_detail = access_denied_detail,
            message = message,
            code = code,
            stack = stack,
        )
        self.name = 'AlibabaCloudException'
        self.status_code = status_code
        self.code = code
        self.message = message
        self.description = description
        self.request_id = request_id

