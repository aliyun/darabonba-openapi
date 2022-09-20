package client

import (
	"encoding/json"
	"net/http"
	"regexp"
	"strings"
	"testing"
	"time"

	openapiutil "github.com/alibabacloud-go/openapi-util/service"
	util "github.com/alibabacloud-go/tea-utils/v2/service"
	"github.com/alibabacloud-go/tea/tea"
	tea_util "github.com/alibabacloud-go/tea/utils"
	credential "github.com/aliyun/credentials-go/credentials"
)

type mockHandler struct {
	content string
}

func (mock *mockHandler) ServeHTTP(w http.ResponseWriter, req *http.Request) {
	if req.Header != nil {
		nv := 0
		for k, vv := range req.Header {
			if k != "Content-Length" {
				nv += len(vv)
			}
		}
		sv := make([]string, nv)
		for k, vv := range req.Header {
			if k != "Content-Length" {
				n := copy(sv, vv)
				w.Header()[k] = sv[:n:n]
				sv = sv[n:]
			}
		}
	}
	w.Header().Set("raw-query", req.URL.RawQuery)
	body, _ := util.ReadAsString(req.Body)
	w.Header().Set("raw-body", tea.StringValue(body))
	w.Header().Set("x-acs-request-id", "A45EE076-334D-5012-9746-A8F828D20FD4")
	responseBody := "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}"
	switch mock.content {
	case "array":
		responseBody = "[\"AppId\", \"ClassId\", \"UserId\"]"
		w.WriteHeader(200)
		w.Write([]byte(responseBody))
	case "error":
		responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"}"
		w.WriteHeader(400)
		w.Write([]byte(responseBody))
	default:
		w.WriteHeader(200)
		w.Write([]byte(responseBody))
	}
}

