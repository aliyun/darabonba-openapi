English | [简体中文](README-CN.md)

![](https://aliyunsdk-pages.alicdn.com/icons/AlibabaCloud.svg)

# Alibaba Cloud OpenApi Client

[![Golang CI](https://github.com/aliyun/darabonba-openapi/actions/workflows/go.yml/badge.svg)](https://github.com/aliyun/darabonba-openapi/actions/workflows/go.yml)
[![Java CI](https://github.com/aliyun/darabonba-openapi/actions/workflows/java.yml/badge.svg)](https://github.com/aliyun/darabonba-openapi/actions/workflows/java.yml)
[![Python CI](https://github.com/aliyun/darabonba-openapi/actions/workflows/python.yml/badge.svg)](https://github.com/aliyun/darabonba-openapi/actions/workflows/python.yml)
[![TypeScript CI](https://github.com/aliyun/darabonba-openapi/actions/workflows/ts.yml/badge.svg)](https://github.com/aliyun/darabonba-openapi/actions/workflows/ts.yml)
[![codecov](https://codecov.io/gh/aliyun/darabonba-openapi/branch/master/graph/badge.svg)](https://codecov.io/gh/aliyun/darabonba-openapi)

## Signature V2 compatibility

In the Java and C# clients, AccessKey and STS requests using signature V2 default to HMAC-SHA256. To use HMAC-SHA1 with a service that requires it, set `x-acs-signature-method` to `HMAC-SHA1` in the request headers or global headers. RPC requests can also set the `SignatureMethod` query parameter to `HMAC-SHA1`; an explicit header takes precedence. The signature version is unchanged.

## Issues

[Opening an Issue](https://github.com/aliyun/darabonba-openapi/issues/new), Issues not conforming to the guidelines may be closed immediately.

## References

- [Latest Release](https://github.com/aliyun/darabonba-openapi)

## License

[Apache-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Copyright (c) 2009-present, Alibaba Cloud All rights reserved.
