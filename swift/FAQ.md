# Swift SDK FAQ

## 常见问题 / Common Issues

### 1. 依赖安装问题 / Dependency Installation Issues

**问题描述 / Problem Description:**
使用 Swift Package Manager (SPM) 或 CocoaPods 安装 SDK 时出现错误。

**解决方案 / Solution:**

**Swift Package Manager:**
```swift
// Package.swift
dependencies: [
    .package(url: "https://github.com/alibabacloud-sdk-swift/darabonba-openapi.git", from: "1.0.0")
]

// 在 target 中添加
.target(
    name: "YourTarget",
    dependencies: [
        .product(name: "AlibabacloudOpenApi", package: "darabonba-openapi")
    ]
)
```

**CocoaPods:**
```ruby
# Podfile
platform :ios, '10.0'
use_frameworks!

target 'YourApp' do
  pod 'AlibabacloudOpenApi', '~> 1.0'
end
```

安装命令：
```bash
# SPM
swift package update

# CocoaPods
pod install
```

---

### 2. 异步调用和回调 / Async Calls and Callbacks

**问题描述 / Problem Description:**
不确定如何正确使用异步 API 和处理回调。

**解决方案 / Solution:**

**使用 async/await (Swift 5.5+):**
```swift
import AlibabacloudOpenApi

func callApiAsync() async throws {
    let config = Config()
    config.accessKeyId = "your-key"
    config.accessKeySecret = "your-secret"
    config.endpoint = "your-endpoint.aliyuncs.com"
    
    let client = try Client(config)
    
    // 使用 async/await
    let response = try await client.callApi(params, request, runtime)
    print("Response: \(response)")
}

// 调用
Task {
    do {
        try await callApiAsync()
    } catch {
        print("Error: \(error)")
    }
}
```

**使用 Completion Handler:**
```swift
func callApiWithCompletion(completion: @escaping (Result<Response, Error>) -> Void) {
    let config = Config()
    config.accessKeyId = "your-key"
    config.accessKeySecret = "your-secret"
    
    do {
        let client = try Client(config)
        
        // 使用回调
        client.callApi(params, request, runtime) { result in
            completion(result)
        }
    } catch {
        completion(.failure(error))
    }
}

// 调用
callApiWithCompletion { result in
    switch result {
    case .success(let response):
        print("Success: \(response)")
    case .failure(let error):
        print("Error: \(error)")
    }
}
```

---

### 3. 内存管理 / Memory Management

**问题描述 / Problem Description:**
长时间运行时出现内存泄漏或循环引用。

**原因 / Root Cause:**
闭包捕获 self 导致循环引用。

**解决方案 / Solution:**
```swift
class ApiManager {
    private let client: Client
    
    init(config: Config) throws {
        self.client = try Client(config)
    }
    
    // 使用 [weak self] 避免循环引用
    func callApiWithCompletion(completion: @escaping (Result<Response, Error>) -> Void) {
        client.callApi(params, request, runtime) { [weak self] result in
            guard let self = self else { return }
            
            switch result {
            case .success(let response):
                self.handleSuccess(response)
            case .failure(let error):
                self.handleError(error)
            }
            
            completion(result)
        }
    }
    
    private func handleSuccess(_ response: Response) {
        // 处理成功响应
    }
    
    private func handleError(_ error: Error) {
        // 处理错误
    }
}
```

---

### 4. 线程和并发 / Threading and Concurrency

**问题描述 / Problem Description:**
在主线程调用网络请求导致 UI 卡顿。

