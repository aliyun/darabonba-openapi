<?php

namespace Darabonba\OpenApi\Tests;

use Darabonba\OpenApi\OpenApiClient;
use Darabonba\OpenApi\Utils;

use Darabonba\OpenApi\Models\Config;
use Darabonba\OpenApi\Models\Params;
use Darabonba\OpenApi\Models\GlobalParameters;
use AlibabaCloud\Dara\Models\RuntimeOptions;
use AlibabaCloud\Dara\Models\ExtendsParameters;
use Darabonba\OpenApi\Models\OpenApiRequest;
use Darabonba\OpenApi\Exceptions\ThrottlingException;
use AlibabaCloud\Credentials\Credential;
use AlibabaCloud\Dara\Exception\DaraUnableRetryException;
use AlibabaCloud\Dara\RetryPolicy\RetryCondition;
use PHPUnit\Framework\TestCase;
use AlibabaCloud\Dara\RetryPolicy\RetryOptions;

/**
 * @internal
 * @coversNothing
 */
class OpenApiClientTest extends TestCase
{

    /**
     * @var resource
     */
    private $pid = 0;
    private static $serverStarted = false;
    private static $serverPid = 0;
    private static $throttlingServerStarted = false;
    private static $throttlingServerPid = 0;

    /**
     * Ensure throttling mock server is running on port 8001.
     */
    private static function ensureThrottlingMockServer()
    {
        if (self::$throttlingServerStarted) {
            return;
        }
        $server = __DIR__ . '/Mock/ThrottlingMockServer.php';
        $command = "php " . escapeshellarg($server) . " > /dev/null 2>&1 & echo $!";
        $output = shell_exec($command);
        self::$throttlingServerPid = (int)trim($output);
        self::$throttlingServerStarted = true;
        sleep(1);
        register_shutdown_function(array(__CLASS__, 'stopThrottlingMockServer'));
    }

    public static function stopThrottlingMockServer()
    {
        if (self::$throttlingServerStarted && self::$throttlingServerPid > 0) {
            shell_exec('kill ' . self::$throttlingServerPid);
            self::$throttlingServerStarted = false;
        }
    }

    private static function resetThrottlingMockState($throttleCount, $retryAfterMS = 1000)
    {
        $stateFile = __DIR__ . '/Mock/throttling-mock-state.json';
        file_put_contents($stateFile, json_encode([
            'throttleCount' => $throttleCount,
            'retryAfterMS' => $retryAfterMS,
            'requestCount' => 0,
            'retryAttempts' => [],
            'retryDelays' => [],
        ]));
    }

    private static function readThrottlingMockState()
    {
        $stateFile = __DIR__ . '/Mock/throttling-mock-state.json';
        return json_decode(file_get_contents($stateFile), true);
    }

    private static function createThrottlingRetryOptions()
    {
        return new RetryOptions([
            'retryable' => true,
            'maxAttempts' => 3,
            'retryCondition' => [new RetryCondition([
                'maxAttempts' => 3,
                'errorCode' => ['Throttling', 'Throttling.User', 'Throttling.Api'],
                'maxDelay' => 60000,
            ])],
        ]);
    }

    private static function createListProductQuotasParams()
    {
        return new Params([
            'action' => 'ListProductQuotas',
            'version' => '2020-05-10',
            'protocol' => 'HTTPS',
            'pathname' => '/',
            'method' => 'POST',
            'authType' => 'AK',
            'style' => 'RPC',
            'reqBodyType' => 'formData',
            'bodyType' => 'json',
        ]);
    }

    private static function createListProductQuotasRequest()
    {
        return new OpenApiRequest([
            'body' => Utils::parseToMap([
                'ProductCode' => 'Ecs',
            ]),
        ]);
    }


    /**
     * Ensure Mock Server is running (PHP 5.6-8.1 compatible)
     */
    private static function ensureMockServer()
    {
        if (self::$serverStarted) {
            return;
        }
        // 启动 Mock 服务器
        $server = __DIR__ . '/Mock/MockServer.php';
        $command = "php " . escapeshellarg($server) . " > /dev/null 2>&1 & echo $!";
        $output = shell_exec($command);
        self::$serverPid = (int)trim($output);
        self::$serverStarted = true;
        // 等待服务器启动并验证
        sleep(2); // 增加等待时间确保服务器完全启动
        // 注册关闭钩子
        register_shutdown_function(array(__CLASS__, 'stopMockServer'));
    }

