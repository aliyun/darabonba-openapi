# Java SDK FAQ

## 常见问题 / Common Issues

### 1. SDK 用法与文档不一致 / SDK Usage Different from Documentation

**问题描述 / Problem Description:**
官方文档中的示例代码与实际 SDK 的使用方式不一致。

**解决方案 / Solution:**
参考以下最新的使用方式：

```java
import com.aliyun.teaopenapi.models.Config;
import com.aliyun.ecs20140526.Client;
import com.aliyun.ecs20140526.models.*;

public class Demo {
    public static void main(String[] args) throws Exception {
        // 1. 配置初始化
        Config config = new Config()
            .setAccessKeyId(System.getenv("ALIBABA_CLOUD_ACCESS_KEY_ID"))
            .setAccessKeySecret(System.getenv("ALIBABA_CLOUD_ACCESS_KEY_SECRET"))
            .setEndpoint("ecs.aliyuncs.com")
            .setRegionId("cn-hangzhou");
        
        // 2. 创建客户端
        Client client = new Client(config);
        
        // 3. 构建请求
        DescribeInstancesRequest request = new DescribeInstancesRequest()
            .setRegionId("cn-hangzhou")
            .setPageSize(10);
        
        // 4. 调用 API
        DescribeInstancesResponse response = client.describeInstances(request);
        System.out.println(response.getBody());
    }
}
```

