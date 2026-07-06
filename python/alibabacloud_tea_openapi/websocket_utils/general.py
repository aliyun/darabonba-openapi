import json
from typing import Any, Dict

from darabonba.websocket import (
    AbstractWebSocketHandler,
    WebSocketMessage,
    WebSocketMessageType,
    WebSocketSessionInfo,
)

from ._constants import ERR_USE_RAW_MESSAGE


GeneralMessage = Dict[str, Any]


class GeneralWebSocketHandler(AbstractWebSocketHandler):
    def handle_general_message(self, session: WebSocketSessionInfo, message: GeneralMessage) -> None:
        raise ERR_USE_RAW_MESSAGE


def new_general_message(body: str) -> GeneralMessage:
    return {'body': body}


def parse_general_message(message: WebSocketMessage) -> GeneralMessage:
    if message.type == WebSocketMessageType.Binary:
        return {
            'body': message.payload,
            'format': 'binary',
        }
    try:
        return {
            'body': json.loads(message.payload.decode('utf-8')),
            'format': 'text',
        }
    except (json.JSONDecodeError, UnicodeDecodeError):
        return {
            'body': message.payload,
            'format': 'text',
        }


def general_message_to_json(message: GeneralMessage) -> bytes:
    return json.dumps(message, separators=(',', ':')).encode('utf-8')
