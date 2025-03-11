package client

import (
	"encoding/json"
	"io"
	"io/ioutil"
	"net/http"
	"regexp"
	"strings"
	"testing"
	"time"

	pop "github.com/alibabacloud-go/alibabacloud-gateway-pop/client"
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
		responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"" +
			", \"Description\":\"error description\", \"AccessDeniedDetail\":{}}"
		w.WriteHeader(400)
		w.Write([]byte(responseBody))
	case "error1":
		responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"" +
			", \"Description\":\"error description\", \"AccessDeniedDetail\":{}, \"accessDeniedDetail\":{\"test\": 0}}"
		w.WriteHeader(400)
		w.Write([]byte(responseBody))
	case "error2":
		responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"" +
			", \"Description\":\"error description\", \"accessDeniedDetail\":{\"test\": 0}}"
		w.WriteHeader(400)
		w.Write([]byte(responseBody))
	case "serverError":
		responseBody = "{\"Code\":\"error code\", \"Message\":\"error message\", \"RequestId\":\"A45EE076-334D-5012-9746-A8F828D20FD4\"" +
			", \"Description\":\"error description\", \"accessDeniedDetail\":{\"test\": 0}}"
		w.WriteHeader(500)
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
	globalParameters.String()
	globalParameters.GoString()
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
		Key:                tea.String("config.key"),
		Cert:               tea.String("config.cert"),
		Ca:                 tea.String("config.ca"),
		TlsMinVersion:      tea.String("config.tlsMinVersion"),
	}
	config.SetEndpoint("config.endpoint")
	config.SetEndpointType("public")
	config.SetNetwork("config.network")
	config.SetSuffix("config.suffix")
	config.SetProtocol("config.protocol")
	config.SetMethod("config.method")
	config.SetRegionId("config.regionId")
	config.SetUserAgent("config.userAgent")
	config.SetReadTimeout(3000)
	config.SetConnectTimeout(3000)
	config.SetHttpProxy("config.httpProxy")
	config.SetHttpsProxy("config.httpsProxy")
	config.SetNoProxy("config.noProxy")
	config.SetSocks5Proxy("config.socks5Proxy")
	config.SetSocks5NetWork("config.socks5NetWork")
	config.SetMaxIdleConns(128)
	config.SetSignatureVersion("config.signatureVersion")
	config.SetSignatureAlgorithm("config.signatureAlgorithm")
	config.SetGlobalParameters(globalParameters)
	config.SetKey("config.key")
	config.SetCert("config.cert")
	config.SetCa("config.ca")
	config.SetOpenPlatformEndpoint("openPlatform.aliyuncs.com")
	config.SetDisableHttp2(true)
	config.SetTlsMinVersion("config.tlsMinVersion")

	creConfig := &credential.Config{
		AccessKeyId:     tea.String("accessKeyId"),
		AccessKeySecret: tea.String("accessKeySecret"),
		SecurityToken:   tea.String("securityToken"),
		Type:            tea.String("sts"),
	}
	cred, _err := credential.NewCredential(creConfig)
	tea_util.AssertNil(t, _err)

	config.SetCredential(cred)
	config.String()
	config.GoString()
	client, _err := NewClient(nil)
	tea_util.AssertNotNil(t, _err)
	_err = client.CheckConfig(&Config{})
	tea_util.AssertNotNil(t, _err)
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	ak, _ := client.GetAccessKeyId()
	tea_util.AssertEqual(t, "accessKeyId", tea.StringValue(ak))
	sk, _ := client.GetAccessKeySecret()
	tea_util.AssertEqual(t, "accessKeySecret", tea.StringValue(sk))
	token, _ := client.GetSecurityToken()
	tea_util.AssertEqual(t, "securityToken", tea.StringValue(token))
	ty, _ := client.GetType()
	tea_util.AssertEqual(t, "sts", tea.StringValue(ty))

	// bearer token
	creConfig = &credential.Config{
		BearerToken: tea.String("token"),
		Type:        tea.String("bearer"),
	}
	cred, _err = credential.NewCredential(creConfig)
	tea_util.AssertNil(t, _err)

	config.Credential = cred
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	ak, _ = client.GetAccessKeyId()
	tea_util.AssertEqual(t, "", tea.StringValue(ak))
	sk, _ = client.GetAccessKeySecret()
	tea_util.AssertEqual(t, "", tea.StringValue(sk))
	token, _ = client.GetSecurityToken()
	tea_util.AssertEqual(t, "", tea.StringValue(token))
	bearer, _ := client.GetBearerToken()
	tea_util.AssertEqual(t, "token", tea.StringValue(bearer))
	ty, _ = client.GetType()
	tea_util.AssertEqual(t, "bearer", tea.StringValue(ty))

	config.SetAccessKeyId("ak")
	config.SetAccessKeySecret("secret")
	config.SetSecurityToken("token")
	config.SetType("sts")
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

	config.SetType("bearer")
	config.SetBearerToken("token")
	config.SetAccessKeyId("")
	config.SetAccessKeySecret("")
	config.SetSecurityToken("")
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	ak, _ = client.GetAccessKeyId()
	tea_util.AssertEqual(t, "", tea.StringValue(ak))
	sk, _ = client.GetAccessKeySecret()
	tea_util.AssertEqual(t, "", tea.StringValue(sk))
	token, _ = client.GetSecurityToken()
	tea_util.AssertEqual(t, "", tea.StringValue(token))
	token, _ = client.GetBearerToken()
	tea_util.AssertEqual(t, "token", tea.StringValue(token))
	ty, _ = client.GetType()
	tea_util.AssertEqual(t, "bearer", tea.StringValue(ty))

	config.AccessKeyId = tea.String("ak")
	config.AccessKeySecret = tea.String("secret")
	config.Type = tea.String("access_key")
	config.SecurityToken = nil
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	ak, _ = client.GetAccessKeyId()
	tea_util.AssertEqual(t, "ak", tea.StringValue(ak))
	sk, _ = client.GetAccessKeySecret()
	tea_util.AssertEqual(t, "secret", tea.StringValue(sk))
	ty, _ = client.GetType()
	tea_util.AssertEqual(t, "access_key", tea.StringValue(ty))
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
	tea_util.AssertEqual(t, "config.key", tea.StringValue(client.Key))
	tea_util.AssertEqual(t, "config.cert", tea.StringValue(client.Cert))
	tea_util.AssertEqual(t, "config.ca", tea.StringValue(client.Ca))
	tea_util.AssertEqual(t, true, tea.BoolValue(client.DisableHttp2))
	tea_util.AssertEqual(t, "config.tlsMinVersion", tea.StringValue(client.TlsMinVersion))

	globalParameters.SetHeaders(map[string]*string{
		"global-key": tea.String("test"),
	})
	globalParameters.SetQueries(map[string]*string{
		"global-query": tea.String("test"),
	})
	tea_util.AssertEqual(t, "test", tea.StringValue(client.GlobalParameters.Headers["global-key"]))
	tea_util.AssertEqual(t, "test", tea.StringValue(client.GlobalParameters.Queries["global-query"]))
	_err = client.CheckConfig(config)
	tea_util.AssertNil(t, _err)
}

