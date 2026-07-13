import threading
from typing import Any, Dict, List, Optional

from darabonba.response import DaraResponse
from darabonba.websocket import (
    AbstractWebSocketHandler,
    DefaultWebSocketClient,
    WebSocketSessionInfo,
)

from .awap import (
    AwapMessage,
    AwapMessageType,
    build_awap_message_binary,
    build_awap_message_text,
    new_awap_message,
    with_id,
    with_type,
)
from .general import GeneralMessage, general_message_to_json, new_general_message


class WebSocketClient:
    def __init__(self, ws_client: DefaultWebSocketClient, response: DaraResponse):
        self.ws_client = ws_client
        self.response = response
        self._pending_requests: Dict[str, List[Any]] = {}
        self._lock = threading.Lock()

    @staticmethod
    def create_websocket_client(client: Any) -> Optional['WebSocketClient']:
        if client is None:
            return None
        if isinstance(client, WebSocketClient):
            return client
        return None

    def get_response(self) -> DaraResponse:
        return self.response

    def close(self) -> None:
        if self.ws_client:
            self.ws_client.close()

    def reconnect(self) -> DaraResponse:
        return self.ws_client.reconnect()

    def reconnect_gracefully(self) -> DaraResponse:
        return self.ws_client.reconnect_gracefully()

    def disconnect(self) -> None:
        self.ws_client.disconnect()

    def is_connected(self) -> bool:
        return self.ws_client.is_connected()

    def validate(self) -> None:
        if not self.ws_client:
            raise Exception('failed to build websocket client')

    def get_session_info(self) -> Optional[WebSocketSessionInfo]:
        return self.ws_client.get_session_info()

    def send_awap_text_message(self, message: AwapMessage) -> None:
        message_text = build_awap_message_text(message)
        self.ws_client.send_text(message_text)

    def send_raw_awap_text_message(self, msg_type: AwapMessageType, payload: Any) -> None:
        message = new_awap_message(payload, with_type(msg_type))
        self.send_awap_text_message(message)

    def send_raw_awap_text_message_with_id(
        self,
        msg_type: AwapMessageType,
        message_id: str,
        payload: Any,
    ) -> None:
        message = new_awap_message(payload, with_type(msg_type), with_id(message_id))
        self.send_awap_text_message(message)

    def send_awap_binary_message(self, message: AwapMessage) -> None:
        message_binary = build_awap_message_binary(message)
        self.ws_client.send_binary(message_binary)

    def send_raw_awap_binary_message(self, msg_type: AwapMessageType, payload: Any) -> None:
        message = new_awap_message(payload, with_type(msg_type))
        self.send_awap_binary_message(message)

    def send_raw_awap_binary_message_with_id(
        self,
        msg_type: AwapMessageType,
        message_id: str,
        payload: Any,
    ) -> None:
        message = new_awap_message(payload, with_type(msg_type), with_id(message_id))
        self.send_awap_binary_message(message)

    def send_awap_request_with_ack(self, message: AwapMessage, timeout_ms: int) -> Any:
        message_text = build_awap_message_text(message)
        return self.send_request(message['id'], message_text, timeout_ms)

    def send_request(self, ack_id: str, message_text: str, timeout_ms: int) -> Any:
        if not ack_id:
            raise ValueError('message ID cannot be empty for request-response pattern')
        if not message_text:
            raise ValueError('message text cannot be empty for request-response pattern')

        result_event = threading.Event()
        result_holder: List[Any] = [None]
        error_holder: List[Optional[Exception]] = [None]

        def resolve(value):
            result_holder[0] = value
            result_event.set()

        with self._lock:
            self._pending_requests.setdefault(ack_id, []).append(resolve)

        try:
            self.ws_client.send_text(message_text)
        except Exception as err:
            with self._lock:
                self._pending_requests.pop(ack_id, None)
            raise err

        timeout = timeout_ms if timeout_ms > 0 else 30000
        if not result_event.wait(timeout=timeout / 1000):
            with self._lock:
                self._pending_requests.pop(ack_id, None)
            raise TimeoutError(
                f'request timeout after {timeout}ms waiting for response to message ID: {ack_id}'
            )
        if error_holder[0] is not None:
            raise error_holder[0]
        return result_holder[0]

    def complete_request(self, ack_id: str, response: Any) -> bool:
        with self._lock:
            waiters = self._pending_requests.pop(ack_id, None)
        if not waiters:
            return False
        for resolve in waiters:
            resolve(response)
        return True

    def send_general_text_message(self, text: str) -> None:
        message = new_general_message(text)
        json_data = general_message_to_json(message)
        self.ws_client.send_text(json_data.decode('utf-8'))

    def send_general_binary_message(self, data: bytes) -> None:
        self.ws_client.send_binary(data)


class Client(WebSocketClient):
    @staticmethod
    def create_web_socket_client(client: Any) -> Optional['Client']:
        return WebSocketClient.create_websocket_client(client)


def new_websocket_client(
    ws_client: DefaultWebSocketClient,
    response: DaraResponse,
) -> WebSocketClient:
    return WebSocketClient(ws_client, response)
