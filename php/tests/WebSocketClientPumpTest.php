<?php

namespace Darabonba\OpenApi\Tests;

use Darabonba\OpenApi\WebsocketUtils\Client;
use AlibabaCloud\Dara\WebSocketClientInterface;
use PHPUnit\Framework\TestCase;

// 记录 pump 调用的桩，实现完整接口以便注入包装类
class PumpRecordingWsClient implements WebSocketClientInterface
{
    public $pumpCalls = [];

    public function connect($request, $runtimeObject) {}
    public function disconnect() {}
    public function reconnect() {}
    public function reconnectGracefully() {}
    public function isConnected() { return true; }
    public function sendText($text) {}
    public function sendBinary($data) {}
    public function getSessionInfo() { return null; }
    public function close() {}

    public function pump($timeoutMs = 100)
    {
        $this->pumpCalls[] = $timeoutMs;
    }
}

class WebSocketClientPumpTest extends TestCase
{
    // 红线1: 包装类必须暴露公开 pump() 方法
    public function testWrapperExposesPublicPump()
    {
        $this->assertTrue(
            method_exists(Client::class, 'pump'),
            'WebSocketClient wrapper must expose a public pump() method'
        );
    }

    // 红线2: pump() 必须以相同 timeout 转发到底层，事件循环才会被真正驱动
    public function testPumpForwardsToInnerClient()
    {
        $inner = new PumpRecordingWsClient();
        $wrapper = new Client($inner, null);

        $wrapper->pump(200);

        $this->assertSame([200], $inner->pumpCalls,
            'pump(200) must be forwarded to the inner client once with 200');
    }
}
