<?php

namespace Darabonba\OpenApi\Tests;

use AlibabaCloud\Dara\AbstractWebSocketHandler;
use AlibabaCloud\Dara\Models\RuntimeOptions;
use AlibabaCloud\Dara\WebSocketMessage;
use Darabonba\OpenApi\Exceptions\AlibabaCloudException;
use Darabonba\OpenApi\Exceptions\ClientException;
use Darabonba\OpenApi\Exceptions\ServerException;
use Darabonba\OpenApi\Exceptions\ThrottlingException;
use Darabonba\OpenApi\Models\Config;
use Darabonba\OpenApi\Models\GlobalParameters;
use Darabonba\OpenApi\Models\OpenApiRequest;
use Darabonba\OpenApi\Models\Params;
use Darabonba\OpenApi\Models\SSEResponse;
use Darabonba\OpenApi\OpenApiClient;
use Darabonba\OpenApi\Utils;
use Darabonba\OpenApi\WebsocketUtils\Awap;
use Darabonba\OpenApi\WebsocketUtils\AwapUtil;
use Darabonba\OpenApi\WebsocketUtils\Client as WebSocketClient;
use Darabonba\OpenApi\WebsocketUtils\General;
use Darabonba\OpenApi\WebsocketUtils\GeneralUtil;
use Darabonba\OpenApi\WebsocketUtils\Handler;
use PHPUnit\Framework\TestCase;

class_exists('Darabonba\\OpenApi\\WebsocketUtils\\Awap');
class_exists('Darabonba\\OpenApi\\WebsocketUtils\\General');

class ExtraRecordingHandler extends AbstractWebSocketHandler implements \Darabonba\OpenApi\WebsocketUtils\AwapWebSocketHandler, \Darabonba\OpenApi\WebsocketUtils\GeneralWebSocketHandler
{
    public $established = false;
    public $closed = false;
    public $errors = array();
    public $raw = 0;
    public $awap = 0;
    public $general = 0;
    public $awapErr = null;

    public function afterConnectionEstablished($session)
    {
        $this->established = true;
    }

    public function afterConnectionClosed($session, $code, $reason)
    {
        $this->closed = true;
    }

    public function handleError($session, $err)
    {
        $this->errors[] = $err;
    }

    public function handleRawMessage($session, $message)
    {
        $this->raw++;
    }

    public function handleAwapMessage($session, $message)
    {
        $this->awap++;
        if ($this->awapErr !== null) {
            throw $this->awapErr;
        }
    }

    public function handleGeneralMessage($session, $message)
    {
        $this->general++;
    }
}

class ExtraRecordingWsClient implements \AlibabaCloud\Dara\WebSocketClientInterface
{
    public $texts = array();
    public $binaries = array();
    public $closed = false;
    public $connected = true;

    public function connect($request, $runtimeObject) {}
    public function disconnect() {}
    public function reconnect() { return null; }
    public function reconnectGracefully() { return null; }
    public function isConnected() { return $this->connected; }
    public function sendText($text) { $this->texts[] = $text; }
    public function sendBinary($data) { $this->binaries[] = $data; }
    public function getSessionInfo() { return null; }
    public function close() { $this->closed = true; }
    public function pump($timeoutMs = 100) {}
}

class ExtraPlainHandler extends AbstractWebSocketHandler
{
}

class ExtraRawFallbackHandler extends AbstractWebSocketHandler implements \Darabonba\OpenApi\WebsocketUtils\AwapWebSocketHandler, \Darabonba\OpenApi\WebsocketUtils\GeneralWebSocketHandler
{
    public $raw = 0;
    public $rawErr = null;

    public function handleAwapMessage($session, $message)
    {
        throw Handler::errUseRawMessage();
    }

    public function handleGeneralMessage($session, $message)
    {
        throw Handler::errUseRawMessage();
    }

    public function handleRawMessage($session, $message)
    {
        $this->raw++;
        if ($this->rawErr !== null) {
            throw $this->rawErr;
        }
    }
}

class ExtraCoverageTest extends TestCase
{
    private function assertMessageContains($haystack, $needle)
    {
        $this->assertTrue(strpos($haystack, $needle) !== false, $haystack);
    }

