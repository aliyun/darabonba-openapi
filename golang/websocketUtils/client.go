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

func NewAwapMessage(msgType dara.AwapMessageType, id string, payload interface{}) *dara.AwapMessage {
	return &dara.AwapMessage{
		Type:    msgType,
		ID:      id,
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

// BuildAwapMessageBinary constructs a binary AWAP message
// Format: [header bytes (text converted to binary)] + [\n\n separator] + [binary body]
// The header part (including the separator) is converted from string to binary,
// then concatenated with the binary body to form the complete AWAP binary message.
func BuildAwapMessageBinary(message *dara.AwapMessage) ([]byte, error) {
	if message == nil {
		return nil, fmt.Errorf("message cannot be nil")
	}
	now := time.Now()
	var headerBuilder strings.Builder

	// Build header part (same format as text message)
	headerBuilder.WriteString(fmt.Sprintf("type:%s\n", string(message.Type)))
	headerBuilder.WriteString(fmt.Sprintf("timestamp:%d\n", now.Unix()*1000+int64(now.Nanosecond())/1e6))

	if message.ID != "" {
		headerBuilder.WriteString(fmt.Sprintf("id:%s\n", message.ID))
	}

	// Add custom headers if present
	if message.Headers != nil {
		for key, value := range message.Headers {
			headerBuilder.WriteString(fmt.Sprintf("%s:%s\n", key, value))
		}
	}

	// Add empty line to separate headers and payload
	headerBuilder.WriteString("\n")

	// Convert header string (including separator) to binary
	headerBytes := []byte(headerBuilder.String())

	var bodyBytes []byte
	if message.Payload != nil {
		// Check if Payload is already []byte
		if payloadBytes, ok := message.Payload.([]byte); ok {
			bodyBytes = payloadBytes
		} else {
			// If Payload is not []byte, try to convert it
			// For binary messages, Payload should be []byte
			return nil, fmt.Errorf("payload for binary AWAP message must be []byte, got %T", message.Payload)
		}
	} else {
		bodyBytes = []byte{} // Empty body
	}

	// Concatenate header bytes (including separator) + binary body
	result := make([]byte, 0, len(headerBytes)+len(bodyBytes))
	result = append(result, headerBytes...)
	result = append(result, bodyBytes...)

	return result, nil
}

// SendAwapMessage sends an AWAP protocol message
// AWAP protocol uses frame format: text headers + JSON payload
// Format: "type:request\nseq:1\ntimestamp:1234567890\nid:msg-001\nack:required\n\n{JSON payload}"
func (c *WebSocketClient) SendAwapTextMessage(message *dara.AwapMessage) error {
	messageText, err := BuildAwapMessageText(message)
	if err != nil {
		return fmt.Errorf("failed to build AWAP message: %w", err)
	}
	return c.wsClient.SendText(messageText)
}

func (c *WebSocketClient) SendRawAwapTextMessage(msgType dara.AwapMessageType, payload interface{}) error {
	id := generateMessageId()
	message := NewAwapMessage(msgType, id, payload)
	messageText, err := BuildAwapMessageText(message)
	if err != nil {
		return fmt.Errorf("failed to build AWAP message: %w", err)
	}
	return c.wsClient.SendText(messageText)
}

func (c *WebSocketClient) SendRawAwapTextMessageWithId(msgType dara.AwapMessageType, id string, payload interface{}) error {
	message := NewAwapMessage(msgType, id, payload)
	messageText, err := BuildAwapMessageText(message)
	if err != nil {
		return fmt.Errorf("failed to build AWAP message: %w", err)
	}
	return c.wsClient.SendText(messageText)
}

// SendAwapRequestWithAck sends an AWAP request that requires acknowledgment and waits for response
func (c *WebSocketClient) SendAwapRequestWithAck(message *dara.AwapMessage, timeout time.Duration) (*dara.AwapMessage, error) {
	messageText, err := BuildAwapMessageText(message)
	if err != nil {
		return nil, fmt.Errorf("failed to build AWAP message: %w", err)
	}
	return c.wsClient.(*dara.DefaultWebSocketClient).SendAwapRequestWithResponse(message.ID, messageText, timeout)
}

// SendRawAwapRequestWithAck sends an AWAP request that requires acknowledgment and waits for response
func (c *WebSocketClient) SendRawAwapRequestWithAck(id string, payload interface{}, timeout time.Duration) (*dara.AwapMessage, error) {
	message := NewAwapMessage("AckRequiredTextEvent", id, payload)
	messageText, err := BuildAwapMessageText(message)
	if err != nil {
		return nil, fmt.Errorf("failed to build AWAP message: %w", err)
	}
	return c.wsClient.(*dara.DefaultWebSocketClient).SendAwapRequestWithResponse(id, messageText, timeout)
}

func (c *WebSocketClient) SendAwapBinaryMessage(message *dara.AwapMessage) error {
	messageBinary, err := BuildAwapMessageBinary(message)
	if err != nil {
		return fmt.Errorf("failed to build AWAP message: %w", err)
	}
	return c.wsClient.SendBinary(messageBinary)
}

func (c *WebSocketClient) SendRawAwapBinaryMessage(msgType dara.AwapMessageType, payload interface{}) error {
	id := generateMessageId()
	message := NewAwapMessage(msgType, id, payload)
	messageBinary, err := BuildAwapMessageBinary(message)
	if err != nil {
		return fmt.Errorf("failed to build AWAP message: %w", err)
	}
	return c.wsClient.SendBinary(messageBinary)
}

func (c *WebSocketClient) SendRawAwapBinaryMessageWithId(msgType dara.AwapMessageType, id string, payload interface{}) error {
	message := NewAwapMessage(msgType, id, payload)
	messageBinary, err := BuildAwapMessageBinary(message)
	if err != nil {
		return fmt.Errorf("failed to build AWAP message: %w", err)
	}
	return c.wsClient.SendBinary(messageBinary)
}

// ============================================================================
// General Protocol Methods
// ============================================================================

func NewGeneralMessage(body string) *dara.GeneralMessage {
	return &dara.GeneralMessage{
		Body: body,
	}
}

func (c *WebSocketClient) SendGeneralTextMessage(text string) error {
	message := NewGeneralMessage(text)
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
