// This file is auto-generated, don't edit it. Thanks.
package client

import (
	"context"

	"github.com/alibabacloud-go/tea/dara"
)

// DoGeneralWebSocketRequest establishes a WebSocket connection with General protocol support
// Reuses runtime options for timeout, proxy, and auth configuration
//
// @param params - request parameters
// @param request - request object
// @param runtime - runtime options (reuses all configs from HTTP requests)
//
//	Can be *dara.RuntimeOptions (type alias) or *dara.ExtendedRuntimeOptions
//
// @param handler - General WebSocket message handler
// @return WebSocket client and error
func (client *Client) DoGeneralWebSocketRequest(params *Params, request *OpenApiRequest, runtime *dara.ExtendedRuntimeOptions, handler dara.GeneralWebSocketHandler) (*dara.DefaultWebSocketClient, error) {
	// Add General protocol identifier to headers
	if request.Headers == nil {
		request.Headers = make(map[string]*string)
	}
	request.Headers["X-Protocol"] = dara.String("General")

	// Reuse the base DoWebSocketRequest with General handler
	return client.DoWebSocketRequest(params, request, runtime, handler)
}

// SendGeneralMessage sends a General protocol message
//
// @param wsClient - WebSocket client
// @param message - General message to send
// @return error
func (client *Client) SendGeneralMessage(wsClient *dara.DefaultWebSocketClient, message *dara.GeneralMessage) error {
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
func (client *Client) SendGeneralTextMessage(wsClient *dara.DefaultWebSocketClient, text string) error {
	message := dara.BuildGeneralTextMessage(text)
	return client.SendGeneralMessage(wsClient, message)
}

// SendGeneralJSONMessage sends a General protocol JSON message
//
// @param wsClient - WebSocket client
// @param body - JSON body
// @return error
func (client *Client) SendGeneralJSONMessage(wsClient *dara.DefaultWebSocketClient, body interface{}) error {
	message := dara.BuildGeneralJSONMessage(body)
	return client.SendGeneralMessage(wsClient, message)
}

// SendGeneralBinaryMessage sends a General protocol binary message
//
// @param wsClient - WebSocket client
// @param data - binary data
// @return error
func (client *Client) SendGeneralBinaryMessage(wsClient *dara.DefaultWebSocketClient, data []byte) error {
	ctx := context.Background()
	return wsClient.SendBinary(ctx, data)
}