func TestOpenApiRequest(t *testing.T) {
	query := map[string]*string{
		"key": tea.String("value"),
	}
	body := map[string]interface{}{
		"key": tea.String("value"),
	}
	headers := map[string]*string{
		"key": tea.String("value"),
	}
	hostMap := map[string]*string{
		"key": tea.String("value"),
	}
	req := &OpenApiRequest{}
	req.SetQuery(query)
	req.SetBody(body)
	req.SetHeaders(headers)
	req.SetHostMap(hostMap)
	req.SetEndpointOverride("test")
	req.SetStream(strings.NewReader("test"))
	req.GoString()
	req.String()

	tea_util.AssertEqual(t, "value", tea.StringValue(req.Headers["key"]))
	tea_util.AssertEqual(t, "value", tea.StringValue(req.Query["key"]))
	tea_util.AssertEqual(t, body, req.Body)
	tea_util.AssertEqual(t, "test", tea.StringValue(req.EndpointOverride))
	tea_util.AssertEqual(t, "value", tea.StringValue(req.HostMap["key"]))
	byt, _ := ioutil.ReadAll(req.Stream)
	tea_util.AssertEqual(t, "test", string(byt))
}

func TestParams(t *testing.T) {
	params := &Params{}
	params.SetAction("test")
	params.SetVersion("test")
	params.SetProtocol("test")
	params.SetPathname("test")
	params.SetMethod("test")
	params.SetAuthType("test")
	params.SetBodyType("test")
	params.SetReqBodyType("test")
	params.SetStyle("test")
	params.GoString()
	params.String()

	tea_util.AssertEqual(t, "test", tea.StringValue(params.Action))
	tea_util.AssertEqual(t, "test", tea.StringValue(params.Version))
	tea_util.AssertEqual(t, "test", tea.StringValue(params.Protocol))
	tea_util.AssertEqual(t, "test", tea.StringValue(params.Pathname))
	tea_util.AssertEqual(t, "test", tea.StringValue(params.Method))
	tea_util.AssertEqual(t, "test", tea.StringValue(params.AuthType))
	tea_util.AssertEqual(t, "test", tea.StringValue(params.BodyType))
	tea_util.AssertEqual(t, "test", tea.StringValue(params.ReqBodyType))
	tea_util.AssertEqual(t, "test", tea.StringValue(params.Style))
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
		TlsMinVersion:      tea.String("TLSv1.2"),
	}
	_result = config
	return _result
}

