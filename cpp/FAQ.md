# C++ SDK FAQ

## 常见问题 / Common Issues

### 1. 编译错误 / Compilation Errors

**问题描述 / Problem Description:**
编译时出现头文件找不到或链接错误。

**原因 / Root Cause:**
1. 未正确配置 CMake
2. 依赖库未安装
3. 编译器版本不兼容

**解决方案 / Solution:**
```bash
# 安装依赖
# Ubuntu/Debian
sudo apt-get install libcurl4-openssl-dev libssl-dev cmake

# CentOS/RHEL
sudo yum install libcurl-devel openssl-devel cmake

# macOS
brew install curl openssl cmake

# 配置 CMake
mkdir build && cd build
cmake .. -DCMAKE_BUILD_TYPE=Release
make
sudo make install
```

**CMakeLists.txt 配置:**
```cmake
cmake_minimum_required(VERSION 3.10)
project(YourProject)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# 查找 OpenAPI SDK
find_package(alibabacloud_darabonba_openapi REQUIRED)

add_executable(your_app main.cpp)
target_link_libraries(your_app alibabacloud_darabonba_openapi::alibabacloud_darabonba_openapi)
```

---

### 2. 内存泄漏 / Memory Leaks

**问题描述 / Problem Description:**
长时间运行后内存占用持续增长。

**原因 / Root Cause:**
1. 未正确释放资源
2. 智能指针使用不当
3. 回调函数中的循环引用

**解决方案 / Solution:**
```cpp
#include <alibabacloud/open_api/open_api.hpp>
#include <memory>

// 使用智能指针管理资源
void callApi() {
    auto config = std::make_shared<Alibabacloud::OpenApi::Config>();
    config->accessKeyId = "your-key";
    config->accessKeySecret = "your-secret";
    config->endpoint = "your-endpoint.aliyuncs.com";
    
    auto client = std::make_shared<Alibabacloud::OpenApi::Client>(config);
    
    // 使用 RAII 确保资源自动释放
    // 不需要手动 delete
}

// 使用 Valgrind 检测内存泄漏
// valgrind --leak-check=full ./your_app
```

---

### 3. 线程安全问题 / Thread Safety Issues

**问题描述 / Problem Description:**
多线程环境下出现崩溃或数据不一致。

**原因 / Root Cause:**
客户端对象在多线程间共享但未加锁保护。

**解决方案 / Solution:**
```cpp
#include <mutex>
#include <thread>

class ThreadSafeClient {
private:
    std::shared_ptr<Alibabacloud::OpenApi::Client> client;
    std::mutex mutex;
    
public:
    ThreadSafeClient(std::shared_ptr<Alibabacloud::OpenApi::Config> config) {
        client = std::make_shared<Alibabacloud::OpenApi::Client>(config);
    }
    
    void callApi() {
        std::lock_guard<std::mutex> lock(mutex);
        // 调用 API
    }
};

// 或者为每个线程创建独立的客户端实例
void workerThread(std::shared_ptr<Alibabacloud::OpenApi::Config> config) {
    auto client = std::make_shared<Alibabacloud::OpenApi::Client>(config);
    // 使用 client
}
```

---

### 4. SSL/TLS 证书验证失败 / SSL/TLS Certificate Verification Failed

**问题描述 / Problem Description:**
连接 API 时出现 SSL 证书验证错误。

**原因 / Root Cause:**
1. 系统 CA 证书不完整
2. OpenSSL 版本过旧
3. 证书路径配置不正确

**解决方案 / Solution:**
```cpp
auto config = std::make_shared<Alibabacloud::OpenApi::Config>();
config->accessKeyId = "your-key";
config->accessKeySecret = "your-secret";
config->endpoint = "your-endpoint.aliyuncs.com";

// 方案 1: 指定 CA 证书路径
config->ca = "/etc/ssl/certs/ca-certificates.crt";

// 方案 2: 禁用 SSL 验证（仅用于测试）
// 注意：生产环境不推荐
auto runtime = std::make_shared<Alibabacloud::OpenApi::RuntimeOptions>();
runtime->ignoreSSL = true;

// 方案 3: 更新系统 CA 证书
// Ubuntu: sudo apt-get install ca-certificates
// CentOS: sudo yum install ca-certificates
```

---

### 5. 字符编码问题 / Character Encoding Issues

**问题描述 / Problem Description:**
中文参数或响应内容出现乱码。

**原因 / Root Cause:**
字符串编码不是 UTF-8。

