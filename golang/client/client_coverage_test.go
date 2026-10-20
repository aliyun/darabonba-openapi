package client

import (
	"context"
	"encoding/json"
	"io"
	"net/http"
	"os"
	"strings"
	"testing"

	pop "github.com/alibabacloud-go/alibabacloud-gateway-pop/client"
	spi "github.com/alibabacloud-go/alibabacloud-gateway-spi/client"
	openapiutil "github.com/alibabacloud-go/darabonba-openapi/v2/utils"
	websocketutils "github.com/alibabacloud-go/darabonba-openapi/v2/websocketUtils"
	util "github.com/alibabacloud-go/tea-utils/v2/service"
	"github.com/alibabacloud-go/tea/dara"
	"github.com/alibabacloud-go/tea/tea"
	tea_util "github.com/alibabacloud-go/tea/utils"
	credential "github.com/aliyun/credentials-go/credentials"
)

type extraMockHandler struct {
	content string
}

func (mock *extraMockHandler) ServeHTTP(w http.ResponseWriter, req *http.Request) {
	w.Header().Set("raw-query", req.URL.RawQuery)
	body, _ := util.ReadAsString(req.Body)
	w.Header().Set("raw-body", tea.StringValue(body))
	w.Header().Set("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")
	switch mock.content {
	case "xml":
		w.Header().Set("content-type", "text/xml;charset=utf-8")
		w.WriteHeader(400)
		w.Write([]byte(`<?xml version="1.0"?><Error><Code>InvalidAccessKeyId</Code><Message>xml error</Message><RequestId>xml-request-id</RequestId></Error>`))
	case "empty204":
		w.WriteHeader(204)
	case "serverError":
		w.WriteHeader(500)
		w.Write([]byte(`{"Code":"InternalError","Message":"server boom","RequestId":"A45EE076-334D-5012-9746-A8F828D20FD4","Description":"desc","Detail":"detail"}`))
	case "error":
		w.WriteHeader(400)
		w.Write([]byte(`{"Code":"error code","Message":"error message","RequestId":"A45EE076-334D-5012-9746-A8F828D20FD4","Description":"error description","AccessDeniedDetail":{"k":"v"}}`))
	case "array":
		w.WriteHeader(200)
		w.Write([]byte(`["AppId","ClassId","UserId"]`))
	default:
		w.WriteHeader(200)
		w.Write([]byte(`{"AppId":"test","ClassId":"test","UserId":123}`))
	}
}

func newCoverageClient(t *testing.T, endpoint string, mutators ...func(*Config)) *Client {
	config := CreateConfig()
	config.Protocol = tea.String("HTTP")
	config.Endpoint = tea.String(endpoint)
	for _, mutator := range mutators {
		mutator(config)
	}
	client, err := NewClient(config)
	tea_util.AssertNil(t, err)
	return client
}

func statusCodeOf(result map[string]interface{}) int {
	switch v := result["statusCode"].(type) {
	case json.Number:
		n, _ := v.Int64()
		return int(n)
	case int:
		return v
	case int64:
		return int(v)
	case int32:
		return int(v)
	default:
		return -1
	}
}

func coverageParams(style, reqBodyType, bodyType, pathname, authType string) *Params {
	return &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String(pathname),
		Method:      tea.String("POST"),
		AuthType:    tea.String(authType),
		Style:       tea.String(style),
		ReqBodyType: tea.String(reqBodyType),
		BodyType:    tea.String(bodyType),
	}
}

func TestCallApiWithCtx_NilParams(t *testing.T) {
	client := newCoverageClient(t, "127.0.0.1:1")
	_, err := client.CallApiWithCtx(context.Background(), nil, CreateOpenApiRequest(), CreateRuntimeOptions())
	tea_util.AssertNotNil(t, err)
	clientErr := err.(*ClientError)
	tea_util.AssertEqual(t, "ParameterMissing", tea.StringValue(clientErr.GetCode()))
}

func TestCallApi_NilParams(t *testing.T) {
	client := newCoverageClient(t, "127.0.0.1:1")
	_, err := client.CallApi(nil, CreateOpenApiRequest(), CreateRuntimeOptions())
	tea_util.AssertNotNil(t, err)
	clientErr := err.(*ClientError)
	tea_util.AssertEqual(t, "ParameterMissing", tea.StringValue(clientErr.GetCode()))
}

func TestCallApiWithCtx_RPCV2AndV3AndROA(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/", &mockHandler{content: "json"})
	mux.Handle("/test", &mockHandler{content: "json"})
	server, endpoint := startMockHTTPServer(mux)
	defer server.Close()
	runtime := CreateRuntimeOptions()
	request := CreateOpenApiRequest()
	ctx := context.Background()

	client := newCoverageClient(t, endpoint, func(c *Config) {
		c.SignatureAlgorithm = tea.String("v2")
	})
	result, err := client.CallApiWithCtx(ctx, coverageParams("RPC", "formData", "json", "/", "AK"), request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	result, err = client.CallApiWithCtx(ctx, coverageParams("ROA", "json", "json", "/test", "Anonymous"), request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	result, err = client.CallApiWithCtx(ctx, coverageParams("ROA", "formData", "json", "/test", "Anonymous"), request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	client = newCoverageClient(t, endpoint)
	result, err = client.CallApiWithCtx(ctx, coverageParams("RPC", "formData", "json", "/", "AK"), request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	bearer := CreateBearerTokenConfig()
	bearer.Protocol = tea.String("HTTP")
	bearer.Endpoint = tea.String(endpoint)
	bearerClient, err := NewClient(bearer)
	tea_util.AssertNil(t, err)
	result, err = bearerClient.CallApiWithCtx(ctx, coverageParams("RPC", "formData", "json", "/", "AK"), request, runtime)
	tea_util.AssertNil(t, err)
	headers, err := util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "token", headers["x-acs-bearer-token"])
}

func TestCallApiWithCtx_ExecuteV4(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	server, endpoint := startMockHTTPServer(mux)
	defer server.Close()
	client := newCoverageClient(t, endpoint, func(c *Config) {
		c.SignatureVersion = tea.String("v4")
	})
	client.ProductId = tea.String("test")
	gatewayClient, err := pop.NewClient()
	tea_util.AssertNil(t, err)
	err = client.SetGatewayClient(gatewayClient)
	tea_util.AssertNil(t, err)

	params := coverageParams("RPC", "json", "json", "/test", "Anonymous")
	result, err := client.CallApi(params, CreateOpenApiRequest(), CreateRuntimeOptions())
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, 200, statusCodeOf(result))

	result, err = client.CallApiWithCtx(context.Background(), params, CreateOpenApiRequest(), CreateRuntimeOptions())
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, 200, statusCodeOf(result))

	result, err = client.ExecuteWithCtx(context.Background(), params, CreateOpenApiRequest(), CreateRuntimeOptions())
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, 200, statusCodeOf(result))
}