func TestConfig(t *testing.T) {
	globalParameters := &GlobalParameters{
		Headers: map[string]*string{
			"global-key": tea.String("global-value"),
		},
		Queries: map[string]*string{
			"global-query": tea.String("global-value"),
		},
	}
	config := &Config{
		Endpoint:           tea.String("config.endpoint"),
		EndpointType:       tea.String("public"),
		Network:            tea.String("config.network"),
		Suffix:             tea.String("config.suffix"),
		Protocol:           tea.String("config.protocol"),
		Method:             tea.String("config.method"),
		RegionId:           tea.String("config.regionId"),
		UserAgent:          tea.String("config.userAgent"),
		ReadTimeout:        tea.Int(3000),
		ConnectTimeout:     tea.Int(3000),
		HttpProxy:          tea.String("config.httpProxy"),
		HttpsProxy:         tea.String("config.httpsProxy"),
		NoProxy:            tea.String("config.noProxy"),
		Socks5Proxy:        tea.String("config.socks5Proxy"),
		Socks5NetWork:      tea.String("config.socks5NetWork"),
		MaxIdleConns:       tea.Int(128),
		SignatureVersion:   tea.String("config.signatureVersion"),
		SignatureAlgorithm: tea.String("config.signatureAlgorithm"),
		GlobalParameters:   globalParameters,
	}
	creConfig := &credential.Config{
		AccessKeyId:     tea.String("accessKeyId"),
		AccessKeySecret: tea.String("accessKeySecret"),
		SecurityToken:   tea.String("securityToken"),
		Type:            tea.String("sts"),
	}
	credential, _err := credential.NewCredential(creConfig)
	tea_util.AssertNil(t, _err)

	config.Credential = credential
	client, _err := NewClient(config)
	tea_util.AssertNil(t, _err)
	ak, _ := client.GetAccessKeyId()
	tea_util.AssertEqual(t, "accessKeyId", tea.StringValue(ak))
	sk, _ := client.GetAccessKeySecret()
	tea_util.AssertEqual(t, "accessKeySecret", tea.StringValue(sk))
	token, _ := client.GetSecurityToken()
	tea_util.AssertEqual(t, "securityToken", tea.StringValue(token))
	ty, _ := client.GetType()
	tea_util.AssertEqual(t, "sts", tea.StringValue(ty))

	config.AccessKeyId = tea.String("ak")
	config.AccessKeySecret = tea.String("secret")
	config.SecurityToken = tea.String("token")
	config.Type = tea.String("sts")
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	ak, _ = client.GetAccessKeyId()
	tea_util.AssertEqual(t, "ak", tea.StringValue(ak))
	sk, _ = client.GetAccessKeySecret()
	tea_util.AssertEqual(t, "secret", tea.StringValue(sk))
	token, _ = client.GetSecurityToken()
	tea_util.AssertEqual(t, "token", tea.StringValue(token))
	ty, _ = client.GetType()
	tea_util.AssertEqual(t, "sts", tea.StringValue(ty))
	tea_util.AssertNil(t, client.Spi)
	tea_util.AssertNil(t, client.EndpointRule)
	tea_util.AssertNil(t, client.EndpointMap)
	tea_util.AssertNil(t, client.ProductId)
	tea_util.AssertEqual(t, "config.endpoint", tea.StringValue(client.Endpoint))
	tea_util.AssertEqual(t, "public", tea.StringValue(client.EndpointType))
	tea_util.AssertEqual(t, "config.network", tea.StringValue(client.Network))
	tea_util.AssertEqual(t, "config.suffix", tea.StringValue(client.Suffix))
	tea_util.AssertEqual(t, "config.protocol", tea.StringValue(client.Protocol))
	tea_util.AssertEqual(t, "config.method", tea.StringValue(client.Method))
	tea_util.AssertEqual(t, "config.regionId", tea.StringValue(client.RegionId))
	tea_util.AssertEqual(t, "config.userAgent", tea.StringValue(client.UserAgent))
	tea_util.AssertEqual(t, 3000, tea.IntValue(client.ReadTimeout))
	tea_util.AssertEqual(t, 3000, tea.IntValue(client.ConnectTimeout))
	tea_util.AssertEqual(t, "config.httpProxy", tea.StringValue(client.HttpProxy))
	tea_util.AssertEqual(t, "config.httpsProxy", tea.StringValue(client.HttpsProxy))
	tea_util.AssertEqual(t, "config.noProxy", tea.StringValue(client.NoProxy))
	tea_util.AssertEqual(t, "config.socks5Proxy", tea.StringValue(client.Socks5Proxy))
	tea_util.AssertEqual(t, "config.socks5NetWork", tea.StringValue(client.Socks5NetWork))
	tea_util.AssertEqual(t, 128, tea.IntValue(client.MaxIdleConns))
	tea_util.AssertEqual(t, "config.signatureVersion", tea.StringValue(client.SignatureVersion))
	tea_util.AssertEqual(t, "config.signatureAlgorithm", tea.StringValue(client.SignatureAlgorithm))
	tea_util.AssertEqual(t, "global-value", tea.StringValue(client.GlobalParameters.Headers["global-key"]))
	tea_util.AssertEqual(t, "global-value", tea.StringValue(client.GlobalParameters.Queries["global-query"]))
}

func CreateConfig() (_result *Config) {
	globalParameters := &GlobalParameters{
		Headers: map[string]*string{
			"global-key": tea.String("global-value"),
		},
		Queries: map[string]*string{
			"global-query": tea.String("global-value"),
		},
	}
	config := &Config{
		AccessKeyId:        tea.String("ak"),
		AccessKeySecret:    tea.String("secret"),
		SecurityToken:      tea.String("token"),
		Type:               tea.String("sts"),
		UserAgent:          tea.String("config.userAgent"),
		ReadTimeout:        tea.Int(3000),
		ConnectTimeout:     tea.Int(3000),
		MaxIdleConns:       tea.Int(128),
		SignatureVersion:   tea.String("config.signatureVersion"),
		SignatureAlgorithm: tea.String("ACS3-HMAC-SHA256"),
		GlobalParameters:   globalParameters,
	}
	_result = config
	return _result
}

func CreateRuntimeOptions() (_result *util.RuntimeOptions) {
	runtime := &util.RuntimeOptions{
		ReadTimeout:    tea.Int(4000),
		ConnectTimeout: tea.Int(4000),
		MaxIdleConns:   tea.Int(100),
		Autoretry:      tea.Bool(true),
		MaxAttempts:    tea.Int(1),
		BackoffPolicy:  tea.String("no"),
		BackoffPeriod:  tea.Int(1),
		IgnoreSSL:      tea.Bool(true),
	}
	_result = runtime
	return _result
}

