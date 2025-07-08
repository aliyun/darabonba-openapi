# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from darabonba.model import DaraModel
from typing import BinaryIO, List


class SourceModelUrlListObject(DaraModel):
    def __init__(
        self,
        url_object: BinaryIO = None,
    ):
        self.url_object = url_object

    def validate(self):
        pass

    def to_map(self):
        _map = super().to_map()
        if _map is not None:
            return _map

        result = dict()
        if self.url_object is not None:
            result['url'] = self.url_object
        return result

    def from_map(self, m: dict = None):
        m = m or dict()
        if m.get('url') is not None:
            self.url_object = m.get('url')
        return self


class SourceModel(DaraModel):
    def __init__(
        self,
        test: str = None,
        empty: float = None,
        body_object: BinaryIO = None,
        list_object: List[BinaryIO] = None,
        url_list_object: List[SourceModelUrlListObject] = None,
    ):
        self.test = test
        self.empty = empty
        self.body_object = body_object
        self.list_object = list_object
        self.url_list_object = url_list_object

    def validate(self):
        if self.url_list_object:
            for k in self.url_list_object:
                if k:
                    k.validate()

    def to_map(self):
        _map = super().to_map()
        if _map is not None:
            return _map

        result = dict()
        if self.test is not None:
            result['Test'] = self.test
        if self.empty is not None:
            result['empty'] = self.empty
        if self.body_object is not None:
            result['body'] = self.body_object
        if self.list_object is not None:
            result['list'] = self.list_object
        result['urlList'] = []
        if self.url_list_object is not None:
            for k in self.url_list_object:
                result['urlList'].append(k.to_map() if k else None)
        return result

    def from_map(self, m: dict = None):
        m = m or dict()
        if m.get('Test') is not None:
            self.test = m.get('Test')
        if m.get('empty') is not None:
            self.empty = m.get('empty')
        if m.get('body') is not None:
            self.body_object = m.get('body')
        if m.get('list') is not None:
            self.list_object = m.get('list')
        self.url_list_object = []
        if m.get('urlList') is not None:
            for k in m.get('urlList'):
                temp_model = SourceModelUrlListObject()
                self.url_list_object.append(temp_model.from_map(k))
        return self


class TargetModelUrlList(DaraModel):
    def __init__(
        self,
        url: str = None,
    ):
        self.url = url

    def validate(self):
        pass

    def to_map(self):
        _map = super().to_map()
        if _map is not None:
            return _map

        result = dict()
        if self.url is not None:
            result['url'] = self.url
        return result

    def from_map(self, m: dict = None):
        m = m or dict()
        if m.get('url') is not None:
            self.url = m.get('url')
        return self


class TargetModel(DaraModel):
    def __init__(
        self,
        test: str = None,
        empty: float = None,
        body: BinaryIO = None,
        list: List[str] = None,
        url_list: List[TargetModelUrlList] = None,
    ):
        self.test = test
        self.empty = empty
        self.body = body
        self.list = list
        self.url_list = url_list

    def validate(self):
        if self.url_list:
            for k in self.url_list:
                if k:
                    k.validate()

    def to_map(self):
        _map = super().to_map()
        if _map is not None:
            return _map

        result = dict()
        if self.test is not None:
            result['Test'] = self.test
        if self.empty is not None:
            result['empty'] = self.empty
        if self.body is not None:
            result['body'] = self.body
        if self.list is not None:
            result['list'] = self.list
        result['urlList'] = []
        if self.url_list is not None:
            for k in self.url_list:
                result['urlList'].append(k.to_map() if k else None)
        return result

    def from_map(self, m: dict = None):
        m = m or dict()
        if m.get('Test') is not None:
            self.test = m.get('Test')
        if m.get('empty') is not None:
            self.empty = m.get('empty')
        if m.get('body') is not None:
            self.body = m.get('body')
        if m.get('list') is not None:
            self.list = m.get('list')
        self.url_list = []
        if m.get('urlList') is not None:
            for k in m.get('urlList'):
                temp_model = TargetModelUrlList()
                self.url_list.append(temp_model.from_map(k))
        return self