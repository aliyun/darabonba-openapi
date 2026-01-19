# C# SDK FAQ

## 常见问题 / Common Issues

### 1. NameInMap 序列化问题 / NameInMap Serialization Issue

**问题描述 / Problem Description:**
在生成 JSON body 时，嵌套对象的属性名没有按照 `[NameInMap("")]` 特性进行正确转换，导致服务端接收到的 JSON 中属性名为大驼峰格式而非预期格式。

**场景示例 / Example Scenario:**
```csharp
public class NetworkParameters : TeaModel {
    [NameInMap("networkType")]
    public string NetworkType { get; set; }  // 期望序列化为 "networkType"
    
    [NameInMap("securityGroupId")]
    public string SecurityGroupId { get; set; }  // 期望序列化为 "securityGroupId"
}

// 实际序列化结果：
// { "NetworkType": "...", "SecurityGroupId": "..." }  // 错误
// 期望结果：
// { "networkType": "...", "securityGroupId": "..." }  // 正确
```

**原因 / Root Cause:**
嵌套对象在序列化时没有递归应用 `NameInMap` 转换规则。

**解决方案 / Solution:**
等待官方修复。临时解决方案可以自定义 JSON 序列化器：

```csharp
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// 创建自定义契约解析器
public class NameInMapContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(
        MemberInfo member, 
        MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        
        var nameInMapAttr = member.GetCustomAttribute<NameInMapAttribute>();
        if (nameInMapAttr != null)
        {
            property.PropertyName = nameInMapAttr.Name;
        }
        
        return property;
    }
}

// 使用自定义序列化设置
var settings = new JsonSerializerSettings
{
    ContractResolver = new NameInMapContractResolver()
};
string json = JsonConvert.SerializeObject(request, settings);
```