func CreateOpenApiRequest() (_result *OpenApiRequest) {
	query := map[string]interface{}{}
	query["key1"] = tea.String("value")
	query["key2"] = tea.Int(1)
	query["key3"] = tea.Bool(true)
	body := map[string]interface{}{}
	body["key1"] = tea.String("value")
	body["key2"] = tea.Int(1)
	body["key3"] = tea.Bool(true)
	headers := map[string]*string{
		"for-test": tea.String("sdk"),
	}
	req := &OpenApiRequest{
		Headers: headers,
		Query:   openapiutil.Query(query),
		Body:    openapiutil.ParseToMap(body),
	}
	_result = req
	return _result
}

func TestCallApiForRPCWithV2Sign_AK_Form(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/", &mockHandler{content: "json"})
	var server *http.Server
	server = &http.Server{
		Addr:         ":9001",
		WriteTimeout: time.Second * 4,
		Handler:      mux,
	}
	go server.ListenAndServe()
	config := CreateConfig()
	runtime := CreateRuntimeOptions()
	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9001")
	client, _err := NewClient(config)
	tea_util.AssertNil(t, _err)
	request := CreateOpenApiRequest()
	params := &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String("/"),
		Method:      tea.String("POST"),
		AuthType:    tea.String("AK"),
		Style:       tea.String("RPC"),
		ReqBodyType: tea.String("formData"),
		BodyType:    tea.String("json"),
	}
	result, _err := client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err := util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "key1=value&key2=1&key3=true", headers["raw-body"])
	regx, _ := regexp.Compile("AccessKeyId=ak&Action=TestAPI&Format=json&SecurityToken=token&Signature=.+" +
		"&SignatureMethod=HMAC-SHA1&SignatureNonce=.+&SignatureVersion=1.0&Timestamp=.+&Version=2022-06-01" +
		"&global-query=global-value&key1=value&key2=1&key3=true")
	str, _ := util.AssertAsString(headers["raw-query"])
	find := regx.FindAllString(tea.StringValue(str), -1)
	tea_util.AssertNotNil(t, find)
	str, _ = util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertEqual(t, "2022-06-01", headers["x-acs-version"])
	tea_util.AssertEqual(t, "TestAPI", headers["x-acs-action"])
	tea_util.AssertEqual(t, "application/x-www-form-urlencoded", headers["content-type"])
	tea_util.AssertEqual(t, "A45EE076-334D-5012-9746-A8F828D20FD4", headers["x-acs-request-id"])

	body, _err := util.AssertAsMap(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", body["AppId"])
	tea_util.AssertEqual(t, "test", body["ClassId"])
	tea_util.AssertEqual(t, "123", body["UserId"].(json.Number).String())
}

func TestCallApiForRPCWithV2Sign_Anonymous_JSON(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/", &mockHandler{content: "json"})
	var server *http.Server
	server = &http.Server{
		Addr:         ":9002",
		WriteTimeout: time.Second * 4,
		Handler:      mux,
	}
	go server.ListenAndServe()
	config := CreateConfig()
	runtime := CreateRuntimeOptions()
	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9002")
	client, _err := NewClient(config)
	tea_util.AssertNil(t, _err)
	request := CreateOpenApiRequest()
	params := &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String("/"),
		Method:      tea.String("POST"),
		AuthType:    tea.String("Anonymous"),
		Style:       tea.String("RPC"),
		ReqBodyType: tea.String("json"),
		BodyType:    tea.String("json"),
	}
	result, _err := client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err := util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "key1=value&key2=1&key3=true", headers["raw-body"])
	regx, _ := regexp.Compile("Action=TestAPI&Format=json&SignatureNonce=.+&Timestamp=.+&Version=2022-06-01" +
		"&global-query=global-value&key1=value&key2=1&key3=true")
	str, _ := util.AssertAsString(headers["raw-query"])
	find := regx.FindAllString(tea.StringValue(str), -1)
	tea_util.AssertNotNil(t, find)
	str, _ = util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertEqual(t, "2022-06-01", headers["x-acs-version"])
	tea_util.AssertEqual(t, "TestAPI", headers["x-acs-action"])
	tea_util.AssertEqual(t, "application/x-www-form-urlencoded", headers["content-type"])
	tea_util.AssertEqual(t, "A45EE076-334D-5012-9746-A8F828D20FD4", headers["x-acs-request-id"])

	body, _err := util.AssertAsMap(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", body["AppId"])
	tea_util.AssertEqual(t, "test", body["ClassId"])
	tea_util.AssertEqual(t, "123", body["UserId"].(json.Number).String())
}

