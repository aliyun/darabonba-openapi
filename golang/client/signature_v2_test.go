package client

import (
	"context"
	"crypto/hmac"
	"crypto/sha256"
	"encoding/base64"
	"fmt"
	"io/ioutil"
	"net/http"
	"net/url"
	"sort"
	"strings"
	"testing"

	"github.com/alibabacloud-go/tea/dara"
)

func TestSignatureV2Requests(t *testing.T) {
	for _, style := range []string{"RPC", "ROA-json", "ROA-form"} {
		for _, withContext := range []bool{false, true} {
			for _, requestedSHA1 := range []bool{false, true} {
				t.Run(fmt.Sprintf("%s/context=%v/requestedSHA1=%v", style, withContext, requestedSHA1), func(t *testing.T) {
					config := CreateConfig()
					config.Protocol = dara.String("HTTP")
					config.Endpoint = dara.String("example.com")
					headerMethod := "HMAC-SHA256"
					if style == "RPC" {
						headerMethod = ""
					}
					if requestedSHA1 {
						headerMethod = "HMAC-SHA1"
					}
					config.HttpClient = &signatureV2Verifier{t: t, style: style, method: "HMAC-SHA256", headerMethod: headerMethod}
					config.SignatureAlgorithm = dara.String("v2")
					if requestedSHA1 {
						config.GlobalParameters.Headers["x-acs-signature-method"] = dara.String("HMAC-SHA1")
					}
					client, err := NewClient(config)
					if err != nil {
						t.Fatal(err)
					}
					request := CreateOpenApiRequest()
					request.Body = nil
					if requestedSHA1 {
						request.Headers["x-acs-signature-method"] = dara.String("HMAC-SHA1")
						request.Query["SignatureMethod"] = dara.String("HMAC-SHA1")
					}
					params := coverageParams("RPC", "formData", "json", "/", "AK")
					if style != "RPC" {
						bodyType := "json"
						if style == "ROA-form" {
							bodyType = "formData"
						}
						params = coverageParams("ROA", bodyType, "json", "/test", "AK")
					}
					if withContext {
						_, err = client.CallApiWithCtx(context.Background(), params, request, CreateRuntimeOptions())
					} else {
						_, err = client.CallApi(params, request, CreateRuntimeOptions())
					}
					if err != nil {
						t.Fatal(err)
					}
				})
			}
		}
	}
}

func TestSignatureV2RpcPreservesBodyAlgorithm(t *testing.T) {
	for _, withContext := range []bool{false, true} {
		t.Run(fmt.Sprintf("context=%v", withContext), func(t *testing.T) {
			config := CreateConfig()
			config.Protocol = dara.String("HTTP")
			config.Endpoint = dara.String("example.com")
			config.SignatureAlgorithm = dara.String("v2")
			config.HttpClient = &signatureV2Verifier{t: t, style: "RPC", method: "HMAC-SHA256", bodyMethod: "HMAC-SHA1"}
			client, err := NewClient(config)
			if err != nil {
				t.Fatal(err)
			}
			request := CreateOpenApiRequest()
			body := map[string]interface{}{"SignatureMethod": "HMAC-SHA1", "Key": "value"}
			request.Body = body
			params := coverageParams("RPC", "formData", "json", "/", "AK")
			if withContext {
				_, err = client.CallApiWithCtx(context.Background(), params, request, CreateRuntimeOptions())
			} else {
				_, err = client.CallApi(params, request, CreateRuntimeOptions())
			}
			if err != nil {
				t.Fatal(err)
			}
			if body["SignatureMethod"] != "HMAC-SHA1" {
				t.Fatal("caller body was modified")
			}
		})
	}
}

type signatureV2Verifier struct {
	t            *testing.T
	style        string
	method       string
	headerMethod string
	bodyMethod   string
}

type signatureV2HeaderCapture struct {
	method string
}

func (c *signatureV2HeaderCapture) Call(req *http.Request, transport *http.Transport) (*http.Response, error) {
	c.method = signatureTestHeader(req.Header, "x-acs-signature-method")
	return &http.Response{
		StatusCode: http.StatusOK,
		Header:     http.Header{"Content-Type": []string{"application/json"}},
		Body:       ioutil.NopCloser(strings.NewReader(`{}`)),
	}, nil
}

