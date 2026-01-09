English | [简体中文](README-CN.md)
![](https://aliyunsdk-pages.alicdn.com/icons/AlibabaCloud.svg)

## Alibaba Cloud OpenApi Client for C++

Alibaba Cloud OpenApi C++ 客户端（基于 Tea/Darabonba C++ 核心库），用于调用阿里云各产品的 OpenAPI。

## Requirements

- A C++11 compatible compiler (e.g. GCC 5+, Clang 3.8+, MSVC 2015+).
- CMake 3.5 or newer.
- OpenSSL and libcurl installed on your system.
- Git (optional, used when CMake fetches dependencies automatically).

## Installation

You can build and install this library from source with CMake:

```bash
# In the root directory of this project
mkdir build && cd build
cmake .. -DCMAKE_BUILD_TYPE=Release
cmake --build . --target install
```

By default it will be installed under `alibabacloud_open_api_v2_build` in the project directory.  
If you want to install it to another location, pass `-DCMAKE_INSTALL_PREFIX=/your/path` to `cmake ..`.

After installation, you can use `find_package` in your own CMake project:

```cmake
find_package(alibabacloud_open_api_v2 REQUIRED)

add_executable(example main.cpp)
target_link_libraries(example PRIVATE alibabacloud_open_api_v2::alibabacloud_open_api_v2)
```

## Usage

Below is a minimal example showing how to create a client and send a request:

```cpp
#include <alibabacloud/Openapi.hpp>
#include <alibabacloud/utils/models/Config.hpp>
#include <alibabacloud/utils/models/OpenApiRequest.hpp>
#include <alibabacloud/utils/models/Params.hpp>
#include <darabonba/Runtime.hpp>

using namespace AlibabaCloud::OpenApi;
using namespace AlibabaCloud::OpenApi::Utils::Models;

int main() {
    Config config;
    config.setAccessKeyId("your-access-key-id");
    config.setAccessKeySecret("your-access-key-secret");
    config.setRegionId("cn-hangzhou");
    config.setEndpoint("ecs.aliyuncs.com");
    config.setProtocol("https");
    config.setMethod("POST");

    Client client(config);

    OpenApiRequest request;
    // TODO: set request.query / request.body / request.headers ...

    Params params;
    // TODO: set action, version, path, etc. on params

    Darabonba::RuntimeOptions runtime;
    auto result = client.callApi(params, request, runtime);

    // Handle result ...
    return 0;
}
```

For more configuration options, please refer to the definitions in `include/alibabacloud/utils/models/Config.hpp` and other headers under `include/alibabacloud`.

## Issues

If you encounter any problems while using this library, please submit an Issue in the corresponding code repository.

## References

This project depends on the C++ implementations of Darabonba core and Alibaba Cloud credentials. Please refer to their documentation for more details.

## License
[Apache-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Copyright (c) 2009-present, Alibaba Cloud All rights reserved.