    private function exceptionMap($overrides = array())
    {
        return array_merge(array(
            'statusCode' => 400,
            'code' => 'X',
            'message' => 'm',
            'detail' => 'd',
            'description' => 'desc',
            'requestId' => 'rid',
            'accessDeniedDetail' => array('k' => 'v'),
        ), $overrides);
    }

    public function testConstructorsHelpersAndExceptions()
    {
        $caught = false;
        try {
            new OpenApiClient(null);
        } catch (\Exception $e) {
            $caught = true;
        }
        $this->assertTrue($caught);

        $empty = new OpenApiClient(new Config(array('endpoint' => 'example.com')));
        $this->assertSame('', $empty->getAccessKeyId());
        $this->assertSame('', $empty->getAccessKeySecret());
        $this->assertSame('', $empty->getSecurityToken());
        $this->assertSame('', $empty->getBearerToken());
        $this->assertSame('', $empty->getType());

        $caught = false;
        try {
            $empty->checkConfig(new Config());
        } catch (\Exception $e) {
            $caught = true;
        }
        $this->assertTrue($caught);
        $empty->checkConfig(new Config(array('endpoint' => 'example.com')));

        $this->assertEquals(array('k' => 'v'), $empty->getAccessDeniedDetail(array('AccessDeniedDetail' => array('k' => 'v'))));
        $this->assertEquals(array('k' => 'v2'), $empty->getAccessDeniedDetail(array('accessDeniedDetail' => array('k' => 'v2'))));
        $this->assertNull($empty->getAccessDeniedDetail(array()));

        $empty->setRpcHeaders(array('debug' => '1'));
        $this->assertEquals(array('debug' => '1'), $empty->getRpcHeaders());
        $this->assertNull($empty->getRpcHeaders());

        $caught = false;
        try {
            $empty->callApi(null, new OpenApiRequest(), new RuntimeOptions());
        } catch (\Exception $e) {
            $caught = true;
        }
        $this->assertTrue($caught);

        $clientExc = new ClientException($this->exceptionMap(array('code' => 'Client.Error')));
        $this->assertEquals('Client.Error', $clientExc->code);
        $this->assertEquals(array('k' => 'v'), $clientExc->getAccessDeniedDetail());
        $serverExc = new ServerException($this->exceptionMap(array('statusCode' => 500, 'code' => 'Internal')));
        $this->assertEquals('Internal', $serverExc->code);
        $throttling = new ThrottlingException($this->exceptionMap(array(
            'statusCode' => 429,
            'code' => 'Throttling',
            'retryAfter' => 1000,
        )));
        $this->assertEquals(1000, $throttling->getRetryAfter());
        $cloud = new AlibabaCloudException($this->exceptionMap());
        $this->assertEquals('rid', $cloud->getRequestId());
        $this->assertEquals(400, $cloud->getStatusCode());
        $this->assertEquals('d', $cloud->getDetail());
        $this->assertEquals('desc', $cloud->getDescription());

        putenv('ALIBABA_CLOUD_USER_AGENT=from-env');
        $envClient = new OpenApiClient(new Config(array(
            'accessKeyId' => 'ak',
            'accessKeySecret' => 'sk',
            'endpoint' => 'example.com',
        )));
        $this->assertEquals('ak', $envClient->getAccessKeyId());
        $this->assertEquals('sk', $envClient->getAccessKeySecret());
        $this->assertEquals('access_key', $envClient->getType());
        putenv('ALIBABA_CLOUD_USER_AGENT');

        $bearer = new OpenApiClient(new Config(array(
            'bearerToken' => 'token',
            'endpoint' => 'example.com',
        )));
        $this->assertEquals('token', $bearer->getBearerToken());
        $this->assertEquals('bearer', $bearer->getType());
    }