**解决方案 / Solution:**
```cpp
#include <string>
#include <codecvt>
#include <locale>

// 确保字符串是 UTF-8 编码
std::string toUtf8(const std::wstring& wstr) {
    std::wstring_convert<std::codecvt_utf8<wchar_t>> converter;
    return converter.to_bytes(wstr);
}

std::wstring fromUtf8(const std::string& str) {
    std::wstring_convert<std::codecvt_utf8<wchar_t>> converter;
    return converter.from_bytes(str);
}

// 使用
std::string chineseText = u8"中文内容";
// 确保源文件以 UTF-8 编码保存
```

---

## 异常处理 / Exception Handling

### 异常类型 / Exception Types

```cpp
namespace Alibabacloud {
namespace OpenApi {

// 基础异常类
class Exception : public std::exception {
public:
    int statusCode;
    std::string code;
    std::string message;
    std::string description;
    std::string requestId;
    
    const char* what() const noexcept override {
        return message.c_str();
    }
};

// 客户端异常
class ClientException : public Exception {
public:
    std::map<std::string, std::string> accessDeniedDetail;
};

// 服务器异常
class ServerException : public Exception {
};

// 限流异常
class ThrottlingException : public Exception {
public:
    long retryAfter;  // 毫秒
};

}  // namespace OpenApi
}  // namespace Alibabacloud
```

---

## 最佳实践 / Best Practices

### 1. 配置初始化
```cpp
#include <alibabacloud/open_api/open_api.hpp>
#include <memory>

std::shared_ptr<Alibabacloud::OpenApi::Client> createClient() {
    auto config = std::make_shared<Alibabacloud::OpenApi::Config>();
    
    // 从环境变量读取
    config->accessKeyId = std::getenv("ALIBABA_CLOUD_ACCESS_KEY_ID");
    config->accessKeySecret = std::getenv("ALIBABA_CLOUD_ACCESS_KEY_SECRET");
    config->endpoint = "your-endpoint.aliyuncs.com";
    config->regionId = "cn-hangzhou";
    
    // 设置超时
    config->readTimeout = 30000;     // 30 seconds
    config->connectTimeout = 10000;  // 10 seconds
    
    return std::make_shared<Alibabacloud::OpenApi::Client>(config);
}
```

### 2. 异常处理
```cpp
#include <iostream>

try {
    auto response = client->callApi(params, request, runtime);
    std::cout << "Success: " << response << std::endl;
}
catch (const Alibabacloud::OpenApi::ThrottlingException& e) {
    std::cerr << "Rate limited. Retry after: " << e.retryAfter << "ms" << std::endl;
    // 实现重试逻辑
    std::this_thread::sleep_for(std::chrono::milliseconds(e.retryAfter));
}
catch (const Alibabacloud::OpenApi::ClientException& e) {
    std::cerr << "Client error: " << e.code << " - " << e.message << std::endl;
    if (!e.accessDeniedDetail.empty()) {
        for (const auto& [key, value] : e.accessDeniedDetail) {
            std::cerr << key << ": " << value << std::endl;
        }
    }
}
catch (const Alibabacloud::OpenApi::ServerException& e) {
    std::cerr << "Server error: " << e.code << " - " << e.message << std::endl;
}
catch (const Alibabacloud::OpenApi::Exception& e) {
    std::cerr << "API error: " << e.what() << std::endl;
    std::cerr << "Status code: " << e.statusCode << std::endl;
    std::cerr << "Request ID: " << e.requestId << std::endl;
}
catch (const std::exception& e) {
    std::cerr << "Unexpected error: " << e.what() << std::endl;
}
```

### 3. RAII 资源管理
```cpp
class ApiClientManager {
private:
    std::shared_ptr<Alibabacloud::OpenApi::Client> client;
    
public:
    ApiClientManager(std::shared_ptr<Alibabacloud::OpenApi::Config> config) {
        client = std::make_shared<Alibabacloud::OpenApi::Client>(config);
    }
    
    ~ApiClientManager() {
        // 资源自动清理
    }
    
    auto getClient() const {
        return client;
    }
};

// 使用
{
    ApiClientManager manager(config);
    auto client = manager.getClient();
    // 使用 client
}  // 自动清理
```

### 4. 异步调用
```cpp
#include <future>
#include <vector>

// 使用 std::async 实现异步调用
std::future<Response> callApiAsync(
    std::shared_ptr<Client> client,
    const Request& request) {
    
    return std::async(std::launch::async, [client, request]() {
        return client->callApi(request);
    });
}

// 并行调用多个 API
std::vector<std::future<Response>> futures;
for (const auto& request : requests) {
    futures.push_back(callApiAsync(client, request));
}

// 等待所有调用完成
for (auto& future : futures) {
    try {
        auto response = future.get();
        // 处理响应
    } catch (const std::exception& e) {
        std::cerr << "Error: " << e.what() << std::endl;
    }
}
```