func TestCallApiForROAWithV2Sign_HTTPS_AK_Form(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	var server *http.Server
	server = &http.Server{
		Addr:         ":9003",
		WriteTimeout: time.Second * 4,
		Handler:      mux,
	}
	go server.ListenAndServe()
	config := CreateConfig()
	runtime := CreateRuntimeOptions()
	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9003")
	client, _err := NewClient(config)
	tea_util.AssertNil(t, _err)
	request := CreateOpenApiRequest()
	params := &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String("/test"),
		Method:      tea.String("POST"),
		AuthType:    tea.String("AK"),
		Style:       tea.String("ROA"),
		ReqBodyType: tea.String("formData"),
		BodyType:    tea.String("json"),
	}
	result, _err := client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err := util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "key1=value&key2=1&key3=true", headers["raw-body"])
	tea_util.AssertEqual(t, "global-query=global-value&key1=value&key2=1&key3=true", headers["raw-query"])
	str, _ := util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	str, _ = util.AssertAsString(headers["authorization"])
	has = strings.Contains(tea.StringValue(str), "acs ak:")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertEqual(t, "sdk", headers["for-test"])
	tea_util.AssertNotNil(t, headers["date"])
	tea_util.AssertEqual(t, "application/json", headers["accept"])
	tea_util.AssertNotNil(t, headers["x-acs-signature-nonce"])
	tea_util.AssertEqual(t, "HMAC-SHA1", headers["x-acs-signature-method"])
	tea_util.AssertEqual(t, "1.0", headers["x-acs-signature-version"])
	tea_util.AssertEqual(t, "ak", headers["x-acs-accesskey-id"])
	tea_util.AssertEqual(t, "token", headers["x-acs-security-token"])
	tea_util.AssertEqual(t, "2022-06-01", headers["x-acs-version"])
	tea_util.AssertEqual(t, "TestAPI", headers["x-acs-action"])
	tea_util.AssertEqual(t, "application/x-www-form-urlencoded", headers["content-type"])
	tea_util.AssertEqual(t, "A45EE076-334D-5012-9746-A8F828D20FD4", headers["x-acs-request-id"])

	body, _err := util.AssertAsMap(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", body["AppId"])
	tea_util.AssertEqual(t, "test", body["ClassId"])
	tea_util.AssertEqual(t, "123", body["UserId"].(json.Number).String())
}

