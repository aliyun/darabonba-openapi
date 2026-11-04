# TypeScript/JavaScript SDK FAQ

## 常见问题 / Common Issues

### 1. keepAlive 配置无法传递 / Unable to Pass keepAlive Configuration

**问题描述 / Problem Description:**
通过 `RuntimeOptions` 传递 `keepAlive: false` 配置时，该配置未被正确读取，请求仍然使用默认的 `keepAlive: true`。

**原因 / Root Cause:**
在 `doRequest` 方法中，`keepAlive` 配置未从 `runtime` 参数中读取并传递到 HTTP 客户端。

**解决方案 / Solution:**
等待官方修复。临时解决方案是直接修改 HTTP 客户端配置：
```typescript
import http from 'http';
import https from 'https';

const httpAgent = new http.Agent({ keepAlive: false });
const httpsAgent = new https.Agent({ keepAlive: false });

// 在配置中使用自定义 agent
// 注意：这需要在底层 HTTP 库层面配置
```

**相关 Issue:** [#238](https://github.com/aliyun/darabonba-openapi/issues/238)

---

### 2. getCredential 不是一个函数 / getCredential is not a function

**问题描述 / Problem Description:**
升级到 `@alicloud/openapi-client` 0.4.10 版本后，出现错误：
```
TypeError: this._credential.getCredential is not a function
```

**原因 / Root Cause:**
新版本的代码期望 `_credential` 对象有 `getCredential()` 方法，但旧版本的凭证配置对象没有这个方法。

**解决方案 / Solution:**
升级 `@alicloud/credentials` 到最新版本：
```bash
npm install @alicloud/credentials@latest
```

或者使用新的凭证初始化方式：
```typescript
import Credential from '@alicloud/credentials';

const cred = new Credential({
  type: 'access_key',
  accessKeyId: 'your-access-key-id',
  accessKeySecret: 'your-access-key-secret'
});

const config = new Config({
  credential: cred
});
```

**相关 Issue:** [#156](https://github.com/aliyun/darabonba-openapi/issues/156), [#162](https://github.com/aliyun/darabonba-openapi/issues/162)

---

### 3. 内网环境签名问题 / Signature Issue in Private Network

**问题描述 / Problem Description:**
在内网环境中通过网闸代理访问阿里云 API 时，由于实际访问的 IP 和端口与签名时的 host 不一致，导致签名校验失败。

**场景说明 / Scenario:**
需要访问 `dysmsapi.aliyuncs.com:443`，但内网映射为 `10.10.10.100:9988`

**解决方案 / Solution:**
目前 SDK 不直接支持这种场景。可能的解决方案：
1. 在网闸层面做 SNI 和 Host header 转换
2. 或者修改 SDK 源码以支持自定义签名 host

```typescript
// 临时解决方案示例（需要修改 SDK 源码）
const request = {
  headers: {
    'host': 'dysmsapi.aliyuncs.com'  // 强制使用正确的 host 进行签名
  }
};
```

**相关 Issue:** [#215](https://github.com/aliyun/darabonba-openapi/issues/215)

---

### 4. HTTP 代理设置不生效 / HTTP Proxy Settings Not Working

**问题描述 / Problem Description:**
在 `RuntimeOptions` 中设置 `httpProxy` 和 `httpsProxy` 后，代理配置未生效。

**原因 / Root Cause:**
可能是代理配置格式不正确或环境变量优先级问题。

**解决方案 / Solution:**
```typescript
import * as $OpenApi from '@alicloud/openapi-client';
import * as $Util from '@alicloud/tea-util';

// 正确的代理配置格式（不包含 http:// 或 https:// 前缀）
const runtime = new $Util.RuntimeOptions({
  httpProxy: '127.0.0.1:7890',
  httpsProxy: '127.0.0.1:7890',
  noProxy: 'localhost,127.0.0.1'
});

const resp = await client.sendSmsWithOptions(request, runtime);
```

**相关 Issue:** [#134](https://github.com/aliyun/darabonba-openapi/issues/134)

---

### 5. TypeScript 类型提示缺失 / Missing TypeScript Intellisense

**问题描述 / Problem Description:**
在 VSCode 等 IDE 中编辑代码时，Config 类的属性（如 `accessKeyId`、`accessKeySecret`）没有智能提示。

**原因 / Root Cause:**
类型定义文件可能不完整或 IDE 未正确加载类型。

**解决方案 / Solution:**
```typescript
// 确保安装了类型定义
npm install --save-dev @types/node

// 显式导入类型
import * as $OpenApi from '@alicloud/openapi-client';

// 使用类型注解
const config: $OpenApi.Config = new $OpenApi.Config({
  accessKeyId: 'your-access-key-id',
  accessKeySecret: 'your-access-key-secret',
  endpoint: 'your-endpoint'
});
```

**相关 Issue:** [#159](https://github.com/aliyun/darabonba-openapi/issues/159)

---

### 6. SAE Client 不是构造函数 / SAE Client is not a constructor

**问题描述 / Problem Description:**
尝试创建 SAE (Serverless App Engine) 客户端时出现 "Client is not a constructor" 错误。

**原因 / Root Cause:**
模块导入方式不正确或模块导出配置问题。

**解决方案 / Solution:**
```typescript
// CommonJS 导入方式
const SAEClient = require('@alicloud/sae20190506').default;
const client = new SAEClient(config);

// ES Module 导入方式
import Client from '@alicloud/sae20190506';
const client = new Client(config);

// 或者
import * as SAE from '@alicloud/sae20190506';
const client = new SAE.default(config);
```

**相关 Issue:** [#172](https://github.com/aliyun/darabonba-openapi/issues/172)

---

## 异常类型 / Exception Types

### ClientError
客户端错误：
```typescript
class ClientError extends AlibabaCloudError {
  accessDeniedDetail?: { [key: string]: any };
}
```

### ServerError
服务器端错误：
```typescript
class ServerError extends AlibabaCloudError {
  // 5xx 状态码相关错误
}
```

### ThrottlingError
限流错误：
```typescript
class ThrottlingError extends AlibabaCloudError {
  retryAfter?: number;  // 重试等待时间（毫秒）
}
```

### AlibabaCloudError
基础错误类：
```typescript
class AlibabaCloudError extends Error {
  statusCode?: number;
  code?: string;
  message?: string;
  description?: string;
  requestId?: string;
}
```

---

## 最佳实践 / Best Practices

### 1. 配置初始化
```typescript
import * as $OpenApi from '@alicloud/openapi-client';

const config = new $OpenApi.Config({
  accessKeyId: process.env.ALIBABA_CLOUD_ACCESS_KEY_ID,
  accessKeySecret: process.env.ALIBABA_CLOUD_ACCESS_KEY_SECRET,
  endpoint: 'your-endpoint.aliyuncs.com',
  regionId: 'cn-hangzhou'
});

// 设置超时时间
config.readTimeout = 30000;  // 30 seconds
config.connectTimeout = 10000;  // 10 seconds
```

### 2. 异常处理
```typescript
import { ClientError, ServerError, ThrottlingError } from '@alicloud/openapi-client';

try {
  const response = await client.callApi(params, request, runtime);
  console.log(response);
} catch (error) {
  if (error instanceof ThrottlingError) {
    console.log(`Rate limited. Retry after ${error.retryAfter}ms`);
    // 实现重试逻辑
  } else if (error instanceof ClientError) {
    console.log(`Client error: ${error.code} - ${error.message}`);
    if (error.accessDeniedDetail) {
      console.log('Access denied details:', error.accessDeniedDetail);
    }
  } else if (error instanceof ServerError) {
    console.log(`Server error: ${error.code} - ${error.message}`);
  } else {
    console.log('Unexpected error:', error);
  }
}
```

### 3. 使用 RuntimeOptions
```typescript
import * as $Util from '@alicloud/tea-util';

const runtime = new $Util.RuntimeOptions({
  autoretry: true,
  maxAttempts: 3,
  backoffPolicy: 'exponential',
  backoffPeriod: 1,
  readTimeout: 30000,
  connectTimeout: 10000
});

const response = await client.someMethodWithOptions(request, runtime);
```

### 4. Promise 和 Async/Await
```typescript
// 推荐使用 async/await
async function callApi() {
  try {
    const response = await client.callApi(params, request, runtime);
    return response;
  } catch (error) {
    console.error('API call failed:', error);
    throw error;
  }
}

// 或者使用 Promise
client.callApi(params, request, runtime)
  .then(response => {
    console.log('Success:', response);
  })
  .catch(error => {
    console.error('Error:', error);
  });
```

### 5. 凭证管理
```typescript
import Credential from '@alicloud/credentials';

// 使用凭证链（推荐）
const cred = new Credential();

// 或者显式指定凭证类型
const cred = new Credential({
  type: 'access_key',
  accessKeyId: 'your-access-key-id',
  accessKeySecret: 'your-access-key-secret'
});

// 使用 STS Token
const cred = new Credential({
  type: 'sts',
  accessKeyId: 'your-access-key-id',
  accessKeySecret: 'your-access-key-secret',
  securityToken: 'your-security-token'
});

const config = new $OpenApi.Config({
  credential: cred
});
```

---

## 调试技巧 / Debugging Tips

### 1. 启用请求日志
```typescript
// 设置环境变量启用调试日志
process.env.DEBUG = 'alicloud:*';
```

### 2. 查看请求和响应
```typescript
const runtime = new $Util.RuntimeOptions({
  autoretry: false  // 禁用自动重试以便查看原始错误
});

try {
  const response = await client.callApi(params, request, runtime);
  console.log('Response:', JSON.stringify(response, null, 2));
} catch (error) {
  console.error('Request failed:', error);
  if (error.data) {
    console.error('Response data:', error.data);
  }
}
```

### 3. 检查网络连接
```typescript
import * as http from 'http';

// 测试代理连接
const options = {
  host: 'proxy.example.com',
  port: 8080,
  method: 'CONNECT',
  path: 'aliyuncs.com:443'
};

const req = http.request(options);
req.on('connect', (res, socket, head) => {
  console.log('Proxy connection established');
});
req.on('error', (err) => {
  console.error('Proxy connection failed:', err);
});
req.end();
```

---

## 常见错误代码 / Common Error Codes

- `InvalidAccessKeyId.NotFound`: Access Key ID 不存在
- `SignatureDoesNotMatch`: 签名不匹配
- `RequestTimeTooSkewed`: 请求时间与服务器时间相差太大
- `Throttling.User`: 用户级别限流
- `ServiceUnavailable`: 服务暂时不可用
- `InternalError`: 服务端内部错误
- `SDK.InvalidParameter`: SDK 参数无效

---

## 相关资源 / Related Resources

- [官方文档 / Official Documentation](https://help.aliyun.com/document_detail/README.html)
- [TypeScript SDK 参考 / TypeScript SDK Reference](https://github.com/aliyun/darabonba-openapi/tree/master/ts)
- [GitHub Issues](https://github.com/aliyun/darabonba-openapi/issues)
- [npm Package](https://www.npmjs.com/package/@alicloud/openapi-client)
