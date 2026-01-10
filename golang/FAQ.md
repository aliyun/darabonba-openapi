# Golang SDK FAQ

## 常见问题 / Common Issues

### 1. 未定义的 tea.HttpClient / Undefined tea.HttpClient

**问题描述 / Problem Description:**
编译时出现错误：
```
undefined: tea.HttpClient
```

**原因 / Root Cause:**
使用的 `github.com/alibabacloud-go/tea` 版本过旧，不包含 `HttpClient` 类型。

**解决方案 / Solution:**
更新 tea 依赖到最新版本：
```bash
go get -u github.com/alibabacloud-go/tea@latest
go mod tidy
```

或在 `go.mod` 中指定版本：
```go
require (
    github.com/alibabacloud-go/tea v1.2.0 // 或更高版本
)
```

**相关 Issue:** [#177](https://github.com/aliyun/darabonba-openapi/issues/177)

---

### 2. TLS 证书读取错误 / TLS Certificate Read Error

**问题描述 / Problem Description:**
使用 mTLS 时出现错误：
```
tls: failed to find any PEM data in certificate input
```

**原因 / Root Cause:**
当 `RuntimeOptions` 中的 `Key`、`Cert`、`Ca` 字段为 `nil` 时，`util.DefaultString()` 会将它们转换为空字符串 `""`，而不是保持 `nil`。导致 TLS 配置尝试读取空字符串作为证书内容。

**解决方案 / Solution:**
1. 确保证书字段包含有效的 PEM 数据
2. 如果不需要 mTLS，不要设置这些字段：

```go
import (
    "github.com/alibabacloud-go/tea/tea"
    "github.com/alibabacloud-go/tea-utils/v2/service"
)

// 正确使用 - 只在需要时设置证书
runtime := &util.RuntimeOptions{}
if needMTLS {
    runtime.Key = tea.String(keyContent)
    runtime.Cert = tea.String(certContent)
    runtime.Ca = tea.String(caContent)
}

// 不要这样做
// runtime.Key = tea.String("")  // 会导致错误
```

**相关 Issue:** [#131](https://github.com/aliyun/darabonba-openapi/issues/131)

---

### 3. Context 支持缺失 / Missing Context Support

**问题描述 / Problem Description:**
无法将父 Context 传递给 SDK 请求，导致无法实现超时控制和取消操作。

**场景说明 / Scenario:**
```go
ctx := context.WithTimeout(context.Background(), 30*time.Second)
// 如何将 ctx 传递给 SDK？
resp, err := client.ListPoliciesWithOptions(req, &util.RuntimeOptions{})
```

**当前状态 / Current Status:**
Go SDK 目前不支持直接传递 `context.Context`。

**解决方案 / Workaround:**
使用 `RuntimeOptions` 的超时设置：
```go
import (
    "github.com/alibabacloud-go/tea/tea"
    "github.com/alibabacloud-go/tea-utils/v2/service"
)

runtime := &util.RuntimeOptions{
    ReadTimeout:    tea.Int(30000),    // 30 seconds
    ConnectTimeout: tea.Int(10000),    // 10 seconds
}

resp, err := client.ListPoliciesWithOptions(req, runtime)
```

**相关 Issue:** [#171](https://github.com/aliyun/darabonba-openapi/issues/171)

---

### 4. Swagger 注释解析错误 / Swagger Comment Parse Error

**问题描述 / Problem Description:**
使用 `swag init` 生成 Swagger 文档时出错：
```
ParseComment error: missing required param comment parameters
"config - config contains the necessary information to create a client"
```

**原因 / Root Cause:**
Go SDK 生成的注释格式不符合 swaggo 的严格要求。

**解决方案 / Solution:**
1. 在项目中使用 `.swagignore` 排除 SDK 包：
```
# .swagignore
github.com/alibabacloud-go/darabonba-openapi/v2/client
```

2. 或者更新 swag 到最新版本：
```bash
go install github.com/swaggo/swag/cmd/swag@latest
```

**相关 Issue:** [#221](https://github.com/aliyun/darabonba-openapi/issues/221)

---

### 5. 依赖缺失许可证文件 / Missing License File in Dependency

**问题描述 / Problem Description:**
依赖包 `alibabacloud-gateway-pop` 没有 LICENSE 文件，导致合规检查失败。

**原因 / Root Cause:**
上游依赖包未提供许可证文件。

**当前状态 / Current Status:**
这是一个依赖包的问题，已经向上游反馈。

**解决方案 / Workaround:**
如果你的合规工具允许，可以在配置中明确声明该依赖使用 Apache-2.0 许可证（基于阿里云 SDK 的一般许可证策略）。

**相关 Issue:** [#225](https://github.com/aliyun/darabonba-openapi/issues/225)

---

### 6. SDK 版本选择 / Which SDK to Use

**问题描述 / Problem Description:**
阿里云有多个 Go SDK：
- `github.com/aliyun/alibaba-cloud-sdk-go` (旧版本)
- `github.com/alibabacloud-go/darabonba-openapi/v2` (新版本)

应该使用哪一个？

**建议 / Recommendation:**
- 新项目：使用 `github.com/alibabacloud-go/*` 系列 SDK（基于 Darabonba 生成）
- 旧项目迁移：评估后逐步迁移到新 SDK

**新 SDK 的优势：**
1. 更好的类型安全
2. 自动生成，保持与 API 定义同步
3. 更好的文档和示例
4. 统一的错误处理

**相关 Issue:** [#98](https://github.com/aliyun/darabonba-openapi/issues/98)

---

## 异常类型 / Exception Types

### ClientError
客户端错误：
```go
type ClientError struct {
    *AlibabaCloudError
    AccessDeniedDetail map[string]interface{} `json:"accessDeniedDetail,omitempty"`
}
```

### ServerError
服务器端错误：
```go
type ServerError struct {
    *AlibabaCloudError
}
```

### ThrottlingError
限流错误：
```go
type ThrottlingError struct {
    *AlibabaCloudError
    RetryAfter *int64 `json:"retryAfter,omitempty"`
}
```

### AlibabaCloudError
基础错误类型：
```go
type AlibabaCloudError struct {
    StatusCode  *int    `json:"statusCode,omitempty"`
    Code        *string `json:"code,omitempty"`
    Message     *string `json:"message,omitempty"`
    Description *string `json:"description,omitempty"`
    RequestId   *string `json:"requestId,omitempty"`
}
```

---

## 最佳实践 / Best Practices

### 1. 配置初始化
```go
import (
    openapi "github.com/alibabacloud-go/darabonba-openapi/v2/client"
    "github.com/alibabacloud-go/tea/tea"
)

func CreateClient() (*openapi.Client, error) {
    config := &openapi.Config{
        AccessKeyId:     tea.String("your-access-key-id"),
        AccessKeySecret: tea.String("your-access-key-secret"),
        Endpoint:        tea.String("your-endpoint.aliyuncs.com"),
        RegionId:        tea.String("cn-hangzhou"),
    }
    
    // 设置超时
    config.ReadTimeout = tea.Int(30000)    // 30 seconds
    config.ConnectTimeout = tea.Int(10000) // 10 seconds
    
    return openapi.NewClient(config)
}
```

### 2. 错误处理
```go
import (
    "fmt"
    "github.com/alibabacloud-go/tea/tea"
)

func CallAPI(client *Client) error {
    request := &SomeRequest{
        // ... 设置请求参数
    }
    
    runtime := &util.RuntimeOptions{
        Autoretry:     tea.Bool(true),
        MaxAttempts:   tea.Int(3),
        BackoffPolicy: tea.String("exponential"),
        BackoffPeriod: tea.Int(1),
    }
    
    resp, err := client.SomeMethodWithOptions(request, runtime)
    if err != nil {
        // 检查错误类型
        if _t, ok := err.(*tea.SDKError); ok {
            fmt.Printf("SDK Error: %s\n", tea.StringValue(_t.Message))
            fmt.Printf("Status Code: %d\n", tea.IntValue(_t.StatusCode))
            fmt.Printf("Error Code: %s\n", tea.StringValue(_t.Code))
            return err
        }
        return fmt.Errorf("unexpected error: %w", err)
    }
    
    fmt.Printf("Request ID: %s\n", tea.StringValue(resp.Headers["x-acs-request-id"]))
    return nil
}
```

### 3. 使用指针辅助函数
```go
import "github.com/alibabacloud-go/tea/tea"

// 推荐使用 tea.String, tea.Int 等辅助函数
config := &openapi.Config{
    AccessKeyId: tea.String("your-key"),  // 推荐
    RegionId:    tea.String("cn-hangzhou"),
}

// 而不是
// config.AccessKeyId = &someString  // 容易出错
```

### 4. 重试策略
```go
import (
    "github.com/alibabacloud-go/tea/tea"
    "github.com/alibabacloud-go/tea-utils/v2/service"
)

runtime := &util.RuntimeOptions{
    Autoretry:      tea.Bool(true),
    MaxAttempts:    tea.Int(3),
    BackoffPolicy:  tea.String("exponential"),
    BackoffPeriod:  tea.Int(1),
    ReadTimeout:    tea.Int(30000),
    ConnectTimeout: tea.Int(10000),
}
```

### 5. 代理设置
```go
config := &openapi.Config{
    AccessKeyId:     tea.String("your-key"),
    AccessKeySecret: tea.String("your-secret"),
    HttpProxy:       tea.String("http://proxy.example.com:8080"),
    HttpsProxy:      tea.String("https://proxy.example.com:8443"),
    NoProxy:         tea.String("localhost,127.0.0.1"),
}
```

### 6. 凭证管理
```go
import (
    credential "github.com/alibabacloud-go/credentials-go/credentials"
    openapi "github.com/alibabacloud-go/darabonba-openapi/v2/client"
)

// 使用默认凭证链
cred, err := credential.NewCredential(nil)
if err != nil {
    return nil, err
}

config := &openapi.Config{
    Credential: cred,
    Endpoint:   tea.String("your-endpoint.aliyuncs.com"),
}

// 或者显式指定凭证类型
credConfig := &credential.Config{
    Type:            tea.String("access_key"),
    AccessKeyId:     tea.String("your-key"),
    AccessKeySecret: tea.String("your-secret"),
}
cred, err := credential.NewCredential(credConfig)
```

---

## 调试技巧 / Debugging Tips

### 1. 启用 SDK 日志
```go
import (
    "os"
    "github.com/alibabacloud-go/tea/tea"
)

// 设置环境变量启用调试
os.Setenv("DEBUG", "sdk")

// 或在代码中
tea.SetLogger(os.Stderr)
```

### 2. 查看请求详情
```go
runtime := &util.RuntimeOptions{
    Autoretry: tea.Bool(false), // 禁用重试以查看原始错误
}

resp, err := client.CallApi(params, request, runtime)
if err != nil {
    if sdkErr, ok := err.(*tea.SDKError); ok {
        fmt.Printf("Request: %v\n", sdkErr.Data["request"])
        fmt.Printf("Response: %v\n", sdkErr.Data["response"])
    }
}
```

### 3. 检查网络连接
```go
import (
    "net/http"
    "time"
)

// 测试网络连接
client := &http.Client{
    Timeout: 10 * time.Second,
}
resp, err := client.Get("https://your-endpoint.aliyuncs.com")
if err != nil {
    fmt.Printf("Network error: %v\n", err)
    return
}
defer resp.Body.Close()
fmt.Printf("Status: %d\n", resp.StatusCode)
```

### 4. 验证凭证
```go
cred, err := credential.NewCredential(nil)
if err != nil {
    panic(err)
}

accessKeyId, err := cred.GetAccessKeyId()
if err != nil {
    panic(err)
}
fmt.Printf("Using Access Key ID: %s...\n", tea.StringValue(accessKeyId)[:4])
```

---

## 常见错误代码 / Common Error Codes

- `InvalidAccessKeyId.NotFound`: Access Key ID 不存在
- `InvalidAccessKeySecret`: Access Key Secret 错误  
- `SignatureDoesNotMatch`: 签名不匹配
- `InvalidParameter`: 参数无效
- `MissingParameter`: 缺少必需参数
- `Throttling.User`: 用户级别限流
- `ServiceUnavailable`: 服务暂时不可用
- `InternalError`: 服务端内部错误

---

## 性能优化 / Performance Optimization

### 1. 连接池配置
```go
config := &openapi.Config{
    // ...
    MaxIdleConns: tea.Int(100), // 最大空闲连接数
}
```

### 2. 超时设置
```go
runtime := &util.RuntimeOptions{
    ReadTimeout:    tea.Int(30000),  // 根据实际需求调整
    ConnectTimeout: tea.Int(5000),   // 连接超时通常较短
}
```

### 3. 禁用 HTTP/2（如果需要）
```go
config := &openapi.Config{
    // ...
    DisableHttp2: tea.Bool(true),
}
```

---

## 相关资源 / Related Resources

- [官方文档 / Official Documentation](https://help.aliyun.com/document_detail/README.html)
- [Golang SDK 参考 / Golang SDK Reference](https://github.com/aliyun/darabonba-openapi/tree/master/golang)
- [GitHub Issues](https://github.com/aliyun/darabonba-openapi/issues)
- [Go Package Documentation](https://pkg.go.dev/github.com/alibabacloud-go/darabonba-openapi/v2/client)
