import json
import os
from typing import Any, Callable, Dict, List, Optional

from darabonba.websocket import (
    AbstractWebSocketHandler,
    WebSocketMessage,
    WebSocketMessageType,
    WebSocketSessionInfo,
)

from ._constants import ERR_USE_RAW_MESSAGE


AwapMessage = Dict[str, Any]
AwapMessageType = str
AwapMessageOption = Callable[[AwapMessage], None]


class AwapWebSocketHandler(AbstractWebSocketHandler):
    def handle_awap_message(self, session: WebSocketSessionInfo, message: AwapMessage) -> None:
        raise ERR_USE_RAW_MESSAGE


def with_type(msg_type: str) -> AwapMessageOption:
    def apply(message: AwapMessage) -> None:
        message['type'] = msg_type

    return apply


def with_id(message_id: str) -> AwapMessageOption:
    def apply(message: AwapMessage) -> None:
        message['id'] = message_id

    return apply


def with_header(key: str, value: str) -> AwapMessageOption:
    def apply(message: AwapMessage) -> None:
        message.setdefault('headers', {})[key] = value

    return apply


def new_awap_message(payload: Any, *opts: AwapMessageOption) -> AwapMessage:
    message: AwapMessage = {
        'type': 'UpstreamTextEvent',
        'id': _generate_message_id(),
        'payload': payload,
        'headers': {},
    }
    for opt in opts:
        opt(message)
    return message


def build_awap_message_text(message: AwapMessage) -> str:
    if message is None:
        raise ValueError('message cannot be nil')

    header_lines = [
        f"type:{message['type']}",
        f"timestamp:{int(__import__('time').time() * 1000)}",
    ]
    if message.get('id'):
        header_lines.append(f"id:{message['id']}")
    if message.get('type') == 'AckRequiredTextEvent':
        header_lines.append('ack:required')
    for key, value in (message.get('headers') or {}).items():
        header_lines.append(f'{key}:{value}')

    payload = message.get('payload')
    payload_json = '{}' if payload is None else json.dumps(payload, separators=(',', ':'))
    return '\n'.join(header_lines) + '\n\n' + payload_json


def build_awap_message_binary(message: AwapMessage) -> bytes:
    if message is None:
        raise ValueError('message cannot be nil')

    header_lines = [
        f"type:{message['type']}",
        f"timestamp:{int(__import__('time').time() * 1000)}",
    ]
    if message.get('id'):
        header_lines.append(f"id:{message['id']}")
    for key, value in (message.get('headers') or {}).items():
        header_lines.append(f'{key}:{value}')

    header_bytes = ('\n'.join(header_lines) + '\n\n').encode('utf-8')
    payload = message.get('payload')
    if payload is None:
        body_bytes = b''
    elif isinstance(payload, bytes):
        body_bytes = payload
    else:
        raise ValueError(
            f'payload for binary AWAP message must be bytes, got {type(payload).__name__}'
        )
    return header_bytes + body_bytes


def parse_awap_message(message: WebSocketMessage) -> AwapMessage:
    data = message.payload
    header_end_index = -1
    for i in range(len(data) - 1):
        if data[i] == 0x0A and data[i + 1] == 0x0A:
            header_end_index = i
            break
    if header_end_index == -1:
        raise ValueError('failed to parse AWAP message: no \\n\\n separator found')

    awap_msg: AwapMessage = {
        'type': '',
        'id': '',
        'headers': {},
    }
    header_str = data[:header_end_index].decode('utf-8')
    payload_bytes = data[header_end_index + 2:]

    for line in header_str.split('\n'):
        trimmed = line.strip()
        if not trimmed:
            continue
        colon_index = trimmed.find(':')
        if colon_index > 0:
            key = trimmed[:colon_index].strip()
            value = trimmed[colon_index + 1:].strip()
            if key == 'type':
                awap_msg['type'] = value
            elif key == 'id':
                awap_msg['id'] = value
            else:
                awap_msg['headers'][key] = value

    if payload_bytes:
        try:
            awap_msg['payload'] = json.loads(payload_bytes.decode('utf-8'))
        except (json.JSONDecodeError, UnicodeDecodeError):
            awap_msg['payload'] = payload_bytes

    awap_msg['format'] = 'binary' if message.type == WebSocketMessageType.Binary else 'text'
    return awap_msg


def _generate_message_id() -> str:
    hex_str = os.urandom(16).hex()
    if len(hex_str) >= 32:
        return hex_str[:32]
    timestamp = str(int(__import__('time').time() * 1000))
    combined = timestamp + hex_str
    if len(combined) >= 32:
        return combined[:32]
    return combined.ljust(32, '0')