func TestCallApiWithCtx_ResponseBodyTypes(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	mux.Handle("/testArray", &mockHandler{content: "array"})
	mux.Handle("/testError", &mockHandler{content: "error"})
	mux.Handle("/testError2", &mockHandler{content: "error2"})
	mux.Handle("/serverError", &mockHandler{content: "serverError"})
	server, endpoint := startMockHTTPServer(mux)
	defer server.Close()
	client := newCoverageClient(t, endpoint)
	runtime := CreateRuntimeOptions()
	request := CreateOpenApiRequest()
	ctx := context.Background()

	params := coverageParams("ROA", "formData", "json", "/test", "AK")
	result, err := client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)
	body, err := util.AssertAsMap(result["body"])
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "test", body["AppId"])

	params.BodyType = tea.String("array")
	params.Pathname = tea.String("/testArray")
	result, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)
	bodyArray, err := util.AssertAsArray(result["body"])
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "AppId", bodyArray[0])

	params.BodyType = tea.String("string")
	params.Pathname = tea.String("/test")
	result, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)
	bodyStr, err := util.AssertAsString(result["body"])
	tea_util.AssertNil(t, err)
	tea_util.AssertNotNil(t, bodyStr)

	params.BodyType = tea.String("byte")
	result, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertNotNil(t, result["body"])

	params.BodyType = tea.String("binary")
	result, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)
	_, err = util.ReadAsString(result["body"].(io.Reader))
	tea_util.AssertNil(t, err)

	params.BodyType = tea.String("none")
	result, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	params.Pathname = tea.String("/testError")
	_, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNotNil(t, err)

	params.Pathname = tea.String("/testError2")
	_, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNotNil(t, err)

	client.DisableSDKError = tea.Bool(true)
	params.Pathname = tea.String("/serverError")
	_, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNotNil(t, err)
	serverErr := err.(*ServerError)
	tea_util.AssertEqual(t, 500, tea.IntValue(serverErr.GetStatusCode()))
}

func TestCallApiWithCtx_RequestBodyTypes(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	server, endpoint := startMockHTTPServer(mux)
	defer server.Close()
	client := newCoverageClient(t, endpoint)
	runtime := CreateRuntimeOptions()
	params := coverageParams("RPC", "formData", "json", "/test", "AK")
	ctx := context.Background()
	body := map[string]interface{}{
		"key1": tea.String("value"),
		"key2": tea.Int(1),
		"key3": tea.Bool(true),
	}

	result, err := client.CallApiWithCtx(ctx, params, &OpenApiRequest{Body: openapiutil.ParseToMap(body)}, runtime)
	tea_util.AssertNil(t, err)
	headers, err := util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "application/x-www-form-urlencoded", headers["content-type"])

	params.ReqBodyType = tea.String("json")
	result, err = client.CallApiWithCtx(ctx, params, &OpenApiRequest{Body: openapiutil.ParseToMap(body)}, runtime)
	tea_util.AssertNil(t, err)
	headers, err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "application/json; charset=utf-8", headers["content-type"])

	params.ReqBodyType = tea.String("byte")
	result, err = client.CallApiWithCtx(ctx, params, &OpenApiRequest{Body: util.ToBytes(tea.String("test byte"))}, runtime)
	tea_util.AssertNil(t, err)
	headers, err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "test byte", headers["raw-body"])

	params.ReqBodyType = tea.String("binary")
	result, err = client.CallApiWithCtx(ctx, params, &OpenApiRequest{Stream: strings.NewReader("stream-body")}, runtime)
	tea_util.AssertNil(t, err)
	headers, err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "stream-body", headers["raw-body"])
}

func TestDoRequestWithCtx_AndROAFormDirect(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	mux.Handle("/", &mockHandler{content: "json"})
	server, endpoint := startMockHTTPServer(mux)
	defer server.Close()
	client := newCoverageClient(t, endpoint)
	runtime := CreateRuntimeOptions()
	request := CreateOpenApiRequest()
	ctx := context.Background()

	result, err := client.DoRPCRequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String("json"), request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	result, err = client.DoROARequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String("/test"), tea.String("json"), request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	result, err = client.DoROAFormRequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String("/test"), tea.String("json"), request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	result, err = client.DoRequestWithCtx(ctx, coverageParams("RPC", "json", "json", "/test", "Anonymous"), request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())
}