**解决方案 / Solution:**
```swift
import Foundation

// 使用 DispatchQueue
func callApiInBackground() {
    DispatchQueue.global(qos: .background).async {
        do {
            let config = Config()
            config.accessKeyId = "your-key"
            config.accessKeySecret = "your-secret"
            
            let client = try Client(config)
            let response = try client.callApi(params, request, runtime)
            
            // 更新 UI 需要在主线程
            DispatchQueue.main.async {
                self.updateUI(with: response)
            }
        } catch {
            DispatchQueue.main.async {
                self.showError(error)
            }
        }
    }
}

// 使用 async/await (推荐)
func callApiModern() async {
    do {
        let config = Config()
        config.accessKeyId = "your-key"
        config.accessKeySecret = "your-secret"
        
        let client = try Client(config)
        
        // 自动在后台线程执行
        let response = try await client.callApi(params, request, runtime)
        
        // 更新 UI（自动回到主线程）
        await MainActor.run {
            self.updateUI(with: response)
        }
    } catch {
        await MainActor.run {
            self.showError(error)
        }
    }
}
```

---

### 5. SSL/TLS 证书验证 / SSL/TLS Certificate Verification

**问题描述 / Problem Description:**
连接 API 时出现 SSL 证书验证错误。

**解决方案 / Solution:**
```swift
let config = Config()
config.accessKeyId = "your-key"
config.accessKeySecret = "your-secret"
config.endpoint = "your-endpoint.aliyuncs.com"

// 禁用 SSL 验证（仅用于测试）
let runtime = RuntimeOptions()
runtime.ignoreSSL = true

// 或配置自定义证书
config.ca = "/path/to/ca-certificate.pem"
config.cert = "/path/to/client-certificate.pem"
config.key = "/path/to/client-key.pem"
```

---

## 异常处理 / Exception Handling

### 异常类型 / Exception Types

```swift
// 基础异常
public class AlibabaCloudError: Error {
    public var statusCode: Int?
    public var code: String?
    public var message: String?
    public var description: String?
    public var requestId: String?
}

// 客户端异常
public class ClientError: AlibabaCloudError {
    public var accessDeniedDetail: [String: Any]?
}

// 服务器异常
public class ServerError: AlibabaCloudError {
    // 5xx 状态码相关错误
}

// 限流异常
public class ThrottlingError: AlibabaCloudError {
    public var retryAfter: Int64?  // 毫秒
}
```

---

## 最佳实践 / Best Practices

### 1. 配置初始化
```swift
import AlibabacloudOpenApi

class ClientFactory {
    static func createClient() throws -> Client {
        let config = Config()
        
        // 从环境变量或配置文件读取
        config.accessKeyId = ProcessInfo.processInfo.environment["ALIBABA_CLOUD_ACCESS_KEY_ID"]
        config.accessKeySecret = ProcessInfo.processInfo.environment["ALIBABA_CLOUD_ACCESS_KEY_SECRET"]
        config.endpoint = "your-endpoint.aliyuncs.com"
        config.regionId = "cn-hangzhou"
        
        // 设置超时
        config.readTimeout = 30000     // 30 seconds
        config.connectTimeout = 10000  // 10 seconds
        
        return try Client(config)
    }
}
```

### 2. 错误处理
```swift
func callApi() async {
    do {
        let client = try ClientFactory.createClient()
        let response = try await client.callApi(params, request, runtime)
        print("Success: \(response)")
    } catch let error as ThrottlingError {
        print("Rate limited. Retry after: \(error.retryAfter ?? 0)ms")
        // 实现重试逻辑
        if let retryAfter = error.retryAfter {
            try? await Task.sleep(nanoseconds: UInt64(retryAfter) * 1_000_000)
            await callApi()  // 重试
        }
    } catch let error as ClientError {
        print("Client error: \(error.code ?? "") - \(error.message ?? "")")
        if let detail = error.accessDeniedDetail {
            print("Access denied details: \(detail)")
        }
    } catch let error as ServerError {
        print("Server error: \(error.code ?? "") - \(error.message ?? "")")
    } catch let error as AlibabaCloudError {
        print("API error: \(error.message ?? "")")
        print("Status code: \(error.statusCode ?? 0)")
        print("Request ID: \(error.requestId ?? "")")
    } catch {
        print("Unexpected error: \(error)")
    }
}
```