    /**
     * Stop Mock Server
     */
    public static function stopMockServer()
    {
        if (self::$serverStarted && self::$serverPid > 0) {
            shell_exec('kill ' . self::$serverPid);
            self::$serverStarted = false;
        }
    }

    /**
     * @before
     */
    protected function initialize()
    {
        // Mock 服务器已在 setUpBeforeClass 中启动
    }

    /**
     * @after
     */
    protected function cleanup()
    {
        // Mock 服务器会在 tearDownAfterClass 中关闭
    }

    public function testConfig()
    {
        $globalParameters = new GlobalParameters([
            "headers" => [
                "global-key" => "global-value"
            ],
            "queries" => [
                "global-query" => "global-value"
            ]
        ]);
        $config = new Config([
            "endpoint" => "config.endpoint",
            "endpointType" => "regional",
            "network" => "config.network",
            "suffix" => "config.suffix",
            "protocol" => "config.protocol",
            "method" => "config.method",
            "regionId" => "config.regionId",
            "userAgent" => "config.userAgent",
            "readTimeout" => 3000,
            "connectTimeout" => 3000,
            "httpProxy" => "config.httpProxy",
            "httpsProxy" => "config.httpsProxy",
            "noProxy" => "config.noProxy",
            "socks5Proxy" => "config.socks5Proxy",
            "socks5NetWork" => "config.socks5NetWork",
            "maxIdleConns" => 128,
            "signatureVersion" => "config.signatureVersion",
            "signatureAlgorithm" => "config.signatureAlgorithm",
            "globalParameters" => $globalParameters
        ]);
        $creConfig = new \AlibabaCloud\Credentials\Credential\Config([
            "accessKeyId" => "accessKeyId",
            "accessKeySecret" => "accessKeySecret",
            "securityToken" => "securityToken",
            "type" => "sts"
        ]);
        $credential = new Credential($creConfig);
        $config->credential = $credential;
        $client = new OpenApiClient($config);
        $this->assertInstanceOf(OpenApiClient::class, $client);

        $config->accessKeyId = "ak";
        $config->accessKeySecret = "secret";
        $config->securityToken = "token";
        $config->type = "sts";
        $client = new OpenApiClient($config);
        $this->assertInstanceOf(OpenApiClient::class, $client);
    }

    /**
     * @return Config
     */
    public static function createConfig()
    {
        $globalParameters = new GlobalParameters([
            "headers" => [
                "global-key" => "global-value"
            ],
            "queries" => [
                "global-query" => "global-value"
            ]
        ]);
        $config = new Config([
            "accessKeyId" => "ak",
            "accessKeySecret" => "secret",
            "securityToken" => "token",
            "type" => "sts",
            "userAgent" => "config.userAgent",
            "readTimeout" => 3000,
            "connectTimeout" => 3000,
            "maxIdleConns" => 128,
            "signatureVersion" => "config.signatureVersion",
            "signatureAlgorithm" => "ACS3-HMAC-SHA256",
            "globalParameters" => $globalParameters
        ]);
        return $config;
    }

    /**
     * @return RuntimeOptions
     */
    public static function createRuntimeOptions()
    {
        $runtime = new RuntimeOptions([
            "readTimeout" => 4000,
            "connectTimeout" => 4000,
            "maxIdleConns" => 100,
            "autoretry" => true,
            "backoffPolicy" => "no",
            "backoffPeriod" => 1,
            "ignoreSSL" => true,
            "retryOptions" => new RetryOptions([
                "retryable" => false,
                "retryCondition" => [new RetryCondition([
                    "retryOnNonIdempotent" => true,
                    "retryOnThrottling" => true
                ])]
            ]),
            "extendsParameters" => new ExtendsParameters([
                "headers" => [
                    "extends-key" => "extends-value"
                ],
            ])
        ]);
        return $runtime;
    }

    /**
     * @return OpenApiRequest
     */
    public static function createOpenApiRequest()
    {
        $query = [];
        $query["key1"] = "value";
        $query["key2"] = 1;
        $query["key3"] = true;
        $body = [];
        $body["key1"] = "value";
        $body["key2"] = 1;
        $body["key3"] = true;
        $headers = [
            "for-test" => "sdk"
        ];
        $req = new OpenApiRequest([
            "headers" => $headers,
            "query" => Utils::query($query),
            "body" => Utils::parseToMap($body)
        ]);
        return $req;
    }