func TestROA204AndXMLErrorAndByteBody(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/empty", &extraMockHandler{content: "empty204"})
	mux.Handle("/xml", &extraMockHandler{content: "xml"})
	mux.Handle("/json", &extraMockHandler{content: "json"})
	server, endpoint := startMockHTTPServer(mux)
	defer server.Close()
	client := newCoverageClient(t, endpoint, func(c *Config) {
		c.SignatureAlgorithm = tea.String("v2")
	})
	runtime := CreateRuntimeOptions()
	request := CreateOpenApiRequest()

	params := coverageParams("ROA", "json", "json", "/empty", "Anonymous")
	result, err := client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertNotNil(t, result["headers"])

	result, err = client.CallApiWithCtx(context.Background(), params, request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertNotNil(t, result["headers"])

	params = coverageParams("ROA", "formData", "json", "/empty", "Anonymous")
	result, err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, err)
	result, err = client.CallApiWithCtx(context.Background(), params, request, runtime)
	tea_util.AssertNil(t, err)

	acsClient := newCoverageClient(t, endpoint)
	xmlParams := coverageParams("RPC", "json", "json", "/xml", "Anonymous")
	_, err = acsClient.CallApi(xmlParams, request, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = acsClient.CallApiWithCtx(context.Background(), xmlParams, request, runtime)
	tea_util.AssertNotNil(t, err)

	byteParams := coverageParams("RPC", "json", "byte", "/json", "Anonymous")
	result, err = acsClient.CallApi(byteParams, request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertNotNil(t, result["body"])
	result, err = acsClient.CallApiWithCtx(context.Background(), byteParams, request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertNotNil(t, result["body"])

	noneParams := coverageParams("RPC", "json", "none", "/json", "Anonymous")
	result, err = acsClient.CallApi(noneParams, request, runtime)
	tea_util.AssertNil(t, err)
	result, err = acsClient.CallApiWithCtx(context.Background(), noneParams, request, runtime)
	tea_util.AssertNil(t, err)
}

func TestCallSSEApiWithCtxAndError(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/sse", &mockHandler{content: "json"})
	mux.Handle("/sse-error", &extraMockHandler{content: "error"})
	server, endpoint := startMockHTTPServer(mux)
	defer server.Close()
	client := newCoverageClient(t, endpoint)
	runtime := CreateRuntimeOptions()
	request := CreateOpenApiRequest()
	params := coverageParams("ROA", "formData", "json", "/sse", "AK")
	params.Method = tea.String("GET")

	events := make([]*dara.SSEEvent, 0)
	result := make(chan *SSEResponse, 1)
	yieldErr := make(chan error, 1)
	go client.CallSSEApiWithCtx(context.Background(), params, request, runtime, result, yieldErr)
	for resp := range result {
		events = append(events, resp.Event)
	}
	err := <-yieldErr
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, 5, len(events))

	params.Pathname = tea.String("/sse-error")
	errorResult := make(chan *SSEResponse, 1)
	errorYield := make(chan error, 1)
	go client.CallSSEApi(params, request, runtime, errorResult, errorYield)
	for range errorResult {
	}
	err = <-errorYield
	tea_util.AssertNotNil(t, err)

	errorResult = make(chan *SSEResponse, 1)
	errorYield = make(chan error, 1)
	go client.CallSSEApiWithCtx(context.Background(), params, request, runtime, errorResult, errorYield)
	for range errorResult {
	}
	err = <-errorYield
	tea_util.AssertNotNil(t, err)
}

func TestWebSocketDoRequestValidation(t *testing.T) {
	client := newCoverageClient(t, "example.com")
	runtime := CreateRuntimeOptions()
	request := &OpenApiRequest{}
	params := &Params{
		Action:      tea.String("TestAction"),
		Version:     tea.String("2020-01-01"),
		Protocol:    tea.String("ws"),
		Pathname:    tea.String("/ws"),
		Method:      tea.String("GET"),
		AuthType:    tea.String("AK"),
		BodyType:    tea.String("string"),
		ReqBodyType: tea.String("json"),
	}

	_, err := client.DoRequest(params, request, runtime)
	tea_util.AssertNotNil(t, err)
	tea_util.AssertEqual(t, true, strings.Contains(err.Error(), "websocketSubProtocol is required"))
	_, err = client.DoRequestWithCtx(context.Background(), params, request, runtime)
	tea_util.AssertNotNil(t, err)
	tea_util.AssertEqual(t, true, strings.Contains(err.Error(), "websocketSubProtocol is required"))

	params.WebsocketSubProtocol = tea.String("invalid")
	runtime.WebSocketHandler = &dara.AbstractWebSocketHandler{}
	_, err = client.DoRequest(params, request, runtime)
	tea_util.AssertNotNil(t, err)
	tea_util.AssertEqual(t, true, strings.Contains(err.Error(), "websocketSubProtocol must be"))
	_, err = client.DoRequestWithCtx(context.Background(), params, request, runtime)
	tea_util.AssertNotNil(t, err)

	params.WebsocketSubProtocol = tea.String(websocketutils.SubProtocolAWAP)
	runtime.WebSocketHandler = nil
	_, err = client.DoRequest(params, request, runtime)
	tea_util.AssertNotNil(t, err)
	tea_util.AssertEqual(t, true, strings.Contains(err.Error(), "WebSocketHandler is required"))
	_, err = client.DoRequestWithCtx(context.Background(), params, request, runtime)
	tea_util.AssertNotNil(t, err)
}

func TestCredentialHelpersAndAccessDeniedDetail(t *testing.T) {
	client, err := NewClient(&Config{Endpoint: tea.String("example.com")})
	tea_util.AssertNil(t, err)
	ak, err := client.GetAccessKeyId()
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "", tea.StringValue(ak))
	sk, err := client.GetAccessKeySecret()
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "", tea.StringValue(sk))
	token, err := client.GetSecurityToken()
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "", tea.StringValue(token))
	bearer, err := client.GetBearerToken()
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "", tea.StringValue(bearer))
	typ, err := client.GetType()
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "", tea.StringValue(typ))

	detail := client.GetAccessDeniedDetail(map[string]interface{}{
		"AccessDeniedDetail": map[string]interface{}{"k": "v"},
	})
	tea_util.AssertEqual(t, "v", detail["k"])
	detail = client.GetAccessDeniedDetail(map[string]interface{}{
		"accessDeniedDetail": map[string]interface{}{"k": "v2"},
	})
	tea_util.AssertEqual(t, "v2", detail["k"])
	detail = client.GetAccessDeniedDetail(map[string]interface{}{})
	tea_util.AssertNil(t, detail)

	os.Setenv("ALIBABA_CLOUD_USER_AGENT", "from-env")
	defer os.Unsetenv("ALIBABA_CLOUD_USER_AGENT")
	envClient, err := NewClient(&Config{
		AccessKeyId:     tea.String("ak"),
		AccessKeySecret: tea.String("secret"),
		Endpoint:        tea.String("example.com"),
	})
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "from-env", tea.StringValue(envClient.UserAgent))
}

