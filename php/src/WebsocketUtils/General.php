<?php

namespace Darabonba\OpenApi\WebsocketUtils;

class General
{
    const MESSAGE_FORMAT_TEXT = 'text';
    const MESSAGE_FORMAT_BINARY = 'binary';

    /** @var mixed */
    public $body;

    /** @var string */
    public $format;

    public function toJSON()
    {
        return json_encode($this);
    }
}

interface GeneralWebSocketHandler extends \AlibabaCloud\Dara\WebSocketHandler
{
    public function handleGeneralMessage($session, $message);
}

class AbstractGeneralWebSocketHandler extends \AlibabaCloud\Dara\AbstractWebSocketHandler implements GeneralWebSocketHandler
{
    public function handleGeneralMessage($session, $message)
    {
        throw Handler::errUseRawMessage();
    }
}

class GeneralUtil
{
    public static function newGeneralMessage($body)
    {
        $message = new General();
        $message->body = $body;

        return $message;
    }

    public static function parseGeneralMessage($message)
    {
        if ($message->type === \AlibabaCloud\Dara\WebSocketMessageType::BINARY) {
            $general = self::newGeneralMessage($message->payload);
            $general->format = General::MESSAGE_FORMAT_BINARY;

            return $general;
        }

        $body = json_decode($message->payload, true);
        if ($body === null && json_last_error() !== JSON_ERROR_NONE) {
            $general = self::newGeneralMessage($message->payload);
            $general->format = General::MESSAGE_FORMAT_TEXT;

            return $general;
        }
        if ($body === null) {
            $body = $message->payload;
        }
        $general = new General();
        $general->body = $body;
        $general->format = General::MESSAGE_FORMAT_TEXT;

        return $general;
    }
}