    public function testModelsAndUtilsRetryHeaders()
    {
        $global = GlobalParameters::fromMap(array(
            'headers' => array('h' => '1'),
            'queries' => array('q' => '2'),
        ));
        $global->validate();
        $this->assertEquals('1', $global->toArray()['headers']['h']);
        $this->assertEquals('2', $global->toMap()['queries']['q']);

        $params = Params::fromMap(array(
            'action' => 'A',
            'version' => '2022-06-01',
            'protocol' => 'HTTPS',
            'pathname' => '/',
            'method' => 'POST',
            'authType' => 'AK',
            'bodyType' => 'json',
            'reqBodyType' => 'json',
            'style' => 'RPC',
            'websocketSubProtocol' => 'awap',
        ));
        $params->validate();
        $this->assertEquals('A', $params->toArray()['action']);
        $this->assertEquals('RPC', $params->toMap()['style']);

        $request = OpenApiRequest::fromMap(array(
            'headers' => array('for-test' => 'sdk'),
            'query' => array('k' => 'v'),
            'body' => array('b' => '1'),
            'hostMap' => array('region' => 'cn-hangzhou'),
            'endpointOverride' => 'override.example.com',
        ));
        $request->validate();
        $this->assertEquals('sdk', $request->toArray()['headers']['for-test']);

        $sse = SSEResponse::fromMap(array(
            'headers' => array('k' => 'v'),
            'statusCode' => 200,
        ));
        $this->assertEquals(200, $sse->toArray()['statusCode']);
        $this->assertEquals(200, $sse->toMap()['statusCode']);

        $config = Config::fromMap(array(
            'accessKeyId' => 'ak',
            'accessKeySecret' => 'sk',
            'securityToken' => 'st',
            'bearerToken' => 'bt',
            'protocol' => 'HTTP',
            'method' => 'POST',
            'regionId' => 'cn-hangzhou',
            'readTimeout' => 1000,
            'connectTimeout' => 1000,
            'httpProxy' => 'http://127.0.0.1',
            'httpsProxy' => 'https://127.0.0.1',
            'endpoint' => 'example.com',
            'noProxy' => 'localhost',
            'maxIdleConns' => 10,
            'network' => 'public',
            'userAgent' => 'ua',
            'suffix' => 'sfx',
            'socks5Proxy' => 'socks5://127.0.0.1',
            'socks5NetWork' => 'tcp',
            'endpointType' => 'regional',
            'openPlatformEndpoint' => 'open.aliyun.com',
            'type' => 'access_key',
            'signatureVersion' => 'v4',
            'signatureAlgorithm' => 'ACS3-HMAC-SHA256',
            'key' => 'k',
            'cert' => 'c',
            'ca' => 'ca',
            'disableHttp2' => true,
            'tlsMinVersion' => '1.2',
        ));
        $config->validate();
        $this->assertEquals('example.com', $config->toArray()['endpoint']);
        $this->assertEquals('HTTP', $config->toMap()['protocol']);
        $this->assertEquals('ak', $config->toMap()['accessKeyId']);
        $this->assertEquals('bt', $config->toArray()['bearerToken']);

        $headers = array();
        Utils::applyRetryHeaders($headers, 0, 10);
        $this->assertFalse(isset($headers['x-acs-retry-attempts']));
        Utils::applyRetryHeaders($headers, 2, 15);
        $this->assertEquals('2', $headers['x-acs-retry-attempts']);
        $this->assertEquals('15', $headers['x-acs-retry-delay']);

        $src = Params::fromMap(array(
            'action' => 'DescribeRegions',
            'version' => '2014-05-26',
            'protocol' => 'HTTPS',
            'pathname' => '/',
            'method' => 'POST',
            'authType' => 'AK',
            'bodyType' => 'json',
            'reqBodyType' => 'json',
        ));
        $dest = new Params();
        Utils::convert($src, $dest);
        $this->assertEquals('DescribeRegions', $dest->action);
        $this->assertEquals('2014-05-26', $dest->version);

        $nested = array(
            'Name' => 'aliyun',
            'Tags' => array('a', 'b'),
            'Bytes' => 'ok',
        );
        $query = Utils::query($nested);
        $this->assertEquals('aliyun', $query['Name']);
        $form = Utils::toForm($nested);
        $this->assertTrue(strpos($form, 'Name=aliyun') !== false);
        $this->assertEquals('', Utils::toForm(null));
        $parsedNull = Utils::parseToMap(null);
        $this->assertTrue($parsedNull === null || is_array($parsedNull));
        $converted = Utils::toArray(array(array('k' => 'v')));
        $this->assertTrue(is_array($converted));
        Utils::toArray(null);
        $this->assertEquals('', Utils::arrayToStringWithSpecifiedStyle(null, 'Ids', 'json'));
        $this->assertEquals('1,2', Utils::arrayToStringWithSpecifiedStyle(array('1', '2'), 'Ids', 'simple'));
        $this->assertEquals('1 2', Utils::arrayToStringWithSpecifiedStyle(array('1', '2'), 'Ids', 'spaceDelimited'));
        $this->assertEquals('1|2', Utils::arrayToStringWithSpecifiedStyle(array('1', '2'), 'Ids', 'pipeDelimited'));

        $sm3 = new \Darabonba\OpenApi\Sm3();
        $this->assertEquals(64, strlen($sm3->sign('')));
        $this->assertEquals(64, strlen($sm3->sign(str_repeat('a', 80))));
        $this->assertEquals(64, strlen($sm3->sign(str_repeat('b', 200))));
    }