func TestErrorTypesAndSSEResponse(t *testing.T) {
	clientErr := &ClientError{
		StatusCode:         tea.Int(400),
		Code:               tea.String("Client.Error"),
		Detail:             tea.String("detail"),
		Description:        tea.String("desc"),
		RequestId:          tea.String("rid"),
		RetryAfter:         tea.Int64(10),
		Data:               map[string]interface{}{"a": 1},
		Name:               tea.String("ClientError"),
		Stack:              tea.String("stack"),
		AccessDeniedDetail: map[string]interface{}{"k": "v"},
	}
	tea_util.AssertEqual(t, true, strings.Contains(clientErr.Error(), "ClientError"))
	clientErr.Message = tea.String("client-msg")
	tea_util.AssertEqual(t, "client-msg", tea.StringValue(clientErr.GetMessage()))
	tea_util.AssertEqual(t, 400, tea.IntValue(clientErr.GetStatusCode()))
	tea_util.AssertEqual(t, "Client.Error", tea.StringValue(clientErr.GetCode()))
	tea_util.AssertEqual(t, "detail", tea.StringValue(clientErr.GetDetail()))
	tea_util.AssertEqual(t, "desc", tea.StringValue(clientErr.GetDescription()))
	tea_util.AssertEqual(t, "rid", tea.StringValue(clientErr.GetRequestId()))
	tea_util.AssertEqual(t, int64(10), tea.Int64Value(clientErr.GetRetryAfter()))
	tea_util.AssertEqual(t, 1, clientErr.GetData()["a"])
	tea_util.AssertEqual(t, "ClientError", tea.StringValue(clientErr.GetName()))
	tea_util.AssertEqual(t, "stack", tea.StringValue(clientErr.GetStack()))
	tea_util.AssertEqual(t, "v", clientErr.GetAccessDeniedDetail()["k"])
	tea_util.AssertEqual(t, "client-msg", clientErr.Error())

	serverErr := &ServerError{Name: tea.String("ServerError"), Code: tea.String("Internal")}
	tea_util.AssertEqual(t, true, strings.Contains(serverErr.Error(), "ServerError"))
	serverErr.Message = tea.String("server-msg")
	tea_util.AssertEqual(t, "server-msg", tea.StringValue(serverErr.GetMessage()))
	tea_util.AssertEqual(t, "Internal", tea.StringValue(serverErr.GetCode()))
	tea_util.AssertNil(t, serverErr.GetStatusCode())
	tea_util.AssertNil(t, serverErr.GetDetail())
	tea_util.AssertNil(t, serverErr.GetDescription())
	tea_util.AssertNil(t, serverErr.GetRequestId())
	tea_util.AssertNil(t, serverErr.GetRetryAfter())
	tea_util.AssertNil(t, serverErr.GetData())
	tea_util.AssertNil(t, serverErr.GetAccessDeniedDetail())
	tea_util.AssertEqual(t, "ServerError", tea.StringValue(serverErr.GetName()))
	tea_util.AssertNil(t, serverErr.GetStack())

	throttling := &ThrottlingError{
		StatusCode:  tea.Int(429),
		Code:        tea.String("Throttling"),
		Message:     tea.String("slow down"),
		RetryAfter:  tea.Int64(2000),
		RequestId:   tea.String("rid"),
		Detail:      tea.String("d"),
		Description: tea.String("desc"),
		Data:        map[string]interface{}{"n": 1},
		Name:        tea.String("ThrottlingError"),
		Stack:       tea.String("s"),
	}
	tea_util.AssertEqual(t, "slow down", throttling.Error())
	tea_util.AssertEqual(t, 429, tea.IntValue(throttling.GetStatusCode()))
	tea_util.AssertEqual(t, "Throttling", tea.StringValue(throttling.GetCode()))
	tea_util.AssertEqual(t, "slow down", tea.StringValue(throttling.GetMessage()))
	tea_util.AssertEqual(t, "d", tea.StringValue(throttling.GetDetail()))
	tea_util.AssertEqual(t, "desc", tea.StringValue(throttling.GetDescription()))
	tea_util.AssertEqual(t, "rid", tea.StringValue(throttling.GetRequestId()))
	tea_util.AssertEqual(t, int64(2000), tea.Int64Value(throttling.GetRetryAfter()))
	tea_util.AssertEqual(t, 1, throttling.GetData()["n"])
	tea_util.AssertEqual(t, "ThrottlingError", tea.StringValue(throttling.GetName()))
	tea_util.AssertEqual(t, "s", tea.StringValue(throttling.GetStack()))
	tea_util.AssertNil(t, throttling.GetAccessDeniedDetail())

	nilThrottle := &ThrottlingError{Name: tea.String("ThrottlingError"), Code: tea.String("Throttling")}
	tea_util.AssertEqual(t, true, strings.Contains(nilThrottle.Error(), "ThrottlingError"))
	nilThrottle.Message = tea.String("throttle-msg")
	tea_util.AssertEqual(t, "throttle-msg", tea.StringValue(nilThrottle.GetMessage()))

	cloudErr := &AlibabaCloudError{Name: tea.String("AlibabaCloudError"), Code: tea.String("X")}
	tea_util.AssertEqual(t, true, strings.Contains(cloudErr.Error(), "AlibabaCloudError"))
	cloudErr.Message = tea.String("cloud-msg")
	tea_util.AssertEqual(t, "cloud-msg", tea.StringValue(cloudErr.GetMessage()))
	tea_util.AssertNil(t, cloudErr.GetRetryAfter())
	tea_util.AssertNil(t, cloudErr.GetData())
	tea_util.AssertNil(t, cloudErr.GetAccessDeniedDetail())
	tea_util.AssertEqual(t, "AlibabaCloudError", tea.StringValue(cloudErr.GetName()))
	tea_util.AssertNil(t, cloudErr.GetStack())
	tea_util.AssertNil(t, cloudErr.GetStatusCode())
	tea_util.AssertEqual(t, "X", tea.StringValue(cloudErr.GetCode()))
	tea_util.AssertNil(t, cloudErr.GetDetail())
	tea_util.AssertNil(t, cloudErr.GetDescription())
	tea_util.AssertNil(t, cloudErr.GetRequestId())

	sse := &SSEResponse{}
	sse.SetHeaders(map[string]*string{"k": tea.String("v")})
	sse.SetStatusCode(200)
	event := &dara.SSEEvent{Data: tea.String("d")}
	sse.SetEvent(event)
	tea_util.AssertEqual(t, "v", tea.StringValue(sse.GetHeaders()["k"]))
	tea_util.AssertEqual(t, 200, tea.IntValue(sse.GetStatusCode()))
	tea_util.AssertEqual(t, "d", tea.StringValue(sse.GetEvent().Data))
	tea_util.AssertNotNil(t, sse.String())
	tea_util.AssertNotNil(t, sse.GoString())
	_ = sse.Validate()
	_ = (&SSEResponse{}).Validate()
	_ = (&SSEResponse{Headers: map[string]*string{"k": tea.String("v")}}).Validate()
	_ = (&SSEResponse{Headers: map[string]*string{"k": tea.String("v")}, StatusCode: tea.Int(200)}).Validate()
}

