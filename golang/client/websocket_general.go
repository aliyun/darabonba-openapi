// This file is auto-generated, don't edit it. Thanks.
package client

import (
	"context"

	"github.com/alibabacloud-go/tea/dara"
)

// SendGeneralMessage sends a General protocol message
//
// @param wsClient - WebSocket client
// @param message - General message to send
// @return error
func (client *Client) SendGeneralMessage(wsClient dara.WebSocketClient, message *dara.GeneralMessage) error {
	// Ensure headers exist
	if message.Headers == nil {
		message.Headers = make(map[string]string)
	}

	// If type is not set in headers, set default upstream type for text messages
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

// SendGeneralTextMessage sends a General protocol text message
//
// @param wsClient - WebSocket client
// @param text - text content
// @return error
func (client *Client) SendGeneralTextMessage(wsClient dara.WebSocketClient, text string) error {
	message := dara.BuildGeneralTextMessage(text)
	return client.SendGeneralMessage(wsClient, message)
}

// SendGeneralJSONMessage sends a General protocol JSON message
//
// @param wsClient - WebSocket client
// @param body - JSON body
// @return error
func (client *Client) SendGeneralJSONMessage(wsClient dara.WebSocketClient, body interface{}) error {
	message := dara.BuildGeneralJSONMessage(body)
	return client.SendGeneralMessage(wsClient, message)
}

// SendGeneralBinaryMessage sends a General protocol binary message
//
// @param wsClient - WebSocket client
// @param data - binary data
// @return error
func (client *Client) SendGeneralBinaryMessage(wsClient dara.WebSocketClient, data []byte) error {
	// Note: Binary messages don't have headers in the General protocol format
	// The type information would need to be handled at a different layer if required
	ctx := context.Background()
	return wsClient.SendBinary(ctx, data)
}
