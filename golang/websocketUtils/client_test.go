package websocketUtils

import (
	"context"
	"strings"
	"testing"
	"time"

	"github.com/alibabacloud-go/tea/dara"
	"github.com/alibabacloud-go/tea/utils"
)

type mockWSClient struct {
	connected    bool
	texts        []string
	binaries     [][]byte
	closeErr     error
	reconnectErr error
	gracefulErr  error
	sendErr      error
	session      *dara.WebSocketSessionInfo
}

func (m *mockWSClient) Connect(ctx context.Context, request *dara.Request, runtimeObject *dara.RuntimeObject) (*dara.Response, error) {
	return &dara.Response{}, nil
}

func (m *mockWSClient) Disconnect() error { return nil }

func (m *mockWSClient) Reconnect() (*dara.Response, error) {
	if m.reconnectErr != nil {
		return nil, m.reconnectErr
	}
	return &dara.Response{}, nil
}

func (m *mockWSClient) ReconnectGracefully() (*dara.Response, error) {
	if m.gracefulErr != nil {
		return nil, m.gracefulErr
	}
	return &dara.Response{}, nil
}

func (m *mockWSClient) IsConnected() bool { return m.connected }

func (m *mockWSClient) SendText(text string) error {
	m.texts = append(m.texts, text)
	return m.sendErr
}

func (m *mockWSClient) SendBinary(data []byte) error {
	m.binaries = append(m.binaries, data)
	return m.sendErr
}

func (m *mockWSClient) GetSessionInfo() *dara.WebSocketSessionInfo { return m.session }

func (m *mockWSClient) Close() error { return m.closeErr }

func TestCreateAndLifecycle(t *testing.T) {
	utils.AssertNil(t, CreateWebSocketClient(nil))
	utils.AssertNil(t, CreateWebSocketClient("nope"))

	inner := &mockWSClient{connected: true, session: &dara.WebSocketSessionInfo{SessionID: "s1"}}
	client := NewWebSocketClient(inner, &dara.Response{StatusCode: dara.Int(101)})
	utils.AssertEqual(t, client, CreateWebSocketClient(client))
	utils.AssertEqual(t, 101, dara.IntValue(client.GetResponse().StatusCode))
	utils.AssertEqual(t, true, client.IsConnected())
	utils.AssertEqual(t, "s1", client.GetSessionInfo().SessionID)
	utils.AssertNil(t, client.Validate())
	utils.AssertNil(t, client.Disconnect())
	utils.AssertNil(t, client.Close())
	_, err := client.Reconnect()
	utils.AssertNil(t, err)
	_, err = client.ReconnectGracefully()
	utils.AssertNil(t, err)

	empty := &WebSocketClient{}
	utils.AssertEqual(t, false, empty.IsConnected())
	utils.AssertNil(t, empty.GetSessionInfo())
	utils.AssertNil(t, empty.Close())
	utils.AssertNil(t, empty.Disconnect())
	utils.AssertNotNil(t, empty.Validate())
	_, err = empty.Reconnect()
	utils.AssertNotNil(t, err)
	_, err = empty.ReconnectGracefully()
	utils.AssertNotNil(t, err)
}

