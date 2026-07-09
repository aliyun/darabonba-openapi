import time
import unittest
import unittest.mock

from darabonba.runtime import RuntimeOptions
from darabonba.websocket import AbstractWebSocketHandler, WebSocketMessage, WebSocketMessageType

from alibabacloud_credentials import models as credential_models
from alibabacloud_credentials.client import Client as CredentialClient
from alibabacloud_tea_openapi import models as open_api_models
from alibabacloud_tea_openapi.client import Client as OpenApiClient
from alibabacloud_tea_openapi.utils_models import OpenApiRequest, Params
from alibabacloud_tea_openapi import websocket_utils


class MockWebSocketHandler(AbstractWebSocketHandler):
    def __init__(self):
        self.connected_called = False

    def after_connection_established(self, session):
        self.connected_called = True


class TestWebSocketUtils(unittest.TestCase):
    def test_build_and_parse_awap_text_messages(self):
        message = websocket_utils.new_awap_message(
            {'hello': 'world'},
            websocket_utils.with_type('UpstreamTextEvent'),
            websocket_utils.with_id('msg-001'),
            websocket_utils.with_header('custom', 'value'),
        )
        text = websocket_utils.build_awap_message_text(message)
        self.assertIn('type:UpstreamTextEvent', text)
        self.assertIn('id:msg-001', text)
        self.assertIn('custom:value', text)
        self.assertTrue(text.endswith('{"hello":"world"}'))

        raw_message = WebSocketMessage(
            type=WebSocketMessageType.Text,
            payload=text.encode('utf-8'),
            headers={},
        )
        parsed = websocket_utils.parse_awap_message(raw_message)
        self.assertEqual('UpstreamTextEvent', parsed['type'])
        self.assertEqual('msg-001', parsed['id'])
        self.assertEqual({'hello': 'world'}, parsed['payload'])
        self.assertEqual('value', parsed['headers']['custom'])

    def test_ack_required_header(self):
        message = websocket_utils.new_awap_message(
            {},
            websocket_utils.with_type('AckRequiredTextEvent'),
        )
        text = websocket_utils.build_awap_message_text(message)
        self.assertIn('ack:required', text)

    def test_parse_general_messages(self):
        json_message = WebSocketMessage(
            type=WebSocketMessageType.Text,
            payload=b'{"body":"hello"}',
            headers={},
        )
        parsed_json = websocket_utils.parse_general_message(json_message)
        self.assertEqual({'body': 'hello'}, parsed_json['body'])
        self.assertEqual('text', parsed_json['format'])

        binary_message = WebSocketMessage(
            type=WebSocketMessageType.Binary,
            payload=b'binary-data',
            headers={},
        )
        parsed_binary = websocket_utils.parse_general_message(binary_message)
        self.assertEqual(b'binary-data', parsed_binary['body'])
        self.assertEqual('binary', parsed_binary['format'])

    def test_complete_awap_request_response(self):
        class MockWsClient:
            def send_text(self, _text):
                return None

            def is_connected(self):
                return True

            def close(self):
                return None

            def reconnect(self):
                raise NotImplementedError('not implemented')

            def reconnect_gracefully(self):
                raise NotImplementedError('not implemented')

            def disconnect(self):
                return None

            def get_session_info(self):
                return None

        from darabonba.response import DaraResponse

        client = websocket_utils.WebSocketClient(
            MockWsClient(),
            DaraResponse(),
        )
        response_promise = []
        import threading

        def run_request():
            response_promise.append(
                client.send_request('ack-001', 'type:test\nid:ack-001\n\n{}', 1000)
            )

        thread = threading.Thread(target=run_request)
        thread.start()
        time.sleep(0.05)
        self.assertTrue(client.complete_request('ack-001', {'ok': True}))
        thread.join(timeout=2)
        self.assertEqual({'ok': True}, response_promise[0])

    def test_route_awap_reconnect_messages(self):
        graceful_called = []

        class MockWsClient:
            def reconnect_gracefully(self):
                graceful_called.append(True)
                from darabonba.response import DaraResponse

                return DaraResponse()

        from darabonba.response import DaraResponse

        ws_client = websocket_utils.WebSocketClient(MockWsClient(), DaraResponse())
        handler = websocket_utils.StreamHandler(
            MockWebSocketHandler(),
            websocket_utils.SubProtocolAWAP,
        )
        handler.client = ws_client

        awap_text = websocket_utils.build_awap_message_text(
            websocket_utils.new_awap_message({}, websocket_utils.with_type('RECONNECT')),
        )
        handler.handle_raw_message(
            __import__('darabonba.websocket', fromlist=['WebSocketSessionInfo']).WebSocketSessionInfo('s1'),
            WebSocketMessage(
                type=WebSocketMessageType.Text,
                payload=awap_text.encode('utf-8'),
                headers={},
            ),
        )
        time.sleep(0.05)
        self.assertEqual([True], graceful_called)

    def test_reject_unknown_sub_protocol(self):
        handler = websocket_utils.StreamHandler(
            MockWebSocketHandler(),
            'invalid',
        )
        session = __import__('darabonba.websocket', fromlist=['WebSocketSessionInfo']).WebSocketSessionInfo('s1')
        message = WebSocketMessage(
            type=WebSocketMessageType.Text,
            payload=b'{}',
            headers={},
        )
        with self.assertRaisesRegex(ValueError, "unsupported websocketSubProtocol"):
            handler.handle_raw_message(session, message)

    def test_reconnect_failure_calls_handle_error(self):
        errors = []

        class ErrorHandler(MockWebSocketHandler):
            def handle_error(self, session, err):
                errors.append(err)

        class FailingWsClient:
            def reconnect_gracefully(self):
                raise RuntimeError('reconnect failed')

        from darabonba.response import DaraResponse

        ws_client = websocket_utils.WebSocketClient(FailingWsClient(), DaraResponse())
        handler = websocket_utils.StreamHandler(ErrorHandler(), websocket_utils.SubProtocolAWAP)
        handler.client = ws_client

        awap_text = websocket_utils.build_awap_message_text(
            websocket_utils.new_awap_message({}, websocket_utils.with_type('RECONNECT')),
        )
        session = __import__('darabonba.websocket', fromlist=['WebSocketSessionInfo']).WebSocketSessionInfo('s1')
        handler.handle_raw_message(
            session,
            WebSocketMessage(
                type=WebSocketMessageType.Text,
                payload=awap_text.encode('utf-8'),
                headers={},
            ),
        )
        self.assertEqual(1, len(errors))
        self.assertIn('reconnect failed', str(errors[0]))

    def test_awap_handler_must_implement_handle_awap_message(self):
        errors = []

        class RawOnlyHandler(MockWebSocketHandler):
            def handle_error(self, session, err):
                errors.append(err)

        handler = websocket_utils.StreamHandler(
            RawOnlyHandler(),
            websocket_utils.SubProtocolAWAP,
        )
        awap_text = websocket_utils.build_awap_message_text(
            websocket_utils.new_awap_message({'x': 1}, websocket_utils.with_type('UpstreamTextEvent')),
        )
        session = __import__('darabonba.websocket', fromlist=['WebSocketSessionInfo']).WebSocketSessionInfo('s1')
        handler.handle_raw_message(
            session,
            WebSocketMessage(
                type=WebSocketMessageType.Text,
                payload=awap_text.encode('utf-8'),
                headers={},
            ),
        )
        self.assertEqual(1, len(errors))
        self.assertIn('handle_awap_message', str(errors[0]))


