### 2026-11-05 Version 0.3.20

* Pass `endpointSuffix` from client config, then `ALIBABA_CLOUD_ENDPOINT_SUFFIX`, onto the request attribute map.
* Leave that map unchanged when the caller already set a non-empty `endpointSuffix`.
