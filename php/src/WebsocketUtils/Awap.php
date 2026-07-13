<?php

namespace Darabonba\OpenApi\WebsocketUtils;

class Awap
{
    const MESSAGE_FORMAT_TEXT = 'text';
    const MESSAGE_FORMAT_BINARY = 'binary';

    /** @var string */
    public $type;

    /** @var string */
    public $id;

    /** @var array */
    public $headers;

    /** @var mixed */
    public $payload;

    /** @var string */
    public $format;

    public function withHeader($key, $value)
    {
        if ($this->headers === null) {
            $this->headers = [];
        }
        $this->headers[$key] = $value;

        return $this;
    }

    public function withFormat($format)
    {
        $this->format = $format;

        return $this;
    }

    public function toJSON()
    {
        return json_encode($this);
    }
}

interface AwapWebSocketHandler extends \AlibabaCloud\Dara\WebSocketHandler
{
    /**
     * @param \AlibabaCloud\Dara\WebSocketSessionInfo $session
     * @param Awap                                      $message
     *
     * @return void
     */
    public function handleAwapMessage($session, $message);
}

class AbstractAwapWebSocketHandler extends \AlibabaCloud\Dara\AbstractWebSocketHandler implements AwapWebSocketHandler
{
    public function handleAwapMessage($session, $message)
    {
        throw Handler::errUseRawMessage();
    }
}

class AwapUtil
{
    /**
     * @param mixed              $payload
     * @param AwapMessageType|null $type
     * @param string|null        $id
     * @param array              $headers
     *
     * @return Awap
     */
    public static function newAwapMessage($payload, $type = null, $id = null, $headers = [])
    {
        $message = new Awap();
        $message->type = $type !== null ? $type : 'UpstreamTextEvent';
        $message->id = $id !== null ? $id : Client::generateMessageId();
        $message->payload = $payload;
        $message->headers = $headers;

        return $message;
    }

    /**
     * @param Awap $message
     *
     * @return string
     */
    public static function buildAwapMessageText($message)
    {
        if ($message === null) {
            throw new \InvalidArgumentException('message cannot be nil');
        }

        $now = microtime(true);
        $timestamp = (int) ($now * 1000);
        $headerBuilder = 'type:' . $message->type . "\n";
        $headerBuilder .= 'timestamp:' . $timestamp . "\n";
        if (!empty($message->id)) {
            $headerBuilder .= 'id:' . $message->id . "\n";
        }
        if ($message->type === 'AckRequiredTextEvent') {
            $headerBuilder .= "ack:required\n";
        }
        if (!empty($message->headers)) {
            foreach ($message->headers as $key => $value) {
                $headerBuilder .= $key . ':' . $value . "\n";
            }
        }
        $headerBuilder .= "\n";

        if ($message->payload !== null) {
            $payloadJSON = json_encode($message->payload);
            if ($payloadJSON === false) {
                throw new \RuntimeException('failed to marshal AWAP payload');
            }
        } else {
            $payloadJSON = '{}';
        }

        return $headerBuilder . $payloadJSON;
    }

    /**
     * @param Awap $message
     *
     * @return string
     */
    public static function buildAwapMessageBinary($message)
    {
        if ($message === null) {
            throw new \InvalidArgumentException('message cannot be nil');
        }

        $textPart = self::buildAwapMessageText($message);
        $headerEnd = strpos($textPart, "\n\n");
        if ($headerEnd === false) {
            throw new \RuntimeException('failed to build AWAP binary header');
        }
        $headerBytes = substr($textPart, 0, $headerEnd + 2);
        if (!\is_string($message->payload)) {
            throw new \InvalidArgumentException('payload for binary AWAP message must be string bytes');
        }

        return $headerBytes . $message->payload;
    }

    /**
     * @param \AlibabaCloud\Dara\WebSocketMessage $message
     *
     * @return Awap
     */
    public static function parseAwapMessage($message)
    {
        $data = $message->payload;
        $headerEndIndex = strpos($data, "\n\n");
        if ($headerEndIndex === false) {
            throw new \RuntimeException('failed to parse AWAP message: no separator found');
        }

        $awapMsg = new Awap();
        $awapMsg->headers = [];
        $headerStr = substr($data, 0, $headerEndIndex);
        $payloadBytes = substr($data, $headerEndIndex + 2);
        foreach (explode("\n", $headerStr) as $line) {
            $line = trim($line);
            if ($line === '') {
                continue;
            }
            $colonIndex = strpos($line, ':');
            if ($colonIndex > 0) {
                $key = trim(substr($line, 0, $colonIndex));
                $value = trim(substr($line, $colonIndex + 1));
                if ($key === 'type') {
                    $awapMsg->type = $value;
                } elseif ($key === 'id') {
                    $awapMsg->id = $value;
                } else {
                    $awapMsg->headers[$key] = $value;
                }
            }
        }

        if (strlen($payloadBytes) > 0) {
            $payload = json_decode($payloadBytes, true);
            $awapMsg->payload = $payload === null ? $payloadBytes : $payload;
        }

        $awapMsg->format = $message->type === \AlibabaCloud\Dara\WebSocketMessageType::BINARY
            ? Awap::MESSAGE_FORMAT_BINARY
            : Awap::MESSAGE_FORMAT_TEXT;

        return $awapMsg;
    }
}
