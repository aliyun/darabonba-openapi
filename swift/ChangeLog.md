### 2026-11-24 Version 1.1.2

* Allow any domain as `endpointSuffix`. Do not reject values other than `aliyuncs.com` or `aliyuncs.alibaba`.
* Support OSS PrivateLink file upload.

### 2026-11-05 Version 1.1.1

* Pass `endpointSuffix` from client config, then `ALIBABA_CLOUD_ENDPOINT_SUFFIX`, onto the request attribute map.
* Leave that map unchanged when the caller already set a non-empty `endpointSuffix`.