**相关 Issue:** [#155](https://github.com/aliyun/darabonba-openapi/issues/155), [#89](https://github.com/aliyun/darabonba-openapi/issues/89)

---

### 2. 依赖冲突 / Dependency Conflicts

**问题描述 / Problem Description:**
Maven 或 Gradle 项目中出现依赖冲突，导致 SDK 无法正常工作。

**原因 / Root Cause:**
多个依赖库使用了不同版本的公共依赖（如 `okhttp`、`gson` 等）。

**解决方案 / Solution:**

**Maven:**
```xml
<dependencyManagement>
    <dependencies>
        <!-- 统一管理依赖版本 -->
        <dependency>
            <groupId>com.aliyun</groupId>
            <artifactId>tea-openapi</artifactId>
            <version>0.3.0</version>
        </dependency>
        <dependency>
            <groupId>com.squareup.okhttp3</groupId>
            <artifactId>okhttp</artifactId>
            <version>4.10.0</version>
        </dependency>
    </dependencies>
</dependencyManagement>
```

**Gradle:**
```gradle
configurations.all {
    resolutionStrategy {
        force 'com.squareup.okhttp3:okhttp:4.10.0'
        force 'com.google.code.gson:gson:2.9.0'
    }
}
```

---

### 3. 编码问题 / Encoding Issues

**问题描述 / Problem Description:**
中文参数或响应内容出现乱码。

**原因 / Root Cause:**
JVM 默认编码不是 UTF-8。

**解决方案 / Solution:**
```bash
# 启动 JVM 时指定编码
java -Dfile.encoding=UTF-8 -jar your-app.jar

# 或在代码中设置
System.setProperty("file.encoding", "UTF-8");
```

在 Maven 中配置：
```xml
<properties>
    <project.build.sourceEncoding>UTF-8</project.build.sourceEncoding>
    <project.reporting.outputEncoding>UTF-8</project.reporting.outputEncoding>
</properties>

<build>
    <plugins>
        <plugin>
            <groupId>org.apache.maven.plugins</groupId>
            <artifactId>maven-compiler-plugin</artifactId>
            <configuration>
                <encoding>UTF-8</encoding>
            </configuration>
        </plugin>
    </plugins>
</build>
```

---

### 4. SSL 证书验证问题 / SSL Certificate Verification Issues

**问题描述 / Problem Description:**
连接阿里云 API 时出现 SSL 证书验证失败。

**原因 / Root Cause:**
1. JDK 版本过旧，不支持新的 CA 证书
2. 企业环境使用了自签名证书或代理

**解决方案 / Solution:**
```java
import com.aliyun.teaopenapi.models.Config;
import com.aliyun.teautil.models.RuntimeOptions;

// 方案 1: 更新 JDK 到最新版本

// 方案 2: 禁用 SSL 验证（仅用于测试环境）
RuntimeOptions runtime = new RuntimeOptions();
runtime.ignoreSSL = true;

// 方案 3: 使用自定义 SSL 上下文
Config config = new Config()
    .setAccessKeyId("your-key")
    .setAccessKeySecret("your-secret")
    .setKey("path/to/client-key.pem")
    .setCert("path/to/client-cert.pem")
    .setCa("path/to/ca-cert.pem");
```

---

### 5. 连接超时 / Connection Timeout

**问题描述 / Problem Description:**
请求时出现连接超时错误。

**原因 / Root Cause:**
1. 网络问题
2. 默认超时时间太短
3. 代理配置不正确

**解决方案 / Solution:**
```java
import com.aliyun.teaopenapi.models.Config;
import com.aliyun.teautil.models.RuntimeOptions;

// 配置超时时间
Config config = new Config()
    .setAccessKeyId("your-key")
    .setAccessKeySecret("your-secret")
    .setReadTimeout(30000)      // 30 seconds
    .setConnectTimeout(10000);  // 10 seconds

// 或在运行时配置
RuntimeOptions runtime = new RuntimeOptions()
    .setReadTimeout(30000)
    .setConnectTimeout(10000)
    .setAutoretry(true)
    .setMaxAttempts(3);
```

---

## 异常类型 / Exception Types

### TeaException
基础异常类：
```java
public class TeaException extends Exception {
    public Integer statusCode;
    public String code;
    public String message;
    public Map<String, Object> data;
}
```

### ClientException
客户端异常：
```java
public class ClientException extends TeaException {
    public Map<String, Object> accessDeniedDetail;
}
```

### ServerException
服务器异常：
```java
public class ServerException extends TeaException {
    // 5xx 状态码相关错误
}
```

### ThrottlingException
限流异常：
```java
public class ThrottlingException extends TeaException {
    public Long retryAfter;  // 重试等待时间（毫秒）
}
```

---

## 最佳实践 / Best Practices

### 1. 配置初始化
```java
import com.aliyun.teaopenapi.models.Config;

public class ClientFactory {
    public static Config createConfig() {
        return new Config()
            .setAccessKeyId(System.getenv("ALIBABA_CLOUD_ACCESS_KEY_ID"))
            .setAccessKeySecret(System.getenv("ALIBABA_CLOUD_ACCESS_KEY_SECRET"))
            .setEndpoint("your-endpoint.aliyuncs.com")
            .setRegionId("cn-hangzhou")
            .setReadTimeout(30000)
            .setConnectTimeout(10000);
    }
}
```

### 2. 异常处理
```java
import com.aliyun.tea.TeaException;
import com.aliyun.teaopenapi.models.*;

try {
    response = client.describeInstances(request);
    System.out.println("Success: " + response.getBody());
} catch (ClientException e) {
    System.err.println("Client error: " + e.getCode() + " - " + e.getMessage());
    if (e.accessDeniedDetail != null) {
        System.err.println("Access denied details: " + e.accessDeniedDetail);
    }
} catch (ServerException e) {
    System.err.println("Server error: " + e.getCode() + " - " + e.getMessage());
} catch (ThrottlingException e) {
    System.err.println("Rate limited. Retry after: " + e.retryAfter + "ms");
    // 实现重试逻辑
    Thread.sleep(e.retryAfter);
} catch (TeaException e) {
    System.err.println("Request failed: " + e.getMessage());
    System.err.println("Status code: " + e.statusCode);
    System.err.println("Error code: " + e.code);
    System.err.println("Request ID: " + e.data.get("requestId"));
} catch (Exception e) {
    System.err.println("Unexpected error: " + e.getMessage());
    e.printStackTrace();
}
```

### 3. 使用 RuntimeOptions
```java
import com.aliyun.teautil.models.RuntimeOptions;

RuntimeOptions runtime = new RuntimeOptions()
    .setAutoretry(true)
    .setMaxAttempts(3)
    .setBackoffPolicy("exponential")
    .setBackoffPeriod(1)
    .setReadTimeout(30000)
    .setConnectTimeout(10000);

DescribeInstancesResponse response = client.describeInstancesWithOptions(request, runtime);
```

### 4. 资源管理
```java
// 使用 try-with-resources 确保资源释放
public class ClientManager implements AutoCloseable {
    private final Client client;
    
    public ClientManager(Config config) throws Exception {
        this.client = new Client(config);
    }
    
    public Client getClient() {
        return client;
    }
    
    @Override
    public void close() throws Exception {
        // 清理资源
        if (client != null) {
            // SDK 通常会自动管理资源
        }
    }
}

// 使用
try (ClientManager manager = new ClientManager(config)) {
    Client client = manager.getClient();
    // 使用 client
} catch (Exception e) {
    e.printStackTrace();
}
```

### 5. 异步调用
```java
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class AsyncClient {
    private final Client client;
    private final ExecutorService executor;
    
    public AsyncClient(Config config) throws Exception {
        this.client = new Client(config);
        this.executor = Executors.newFixedThreadPool(10);
    }
    
    public CompletableFuture<DescribeInstancesResponse> describeInstancesAsync(
            DescribeInstancesRequest request) {
        return CompletableFuture.supplyAsync(() -> {
            try {
                return client.describeInstances(request);
            } catch (Exception e) {
                throw new RuntimeException(e);
            }
        }, executor);
    }
    
    public void shutdown() {
        executor.shutdown();
    }
}
```

### 6. 凭证管理
```java
import com.aliyun.credentials.Client as CredentialClient;
import com.aliyun.credentials.models.Config as CredentialConfig;

// 使用默认凭证链
CredentialClient credential = new CredentialClient();

Config config = new Config()
    .setCredential(credential)
    .setEndpoint("your-endpoint.aliyuncs.com");

// 或者使用 STS Token
Config config = new Config()
    .setAccessKeyId("your-sts-key-id")
    .setAccessKeySecret("your-sts-key-secret")
    .setSecurityToken("your-security-token")
    .setEndpoint("your-endpoint.aliyuncs.com");
```

### 7. 代理设置
```java
Config config = new Config()
    .setAccessKeyId("your-key")
    .setAccessKeySecret("your-secret")
    .setHttpProxy("http://proxy.example.com:8080")
    .setHttpsProxy("https://proxy.example.com:8443")
    .setNoProxy("localhost,127.0.0.1");
```

---

## 调试技巧 / Debugging Tips

### 1. 启用日志
```java
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

// 使用 SLF4J + Logback
Logger logger = LoggerFactory.getLogger(YourClass.class);

// logback.xml 配置
/*
<configuration>
    <appender name="STDOUT" class="ch.qos.logback.core.ConsoleAppender">
        <encoder>
            <pattern>%d{HH:mm:ss.SSS} [%thread] %-5level %logger{36} - %msg%n</pattern>
        </encoder>
    </appender>
    
    <logger name="com.aliyun" level="DEBUG"/>
    
    <root level="INFO">
        <appender-ref ref="STDOUT"/>
    </root>
</configuration>
*/
```

### 2. 打印请求详情
```java
RuntimeOptions runtime = new RuntimeOptions()
    .setAutoretry(false);  // 禁用重试以查看原始错误

try {
    response = client.describeInstancesWithOptions(request, runtime);
    System.out.println("Response headers: " + response.getHeaders());
    System.out.println("Response body: " + response.getBody());
} catch (TeaException e) {
    System.err.println("Request: " + e.getData().get("request"));
    System.err.println("Response: " + e.getData().get("response"));
}
```

### 3. 使用调试代理
```java
// 配置系统代理
System.setProperty("http.proxyHost", "127.0.0.1");
System.setProperty("http.proxyPort", "8888");
System.setProperty("https.proxyHost", "127.0.0.1");
System.setProperty("https.proxyPort", "8888");

// 或在 Config 中设置
Config config = new Config()
    .setHttpProxy("http://127.0.0.1:8888")
    .setHttpsProxy("http://127.0.0.1:8888");
```

### 4. 单元测试
```java
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.BeforeEach;
import static org.junit.jupiter.api.Assertions.*;

public class ClientTest {
    private Client client;
    
    @BeforeEach
    public void setup() throws Exception {
        Config config = new Config()
            .setAccessKeyId("test-key")
            .setAccessKeySecret("test-secret")
            .setEndpoint("ecs.aliyuncs.com");
        client = new Client(config);
    }
    
    @Test
    public void testDescribeInstances() throws Exception {
        DescribeInstancesRequest request = new DescribeInstancesRequest()
            .setRegionId("cn-hangzhou");
        
        DescribeInstancesResponse response = client.describeInstances(request);
        
        assertNotNull(response);
        assertNotNull(response.getBody());
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

## Java 版本兼容性 / Java Version Compatibility

| SDK 版本 | Java 最低版本 | 推荐版本 |
|---------|--------------|---------|
| 0.2.x   | Java 8       | Java 11+ |
| 0.3.x   | Java 8       | Java 17+ |

**推荐配置：**
- Java 11 LTS 或更高版本
- Maven 3.6+ 或 Gradle 7.0+

---

## Maven 依赖配置 / Maven Dependency Configuration

```xml
<dependencies>
    <!-- 核心依赖 -->
    <dependency>
        <groupId>com.aliyun</groupId>
        <artifactId>tea-openapi</artifactId>
        <version>0.3.0</version>
    </dependency>
    
    <!-- 凭证管理 -->
    <dependency>
        <groupId>com.aliyun</groupId>
        <artifactId>credentials-java</artifactId>
        <version>0.3.0</version>
    </dependency>
    
    <!-- 日志 -->
    <dependency>
        <groupId>org.slf4j</groupId>
        <artifactId>slf4j-api</artifactId>
        <version>2.0.0</version>
    </dependency>
    <dependency>
        <groupId>ch.qos.logback</groupId>
        <artifactId>logback-classic</artifactId>
        <version>1.4.0</version>
    </dependency>
</dependencies>
```

---

## Gradle 依赖配置 / Gradle Dependency Configuration

```gradle
dependencies {
    implementation 'com.aliyun:tea-openapi:0.3.0'
    implementation 'com.aliyun:credentials-java:0.3.0'
    implementation 'org.slf4j:slf4j-api:2.0.0'
    implementation 'ch.qos.logback:logback-classic:1.4.0'
}
```

---

## 性能优化 / Performance Optimization

### 1. 连接池配置
```java
Config config = new Config()
    .setMaxIdleConns(100);  // 最大空闲连接数
```

### 2. 启用 HTTP/2
```java
Config config = new Config()
    .setDisableHttp2(false);  // 启用 HTTP/2
```

### 3. 批量操作
```java
// 使用 CompletableFuture 并行处理
List<CompletableFuture<DescribeInstancesResponse>> futures = regions.stream()
    .map(region -> CompletableFuture.supplyAsync(() -> {
        try {
            return client.describeInstances(
                new DescribeInstancesRequest().setRegionId(region)
            );
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }))
    .collect(Collectors.toList());

List<DescribeInstancesResponse> responses = futures.stream()
    .map(CompletableFuture::join)
    .collect(Collectors.toList());
```

---

## 相关资源 / Related Resources

- [官方文档 / Official Documentation](https://help.aliyun.com/document_detail/README.html)
- [Java SDK 参考 / Java SDK Reference](https://github.com/aliyun/darabonba-openapi/tree/master/java)
- [GitHub Issues](https://github.com/aliyun/darabonba-openapi/issues)
- [Maven Central](https://search.maven.org/search?q=g:com.aliyun%20AND%20a:tea-openapi)
