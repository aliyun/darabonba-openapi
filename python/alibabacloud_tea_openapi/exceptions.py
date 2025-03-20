# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from __future__ import annotations
from darabonba.exceptions import ResponseException 
from typing import Dict, Any


class AlibabaCloudException(ResponseException):
    def __init__(
        self, 
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

class ClientException(AlibabaCloudException):
    def __init__(
        self, 
        status_code: int = None,
        code: str = None,
        message: str = None,
        description: str = None,
        request_id: str = None,
        retry_after: int = None,
        data: Dict[str, Any] = None,
        stack: str = None,
        access_denied_detail: Dict[str, Any] = None,
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
        self.name = 'ClientException'
        self.access_denied_detail = access_denied_detail

class ServerException(AlibabaCloudException):
    def __init__(
        self, 
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

class ThrottlingException(AlibabaCloudException):
    def __init__(
        self, 
        status_code: int = None,
        code: str = None,
        message: str = None,
        description: str = None,
        request_id: str = None,
        data: Dict[str, Any] = None,
        access_denied_detail: Dict[str, Any] = None,
        stack: str = None,
        retry_after: int = None,
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
        self.name = 'ThrottlingException'
        self.retry_after = retry_after

