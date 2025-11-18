// This file is auto-generated, don't edit it. Thanks.
package client

import (
	"context"
	"encoding/json"
	"fmt"
	"strings"
	"time"

	"github.com/alibabacloud-go/tea/dara"
)

// SendAwapMessage sends an AWAP protocol message
// AWAP protocol uses frame format: text headers + JSON payload
// Format: "type:request\nseq:1\ntimestamp:1234567890\nid:msg-001\nack:required\n\n{JSON payload}"
//
// @param wsClient - WebSocket client
// @param message - AWAP message to send
// @return error
func (client *Client) SendAwapMessage(wsClient dara.WebSocketClient, message *dara.AwapMessage) error {
	// Build AWAP frame header (text format)
	var headerBuilder strings.Builder

	// Add type
	headerBuilder.WriteString(fmt.Sprintf("type:%s\n", string(message.Type)))

	// Add seq
	headerBuilder.WriteString(fmt.Sprintf("seq:%d\n", message.Seq))

	// Add timestamp
	timestamp := time.Now().UnixMilli()
	headerBuilder.WriteString(fmt.Sprintf("timestamp:%d\n", timestamp))

	// Add id
	if message.ID != "" {
		headerBuilder.WriteString(fmt.Sprintf("id:%s\n", message.ID))
	}

	// Add ack if it's a request or AckRequiredTextEvent
	if message.Type == dara.AwapMessageTypeRequest || message.Type == dara.AwapMessageTypeAckRequiredTextEvent {
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
			return fmt.Errorf("failed to marshal AWAP payload: %w", err)
		}
	} else {
		payloadJSON = []byte("{}")
	}

	// Combine header and payload
	frameData := headerBuilder.String() + string(payloadJSON)

	// Debug: log the AWAP frame being sent
	fmt.Printf("[AWAP] Sending frame:\n%s\n", frameData)

	ctx := context.Background()
	return wsClient.SendText(ctx, frameData)
}

// SendAwapRequest sends an AWAP request message
//
// @param wsClient - WebSocket client
// @param id - message ID
// @param seq - sequence number
// @param payload - message payload
// @return error
func (client *Client) SendAwapRequest(wsClient dara.WebSocketClient, id string, seq int64, payload interface{}) error {
	message := dara.BuildAwapRequest(id, seq, payload)
	return client.SendAwapMessage(wsClient, message)
}

// SendAwapEvent sends an AWAP event message
//
// @param wsClient - WebSocket client
// @param id - message ID
// @param seq - sequence number
// @param payload - event payload
// @return error
func (client *Client) SendAwapEvent(wsClient dara.WebSocketClient, id string, seq int64, payload interface{}) error {
	message := dara.BuildAwapEvent(id, seq, payload)
	return client.SendAwapMessage(wsClient, message)
}
