package utils

import (
	"io"
	"reflect"
	"strings"
	"testing"
	"encoding/hex"

	"github.com/alibabacloud-go/tea/dara"
	"github.com/alibabacloud-go/tea/utils"
)

func Test_GetROASignature(t *testing.T) {
	request := dara.NewRequest()
	sign := GetStringToSign(request)
	signature := GetROASignature(sign, dara.String("secret"))
	utils.AssertEqual(t, 28, len(dara.StringValue(signature)))
}

func Test_Sorter(t *testing.T) {
	tmp := map[string]string{
		"key":   "ccp",
		"value": "ok",
	}
	sort := newSorter(tmp)
	sort.Sort()

	len := sort.Len()
	utils.AssertEqual(t, len, 2)

	isLess := sort.Less(0, 1)
	utils.AssertEqual(t, isLess, true)

	sort.Swap(0, 1)
	isLess = sort.Less(0, 1)
	utils.AssertEqual(t, isLess, false)
}

type TestCommon struct {
	Body io.Reader `json:"Body"`
	Test string    `json:"Test"`
}

func Test_Convert(t *testing.T) {
	in := &TestCommon{
		Body: strings.NewReader("common"),
		Test: "ok",
	}
	out := new(TestCommon)
	Convert(in, &out)
	utils.AssertEqual(t, "ok", out.Test)
}

func Test_getStringToSign(t *testing.T) {
	request := dara.NewRequest()
	request.Query = map[string]*string{
		"roa":  dara.String("ok"),
		"null": dara.String(""),
	}
	request.Headers = map[string]*string{
		"x-acs-meta": dara.String("user"),
	}
	str := getStringToSign(request)
	utils.AssertEqual(t, 33, len(str))
}

func Test_ToForm(t *testing.T) {
	filter := map[string]interface{}{
		"client": "test",
		"tag": map[string]*string{
			"key": dara.String("value"),
		},
		"strs": []string{"str1", "str2"},
	}

	result := ToForm(filter)
	utils.AssertEqual(t, "client=test&strs.1=str1&strs.2=str2&tag.key=value", dara.StringValue(result))
}

func Test_flatRepeatedList(t *testing.T) {
	filter := map[string]interface{}{
		"client":  "test",
		"version": "1",
		"null":    nil,
		"slice": []interface{}{
			map[string]interface{}{
				"map": "valid",
			},
			6,
		},
		"map": map[string]interface{}{
			"value": "ok",
		},
	}

	result := make(map[string]*string)
	for key, value := range filter {
		filterValue := reflect.ValueOf(value)
		flatRepeatedList(filterValue, result, key)
	}
	utils.AssertEqual(t, dara.StringValue(result["slice.1.map"]), "valid")
	utils.AssertEqual(t, dara.StringValue(result["slice.2"]), "6")
	utils.AssertEqual(t, dara.StringValue(result["map.value"]), "ok")
	utils.AssertEqual(t, dara.StringValue(result["client"]), "test")
	utils.AssertEqual(t, dara.StringValue(result["slice.1.map"]), "valid")
}

func Test_GetRPCSignature(t *testing.T) {
	signed := map[string]*string{
		"test": dara.String("ok"),
	}

	sign := GetRPCSignature(signed, dara.String(""), dara.String("accessKeySecret"))
	utils.AssertEqual(t, "jHx/oHoHNrbVfhncHEvPdHXZwHU=", dara.StringValue(sign))
}

func Test_GetTimestamp(t *testing.T) {
	stamp := GetTimestamp()
	utils.AssertNotNil(t, stamp)
}

func Test_Query(t *testing.T) {
	filter := map[string]interface{}{
		"client": "test",
		"tag": map[string]string{
			"key": "value",
		},
		"strs": []string{"str1", "str2"},
	}

	result := Query(filter)
	res := Query(result)
	utils.AssertEqual(t, "test", dara.StringValue(res["client"]))
	utils.AssertEqual(t, "test", dara.StringValue(result["client"]))
	utils.AssertEqual(t, "value", dara.StringValue(result["tag.key"]))
	utils.AssertEqual(t, "str1", dara.StringValue(result["strs.1"]))
	utils.AssertEqual(t, "str2", dara.StringValue(result["strs.2"]))
}