    public function testWebsocketClientAndMessages()
    {
        $this->assertNull(WebSocketClient::createWebSocketClient(null));
        $this->assertNull(WebSocketClient::createWebSocketClient('nope'));
        $inner = new ExtraRecordingWsClient();
        $client = new WebSocketClient($inner, null);
        $this->assertSame($client, WebSocketClient::createWebSocketClient($client));
        $this->assertNull($client->getResponse());
        $this->assertTrue($client->isConnected());
        $this->assertNull($client->getSessionInfo());
        $client->validate();
        $client->reconnect();
        $client->reconnectGracefully();
        $client->reconnectGracefullyAsync();
        $client->disconnect();

        $msg = AwapUtil::newAwapMessage(array('hello' => 'world'), 'UpstreamTextEvent', 'msg-001', array('custom' => 'value'));
        $msg->withHeader('extra', '1')->withFormat(Awap::MESSAGE_FORMAT_TEXT);
        $text = AwapUtil::buildAwapMessageText($msg);
        $this->assertMessageContains($text, 'type:UpstreamTextEvent');
        $this->assertMessageContains($text, 'id:msg-001');
        $this->assertMessageContains($text, 'custom:value');

        $ack = AwapUtil::newAwapMessage(array(), 'AckRequiredTextEvent', 'ack-1');
        $this->assertMessageContains(AwapUtil::buildAwapMessageText($ack), 'ack:required');
        $this->assertMessageContains(AwapUtil::buildAwapMessageText(AwapUtil::newAwapMessage(null, 'UpstreamTextEvent', 'n1')), '{}');

        $caught = false;
        try {
            AwapUtil::buildAwapMessageText(null);
        } catch (\InvalidArgumentException $e) {
            $caught = true;
        }
        $this->assertTrue($caught);

        $binMsg = AwapUtil::newAwapMessage('bin-data', 'UpstreamBinaryEvent', 'b1');
        $bin = AwapUtil::buildAwapMessageBinary($binMsg);
        $this->assertTrue(strlen($bin) > 0);
        $caught = false;
        try {
            AwapUtil::buildAwapMessageBinary(AwapUtil::newAwapMessage(array('not' => 'bytes'), 'UpstreamBinaryEvent', 'b2'));
        } catch (\InvalidArgumentException $e) {
            $caught = true;
        }
        $this->assertTrue($caught);

        $raw = new WebSocketMessage();
        $raw->type = \AlibabaCloud\Dara\WebSocketMessageType::TEXT;
        $raw->payload = $text;
        $parsed = AwapUtil::parseAwapMessage($raw);
        $this->assertEquals('UpstreamTextEvent', $parsed->type);
        $this->assertEquals('msg-001', $parsed->id);
        $this->assertEquals('value', $parsed->headers['custom']);

        $caught = false;
        try {
            $bad = new WebSocketMessage();
            $bad->payload = 'no-separator';
            AwapUtil::parseAwapMessage($bad);
        } catch (\RuntimeException $e) {
            $caught = true;
        }
        $this->assertTrue($caught);

        $jsonMsg = new WebSocketMessage();
        $jsonMsg->type = \AlibabaCloud\Dara\WebSocketMessageType::TEXT;
        $jsonMsg->payload = '{"body":"hello"}';
        $parsedJson = GeneralUtil::parseGeneralMessage($jsonMsg);
        $this->assertEquals(General::MESSAGE_FORMAT_TEXT, $parsedJson->format);

        $plain = new WebSocketMessage();
        $plain->type = \AlibabaCloud\Dara\WebSocketMessageType::TEXT;
        $plain->payload = 'not-json';
        $this->assertEquals(General::MESSAGE_FORMAT_TEXT, GeneralUtil::parseGeneralMessage($plain)->format);

        $binary = new WebSocketMessage();
        $binary->type = \AlibabaCloud\Dara\WebSocketMessageType::BINARY;
        $binary->payload = 'binary-data';
        $this->assertEquals(General::MESSAGE_FORMAT_BINARY, GeneralUtil::parseGeneralMessage($binary)->format);

        if (function_exists('random_bytes')) {
            $client->sendRawAwapTextMessage('UpstreamTextEvent', array('a' => 'b'));
            $client->sendRawAwapBinaryMessage('UpstreamBinaryEvent', 'bin');
        } else {
            $client->sendRawAwapTextMessageWithId('UpstreamTextEvent', 'id-rand-1', array('a' => 'b'));
            $client->sendRawAwapBinaryMessageWithId('UpstreamBinaryEvent', 'id-rand-2', 'bin');
        }
        $client->sendRawAwapTextMessageWithId('UpstreamTextEvent', 'id-1', array('a' => 'b'));
        $client->sendRawAwapBinaryMessageWithId('UpstreamBinaryEvent', 'id-2', 'bin2');
        $client->sendAwapTextMessage(AwapUtil::newAwapMessage(array('a' => 'b'), 'UpstreamTextEvent', 'id-send'));
        $client->sendAwapBinaryMessage(AwapUtil::newAwapMessage('bin', 'UpstreamBinaryEvent', 'id-bin'));
        $client->sendGeneralTextMessage('hello');
        $client->sendGeneralBinaryMessage('bin');
        $this->assertTrue(count($inner->texts) >= 3);
        $this->assertTrue(count($inner->binaries) >= 3);

        $caught = false;
        try {
            $client->sendRequest('', 'text', 1);
        } catch (\InvalidArgumentException $e) {
            $caught = true;
        }
        $this->assertTrue($caught);
        $caught = false;
        try {
            $client->sendRequest('id', '', 1);
        } catch (\InvalidArgumentException $e) {
            $caught = true;
        }
        $this->assertTrue($caught);
        $this->assertFalse($client->completeRequest('missing', 'x'));
        try {
            $client->sendRequest('ack-timeout', 'payload', 1);
            $this->fail('expected timeout');
        } catch (\RuntimeException $e) {
            $this->assertMessageContains($e->getMessage(), 'timeout');
        }

        $client->close();
        $this->assertTrue($inner->closed);

        $empty = new WebSocketClient(null, null);
        $this->assertFalse($empty->isConnected());
        $this->assertNull($empty->getSessionInfo());
        $empty->close();
        $empty->disconnect();
        $caught = false;
        try {
            $empty->validate();
        } catch (\RuntimeException $e) {
            $caught = true;
        }
        $this->assertTrue($caught);
    }

