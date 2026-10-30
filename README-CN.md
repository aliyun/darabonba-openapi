[English](README.md) | 简体中文
![](https://aliyunsdk-pages.alicdn.com/icons/AlibabaCloud.svg)

# Alibaba Cloud OpenApi Client

[![Golang CI](https://github.com/aliyun/darabonba-openapi/actions/workflows/go.yml/badge.svg)](https://github.com/aliyun/darabonba-openapi/actions/workflows/go.yml)
[![Java CI](https://github.com/aliyun/darabonba-openapi/actions/workflows/java.yml/badge.svg)](https://github.com/aliyun/darabonba-openapi/actions/workflows/java.yml)
[![Python CI](https://github.com/aliyun/darabonba-openapi/actions/workflows/python.yml/badge.svg)](https://github.com/aliyun/darabonba-openapi/actions/workflows/python.yml)
[![TypeScript CI](https://github.com/aliyun/darabonba-openapi/actions/workflows/ts.yml/badge.svg)](https://github.com/aliyun/darabonba-openapi/actions/workflows/ts.yml)
[![codecov](https://codecov.io/gh/aliyun/darabonba-openapi/branch/master/graph/badge.svg)](https://codecov.io/gh/aliyun/darabonba-openapi)

## 签名 V2 兼容性

Java、C#、Python、PHP 和 TypeScript 客户端使用签名 V2 的 AccessKey 和 STS 请求默认采用 HMAC-SHA256。如果服务需要 HMAC-SHA1，可在请求头或全局请求头中将 `x-acs-signature-method` 设置为 `HMAC-SHA1`。RPC 请求也可将查询参数 `SignatureMethod` 设置为 `HMAC-SHA1`，显式请求头优先。签名版本保持不变。

## 问题

[Opening an Issue](https://github.com/aliyun/darabonba-openapi/issues/new)，不符合指南的问题可能会立即关闭。

## 相关

- [Latest Release](https://github.com/aliyun/darabonba-openapi)

## 许可证

[Apache-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Copyright (c) 2009-present, Alibaba Cloud All rights reserved.