func TestCallApiForROAWithV2Sign_Anonymous_JSON(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	var server *http.Server
	server = &http.Server{
		Addr:         ":9004",
		WriteTimeout: time.Second * 4,
		Handler:      mux,
	}
	go server.ListenAndServe()
	config := CreateConfig()
	runtime := CreateRuntimeOptions()
	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9004")
	client, _err := NewClient(config)
	tea_util.AssertNil(t, _err)
	request := CreateOpenApiRequest()
	params := &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String("/test"),
		Method:      tea.String("POST"),
		AuthType:    tea.String("Anonymous"),
		Style:       tea.String("ROA"),
		ReqBodyType: tea.String("json"),
		BodyType:    tea.String("json"),
	}
	result, _err := client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err := util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "{\"key1\":\"value\",\"key2\":1,\"key3\":true}", headers["raw-body"])
	tea_util.AssertEqual(t, "global-query=global-value&key1=value&key2=1&key3=true", headers["raw-query"])
	str, _ := util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertEqual(t, "sdk", headers["for-test"])
	tea_util.AssertNotNil(t, headers["date"])
	tea_util.AssertEqual(t, "application/json", headers["accept"])
	tea_util.AssertNotNil(t, headers["x-acs-signature-nonce"])
	tea_util.AssertEqual(t, "HMAC-SHA1", headers["x-acs-signature-method"])
	tea_util.AssertEqual(t, "1.0", headers["x-acs-signature-version"])
	tea_util.AssertEqual(t, "2022-06-01", headers["x-acs-version"])
	tea_util.AssertEqual(t, "TestAPI", headers["x-acs-action"])
	tea_util.AssertEqual(t, "application/json; charset=utf-8", headers["content-type"])
	tea_util.AssertEqual(t, "A45EE076-334D-5012-9746-A8F828D20FD4", headers["x-acs-request-id"])

	body, _err := util.AssertAsMap(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", body["AppId"])
	tea_util.AssertEqual(t, "test", body["ClassId"])
	tea_util.AssertEqual(t, "123", body["UserId"].(json.Number).String())
}

func TestCallApiForRPCWithV3Sign_AK_Form(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/", &mockHandler{content: "json"})
	var server *http.Server
	server = &http.Server{
		Addr:         ":9005",
		WriteTimeout: time.Second * 4,
		Handler:      mux,
	}
	go server.ListenAndServe()
	config := CreateConfig()
	runtime := CreateRuntimeOptions()
	config.Protocol = tea.String("HTTP")
	config.Endpoint = tea.String("127.0.0.1:9005")
	client, _err := NewClient(config)
	tea_util.AssertNil(t, _err)
	request := CreateOpenApiRequest()
	params := &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String("/"),
		Method:      tea.String("POST"),
		AuthType:    tea.String("AK"),
		Style:       tea.String("RPC"),
		ReqBodyType: tea.String("formData"),
		BodyType:    tea.String("json"),
	}
	result, _err := client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err := util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "key1=value&key2=1&key3=true", headers["raw-body"])
	tea_util.AssertEqual(t, "global-query=global-value&key1=value&key2=1&key3=true", headers["raw-query"])
	str, _ := util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	str, _ = util.AssertAsString(headers["authorization"])
	has = strings.Contains(tea.StringValue(str), "ACS3-HMAC-SHA256 Credential=ak,"+
		"SignedHeaders=content-type;host;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-date;x-acs-security-token;"+
		"x-acs-signature-nonce;x-acs-version,Signature=")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "sdk", headers["for-test"])
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertNotNil(t, headers["x-acs-date"])
	tea_util.AssertEqual(t, "application/json", headers["accept"])
	tea_util.AssertNotNil(t, headers["x-acs-signature-nonce"])
	tea_util.AssertNotNil(t, headers["x-acs-content-sha256"])
	tea_util.AssertEqual(t, "ak", headers["x-acs-accesskey-id"])
	tea_util.AssertEqual(t, "token", headers["x-acs-security-token"])
	tea_util.AssertEqual(t, "2022-06-01", headers["x-acs-version"])
	tea_util.AssertEqual(t, "TestAPI", headers["x-acs-action"])
	tea_util.AssertEqual(t, "application/x-www-form-urlencoded", headers["content-type"])
	tea_util.AssertEqual(t, "A45EE076-334D-5012-9746-A8F828D20FD4", headers["x-acs-request-id"])

	body, _err := util.AssertAsMap(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", body["AppId"])
	tea_util.AssertEqual(t, "test", body["ClassId"])
	tea_util.AssertEqual(t, "123", body["UserId"].(json.Number).String())
}