func TestV2ROAAllResponseTypes(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	mux.Handle("/testArray", &mockHandler{content: "array"})
	mux.Handle("/testError", &mockHandler{content: "error"})
	mux.Handle("/testError2", &mockHandler{content: "error2"})
	mux.Handle("/serverError", &mockHandler{content: "serverError"})
	server, endpoint := startMockHTTPServer(mux)
	defer server.Close()
	client := newCoverageClient(t, endpoint, func(c *Config) {
		c.SignatureAlgorithm = tea.String("v2")
	})
	runtime := CreateRuntimeOptions()
	request := CreateOpenApiRequest()
	params := coverageParams("ROA", "json", "json", "/test", "Anonymous")
	result, err := client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, err)

	params.BodyType = tea.String("array")
	params.Pathname = tea.String("/testArray")
	result, err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertNotNil(t, result["body"])

	params.BodyType = tea.String("string")
	params.Pathname = tea.String("/test")
	result, err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, err)

	params.BodyType = tea.String("byte")
	result, err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, err)

	params.BodyType = tea.String("binary")
	result, err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, err)

	params.BodyType = tea.String("none")
	result, err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, err)

	client.DisableSDKError = tea.Bool(true)
	params.Pathname = tea.String("/testError")
	_, err = client.CallApi(params, request, runtime)
	tea_util.AssertNotNil(t, err)
	params.Pathname = tea.String("/testError2")
	_, err = client.CallApi(params, request, runtime)
	tea_util.AssertNotNil(t, err)
	params.Pathname = tea.String("/serverError")
	_, err = client.CallApi(params, request, runtime)
	tea_util.AssertNotNil(t, err)

	formParams := coverageParams("ROA", "formData", "string", "/test", "Anonymous")
	result, err = client.CallApi(formParams, request, runtime)
	tea_util.AssertNil(t, err)
	formParams.BodyType = tea.String("array")
	formParams.Pathname = tea.String("/testArray")
	result, err = client.CallApi(formParams, request, runtime)
	tea_util.AssertNil(t, err)
	formParams.BodyType = tea.String("binary")
	formParams.Pathname = tea.String("/test")
	result, err = client.CallApi(formParams, request, runtime)
	tea_util.AssertNil(t, err)
}

func TestNetworkErrorsCoverRetryPaths(t *testing.T) {
	client := newCoverageClient(t, "127.0.0.1:1")
	runtime := CreateRuntimeOptions()
	runtime.Autoretry = tea.Bool(true)
	runtime.MaxAttempts = tea.Int(1)
	runtime.ConnectTimeout = tea.Int(50)
	runtime.ReadTimeout = tea.Int(50)
	request := CreateOpenApiRequest()
	ctx := context.Background()

	_, err := client.CallApi(coverageParams("RPC", "json", "json", "/", "Anonymous"), request, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = client.CallApiWithCtx(ctx, coverageParams("RPC", "json", "json", "/", "Anonymous"), request, runtime)
	tea_util.AssertNotNil(t, err)

	v2 := newCoverageClient(t, "127.0.0.1:1", func(c *Config) {
		c.SignatureAlgorithm = tea.String("v2")
	})
	_, err = v2.CallApi(coverageParams("RPC", "formData", "json", "/", "Anonymous"), request, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = v2.CallApiWithCtx(ctx, coverageParams("ROA", "json", "json", "/test", "Anonymous"), request, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = v2.CallApiWithCtx(ctx, coverageParams("ROA", "formData", "json", "/test", "Anonymous"), request, runtime)
	tea_util.AssertNotNil(t, err)

	execClient := newCoverageClient(t, "127.0.0.1:1", func(c *Config) {
		c.SignatureVersion = tea.String("v4")
	})
	execClient.ProductId = tea.String("test")
	gatewayClient, gerr := pop.NewClient()
	tea_util.AssertNil(t, gerr)
	_ = execClient.SetGatewayClient(gatewayClient)
	_, err = execClient.CallApi(coverageParams("RPC", "json", "json", "/", "Anonymous"), request, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = execClient.ExecuteWithCtx(ctx, coverageParams("RPC", "json", "json", "/", "Anonymous"), request, runtime)
	tea_util.AssertNotNil(t, err)
}

func TestSSEXmlErrorAndACS3Xml(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/sse-xml", &extraMockHandler{content: "xml"})
	mux.Handle("/xml", &extraMockHandler{content: "xml"})
	server, endpoint := startMockHTTPServer(mux)
	defer server.Close()
	client := newCoverageClient(t, endpoint)
	runtime := CreateRuntimeOptions()
	request := CreateOpenApiRequest()
	params := coverageParams("ROA", "formData", "json", "/sse-xml", "Anonymous")
	params.Method = tea.String("GET")

	result := make(chan *SSEResponse, 1)
	yieldErr := make(chan error, 1)
	go client.CallSSEApi(params, request, runtime, result, yieldErr)
	for range result {
	}
	err := <-yieldErr
	tea_util.AssertNotNil(t, err)

	result = make(chan *SSEResponse, 1)
	yieldErr = make(chan error, 1)
	go client.CallSSEApiWithCtx(context.Background(), params, request, runtime, result, yieldErr)
	for range result {
	}
	err = <-yieldErr
	tea_util.AssertNotNil(t, err)
}

func TestV2RPCAndROAByteNoneAndServerErrorWithCtx(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/", &extraMockHandler{content: "json"})
	mux.Handle("/test", &extraMockHandler{content: "json"})
	mux.Handle("/array", &extraMockHandler{content: "array"})
	mux.Handle("/error", &extraMockHandler{content: "error"})
	mux.Handle("/serverError", &extraMockHandler{content: "serverError"})
	server, endpoint := startMockHTTPServer(mux)
	defer server.Close()
	client := newCoverageClient(t, endpoint, func(c *Config) {
		c.SignatureAlgorithm = tea.String("v2")
	})
	client.DisableSDKError = tea.Bool(true)
	runtime := CreateRuntimeOptions()
	request := CreateOpenApiRequest()
	ctx := context.Background()

	params := coverageParams("RPC", "formData", "byte", "/", "Anonymous")
	result, err := client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertNotNil(t, result["body"])
	result, err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, err)

	params.BodyType = tea.String("none")
	result, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)
	result, err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, err)

	params.BodyType = tea.String("array")
	params.Pathname = tea.String("/array")
	result, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)

	params = coverageParams("ROA", "json", "byte", "/test", "Anonymous")
	result, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)
	params.BodyType = tea.String("none")
	result, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)
	params.BodyType = tea.String("array")
	params.Pathname = tea.String("/array")
	result, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)

	params = coverageParams("ROA", "formData", "byte", "/test", "Anonymous")
	result, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)
	params.BodyType = tea.String("none")
	result, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNil(t, err)

	params = coverageParams("ROA", "json", "json", "/error", "Anonymous")
	_, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNotNil(t, err)
	params = coverageParams("ROA", "formData", "json", "/error", "Anonymous")
	_, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNotNil(t, err)
	params = coverageParams("ROA", "json", "json", "/serverError", "Anonymous")
	_, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNotNil(t, err)
	params = coverageParams("ROA", "formData", "json", "/serverError", "Anonymous")
	_, err = client.CallApiWithCtx(ctx, params, request, runtime)
	tea_util.AssertNotNil(t, err)

	errorServer, errorEndpoint := startMockHTTPServer(&extraMockHandler{content: "error"})
	defer errorServer.Close()
	errorClient := newCoverageClient(t, errorEndpoint, func(c *Config) {
		c.SignatureAlgorithm = tea.String("v2")
	})
	errorClient.DisableSDKError = tea.Bool(true)
	_, err = errorClient.CallApiWithCtx(ctx, coverageParams("RPC", "formData", "json", "/", "Anonymous"), request, runtime)
	tea_util.AssertNotNil(t, err)

	serverErrServer, serverErrEndpoint := startMockHTTPServer(&extraMockHandler{content: "serverError"})
	defer serverErrServer.Close()
	serverErrClient := newCoverageClient(t, serverErrEndpoint, func(c *Config) {
		c.SignatureAlgorithm = tea.String("v2")
	})
	serverErrClient.DisableSDKError = tea.Bool(true)
	_, err = serverErrClient.CallApiWithCtx(ctx, coverageParams("RPC", "formData", "json", "/", "Anonymous"), request, runtime)
	tea_util.AssertNotNil(t, err)
}