**相关 Issue:** [#169](https://github.com/aliyun/darabonba-openapi/issues/169)

---

### 2. Tea 包源码位置 / Tea Package Source Location

**问题描述 / Problem Description:**
需要查看或调试 `Tea` 包（例如 `Tea` Version 1.0.11）的源代码。

**解决方案 / Solution:**
Tea 包的源代码位于以下仓库：
- GitHub: https://github.com/aliyun/tea-csharp
- NuGet: https://www.nuget.org/packages/Tea/

```bash
# 通过 NuGet 安装
dotnet add package Tea

# 查看本地缓存的包
# Windows: %USERPROFILE%\.nuget\packages\tea\
# Linux/Mac: ~/.nuget/packages/tea/
```

**相关 Issue:** [#115](https://github.com/aliyun/darabonba-openapi/issues/115)

---

### 3. SDK 用法与文档不一致 / SDK Usage Different from Documentation

**问题描述 / Problem Description:**
SDK 的实际用法与官方文档描述不一致，导致集成困难。

**解决方案 / Solution:**
参考以下最新的使用方式：

```csharp
using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.ECS20140526;
using AlibabaCloud.SDK.ECS20140526.Models;

// 1. 配置初始化
var config = new Config
{
    AccessKeyId = Environment.GetEnvironmentVariable("ALIBABA_CLOUD_ACCESS_KEY_ID"),
    AccessKeySecret = Environment.GetEnvironmentVariable("ALIBABA_CLOUD_ACCESS_KEY_SECRET"),
    Endpoint = "ecs.aliyuncs.com",
    RegionId = "cn-hangzhou"
};

// 2. 创建客户端
var client = new Client(config);

// 3. 构建请求
var request = new DescribeInstancesRequest
{
    RegionId = "cn-hangzhou",
    PageSize = 10
};

// 4. 调用 API
var response = await client.DescribeInstancesAsync(request);
```

**相关 Issue:** [#155](https://github.com/aliyun/darabonba-openapi/issues/155), [#89](https://github.com/aliyun/darabonba-openapi/issues/89)

---

## 异常类型 / Exception Types

### AlibabaCloudException
基础异常类：
```csharp
public class AlibabaCloudException : Exception
{
    public int? StatusCode { get; set; }
    public string Code { get; set; }
    public string Message { get; set; }
    public string Description { get; set; }
    public string RequestId { get; set; }
}
```

### ClientException
客户端异常：
```csharp
public class ClientException : AlibabaCloudException
{
    public Dictionary<string, object> AccessDeniedDetail { get; set; }
}
```

### ServerException
服务器异常：
```csharp
public class ServerException : AlibabaCloudException
{
    // 5xx 状态码相关错误
}
```

### ThrottlingException
限流异常：
```csharp
public class ThrottlingException : AlibabaCloudException
{
    public long? RetryAfter { get; set; }  // 重试等待时间（毫秒）
}
```

---

## 最佳实践 / Best Practices

### 1. 配置初始化
```csharp
using AlibabaCloud.OpenApiClient.Models;

var config = new Config
{
    AccessKeyId = "your-access-key-id",
    AccessKeySecret = "your-access-key-secret",
    Endpoint = "your-endpoint.aliyuncs.com",
    RegionId = "cn-hangzhou",
    ReadTimeout = 30000,     // 30 seconds
    ConnectTimeout = 10000   // 10 seconds
};
```

### 2. 异常处理
```csharp
using AlibabaCloud.OpenApiClient.Exceptions;

try
{
    var response = await client.CallApiAsync(params, request, runtime);
    Console.WriteLine($"Success: {response}");
}
catch (ThrottlingException ex)
{
    Console.WriteLine($"Rate limited. Retry after: {ex.RetryAfter}ms");
    // 实现重试逻辑
    await Task.Delay((int)ex.RetryAfter.GetValueOrDefault());
}
catch (ClientException ex)
{
    Console.WriteLine($"Client error: {ex.Code} - {ex.Message}");
    if (ex.AccessDeniedDetail != null)
    {
        foreach (var detail in ex.AccessDeniedDetail)
        {
            Console.WriteLine($"{detail.Key}: {detail.Value}");
        }
    }
}
catch (ServerException ex)
{
    Console.WriteLine($"Server error: {ex.Code} - {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

### 3. 使用 RuntimeOptions
```csharp
using Tea;

var runtime = new TeaModel.RuntimeOptions
{
    Autoretry = true,
    MaxAttempts = 3,
    BackoffPolicy = "exponential",
    BackoffPeriod = 1,
    ReadTimeout = 30000,
    ConnectTimeout = 10000
};

var response = await client.SomeMethodWithOptionsAsync(request, runtime);
```

### 4. 异步编程
```csharp
// 推荐使用异步方法
public async Task<DescribeInstancesResponse> GetInstancesAsync()
{
    var request = new DescribeInstancesRequest
    {
        RegionId = "cn-hangzhou"
    };
    
    var response = await client.DescribeInstancesAsync(request);
    return response;
}

// 处理多个并发请求
public async Task<List<DescribeInstancesResponse>> GetMultipleRegionsAsync()
{
    var regions = new[] { "cn-hangzhou", "cn-shanghai", "cn-beijing" };
    
    var tasks = regions.Select(region => 
        client.DescribeInstancesAsync(new DescribeInstancesRequest 
        { 
            RegionId = region 
        })
    );
    
    var responses = await Task.WhenAll(tasks);
    return responses.ToList();
}
```

### 5. 依赖注入
```csharp
using Microsoft.Extensions.DependencyInjection;

// Startup.cs 或 Program.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<Config>(sp => 
    {
        return new Config
        {
            AccessKeyId = Configuration["Alibaba:AccessKeyId"],
            AccessKeySecret = Configuration["Alibaba:AccessKeySecret"],
            Endpoint = Configuration["Alibaba:Endpoint"]
        };
    });
    
    services.AddScoped<Client>(sp =>
    {
        var config = sp.GetRequiredService<Config>();
        return new Client(config);
    });
}

// 在控制器或服务中使用
public class MyService
{
    private readonly Client _client;
    
    public MyService(Client client)
    {
        _client = client;
    }
    
    public async Task DoSomethingAsync()
    {
        var response = await _client.DescribeInstancesAsync(request);
        // ...
    }
}
```

### 6. 凭证管理
```csharp
using AlibabaCloud.Credentials;
using AlibabaCloud.Credentials.Models;

// 使用默认凭证链
var credential = new Credential();

var config = new Config
{
    Credential = credential,
    Endpoint = "your-endpoint.aliyuncs.com"
};

// 或者使用 STS Token
var config = new Config
{
    AccessKeyId = "your-sts-key-id",
    AccessKeySecret = "your-sts-key-secret",
    SecurityToken = "your-security-token",
    Endpoint = "your-endpoint.aliyuncs.com"
};
```

### 7. 代理设置
```csharp
var config = new Config
{
    AccessKeyId = "your-key",
    AccessKeySecret = "your-secret",
    HttpProxy = "http://proxy.example.com:8080",
    HttpsProxy = "https://proxy.example.com:8443",
    NoProxy = "localhost,127.0.0.1"
};
```

---

## 调试技巧 / Debugging Tips

### 1. 启用详细日志
```csharp
// 在 appsettings.json 中配置
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "AlibabaCloud": "Debug"
    }
  }
}
```

### 2. 查看请求详情
```csharp
using System.Diagnostics;

