package websocketutils

import (
	"crypto/rand"
	"encoding/hex"
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

// ============================================================================
// AWAP Protocol Methods
// ============================================================================

func BuildAwapMessage(msgType dara.AwapMessageType, id string, seq int64, payload interface{}) *dara.AwapMessage {
	return &dara.AwapMessage{
		Type:    msgType,
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
	messageText, err := BuildAwapMessageText(message)
	if err != nil {
		return fmt.Errorf("failed to build AWAP message: %w", err)
	}
	return c.wsClient.SendText(messageText)
}

func (c *WebSocketClient) SendRawAwapMessage(msgType dara.AwapMessageType, seq int64, payload interface{}) error {
	id := generateMessageId()
	message := BuildAwapMessage(msgType, id, seq, payload)
	messageText, err := BuildAwapMessageText(message)
	if err != nil {
		return fmt.Errorf("failed to build AWAP message: %w", err)
	}
	return c.wsClient.SendText(messageText)
}

func (c *WebSocketClient) SendRawAwapMessageWithId(msgType dara.AwapMessageType, id string, seq int64, payload interface{}) error {
	message := BuildAwapMessage(msgType, id, seq, payload)
	messageText, err := BuildAwapMessageText(message)
	if err != nil {
		return fmt.Errorf("failed to build AWAP message: %w", err)
	}
	return c.wsClient.SendText(messageText)
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

// generateMessageId generates a 32-character message ID
// Logic matches Java implementation:
// 1. Generate random hex string (equivalent to UUID without hyphens)
// 2. If hex string length >= 32, return first 32 characters
// 3. Otherwise, combine timestamp and hex string
// 4. If combined length >= 32, return first 32 characters
// 5. If still less than 32, pad with zeros to 32 characters
func generateMessageId() string {
	// Generate 16 random bytes and convert to hex (32 characters, equivalent to UUID without hyphens)
	randomBytes := make([]byte, 16)
	if _, err := rand.Read(randomBytes); err != nil {
		// Fallback: use timestamp-based ID if random generation fails
		now := time.Now()
		timestamp := fmt.Sprintf("%d", now.Unix()*1000+int64(now.Nanosecond())/1e6)
		// Pad timestamp to 32 characters
		padded := fmt.Sprintf("%-32s", timestamp)
		return strings.ReplaceAll(padded, " ", "0")
	}
	hexStr := hex.EncodeToString(randomBytes)

	// If hex string length >= 32, return first 32 characters
	if len(hexStr) >= 32 {
		return hexStr[:32]
	}

	// Otherwise, combine timestamp and hex string
	// Use UnixMilli equivalent for Go 1.14 compatibility
	now := time.Now()
	timestamp := fmt.Sprintf("%d", now.Unix()*1000+int64(now.Nanosecond())/1e6)
	combined := timestamp + hexStr

	// If combined length >= 32, return first 32 characters
	if len(combined) >= 32 {
		return combined[:32]
	}

	// If still less than 32, pad with zeros to 32 characters
	// Format: %-32s pads on the right, then replace spaces with zeros
	padded := fmt.Sprintf("%-32s", combined)
	return strings.ReplaceAll(padded, " ", "0")
}
