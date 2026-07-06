<?php

namespace Darabonba\OpenApi\WebsocketUtils;

use AlibabaCloud\Dara\Response;
use AlibabaCloud\Dara\WebSocketClientInterface;
use AlibabaCloud\Dara\WebSocketSessionInfo;

class Client
{
    /** @var WebSocketClientInterface */
    private $wsClient;

    /** @var Response */
    private $response;

    /** @var array */
    private $pendingRequests = [];

    public function __construct($wsClient, $response)
    {
        $this->wsClient = $wsClient;
        $this->response = $response;
    }

    public static function createWebSocketClient($client)
    {
        if ($client === null) {
            return null;
        }
        if ($client instanceof self) {
            return $client;
        }

        return null;
    }

    public function getResponse()
    {
        return $this->response;
    }

    public function close()
    {
        if ($this->wsClient !== null) {
            return $this->wsClient->close();
        }
    }

    public function reconnect()
    {
        if ($this->wsClient !== null) {
            return $this->wsClient->reconnect();
        }
        throw new \RuntimeException('wsClient is nil');
    }

    public function reconnectGracefully()
    {
        if ($this->wsClient !== null) {
            return $this->wsClient->reconnectGracefully();
        }
        throw new \RuntimeException('wsClient is nil');
    }

    public function reconnectGracefullyAsync()
    {
        if ($this->wsClient !== null && method_exists($this->wsClient, 'reconnectGracefully')) {
            try {
                $this->wsClient->reconnectGracefully();
            } catch (\Exception $e) {
                // ignore async reconnect errors
            }
        }
    }

    public function disconnect()
    {
        if ($this->wsClient !== null) {
            return $this->wsClient->disconnect();
        }
    }

    public function isConnected()
    {
        if ($this->wsClient !== null) {
            return $this->wsClient->isConnected();
        }

        return false;
    }

    public function validate()
    {
        if ($this->wsClient !== null) {
            return null;
        }
        throw new \RuntimeException('failed to build websocket client');
    }

    /**
     * @return WebSocketSessionInfo|null
     */
    public function getSessionInfo()
    {
        if ($this->wsClient !== null) {
            return $this->wsClient->getSessionInfo();
        }

        return null;
    }

    public function sendAwapTextMessage($message)
    {
        $messageText = AwapUtil::buildAwapMessageText($message);
        $this->wsClient->sendText($messageText);
    }

    public function sendRawAwapTextMessage($msgType, $payload)
    {
        $message = AwapUtil::newAwapMessage($payload, $msgType);
        $this->sendAwapTextMessage($message);
    }

    public function sendRawAwapTextMessageWithId($msgType, $id, $payload)
    {
        $message = AwapUtil::newAwapMessage($payload, $msgType, $id);
        $this->sendAwapTextMessage($message);
    }

    public function sendAwapBinaryMessage($message)
    {
        $messageBinary = AwapUtil::buildAwapMessageBinary($message);
        $this->wsClient->sendBinary($messageBinary);
    }

    public function sendRawAwapBinaryMessage($msgType, $payload)
    {
        $message = AwapUtil::newAwapMessage($payload, $msgType);
        $this->sendAwapBinaryMessage($message);
    }

    public function sendRawAwapBinaryMessageWithId($msgType, $id, $payload)
    {
        $message = AwapUtil::newAwapMessage($payload, $msgType, $id);
        $this->sendAwapBinaryMessage($message);
    }

    public function sendAwapRequestWithAck($message, $timeoutMs = 30000)
    {
        $messageText = AwapUtil::buildAwapMessageText($message);

        return $this->sendRequest($message->id, $messageText, $timeoutMs);
    }

    public function sendRequest($ackID, $messageText, $timeoutMs = 30000)
    {
        if ($ackID === '') {
            throw new \InvalidArgumentException('message ID cannot be empty for request-response pattern');
        }
        if ($messageText === '') {
            throw new \InvalidArgumentException('message text cannot be empty for request-response pattern');
        }

        $this->pendingRequests[$ackID] = null;
        $this->wsClient->sendText($messageText);

        $timeoutMs = $timeoutMs > 0 ? $timeoutMs : 30000;
        $deadline = microtime(true) + ($timeoutMs / 1000);
        while (microtime(true) < $deadline) {
            if (\array_key_exists($ackID, $this->pendingRequests) && $this->pendingRequests[$ackID] !== null) {
                $response = $this->pendingRequests[$ackID];
                unset($this->pendingRequests[$ackID]);

                return $response;
            }
            usleep(10000);
        }
        unset($this->pendingRequests[$ackID]);
        throw new \RuntimeException('request timeout after ' . $timeoutMs . 'ms waiting for response to message ID: ' . $ackID);
    }

    public function completeRequest($ackID, $response)
    {
        if (!isset($this->pendingRequests[$ackID])) {
            return false;
        }
        $this->pendingRequests[$ackID] = $response;

        return true;
    }

    public function sendGeneralTextMessage($text)
    {
        $message = GeneralUtil::newGeneralMessage($text);
        $this->wsClient->sendText($message->toJSON());
    }

    public function sendGeneralBinaryMessage($data)
    {
        $this->wsClient->sendBinary($data);
    }

    public static function generateMessageId()
    {
        $randomBytes = random_bytes(16);
        $hexStr = bin2hex($randomBytes);
        if (strlen($hexStr) >= 32) {
            return substr($hexStr, 0, 32);
        }

        $timestamp = (string) (int) (microtime(true) * 1000);
        $combined = $timestamp . $hexStr;
        if (strlen($combined) >= 32) {
            return substr($combined, 0, 32);
        }

        return str_pad($combined, 32, '0');
    }
}