class TestClientDoRequestWebSocket(unittest.TestCase):
    def _create_client(self):
        config = open_api_models.Config(
            access_key_id='ak',
            access_key_secret='sk',
            endpoint='example.com',
            protocol='ws',
        )
        cre_config = credential_models.Config(
            access_key_id='ak',
            access_key_secret='sk',
            type='access_key',
        )
        config.credential = CredentialClient(cre_config)
        return OpenApiClient(config)

    def test_require_websocket_sub_protocol(self):
        client = self._create_client()
        params = Params(
            action='TestAction',
            version='2020-01-01',
            protocol='ws',
            pathname='/ws',
            method='GET',
            auth_type='AK',
            body_type='string',
            req_body_type='json',
        )
        with self.assertRaisesRegex(Exception, 'websocketSubProtocol is required'):
            client.do_request(params, OpenApiRequest(), RuntimeOptions(web_socket_handler=MockWebSocketHandler()))

    def test_require_websocket_handler(self):
        client = self._create_client()
        params = Params(
            action='TestAction',
            version='2020-01-01',
            protocol='ws',
            pathname='/ws',
            method='GET',
            auth_type='AK',
            body_type='string',
            req_body_type='json',
            websocket_sub_protocol=websocket_utils.SubProtocolAWAP,
        )
        with self.assertRaisesRegex(Exception, 'WebSocketHandler is required'):
            client.do_request(params, OpenApiRequest(), RuntimeOptions())

    def test_reject_invalid_websocket_sub_protocol(self):
        client = self._create_client()
        params = Params(
            action='TestAction',
            version='2020-01-01',
            protocol='ws',
            pathname='/ws',
            method='GET',
            auth_type='AK',
            body_type='string',
            req_body_type='json',
            websocket_sub_protocol='invalid',
        )
        with self.assertRaisesRegex(Exception, "websocketSubProtocol must be 'awap' or 'general'"):
            client.do_request(
                params,
                OpenApiRequest(),
                RuntimeOptions(web_socket_handler=MockWebSocketHandler()),
            )

    def test_codegen_exports(self):
        from alibabacloud_tea_openapi import websocket_utils_models
        from alibabacloud_tea_openapi.websocket_utils import Client as WebSocketUtilsClient

        self.assertIs(websocket_utils_models.WebSocketClient, websocket_utils.WebSocketClient)
        self.assertTrue(issubclass(WebSocketUtilsClient, websocket_utils.WebSocketClient))
        self.assertIsNone(WebSocketUtilsClient.create_web_socket_client(None))
        ws_client = websocket_utils.WebSocketClient(
            unittest.mock.MagicMock(),
            __import__('darabonba.response', fromlist=['DaraResponse']).DaraResponse(),
        )
        self.assertIs(
            ws_client,
            WebSocketUtilsClient.create_web_socket_client(ws_client),
        )


if __name__ == '__main__':
    unittest.main()