func TestAwapBuildParseAndSend(t *testing.T) {
	msg := NewAwapMessage(map[string]string{"hello": "world"}, WithType("UpstreamTextEvent"), WithID("msg-001"), WithHeader("custom", "value"))
	msg.WithHeader("extra", "1").WithFormat(AwapMessageFormatText)
	text, err := BuildAwapMessageText(msg)
	utils.AssertNil(t, err)
	utils.AssertEqual(t, true, strings.Contains(text, "type:UpstreamTextEvent"))
	utils.AssertEqual(t, true, strings.Contains(text, "id:msg-001"))
	utils.AssertEqual(t, true, strings.Contains(text, "custom:value"))

	parsed, err := ParseAwapMessage(&dara.WebSocketMessage{
		Type:    dara.WebSocketMessageTypeText,
		Payload: []byte(text),
	})
	utils.AssertNil(t, err)
	utils.AssertEqual(t, AwapMessageType("UpstreamTextEvent"), parsed.Type)
	utils.AssertEqual(t, "msg-001", parsed.ID)
	utils.AssertEqual(t, "value", parsed.Headers["custom"])

	ack := NewAwapMessage(map[string]string{}, WithType("AckRequiredTextEvent"))
	ackText, err := BuildAwapMessageText(ack)
	utils.AssertNil(t, err)
	utils.AssertEqual(t, true, strings.Contains(ackText, "ack:required"))

	_, err = BuildAwapMessageText(nil)
	utils.AssertNotNil(t, err)
	_, err = ParseAwapMessage(&dara.WebSocketMessage{Payload: []byte("no-separator")})
	utils.AssertNotNil(t, err)

	bin, err := BuildAwapMessageBinary(&AwapMessage{Type: "UpstreamBinaryEvent", ID: "b1", Payload: []byte("bin")})
	utils.AssertNil(t, err)
	utils.AssertEqual(t, true, len(bin) > 0)
	_, err = BuildAwapMessageBinary(nil)
	utils.AssertNotNil(t, err)
	_, err = BuildAwapMessageBinary(&AwapMessage{Payload: "not-bytes"})
	utils.AssertNotNil(t, err)
	emptyBin, err := BuildAwapMessageBinary(&AwapMessage{Type: "UpstreamBinaryEvent"})
	utils.AssertNil(t, err)
	utils.AssertNotNil(t, emptyBin)

	parsedBin, err := ParseAwapMessage(&dara.WebSocketMessage{
		Type:    dara.WebSocketMessageTypeBinary,
		Payload: bin,
	})
	utils.AssertNil(t, err)
	utils.AssertEqual(t, AwapMessageFormatBinary, parsedBin.Format)

	rawJSON, err := msg.ToJSON()
	utils.AssertNil(t, err)
	utils.AssertEqual(t, true, len(rawJSON) > 0)

	inner := &mockWSClient{connected: true}
	client := NewWebSocketClient(inner, &dara.Response{})
	utils.AssertNil(t, client.SendAwapTextMessage(msg))
	utils.AssertNil(t, client.SendRawAwapTextMessage("UpstreamTextEvent", map[string]string{"a": "b"}))
	utils.AssertNil(t, client.SendRawAwapTextMessageWithId("UpstreamTextEvent", "id-1", map[string]string{"a": "b"}))
	utils.AssertNil(t, client.SendAwapBinaryMessage(&AwapMessage{Type: "UpstreamBinaryEvent", Payload: []byte("x")}))
	utils.AssertNil(t, client.SendRawAwapBinaryMessage("UpstreamBinaryEvent", []byte("y")))
	utils.AssertNil(t, client.SendRawAwapBinaryMessageWithId("UpstreamBinaryEvent", "id-2", []byte("z")))
	utils.AssertNil(t, client.SendGeneralTextMessage("hello"))
	utils.AssertNil(t, client.SendGeneralBinaryMessage([]byte("bin")))
	utils.AssertEqual(t, true, len(inner.texts) >= 4)
	utils.AssertEqual(t, true, len(inner.binaries) >= 3)

	id := generateMessageId()
	utils.AssertEqual(t, 32, len(id))

	utils.AssertNotNil(t, client.SendAwapTextMessage(nil))
	utils.AssertNotNil(t, client.SendAwapBinaryMessage(nil))
	_, err = client.SendAwapRequestWithAck(nil, time.Millisecond)
	utils.AssertNotNil(t, err)
	emptyHeaders := &AwapMessage{Type: "UpstreamTextEvent"}
	emptyHeaders.WithHeader("k", "v").WithHeader("k2", "v2")
	utils.AssertEqual(t, "v", emptyHeaders.Headers["k"])
	WithHeader("k3", "v3")(emptyHeaders)
	utils.AssertEqual(t, "v3", emptyHeaders.Headers["k3"])
}

func TestSendRequestAndComplete(t *testing.T) {
	inner := &mockWSClient{connected: true}
	client := NewWebSocketClient(inner, &dara.Response{})
	_, err := client.SendRequest("", "text", time.Millisecond)
	utils.AssertNotNil(t, err)
	_, err = client.SendRequest("id", "", time.Millisecond)
	utils.AssertNotNil(t, err)

	done := make(chan interface{}, 1)
	go func() {
		resp, reqErr := client.SendRequest("ack-001", "type:test\nid:ack-001\n\n{}", 2*time.Second)
		if reqErr != nil {
			done <- reqErr
			return
		}
		done <- resp
	}()
	time.Sleep(20 * time.Millisecond)
	utils.AssertEqual(t, true, client.completeRequest("ack-001", map[string]bool{"ok": true}))
	got := <-done
	utils.AssertEqual(t, true, got.(map[string]bool)["ok"])
	utils.AssertEqual(t, false, client.completeRequest("missing", "x"))

	inner.sendErr = &timeoutError{"send failed"}
	_, err = client.SendRequest("ack-002", "payload", time.Millisecond)
	utils.AssertNotNil(t, err)

	inner.sendErr = nil
	_, err = client.SendRequest("ack-003", "payload", time.Millisecond)
	utils.AssertNotNil(t, err)

	_, err = client.SendAwapRequestWithAck(NewAwapMessage(map[string]string{"a": "b"}, WithID("ack-004")), time.Millisecond)
	utils.AssertNotNil(t, err)

	inner.sendErr = &timeoutError{"send failed"}
	utils.AssertNotNil(t, client.SendRawAwapTextMessage("UpstreamTextEvent", map[string]string{"a": "b"}))
	utils.AssertNotNil(t, client.SendRawAwapTextMessageWithId("UpstreamTextEvent", "id-err", map[string]string{"a": "b"}))
	utils.AssertNotNil(t, client.SendRawAwapBinaryMessage("UpstreamBinaryEvent", []byte("y")))
	utils.AssertNotNil(t, client.SendRawAwapBinaryMessageWithId("UpstreamBinaryEvent", "id-err", []byte("z")))
	utils.AssertNotNil(t, client.SendGeneralTextMessage("hello"))
	utils.AssertNotNil(t, client.SendAwapTextMessage(NewAwapMessage(make(chan int), WithType("UpstreamTextEvent"))))
}