    public function testStreamHandlerRouting()
    {
        $session = null;
        $user = new ExtraRecordingHandler();
        $handler = new Handler($user, Handler::SUB_PROTOCOL_AWAP);
        $handler->afterConnectionEstablished($session);
        $this->assertTrue($user->established);
        $handler->afterConnectionClosed($session, 1000, 'bye');
        $this->assertTrue($user->closed);
        $handler->handleError($session, new \RuntimeException('x'));
        $this->assertEquals(1, count($user->errors));

        $text = AwapUtil::buildAwapMessageText(AwapUtil::newAwapMessage(array('x' => '1'), 'UpstreamTextEvent', 'm1'));
        $raw = new WebSocketMessage();
        $raw->type = \AlibabaCloud\Dara\WebSocketMessageType::TEXT;
        $raw->payload = $text;
        $handler->handleRawMessage($session, $raw);

        $caught = false;
        try {
            $bad = new Handler($user, 'invalid');
            $msg = new WebSocketMessage();
            $msg->payload = '{}';
            $bad->handleRawMessage($session, $msg);
        } catch (\InvalidArgumentException $e) {
            $caught = true;
        }
        $this->assertTrue($caught);

        $inner = new ExtraRecordingWsClient();
        $wsClient = new WebSocketClient($inner, null);
        $handler->client = $wsClient;
        $reconnect = new WebSocketMessage();
        $reconnect->type = \AlibabaCloud\Dara\WebSocketMessageType::TEXT;
        $reconnect->payload = AwapUtil::buildAwapMessageText(AwapUtil::newAwapMessage(array(), 'RECONNECT', 'r1'));
        $handler->handleRawMessage($session, $reconnect);

        $generalUser = new ExtraRecordingHandler();
        $generalHandler = new Handler($generalUser, Handler::SUB_PROTOCOL_GENERAL);
        $gmsg = new WebSocketMessage();
        $gmsg->type = \AlibabaCloud\Dara\WebSocketMessageType::TEXT;
        $gmsg->payload = '{"body":"hi"}';
        $generalHandler->handleRawMessage($session, $gmsg);
        $this->assertEquals(1, $generalUser->general);

        $plain = new ExtraPlainHandler();
        $awapPlain = new Handler($plain, Handler::SUB_PROTOCOL_AWAP);
        $awapPlain->handleRawMessage($session, $raw);
        $generalPlain = new Handler($plain, Handler::SUB_PROTOCOL_GENERAL);
        $generalPlain->handleRawMessage($session, $gmsg);

        $fallback = new ExtraRawFallbackHandler();
        $awapFallback = new Handler($fallback, Handler::SUB_PROTOCOL_AWAP);
        $awapFallback->handleRawMessage($session, $raw);
        $this->assertEquals(1, $fallback->raw);
        $generalFallback = new Handler($fallback, Handler::SUB_PROTOCOL_GENERAL);
        $generalFallback->handleRawMessage($session, $gmsg);
        $this->assertEquals(2, $fallback->raw);

        $fallback->rawErr = new \RuntimeException('raw-fail');
        $awapFallback->handleRawMessage($session, $raw);

        $throwing = new ExtraRecordingHandler();
        $throwing->awapErr = new \RuntimeException('awap-fail');
        $awapThrow = new Handler($throwing, Handler::SUB_PROTOCOL_AWAP);
        $awapThrow->handleRawMessage($session, $raw);
        $this->assertEquals(1, count($throwing->errors));

        Handler::errUseRawMessage();
        if (function_exists('random_bytes')) {
            $this->assertEquals(32, strlen(WebSocketClient::generateMessageId()));
        }
    }

