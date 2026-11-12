# PHP SDK FAQ

## 常见问题 / Common Issues

### 1. Class 'Darabonba\OpenApi\Models\Config' not found

**问题描述 / Problem Description:**
运行时出现 `Class 'Darabonba\OpenApi\Models\Config' not found` 错误。

**原因 / Root Cause:**
1. 未正确安装依赖包
2. Composer autoload 未更新
3. 命名空间使用错误

**解决方案 / Solution:**
```bash
# 1. 安装依赖
composer require alibabacloud/darabonba-openapi

# 2. 更新 autoload
composer dump-autoload

# 3. 确保引入了正确的命名空间
```

在代码中：
```php
<?php
require_once 'vendor/autoload.php';

use Darabonba\OpenApi\Models\Config;
use Darabonba\OpenApi\OpenApiClient;

$config = new Config([
    'accessKeyId' => 'your-access-key-id',
    'accessKeySecret' => 'your-access-key-secret',
    'endpoint' => 'your-endpoint.aliyuncs.com'
]);
```

**相关 Issue:** [#132](https://github.com/aliyun/darabonba-openapi/issues/132)

---

### 2. Class 'AlibabaCloud\Tea\Model' not found

**问题描述 / Problem Description:**
```
Class 'AlibabaCloud\Tea\Model' not found
```

**原因 / Root Cause:**
缺少 `alibabacloud/tea` 依赖包或版本不兼容。

**解决方案 / Solution:**
```bash
# 安装或更新 tea 包
composer require alibabacloud/tea

# 或者更新所有依赖
composer update
```

检查 `composer.json` 中的依赖版本：
```json
{
    "require": {
        "alibabacloud/tea": "^3.0",
        "alibabacloud/darabonba-openapi": "^0.2"
    }
}
```

**相关 Issue:** [#130](https://github.com/aliyun/darabonba-openapi/issues/130)

---

### 3. 参数不能通过引用传递 / Argument could not be passed by reference

**问题描述 / Problem Description:**
```
AlibabaCloud\Tea\Utils\Utils::isUnset(): Argument #1 ($value) could not be passed by reference
```

**原因 / Root Cause:**
在 PHP 8.0+ 中，某些函数参数传递方式发生了变化，导致与旧版本代码不兼容。

**解决方案 / Solution:**
1. 更新到最新版本的 SDK：
```bash
composer update alibabacloud/darabonba-openapi
```

2. 如果问题依然存在，检查 PHP 版本兼容性：
```bash
php -v
```

确保使用兼容的 PHP 版本（推荐 7.4 或 8.0+）。

**相关 Issue:** [#206](https://github.com/aliyun/darabonba-openapi/issues/206)

---

### 4. 签名算法属性未定义 / Undefined property: signatureAlgorithm

**问题描述 / Problem Description:**
```
Undefined property: AlibabaCloud\Tea\Rpc\Rpc\Config::$signatureAlgorithm
```

**原因 / Root Cause:**
配置对象缺少 `signatureAlgorithm` 属性，SDK 尝试访问该属性时出错。

**解决方案 / Solution:**
```php
use Darabonba\OpenApi\Models\Config;

$config = new Config([
    'accessKeyId' => 'your-access-key-id',
    'accessKeySecret' => 'your-access-key-secret',
    'endpoint' => 'your-endpoint.aliyuncs.com',
    'signatureAlgorithm' => 'v2'  // 显式设置签名算法
]);
```

或者更新到最新版本的 SDK，它会自动设置默认值。

**相关 Issue:** [#113](https://github.com/aliyun/darabonba-openapi/issues/113)

---

### 5. tea-xml 依赖版本问题 / tea-xml Dependency Version Issue

**问题描述 / Problem Description:**
`tea-xml` 依赖包已经修复了某些问题，但 `openapi` 中的依赖版本没有更新。

**解决方案 / Solution:**
```bash
# 手动更新 tea-xml 到最新版本
composer require alibabacloud/tea-xml:^0.2.2

# 或强制更新所有依赖
composer update --with-all-dependencies
```

**相关 Issue:** [#112](https://github.com/aliyun/darabonba-openapi/issues/112)

---

## 异常类型 / Exception Types

### AlibabaCloudException
基础异常类：
```php
class AlibabaCloudException extends \Exception
{
    public $statusCode;
    public $code;
    public $message;
    public $description;
    public $requestId;
}
```

### ClientException
客户端异常：
```php
class ClientException extends AlibabaCloudException
{
    public $accessDeniedDetail;
}
```

### ServerException
服务器异常：
```php
class ServerException extends AlibabaCloudException
{
    // 5xx 状态码相关错误
}
```

### ThrottlingException
限流异常：
```php
class ThrottlingException extends AlibabaCloudException
{
    public $retryAfter;  // 重试等待时间（毫秒）
}
```

---

## 最佳实践 / Best Practices

### 1. 配置初始化
```php
<?php
use Darabonba\OpenApi\Models\Config;
use Darabonba\OpenApi\OpenApiClient;

$config = new Config([
    'accessKeyId' => getenv('ALIBABA_CLOUD_ACCESS_KEY_ID'),
    'accessKeySecret' => getenv('ALIBABA_CLOUD_ACCESS_KEY_SECRET'),
    'endpoint' => 'your-endpoint.aliyuncs.com',
    'regionId' => 'cn-hangzhou',
]);

// 设置超时时间
$config->readTimeout = 30000;     // 30 seconds
$config->connectTimeout = 10000;  // 10 seconds

$client = new OpenApiClient($config);
```

### 2. 异常处理
```php
<?php
use Darabonba\OpenApi\Exceptions\ClientException;
use Darabonba\OpenApi\Exceptions\ServerException;
use Darabonba\OpenApi\Exceptions\ThrottlingException;

try {
    $response = $client->callApi($params, $request, $runtime);
    echo "Success: " . json_encode($response);
} catch (ThrottlingException $e) {
    echo "Rate limited. Retry after: " . $e->retryAfter . "ms\n";
    // 实现重试逻辑
} catch (ClientException $e) {
    echo "Client error: " . $e->code . " - " . $e->message . "\n";
    if ($e->accessDeniedDetail) {
        print_r($e->accessDeniedDetail);
    }
} catch (ServerException $e) {
    echo "Server error: " . $e->code . " - " . $e->message . "\n";
} catch (\Exception $e) {
    echo "Unexpected error: " . $e->getMessage() . "\n";
}
```

### 3. 使用 RuntimeOptions
```php
<?php
use AlibabaCloud\Tea\Model;
use AlibabaCloud\Tea\Utils\Utils\RuntimeOptions;

$runtime = new RuntimeOptions([
    'autoretry' => true,
    'maxAttempts' => 3,
    'backoffPolicy' => 'exponential',
    'backoffPeriod' => 1,
    'readTimeout' => 30000,
    'connectTimeout' => 10000
]);

$response = $client->someMethodWithOptions($request, $runtime);
```

### 4. 数组转对象
```php
<?php
// PHP SDK 通常接受数组参数
$config = new Config([
    'accessKeyId' => 'your-key',
    'accessKeySecret' => 'your-secret',
    'endpoint' => 'your-endpoint.aliyuncs.com'
]);

// 或者使用对象属性
$config = new Config();
$config->accessKeyId = 'your-key';
$config->accessKeySecret = 'your-secret';
$config->endpoint = 'your-endpoint.aliyuncs.com';
```

### 5. 代理设置
```php
<?php
$config = new Config([
    'accessKeyId' => 'your-key',
    'accessKeySecret' => 'your-secret',
    'httpProxy' => 'http://proxy.example.com:8080',
    'httpsProxy' => 'https://proxy.example.com:8443',
    'noProxy' => 'localhost,127.0.0.1'
]);
```

### 6. 凭证管理
```php
<?php
use AlibabaCloud\Credentials\Credential;

// 使用默认凭证链
$credential = new Credential();

$config = new Config([
    'credential' => $credential,
    'endpoint' => 'your-endpoint.aliyuncs.com'
]);

// 或者使用 STS Token
$config = new Config([
    'accessKeyId' => 'your-sts-key-id',
    'accessKeySecret' => 'your-sts-key-secret',
    'securityToken' => 'your-security-token',
    'endpoint' => 'your-endpoint.aliyuncs.com'
]);
```

---

## 调试技巧 / Debugging Tips

### 1. 启用错误显示
```php
<?php
// 开发环境启用所有错误
error_reporting(E_ALL);
ini_set('display_errors', '1');

// 生产环境记录到日志
ini_set('log_errors', '1');
ini_set('error_log', '/path/to/error.log');
```

### 2. 查看请求详情
```php
<?php
try {
    $response = $client->callApi($params, $request, $runtime);
    var_dump($response);
} catch (\Exception $e) {
    echo "Error: " . $e->getMessage() . "\n";
    echo "Trace: " . $e->getTraceAsString() . "\n";
}
```

### 3. 检查 Composer 依赖
```bash
# 查看已安装的包
composer show

# 查看特定包的版本
composer show alibabacloud/darabonba-openapi

# 检查依赖冲突
composer diagnose
```

### 4. 验证 autoload
```php
<?php
// 检查类是否可以加载
if (class_exists('Darabonba\OpenApi\Models\Config')) {
    echo "Config class is loaded\n";
} else {
    echo "Config class not found\n";
    echo "Include path: " . get_include_path() . "\n";
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

## PHP 版本兼容性 / PHP Version Compatibility

| SDK 版本 | PHP 最低版本 | 推荐版本 |
|---------|------------|---------|
| 0.2.x   | PHP 5.6    | PHP 7.4+ |
| 0.3.x   | PHP 7.2    | PHP 8.0+ |

**注意事项：**
- PHP 7.4 及以上版本推荐使用最新版本 SDK
- PHP 5.6 - 7.1 只能使用较旧版本的 SDK
- PHP 8.0+ 提供了更好的性能和类型安全

---

## Composer 配置建议 / Composer Configuration Tips

```json
{
    "require": {
        "php": ">=7.2",
        "alibabacloud/darabonba-openapi": "^0.2",
        "alibabacloud/tea": "^3.0",
        "alibabacloud/tea-utils": "^0.2",
        "alibabacloud/credentials": "^1.1"
    },
    "config": {
        "optimize-autoloader": true,
        "classmap-authoritative": true
    }
}
```

---

## 性能优化 / Performance Optimization

### 1. 启用 Composer 优化
```bash
composer dump-autoload --optimize --classmap-authoritative
```

### 2. 使用连接池
```php
<?php
$config = new Config([
    // ...
    'maxIdleConns' => 100  // 最大空闲连接数
]);
```

### 3. 启用 OPcache
```ini
; php.ini
opcache.enable=1
opcache.memory_consumption=128
opcache.interned_strings_buffer=8
opcache.max_accelerated_files=10000
```

---

## 相关资源 / Related Resources

- [官方文档 / Official Documentation](https://help.aliyun.com/document_detail/README.html)
- [PHP SDK 参考 / PHP SDK Reference](https://github.com/aliyun/darabonba-openapi/tree/master/php)
- [GitHub Issues](https://github.com/aliyun/darabonba-openapi/issues)
- [Packagist](https://packagist.org/packages/alibabacloud/darabonba-openapi)