func TestSignatureV2RoaNonHmacHeadersUnchanged(t *testing.T) {
	for _, authType := range []string{"bearer", "Anonymous"} {
		t.Run(authType, func(t *testing.T) {
			config := CreateConfig()
			if authType == "bearer" {
				config = CreateBearerTokenConfig()
			}
			config.Protocol = dara.String("HTTP")
			config.Endpoint = dara.String("example.com")
			config.SignatureAlgorithm = dara.String("v2")
			capture := &signatureV2HeaderCapture{}
			config.HttpClient = capture
			client, err := NewClient(config)
			if err != nil {
				t.Fatal(err)
			}
			params := coverageParams("ROA", "json", "json", "/test", "AK")
			if authType == "Anonymous" {
				params.AuthType = dara.String("Anonymous")
			}
			request := CreateOpenApiRequest()
			request.Body = nil
			if _, err := client.CallApi(params, request, CreateRuntimeOptions()); err != nil {
				t.Fatal(err)
			}
			if capture.method != "HMAC-SHA256" {
				t.Fatalf("signature method = %q", capture.method)
			}
		})
	}
}

func (v *signatureV2Verifier) Call(req *http.Request, transport *http.Transport) (*http.Response, error) {
	verifyV2Request(v.t, req, v.style, v.method, v.headerMethod, v.bodyMethod)
	return &http.Response{
		StatusCode: http.StatusOK,
		Header:     http.Header{"Content-Type": []string{"application/json"}},
		Body:       ioutil.NopCloser(strings.NewReader(`{}`)),
	}, nil
}

func verifyV2Request(t *testing.T, request *http.Request, style, method, headerMethod, bodyMethod string) {
	t.Helper()
	query := request.URL.Query()
	var signature, stringToSign, key string
	if style == "RPC" {
		if actual := signatureTestHeader(request.Header, "x-acs-signature-method"); actual != headerMethod {
			t.Errorf("signature method header = %q, want %q", actual, headerMethod)
		}
		if query.Get("SignatureMethod") != method {
			t.Errorf("signature method = %q, want %q", query.Get("SignatureMethod"), method)
		}
		signature = query.Get("Signature")
		query.Del("Signature")
		if request.Body != nil {
			body, err := ioutil.ReadAll(request.Body)
			if err != nil {
				t.Fatal(err)
			}
			if len(body) > 0 {
				form, err := url.ParseQuery(string(body))
				if err != nil {
					t.Fatal(err)
				}
				if bodyMethod != "" && form.Get("SignatureMethod") != bodyMethod {
					t.Errorf("signature method body = %q, want %q", form.Get("SignatureMethod"), bodyMethod)
				}
				for key, values := range form {
					query[key] = values
				}
			}
		}
		canonical := strings.ReplaceAll(query.Encode(), "+", "%20")
		canonical = strings.ReplaceAll(canonical, "*", "%2A")
		canonical = strings.ReplaceAll(canonical, "%7E", "~")
		stringToSign = request.Method + "&%2F&" + url.QueryEscape(canonical)
		key = "secret&"
	} else {
		if actual := signatureTestHeader(request.Header, "x-acs-signature-method"); actual != headerMethod {
			t.Errorf("signature method header = %q, want %q", actual, headerMethod)
		}
		signature = strings.TrimPrefix(signatureTestHeader(request.Header, "authorization"), "acs ak:")
		var keys []string
		for key := range request.Header {
			key = strings.ToLower(key)
			if strings.HasPrefix(key, "x-acs-") {
				keys = append(keys, key)
			}
		}
		sort.Strings(keys)
		var canonicalHeaders strings.Builder
		for _, key := range keys {
			canonicalHeaders.WriteString(key + ":" + signatureTestHeader(request.Header, key) + "\n")
		}
		resource := request.URL.Path
		var queryKeys []string
		for key := range query {
			queryKeys = append(queryKeys, key)
		}
		sort.Strings(queryKeys)
		for i, key := range queryKeys {
			if i == 0 {
				resource += "?"
			} else {
				resource += "&"
			}
			resource += key
			if query.Get(key) != "" {
				resource += "=" + query.Get(key)
			}
		}
		stringToSign = request.Method + "\n" + signatureTestHeader(request.Header, "accept") + "\n" + signatureTestHeader(request.Header, "content-md5") + "\n" + signatureTestHeader(request.Header, "content-type") + "\n" + signatureTestHeader(request.Header, "date") + "\n" + canonicalHeaders.String() + resource
		key = "secret"
	}
	mac := hmac.New(sha256.New, []byte(key))
	mac.Write([]byte(stringToSign))
	want := base64.StdEncoding.EncodeToString(mac.Sum(nil))
	if signature != want {
		t.Errorf("signature does not match %s", method)
	}
}

func signatureTestHeader(headers http.Header, name string) string {
	for key, values := range headers {
		if strings.EqualFold(key, name) && len(values) > 0 {
			return values[0]
		}
	}
	return ""
}
