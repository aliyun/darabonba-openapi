// This file is auto-generated, don't edit it. Thanks.
package client

import (
	"context"
	"fmt"
	"time"

	"github.com/alibabacloud-go/tea/dara"
)

// DoWebSocketRequest establishes a WebSocket connection using runtime options
// This method reuses the same configuration pattern as DoRPCRequest/DoROARequest
//
// @param params - request parameters
// @param request - request object
// @param runtime - runtime options (reuses timeout, proxy, SSL configs)
//
//	Can be *dara.RuntimeOptions (type alias) or *dara.ExtendedRuntimeOptions
//
// @param handler - WebSocket message handler
// @return WebSocket client and error
func (client *Client) DoWebSocketRequest(params *Params, request *OpenApiRequest, runtime interface{}, handler dara.WebSocketHandler) (*dara.DefaultWebSocketClient, error) {
	// Build WebSocket URL
	url, err := client.buildWebSocketURL(params, request)
	if err != nil {
		return nil, err
	}

	// Extract base RuntimeOptions (works with both type alias and ExtendedRuntimeOptions)
	var baseRuntime *dara.RuntimeOptions
	if ext, ok := runtime.(*dara.ExtendedRuntimeOptions); ok {
		baseRuntime = ext.ToRuntimeOptions()
	} else if rt, ok := runtime.(*dara.RuntimeOptions); ok {
		baseRuntime = rt
	} else {
		return nil, fmt.Errorf("runtime must be *dara.RuntimeOptions or *dara.ExtendedRuntimeOptions")
	}

	// Helper function to safely convert interface{} from Default to *int
	getIntValue := func(val interface{}) *int {
		if val == nil {
			return nil
		}
		if intPtr, ok := val.(*int); ok {
			return intPtr
		}
		return nil
	}

	// Helper function to safely convert interface{} from Default to *bool
	getBoolValue := func(val interface{}) *bool {
		if val == nil {
			return nil
		}
		if boolPtr, ok := val.(*bool); ok {
			return boolPtr
		}
		return nil
	}

	// Create WebSocket config from runtime options (reuse existing configs)
	config := &dara.WebSocketConfig{
		URL:     url,
		Headers: make(map[string]string),
		// Reuse timeouts from runtime/client
		ConnectTimeout: time.Duration(dara.IntValue(getIntValue(dara.Default(baseRuntime.ConnectTimeout, client.ConnectTimeout)))) * time.Millisecond,
		ReadTimeout:    time.Duration(dara.IntValue(getIntValue(dara.Default(baseRuntime.ReadTimeout, client.ReadTimeout)))) * time.Millisecond,
		// WebSocket specific settings from runtime ⭐
		// Use helper functions to safely extract WebSocket config (works with both types)
		WriteTimeout:      time.Duration(dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketWriteTimeout(runtime), dara.Int(10000))))) * time.Millisecond,
		HandshakeTimeout:  time.Duration(dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketHandshakeTimeout(runtime), dara.Int(10000))))) * time.Millisecond,
		PingInterval:      time.Duration(dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketPingInterval(runtime), dara.Int(30000))))) * time.Millisecond,
		PongTimeout:       time.Duration(dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketPongTimeout(runtime), dara.Int(10000))))) * time.Millisecond,
		MaxMessageSize:    int64(dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketMaxMessageSize(runtime), dara.Int(1024*1024))))),
		EnableReconnect:   dara.BoolValue(getBoolValue(dara.Default(dara.GetWebSocketEnableReconnect(runtime), dara.Bool(true)))),
		ReconnectInterval: time.Duration(dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketReconnectInterval(runtime), dara.Int(5000))))) * time.Millisecond,
		MaxReconnectTimes: dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketMaxReconnectTimes(runtime), dara.Int(5)))),
	}

	// Reuse proxy settings from runtime
	if baseRuntime.HttpProxy != nil {
		config.Headers["X-Http-Proxy"] = dara.StringValue(baseRuntime.HttpProxy)
	}
	if baseRuntime.HttpsProxy != nil {
		config.Headers["X-Https-Proxy"] = dara.StringValue(baseRuntime.HttpsProxy)
	}

	// Add headers from request
	if request.Headers != nil {
		for k, v := range request.Headers {
			if v != nil {
				config.Headers[k] = dara.StringValue(v)
			}
		}
	}

	// Add global headers
	if client.GlobalParameters != nil && client.GlobalParameters.Headers != nil {
		for k, v := range client.GlobalParameters.Headers {
			if v != nil {
				config.Headers[k] = dara.StringValue(v)
			}
		}
	}

	// Add extended headers from runtime
	if baseRuntime.ExtendsParameters != nil && baseRuntime.ExtendsParameters.Headers != nil {
		for k, v := range baseRuntime.ExtendsParameters.Headers {
			if v != nil {
				config.Headers[k] = dara.StringValue(v)
			}
		}
	}

	// Add user agent
	config.Headers["user-agent"] = dara.StringValue(client.UserAgent)

	// Reuse authentication logic
	if dara.StringValue(params.AuthType) != "Anonymous" {
		if err := client.addAuthHeaders(config.Headers, params); err != nil {
			return nil, err
		}
	}

	// Create WebSocket client
	wsClient, err := dara.NewDefaultWebSocketClient(config, handler)
	if err != nil {
		return nil, err
	}

	// Connect
	ctx := context.Background()
	_, err = wsClient.Connect(ctx)
	if err != nil {
		return nil, err
	}

	return wsClient, nil
}

