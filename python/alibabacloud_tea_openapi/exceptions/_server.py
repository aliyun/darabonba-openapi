# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from __future__ import annotations

from typing import Dict, Any

from alibabacloud_tea_openapi import exceptions as main_exceptions

class ServerException(main_exceptions.AlibabaCloudException):
    def __init__(
        self, *,
        status_code: int = None,
        code: str = None,
        message: str = None,
        description: str = None,
        request_id: str = None,
        retry_after: int = None,
        data: Dict[str, Any] = None,
        access_denied_detail: Dict[str, Any] = None,
        stack: str = None,
    ):
        super().__init__(
            status_code = status_code,
            code = code,
            message = message,
            description = description,
            request_id = request_id,
            retry_after = retry_after,
            data = data,
            access_denied_detail = access_denied_detail,
            stack = stack,
        )
        self.name = 'ServerException'