### 3. 使用 RuntimeOptions
```swift
let runtime = RuntimeOptions()
runtime.autoretry = true
runtime.maxAttempts = 3
runtime.backoffPolicy = "exponential"
runtime.backoffPeriod = 1
runtime.readTimeout = 30000
runtime.connectTimeout = 10000

let response = try await client.someMethodWithOptions(request, runtime)
```

### 4. Actor 模式（Swift 5.5+）
```swift
actor ApiClient {
    private let client: Client
    
    init(config: Config) throws {
        self.client = try Client(config)
    }
    
    func callApi(_ request: Request) async throws -> Response {
        return try await client.callApi(params, request, runtime)
    }
}

// 使用
let apiClient = try ApiClient(config: config)
let response = try await apiClient.callApi(request)
```

### 5. 泛型和协议
```swift
protocol ApiRequestProtocol {
    associatedtype ResponseType
    func execute() async throws -> ResponseType
}

struct DescribeInstancesRequest: ApiRequestProtocol {
    typealias ResponseType = DescribeInstancesResponse
    
    let client: Client
    let regionId: String
    
    func execute() async throws -> DescribeInstancesResponse {
        let request = ECSDescribeInstancesRequest()
        request.regionId = regionId
        return try await client.describeInstances(request)
    }
}
```

### 6. 依赖注入
```swift
protocol ApiClientProtocol {
    func callApi(_ request: Request) async throws -> Response
}

class DefaultApiClient: ApiClientProtocol {
    private let client: Client
    
    init(config: Config) throws {
        self.client = try Client(config)
    }
    
    func callApi(_ request: Request) async throws -> Response {
        return try await client.callApi(params, request, runtime)
    }
}

// 在视图模型或服务中使用
class ViewModel {
    private let apiClient: ApiClientProtocol
    
    init(apiClient: ApiClientProtocol) {
        self.apiClient = apiClient
    }
    
    func fetchData() async throws {
        let response = try await apiClient.callApi(request)
        // 处理响应
    }
}
```

---

## 调试技巧 / Debugging Tips

### 1. 启用详细日志
```swift
// 设置环境变量
// ALIBABA_CLOUD_DEBUG=1

#if DEBUG
print("Request: \(request)")
print("Response: \(response)")
#endif
```

### 2. 使用 Instruments
```swift
// 使用 Xcode Instruments 分析性能和内存
// Product -> Profile (Cmd + I)
// 选择 Time Profiler 或 Leaks
```

### 3. 断点和调试
```swift
func callApi() async {
    do {
        let response = try await client.callApi(request)
        debugPrint(response)  // 在这里设置断点
    } catch {
        debugPrint(error)
    }
}
```

### 4. 单元测试
```swift
import XCTest
@testable import YourModule

class ApiClientTests: XCTestCase {
    var client: Client!
    
    override func setUpWithError() throws {
        let config = Config()
        config.accessKeyId = "test-key"
        config.accessKeySecret = "test-secret"
        config.endpoint = "test.aliyuncs.com"
        
        client = try Client(config)
    }
    
    func testCallApi() async throws {
        let request = TestRequest()
        let response = try await client.callApi(request)
        
        XCTAssertNotNil(response)
    }
}
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

## Swift 版本要求 / Swift Version Requirements

| SDK 版本 | Swift 最低版本 | 推荐版本 | 平台支持 |
|---------|---------------|---------|---------|
| 1.0.x   | Swift 5.0     | Swift 5.5+ | iOS 10.0+, macOS 10.12+ |
| 2.0.x   | Swift 5.5     | Swift 5.9+ | iOS 13.0+, macOS 10.15+ |

**支持平台：**
- iOS 10.0+
- macOS 10.12+
- tvOS 10.0+
- watchOS 3.0+
- Linux

---

## 包管理器 / Package Managers

### Swift Package Manager (推荐)
```swift
// Package.swift
dependencies: [
    .package(url: "https://github.com/alibabacloud-sdk-swift/darabonba-openapi.git", 
             from: "1.0.0")
]
```

### CocoaPods
```ruby
pod 'AlibabacloudOpenApi', '~> 1.0'
```

### Carthage
```
github "alibabacloud-sdk-swift/darabonba-openapi" ~> 1.0
```

---

## SwiftUI 集成 / SwiftUI Integration

```swift
import SwiftUI
import AlibabacloudOpenApi