func Test_ArrayToStringWithSpecifiedStyle(t *testing.T) {
	strs := []interface{}{dara.String("ok"), "test", 2, dara.Int(3)}

	result := ArrayToStringWithSpecifiedStyle(strs, dara.String("instance"), dara.String("repeatList"))
	utils.AssertEqual(t, "instance.1=ok&&instance.2=test&&instance.3=2&&instance.4=3", dara.StringValue(result))
	result = ArrayToStringWithSpecifiedStyle(strs, dara.String("instance"), dara.String("json"))
	utils.AssertEqual(t, "[\"ok\",\"test\",2,3]", dara.StringValue(result))
	result = ArrayToStringWithSpecifiedStyle(strs, dara.String("instance"), dara.String("simple"))
	utils.AssertEqual(t, "ok,test,2,3", dara.StringValue(result))
	result = ArrayToStringWithSpecifiedStyle(strs, dara.String("instance"), dara.String("spaceDelimited"))
	utils.AssertEqual(t, "ok test 2 3", dara.StringValue(result))
	result = ArrayToStringWithSpecifiedStyle(strs, dara.String("instance"), dara.String("pipeDelimited"))
	utils.AssertEqual(t, "ok|test|2|3", dara.StringValue(result))
	result = ArrayToStringWithSpecifiedStyle(strs, dara.String("instance"), dara.String("piDelimited"))
	utils.AssertEqual(t, "", dara.StringValue(result))
	result = ArrayToStringWithSpecifiedStyle(nil, dara.String("instance"), dara.String("pipeDelimited"))
	utils.AssertEqual(t, "", dara.StringValue(result))
}

type Str struct {
	Key string `json:"key"`
}

func Test_ParseToMap(t *testing.T) {
	in := &Str{
		Key: "value",
	}
	res := ParseToMap(in)
	utils.AssertEqual(t, res["key"], "value")

	in0 := map[string]*Str{"test": in}
	res = ParseToMap(in0)
	utils.AssertEqual(t, res["test"], map[string]interface{}{"key": "value"})

	res = ParseToMap(nil)
	utils.AssertNil(t, res)
}

func Test_GetAuthorization(t *testing.T) {
	query := map[string]*string{
		"test":  dara.String("ok"),
		"empty": dara.String(""),
	}

	headers := map[string]*string{
		"x-acs-test": dara.String("http"),
		"x-acs-TEST": dara.String("https"),
	}
	req := &dara.Request{
		Query:   query,
		Headers: headers,
	}
	req.Pathname = dara.String("")
	res := GetAuthorization(req, dara.String("ACS3-HMAC-SHA256"),
		dara.String("55e12e91650d2fec56ec74e1d3e4ddbfce2ef3a65890c2a19ecf88a307e76a23"),
		dara.String("acesskey"), dara.String("secret"))
	utils.AssertEqual(t, "ACS3-HMAC-SHA256 Credential=acesskey,SignedHeaders=x-acs-test,Signature=4ab59fffe3c5738ff8a2729f90cc04fe18b02a4b15b2102cbaf92f9ff3df2ea3", dara.StringValue(res))
}

func Test_SignatureMethod(t *testing.T) {
	priKey := `MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAKzSQmrnH0YnezZ9
	8NK50WjMuci0hgGVcSthIZOTWMIySznY9Jj1hlvek7W0uYagtFHz03BHQnHAb5Xs
	0DZm0Sj9+5r79GggwEzTJDYEsLyFwXM3ZOIxqxL4sRg94MHsa81M9NXGHMyMvvff
	QTn1OBVLTVz5jgJ48foMn7j7r9kRAgMBAAECgYEAnZppw3/ef2XF8Z3Mnv+iP0Zk
	LuqiQpN8TykXK7P1/7NJ8wktlshhrSo/3jdf8axghVQsgHob2Ay8Nidugg4lsxIL
	AUBHvfQsQp1MAWvxslsVj+ddw01MQnt8kHmC/qhok+YuNqqAGBcoD6cthRUjEri6
	hfs599EfPs2DcWW06qECQQDfNqUUhcDQ/SQHRhfY9UIlaSEs2CVagDrSYFG1wyG+
	PXDSMes9ZRHsvVVBmNGmtUTg/jioTU3yuPsis5s9ppbVAkEAxjTAQxv5lBBm/ikM
	TzPShljxDZnXh6lKWG9gR1p5fKoQTzLyyhHzkBSFe848sMm68HWCX2wgIpQLHj0G
	ccYPTQJAduMKBeY/jpBlkiI5LWtj8b0O2G2/Z3aI3ehDXQYzgLoEz0+bNbYRWAB3
	2lpkv+AocZW1455Y+ACichcrhiimiQJAW/6L5hoL4u8h/oFq1zAEXJrXdyqaYLrw
	aM947mVN0dDVNQ0+pw9h7tO3iNkWTi+zdnv0APociDASYPyOCyyUWQJACMNRM1/r
	boXuKfMmVjmmz0XhaDUC/JkqSwIiaZi+47M21e9BTp1218NA6VaPgJJHeJr4sNOn
	Ysx+1cwXO5cuZg==`
	res := SignatureMethod("secret", "source", "ACS3-HMAC-SM3")
	utils.AssertEqual(t, "b9ff646822f41ef647c1416fa2b8408923828abc0464af6706e18db3e8553da8", hex.EncodeToString(res))

	res = SignatureMethod("secret", "source", "ACS3-RSA-SHA256")
	utils.AssertEqual(t, "", hex.EncodeToString(res))

	res = SignatureMethod(priKey, "source", "ACS3-RSA-SHA256")
	utils.AssertEqual(t, "a00b88ae04f651a8ab645e724949ff435bbb2cf9a37aa54323024477f8031f4e13dc948484c5c5a81ba53a55eb0571dffccc1e953c93269d6da23ed319e0f1ef699bcc9823a646574628ae1b70ed569b5a07d139dda28996b5b9231f5ba96141f0893deec2fbf54a0fa2c203b8ae74dd26f457ac29c873745a5b88273d2b3d12", hex.EncodeToString(res))
}

