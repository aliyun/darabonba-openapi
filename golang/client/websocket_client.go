// This file is auto-generated, don't edit it. Thanks.
package client

import (
	"context"
	"encoding/hex"
	"fmt"
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
// @param runtime - runtime options (reuses timeout, proxy, SSL configs)
//
//	Can be *dara.RuntimeOptions (type alias) or *dara.ExtendedRuntimeOptions
//
// @param handler - WebSocket message handler
// @return WebSocket client and error
func (client *Client) DoWebSocketRequest(params *Params, request *OpenApiRequest, runtime *dara.ExtendedRuntimeOptions, handler dara.WebSocketHandler) (*dara.DefaultWebSocketClient, error) {

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
	})

	var retryPolicyContext *dara.RetryPolicyContext
	var request_ *dara.Request
	var response_ *dara.Response
	retriesAttempted := int(0)
	retryPolicyContext = &dara.RetryPolicyContext{
		RetriesAttempted: retriesAttempted,
	}

	_backoffDelayTime := dara.GetBackoffDelay(_runtime.RetryOptions, retryPolicyContext)
	dara.Sleep(_backoffDelayTime)

	request_ = dara.NewRequest()
	request_.Protocol = dara.String("https")
	request_.Method = params.Method
	request_.Pathname = params.Pathname
	globalQueries := make(map[string]*string)
	globalHeaders := make(map[string]*string)
	if !dara.IsNil(client.GlobalParameters) {
		globalParams := client.GlobalParameters
		if !dara.IsNil(globalParams.Queries) {
			globalQueries = globalParams.Queries
		}

		if !dara.IsNil(globalParams.Headers) {
			globalHeaders = globalParams.Headers
		}

	}

	extendsHeaders := make(map[string]*string)
	extendsQueries := make(map[string]*string)

	request_.Query = dara.Merge(globalQueries,
		extendsQueries,
		request.Query)
	// endpoint is setted in product client
	request_.Headers = dara.Merge(map[string]*string{
		// "host":                  client.Endpoint,
		"x-acs-version":         params.Version,
		"x-acs-action":          params.Action,
		"user-agent":            openapiutil.GetUserAgent(client.UserAgent),
		"x-acs-date":            openapiutil.GetTimestamp(),
		"x-acs-signature-nonce": openapiutil.GetNonce(),
		"accept":                dara.String("application/json"),
	}, extendsHeaders,
		globalHeaders,
		request.Headers)

	signatureAlgorithm := dara.ToString(dara.Default(dara.StringValue(client.SignatureAlgorithm), "ACS3-HMAC-SHA256"))
	hashedRequestPayload := openapiutil.Hash(dara.BytesFromString("", "utf-8"), dara.String(signatureAlgorithm))

	if !dara.IsNil(request.Body) {
		if dara.StringValue(params.ReqBodyType) == "byte" {
			byteObj := []byte(dara.ToString(request.Body))
			hashedRequestPayload = openapiutil.Hash(byteObj, dara.String(signatureAlgorithm))
			request_.Body = dara.ToReader(byteObj)
		} else if dara.StringValue(params.ReqBodyType) == "json" {
			jsonObj := dara.Stringify(request.Body)
			hashedRequestPayload = openapiutil.Hash(dara.ToBytes(jsonObj, "utf8"), dara.String(signatureAlgorithm))
			request_.Body = dara.ToReader(dara.String(jsonObj))
			request_.Headers["content-type"] = dara.String("application/json; charset=utf-8")
		} else {
			m := dara.ToMap(request.Body)
			formObj := dara.StringValue(openapiutil.ToForm(m))
			hashedRequestPayload = openapiutil.Hash(dara.ToBytes(formObj, "utf8"), dara.String(signatureAlgorithm))
			request_.Body = dara.ToReader(dara.String(formObj))
			request_.Headers["content-type"] = dara.String("application/x-www-form-urlencoded")
		}
	}

	request_.Headers["x-acs-content-sha256"] = dara.String(hex.EncodeToString(hashedRequestPayload))
	if dara.StringValue(params.AuthType) != "Anonymous" {
		credentialModel, _err := client.Credential.GetCredential()
		if _err != nil {
			retriesAttempted++
			retryPolicyContext = &dara.RetryPolicyContext{
				RetriesAttempted: retriesAttempted,
				HttpRequest:      request_,
				HttpResponse:     response_,
				Exception:        _err,
			}
			return nil, _err
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

	url, err := client.buildWebSocketURL(params, request)
	if err != nil {
		return nil, err
	}
	// Debug: log the WebSocket URL being used
	fmt.Printf("[WebSocket] Connecting to: %s\n", url)

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
	// Ensure ConnectTimeout has a minimum value to avoid immediate timeout
	connectTimeout := dara.IntValue(getIntValue(dara.Default(runtime.ConnectTimeout, client.ConnectTimeout)))
	if connectTimeout <= 0 {
		connectTimeout = 10000 // Default to 10 seconds
	}
	readTimeout := dara.IntValue(getIntValue(dara.Default(runtime.ReadTimeout, client.ReadTimeout)))
	if readTimeout <= 0 {
		readTimeout = 60000 // Default to 60 seconds
	}
	config := &dara.WebSocketConfig{
		URL:            url,
		Headers:        make(map[string]string),
		ConnectTimeout: time.Duration(connectTimeout) * time.Millisecond,
		ReadTimeout:    time.Duration(readTimeout) * time.Millisecond,
		// Use helper functions to safely extract WebSocket config (works with both types)
		WriteTimeout:      time.Duration(dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketWriteTimeout(runtime), dara.Int(30000))))) * time.Millisecond,
		HandshakeTimeout:  time.Duration(dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketHandshakeTimeout(runtime), dara.Int(30000))))) * time.Millisecond,
		PingInterval:      time.Duration(dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketPingInterval(runtime), dara.Int(30000))))) * time.Millisecond,
		PongTimeout:       time.Duration(dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketPongTimeout(runtime), dara.Int(10000))))) * time.Millisecond,
		MaxMessageSize:    int64(dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketMaxMessageSize(runtime), dara.Int(1024*1024))))),
		EnableReconnect:   dara.BoolValue(getBoolValue(dara.Default(dara.GetWebSocketEnableReconnect(runtime), dara.Bool(true)))),
		ReconnectInterval: time.Duration(dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketReconnectInterval(runtime), dara.Int(5000))))) * time.Millisecond,
		MaxReconnectTimes: dara.IntValue(getIntValue(dara.Default(dara.GetWebSocketMaxReconnectTimes(runtime), dara.Int(5)))),
	}
	// Debug: log timeout configuration
	fmt.Printf("[WebSocket] ConnectTimeout: %v, ReadTimeout: %v\n", config.ConnectTimeout, config.ReadTimeout)

	if runtime.HttpProxy != nil {
		config.Headers["X-Http-Proxy"] = dara.StringValue(runtime.HttpProxy)
	}
	if runtime.HttpsProxy != nil {
		config.Headers["X-Https-Proxy"] = dara.StringValue(runtime.HttpsProxy)
	}

	if request.Headers != nil {
		for k, v := range request.Headers {
			if v != nil {
				config.Headers[k] = dara.StringValue(v)
			}
		}
	}

	if client.GlobalParameters != nil && client.GlobalParameters.Headers != nil {
		for k, v := range client.GlobalParameters.Headers {
			if v != nil {
				config.Headers[k] = dara.StringValue(v)
			}
		}
	}

	if runtime.ExtendsParameters != nil && runtime.ExtendsParameters.Headers != nil {
		for k, v := range runtime.ExtendsParameters.Headers {
			if v != nil {
				config.Headers[k] = dara.StringValue(v)
			}
		}
	}

	config.Headers["user-agent"] = dara.StringValue(client.UserAgent)

	// Merge request_.Headers (includes auth headers, signatures, etc.) into config.Headers
	// Filter out headers that should not be sent in WebSocket handshake:
	// - "host": gorilla/websocket automatically sets this from URL
	// - "content-type": WebSocket handshake doesn't have a body
	// - "accept": gorilla/websocket automatically sets this
	// - "connection", "upgrade", "sec-websocket-*": gorilla/websocket automatically sets these
	// Note: "X-Protocol" is preserved as it's needed for AWAP/General protocol identification
	excludedHeaders := map[string]bool{
		"host":                     true,
		"content-type":             true,
		"accept":                   true,
		"connection":               true,
		"upgrade":                  true,
		"sec-websocket-key":        true,
		"sec-websocket-version":    true,
		"sec-websocket-protocol":   true,
		"sec-websocket-extensions": true,
	}
	if request_ != nil && request_.Headers != nil {
		for k, v := range request_.Headers {
			if v != nil {
				// Convert to lowercase for case-insensitive comparison
				lowerKey := strings.ToLower(k)
				if !excludedHeaders[lowerKey] {
					// Preserve X-Protocol header (set by DoAwapWebSocketRequest or DoGeneralWebSocketRequest)
					// Don't override if it's already set in request.Headers
					if lowerKey == "x-protocol" && config.Headers["X-Protocol"] != "" {
						continue
					}
					config.Headers[k] = dara.StringValue(v)
				}
			}
		}
	}

	wsClient, err := dara.NewDefaultWebSocketClient(config, handler)
	if err != nil {
		return nil, err
	}

	ctx := context.Background()
	_, err = wsClient.Connect(ctx)
	if err != nil {
		return nil, err
	}

	return wsClient, nil
}

func (client *Client) buildWebSocketURL(params *Params, request *OpenApiRequest) (string, error) {
	// Determine protocol
	protocol := "ws"
	paramsProtocol := dara.StringValue(params.Protocol)
	clientProtocol := dara.StringValue(client.Protocol)
	if paramsProtocol == "wss" || paramsProtocol == "https" || clientProtocol == "https" {
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