func CreateBearerTokenConfig() (_result *Config) {
	creConfig := &credential.Config{
		BearerToken: tea.String("token"),
		Type:        tea.String("bearer"),
	}
	cred, _err := credential.NewCredential(creConfig)
	if _err != nil {
		return nil
	}
	config := &Config{
		Credential: cred,
	}
	_result = config
	return _result
}

func CreateAnonymousConfig() (_result *Config) {
	return &Config{}
}

func CreateRuntimeOptions() (_result *util.RuntimeOptions) {
	extendsParameters := &util.ExtendsParameters{
		Headers: map[string]*string{
			"extends-key": tea.String("extends-value"),
		},
		Queries: map[string]*string{
			"extends-key": tea.String("extends-value"),
		},
	}
	runtime := &util.RuntimeOptions{
		ReadTimeout:       tea.Int(4000),
		ConnectTimeout:    tea.Int(4000),
		MaxIdleConns:      tea.Int(100),
		Autoretry:         tea.Bool(true),
		MaxAttempts:       tea.Int(1),
		BackoffPolicy:     tea.String("no"),
		BackoffPeriod:     tea.Int(1),
		IgnoreSSL:         tea.Bool(true),
		ExtendsParameters: extendsParameters,
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
		"&extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=true")
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
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	// bearer token
	config = CreateBearerTokenConfig()
	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9001")
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	regx, _ = regexp.Compile("Action=TestAPI&BearerToken=token&Format=json" +
		"&SignatureNonce=.+&SignatureType=BEARERTOKEN&Timestamp=.+&Version=2022-06-01" +
		"&extends-key=extends-value&key1=value&key2=1&key3=true")
	str, _ = util.AssertAsString(headers["raw-query"])
	find = regx.FindAllString(tea.StringValue(str), -1)
	tea_util.AssertNotNil(t, find)
	// tea_util.AssertEqual(t, "bearer token", headers["authorization"])

	// Anonymous error
	config = CreateAnonymousConfig()
	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9001")
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	_, _err = client.CallApi(params, request, runtime)
	err := _err.(*tea.SDKError)
	tea_util.AssertEqual(t, "InvalidCredentials", tea.StringValue(err.Code))
	tea_util.AssertEqual(t, "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.", tea.StringValue(err.Message))
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
		"&extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=true")
	str, _ := util.AssertAsString(headers["raw-query"])
	find := regx.FindAllString(tea.StringValue(str), -1)
	tea_util.AssertNotNil(t, find)
	str, _ = util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertEqual(t, "extends-value", headers["extends-key"])
	tea_util.AssertEqual(t, "2022-06-01", headers["x-acs-version"])
	tea_util.AssertEqual(t, "TestAPI", headers["x-acs-action"])
	tea_util.AssertEqual(t, "application/x-www-form-urlencoded", headers["content-type"])
	tea_util.AssertEqual(t, "A45EE076-334D-5012-9746-A8F828D20FD4", headers["x-acs-request-id"])

	body, _err := util.AssertAsMap(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", body["AppId"])
	tea_util.AssertEqual(t, "test", body["ClassId"])
	tea_util.AssertEqual(t, "123", body["UserId"].(json.Number).String())
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	client.SetRpcHeaders(map[string]*string{
		"extends-key": tea.String("test"),
	})
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", headers["extends-key"])
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
	tea_util.AssertEqual(t, "extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=true", headers["raw-query"])
	str, _ := util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	str, _ = util.AssertAsString(headers["authorization"])
	has = strings.Contains(tea.StringValue(str), "acs ak:")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertEqual(t, "extends-value", headers["extends-key"])
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
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	client.SetRpcHeaders(map[string]*string{
		"extends-key": tea.String("test"),
	})
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "extends-value", headers["extends-key"])

	// bearer token
	config = CreateBearerTokenConfig()
	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9003")
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	// tea_util.AssertEqual(t, "bearer token", headers["authorization"])
	tea_util.AssertEqual(t, "token", headers["x-acs-bearer-token"])
	tea_util.AssertEqual(t, "BEARERTOKEN", headers["x-acs-signature-type"])

	// Anonymous error
	config = CreateAnonymousConfig()
	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9003")
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	_, _err = client.CallApi(params, request, runtime)
	err := _err.(*tea.SDKError)
	tea_util.AssertEqual(t, "InvalidCredentials", tea.StringValue(err.Code))
	tea_util.AssertEqual(t, "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.", tea.StringValue(err.Message))

	params = &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String("/test"),
		Method:      tea.String("POST"),
		AuthType:    tea.String("AK"),
		Style:       tea.String("ROA"),
		ReqBodyType: tea.String("json"),
		BodyType:    tea.String("json"),
	}
	// bearer token
	config = CreateBearerTokenConfig()
	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9003")
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	// tea_util.AssertEqual(t, "bearer token", headers["authorization"])
	tea_util.AssertEqual(t, "token", headers["x-acs-bearer-token"])
	tea_util.AssertEqual(t, "BEARERTOKEN", headers["x-acs-signature-type"])

	// Anonymous error
	config = CreateAnonymousConfig()
	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9003")
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	_, _err = client.CallApi(params, request, runtime)
	err = _err.(*tea.SDKError)
	tea_util.AssertEqual(t, "InvalidCredentials", tea.StringValue(err.Code))
	tea_util.AssertEqual(t, "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.", tea.StringValue(err.Message))

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
	tea_util.AssertEqual(t, "extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=true", headers["raw-query"])
	str, _ := util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertEqual(t, "extends-value", headers["extends-key"])
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
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	client.SetRpcHeaders(map[string]*string{
		"extends-key": tea.String("test"),
	})
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "extends-value", headers["extends-key"])
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
	tea_util.AssertEqual(t, "extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=true", headers["raw-query"])
	str, _ := util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	str, _ = util.AssertAsString(headers["authorization"])
	has = strings.Contains(tea.StringValue(str), "ACS3-HMAC-SHA256 Credential=ak,"+
		"SignedHeaders=content-type;host;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-credentials-provider;x-acs-date;x-acs-security-token;"+
		"x-acs-signature-nonce;x-acs-version,Signature=")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "sdk", headers["for-test"])
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertEqual(t, "extends-value", headers["extends-key"])
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
	tea_util.AssertEqual(t, "static_sts", headers["x-acs-credentials-provider"])

	body, _err := util.AssertAsMap(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", body["AppId"])
	tea_util.AssertEqual(t, "test", body["ClassId"])
	tea_util.AssertEqual(t, "123", body["UserId"].(json.Number).String())
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	client.SetRpcHeaders(map[string]*string{
		"extends-key": tea.String("test"),
	})
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", headers["extends-key"])

	// bearer token
	config = CreateBearerTokenConfig()
	config.Protocol = tea.String("HTTP")
	config.Endpoint = tea.String("127.0.0.1:9005")
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	// tea_util.AssertEqual(t, "bearer token", headers["authorization"])
	tea_util.AssertEqual(t, "token", headers["x-acs-bearer-token"])
	tea_util.AssertEqual(t, "SignatureType=BEARERTOKEN&extends-key=extends-value&key1=value&key2=1&key3=true", headers["raw-query"])

	// Anonymous error
	config = CreateAnonymousConfig()
	config.Protocol = tea.String("HTTP")
	config.Endpoint = tea.String("127.0.0.1:9005")
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	_, _err = client.CallApi(params, request, runtime)
	err := _err.(*tea.SDKError)
	tea_util.AssertEqual(t, "InvalidCredentials", tea.StringValue(err.Code))
	tea_util.AssertEqual(t, "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.", tea.StringValue(err.Message))
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
	tea_util.AssertEqual(t, "extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=true", headers["raw-query"])
	str, _ := util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "sdk", headers["for-test"])
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertEqual(t, "extends-value", headers["extends-key"])
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
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	client.SetRpcHeaders(map[string]*string{
		"extends-key": tea.String("test"),
	})
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", headers["extends-key"])
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
	tea_util.AssertEqual(t, "extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=true", headers["raw-query"])
	str, _ := util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	str, _ = util.AssertAsString(headers["authorization"])
	has = strings.Contains(tea.StringValue(str), "ACS3-HMAC-SHA256 Credential=ak,"+
		"SignedHeaders=content-type;host;x-acs-accesskey-id;x-acs-action;x-acs-content-sha256;x-acs-credentials-provider;x-acs-date;x-acs-security-token;"+
		"x-acs-signature-nonce;x-acs-version,Signature=")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "sdk", headers["for-test"])
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertEqual(t, "extends-value", headers["extends-key"])
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
	tea_util.AssertEqual(t, "static_sts", headers["x-acs-credentials-provider"])

	body, _err := util.AssertAsMap(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test", body["AppId"])
	tea_util.AssertEqual(t, "test", body["ClassId"])
	tea_util.AssertEqual(t, "123", body["UserId"].(json.Number).String())
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	client.SetRpcHeaders(map[string]*string{
		"extends-key": tea.String("test"),
	})
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "extends-value", headers["extends-key"])

	// bearer token
	config = CreateBearerTokenConfig()
	config.Protocol = tea.String("HTTP")
	config.Endpoint = tea.String("127.0.0.1:9007")
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	// tea_util.AssertEqual(t, "bearer token", headers["authorization"])
	tea_util.AssertEqual(t, "token", headers["x-acs-bearer-token"])
	tea_util.AssertEqual(t, "BEARERTOKEN", headers["x-acs-signature-type"])

	// Anonymous error
	config = CreateAnonymousConfig()
	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9003")
	client, _err = NewClient(config)
	tea_util.AssertNil(t, _err)
	_, _err = client.CallApi(params, request, runtime)
	err := _err.(*tea.SDKError)
	tea_util.AssertEqual(t, "InvalidCredentials", tea.StringValue(err.Code))
	tea_util.AssertEqual(t, "Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.", tea.StringValue(err.Message))
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
	tea_util.AssertEqual(t, "extends-key=extends-value&global-query=global-value&key1=value&key2=1&key3=true", headers["raw-query"])
	str, _ := util.AssertAsString(headers["user-agent"])
	has := strings.Contains(tea.StringValue(str), "TeaDSL/1 config.userAgent")
	tea_util.AssertEqual(t, true, has)
	tea_util.AssertEqual(t, "sdk", headers["for-test"])
	tea_util.AssertEqual(t, "global-value", headers["global-key"])
	tea_util.AssertEqual(t, "extends-value", headers["extends-key"])
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
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	client.SetRpcHeaders(map[string]*string{
		"extends-key": tea.String("test"),
	})
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "extends-value", headers["extends-key"])
}