var runtime = new TeaModel.RuntimeOptions
{
    Autoretry = false  // 禁用重试以查看原始错误
};

try
{
    var response = await client.CallApiAsync(params, request, runtime);
    Debug.WriteLine($"Response: {Newtonsoft.Json.JsonConvert.SerializeObject(response)}");
}
catch (Exception ex)
{
    Debug.WriteLine($"Error: {ex.Message}");
    Debug.WriteLine($"Stack trace: {ex.StackTrace}");
}
```

### 3. 使用 Fiddler 抓包
```csharp
// 设置系统代理以便 Fiddler 抓包
var config = new Config
{
    // ...
    HttpProxy = "http://127.0.0.1:8888",  // Fiddler 默认端口
    HttpsProxy = "http://127.0.0.1:8888"
};
```

### 4. 单元测试
```csharp
using Xunit;
using Moq;

public class ClientTests
{
    [Fact]
    public async Task TestDescribeInstances()
    {
        // Arrange
        var config = new Config
        {
            AccessKeyId = "test-key",
            AccessKeySecret = "test-secret",
            Endpoint = "ecs.aliyuncs.com"
        };
        var client = new Client(config);
        var request = new DescribeInstancesRequest
        {
            RegionId = "cn-hangzhou"
        };
        
        // Act
        var response = await client.DescribeInstancesAsync(request);
        
        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Body);
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

## .NET 版本兼容性 / .NET Version Compatibility

| SDK 版本 | .NET 最低版本 | 推荐版本 |
|---------|--------------|---------|
| 1.0.x   | .NET Standard 2.0 | .NET 6.0+ |
| 2.0.x   | .NET Standard 2.0 | .NET 8.0+ |

**支持的框架：**
- .NET Framework 4.6.1+
- .NET Core 2.0+
- .NET 5.0+
- .NET 6.0+
- .NET 8.0+

---

## NuGet 包管理 / NuGet Package Management

```bash
# 安装 SDK
dotnet add package AlibabaCloud.OpenApiClient

# 安装特定服务 SDK（例如 ECS）
dotnet add package AlibabaCloud.SDK.ECS20140526

# 更新到最新版本
dotnet add package AlibabaCloud.OpenApiClient --version *

# 查看已安装的包
dotnet list package
```

---

## 性能优化 / Performance Optimization

### 1. 使用连接池
```csharp
var config = new Config
{
    // ...
    MaxIdleConns = 100  // 最大空闲连接数
};
```

### 2. 启用 HTTP/2（默认）
```csharp
var config = new Config
{
    // ...
    DisableHttp2 = false  // 启用 HTTP/2
};
```

### 3. 批量操作
```csharp
// 使用并行任务处理批量请求
var tasks = instances.Select(async instance =>
{
    return await client.SomeOperationAsync(new Request { InstanceId = instance.Id });
});

var results = await Task.WhenAll(tasks);
```

---

## 相关资源 / Related Resources

- [官方文档 / Official Documentation](https://help.aliyun.com/document_detail/README.html)
- [C# SDK 参考 / C# SDK Reference](https://github.com/aliyun/darabonba-openapi/tree/master/csharp)
- [GitHub Issues](https://github.com/aliyun/darabonba-openapi/issues)
- [NuGet Gallery](https://www.nuget.org/packages/AlibabaCloud.OpenApiClient/)
- [Tea C# GitHub](https://github.com/aliyun/tea-csharp)
