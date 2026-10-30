package client

import (
	"context"
	"crypto/hmac"
	"crypto/sha1"
	"crypto/sha256"
	"encoding/base64"
	"fmt"
	"hash"
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
		for _, method := range []string{"HMAC-SHA256", "HMAC-SHA1"} {
			for _, withContext := range []bool{false, true} {
				t.Run(fmt.Sprintf("%s/%s/context=%v", style, method, withContext), func(t *testing.T) {
					config := CreateConfig()
					config.Protocol = dara.String("HTTP")
					config.Endpoint = dara.String("example.com")
					config.HttpClient = &signatureV2Verifier{t: t, style: style, method: method}
					config.SignatureAlgorithm = dara.String("v2")
					if method == "HMAC-SHA1" {
						config.GlobalParameters.Headers["X-Acs-Signature-Method"] = dara.String("HMAC-SHA256")
					}
					client, err := NewClient(config)
					if err != nil {
						t.Fatal(err)
					}
					request := CreateOpenApiRequest()
					request.Body = nil
					if method == "HMAC-SHA1" {
						request.Headers["X-Acs-Signature-Method"] = dara.String(method)
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

type signatureV2Verifier struct {
	t      *testing.T
	style  string
	method string
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
			if capture.method != "HMAC-SHA1" {
				t.Fatalf("signature method = %q", capture.method)
			}
		})
	}
}

func (v *signatureV2Verifier) Call(req *http.Request, transport *http.Transport) (*http.Response, error) {
	verifyV2Request(v.t, req, v.style, v.method)
	return &http.Response{
		StatusCode: http.StatusOK,
		Header:     http.Header{"Content-Type": []string{"application/json"}},
		Body:       ioutil.NopCloser(strings.NewReader(`{}`)),
	}, nil
}

func verifyV2Request(t *testing.T, request *http.Request, style, method string) {
	t.Helper()
	query := request.URL.Query()
	var signature, stringToSign, key string
	if style == "RPC" {
		if query.Get("SignatureMethod") != method {
			t.Errorf("signature method = %q, want %q", query.Get("SignatureMethod"), method)
		}
		signature = query.Get("Signature")
		query.Del("Signature")
		canonical := strings.ReplaceAll(query.Encode(), "+", "%20")
		canonical = strings.ReplaceAll(canonical, "*", "%2A")
		canonical = strings.ReplaceAll(canonical, "%7E", "~")
		stringToSign = request.Method + "&%2F&" + url.QueryEscape(canonical)
		key = "secret&"
	} else {
		if signatureTestHeader(request.Header, "x-acs-signature-method") != method {
			t.Errorf("signature method = %q, want %q", signatureTestHeader(request.Header, "x-acs-signature-method"), method)
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
	var newHash func() hash.Hash = sha256.New
	if method == "HMAC-SHA1" {
		newHash = sha1.New
	}
	mac := hmac.New(newHash, []byte(key))
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

func TestSignatureV2HeaderPrecedenceAndConflicts(t *testing.T) {
	global := map[string]*string{"X-Acs-Signature-Method": dara.String("HMAC-SHA1")}
	runtime := map[string]*string{"x-acs-signature-method": dara.String("HMAC-SHA256")}
	request := map[string]*string{"X-ACS-SIGNATURE-METHOD": dara.String("HMAC-SHA1")}
	headers := dara.Merge(global, runtime, request)
	method, err := signatureV2Method(headers, global, runtime, request)
	if err != nil || method != "HMAC-SHA1" || len(headers) != 1 || dara.StringValue(headers["x-acs-signature-method"]) != method {
		t.Fatalf("method = %q, headers = %v, error = %v", method, headers, err)
	}
	conflict := map[string]*string{"X-Acs-Signature-Method": dara.String("HMAC-SHA1"), "x-acs-signature-method": dara.String("HMAC-SHA256")}
	if _, err := signatureV2Method(dara.Merge(conflict), conflict); err == nil {
		t.Fatal("conflicting header values should be rejected")
	}
	duplicate := map[string]*string{"X-Acs-Signature-Method": dara.String("HMAC-SHA1"), "x-acs-signature-method": dara.String("HMAC-SHA1")}
	headers = dara.Merge(duplicate)
	if method, err := signatureV2Method(headers, duplicate); err != nil || method != "HMAC-SHA1" || len(headers) != 1 {
		t.Fatalf("matching duplicate headers: method = %q, headers = %v, error = %v", method, headers, err)
	}
	if _, err := signatureV2Method(map[string]*string{}, map[string]*string{"X-Acs-Signature-Method": dara.String("unknown")}); err == nil {
		t.Fatal("unsupported signature algorithm should be rejected")
	}
}