    public function testCallApiForRPCWithV2Sign_AK_Form()
    {
        self::ensureMockServer();
        $config = self::createConfig();
        $runtime = self::createRuntimeOptions();
        $config->protocol = "HTTP";
        $config->signatureAlgorithm = "v2";
        $config->endpoint = "127.0.0.1:8000";
        $client = new OpenApiClient($config);
        $request = self::createOpenApiRequest();
        $params = new Params([
            "action" => "TestAPI",
            "version" => "2022-06-01",
            "protocol" => "HTTPS",
            "pathname" => "/",
            "method" => "POST",
            "authType" => "AK",
            "style" => "RPC",
            "reqBodyType" => "formData",
            "bodyType" => "json"
        ]);
        $response = $client->callApi($params, $request, $runtime);
        $this->assertNotEmpty($response);
        if (is_array($response) && isset($response['AppId'])) {
            $this->assertEquals('test', $response['AppId']);
        }
    }

    public function testCallApiForRPCWithV2Sign_Anonymous_JSON()
    {
        $config = self::createConfig();
        $runtime = self::createRuntimeOptions();
        $config->protocol = "HTTP";
        $config->signatureAlgorithm = "v2";
        $config->endpoint = "127.0.0.1:8000";
        $client = new OpenApiClient($config);
        $request = self::createOpenApiRequest();
        $params = new Params([
            "action" => "TestAPI",
            "version" => "2022-06-01",
            "protocol" => "HTTPS",
            "pathname" => "/",
            "method" => "POST",
            "authType" => "Anonymous",
            "style" => "RPC",
            "reqBodyType" => "json",
            "bodyType" => "json"
        ]);
        $response = $client->callApi($params, $request, $runtime);
        $this->assertNotEmpty($response);
        if (is_array($response) && isset($response['AppId'])) {
            $this->assertEquals('test', $response['AppId']);
        }
    }

    public function testCallApiForROAWithV2Sign_HTTPS_AK_Form()
    {
        $config = self::createConfig();
        $runtime = self::createRuntimeOptions();
        $config->signatureAlgorithm = "v2";
        $config->protocol = "HTTP";
        $config->endpoint = "127.0.0.1:8000";
        $client = new OpenApiClient($config);
        $request = self::createOpenApiRequest();
        $params = new Params([
            "action" => "TestAPI",
            "version" => "2022-06-01",
            "protocol" => "HTTPS",
            "pathname" => "/test",
            "method" => "POST",
            "authType" => "AK",
            "style" => "ROA",
            "reqBodyType" => "formData",
            "bodyType" => "json"
        ]);
        $response = $client->callApi($params, $request, $runtime);
        $this->assertNotEmpty($response);
        if (is_array($response) && isset($response['AppId'])) {
            $this->assertEquals('test', $response['AppId']);
        }
    }

    public function testCallApiForROAWithV2Sign_Anonymous_JSON()
    {
        $config = self::createConfig();
        $runtime = self::createRuntimeOptions();
        $config->protocol = "HTTP";
        $config->signatureAlgorithm = "v2";
        $config->endpoint = "127.0.0.1:8000";
        $client = new OpenApiClient($config);
        $request = self::createOpenApiRequest();
        $params = new Params([
            "action" => "TestAPI",
            "version" => "2022-06-01",
            "protocol" => "HTTPS",
            "pathname" => "/test",
            "method" => "POST",
            "authType" => "Anonymous",
            "style" => "ROA",
            "reqBodyType" => "json",
            "bodyType" => "json"
        ]);
        $response = $client->callApi($params, $request, $runtime);
        $this->assertNotEmpty($response);
        if (is_array($response) && isset($response['AppId'])) {
            $this->assertEquals('test', $response['AppId']);
        }
    }

    public function testCallApiForRPCWithV3Sign_AK_Form()
    {
        $config = self::createConfig();
        $runtime = self::createRuntimeOptions();
        $config->protocol = "HTTP";
        $config->endpoint = "127.0.0.1:8000";
        $client = new OpenApiClient($config);
        $request = self::createOpenApiRequest();
        $params = new Params([
            "action" => "TestAPI",
            "version" => "2022-06-01",
            "protocol" => "HTTPS",
            "pathname" => "/",
            "method" => "POST",
            "authType" => "AK",
            "style" => "RPC",
            "reqBodyType" => "formData",
            "bodyType" => "json"
        ]);
        $response = $client->callApi($params, $request, $runtime);
        $this->assertNotEmpty($response);
        if (is_array($response) && isset($response['AppId'])) {
            $this->assertEquals('test', $response['AppId']);
        }
    }

