[English](README.md) | 简体中文

![](https://aliyunsdk-pages.alicdn.com/icons/AlibabaCloud.svg)

## Alibaba Cloud OpenApi Client for C++

Alibaba Cloud OpenApi C++ 客户端（基于 Tea/Darabonba C++ 核心库），用于在 C++ 应用中方便地调用阿里云各产品的 OpenAPI。

## 要求

- 支持 C++11 的编译器（例如 GCC 5+、Clang 3.8+、MSVC 2015+ 等）。
- CMake 版本不低于 3.5。
- 已安装 OpenSSL 和 libcurl。
- 建议安装 Git（用于 CMake 自动拉取依赖）。

## 安装

推荐通过 CMake 从源码构建并安装本库：

```bash
# 在本项目根目录下
mkdir build && cd build
cmake .. -DCMAKE_BUILD_TYPE=Release
cmake --build . --target install
```

默认情况下，会安装到项目目录下的 `alibabacloud_open_api_v2_build` 目录。  
如果希望安装到其他位置，可以在 `cmake ..` 时传入 `-DCMAKE_INSTALL_PREFIX=/your/path`。

安装完成后，可以在你自己的 CMake 工程中这样使用：

```cmake
find_package(alibabacloud_open_api_v2 REQUIRED)

add_executable(example main.cpp)
target_link_libraries(example PRIVATE alibabacloud_open_api_v2::alibabacloud_open_api_v2)
```

## 使用示例

下面给出一个简单示例，展示如何创建客户端并发起一次调用：

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
    // TODO: 设置 request.query / request.body / request.headers 等字段

    Params params;
    // TODO: 在 params 中设置 action、version、path 等信息

    Darabonba::RuntimeOptions runtime;
    auto result = client.callApi(params, request, runtime);

    // 处理返回结果 ...
    return 0;
}
```

更多配置项和数据结构可以参考 `include/alibabacloud/utils/models/Config.hpp` 以及 `include/alibabacloud` 目录下的其它头文件。

## 问题

如果你在使用过程中遇到问题，请在本项目对应的代码仓库中提交 Issue。

## 相关

本项目依赖 Darabonba C++ Core 和 Alibaba Cloud Credentials C++ 等库，具体使用方式请参考这些依赖项目的文档。

## 许可证
[Apache-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Copyright (c) 2009-present, Alibaba Cloud All rights reserved.
