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
    private static $serverProcess = null;
    private static $serverPid = 0;
    private static $mockServerPort = 8000;
    private static $throttlingServerStarted = false;
    private static $throttlingServerProcess = null;
    private static $throttlingServerPort = 0;

    /**
     * Ensure throttling mock server is running on a dynamic local port.
     */
    private static function ensureThrottlingMockServer()
    {
        if (self::$throttlingServerStarted && self::isThrottlingMockServerReady()) {
            return;
        }

        self::stopThrottlingMockServer();

        $server = __DIR__ . '/Mock/ThrottlingMockServer.php';
        $descriptors = [
            0 => ['pipe', 'r'],
            1 => ['file', '/dev/null', 'w'],
            2 => ['file', '/dev/null', 'w'],
        ];
        $process = proc_open('php ' . escapeshellarg($server), $descriptors, $pipes);
        if (!is_resource($process)) {
            throw new \RuntimeException('Failed to start throttling mock server process');
        }
        fclose($pipes[0]);
        self::$throttlingServerProcess = $process;

        $deadline = microtime(true) + 10;
        while (microtime(true) < $deadline) {
            if (self::isThrottlingMockServerReady()) {
                self::$throttlingServerStarted = true;
                register_shutdown_function(array(__CLASS__, 'stopThrottlingMockServer'));
                return;
            }
            usleep(100000);
        }

        self::stopThrottlingMockServer();
        throw new \RuntimeException('Failed to start throttling mock server');
    }

    private static function isThrottlingMockServerReady()
    {
        $portFile = __DIR__ . '/Mock/throttling-mock-port.txt';
        if (!file_exists($portFile)) {
            return false;
        }

        $port = (int) trim((string) file_get_contents($portFile));
        if ($port <= 0) {
            return false;
        }

        $connection = @fsockopen('127.0.0.1', $port, $errno, $errstr, 0.2);
        if ($connection === false) {
            return false;
        }

        fclose($connection);
        self::$throttlingServerPort = $port;
        return true;
    }

    private static function getThrottlingEndpoint()
    {
        self::ensureThrottlingMockServer();
        return '127.0.0.1:' . self::$throttlingServerPort;
    }

    private static function getResponseHeader($headers, $name)
    {
        if (!is_array($headers)) {
            return null;
        }

        $key = strtolower($name);
        $value = null;
        if (isset($headers[$key])) {
            $value = $headers[$key];
        } elseif (isset($headers[$name])) {
            $value = $headers[$name];
        } else {
            foreach ($headers as $headerName => $headerValue) {
                if (strtolower($headerName) === $key) {
                    $value = $headerValue;
                    break;
                }
            }
        }

        if ($value === null) {
            return null;
        }

        if (is_array($value)) {
            return $value[0];
        }

        return $value;
    }

    public static function stopThrottlingMockServer()
    {
        if (is_resource(self::$throttlingServerProcess)) {
            @proc_terminate(self::$throttlingServerProcess);
            @proc_close(self::$throttlingServerProcess);
            self::$throttlingServerProcess = null;
        }

        $portFile = __DIR__ . '/Mock/throttling-mock-port.txt';
        if (file_exists($portFile)) {
            @unlink($portFile);
        }

        self::$throttlingServerStarted = false;
        self::$throttlingServerPort = 0;
    }

    private static function resetThrottlingMockState($throttleCount, $retryAfterMS = 1)
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
     * Ensure Mock Server is running and accepting connections (PHP 5.6+).
     * Uses proc_open + readiness probe so CI (esp. PHP 5.6) does not race
     * the first SSE request against a half-started listener on :8000.
     */
    private static function ensureMockServer()
    {
        if (self::$serverStarted && self::isMockServerReady()) {
            return;
        }

        self::stopMockServer();

        $server = __DIR__ . '/Mock/MockServer.php';
        $descriptors = [
            0 => ['pipe', 'r'],
            1 => ['file', '/dev/null', 'w'],
            2 => ['file', '/dev/null', 'w'],
        ];
        $process = proc_open('php ' . escapeshellarg($server), $descriptors, $pipes);
        if (!is_resource($process)) {
            throw new \RuntimeException('Failed to start mock server process');
        }
        fclose($pipes[0]);
        self::$serverProcess = $process;
        $status = proc_get_status($process);
        self::$serverPid = isset($status['pid']) ? (int) $status['pid'] : 0;

        $deadline = microtime(true) + 10;
        while (microtime(true) < $deadline) {
            if (self::isMockServerReady()) {
                self::$serverStarted = true;
                register_shutdown_function(array(__CLASS__, 'stopMockServer'));
                return;
            }
            usleep(100000);
        }

        self::stopMockServer();
        throw new \RuntimeException('Failed to start mock server on 127.0.0.1:' . self::$mockServerPort);
    }

    private static function isMockServerReady()
    {
        $connection = @fsockopen('127.0.0.1', self::$mockServerPort, $errno, $errstr, 0.2);
        if ($connection === false) {
            return false;
        }
        fclose($connection);
        return true;
    }

    private static function getMockEndpoint()
    {
        self::ensureMockServer();
        return '127.0.0.1:' . self::$mockServerPort;
    }

    /**
     * Stop Mock Server
     */
    public static function stopMockServer()
    {
        if (is_resource(self::$serverProcess)) {
            @proc_terminate(self::$serverProcess);
            @proc_close(self::$serverProcess);
            self::$serverProcess = null;
        } elseif (self::$serverPid > 0) {
            @shell_exec('kill ' . self::$serverPid . ' 2>/dev/null');
        }

        self::$serverPid = 0;
        self::$serverStarted = false;
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

        $config = self::createConfig();
        $runtime = self::createRuntimeOptions();
        // SSE streams 5 events; give the client enough time on slow CI (PHP 5.6).
        $runtime->readTimeout = 15000;
        $config->protocol = "HTTP";
        $config->endpoint = self::getMockEndpoint();
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
            $this->assertEquals('text/event-stream;charset=UTF-8', self::getResponseHeader($headers, 'Content-Type'));
            $this->assertEquals('sdk', self::getResponseHeader($headers, 'for-test'));
            $userAgentArray = explode(' ', self::getResponseHeader($headers, 'user-agent'));
            $this->assertEquals('config.userAgent', end($userAgentArray));
            $this->assertEquals('global-value', self::getResponseHeader($headers, 'global-key'));
            $this->assertEquals('extends-value', self::getResponseHeader($headers, 'extends-key'));
            $this->assertNotEmpty(self::getResponseHeader($headers, 'x-acs-signature-nonce'));
            $this->assertNotEmpty(self::getResponseHeader($headers, 'x-acs-date'));
            $this->assertEquals('application/json', self::getResponseHeader($headers, 'accept'));
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
        self::resetThrottlingMockState(2, 1);

        $config = self::createConfig();
        $config->protocol = 'HTTP';
        $config->endpoint = self::getThrottlingEndpoint();
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
        $headers = $response['headers'];
        $this->assertEquals('ProductCode=Ecs', self::getResponseHeader($headers, 'raw-body'));
        $this->assertEquals('ListProductQuotas', self::getResponseHeader($headers, 'x-acs-action'));
        $this->assertEquals('2020-05-10', self::getResponseHeader($headers, 'x-acs-version'));
        $this->assertEquals('A45EE076-334D-5012-9746-A8F828D20FD4', $response['body']['RequestId']);

        $state = self::readThrottlingMockState();
        $this->assertEquals(3, $state['requestCount']);
        $this->assertEquals('', $state['retryAttempts'][0]);
        $this->assertEquals('1', $state['retryAttempts'][1]);
        $this->assertEquals('2', $state['retryAttempts'][2]);
        $this->assertEquals('', $state['retryDelays'][0]);
        $this->assertEquals('1', $state['retryDelays'][1]);
        $this->assertEquals('1', $state['retryDelays'][2]);
        $this->assertGreaterThanOrEqual(1800, $elapsed);
    }

    public function testThrottlingBackoffRetryListProductQuotasExhausted()
    {
        self::ensureThrottlingMockServer();
        self::resetThrottlingMockState(2, 1);

        $config = self::createConfig();
        $config->protocol = 'HTTP';
        $config->endpoint = self::getThrottlingEndpoint();
        $config->retryOptions = new RetryOptions([
            'retryable' => true,
            'maxAttempts' => 2,
            'retryCondition' => [new RetryCondition([
                'maxAttempts' => 2,
                'errorCode' => ['Throttling', 'Throttling.User', 'Throttling.Api'],
                'maxDelay' => 60000,
            ])],
        ]);
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
        $this->assertEquals(2, $state['requestCount']);
    }
}
