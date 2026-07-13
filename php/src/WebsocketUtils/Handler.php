<?php

namespace Darabonba\OpenApi\WebsocketUtils;

use AlibabaCloud\Dara\WebSocketMessage;
use AlibabaCloud\Dara\WebSocketSessionInfo;

class Handler implements \AlibabaCloud\Dara\WebSocketHandler
{
    const SUB_PROTOCOL_AWAP = 'awap';
    const SUB_PROTOCOL_GENERAL = 'general';

    /** @var \AlibabaCloud\Dara\WebSocketHandler */
    public $userHandler;

    /** @var string */
    public $subProtocol;

    /** @var Client|null */
    public $client;

    public function __construct($userHandler, $subProtocol)
    {
        $this->userHandler = $userHandler;
        $this->subProtocol = $subProtocol;
    }

    public static function errUseRawMessage()
    {
        return new \RuntimeException('use HandleRawMessage');
    }

    public function afterConnectionEstablished($session)
    {
        $this->userHandler->afterConnectionEstablished($session);
    }

    public function handleError($session, $err)
    {
        $this->userHandler->handleError($session, $err);
    }

    public function afterConnectionClosed($session, $code, $reason)
    {
        $this->userHandler->afterConnectionClosed($session, $code, $reason);
    }

    public function handleRawMessage($session, $message)
    {
        $subProtocol = strtolower($this->subProtocol);
        if ($subProtocol === self::SUB_PROTOCOL_AWAP) {
            return $this->processAwapMessage($session, $message);
        }
        if ($subProtocol === self::SUB_PROTOCOL_GENERAL) {
            return $this->processGeneralMessage($session, $message);
        }

        throw new \InvalidArgumentException(
            "unsupported websocketSubProtocol: '{$this->subProtocol}', must be 'awap' or 'general'"
        );
    }

    private function processAwapMessage($session, $message)
    {
        try {
            $awapMsg = AwapUtil::parseAwapMessage($message);
        } catch (\Exception $e) {
            throw new \RuntimeException('failed to parse AWAP message: ' . $e->getMessage(), 0, $e);
        }

        if ($awapMsg->type === 'RECONNECT') {
            if ($this->client !== null) {
                try {
                    $this->client->reconnectGracefully();
                } catch (\Exception $e) {
                    $this->userHandler->handleError($session, $e);
                }
            }

            return;
        }

        $ackID = isset($awapMsg->headers['ack-id']) ? $awapMsg->headers['ack-id'] : '';
        if ($ackID !== '' && $this->client !== null && $this->client->completeRequest($ackID, $awapMsg)) {
            return;
        }

        if (!($this->userHandler instanceof AwapWebSocketHandler)) {
            $this->userHandler->handleError(
                $session,
                new \InvalidArgumentException(
                    'WebSocketHandler must implement handleAwapMessage for awap sub-protocol'
                )
            );

            return;
        }

        try {
            $this->userHandler->handleAwapMessage($session, $awapMsg);
        } catch (\RuntimeException $e) {
            if ($e->getMessage() === 'use HandleRawMessage') {
                try {
                    $this->userHandler->handleRawMessage($session, $message);
                } catch (\Exception $rawErr) {
                    $this->userHandler->handleError($session, $rawErr);
                }
            } else {
                $this->userHandler->handleError($session, $e);
            }
        }
    }

    private function processGeneralMessage($session, $message)
    {
        if (!($this->userHandler instanceof GeneralWebSocketHandler)) {
            $this->userHandler->handleError(
                $session,
                new \InvalidArgumentException(
                    'WebSocketHandler must implement handleGeneralMessage for general sub-protocol'
                )
            );

            return;
        }

        try {
            $generalMsg = GeneralUtil::parseGeneralMessage($message);
            $this->userHandler->handleGeneralMessage($session, $generalMsg);
        } catch (\RuntimeException $e) {
            if ($e->getMessage() === 'use HandleRawMessage') {
                try {
                    $this->userHandler->handleRawMessage($session, $message);
                } catch (\Exception $rawErr) {
                    $this->userHandler->handleError($session, $rawErr);
                }
            } else {
                $this->userHandler->handleError($session, $e);
            }
        } catch (\Exception $e) {
            throw new \RuntimeException('failed to parse General message: ' . $e->getMessage(), 0, $e);
        }
    }
}