func Test_GetThrottlingTimeLeft(t *testing.T) {
	headers := map[string]*string{
		"x-ratelimit-user-api": nil,
		"x-ratelimit-user":     nil,
	}
	timeLeft := GetThrottlingTimeLeft(headers)
	utils.AssertNil(t, timeLeft)

	headers = map[string]*string{
		"x-ratelimit-user-api": nil,
		"x-ratelimit-user":     dara.String("Limit:1,Remain:0,TimeLeft:2000,Reset:1234"),
	}
	timeLeft = GetThrottlingTimeLeft(headers)
	utils.AssertEqual(t, int64(2000), dara.Int64Value(timeLeft))

	headers = map[string]*string{
		"x-ratelimit-user-api": dara.String("Limit:1,Remain:0,TimeLeft:2000,Reset:1234"),
		"x-ratelimit-user":     nil,
	}
	timeLeft = GetThrottlingTimeLeft(headers)
	utils.AssertEqual(t, int64(2000), dara.Int64Value(timeLeft))

	headers = map[string]*string{
		"x-ratelimit-user-api": dara.String("Limit:1,Remain:0,TimeLeft:2000,Reset:1234"),
		"x-ratelimit-user":     dara.String("Limit:1,Remain:0,TimeLeft:0,Reset:1234"),
	}
	timeLeft = GetThrottlingTimeLeft(headers)
	utils.AssertEqual(t, int64(2000), dara.Int64Value(timeLeft))

	headers = map[string]*string{
		"x-ratelimit-user-api": dara.String("Limit:1,Remain:0,TimeLeft:0,Reset:1234"),
		"x-ratelimit-user":     dara.String("Limit:1,Remain:0,TimeLeft:0,Reset:1234"),
	}
	timeLeft = GetThrottlingTimeLeft(headers)
	utils.AssertEqual(t, int64(0), dara.Int64Value(timeLeft))
}

func Test_GetNonce(t *testing.T) {
	nonce := GetNonce()
	utils.AssertEqual(t, 32, len(dara.StringValue(nonce)))
}

func Test_GetDateUTCString(t *testing.T) {
	time := GetDateUTCString()
	utils.AssertEqual(t, 29, len(dara.StringValue(time)))
}

func Test_UserAgent(t *testing.T) {
	utils.AssertEqual(t, dara.StringValue(GetUserAgent(dara.String(""))), defaultUserAgent)
	utils.AssertContains(t, dara.StringValue(GetUserAgent(dara.String("tea"))), " tea")
}

func Test_GetEndpointRules(t *testing.T) {
	endpoint, err := GetEndpointRules(dara.String("ecs"), dara.String(""), dara.String("regional"), dara.String(""), dara.String(""))
	utils.AssertEqual(t, "", dara.StringValue(endpoint))
	utils.AssertEqual(t, "RegionId is empty, please set a valid RegionId", err.Error())

	endpoint, err = GetEndpointRules(dara.String("ecs"), dara.String("cn-hangzhou"), dara.String("regional"), dara.String(""), dara.String(""))
	utils.AssertNil(t, err)
	utils.AssertEqual(t, "ecs.cn-hangzhou.aliyuncs.com", dara.StringValue(endpoint))

	endpoint, err = GetEndpointRules(dara.String("ecs"), dara.String("cn-hangzhou"), dara.String("central"), dara.String("intl"), dara.String("test"))
	utils.AssertNil(t, err)
	utils.AssertEqual(t, "ecs-test-intl.aliyuncs.com", dara.StringValue(endpoint))
}