    public function testDoRequestWebsocketValidation()
    {
        $client = new OpenApiClient(new Config(array(
            'accessKeyId' => 'ak',
            'accessKeySecret' => 'sk',
            'endpoint' => 'example.com',
            'protocol' => 'ws',
        )));
        $params = new Params(array(
            'action' => 'TestAction',
            'version' => '2020-01-01',
            'protocol' => 'ws',
            'pathname' => '/ws',
            'method' => 'GET',
            'authType' => 'AK',
            'bodyType' => 'string',
            'reqBodyType' => 'json',
        ));
        $runtime = new RuntimeOptions();
        $caught = false;
        try {
            $client->doRequest($params, new OpenApiRequest(), $runtime);
        } catch (\Exception $e) {
            $caught = true;
        }
        $this->assertTrue($caught);

        $params->websocketSubProtocol = 'invalid';
        $runtime->webSocketHandler = new ExtraRecordingHandler();
        $caught = false;
        try {
            $client->doRequest($params, new OpenApiRequest(), $runtime);
        } catch (\Exception $e) {
            $caught = true;
        }
        $this->assertTrue($caught);

        $params->websocketSubProtocol = Handler::SUB_PROTOCOL_AWAP;
        $runtime->webSocketHandler = null;
        $caught = false;
        try {
            $client->doRequest($params, new OpenApiRequest(), $runtime);
        } catch (\Exception $e) {
            $caught = true;
        }
        $this->assertTrue($caught);
    }
}
