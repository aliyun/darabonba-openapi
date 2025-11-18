// This file is auto-generated, don't edit it. Thanks.
package client

import (
	"context"
	"encoding/hex"
	"encoding/json"
	"fmt"
	"net/url"
	"strings"
	"time"

	openapiutil "github.com/alibabacloud-go/darabonba-openapi/v2/utils"

	"github.com/alibabacloud-go/tea/dara"
)

// DoWebSocketRequest establishes a WebSocket connection using runtime options
// This method reuses the same configuration pattern as DoRPCRequest/DoROARequest
//
// @param params - request parameters
// @param request - request object
// @param runtime - runtime options (reuses timeout, proxy, SSL configs, and WebSocketHandler)
// @return result map containing wsClient and error
func (client *Client) DoWebSocketRequest(params *Params, request *OpenApiRequest, runtime *dara.RuntimeOptions) (_result map[string]interface{}, _err error) {
	_result = make(map[string]interface{})

	var handler dara.WebSocketHandler
	if runtime != nil {
		if runtimeHandler := dara.GetWebSocketHandler(runtime); runtimeHandler != nil {
			if wsHandler, ok := runtimeHandler.(dara.WebSocketHandler); ok {
				handler = wsHandler
			}
		}
	}

	if handler == nil {
		_err = fmt.Errorf("WebSocketHandler is required: please set it in runtime.WebSocketHandler")
		return _result, _err
	}

	// _runtime := dara.NewRuntimeObject(map[string]interface{}{
	// 	"key":            dara.ToString(dara.Default(dara.StringValue(runtime.Key), dara.StringValue(client.Key))),
	// 	"cert":           dara.ToString(dara.Default(dara.StringValue(runtime.Cert), dara.StringValue(client.Cert))),
	// 	"ca":             dara.ToString(dara.Default(dara.StringValue(runtime.Ca), dara.StringValue(client.Ca))),
	// 	"readTimeout":    dara.ForceInt(dara.Default(dara.IntValue(runtime.ReadTimeout), dara.IntValue(client.ReadTimeout))),
	// 	"connectTimeout": dara.ForceInt(dara.Default(dara.IntValue(runtime.ConnectTimeout), dara.IntValue(client.ConnectTimeout))),
	// 	"httpProxy":      dara.ToString(dara.Default(dara.StringValue(runtime.HttpProxy), dara.StringValue(client.HttpProxy))),
	// 	"httpsProxy":     dara.ToString(dara.Default(dara.StringValue(runtime.HttpsProxy), dara.StringValue(client.HttpsProxy))),
	// 	"noProxy":        dara.ToString(dara.Default(dara.StringValue(runtime.NoProxy), dara.StringValue(client.NoProxy))),
	// 	"socks5Proxy":    dara.ToString(dara.Default(dara.StringValue(runtime.Socks5Proxy), dara.StringValue(client.Socks5Proxy))),
	// 	"socks5NetWork":  dara.ToString(dara.Default(dara.StringValue(runtime.Socks5NetWork), dara.StringValue(client.Socks5NetWork))),
	// 	"maxIdleConns":   dara.ForceInt(dara.Default(dara.IntValue(runtime.MaxIdleConns), dara.IntValue(client.MaxIdleConns))),
	// 	"retryOptions":   client.RetryOptions,
	// 	"ignoreSSL":      dara.BoolValue(runtime.IgnoreSSL),
	// 	"httpClient":     client.HttpClient,
	// 	"tlsMinVersion":  dara.StringValue(client.TlsMinVersion),
	// })

	// Create Request object (matching DoRequest pattern)
	request_ := dara.NewRequest()
	// Set protocol (convert http/https to ws/wss for WebSocket)
	protocol := dara.ToString(dara.Default(dara.StringValue(client.Protocol), dara.StringValue(params.Protocol)))
	if protocol == "http" {
		protocol = "ws"
	} else if protocol == "https" {
		protocol = "wss"
	}
	request_.Protocol = dara.String(protocol)
	request_.Method = params.Method
	request_.Pathname = params.Pathname
	globalQueries := make(map[string]*string)
	extendsQueries := make(map[string]*string)

	request_.Query = dara.Merge(globalQueries, extendsQueries, request.Query)
	// endpoint is setted in product client
	request_.Headers = map[string]*string{
		"host":                  client.Endpoint,
		"x-acs-version":         params.Version,
		"x-acs-action":          params.Action,
		"user-agent":            openapiutil.GetUserAgent(client.UserAgent),
		"x-acs-date":            openapiutil.GetTimestamp(),
		"x-acs-signature-nonce": openapiutil.GetNonce(),
	}

	signatureAlgorithm := dara.ToString(dara.Default(dara.StringValue(client.SignatureAlgorithm), "ACS3-HMAC-SHA256"))
	hashedRequestPayload := openapiutil.Hash(dara.BytesFromString("", "utf-8"), dara.String(signatureAlgorithm))

	request_.Headers["x-acs-content-sha256"] = dara.String(hex.EncodeToString(hashedRequestPayload))
	if dara.StringValue(params.AuthType) != "Anonymous" {
		credentialModel, _ := client.Credential.GetCredential()
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

	// Merge request headers from OpenApiRequest into dara.Request
	if request.Headers != nil {
		for k, v := range request.Headers {
			if v != nil {
				request_.Headers[k] = v
			}
		}
	}

	// Create RuntimeObject from RuntimeOptions (matching DoRequest/DoRPCRequest pattern)
	_runtime := dara.NewRuntimeObject(map[string]interface{}{
		"key":            dara.ToString(dara.Default(dara.StringValue(runtime.Key), dara.StringValue(client.Key))),
		"cert":           dara.ToString(dara.Default(dara.StringValue(runtime.Cert), dara.StringValue(client.Cert))),
		"ca":             dara.ToString(dara.Default(dara.StringValue(runtime.Ca), dara.StringValue(client.Ca))),
		"readTimeout":    dara.ForceInt(dara.Default(dara.IntValue(runtime.ReadTimeout), dara.IntValue(client.ReadTimeout))),
		"connectTimeout": dara.ForceInt(dara.Default(dara.IntValue(runtime.ConnectTimeout), dara.IntValue(client.ConnectTimeout))),
		"httpProxy":      dara.ToString(dara.Default(dara.StringValue(runtime.HttpProxy), dara.StringValue(client.HttpProxy))),
		"httpsProxy":     dara.ToString(dara.Default(dara.StringValue(runtime.HttpsProxy), dara.StringValue(client.HttpsProxy))),
		"noProxy":        dara.ToString(dara.Default(dara.StringValue(runtime.NoProxy), dara.StringValue(client.NoProxy))),
		"socks5Proxy":    dara.ToString(dara.Default(dara.StringValue(runtime.Socks5Proxy), dara.StringValue(client.Socks5Proxy))),
		"socks5NetWork":  dara.ToString(dara.Default(dara.StringValue(runtime.Socks5NetWork), dara.StringValue(client.Socks5NetWork))),
		"maxIdleConns":   dara.ForceInt(dara.Default(dara.IntValue(runtime.MaxIdleConns), dara.IntValue(client.MaxIdleConns))),
		"retryOptions":   client.RetryOptions,
		"ignoreSSL":      dara.BoolValue(runtime.IgnoreSSL),
		"httpClient":     client.HttpClient,
		"tlsMinVersion":  dara.StringValue(client.TlsMinVersion),
		// WebSocket-specific configuration
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

	// Create WebSocket client (matching DoRequest pattern: no config, just handler)
	wsClient, err := dara.NewDefaultWebSocketClient(handler)
	if err != nil {
		_err = err
		return _result, _err
	}

	// Connect using Request and RuntimeObject (matching DoRequest pattern)
	ctx := context.Background()
	_, err = wsClient.Connect(ctx, request_, _runtime)
	if err != nil {
		_err = err
		return _result, _err
	}

	_result["wsClient"] = wsClient
	return _result, _err
}

func (client *Client) DoWebSocketRequestBK(params *Params, request *OpenApiRequest, runtime *dara.RuntimeOptions) (_result map[string]interface{}, _err error) {
	_result = make(map[string]interface{})

	var handler dara.WebSocketHandler
	if runtime != nil {
		if runtimeHandler := dara.GetWebSocketHandler(runtime); runtimeHandler != nil {
			if wsHandler, ok := runtimeHandler.(dara.WebSocketHandler); ok {
				handler = wsHandler
			}
		}
	}

	if handler == nil {
		_err = fmt.Errorf("WebSocketHandler is required: please set it in runtime.WebSocketHandler")
		return _result, _err
	}

	// Create Request object (matching DoRequest pattern)
	request_ := dara.NewRequest()
	// Set protocol (convert http/https to ws/wss for WebSocket)
	protocol := dara.ToString(dara.Default(dara.StringValue(client.Protocol), dara.StringValue(params.Protocol)))
	if protocol == "http" {
		protocol = "ws"
	} else if protocol == "https" {
		protocol = "wss"
	}
	request_.Protocol = dara.String(protocol)
	request_.Method = params.Method
	request_.Pathname = params.Pathname
	globalQueries := make(map[string]*string)
	extendsQueries := make(map[string]*string)

	request_.Query = dara.Merge(globalQueries, extendsQueries, request.Query)
	// endpoint is setted in product client
	request_.Headers = map[string]*string{
		"host":                  client.Endpoint,
		"x-acs-version":         params.Version,
		"x-acs-action":          params.Action,
		"user-agent":            openapiutil.GetUserAgent(client.UserAgent),
		"x-acs-date":            openapiutil.GetTimestamp(),
		"x-acs-signature-nonce": openapiutil.GetNonce(),
	}

	signatureAlgorithm := dara.ToString(dara.Default(dara.StringValue(client.SignatureAlgorithm), "ACS3-HMAC-SHA256"))
	hashedRequestPayload := openapiutil.Hash(dara.BytesFromString("", "utf-8"), dara.String(signatureAlgorithm))

	request_.Headers["x-acs-content-sha256"] = dara.String(hex.EncodeToString(hashedRequestPayload))
	if dara.StringValue(params.AuthType) != "Anonymous" {
		credentialModel, _ := client.Credential.GetCredential()
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

	// Merge request headers from OpenApiRequest into dara.Request
	if request.Headers != nil {
		for k, v := range request.Headers {
			if v != nil {
				request_.Headers[k] = v
			}
		}
	}

	// Create RuntimeObject from RuntimeOptions (matching DoRequest/DoRPCRequest pattern)
	_runtime := dara.NewRuntimeObject(map[string]interface{}{
		"key":            dara.ToString(dara.Default(dara.StringValue(runtime.Key), dara.StringValue(client.Key))),
		"cert":           dara.ToString(dara.Default(dara.StringValue(runtime.Cert), dara.StringValue(client.Cert))),
		"ca":             dara.ToString(dara.Default(dara.StringValue(runtime.Ca), dara.StringValue(client.Ca))),
		"readTimeout":    dara.ForceInt(dara.Default(dara.IntValue(runtime.ReadTimeout), dara.IntValue(client.ReadTimeout))),
		"connectTimeout": dara.ForceInt(dara.Default(dara.IntValue(runtime.ConnectTimeout), dara.IntValue(client.ConnectTimeout))),
		"httpProxy":      dara.ToString(dara.Default(dara.StringValue(runtime.HttpProxy), dara.StringValue(client.HttpProxy))),
		"httpsProxy":     dara.ToString(dara.Default(dara.StringValue(runtime.HttpsProxy), dara.StringValue(client.HttpsProxy))),
		"noProxy":        dara.ToString(dara.Default(dara.StringValue(runtime.NoProxy), dara.StringValue(client.NoProxy))),
		"socks5Proxy":    dara.ToString(dara.Default(dara.StringValue(runtime.Socks5Proxy), dara.StringValue(client.Socks5Proxy))),
		"socks5NetWork":  dara.ToString(dara.Default(dara.StringValue(runtime.Socks5NetWork), dara.StringValue(client.Socks5NetWork))),
		"maxIdleConns":   dara.ForceInt(dara.Default(dara.IntValue(runtime.MaxIdleConns), dara.IntValue(client.MaxIdleConns))),
		"retryOptions":   client.RetryOptions,
		"ignoreSSL":      dara.BoolValue(runtime.IgnoreSSL),
		"httpClient":     client.HttpClient,
		"tlsMinVersion":  dara.StringValue(client.TlsMinVersion),
		// WebSocket-specific configuration
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

	// Create WebSocket client (matching DoRequest pattern: no config, just handler)
	wsClient, err := dara.NewDefaultWebSocketClient(handler)
	if err != nil {
		_err = err
		return _result, _err
	}

	// Connect using Request and RuntimeObject (matching DoRequest pattern)
	ctx := context.Background()
	_, err = wsClient.Connect(ctx, request_, _runtime)
	if err != nil {
		_err = err
		return _result, _err
	}

	_result["wsClient"] = wsClient
	return _result, _err
}

func (client *Client) buildWebSocketURL(params *Params, request *OpenApiRequest) (string, error) {
	protocol := "ws"
	paramsProtocol := dara.StringValue(params.Protocol)
	clientProtocol := dara.StringValue(client.Protocol)
	if paramsProtocol == "wss" || paramsProtocol == "https" || clientProtocol == "https" {
		protocol = "wss"
	}

	endpoint := dara.StringValue(client.Endpoint)

	pathname := dara.StringValue(params.Pathname)
	if pathname == "" {
		pathname = "/"
	}

	queryString := ""
	if request.Query != nil && len(request.Query) > 0 {
		queryValues := url.Values{}
		for k, v := range request.Query {
			if v != nil {
				queryValues.Add(k, dara.StringValue(v))
			}
		}
		if len(queryValues) > 0 {
			queryString = "?" + queryValues.Encode()
		}
	}

	url := protocol + "://" + endpoint + pathname + queryString
	return url, nil
}

// ============================================================================
// AWAP Protocol Methods
// ============================================================================

// SendAwapMessage sends an AWAP protocol message
// AWAP protocol uses frame format: text headers + JSON payload
// Format: "type:request\nseq:1\ntimestamp:1234567890\nid:msg-001\nack:required\n\n{JSON payload}"
//
// @param wsClient - WebSocket client
// @param message - AWAP message to send
// @return error
func (client *Client) SendAwapMessage(wsClient dara.WebSocketClient, message *dara.AwapMessage) error {
	var headerBuilder strings.Builder

	headerBuilder.WriteString(fmt.Sprintf("type:%s\n", string(message.Type)))

	headerBuilder.WriteString(fmt.Sprintf("seq:%d\n", message.Seq))

	timestamp := time.Now().UnixMilli()
	headerBuilder.WriteString(fmt.Sprintf("timestamp:%d\n", timestamp))

	if message.ID != "" {
		headerBuilder.WriteString(fmt.Sprintf("id:%s\n", message.ID))
	}

	if message.Type == dara.AwapMessageTypeAckRequiredTextEvent {
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

	fmt.Printf("[SDK AWAP] Sending frame:\n%s\n", frameData)

	ctx := context.Background()
	return wsClient.SendText(ctx, frameData)
}

// BuildAwapMessage builds an AWAP message with the specified type, ID, sequence, and payload
//
// @param msgType - message type
// @param id - message ID
// @param seq - sequence number
// @param payload - message payload
// @return AWAP message
func (client *Client) BuildAwapMessage(msgType dara.AwapMessageType, id string, seq int64, payload interface{}) *dara.AwapMessage {
	return &dara.AwapMessage{
		Type:    msgType,
		ID:      id,
		Seq:     seq,
		Payload: payload,
		Headers: make(map[string]string),
	}
}

// BuildAwapRequest builds an AWAP request message
//
// @param id - message ID
// @param seq - sequence number
// @param payload - message payload
// @return AWAP message
func (client *Client) BuildAwapRequest(id string, seq int64, payload interface{}) *dara.AwapMessage {
	// Use UpstreamTextEvent for text requests (as expected by server)
	return client.BuildAwapMessage(dara.AwapMessageTypeUpstreamTextEvent, id, seq, payload)
}

// BuildAwapEvent builds an AWAP event message
//
// @param id - message ID
// @param seq - sequence number
// @param payload - event payload
// @return AWAP message
func (client *Client) BuildAwapEvent(id string, seq int64, payload interface{}) *dara.AwapMessage {
	// Use UpstreamTextEvent for text events (as expected by server)
	return client.BuildAwapMessage(dara.AwapMessageTypeUpstreamTextEvent, id, seq, payload)
}

// SendAwapRequest sends an AWAP request message
//
// @param wsClient - WebSocket client
// @param id - message ID
// @param seq - sequence number
// @param payload - message payload
// @return error
func (client *Client) SendAwapRequest(wsClient dara.WebSocketClient, id string, seq int64, payload interface{}) error {
	message := client.BuildAwapRequest(id, seq, payload)
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
	message := client.BuildAwapEvent(id, seq, payload)
	return client.SendAwapMessage(wsClient, message)
}

// ============================================================================
// General Protocol Methods
// ============================================================================

// SendGeneralMessage sends a General protocol message
//
// @param wsClient - WebSocket client
// @param message - General message to send
// @return error
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
//
// @param headers - message headers
// @param body - message body
// @return General message
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
//
// @param body - text body
// @return General message
func (client *Client) BuildGeneralTextMessage(body string) *dara.GeneralMessage {
	return client.BuildGeneralMessage(map[string]string{
		"Content-Type": "text/plain",
		"type":         string(dara.GeneralMessageTypeUpstreamDefaultTextEvent),
	}, body)
}

// BuildGeneralJSONMessage builds a General protocol JSON message
//
// @param body - JSON body
// @return General message
func (client *Client) BuildGeneralJSONMessage(body interface{}) *dara.GeneralMessage {
	return client.BuildGeneralMessage(map[string]string{
		"Content-Type": "application/json",
		"type":         string(dara.GeneralMessageTypeUpstreamDefaultTextEvent),
	}, body)
}

// BuildGeneralUpstreamTextMessage builds a General upstream text message with explicit type
//
// @param body - message body
// @return General message
func (client *Client) BuildGeneralUpstreamTextMessage(body interface{}) *dara.GeneralMessage {
	return client.BuildGeneralMessage(map[string]string{
		"Content-Type": "application/json",
		"type":         string(dara.GeneralMessageTypeUpstreamDefaultTextEvent),
	}, body)
}

// BuildGeneralDownstreamTextMessage builds a General downstream text message with explicit type
//
// @param body - message body
// @return General message
func (client *Client) BuildGeneralDownstreamTextMessage(body interface{}) *dara.GeneralMessage {
	return client.BuildGeneralMessage(map[string]string{
		"Content-Type": "application/json",
		"type":         string(dara.GeneralMessageTypeDownstreamDefaultTextEvent),
	}, body)
}

// SendGeneralTextMessage sends a General protocol text message
//
// @param wsClient - WebSocket client
// @param text - text content
// @return error
func (client *Client) SendGeneralTextMessage(wsClient dara.WebSocketClient, text string) error {
	message := client.BuildGeneralTextMessage(text)
	return client.SendGeneralMessage(wsClient, message)
}

// SendGeneralJSONMessage sends a General protocol JSON message
//
// @param wsClient - WebSocket client
// @param body - JSON body
// @return error
func (client *Client) SendGeneralJSONMessage(wsClient dara.WebSocketClient, body interface{}) error {
	message := client.BuildGeneralJSONMessage(body)
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
