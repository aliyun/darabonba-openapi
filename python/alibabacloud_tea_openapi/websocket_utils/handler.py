from darabonba.websocket import AbstractWebSocketHandler, WebSocketMessage, WebSocketSessionInfo

from ._constants import SubProtocolAWAP, SubProtocolGeneral
from .awap import (
    AwapWebSocketHandler,
    ERR_USE_RAW_MESSAGE as AWAP_ERR_USE_RAW_MESSAGE,
    parse_awap_message,
)
from .client import WebSocketClient
from .general import (
    ERR_USE_RAW_MESSAGE as GENERAL_ERR_USE_RAW_MESSAGE,
    GeneralWebSocketHandler,
    parse_general_message,
)


class StreamHandler(AbstractWebSocketHandler):
    def __init__(self, user_handler, sub_protocol: str):
        super().__init__()
        self.user_handler = user_handler
        self.sub_protocol = sub_protocol
        self.client: WebSocketClient = None

    def after_connection_established(self, session: WebSocketSessionInfo) -> None:
        self.user_handler.after_connection_established(session)

    def handle_error(self, session: WebSocketSessionInfo, err: Exception) -> None:
        self.user_handler.handle_error(session, err)

    def after_connection_closed(self, session: WebSocketSessionInfo, code: int, reason: str) -> None:
        self.user_handler.after_connection_closed(session, code, reason)

    def handle_raw_message(self, session: WebSocketSessionInfo, message: WebSocketMessage) -> None:
        sub_protocol = (self.sub_protocol or '').lower()
        if sub_protocol == SubProtocolAWAP:
            self._process_awap_message(session, message)
        elif sub_protocol == SubProtocolGeneral:
            self._process_general_message(session, message)
        else:
            self.user_handler.handle_raw_message(session, message)

    def _process_awap_message(self, session: WebSocketSessionInfo, message: WebSocketMessage) -> None:
        awap_msg = parse_awap_message(message)

        if awap_msg.get('type') == 'RECONNECT':
            if self.client is not None:
                try:
                    self.client.reconnect_gracefully()
                except Exception:
                    pass
            return

        ack_id = (awap_msg.get('headers') or {}).get('ack-id', '')
        if ack_id and self.client and self.client.complete_request(ack_id, awap_msg):
            return

        if hasattr(self.user_handler, 'handle_awap_message'):
            try:
                self.user_handler.handle_awap_message(session, awap_msg)
            except Exception as err:
                if err is AWAP_ERR_USE_RAW_MESSAGE:
                    self.user_handler.handle_raw_message(session, message)
                else:
                    self.user_handler.handle_error(session, err)
        else:
            self.user_handler.handle_raw_message(session, message)

    def _process_general_message(self, session: WebSocketSessionInfo, message: WebSocketMessage) -> None:
        if not hasattr(self.user_handler, 'handle_general_message'):
            self.user_handler.handle_raw_message(session, message)
            return

        general_msg = parse_general_message(message)
        try:
            self.user_handler.handle_general_message(session, general_msg)
        except Exception as err:
            if err is GENERAL_ERR_USE_RAW_MESSAGE:
                self.user_handler.handle_raw_message(session, message)
            else:
                self.user_handler.handle_error(session, err)