// buildWebSocketURL builds the WebSocket URL from params and request
func (client *Client) buildWebSocketURL(params *Params, request *OpenApiRequest) (string, error) {
	// Determine protocol
	protocol := "ws"
	if dara.StringValue(params.Protocol) == "https" || dara.StringValue(client.Protocol) == "https" {
		protocol = "wss"
	}

	// Get endpoint
	endpoint := dara.StringValue(client.Endpoint)

	// Build pathname
	pathname := dara.StringValue(params.Pathname)
	if pathname == "" {
		pathname = "/"
	}

	// Build query string
	queryString := ""
	if request.Query != nil && len(request.Query) > 0 {
		queryString = "?"
		first := true
		for k, v := range request.Query {
			if !first {
				queryString += "&"
			}
			queryString += k + "=" + dara.StringValue(v)
			first = false
		}
	}

	url := protocol + "://" + endpoint + pathname + queryString
	return url, nil
}

// addAuthHeaders adds authentication headers to the config
func (client *Client) addAuthHeaders(headers map[string]string, params *Params) error {
	if dara.IsNil(client.Credential) {
		return &ClientError{
			Code:    dara.String("InvalidCredentials"),
			Message: dara.String("Please set up the credentials correctly."),
		}
	}

	credentialModel, err := client.Credential.GetCredential()
	if err != nil {
		return err
	}

	if !dara.IsNil(credentialModel.ProviderName) {
		headers["x-acs-credentials-provider"] = dara.StringValue(credentialModel.ProviderName)
	}

	credentialType := dara.StringValue(credentialModel.Type)
	if credentialType == "bearer" {
		bearerToken := dara.StringValue(credentialModel.BearerToken)
		headers["x-acs-bearer-token"] = bearerToken
		headers["x-acs-signature-type"] = "BEARERTOKEN"
	} else if credentialType == "id_token" {
		idToken := dara.StringValue(credentialModel.SecurityToken)
		headers["x-acs-zero-trust-idtoken"] = idToken
	} else {
		accessKeyId := dara.StringValue(credentialModel.AccessKeyId)
		securityToken := dara.StringValue(credentialModel.SecurityToken)
		if securityToken != "" {
			headers["x-acs-accesskey-id"] = accessKeyId
			headers["x-acs-security-token"] = securityToken
		}
	}

	return nil
}