### 5. 代理配置
```cpp
auto config = std::make_shared<Alibabacloud::OpenApi::Config>();
config->accessKeyId = "your-key";
config->accessKeySecret = "your-secret";
config->httpProxy = "http://proxy.example.com:8080";
config->httpsProxy = "https://proxy.example.com:8443";
config->noProxy = "localhost,127.0.0.1";
```

---

## 调试技巧 / Debugging Tips

### 1. 启用详细日志
```cpp
// 使用环境变量
// export ALIBABA_CLOUD_DEBUG=1

// 或在代码中
#ifdef DEBUG
    std::cout << "Request: " << request << std::endl;
    std::cout << "Response: " << response << std::endl;
#endif
```

### 2. 使用 GDB 调试
```bash
# 编译时启用调试符号
cmake .. -DCMAKE_BUILD_TYPE=Debug
make

# 使用 GDB
gdb ./your_app
(gdb) break main
(gdb) run
(gdb) bt  # 查看堆栈
(gdb) print variable  # 查看变量值
```

### 3. 内存检查
```bash
# 使用 Valgrind
valgrind --leak-check=full --show-leak-kinds=all ./your_app

# 使用 AddressSanitizer
cmake .. -DCMAKE_CXX_FLAGS="-fsanitize=address -g"
make
./your_app
```

### 4. 性能分析
```bash
# 使用 gprof
cmake .. -DCMAKE_CXX_FLAGS="-pg"
make
./your_app
gprof your_app gmon.out > analysis.txt

# 使用 perf
perf record ./your_app
perf report
```

---

## 常见错误代码 / Common Error Codes

- `InvalidAccessKeyId.NotFound`: Access Key ID 不存在
- `InvalidAccessKeySecret`: Access Key Secret 错误
- `SignatureDoesNotMatch`: 签名不匹配
- `MissingParameter`: 缺少必需参数
- `InvalidParameter`: 参数无效
- `Throttling.User`: 用户级别限流
- `ServiceUnavailable`: 服务暂时不可用
- `InternalError`: 服务端内部错误

---

## C++ 版本要求 / C++ Version Requirements

- **最低版本**: C++11
- **推荐版本**: C++17 或更高
- **编译器支持**:
  - GCC 5.0+
  - Clang 3.4+
  - MSVC 2015+

---

## 依赖库 / Dependencies

### 必需依赖 / Required Dependencies
- libcurl >= 7.50
- OpenSSL >= 1.0.2
- CMake >= 3.10

### 可选依赖 / Optional Dependencies
- Google Test (用于单元测试)
- Boost (某些功能可能需要)

---

## 编译选项 / Compile Options

### Debug 模式
```cmake
cmake .. -DCMAKE_BUILD_TYPE=Debug \
         -DENABLE_TESTING=ON \
         -DENABLE_COVERAGE=ON
```

### Release 模式
```cmake
cmake .. -DCMAKE_BUILD_TYPE=Release \
         -DCMAKE_CXX_FLAGS="-O3 -DNDEBUG"
```

### 静态链接
```cmake
cmake .. -DBUILD_SHARED_LIBS=OFF
```

---

## 性能优化 / Performance Optimization

### 1. 连接复用
```cpp
// 创建长期存在的客户端实例
static std::shared_ptr<Client> globalClient = createClient();

// 重用连接
auto response = globalClient->callApi(request);
```

### 2. 批量操作
```cpp
#include <thread>
#include <vector>

// 使用线程池处理批量请求
const int THREAD_COUNT = 10;
std::vector<std::thread> threads;

for (int i = 0; i < THREAD_COUNT; ++i) {
    threads.emplace_back([&requests, i]() {
        auto client = createClient();
        for (size_t j = i; j < requests.size(); j += THREAD_COUNT) {
            client->callApi(requests[j]);
        }
    });
}

for (auto& thread : threads) {
    thread.join();
}
```

### 3. 编译优化
```cmake
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -O3 -march=native")
```

---

## 相关资源 / Related Resources

- [官方文档 / Official Documentation](https://help.aliyun.com/document_detail/README.html)
- [C++ SDK 参考 / C++ SDK Reference](https://github.com/aliyun/darabonba-openapi/tree/master/cpp)
- [GitHub Issues](https://github.com/aliyun/darabonba-openapi/issues)
- [CMake Documentation](https://cmake.org/documentation/)
