package websocketUtils

import (
	"context"
	"fmt"
	"time"

	"github.com/alibabacloud-go/tea/dara"
)

type WebSocketClient struct {
	wsClient dara.WebSocketClient
	response *dara.Response
}

func NewWebSocketClient(wsClient dara.WebSocketClient, response *dara.Response) *WebSocketClient {
	return &WebSocketClient{
		wsClient: wsClient,
		response: response,
	}
}

func (c *WebSocketClient) GetWebSocketClient() dara.WebSocketClient {
	return c.wsClient
}

// GetResponse returns the WebSocket connection response
func (c *WebSocketClient) GetResponse() *dara.Response {
	return c.response
}

// Close closes the WebSocket connection
func (c *WebSocketClient) Close() error {
	if c.wsClient != nil {
		return c.wsClient.Close()
	}
	return nil
}

// Deprecated: Use WebSocketConnection instead
type WebsocketClient struct {
	wsClient dara.WebSocketClient
	response *dara.Response
}

// Deprecated: Use NewWebSocketConnection instead
func NewWebsocketClient(wsClient dara.WebSocketClient, response *dara.Response) *WebsocketClient {
	return &WebsocketClient{
		wsClient: wsClient,
		response: response,
	}
}

// ============================================================================
// AWAP Protocol Methods
// ============================================================================

// SendAwapMessage sends an AWAP protocol message
// AWAP protocol uses frame format: text headers + JSON payload
// Format: "type:request\nseq:1\ntimestamp:1234567890\nid:msg-001\nack:required\n\n{JSON payload}"
func (c *WebSocketClient) SendAwapMessage(message *dara.AwapMessage) error {
	frameData, err := dara.BuildAwapMessageText(message)
	if err != nil {
		return fmt.Errorf("failed to build AWAP message: %w", err)
	}

	ctx := context.Background()
	return c.wsClient.SendText(ctx, frameData)
}

func BuildAwapMessage(msgType dara.AwapMessageType, id string, seq int64, payload interface{}) *dara.AwapMessage {
	return &dara.AwapMessage{
		Type:    msgType,
		ID:      id,
		Seq:     seq,
		Payload: payload,
		Headers: make(map[string]string),
	}
}

func BuildAwapRequest(id string, seq int64, payload interface{}) *dara.AwapMessage {
	return BuildAwapMessage(dara.AwapMessageTypeUpstreamTextEvent, id, seq, payload)
}

func (c *WebSocketClient) SendAwapRequest(id string, seq int64, payload interface{}) error {
	message := BuildAwapRequest(id, seq, payload)
	return c.SendAwapMessage(message)
}

// ============================================================================
// General Protocol Methods
// ============================================================================

func BuildGeneralTextMessage(body string) *dara.GeneralMessage {
	return &dara.GeneralMessage{
		Body: body,
	}
}

func (c *WebSocketClient) SendGeneralTextMessage(text string) error {
	message := BuildGeneralTextMessage(text)
	jsonData, err := message.ToJSON()
	if err != nil {
		return err
	}

	ctx := context.Background()
	return c.wsClient.SendText(ctx, string(jsonData))
}

func (c *WebSocketClient) SendGeneralBinaryMessage(data []byte) error {
	ctx := context.Background()
	return c.wsClient.SendBinary(ctx, data)
}

// ============================================================================
// AWAP Request-Response Pattern Methods (AckRequired)
// ============================================================================

// SendAwapRequestWithAck sends an AWAP request that requires acknowledgment and waits for response
func (c *WebSocketClient) SendAwapRequestWithAck(id string, seq int64, payload interface{}, timeout time.Duration) (*dara.AwapMessage, error) {
	return c.SendAwapRequestWithAckAndContext(context.Background(), id, seq, payload, timeout)
}

func (c *WebSocketClient) SendAwapRequestWithAckAndContext(ctx context.Context, id string, seq int64, payload interface{}, timeout time.Duration) (*dara.AwapMessage, error) {
	message := BuildAwapMessage(dara.AwapMessageTypeAckRequiredTextEvent, id, seq, payload)
	return c.wsClient.(*dara.DefaultWebSocketClient).SendAwapRequestWithResponse(ctx, message, timeout)
}
