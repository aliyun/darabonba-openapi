// This file is NOT auto-generated.
// It is a Go-specific extension for WebSocket support in OpenAPI SDK.
//
// WebSocket implementations vary significantly across languages, so this
// functionality is implemented as language-specific extensions rather than
// being generated from main.tea.
//
// For details, see: examples/darabonba_websocket_integration_guide.md
//
// DO NOT run Darabonba generator with overwrite on this file.
package client

import (
	"context"
	"fmt"
	"time"

	"github.com/alibabacloud-go/tea/dara"
)

// ============================================================================
// AWAP Protocol Methods
// ============================================================================

// SendAwapMessage sends an AWAP protocol message
// AWAP protocol uses frame format: text headers + JSON payload
// Format: "type:request\nseq:1\ntimestamp:1234567890\nid:msg-001\nack:required\n\n{JSON payload}"
func (client *Client) SendAwapMessage(wsClient dara.WebSocketClient, message *dara.AwapMessage) error {
	frameData, err := dara.BuildAwapMessageText(message)
	if err != nil {
		return fmt.Errorf("failed to build AWAP message: %w", err)
	}

	fmt.Printf("[SDK AWAP] Sending frame:\n%s\n", frameData)

	ctx := context.Background()
	return wsClient.SendText(ctx, frameData)
}

func (client *Client) BuildAwapMessage(msgType dara.AwapMessageType, id string, seq int64, payload interface{}) *dara.AwapMessage {
	return &dara.AwapMessage{
		Type:    msgType,
		ID:      id,
		Seq:     seq,
		Payload: payload,
		Headers: make(map[string]string),
	}
}

func (client *Client) BuildAwapRequest(id string, seq int64, payload interface{}) *dara.AwapMessage {
	return client.BuildAwapMessage(dara.AwapMessageTypeUpstreamTextEvent, id, seq, payload)
}

func (client *Client) SendAwapRequest(wsClient dara.WebSocketClient, id string, seq int64, payload interface{}) error {
	message := client.BuildAwapRequest(id, seq, payload)
	return client.SendAwapMessage(wsClient, message)
}

// ============================================================================
// General Protocol Methods
// ============================================================================

// SendGeneralMessage sends a General protocol message
func (client *Client) SendGeneralMessage(wsClient dara.WebSocketClient, message *dara.GeneralMessage) error {
	if message.Headers == nil {
		message.Headers = make(map[string]string)
	}

	if message.Headers["type"] == "" {
		message.Headers["type"] = string(dara.GeneralMessageTypeUpstreamDefaultTextEvent)
	}

	jsonData, err := message.ToJSON()
	if err != nil {
		return err
	}

	ctx := context.Background()
	return wsClient.SendText(ctx, string(jsonData))
}

// BuildGeneralMessage builds a General protocol message with headers and body
func (client *Client) BuildGeneralMessage(headers map[string]string, body interface{}) *dara.GeneralMessage {
	if headers == nil {
		headers = make(map[string]string)
	}
	return &dara.GeneralMessage{
		Headers: headers,
		Body:    body,
	}
}

// BuildGeneralTextMessage builds a General protocol text message
func (client *Client) BuildGeneralTextMessage(body string) *dara.GeneralMessage {
	return client.BuildGeneralMessage(map[string]string{
		"Content-Type": "text/plain",
		"type":         string(dara.GeneralMessageTypeUpstreamDefaultTextEvent),
	}, body)
}

// BuildGeneralJSONMessage builds a General protocol JSON message
func (client *Client) BuildGeneralJSONMessage(body interface{}) *dara.GeneralMessage {
	return client.BuildGeneralMessage(map[string]string{
		"Content-Type": "application/json",
		"type":         string(dara.GeneralMessageTypeUpstreamDefaultTextEvent),
	}, body)
}

// BuildGeneralUpstreamTextMessage builds a General upstream text message with explicit type
func (client *Client) BuildGeneralUpstreamTextMessage(body interface{}) *dara.GeneralMessage {
	return client.BuildGeneralMessage(map[string]string{
		"Content-Type": "application/json",
		"type":         string(dara.GeneralMessageTypeUpstreamDefaultTextEvent),
	}, body)
}

// BuildGeneralDownstreamTextMessage builds a General downstream text message with explicit type
func (client *Client) BuildGeneralDownstreamTextMessage(body interface{}) *dara.GeneralMessage {
	return client.BuildGeneralMessage(map[string]string{
		"Content-Type": "application/json",
		"type":         string(dara.GeneralMessageTypeDownstreamDefaultTextEvent),
	}, body)
}

func (client *Client) SendGeneralTextMessage(wsClient dara.WebSocketClient, text string) error {
	message := client.BuildGeneralTextMessage(text)
	return client.SendGeneralMessage(wsClient, message)
}

func (client *Client) SendGeneralJSONMessage(wsClient dara.WebSocketClient, body interface{}) error {
	message := client.BuildGeneralJSONMessage(body)
	return client.SendGeneralMessage(wsClient, message)
}

func (client *Client) SendGeneralBinaryMessage(wsClient dara.WebSocketClient, data []byte) error {
	// Note: Binary messages don't have headers in the General protocol format
	// The type information would need to be handled at a different layer if required
	ctx := context.Background()
	return wsClient.SendBinary(ctx, data)
}

// ============================================================================
// AWAP Request-Response Pattern Methods (AckRequired)
// ============================================================================

// SendAwapRequestWithAck sends an AWAP request that requires acknowledgment and waits for response
func (client *Client) SendAwapRequestWithAck(wsClient dara.WebSocketClient, id string, seq int64, payload interface{}, timeout time.Duration) (*dara.AwapMessage, error) {
	return client.SendAwapRequestWithAckAndContext(context.Background(), wsClient, id, seq, payload, timeout)
}

func (client *Client) SendAwapRequestWithAckAndContext(ctx context.Context, wsClient dara.WebSocketClient, id string, seq int64, payload interface{}, timeout time.Duration) (*dara.AwapMessage, error) {
	message := client.BuildAwapMessage(dara.AwapMessageTypeAckRequiredTextEvent, id, seq, payload)

	// Convert to DefaultWebSocketClient to access SendAwapRequestWithResponse method
	if defaultClient, ok := wsClient.(*dara.DefaultWebSocketClient); ok {
		return defaultClient.SendAwapRequestWithResponse(ctx, message, timeout)
	}

	return nil, fmt.Errorf("wsClient does not support request-response pattern (type is %T)", wsClient)
}