func TestResponseBodyType(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	mux.Handle("/testArray", &mockHandler{content: "array"})
	mux.Handle("/testError", &mockHandler{content: "error"})
	mux.Handle("/testError1", &mockHandler{content: "error1"})
	mux.Handle("/testError2", &mockHandler{content: "error2"})
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
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	params.BodyType = tea.String("array")
	params.Pathname = tea.String("/testArray")
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	bodyArray, _err := util.AssertAsArray(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "AppId", bodyArray[0])
	tea_util.AssertEqual(t, "ClassId", bodyArray[1])
	tea_util.AssertEqual(t, "UserId", bodyArray[2])
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	params.BodyType = tea.String("string")
	params.Pathname = tea.String("/test")
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	bodyStr, _err := util.AssertAsString(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}", tea.StringValue(bodyStr))
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	params.BodyType = tea.String("binary")
	params.Pathname = tea.String("/test")
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	bodyStr, _err = util.ReadAsString(result["body"].(io.Reader))
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
	err := tryErr.(*tea.SDKError)
	tea_util.AssertEqual(t, "code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4", tea.StringValue(err.Message))

	tea_util.AssertEqual(t, 400, tea.IntValue(err.StatusCode))
	tea_util.AssertNil(t, err.AccessDeniedDetail["test"])

	params.Pathname = tea.String("/testError1")
	tryErr = func() (_e error) {
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
	err = tryErr.(*tea.SDKError)
	tea_util.AssertEqual(t, "code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4", tea.StringValue(err.Message))

	tea_util.AssertEqual(t, 400, tea.IntValue(err.StatusCode))
	tea_util.AssertNil(t, err.AccessDeniedDetail["test"])

	params.Pathname = tea.String("/testError2")
	tryErr = func() (_e error) {
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
	err = tryErr.(*tea.SDKError)
	tea_util.AssertEqual(t, "code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4", tea.StringValue(err.Message))

	tea_util.AssertEqual(t, 400, tea.IntValue(err.StatusCode))
	accessDeniedDetail, _ := err.AccessDeniedDetail["test"].(int)
	tea_util.AssertEqual(t, 0, accessDeniedDetail)
}

func TestRequestBodyType(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	var server *http.Server
	server = &http.Server{
		Addr:         ":9010",
		WriteTimeout: time.Second * 4,
		Handler:      mux,
	}
	go server.ListenAndServe()
	config := CreateConfig()
	runtime := CreateRuntimeOptions()
	config.Protocol = tea.String("HTTP")
	config.Endpoint = tea.String("127.0.0.1:9010")
	client, _err := NewClient(config)
	tea_util.AssertNil(t, _err)
	// formData
	params := &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String("/test"),
		Method:      tea.String("POST"),
		AuthType:    tea.String("AK"),
		Style:       tea.String("RPC"),
		ReqBodyType: tea.String("formData"),
		BodyType:    tea.String("json"),
	}
	body := map[string]interface{}{}
	body["key1"] = tea.String("value")
	body["key2"] = tea.Int(1)
	body["key3"] = tea.Bool(true)
	request := &OpenApiRequest{
		Body: openapiutil.ParseToMap(body),
	}
	result, _err := client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err := util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "key1=value&key2=1&key3=true", headers["raw-body"])
	tea_util.AssertEqual(t, "application/x-www-form-urlencoded", headers["content-type"])

	// json
	params.ReqBodyType = tea.String("json")
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "{\"key1\":\"value\",\"key2\":1,\"key3\":true}", headers["raw-body"])
	tea_util.AssertEqual(t, "application/json; charset=utf-8", headers["content-type"])

	// byte
	params.ReqBodyType = tea.String("byte")
	byteBody := util.ToBytes(tea.String("test byte"))
	request = &OpenApiRequest{
		Body: byteBody,
	}
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test byte", headers["raw-body"])
	tea_util.AssertEqual(t, "text/plain; charset=utf-8", headers["content-type"])

	// stream
	params.ReqBodyType = tea.String("binary")
	streamBody := strings.NewReader("test byte")
	request = &OpenApiRequest{
		Stream: streamBody,
	}
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)

	headers, _err = util.AssertAsMap(result["headers"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "test byte", headers["raw-body"])
	tea_util.AssertEqual(t, "application/octet-stream", headers["content-type"])
}

