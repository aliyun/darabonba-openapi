// This file is auto-generated, don't edit it. Thanks.
package client

import (
	"context"

	"github.com/alibabacloud-go/tea/dara"
)

// DoAwapWebSocketRequest establishes a WebSocket connection with AWAP protocol support
// Reuses runtime options for timeout, proxy, and auth configuration
//
// @param params - request parameters
// @param request - request object
// @param runtime - runtime options (reuses all configs from HTTP requests)
//
//	Can be *dara.RuntimeOptions (type alias) or *dara.ExtendedRuntimeOptions
//
// @param handler - AWAP WebSocket message handler
// @return WebSocket client and error
func (client *Client) DoAwapWebSocketRequest(params *Params, request *OpenApiRequest, runtime *dara.ExtendedRuntimeOptions, handler dara.AwapWebSocketHandler) (*dara.DefaultWebSocketClient, error) {
	// Add AWAP protocol identifier to headers
	if request.Headers == nil {
		request.Headers = make(map[string]*string)
	}
	request.Headers["X-Protocol"] = dara.String("AWAP")

	// Reuse the base DoWebSocketRequest with AWAP handler
	return client.DoWebSocketRequest(params, request, runtime, handler)
}

// SendAwapMessage sends an AWAP protocol message
//
// @param wsClient - WebSocket client
// @param message - AWAP message to send
// @return error
func (client *Client) SendAwapMessage(wsClient *dara.DefaultWebSocketClient, message *dara.AwapMessage) error {
	jsonData, err := message.ToJSON()
	if err != nil {
		return err
	}

	ctx := context.Background()
	return wsClient.SendText(ctx, string(jsonData))
}

// SendAwapRequest sends an AWAP request message
//
// @param wsClient - WebSocket client
// @param id - message ID
// @param seq - sequence number
// @param payload - message payload
// @return error
func (client *Client) SendAwapRequest(wsClient *dara.DefaultWebSocketClient, id string, seq int64, payload interface{}) error {
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
func (client *Client) SendAwapEvent(wsClient *dara.DefaultWebSocketClient, id string, seq int64, payload interface{}) error {
	message := dara.BuildAwapEvent(id, seq, payload)
	return client.SendAwapMessage(wsClient, message)
}