type timeoutError struct{ msg string }

func (e *timeoutError) Error() string { return e.msg }

func TestParseGeneralMessages(t *testing.T) {
	parsed, err := ParseGeneralMessage(&dara.WebSocketMessage{
		Type:    dara.WebSocketMessageTypeText,
		Payload: []byte(`{"body":"hello"}`),
	})
	utils.AssertNil(t, err)
	utils.AssertEqual(t, GeneralMessageFormatText, parsed.Format)

	parsed, err = ParseGeneralMessage(&dara.WebSocketMessage{
		Type:    dara.WebSocketMessageTypeBinary,
		Payload: []byte("binary-data"),
	})
	utils.AssertNil(t, err)
	utils.AssertEqual(t, GeneralMessageFormatBinary, parsed.Format)

	parsed, err = ParseGeneralMessage(&dara.WebSocketMessage{
		Type:    dara.WebSocketMessageTypeText,
		Payload: []byte("not-json"),
	})
	utils.AssertNil(t, err)
	utils.AssertEqual(t, GeneralMessageFormatText, parsed.Format)

	msg := NewGeneralMessage("hello")
	raw, err := msg.ToJSON()
	utils.AssertNil(t, err)
	utils.AssertEqual(t, true, len(raw) > 0)
	utils.AssertEqual(t, ErrUseRawMessage, (&AbstractGeneralWebSocketHandler{}).HandleGeneralMessage(nil, msg))
	utils.AssertEqual(t, ErrUseRawMessage, (&AbstractAwapWebSocketHandler{}).HandleAwapMessage(nil, &AwapMessage{}))
}

type recordingHandler struct {
	dara.AbstractWebSocketHandler
	rawCount   int
	errCount   int
	lastError  error
	awapCount  int
	generalCnt int
	awapErr    error
}

func (h *recordingHandler) HandleRawMessage(session *dara.WebSocketSessionInfo, message *dara.WebSocketMessage) error {
	h.rawCount++
	return nil
}

func (h *recordingHandler) HandleError(session *dara.WebSocketSessionInfo, err error) error {
	h.errCount++
	h.lastError = err
	return nil
}

func (h *recordingHandler) HandleAwapMessage(session *dara.WebSocketSessionInfo, message *AwapMessage) error {
	h.awapCount++
	if h.awapErr != nil {
		return h.awapErr
	}
	return nil
}

func (h *recordingHandler) HandleGeneralMessage(session *dara.WebSocketSessionInfo, message *GeneralMessage) error {
	h.generalCnt++
	return nil
}

