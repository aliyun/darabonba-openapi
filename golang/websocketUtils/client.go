package websocketUtils

import (
	"encoding/json"
	"fmt"
	"strings"
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

func (c *WebSocketClient) GetResponse() *dara.Response {
	return c.response
}

func (c *WebSocketClient) Close() error {
	if c.wsClient != nil {
		return c.wsClient.Close()
	}
	return nil
}

func (c *WebSocketClient) Reconnect() (*dara.Response, error) {
	if c.wsClient != nil {
		return c.wsClient.Reconnect()
	}
	return nil, fmt.Errorf("wsClient is nil")
}

func (c *WebSocketClient) Disconnect() error {
	if c.wsClient != nil {
		return c.wsClient.Disconnect()
	}
	return nil
}

func (c *WebSocketClient) IsConnected() bool {
	if c.wsClient != nil {
		return c.wsClient.IsConnected()
	}
	return false
}

func (c *WebSocketClient) GetSessionInfo() *dara.WebSocketSessionInfo {
	if c.wsClient != nil {
		return c.wsClient.GetSessionInfo()
	}
	return nil
}

type WebsocketClient struct {
	wsClient dara.WebSocketClient
	response *dara.Response
}

func NewWebsocketClient(wsClient dara.WebSocketClient, response *dara.Response) *WebsocketClient {
	return &WebsocketClient{
		wsClient: wsClient,
		response: response,
	}
}

// ============================================================================
// AWAP Protocol Methods
// ============================================================================

func BuildAwapMessage(msgType string, id string, seq int64, payload interface{}) *dara.AwapMessage {
	return &dara.AwapMessage{
		Type:    dara.AwapMessageType(msgType),
		ID:      id,
		Seq:     seq,
		Payload: payload,
		Headers: make(map[string]string),
	}
}

func BuildAwapMessageText(message *dara.AwapMessage) (string, error) {
	if message == nil {
		return "", fmt.Errorf("message cannot be nil")
	}
	now := time.Now()
	var headerBuilder strings.Builder

	headerBuilder.WriteString(fmt.Sprintf("type:%s\n", string(message.Type)))
	headerBuilder.WriteString(fmt.Sprintf("seq:%d\n", message.Seq))
	headerBuilder.WriteString(fmt.Sprintf("timestamp:%d\n", now.Unix()*1000+int64(now.Nanosecond())/1e6))

	if message.ID != "" {
		headerBuilder.WriteString(fmt.Sprintf("id:%s\n", message.ID))
	}

	// Auto-add ack:required for AckRequiredTextEvent
	if message.Type == "AckRequiredTextEvent" {
		headerBuilder.WriteString("ack:required\n")
	}

	// Add empty line to separate headers and payload
	headerBuilder.WriteString("\n")

	// Serialize payload to JSON
	var payloadJSON []byte
	var err error
	if message.Payload != nil {
		payloadJSON, err = json.Marshal(message.Payload)
		if err != nil {
			return "", fmt.Errorf("failed to marshal AWAP payload: %w", err)
		}
	} else {
		payloadJSON = []byte("{}")
	}

	return headerBuilder.String() + string(payloadJSON), nil
}

// SendAwapMessage sends an AWAP protocol message
// AWAP protocol uses frame format: text headers + JSON payload
// Format: "type:request\nseq:1\ntimestamp:1234567890\nid:msg-001\nack:required\n\n{JSON payload}"
func (c *WebSocketClient) SendAwapMessage(message *dara.AwapMessage) error {
	frameData, err := BuildAwapMessageText(message)
	if err != nil {
		return fmt.Errorf("failed to build AWAP message: %w", err)
	}
	return c.wsClient.SendText(frameData)
}

// SendAwapRequestWithAck sends an AWAP request that requires acknowledgment and waits for response
func (c *WebSocketClient) SendAwapRequestWithAck(id string, seq int64, payload interface{}, timeout time.Duration) (*dara.AwapMessage, error) {
	message := BuildAwapMessage("AckRequiredTextEvent", id, seq, payload)
	messageText, err := BuildAwapMessageText(message)
	if err != nil {
		return nil, fmt.Errorf("failed to build AWAP message: %w", err)
	}
	return c.wsClient.(*dara.DefaultWebSocketClient).SendAwapRequestWithResponse(id, messageText, timeout)
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
	return c.wsClient.SendText(string(jsonData))
}

func (c *WebSocketClient) SendGeneralBinaryMessage(data []byte) error {
	return c.wsClient.SendBinary(data)
}