@MainActor
class ViewModel: ObservableObject {
    @Published var instances: [Instance] = []
    @Published var isLoading = false
    @Published var error: String?
    
    private let client: Client
    
    init() throws {
        let config = Config()
        config.accessKeyId = "your-key"
        config.accessKeySecret = "your-secret"
        self.client = try Client(config)
    }
    
    func fetchInstances() async {
        isLoading = true
        error = nil
        
        do {
            let response = try await client.describeInstances(request)
            instances = response.instances
        } catch {
            self.error = error.localizedDescription
        }
        
        isLoading = false
    }
}

struct ContentView: View {
    @StateObject private var viewModel = try! ViewModel()
    
    var body: some View {
        List(viewModel.instances) { instance in
            Text(instance.name)
        }
        .task {
            await viewModel.fetchInstances()
        }
        .overlay {
            if viewModel.isLoading {
                ProgressView()
            }
        }
        .alert("Error", isPresented: .constant(viewModel.error != nil)) {
            Button("OK") {
                viewModel.error = nil
            }
        } message: {
            if let error = viewModel.error {
                Text(error)
            }
        }
    }
}
```

---

## Combine 集成 / Combine Integration

```swift
import Combine
import AlibabacloudOpenApi

class ApiService {
    private let client: Client
    
    init(config: Config) throws {
        self.client = try Client(config)
    }
    
    func callApi(request: Request) -> AnyPublisher<Response, Error> {
        Future { promise in
            Task {
                do {
                    let response = try await self.client.callApi(request)
                    promise(.success(response))
                } catch {
                    promise(.failure(error))
                }
            }
        }
        .eraseToAnyPublisher()
    }
}

// 使用
let cancellable = apiService.callApi(request: request)
    .sink { completion in
        switch completion {
        case .finished:
            print("Completed")
        case .failure(let error):
            print("Error: \(error)")
        }
    } receiveValue: { response in
        print("Response: \(response)")
    }
```

---

## 性能优化 / Performance Optimization

### 1. 连接复用
```swift
// 创建单例客户端
class ApiClientManager {
    static let shared = try! ApiClientManager()
    
    let client: Client
    
    private init() throws {
        let config = Config()
        config.accessKeyId = "your-key"
        config.accessKeySecret = "your-secret"
        config.maxIdleConns = 100
        
        self.client = try Client(config)
    }
}

// 使用
let response = try await ApiClientManager.shared.client.callApi(request)
```

### 2. 批量操作
```swift
func fetchMultipleRegions() async throws -> [Response] {
    let regions = ["cn-hangzhou", "cn-shanghai", "cn-beijing"]
    
    return try await withThrowingTaskGroup(of: Response.self) { group in
        for region in regions {
            group.addTask {
                return try await self.client.describeInstances(regionId: region)
            }
        }
        
        var responses: [Response] = []
        for try await response in group {
            responses.append(response)
        }
        return responses
    }
}
```

---

## 相关资源 / Related Resources

- [官方文档 / Official Documentation](https://help.aliyun.com/document_detail/README.html)
- [Swift SDK 参考 / Swift SDK Reference](https://github.com/aliyun/darabonba-openapi/tree/master/swift)
- [GitHub Issues](https://github.com/aliyun/darabonba-openapi/issues)
- [Swift.org](https://swift.org/)