func TestResponseBodyTypeRPC(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/", &mockHandler{content: "string"})
	var server *http.Server
	server = &http.Server{
		Addr:         ":9011",
		WriteTimeout: time.Second * 4,
		Handler:      mux,
	}
	go server.ListenAndServe()
	config := CreateConfig()
	runtime := CreateRuntimeOptions()
	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9011")
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
		Style:       tea.String("RPC"),
		ReqBodyType: tea.String("formData"),
		BodyType:    tea.String("json"),
	}

	params.BodyType = tea.String("string")
	result, _err := client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	bodyStr, _err := util.AssertAsString(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}", tea.StringValue(bodyStr))
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	params.BodyType = tea.String("binary")
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	bodyStr, _err = util.ReadAsString(result["body"].(io.Reader))
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}", tea.StringValue(bodyStr))
}

func TestResponseBodyTypeROA(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/test", &mockHandler{content: "json"})
	mux.Handle("/testArray", &mockHandler{content: "array"})
	mux.Handle("/testError", &mockHandler{content: "error"})
	mux.Handle("/testError1", &mockHandler{content: "error1"})
	mux.Handle("/testError2", &mockHandler{content: "error2"})
	var server *http.Server
	server = &http.Server{
		Addr:         ":9012",
		WriteTimeout: time.Second * 4,
		Handler:      mux,
	}
	go server.ListenAndServe()
	config := CreateConfig()
	runtime := CreateRuntimeOptions()
	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9012")
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
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	params.BodyType = tea.String("array")
	params.Pathname = tea.String("/testArray")
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	bodyArray, _err := util.AssertAsArray(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "AppId", bodyArray[0])
	tea_util.AssertEqual(t, "ClassId", bodyArray[1])
	tea_util.AssertEqual(t, "UserId", bodyArray[2])
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	params.BodyType = tea.String("string")
	params.Pathname = tea.String("/test")
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	bodyStr, _err := util.AssertAsString(result["body"])
	tea_util.AssertNil(t, _err)
	tea_util.AssertEqual(t, "{\"AppId\":\"test\", \"ClassId\":\"test\", \"UserId\":123}", tea.StringValue(bodyStr))
	tea_util.AssertEqual(t, "200", result["statusCode"].(json.Number).String())

	params.BodyType = tea.String("binary")
	params.Pathname = tea.String("/test")
	result, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNil(t, _err)
	bodyStr, _err = util.ReadAsString(result["body"].(io.Reader))
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
	err := tryErr.(*tea.SDKError)
	tea_util.AssertEqual(t, "code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4", tea.StringValue(err.Message))

	tea_util.AssertEqual(t, 400, tea.IntValue(err.StatusCode))
	tea_util.AssertNil(t, err.AccessDeniedDetail["test"])

	params.Pathname = tea.String("/testError1")
	tryErr = func() (_e error) {
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
	err = tryErr.(*tea.SDKError)
	tea_util.AssertEqual(t, "code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4", tea.StringValue(err.Message))

	tea_util.AssertEqual(t, 400, tea.IntValue(err.StatusCode))
	tea_util.AssertNil(t, err.AccessDeniedDetail["test"])

	params.Pathname = tea.String("/testError2")
	tryErr = func() (_e error) {
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
	err = tryErr.(*tea.SDKError)
	tea_util.AssertEqual(t, "code: 400, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4", tea.StringValue(err.Message))

	tea_util.AssertEqual(t, 400, tea.IntValue(err.StatusCode))
	accessDeniedDetail, _ := err.AccessDeniedDetail["test"].(int)
	tea_util.AssertEqual(t, 0, accessDeniedDetail)
}

func TestRetryWithError(t *testing.T) {
	mux := http.NewServeMux()
	mux.Handle("/", &mockHandler{content: "serverError"})
	mux.Handle("/test", &mockHandler{content: "serverError"})
	var server *http.Server
	server = &http.Server{
		Addr:         ":9013",
		WriteTimeout: time.Second * 4,
		Handler:      mux,
	}
	go server.ListenAndServe()
	config := CreateConfig()
	runtime := CreateRuntimeOptions()
	runtime.Autoretry = tea.Bool(true)
	runtime.MaxAttempts = tea.Int(3)
	runtime.BackoffPolicy = tea.String("fix")
	runtime.BackoffPeriod = tea.Int(1)

	config.Protocol = tea.String("HTTP")
	config.SignatureAlgorithm = tea.String("v2")
	config.Endpoint = tea.String("127.0.0.1:9013")
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
		ReqBodyType: tea.String("formData"),
		BodyType:    tea.String("json"),
	}
	_, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNotNil(t, _err)
	err := _err.(*tea.SDKError)
	tea_util.AssertEqual(t, "code: 500, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4", tea.StringValue(err.Message))

	params = &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String("/test"),
		Method:      tea.String("POST"),
		AuthType:    tea.String("Anonymous"),
		Style:       tea.String("ROA"),
		ReqBodyType: tea.String("formData"),
		BodyType:    tea.String("json"),
	}
	_, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNotNil(t, _err)
	err = _err.(*tea.SDKError)
	tea_util.AssertEqual(t, "code: 500, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4", tea.StringValue(err.Message))

	params = &Params{
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
	_, _err = client.CallApi(params, request, runtime)
	tea_util.AssertNotNil(t, _err)
	err = _err.(*tea.SDKError)
	tea_util.AssertEqual(t, "code: 500, error message request id: A45EE076-334D-5012-9746-A8F828D20FD4", tea.StringValue(err.Message))

	params = &Params{
		Action:      tea.String("TestAPI"),
		Version:     tea.String("2022-06-01"),
		Protocol:    tea.String("HTTPS"),
		Pathname:    tea.String("/test"),
		Method:      tea.String("POST"),
		AuthType:    tea.String("Anonymous"),
		Style:       tea.String("RPC"),
		ReqBodyType: tea.String("json"),
		BodyType:    tea.String("json"),
	}
	client.ProductId = tea.String("test")
	gatewayClient, _err := pop.NewClient()
	tea_util.AssertNil(t, _err)
	client.SetGatewayClient(gatewayClient)
	_, _err = client.Execute(params, request, runtime)
	tea_util.AssertNotNil(t, _err)
}
