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
	"encoding/hex"
	"fmt"
	"time"

	openapiutil "github.com/alibabacloud-go/darabonba-openapi/v2/utils"

	"github.com/alibabacloud-go/tea/dara"
)

// @param params - request parameters
// @param request - request object
// @param runtime - runtime options (controls retry, timeout, proxy, SSL configs, and WebSocketHandler)
// @return result map containing wsClient and error
func (client *Client) DoWebSocketRequest(params *Params, request *OpenApiRequest, runtime *dara.RuntimeOptions) (_result map[string]interface{}, _err error) {
	// Validate runtime parameter
	if runtime == nil {
		_err = fmt.Errorf("runtime cannot be nil for WebSocket requests")
		return nil, _err
	}

	// Extract and validate WebSocket handler
	var handler dara.WebSocketHandler
	if runtimeHandler := dara.GetWebSocketHandler(runtime); runtimeHandler != nil {
		if wsHandler, ok := runtimeHandler.(dara.WebSocketHandler); ok {
			handler = wsHandler
		}
	}

	if handler == nil {
		_err = fmt.Errorf("WebSocketHandler is required: please set it in runtime.WebSocketHandler")
		return nil, _err
	}

	// Build runtime object with merged configurations
	_runtime := dara.NewRuntimeObject(map[string]interface{}{
		"key":                        dara.ToString(dara.Default(dara.StringValue(runtime.Key), dara.StringValue(client.Key))),
		"cert":                       dara.ToString(dara.Default(dara.StringValue(runtime.Cert), dara.StringValue(client.Cert))),
		"ca":                         dara.ToString(dara.Default(dara.StringValue(runtime.Ca), dara.StringValue(client.Ca))),
		"readTimeout":                dara.ForceInt(dara.Default(dara.IntValue(runtime.ReadTimeout), dara.IntValue(client.ReadTimeout))),
		"connectTimeout":             dara.ForceInt(dara.Default(dara.IntValue(runtime.ConnectTimeout), dara.IntValue(client.ConnectTimeout))),
		"httpProxy":                  dara.ToString(dara.Default(dara.StringValue(runtime.HttpProxy), dara.StringValue(client.HttpProxy))),
		"httpsProxy":                 dara.ToString(dara.Default(dara.StringValue(runtime.HttpsProxy), dara.StringValue(client.HttpsProxy))),
		"noProxy":                    dara.ToString(dara.Default(dara.StringValue(runtime.NoProxy), dara.StringValue(client.NoProxy))),
		"socks5Proxy":                dara.ToString(dara.Default(dara.StringValue(runtime.Socks5Proxy), dara.StringValue(client.Socks5Proxy))),
		"socks5NetWork":              dara.ToString(dara.Default(dara.StringValue(runtime.Socks5NetWork), dara.StringValue(client.Socks5NetWork))),
		"maxIdleConns":               dara.ForceInt(dara.Default(dara.IntValue(runtime.MaxIdleConns), dara.IntValue(client.MaxIdleConns))),
		"retryOptions":               client.RetryOptions,
		"ignoreSSL":                  dara.BoolValue(runtime.IgnoreSSL),
		"httpClient":                 client.HttpClient,
		"tlsMinVersion":              dara.StringValue(client.TlsMinVersion),
		"webSocketPingInterval":      dara.IntValue(dara.GetWebSocketPingInterval(runtime)),
		"webSocketPongTimeout":       dara.IntValue(dara.GetWebSocketPongTimeout(runtime)),
		"webSocketMaxMessageSize":    dara.IntValue(dara.GetWebSocketMaxMessageSize(runtime)),
		"webSocketEnableReconnect":   dara.BoolValue(dara.GetWebSocketEnableReconnect(runtime)),
		"webSocketReconnectInterval": dara.IntValue(dara.GetWebSocketReconnectInterval(runtime)),
		"webSocketMaxReconnectTimes": dara.IntValue(dara.GetWebSocketMaxReconnectTimes(runtime)),
		"webSocketWriteTimeout":      dara.IntValue(dara.GetWebSocketWriteTimeout(runtime)),
		"webSocketHandshakeTimeout":  dara.IntValue(dara.GetWebSocketHandshakeTimeout(runtime)),
		"webSocketEnableCompression": dara.BoolValue(dara.GetWebSocketEnableCompression(runtime)),
		"webSocketHandler":           handler,
	})

	// Initialize retry context
	var retryPolicyContext *dara.RetryPolicyContext
	var request_ *dara.Request
	var _resultErr error
	retriesAttempted := int(0)
	retryPolicyContext = &dara.RetryPolicyContext{
		RetriesAttempted: retriesAttempted,
	}

	_result = make(map[string]interface{})

	// Retry loop for WebSocket connection establishment
	for dara.ShouldRetry(_runtime.RetryOptions, retryPolicyContext) {
		_resultErr = nil
		_backoffDelayTime := dara.GetBackoffDelay(_runtime.RetryOptions, retryPolicyContext)
		dara.Sleep(_backoffDelayTime)

		// Build WebSocket request
		request_ = dara.NewRequest()
		protocol := dara.ToString(dara.Default(dara.StringValue(client.Protocol), dara.StringValue(params.Protocol)))
		if protocol == "http" {
			protocol = "ws"
		} else if protocol == "https" {
			protocol = "wss"
		}
		request_.Protocol = dara.String(protocol)
		request_.Method = params.Method
		request_.Pathname = params.Pathname

		// Merge queries
		globalQueries := make(map[string]*string)
		extendsQueries := make(map[string]*string)
		if !dara.IsNil(client.GlobalParameters) {
			globalParams := client.GlobalParameters
			if !dara.IsNil(globalParams.Queries) {
				globalQueries = globalParams.Queries
			}
		}

		if !dara.IsNil(runtime.ExtendsParameters) {
			extendsParameters := runtime.ExtendsParameters
			if !dara.IsNil(extendsParameters.Queries) {
				extendsQueries = extendsParameters.Queries
			}
		}

		request_.Query = dara.Merge(globalQueries, extendsQueries, request.Query)

		globalHeaders := make(map[string]*string)
		extendsHeaders := make(map[string]*string)
		if !dara.IsNil(client.GlobalParameters) {
			globalParams := client.GlobalParameters
			if !dara.IsNil(globalParams.Headers) {
				globalHeaders = globalParams.Headers
			}
		}

		if !dara.IsNil(runtime.ExtendsParameters) {
			extendsParameters := runtime.ExtendsParameters
			if !dara.IsNil(extendsParameters.Headers) {
				extendsHeaders = extendsParameters.Headers
			}
		}

		request_.Headers = dara.Merge(map[string]*string{
			"host":                  client.Endpoint,
			"x-acs-version":         params.Version,
			"x-acs-action":          params.Action,
			"user-agent":            openapiutil.GetUserAgent(client.UserAgent),
			"x-acs-date":            openapiutil.GetTimestamp(),
			"x-acs-signature-nonce": openapiutil.GetNonce(),
		}, globalHeaders, extendsHeaders, request.Headers)

		signatureAlgorithm := dara.ToString(dara.Default(dara.StringValue(client.SignatureAlgorithm), "ACS3-HMAC-SHA256"))
		hashedRequestPayload := openapiutil.Hash(dara.BytesFromString("", "utf-8"), dara.String(signatureAlgorithm))
		request_.Headers["x-acs-content-sha256"] = dara.String(hex.EncodeToString(hashedRequestPayload))

		if dara.StringValue(params.AuthType) != "Anonymous" {
			if dara.IsNil(client.Credential) {
				_err = fmt.Errorf("InvalidCredentials: Please set up the credentials correctly")
				retriesAttempted++
				retryPolicyContext = &dara.RetryPolicyContext{
					RetriesAttempted: retriesAttempted,
					HttpRequest:      request_,
					HttpResponse:     nil,
					Exception:        _err,
				}
				_resultErr = _err
				continue
			}

			credentialModel, _err := client.Credential.GetCredential()
			if _err != nil {
				retriesAttempted++
				retryPolicyContext = &dara.RetryPolicyContext{
					RetriesAttempted: retriesAttempted,
					HttpRequest:      request_,
					HttpResponse:     nil,
					Exception:        _err,
				}
				_resultErr = _err
				continue
			}

			if !dara.IsNil(credentialModel.ProviderName) {
				request_.Headers["x-acs-credentials-provider"] = credentialModel.ProviderName
			}

			authType := dara.StringValue(credentialModel.Type)
			if authType == "bearer" {
				bearerToken := dara.StringValue(credentialModel.BearerToken)
				request_.Headers["x-acs-bearer-token"] = dara.String(bearerToken)
			} else if authType == "id_token" {
				idToken := dara.StringValue(credentialModel.SecurityToken)
				request_.Headers["x-acs-zero-trust-idtoken"] = dara.String(idToken)
			} else {
				accessKeyId := dara.StringValue(credentialModel.AccessKeyId)
				accessKeySecret := dara.StringValue(credentialModel.AccessKeySecret)
				securityToken := dara.StringValue(credentialModel.SecurityToken)
				if !dara.IsNil(dara.String(securityToken)) && securityToken != "" {
					request_.Headers["x-acs-accesskey-id"] = dara.String(accessKeyId)
					request_.Headers["x-acs-security-token"] = dara.String(securityToken)
				}

				request_.Headers["Authorization"] = openapiutil.GetAuthorization(request_, dara.String(signatureAlgorithm), dara.String(hex.EncodeToString(hashedRequestPayload)), dara.String(accessKeyId), dara.String(accessKeySecret))
			}
		}

		wsClient, response, _err := dara.NewWebSocketClientAndConnect(request_, _runtime)
		if _err != nil {
			retriesAttempted++
			retryPolicyContext = &dara.RetryPolicyContext{
				RetriesAttempted: retriesAttempted,
				HttpRequest:      request_,
				HttpResponse:     nil,
				Exception:        _err,
			}
			_resultErr = _err
			continue
		}

		_result["wsClient"] = wsClient
		_result["response"] = response
		return _result, nil
	}

	if dara.BoolValue(client.DisableSDKError) != true {
		_resultErr = dara.TeaSDKError(_resultErr)
	}
	return _result, _resultErr
}

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