func TestCallApiWithCtx_AKBearerRPCHeadersAndBodies(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/", &mockHandler{content: "json"})
	mux.Handle("/test", &mockHandler{content: "json"})
	mux.Handle("/sse", &mockHandler{content: "json"})
	server, endpoint := startMockHTTPServer(mux)
	defer server.Close()
	runtime := CreateRuntimeOptions()
	ctx := context.Background()

	client := newCoverageClient(t, endpoint)
	_ = client.SetRpcHeaders(map[string]*string{"rpc-key": tea.String("rpc-value")})
	result, err := client.CallApiWithCtx(ctx, coverageParams("RPC", "json", "json", "/test", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	streamReq := &OpenApiRequest{
		Headers: map[string]*string{"for-test": tea.String("sdk")},
		Query:   map[string]*string{"q": tea.String("1")},
		Stream:  strings.NewReader("stream-body"),
	}
	result, err = client.DoRequestWithCtx(ctx, coverageParams("ROA", "json", "json", "/test", "AK"), streamReq, runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	byteReq := &OpenApiRequest{Body: util.ToBytes(tea.String("test byte"))}
	result, err = client.DoRequest(coverageParams("ROA", "byte", "string", "/test", "AK"), byteReq, runtime)
	tea_util.AssertNil(t, err)

	v2 := newCoverageClient(t, endpoint, func(c *Config) {
		c.SignatureAlgorithm = tea.String("v2")
	})
	result, err = v2.CallApiWithCtx(ctx, coverageParams("RPC", "formData", "json", "/", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = v2.CallApiWithCtx(ctx, coverageParams("ROA", "json", "json", "/test", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = v2.CallApiWithCtx(ctx, coverageParams("ROA", "formData", "json", "/test", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = v2.DoRPCRequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("AK"), tea.String("string"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = v2.DoROARequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("AK"), tea.String("/test"), tea.String("byte"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = v2.DoROAFormRequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("AK"), tea.String("/test"), tea.String("none"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)

	bearer := CreateBearerTokenConfig()
	bearer.Protocol = tea.String("HTTP")
	bearer.Endpoint = tea.String(endpoint)
	bearerClient, err := NewClient(bearer)
	tea_util.AssertNil(t, err)
	result, err = bearerClient.CallApiWithCtx(ctx, coverageParams("RPC", "json", "json", "/test", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = bearerClient.CallApiWithCtx(ctx, coverageParams("ROA", "json", "json", "/test", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = bearerClient.DoRequest(coverageParams("RPC", "json", "json", "/test", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)

	v2Bearer := CreateBearerTokenConfig()
	v2Bearer.Protocol = tea.String("HTTP")
	v2Bearer.Endpoint = tea.String(endpoint)
	v2Bearer.SignatureAlgorithm = tea.String("v2")
	v2BearerClient, err := NewClient(v2Bearer)
	tea_util.AssertNil(t, err)
	result, err = v2BearerClient.CallApiWithCtx(ctx, coverageParams("RPC", "formData", "json", "/", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = v2BearerClient.CallApiWithCtx(ctx, coverageParams("ROA", "json", "json", "/test", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = v2BearerClient.CallApiWithCtx(ctx, coverageParams("ROA", "formData", "json", "/test", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)

	execClient := newCoverageClient(t, endpoint, func(c *Config) {
		c.SignatureVersion = tea.String("v4")
	})
	execClient.ProductId = tea.String("test")
	gatewayClient, gerr := pop.NewClient()
	tea_util.AssertNil(t, gerr)
	_ = execClient.SetGatewayClient(gatewayClient)
	_ = execClient.SetRpcHeaders(map[string]*string{"rpc-exec": tea.String("1")})
	result, err = execClient.Execute(coverageParams("RPC", "json", "json", "/test", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = execClient.ExecuteWithCtx(ctx, coverageParams("ROA", "formData", "byte", "/test", "Anonymous"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)

	params := coverageParams("RPC", "formData", "json", "/sse", "AK")
	params.Method = tea.String("GET")
	events := make(chan *SSEResponse, 1)
	yieldErr := make(chan error, 1)
	go client.CallSSEApiWithCtx(ctx, params, CreateOpenApiRequest(), runtime, events, yieldErr)
	for range events {
	}
	err = <-yieldErr
	tea_util.AssertNil(t, err)

	anon := CreateAnonymousConfig()
	anon.Protocol = tea.String("HTTP")
	anon.Endpoint = tea.String(endpoint)
	anonClient, err := NewClient(anon)
	tea_util.AssertNil(t, err)
	_, err = anonClient.CallApiWithCtx(ctx, coverageParams("RPC", "json", "json", "/test", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNotNil(t, err)
	_, err = anonClient.DoRequest(coverageParams("ROA", "json", "json", "/test", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNotNil(t, err)
}

func TestWebSocketConnectFailureAndInvalidCredentials(t *testing.T) {
	client := newCoverageClient(t, "127.0.0.1:1")
	runtime := CreateRuntimeOptions()
	runtime.ConnectTimeout = tea.Int(50)
	runtime.ReadTimeout = tea.Int(50)
	runtime.MaxAttempts = tea.Int(1)
	runtime.WebSocketHandler = &dara.AbstractWebSocketHandler{}
	params := coverageParams("RPC", "json", "json", "/ws", "Anonymous")
	params.Protocol = tea.String("ws")
	params.WebsocketSubProtocol = tea.String(websocketutils.SubProtocolAWAP)
	_, err := client.DoRequest(params, CreateOpenApiRequest(), runtime)
	tea_util.AssertNotNil(t, err)
	params.Protocol = tea.String("wss")
	params.WebsocketSubProtocol = tea.String(websocketutils.SubProtocolGeneral)
	_, err = client.DoRequestWithCtx(context.Background(), params, CreateOpenApiRequest(), runtime)
	tea_util.AssertNotNil(t, err)
}

type errReader struct{}

func (*errReader) Read(p []byte) (int, error) {
	return 0, io.ErrUnexpectedEOF
}

type mockCredential struct {
	model *credential.CredentialModel
	err   error
}

func (m *mockCredential) GetAccessKeyId() (*string, error) {
	if m.err != nil {
		return nil, m.err
	}
	return m.model.AccessKeyId, nil
}
func (m *mockCredential) GetAccessKeySecret() (*string, error) {
	if m.err != nil {
		return nil, m.err
	}
	return m.model.AccessKeySecret, nil
}
func (m *mockCredential) GetSecurityToken() (*string, error) {
	if m.err != nil {
		return nil, m.err
	}
	return m.model.SecurityToken, nil
}
func (m *mockCredential) GetBearerToken() *string {
	if m.model == nil {
		return nil
	}
	return m.model.BearerToken
}
func (m *mockCredential) GetType() *string {
	if m.model == nil {
		return tea.String("")
	}
	return m.model.Type
}
func (m *mockCredential) GetCredential() (*credential.CredentialModel, error) {
	return m.model, m.err
}

type mockSPI struct {
	cfgErr  error
	reqErr  error
	respErr error
}

func (m *mockSPI) ModifyConfiguration(context *spi.InterceptorContext, attributeMap *spi.AttributeMap) error {
	return m.cfgErr
}
func (m *mockSPI) ModifyRequest(context *spi.InterceptorContext, attributeMap *spi.AttributeMap) error {
	return m.reqErr
}
func (m *mockSPI) ModifyResponse(context *spi.InterceptorContext, attributeMap *spi.AttributeMap) error {
	return m.respErr
}

func TestRPCArrayFailingStreamIdTokenAndSPI(t *testing.T) {
	arrayServer, arrayEndpoint := startMockHTTPServer(&extraMockHandler{content: "array"})
	defer arrayServer.Close()
	runtime := CreateRuntimeOptions()
	ctx := context.Background()
	v2 := newCoverageClient(t, arrayEndpoint, func(c *Config) {
		c.SignatureAlgorithm = tea.String("v2")
	})
	params := coverageParams("RPC", "formData", "array", "/", "Anonymous")
	result, err := v2.CallApi(params, CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	tea_util.AssertNotNil(t, result["body"])
	result, err = v2.CallApiWithCtx(ctx, params, CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)

	formParams := coverageParams("ROA", "formData", "byte", "/array", "Anonymous")
	result, err = v2.CallApiWithCtx(ctx, formParams, CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)

	acs := newCoverageClient(t, arrayEndpoint)
	result, err = acs.CallApi(coverageParams("RPC", "json", "array", "/", "Anonymous"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = acs.CallApiWithCtx(ctx, coverageParams("RPC", "json", "array", "/", "Anonymous"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)

	// Use a pointer reader: older dara.IsNil panics on a struct value and
	// treats it as a nil stream, so Go 1.13–1.15 never hit the error path.
	failReq := &OpenApiRequest{Stream: &errReader{}}
	_, err = acs.DoRequest(coverageParams("ROA", "json", "json", "/", "Anonymous"), failReq, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = acs.DoRequestWithCtx(ctx, coverageParams("ROA", "json", "json", "/", "Anonymous"), failReq, runtime)
	tea_util.AssertNotNil(t, err)
	sseParams := coverageParams("ROA", "json", "json", "/", "Anonymous")
	sseParams.Method = tea.String("GET")
	events := make(chan *SSEResponse, 1)
	yieldErr := make(chan error, 1)
	go acs.CallSSEApi(sseParams, failReq, runtime, events, yieldErr)
	for range events {
	}
	err = <-yieldErr
	tea_util.AssertNotNil(t, err)

	jsonServer, jsonEndpoint := startMockHTTPServer(&mockHandler{content: "json"})
	defer jsonServer.Close()
	idClient := newCoverageClient(t, jsonEndpoint)
	idClient.Credential = &mockCredential{model: &credential.CredentialModel{
		Type:          tea.String("id_token"),
		SecurityToken: tea.String("id-token-value"),
		ProviderName:  tea.String("mock"),
	}}
	result, err = idClient.CallApi(coverageParams("RPC", "json", "json", "/", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = idClient.CallApiWithCtx(ctx, coverageParams("ROA", "json", "json", "/", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = idClient.DoRequest(coverageParams("RPC", "json", "json", "/", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	v2ID := newCoverageClient(t, jsonEndpoint, func(c *Config) {
		c.SignatureAlgorithm = tea.String("v2")
	})
	v2ID.Credential = idClient.Credential
	result, err = v2ID.CallApiWithCtx(ctx, coverageParams("RPC", "formData", "json", "/", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = v2ID.CallApiWithCtx(ctx, coverageParams("ROA", "json", "json", "/", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)
	result, err = v2ID.CallApiWithCtx(ctx, coverageParams("ROA", "formData", "json", "/", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNil(t, err)

	failCred := newCoverageClient(t, jsonEndpoint)
	failCred.Credential = &mockCredential{err: io.ErrUnexpectedEOF}
	_, err = failCred.CallApi(coverageParams("RPC", "json", "json", "/", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNotNil(t, err)
	_, err = failCred.CallApiWithCtx(ctx, coverageParams("ROA", "json", "json", "/", "AK"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNotNil(t, err)
	_, err = failCred.GetAccessKeyId()
	tea_util.AssertNotNil(t, err)
	_, err = failCred.GetAccessKeySecret()
	tea_util.AssertNotNil(t, err)
	_, err = failCred.GetSecurityToken()
	tea_util.AssertNotNil(t, err)

	sseJSON := coverageParams("ROA", "json", "json", "/", "AK")
	sseJSON.Method = tea.String("GET")
	okEvents := make(chan *SSEResponse, 1)
	okErr := make(chan error, 1)
	go idClient.CallSSEApi(sseJSON, CreateOpenApiRequest(), runtime, okEvents, okErr)
	for range okEvents {
	}
	err = <-okErr
	tea_util.AssertNil(t, err)
	sseByte := coverageParams("ROA", "byte", "json", "/", "Anonymous")
	sseByte.Method = tea.String("GET")
	byteEvents := make(chan *SSEResponse, 1)
	byteErr := make(chan error, 1)
	go acs.CallSSEApiWithCtx(ctx, sseByte, &OpenApiRequest{Body: util.ToBytes(tea.String("test byte"))}, runtime, byteEvents, byteErr)
	for range byteEvents {
	}
	<-byteErr

	execClient := newCoverageClient(t, jsonEndpoint, func(c *Config) {
		c.SignatureVersion = tea.String("v4")
	})
	execClient.ProductId = tea.String("test")
	execClient.Spi = &mockSPI{cfgErr: io.ErrUnexpectedEOF}
	execClient.DisableSDKError = tea.Bool(true)
	_, err = execClient.Execute(coverageParams("RPC", "json", "json", "/", "Anonymous"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNotNil(t, err)
	execClient.Spi = &mockSPI{reqErr: io.ErrUnexpectedEOF}
	_, err = execClient.ExecuteWithCtx(ctx, coverageParams("RPC", "json", "json", "/", "Anonymous"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNotNil(t, err)
	execClient.Spi = &mockSPI{respErr: io.ErrUnexpectedEOF}
	_, err = execClient.Execute(coverageParams("RPC", "json", "json", "/", "Anonymous"), CreateOpenApiRequest(), runtime)
	tea_util.AssertNotNil(t, err)
}

func TestV2ROAFormWithCtxAllBodies(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	mux.Handle("/testArray", &mockHandler{content: "array"})
	mux.Handle("/empty", &extraMockHandler{content: "empty204"})
	mux.Handle("/error", &extraMockHandler{content: "error"})
	mux.Handle("/serverError", &extraMockHandler{content: "serverError"})
	server, endpoint := startMockHTTPServer(mux)
	defer server.Close()
	client := newCoverageClient(t, endpoint, func(c *Config) {
		c.SignatureAlgorithm = tea.String("v2")
	})
	client.DisableSDKError = tea.Bool(true)
	runtime := CreateRuntimeOptions()
	request := CreateOpenApiRequest()
	ctx := context.Background()
	bodyTypes := []string{"json", "string", "byte", "binary", "array", "none"}
	for i := 0; i < len(bodyTypes); i++ {
		bodyType := bodyTypes[i]
		pathname := "/test"
		if bodyType == "array" {
			pathname = "/testArray"
		}
		_, err := client.DoROAFormRequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String(pathname), tea.String(bodyType), request, runtime)
		tea_util.AssertNil(t, err)
		_, err = client.DoROARequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String(pathname), tea.String(bodyType), request, runtime)
		tea_util.AssertNil(t, err)
	}
	_, err := client.DoROAFormRequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String("/empty"), tea.String("json"), request, runtime)
	tea_util.AssertNil(t, err)
	_, err = client.DoROARequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String("/empty"), tea.String("json"), request, runtime)
	tea_util.AssertNil(t, err)
	_, err = client.DoROAFormRequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String("/error"), tea.String("json"), request, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = client.DoROAFormRequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String("/serverError"), tea.String("json"), request, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = client.DoROARequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String("/error"), tea.String("json"), request, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = client.DoROARequestWithCtx(ctx, tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String("/serverError"), tea.String("json"), request, runtime)
	tea_util.AssertNotNil(t, err)
}

func TestXMLErrorsOnRPCAndDoRequest(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/", &extraMockHandler{content: "xml"})
	mux.Handle("/xml", &extraMockHandler{content: "xml"})
	mux.Handle("/test", &extraMockHandler{content: "xml"})
	server, endpoint := startMockHTTPServer(mux)
	defer server.Close()
	client := newCoverageClient(t, endpoint)
	runtime := CreateRuntimeOptions()
	request := CreateOpenApiRequest()
	_, err := client.DoRPCRequest(tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String("json"), request, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = client.DoRPCRequestWithCtx(context.Background(), tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String("json"), request, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = client.DoRequest(coverageParams("ROA", "json", "json", "/xml", "Anonymous"), request, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = client.DoRequestWithCtx(context.Background(), coverageParams("ROA", "formData", "json", "/xml", "AK"), request, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = client.DoROARequest(tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String("/xml"), tea.String("json"), request, runtime)
	tea_util.AssertNotNil(t, err)
	_, err = client.DoROARequestWithForm(tea.String("TestAPI"), tea.String("2022-06-01"), tea.String("HTTPS"), tea.String("POST"), tea.String("Anonymous"), tea.String("/xml"), tea.String("json"), request, runtime)
	tea_util.AssertNotNil(t, err)
}
