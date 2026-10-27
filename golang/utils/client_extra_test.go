package utils

import (
	"encoding/hex"
	"strings"
	"testing"

	"github.com/alibabacloud-go/tea/dara"
	"github.com/alibabacloud-go/tea/utils"
)

func Test_Hash(t *testing.T) {
	sha := Hash([]byte("test"), dara.String("ACS3-HMAC-SHA256"))
	utils.AssertEqual(t, "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", hex.EncodeToString(sha))

	rsa := Hash([]byte("test"), dara.String("ACS3-RSA-SHA256"))
	utils.AssertEqual(t, hex.EncodeToString(sha), hex.EncodeToString(rsa))

	sm3 := Hash([]byte("test"), dara.String("ACS3-HMAC-SM3"))
	utils.AssertEqual(t, "55e12e91650d2fec56ec74e1d3e4ddbfce2ef3a65890c2a19ecf88a307e76a23", hex.EncodeToString(sm3))

	unknown := Hash([]byte("test"), dara.String("ACS3-SHA256"))
	utils.AssertNil(t, unknown)
}

func Test_ApplyRetryHeaders(t *testing.T) {
	headers := map[string]*string{"host": dara.String("example.com")}
	ApplyRetryHeaders(headers, 0, 10)
	utils.AssertNil(t, headers["x-acs-retry-attempts"])

	ApplyRetryHeaders(nil, 2, 10)

	ApplyRetryHeaders(headers, 2, 15)
	utils.AssertEqual(t, "2", dara.StringValue(headers["x-acs-retry-attempts"]))
	utils.AssertEqual(t, "15", dara.StringValue(headers["x-acs-retry-delay"]))
}

func Test_getTimeLeft(t *testing.T) {
	utils.AssertNil(t, getTimeLeft(nil))
	utils.AssertNil(t, getTimeLeft(dara.String("invalid")))
	utils.AssertNil(t, getTimeLeft(dara.String("Foo:1,Bar:2")))
	utils.AssertNil(t, getTimeLeft(dara.String("TimeLeft:abc")))
	left := getTimeLeft(dara.String("Limit:1,TimeLeft:1500"))
	utils.AssertEqual(t, int64(1500), dara.Int64Value(left))
}

func Test_toJSONString(t *testing.T) {
	utils.AssertEqual(t, "ok", dara.StringValue(toJSONString("ok")))
	ptr := dara.String("ptr")
	utils.AssertEqual(t, "ptr", dara.StringValue(toJSONString(ptr)))
	utils.AssertEqual(t, "bytes", dara.StringValue(toJSONString([]byte("bytes"))))
	utils.AssertEqual(t, "reader", dara.StringValue(toJSONString(strings.NewReader("reader"))))
	utils.AssertEqual(t, `{"k":1}`, dara.StringValue(toJSONString(map[string]int{"k": 1})))
	utils.AssertNil(t, toJSONString(make(chan int)))
}

func Test_ParseToMapAndToArrayInvalid(t *testing.T) {
	utils.AssertNil(t, ParseToMap("not-a-map"))
	utils.AssertNil(t, ToArray("not-an-array"))
	utils.AssertNil(t, ToArray(map[string]string{"k": "v"}))
}

func Test_GetUserAgentNil(t *testing.T) {
	utils.AssertEqual(t, defaultUserAgent, dara.StringValue(GetUserAgent(nil)))
}

func Test_GetAuthorizationSpecialChars(t *testing.T) {
	req := &dara.Request{
		Method:   dara.String("GET"),
		Pathname: dara.String("/path+star*tilde~"),
		Query: map[string]*string{
			"q":     dara.String("a b"),
			"empty": dara.String(""),
			"star":  dara.String("x*y"),
		},
		Headers: map[string]*string{
			"host":         dara.String("example.com"),
			"content-type": dara.String("application/json"),
			"x-acs-action": dara.String("Test"),
		},
	}
	auth := GetAuthorization(req, dara.String("ACS3-HMAC-SHA256"), dara.String("payload"), dara.String("ak"), dara.String("secret"))
	utils.AssertEqual(t, true, strings.HasPrefix(dara.StringValue(auth), "ACS3-HMAC-SHA256 Credential=ak,"))
}

func Test_SignatureMethodUnknown(t *testing.T) {
	utils.AssertNil(t, SignatureMethod("secret", "source", "unknown"))
}

func Test_GetStringToSignEmptyQuery(t *testing.T) {
	req := dara.NewRequest()
	req.Method = dara.String("GET")
	req.Pathname = dara.String("/foo")
	req.Query = map[string]*string{
		"empty": dara.String(""),
		"ok":    dara.String("1"),
	}
	req.Headers = map[string]*string{
		"accept":       dara.String("json"),
		"content-type": dara.String("text/plain"),
		"date":         dara.String("now"),
		"content-md5":  dara.String("md5"),
		"x-acs-foo":    dara.String("bar"),
	}
	sign := GetStringToSign(req)
	utils.AssertEqual(t, true, strings.Contains(dara.StringValue(sign), "/foo?"))
	utils.AssertEqual(t, true, strings.Contains(dara.StringValue(sign), "empty"))
}

func Test_QueryNestedAndNilSlice(t *testing.T) {
	query := Query(map[string]interface{}{
		"nested": map[string]interface{}{
			"inner": "v",
		},
		"list": []interface{}{
			map[string]interface{}{"k": "1"},
			[]string{"a", "b"},
		},
		"nilval": nil,
	})
	utils.AssertEqual(t, "v", dara.StringValue(query["nested.inner"]))
	utils.AssertEqual(t, "1", dara.StringValue(query["list.1.k"]))
	utils.AssertEqual(t, "a", dara.StringValue(query["list.2.1"]))
}

func Test_MapToFlatStyleMoreCases(t *testing.T) {
	var nilPtr *map[string]interface{}
	utils.AssertEqual(t, nilPtr, MapToFlatStyle(nilPtr))

	type Nested struct {
		Value string
	}
	in := Nested{Value: "x"}
	out := MapToFlatStyle(in)
	utils.AssertNotNil(t, out)

	result := MapToFlatStyle(123)
	utils.AssertEqual(t, 123, result)
	utils.AssertNotNil(t, MapToFlatStyle([]interface{}{map[string]interface{}{"k": "v"}, 1}))
}

func Test_rsaSignInvalidKey(t *testing.T) {
	utils.AssertNil(t, rsaSign("content", "not-a-key"))
}

func Test_StringifyMapValueNil(t *testing.T) {
	out := StringifyMapValue(map[string]interface{}{
		"nil":    nil,
		"reader": strings.NewReader("from-reader"),
	})
	utils.AssertNil(t, out["nil"])
	utils.AssertEqual(t, "from-reader", dara.StringValue(out["reader"]))
}

func Test_GetEndpointRulesPublicNetwork(t *testing.T) {
	endpoint, err := GetEndpointRules(dara.String("ecs"), dara.String("cn-hangzhou"), dara.String("regional"), dara.String("public"), dara.String(""))
	utils.AssertNil(t, err)
	utils.AssertEqual(t, "ecs.cn-hangzhou.aliyuncs.com", dara.StringValue(endpoint))
}
