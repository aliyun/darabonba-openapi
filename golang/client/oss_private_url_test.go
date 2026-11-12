package client

import (
	"bytes"
	"encoding/json"
	"errors"
	"io"
	"mime/multipart"
	"net/http"
	"net/http/httptest"
	"strings"
	"testing"

	openapiutil "github.com/alibabacloud-go/darabonba-openapi/v2/utils"
	"github.com/alibabacloud-go/tea/dara"
)

func pvlForm() map[string]interface{} {
	return map[string]interface{}{
		"host":           "oss-cn-hangzhou.aliyuncs.com",
		"OSSAccessKeyId": "test-id", "policy": "test-policy", "Signature": "test-signature",
		"key": "dir/file.bin", "success_action_status": "201",
		"file": &dara.FileField{Filename: dara.String("dir/file.bin"), Content: bytes.NewReader([]byte{0, 1, 255, 10}), ContentType: dara.String("application/octet-stream")},
	}
}

func TestOSSPrivateURLPriority(t *testing.T) {
	client, err := NewClient(new(openapiutil.Config).SetOssPrivateUrl("http://config.example"))
	if err != nil {
		t.Fatal(err)
	}
	for _, tc := range []struct {
		runtime *dara.RuntimeOptions
		want    string
	}{
		{new(dara.RuntimeOptions), "http://config.example"},
		{new(dara.RuntimeOptions).SetOssPrivateUrl(""), "http://config.example"},
		{new(dara.RuntimeOptions).SetOssPrivateUrl("http://runtime.example"), "http://runtime.example"},
	} {
		if got := dara.StringValue(client.GetOSSPrivateUrl(tc.runtime)); got != tc.want {
			t.Fatalf("优先级: %q != %q", got, tc.want)
		}
	}
	runtime := new(dara.RuntimeOptions).SetOssPrivateUrl("http://runtime.example")
	data, err := json.Marshal(runtime)
	if err != nil || !strings.Contains(string(data), `"ossPrivateUrl":"http://runtime.example"`) {
		t.Fatalf("Runtime 序列化: %s %v", data, err)
	}
	var restored dara.RuntimeOptions
	if err := json.Unmarshal(data, &restored); err != nil || dara.StringValue(restored.OssPrivateUrl) != "http://runtime.example" {
		t.Fatalf("Runtime 反序列化: %v", err)
	}
}

func TestOSSPrivateURLUpload(t *testing.T) {
	// PVL 必须保持原序列化结果；现有序列化器会在文件尾追加 CRLF，此问题不在本次改造范围内。
	baseline := multipart.NewReader(dara.ToFileForm(pvlForm(), "baseline"), "baseline")
	var expectedPayload []byte
	for {
		part, err := baseline.NextPart()
		if err == io.EOF {
			break
		}
		if err != nil {
			t.Fatal(err)
		}
		data, err := io.ReadAll(part)
		if err != nil {
			t.Fatal(err)
		}
		if part.FormName() == "file" {
			expectedPayload = data
		}
	}
	if !bytes.HasPrefix(expectedPayload, []byte{0, 1, 255, 10}) {
		t.Fatal("原上传序列化器未保留文件内容")
	}
	for _, tls := range []bool{false, true} {
		for _, prefix := range []string{"", "/", "/proxy", "/proxy/"} {
			t.Run(strings.ReplaceAll(prefix, "/", "_")+map[bool]string{true: "https", false: "http"}[tls], func(t *testing.T) {
				requests := 0
				expectedHost := ""
				handler := http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
					requests++
					if r.Method != "POST" {
						t.Errorf("Method: %s", r.Method)
					}
					if r.Host != expectedHost {
						t.Errorf("Host 未保留地址和端口: %q != %q", r.Host, expectedHost)
					}
					if strings.Contains(r.Host, "test-bucket.") {
						t.Errorf("Host 不能拼 Bucket: %s", r.Host)
					}
					want := strings.TrimSuffix(prefix, "/") + "/test-bucket/"
					if r.URL.Path != want {
						t.Errorf("Path: %q != %q", r.URL.Path, want)
					}
					if (r.TLS != nil) != tls {
						t.Errorf("协议未沿用用户配置")
					}
					reader, err := r.MultipartReader()
					if err != nil {
						t.Error(err)
						return
					}
					fields := map[string]string{}
					for {
						part, err := reader.NextPart()
						if err == io.EOF {
							break
						}
						if err != nil {
							t.Error(err)
							return
						}
						data, err := io.ReadAll(part)
						if err != nil {
							t.Error(err)
							return
						}
						if part.FormName() == "file" {
							if part.FileName() != "file.bin" || !bytes.Equal(data, expectedPayload) {
								t.Errorf("文件不一致: %q %v", part.FileName(), data)
							}
						} else {
							fields[part.FormName()] = string(data)
						}
					}
					for key, want := range map[string]string{"OSSAccessKeyId": "test-id", "policy": "test-policy", "Signature": "test-signature", "key": "dir/file.bin", "success_action_status": "201"} {
						if fields[key] != want {
							t.Errorf("表单 %s = %q", key, fields[key])
						}
					}
					w.Header().Set("Content-Type", "application/xml")
					w.WriteHeader(201)
					io.WriteString(w, "<PostResponse><Key>dir/file.bin</Key></PostResponse>")
				})
				var server *httptest.Server
				if tls {
					server = httptest.NewTLSServer(handler)
				} else {
					server = httptest.NewServer(handler)
				}
				defer server.Close()
				expectedHost = strings.TrimPrefix(strings.TrimPrefix(server.URL, "http://"), "https://")
				client, err := NewClient(new(openapiutil.Config).SetOssPrivateUrl(server.URL + prefix))
				if err != nil {
					t.Fatal(err)
				}
				runtime := new(dara.RuntimeOptions).SetIgnoreSSL(tls).SetReadTimeout(2000).SetConnectTimeout(2000)
				_, err = client.PostOSSObjectWithURL(client.GetOSSPrivateUrl(runtime), dara.String("test-bucket"), pvlForm(), runtime)
				if err != nil {
					t.Fatal(err)
				}
				if requests != 1 {
					t.Fatalf("请求次数: %d", requests)
				}
			})
		}
	}
}

func TestOSSPrivateURLError(t *testing.T) {
	server := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		io.Copy(io.Discard, r.Body)
		w.WriteHeader(403)
		io.WriteString(w, "<Error><Code>AccessDenied</Code><Message>denied</Message><RequestId>req-id</RequestId><HostId>host-id</HostId></Error>")
	}))
	defer server.Close()
	client, err := NewClient(new(openapiutil.Config))
	if err != nil {
		t.Fatal(err)
	}
	client.DisableSDKError = dara.Bool(true)
	_, err = client.PostOSSObjectWithURL(dara.String(server.URL), dara.String("test-bucket"), pvlForm(), new(dara.RuntimeOptions))
	var apiErr *ClientError
	if !errors.As(err, &apiErr) || dara.StringValue(apiErr.Code) != "AccessDenied" || apiErr.Data["httpCode"] != 403 || apiErr.Data["requestId"] != "req-id" || apiErr.Data["hostId"] != "host-id" {
		t.Fatalf("OSS XML 错误: %#v", err)
	}
	for _, endpoint := range []string{"ftp://example.com", "http://user@example.com", "http://example.com/?a=b", "http://example.com/#fragment", "example.com"} {
		_, err := client.PostOSSObjectWithURL(dara.String(endpoint), dara.String("test-bucket"), pvlForm(), new(dara.RuntimeOptions))
		if err == nil {
			t.Errorf("应拒绝无效地址: %s", endpoint)
		}
	}
}