func TestCallApiForRPCWithV3Sign_Anonymous_JSON(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/", &mockHandler{content: "json"})
	var server *http.Server
	server = &http.Server{
		Addr:         ":9006",
		WriteTimeout: time.Second * 4,
		Handler:      mux,
	}
	go server.ListenAndServe()
	config := CreateConfig()
	runtime := CreateRuntimeOptions()
	config.Protocol = tea.String("HTTP")
	config.Endpoint = tea.String("127.0.0.1:9006")
	client, _err := NewClient(config)
	tea_util.AssertNil(t, _err)
	request := CreateOpenApiRequest()
	params := &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String("/"),
		Method:      tea.String("POST"),
		AuthType:    tea.String("Anonymous"),
		Style:       tea.String("RPC"),
		ReqBodyType: tea.String("json"),
		BodyType:    tea.String("json"),
	}
	result, _err := client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err := util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "{\"key1\":\"value\",\"key2\":1,\"key3\":true}", headers["raw-body"])
	tea_util.AssertEqual(t, "global-query=global-value&key1=value&key2=1&key3=true", headers["raw-query"])
	str, _ := util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "sdk", headers["for-test"])
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertNotNil(t, headers["x-acs-date"])
	tea_util.AssertEqual(t, "application/json", headers["accept"])
	tea_util.AssertNotNil(t, headers["x-acs-signature-nonce"])
	tea_util.AssertNotNil(t, headers["x-acs-content-sha256"])
	tea_util.AssertEqual(t, "2022-06-01", headers["x-acs-version"])
	tea_util.AssertEqual(t, "TestAPI", headers["x-acs-action"])
	tea_util.AssertEqual(t, "application/json; charset=utf-8", headers["content-type"])
	tea_util.AssertEqual(t, "A45EE076-334D-5012-9746-A8F828D20FD4", headers["x-acs-request-id"])

	body, _err := util.AssertAsMap(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", body["AppId"])
	tea_util.AssertEqual(t, "test", body["ClassId"])
	tea_util.AssertEqual(t, "123", body["UserId"].(json.Number).String())
}

func TestCallApiForROAWithV3Sign_AK_Form(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	var server *http.Server
	server = &http.Server{
		Addr:         ":9007",
		WriteTimeout: time.Second * 4,
		Handler:      mux,
	}
	go server.ListenAndServe()
	config := CreateConfig()
	runtime := CreateRuntimeOptions()
	config.Protocol = tea.String("HTTP")
	config.Endpoint = tea.String("127.0.0.1:9007")
	client, _err := NewClient(config)
	tea_util.AssertNil(t, _err)
	request := CreateOpenApiRequest()
	params := &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String("/test"),
		Method:      tea.String("POST"),
		AuthType:    tea.String("AK"),
		Style:       tea.String("ROA"),
		ReqBodyType: tea.String("formData"),
		BodyType:    tea.String("json"),
	}
	result, _err := client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err := util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "key1=value&key2=1&key3=true", headers["raw-body"])
	tea_util.AssertEqual(t, "global-query=global-value&key1=value&key2=1&key3=true", headers["raw-query"])
	str, _ := util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	str, _ = util.AssertAsString(headers["authorization"])
	has = strings.Contains(tea.StringValue(str), "ACS3-HMAC-SHA256 Credential=ak,"+
		"SignedHeaders=content-type;host;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-date;x-acs-security-token;"+
		"x-acs-signature-nonce;x-acs-version,Signature=")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "sdk", headers["for-test"])
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertNotNil(t, headers["x-acs-date"])
	tea_util.AssertEqual(t, "application/json", headers["accept"])
	tea_util.AssertNotNil(t, headers["x-acs-signature-nonce"])
	tea_util.AssertNotNil(t, headers["x-acs-content-sha256"])
	tea_util.AssertEqual(t, "ak", headers["x-acs-accesskey-id"])
	tea_util.AssertEqual(t, "token", headers["x-acs-security-token"])
	tea_util.AssertEqual(t, "2022-06-01", headers["x-acs-version"])
	tea_util.AssertEqual(t, "TestAPI", headers["x-acs-action"])
	tea_util.AssertEqual(t, "application/x-www-form-urlencoded", headers["content-type"])
	tea_util.AssertEqual(t, "A45EE076-334D-5012-9746-A8F828D20FD4", headers["x-acs-request-id"])

	body, _err := util.AssertAsMap(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", body["AppId"])
	tea_util.AssertEqual(t, "test", body["ClassId"])
	tea_util.AssertEqual(t, "123", body["UserId"].(json.Number).String())
}