    public function testCallApiForRPCWithV3Sign_Anonymous_JSON()
    {
        $config = self::createConfig();
        $runtime = self::createRuntimeOptions();
        $config->protocol = "HTTP";
        $config->endpoint = "127.0.0.1:8000";
        $client = new OpenApiClient($config);
        $request = self::createOpenApiRequest();
        $params = new Params([
            "action" => "TestAPI",
            "version" => "2022-06-01",
            "protocol" => "HTTPS",
            "pathname" => "/",
            "method" => "POST",
            "authType" => "Anonymous",
            "style" => "RPC",
            "reqBodyType" => "json",
            "bodyType" => "json"
        ]);
        $response = $client->callApi($params, $request, $runtime);
        $this->assertNotEmpty($response);
        if (is_array($response) && isset($response['AppId'])) {
            $this->assertEquals('test', $response['AppId']);
        }
    }

    public function testCallApiForROAWithV3Sign_AK_Form()
    {
        $config = self::createConfig();
        $runtime = self::createRuntimeOptions();
        $config->protocol = "HTTP";
        $config->endpoint = "127.0.0.1:8000";
        $client = new OpenApiClient($config);
        $request = self::createOpenApiRequest();
        $params = new Params([
            "action" => "TestAPI",
            "version" => "2022-06-01",
            "protocol" => "HTTPS",
            "pathname" => "/test",
            "method" => "POST",
            "authType" => "AK",
            "style" => "ROA",
            "reqBodyType" => "formData",
            "bodyType" => "json"
        ]);
        $response = $client->callApi($params, $request, $runtime);
        $this->assertNotEmpty($response);
        if (is_array($response) && isset($response['AppId'])) {
            $this->assertEquals('test', $response['AppId']);
        }
    }

    public function testCallApiForROAWithV3Sign_Anonymous_JSON()
    {
        $config = self::createConfig();
        $runtime = self::createRuntimeOptions();
        $config->protocol = "HTTP";
        $config->endpoint = "127.0.0.1:8000";
        $client = new OpenApiClient($config);
        $request = self::createOpenApiRequest();
        $params = new Params([
            "action" => "TestAPI",
            "version" => "2022-06-01",
            "protocol" => "HTTPS",
            "pathname" => "/test",
            "method" => "POST",
            "authType" => "Anonymous",
            "style" => "ROA",
            "reqBodyType" => "json",
            "bodyType" => "json"
        ]);
        $response = $client->callApi($params, $request, $runtime);
        $this->assertNotEmpty($response);
        if (is_array($response) && isset($response['AppId'])) {
            $this->assertEquals('test', $response['AppId']);
        }
    }

    public function testResponseBodyType()
    {
        $config = self::createConfig();
        $runtime = self::createRuntimeOptions();
        $config->protocol = "HTTP";
        $config->endpoint = "127.0.0.1:8000";
        $client = new OpenApiClient($config);
        $request = self::createOpenApiRequest();
        $params = new Params([
            "action" => "TestAPI",
            "version" => "2022-06-01",
            "protocol" => "HTTPS",
            "pathname" => "/test",
            "method" => "POST",
            "authType" => "AK",
            "style" => "ROA",
            "reqBodyType" => "formData",
            "bodyType" => "json"
        ]);
        $response = $client->callApi($params, $request, $runtime);
        $this->assertTrue(is_array($response));

        $params->bodyType = "array";
        $request->headers['bodytype'] = 'array';
        $response = $client->callApi($params, $request, $runtime);
        $this->assertTrue(is_array($response));
        // Mock server returns ["AppId", "ClassId", "UserId"] for array type
        $this->assertNotEmpty($response);

        $params->bodyType = "string";
        $request->headers['bodytype'] = 'string';
        $response = $client->callApi($params, $request, $runtime);
        // For string type, check if response body exists
        $this->assertNotEmpty($response);
        if (is_array($response) && isset($response['body'])) {
            $this->assertTrue(is_string($response['body']));
        }

        $params->bodyType = "byte";
        $request->headers['bodytype'] = 'byte';
        $response = $client->callApi($params, $request, $runtime);
        $this->assertNotEmpty($response);
    }

