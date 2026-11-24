# Python SDK FAQ

## 常见问题 / Common Issues

### 1. 依赖版本不匹配 / Dependency Version Mismatch

**问题描述 / Problem Description:**
使用 `alibabacloud-tea-openapi` 0.3.14 版本时，出现 `AttributeError: 'CredentialModel' object has no attribute 'provider_name'` 错误。

**原因 / Root Cause:**
`alibabacloud-tea-openapi` 0.3.14 版本使用了 `CredentialModel.provider_name` 属性，该属性只在 `alibabacloud-credentials` >= 1.0.0 中存在，但依赖配置中允许使用 >= 0.3.6 的版本。

**解决方案 / Solution:**
```bash
pip install --upgrade alibabacloud-credentials>=1.0.0
```

**相关 Issue:** [#203](https://github.com/aliyun/darabonba-openapi/issues/203)

---

### 2. 中文环境下的日期格式错误 / Date Format Error in Chinese Locale

**问题描述 / Problem Description:**
在中文环境下调用 API 时出现 `UnicodeEncodeError: 'latin-1' codec can't encode character '\u4e94'` 错误。

**原因 / Root Cause:**
`UtilClient.get_date_utcstring()` 方法在中文环境下返回的日期字符串包含中文字符（例如："五, 17 12 2021 01:16:54 GMT"），导致 HTTP header 编码失败。

**解决方案 / Solution:**
确保系统环境使用英文 locale：
```python
import locale
locale.setlocale(locale.LC_TIME, 'en_US.UTF-8')
```

或者在初始化 SDK 前设置环境变量：
```python
import os
os.environ['LC_ALL'] = 'en_US.UTF-8'
os.environ['LANG'] = 'en_US.UTF-8'
```

**相关 Issue:** [#99](https://github.com/aliyun/darabonba-openapi/issues/99)

---

### 3. 读取超时错误 / Read Timeout Error

**问题描述 / Problem Description:**
长时间运行的操作（如镜像导入）出现 `Read timed out` 错误。

**原因 / Root Cause:**
默认的读取超时时间（10秒）对于某些长时间运行的操作来说太短。

**解决方案 / Solution:**
通过 `RuntimeOptions` 增加超时时间：
```python
from alibabacloud_tea_util.models import RuntimeOptions

runtime = RuntimeOptions(
    read_timeout=60000,  # 60 seconds in milliseconds
    connect_timeout=10000
)
response = client.describe_images_with_options(request, runtime)
```

**相关 Issue:** [#140](https://github.com/aliyun/darabonba-openapi/issues/140)

---

### 4. 异步重试等待问题 / Async Retry Wait Issue

**问题描述 / Problem Description:**
在异步应用中，重试机制使用 `time.sleep()` 阻塞事件循环。

**原因 / Root Cause:**
SDK 使用同步的 `time.sleep()` 进行重试等待，在异步环境中会阻塞整个事件循环。

**解决方案 / Solution:**
对于异步应用，建议：
1. 减少重试次数
2. 使用较短的重试间隔
3. 或者在同步上下文中调用 SDK

```python
from alibabacloud_tea_util.models import RuntimeOptions

runtime = RuntimeOptions(
    autoretry=True,
    max_attempts=2,  # 减少重试次数
    backoff_policy='no'  # 不使用退避策略
)
```

**相关 Issue:** [#136](https://github.com/aliyun/darabonba-openapi/issues/136)

---

### 5. 缺少 wheel 分发包 / Missing Wheel Distribution

**问题描述 / Problem Description:**
PyPI 上只有源码分发包（source distribution），每个用户都需要自己构建 wheel。

**说明 / Note:**
这是一个包分发的优化问题，不影响功能使用。如果遇到安装缓慢，可以：

**解决方案 / Solution:**
```bash
# 使用 pip 缓存加速
pip install --use-pep517 alibabacloud-tea-openapi

# 或者手动构建 wheel
pip install build
python -m build
pip install dist/*.whl
```

**相关 Issue:** [#218](https://github.com/aliyun/darabonba-openapi/issues/218)

---

## 异常类型 / Exception Types

### ClientException
客户端异常，通常由参数错误或配置问题引起：
- `ParameterMissing`: 必需参数缺失
- `InvalidParameter`: 参数值无效

### ServerException
服务器端异常，由服务端返回的错误：
- 5xx 状态码
- 服务端内部错误

### ThrottlingException
请求限流异常：
- `Throttling`: 请求频率超过限制
- 包含 `retry_after` 字段指示重试等待时间

### UnretryableException
不可重试的异常：
- 网络连接失败
- 超时错误
- SSL/TLS 错误

---

## 最佳实践 / Best Practices

### 1. 配置初始化
```python
from alibabacloud_tea_openapi import models as open_api_models

config = open_api_models.Config(
    access_key_id='your-access-key-id',
    access_key_secret='your-access-key-secret',
    endpoint='your-endpoint'
)

# 设置合理的超时时间
config.read_timeout = 30000  # 30 seconds
config.connect_timeout = 10000  # 10 seconds
```

### 2. 异常处理
```python
from alibabacloud_tea_openapi import exceptions

try:
    response = client.call_api(params, request, runtime)
except exceptions.ClientException as e:
    print(f"Client error: {e.code} - {e.message}")
except exceptions.ServerException as e:
    print(f"Server error: {e.code} - {e.message}")
except exceptions.ThrottlingException as e:
    print(f"Throttling: retry after {e.retry_after}ms")
    # 等待后重试
except Exception as e:
    print(f"Unexpected error: {str(e)}")
```

### 3. 重试策略
```python
from alibabacloud_tea_util.models import RuntimeOptions

runtime = RuntimeOptions(
    autoretry=True,
    max_attempts=3,
    backoff_policy='exponential',  # 指数退避
    backoff_period=1  # 初始退避时间（秒）
)
```

### 4. 代理设置
```python
config = open_api_models.Config(
    access_key_id='your-access-key-id',
    access_key_secret='your-access-key-secret'
)

# 设置代理
config.http_proxy = 'http://proxy.example.com:8080'
config.https_proxy = 'https://proxy.example.com:8443'
config.no_proxy = 'localhost,127.0.0.1'
```

---

## 调试技巧 / Debugging Tips

### 1. 启用调试日志
```python
import logging

logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger('alibabacloud_tea_openapi')
logger.setLevel(logging.DEBUG)
```

### 2. 查看请求详情
```python
# 在 RuntimeOptions 中启用详细日志
runtime = RuntimeOptions(
    autoretry=False  # 禁用重试以便查看原始错误
)
```

### 3. 检查凭证
```python
from alibabacloud_credentials.client import Client as CredentialClient

# 验证凭证是否正确加载
credential = CredentialClient()
cred_model = credential.get_credential()
print(f"Access Key ID: {cred_model.access_key_id[:4]}...")
print(f"Provider: {cred_model.provider_name}")
```

---

## 常见错误代码 / Common Error Codes

- `InvalidAccessKeyId.NotFound`: Access Key ID 不存在
- `InvalidAccessKeySecret`: Access Key Secret 错误
- `SignatureDoesNotMatch`: 签名不匹配
- `Throttling.User`: 用户级别限流
- `ServiceUnavailable`: 服务暂时不可用
- `InternalError`: 服务端内部错误

---

## 相关资源 / Related Resources

- [官方文档 / Official Documentation](https://help.aliyun.com/document_detail/README.html)
- [Python SDK 参考 / Python SDK Reference](https://github.com/aliyun/darabonba-openapi/tree/master/python)
- [GitHub Issues](https://github.com/aliyun/darabonba-openapi/issues)