func TestStreamHandlerRouting(t *testing.T) {
	session := &dara.WebSocketSessionInfo{SessionID: "s1"}
	user := &recordingHandler{}
	handler := &StreamHandler{UserHandler: user, SubProtocol: SubProtocolAWAP}
	utils.AssertNil(t, handler.AfterConnectionEstablished(session))
	utils.AssertNil(t, handler.HandleError(session, &timeoutError{"x"}))
	utils.AssertNil(t, handler.AfterConnectionClosed(session, 1000, "bye"))

	text, err := BuildAwapMessageText(NewAwapMessage(map[string]string{"x": "1"}, WithType("UpstreamTextEvent"), WithID("m1")))
	utils.AssertNil(t, err)
	err = handler.HandleRawMessage(session, &dara.WebSocketMessage{Type: dara.WebSocketMessageTypeText, Payload: []byte(text)})
	utils.AssertNil(t, err)
	utils.AssertEqual(t, 1, user.awapCount)

	user.awapErr = ErrUseRawMessage
	err = handler.HandleRawMessage(session, &dara.WebSocketMessage{Type: dara.WebSocketMessageTypeText, Payload: []byte(text)})
	utils.AssertNil(t, err)
	utils.AssertEqual(t, true, user.rawCount > 0)

	user.awapErr = &timeoutError{"awap failed"}
	err = handler.HandleRawMessage(session, &dara.WebSocketMessage{Type: dara.WebSocketMessageTypeText, Payload: []byte(text)})
	utils.AssertNil(t, err)
	utils.AssertEqual(t, true, user.errCount > 0)

	inner := &mockWSClient{}
	client := NewWebSocketClient(inner, &dara.Response{})
	handler.Client = client
	reconnectText, err := BuildAwapMessageText(NewAwapMessage(map[string]string{}, WithType("RECONNECT")))
	utils.AssertNil(t, err)
	err = handler.HandleRawMessage(session, &dara.WebSocketMessage{Type: dara.WebSocketMessageTypeText, Payload: []byte(reconnectText)})
	utils.AssertNil(t, err)
	time.Sleep(20 * time.Millisecond)

	ackText, err := BuildAwapMessageText(NewAwapMessage(map[string]string{}, WithType("DownstreamTextEvent"), WithHeader("ack-id", "pending-1")))
	utils.AssertNil(t, err)
	done := make(chan error, 1)
	go func() {
		_, reqErr := client.SendRequest("pending-1", "msg", time.Second)
		done <- reqErr
	}()
	time.Sleep(20 * time.Millisecond)
	err = handler.HandleRawMessage(session, &dara.WebSocketMessage{Type: dara.WebSocketMessageTypeText, Payload: []byte(ackText)})
	utils.AssertNil(t, err)
	utils.AssertNil(t, <-done)

	generalUser := &recordingHandler{}
	generalHandler := &StreamHandler{UserHandler: generalUser, SubProtocol: SubProtocolGeneral}
	err = generalHandler.HandleRawMessage(session, &dara.WebSocketMessage{Type: dara.WebSocketMessageTypeText, Payload: []byte(`{"body":"hi"}`)})
	utils.AssertNil(t, err)
	utils.AssertEqual(t, 1, generalUser.generalCnt)

	fallback := &StreamHandler{UserHandler: &recordingHandler{}, SubProtocol: "other"}
	err = fallback.HandleRawMessage(session, &dara.WebSocketMessage{Type: dara.WebSocketMessageTypeText, Payload: []byte("{}")})
	utils.AssertNil(t, err)

	plain := &StreamHandler{UserHandler: &dara.AbstractWebSocketHandler{}, SubProtocol: SubProtocolAWAP}
	err = plain.HandleRawMessage(session, &dara.WebSocketMessage{Type: dara.WebSocketMessageTypeText, Payload: []byte(text)})
	utils.AssertNil(t, err)

	plainGeneral := &StreamHandler{UserHandler: &dara.AbstractWebSocketHandler{}, SubProtocol: SubProtocolGeneral}
	err = plainGeneral.HandleRawMessage(session, &dara.WebSocketMessage{Type: dara.WebSocketMessageTypeText, Payload: []byte(`{"body":"hi"}`)})
	utils.AssertNil(t, err)

	adapter := &generalAdapter{inner: &recordingHandler{}, mode: "raw"}
	err = (&StreamHandler{UserHandler: adapter, SubProtocol: SubProtocolGeneral}).HandleRawMessage(session, &dara.WebSocketMessage{Type: dara.WebSocketMessageTypeText, Payload: []byte(`{"body":"hi"}`)})
	utils.AssertNil(t, err)
	adapter.mode = "err"
	err = (&StreamHandler{UserHandler: adapter, SubProtocol: SubProtocolGeneral}).HandleRawMessage(session, &dara.WebSocketMessage{Type: dara.WebSocketMessageTypeText, Payload: []byte(`{"body":"hi"}`)})
	utils.AssertNil(t, err)
}

type generalAdapter struct {
	dara.AbstractWebSocketHandler
	inner *recordingHandler
	mode  string
}

func (h *generalAdapter) HandleGeneralMessage(session *dara.WebSocketSessionInfo, message *GeneralMessage) error {
	if h.mode == "raw" {
		return ErrUseRawMessage
	}
	return &timeoutError{"general failed"}
}

func (h *generalAdapter) HandleRawMessage(session *dara.WebSocketSessionInfo, message *dara.WebSocketMessage) error {
	if h.mode == "raw" {
		return &timeoutError{"raw failed"}
	}
	return nil
}

func (h *generalAdapter) HandleError(session *dara.WebSocketSessionInfo, err error) error {
	h.inner.lastError = err
	h.inner.errCount++
	return nil
}