func TestCallApiForROAWithV3Sign_Anonymous_JSON(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	var server *http.Server
	server = &http.Server{
		Addr:         ":9008",
		WriteTimeout: time.Second * 4,
		Handler:      mux,
	}
	go server.ListenAndServe()
	config := CreateConfig()
	runtime := CreateRuntimeOptions()
	config.Protocol = tea.String("HTTP")
	config.Endpoint = tea.String("127.0.0.1:9008")
	client, _err := NewClient(config)
	tea_util.AssertNil(t, _err)
	request := CreateOpenApiRequest()
	params := &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String("/test"),
		Method:      tea.String("POST"),
		AuthType:    tea.String("Anonymous"),
		Style:       tea.String("ROA"),
		ReqBodyType: tea.String("json"),
		BodyType:    tea.String("json"),
	}
	result, _err := client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err := util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "{\"key1\":\"value\",\"key2\":1,\"key3\":true}", headers["raw-body"])
	tea_util.AssertEqual(t, "global-query=global-value&key1=value&key2=1&key3=true", headers["raw-query"])
	str, _ := util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "sdk", headers["for-test"])
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertNotNil(t, headers["x-acs-date"])
	tea_util.AssertEqual(t, "application/json", headers["accept"])
	tea_util.AssertNotNil(t, headers["x-acs-signature-nonce"])
	tea_util.AssertNotNil(t, headers["x-acs-content-sha256"])
	tea_util.AssertEqual(t, "2022-06-01", headers["x-acs-version"])
	tea_util.AssertEqual(t, "TestAPI", headers["x-acs-action"])
	tea_util.AssertEqual(t, "application/json; charset=utf-8", headers["content-type"])
	tea_util.AssertEqual(t, "A45EE076-334D-5012-9746-A8F828D20FD4", headers["x-acs-request-id"])

	body, _err := util.AssertAsMap(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", body["AppId"])
	tea_util.AssertEqual(t, "test", body["ClassId"])
	tea_util.AssertEqual(t, "123", body["UserId"].(json.Number).String())
}

func TestResponseBodyType(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	mux.Handle("/testArray", &mockHandler{content: "array"})
	mux.Handle("/testError", &mockHandler{content: "error"})
	var server *http.Server
	server = &http.Server{
		Addr:         ":9009",
		WriteTimeout: time.Second * 4,
		Handler:      mux,
	}
	go server.ListenAndServe()
	config := CreateConfig()
	runtime := CreateRuntimeOptions()
	config.Protocol = tea.String("HTTP")
	config.Endpoint = tea.String("127.0.0.1:9009")
	client, _err := NewClient(config)
	tea_util.AssertNil(t, _err)
	request := CreateOpenApiRequest()
	params := &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String("/test"),
		Method:      tea.String("POST"),
		AuthType:    tea.String("AK"),
		Style:       tea.String("ROA"),
		ReqBodyType: tea.String("formData"),
		BodyType:    tea.String("json"),
	}
	result, _err := client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	body, _err := util.AssertAsMap(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", body["AppId"])
	tea_util.AssertEqual(t, "test", body["ClassId"])
	tea_util.AssertEqual(t, "123", body["UserId"].(json.Number).String())

	params.BodyType = tea.String("array")
	params.Pathname = tea.String("/testArray")
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	bodyArray, _err := util.AssertAsArray(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "AppId", bodyArray[0])
	tea_util.AssertEqual(t, "ClassId", bodyArray[1])
	tea_util.AssertEqual(t, "UserId", bodyArray[2])

	params.BodyType = tea.String("string")
	params.Pathname = tea.String("/test")
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	bodyStr, _err := util.AssertAsString(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}", tea.StringValue(bodyStr))

	params.Pathname = tea.String("/testError")
	tryErr := func() (_e error) {
		defer func() {
			if r := tea.Recover(recover()); r != nil {
				_e = r
			}
		}()
		_, _err = client.CallApi(params, request, runtime)
		if _err != nil {
			return _err
		}
		return nil
	}()

	tea_util.AssertNotNil(t, tryErr)
	error := tryErr.(*tea.SDKError)
	tea_util.AssertEqual(t, "code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4", tea.StringValue(error.Message))
}