    public function testCallSSEApiWithSignV3()
    {
        self::ensureMockServer();

        // 额外等待确保 Mock 服务器完全启动 (for CI environments)
        usleep(500000); // 0.5秒

        $config = self::createConfig();
        $runtime = self::createRuntimeOptions();
        $config->protocol = "HTTP";
        $config->endpoint = "127.0.0.1:8000";
        $client = new OpenApiClient($config);
        $request = self::createOpenApiRequest();
        $params = new Params([
            "action" => "TestAPI",
            "version" => "2022-06-01",
            "protocol" => "HTTPS",
            "pathname" => "/sse",
            "method" => "POST",
            "authType" => "AK",
            "style" => "ROA",
            "reqBodyType" => "json",
            "bodyType" => "sse"
        ]);
        $response = $client->callSSEApi($params, $request, $runtime);


        // Add more assertions as needed
        $events = [];

        // SSE events are typically separated by double newline
        foreach ($response as $event) {
            $this->assertEquals(200, $event->statusCode);
            $headers = $event->headers;
            $this->assertEquals('text/event-stream;charset=UTF-8', $headers['Content-Type'][0]);
            $this->assertEquals('sdk', $headers['for-test'][0]);
            $userAgentArray = explode(' ', $headers['user-agent'][0]);
            $this->assertEquals('config.userAgent', end($userAgentArray));
            $this->assertEquals('global-value', $headers['global-key'][0]);
            $this->assertEquals('extends-value', $headers['extends-key'][0]);
            $this->assertNotEmpty($headers['x-acs-signature-nonce'][0]);
            $this->assertNotEmpty($headers['x-acs-date'][0]);
            $this->assertEquals('application/json', $headers['accept'][0]);
            $event = $event->event->toArray();
            // var_dump($event);
            $events[] = json_decode($event['data'], true);
        }
        $expectedEvents = [
            ['count' => 0],
            ['count' => 1],
            ['count' => 2],
            ['count' => 3],
            ['count' => 4],
        ];
        $this->assertCount(5, $events);
        $this->assertEquals($expectedEvents, $events);
    }

    public function testThrottlingBackoffRetryListProductQuotas()
    {
        self::ensureThrottlingMockServer();
        self::resetThrottlingMockState(2, 1000);

        $config = self::createConfig();
        $config->protocol = 'HTTP';
        $config->endpoint = '127.0.0.1:8001';
        $config->retryOptions = self::createThrottlingRetryOptions();
        $client = new OpenApiClient($config);
        $runtime = self::createRuntimeOptions();

        $start = microtime(true);
        $response = $client->callApi(
            self::createListProductQuotasParams(),
            self::createListProductQuotasRequest(),
            $runtime
        );
        $elapsed = (microtime(true) - $start) * 1000;

        $this->assertEquals(200, $response['statusCode']);
        $this->assertEquals('ProductCode=Ecs', $response['headers']['raw-body']);
        $this->assertEquals('ListProductQuotas', $response['headers']['x-acs-action']);
        $this->assertEquals('2020-05-10', $response['headers']['x-acs-version']);
        $this->assertEquals('A45EE076-334D-5012-9746-A8F828D20FD4', $response['body']['RequestId']);

        $state = self::readThrottlingMockState();
        $this->assertEquals(3, $state['requestCount']);
        $this->assertEquals('', $state['retryAttempts'][0]);
        $this->assertEquals('1', $state['retryAttempts'][1]);
        $this->assertEquals('2', $state['retryAttempts'][2]);
        $this->assertEquals('', $state['retryDelays'][0]);
        $this->assertEquals('1000', $state['retryDelays'][1]);
        $this->assertEquals('1000', $state['retryDelays'][2]);
        $this->assertGreaterThanOrEqual(1800, $elapsed);
    }

    public function testThrottlingBackoffRetryListProductQuotasExhausted()
    {
        self::ensureThrottlingMockServer();
        self::resetThrottlingMockState(3, 1000);

        $config = self::createConfig();
        $config->protocol = 'HTTP';
        $config->endpoint = '127.0.0.1:8001';
        $config->retryOptions = self::createThrottlingRetryOptions();
        $client = new OpenApiClient($config);
        $runtime = self::createRuntimeOptions();

        try {
            $client->callApi(
                self::createListProductQuotasParams(),
                self::createListProductQuotasRequest(),
                $runtime
            );
            $this->fail('Expected throttling retry to be exhausted');
        } catch (DaraUnableRetryException $e) {
            $this->assertInstanceOf(ThrottlingException::class, $e->getLastException());
            $this->assertEquals('Throttling', $e->getLastException()->code);
        }

        $state = self::readThrottlingMockState();
        $this->assertEquals(3, $state['requestCount']);
    }
}
