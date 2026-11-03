package client

import (
	"context"
	"errors"
	"net/http"
	"os"
	"sync"
	"testing"

	spi "github.com/alibabacloud-go/alibabacloud-gateway-spi/client"
	"github.com/alibabacloud-go/tea/dara"
	"github.com/alibabacloud-go/tea/tea"
)

type endpointCaptureClient struct{ hosts chan string }

type endpointTestGateway struct{ mockSPI }

func (g *endpointTestGateway) ModifyRequest(c *spi.InterceptorContext, _ *spi.AttributeMap) error {
	c.Request.Headers["host"] = c.Configuration.Endpoint
	return nil
}

func (c *endpointCaptureClient) Call(req *http.Request, _ *http.Transport) (*http.Response, error) {
	c.hosts <- req.URL.Host
	return nil, errors.New("stop-before-network")
}

func TestRequestEndpointSuffixPriority(t *testing.T) {
	previous, had := os.LookupEnv("ALIBABA_CLOUD_ENDPOINT_SUFFIX")
	defer func() {
		if had {
			os.Setenv("ALIBABA_CLOUD_ENDPOINT_SUFFIX", previous)
		} else {
			os.Unsetenv("ALIBABA_CLOUD_ENDPOINT_SUFFIX")
		}
	}()
	os.Unsetenv("ALIBABA_CLOUD_ENDPOINT_SUFFIX")
	c, _ := NewClient(&Config{Endpoint: tea.String("ecs.aliyuncs.com")})
	check := func(want string) {
		got, e := c.endpointForRequest(&OpenApiRequest{})
		if e != nil || tea.StringValue(got) != want {
			t.Fatalf("got %v error=%v want=%s", got, e, want)
		}
	}
	check("ecs.aliyuncs.com")
	os.Setenv("ALIBABA_CLOUD_ENDPOINT_SUFFIX", " ALIYUNCS.ALIBABA ")
	check("ecs.aliyuncs.alibaba")
	c.EndpointSuffix = tea.String("aliyuncs.com")
	check("ecs.aliyuncs.com")
	c.AttributeMap = &spi.AttributeMap{Key: map[string]*string{"endpointSuffix": tea.String(" ALIYUNCS.ALIBABA ")}}
	check("ecs.aliyuncs.alibaba")
	c.AttributeMap.Key["endpointSuffix"] = tea.String("  ")
	check("ecs.aliyuncs.com")
	c.AttributeMap.Key["endpointSuffix"] = tea.String("evil.com")
	if _, e := c.endpointForRequest(&OpenApiRequest{}); e == nil {
		t.Fatal("invalid suffix accepted")
	}
}

func TestEndpointSuffixAllRequestPaths(t *testing.T) {
	ctx := context.Background()
	paths := map[string]func(*Client, *OpenApiRequest, *dara.RuntimeOptions) error{
		"execute": func(c *Client, r *OpenApiRequest, o *dara.RuntimeOptions) error {
			c.SetGatewayClient(&endpointTestGateway{})
			_, e := c.Execute(coverageParams("ROA", "json", "string", "/", "Anonymous"), r, o)
			return e
		},
		"executeCtx": func(c *Client, r *OpenApiRequest, o *dara.RuntimeOptions) error {
			c.SetGatewayClient(&endpointTestGateway{})
			_, e := c.ExecuteWithCtx(ctx, coverageParams("ROA", "json", "string", "/", "Anonymous"), r, o)
			return e
		},
		"rpc": func(c *Client, r *OpenApiRequest, o *dara.RuntimeOptions) error {
			_, e := c.DoRPCRequest(tea.String("Test"), tea.String("2020-01-01"), tea.String("HTTPS"), tea.String("GET"), tea.String("Anonymous"), tea.String("string"), r, o)
			return e
		},
		"roa": func(c *Client, r *OpenApiRequest, o *dara.RuntimeOptions) error {
			_, e := c.DoROARequest(tea.String("Test"), tea.String("2020-01-01"), tea.String("HTTPS"), tea.String("GET"), tea.String("Anonymous"), tea.String("/"), tea.String("string"), r, o)
			return e
		},
		"roaForm": func(c *Client, r *OpenApiRequest, o *dara.RuntimeOptions) error {
			_, e := c.DoROARequestWithForm(tea.String("Test"), tea.String("2020-01-01"), tea.String("HTTPS"), tea.String("GET"), tea.String("Anonymous"), tea.String("/"), tea.String("string"), r, o)
			return e
		},
		"request": func(c *Client, r *OpenApiRequest, o *dara.RuntimeOptions) error {
			_, e := c.DoRequest(coverageParams("ROA", "json", "string", "/", "Anonymous"), r, o)
			return e
		},
		"callApi": func(c *Client, r *OpenApiRequest, o *dara.RuntimeOptions) error {
			_, e := c.CallApi(coverageParams("ROA", "json", "string", "/", "Anonymous"), r, o)
			return e
		},
		"rpcCtx": func(c *Client, r *OpenApiRequest, o *dara.RuntimeOptions) error {
			_, e := c.DoRPCRequestWithCtx(ctx, tea.String("Test"), tea.String("2020-01-01"), tea.String("HTTPS"), tea.String("GET"), tea.String("Anonymous"), tea.String("string"), r, o)
			return e
		},
		"roaCtx": func(c *Client, r *OpenApiRequest, o *dara.RuntimeOptions) error {
			_, e := c.DoROARequestWithCtx(ctx, tea.String("Test"), tea.String("2020-01-01"), tea.String("HTTPS"), tea.String("GET"), tea.String("Anonymous"), tea.String("/"), tea.String("string"), r, o)
			return e
		},
		"roaFormCtx": func(c *Client, r *OpenApiRequest, o *dara.RuntimeOptions) error {
			_, e := c.DoROAFormRequestWithCtx(ctx, tea.String("Test"), tea.String("2020-01-01"), tea.String("HTTPS"), tea.String("GET"), tea.String("Anonymous"), tea.String("/"), tea.String("string"), r, o)
			return e
		},
		"requestCtx": func(c *Client, r *OpenApiRequest, o *dara.RuntimeOptions) error {
			_, e := c.DoRequestWithCtx(ctx, coverageParams("ROA", "json", "string", "/", "Anonymous"), r, o)
			return e
		},
		"callApiCtx": func(c *Client, r *OpenApiRequest, o *dara.RuntimeOptions) error {
			_, e := c.CallApiWithCtx(ctx, coverageParams("ROA", "json", "string", "/", "Anonymous"), r, o)
			return e
		},
	}
	for name, path := range paths {
		for _, item := range []struct{ endpoint, override, want string }{
			{"ecs.cn-hangzhou.aliyuncs.com", "", "ecs.cn-hangzhou.aliyuncs.alibaba"},
			{"ecs.cn-hangzhou.aliyuncs.com", "oss-cn-hangzhou.aliyuncs.com", "oss-cn-hangzhou.aliyuncs.alibaba"},
			{"custom.aliyun.com", "", "custom.aliyun.com"},
			{"ecs.aliyuncs.alibaba", "", "ecs.aliyuncs.alibaba"},
		} {
			t.Run(name+"/"+item.want, func(t *testing.T) {
				hosts := make(chan string, 8)
				c, e := NewClient(&Config{Endpoint: tea.String(item.endpoint), EndpointSuffix: tea.String("aliyuncs.alibaba"), HttpClient: &endpointCaptureClient{hosts: hosts}})
				if e != nil {
					t.Fatal(e)
				}
				e = path(c, &OpenApiRequest{EndpointOverride: tea.String(item.override)}, &dara.RuntimeOptions{Autoretry: tea.Bool(false), MaxAttempts: tea.Int(1)})
				if e == nil {
					t.Fatal("expected transport sentinel")
				}
				select {
				case host := <-hosts:
					if host != item.want {
						t.Fatalf("got %s want %s", host, item.want)
					}
				default:
					t.Fatalf("request never reached transport: %v", e)
				}
				if tea.StringValue(c.Endpoint) != item.endpoint {
					t.Fatal("shared endpoint was mutated")
				}
			})
		}
	}
	for _, withCtx := range []bool{false, true} {
		hosts := make(chan string, 8)
		c, _ := NewClient(&Config{Endpoint: tea.String("ecs.aliyuncs.com"), EndpointSuffix: tea.String("aliyuncs.alibaba"), HttpClient: &endpointCaptureClient{hosts: hosts}})
		result, errs := make(chan *SSEResponse, 1), make(chan error, 8)
		params := coverageParams("ROA", "json", "string", "/", "Anonymous")
		req, rt := &OpenApiRequest{}, &dara.RuntimeOptions{Autoretry: tea.Bool(false), MaxAttempts: tea.Int(1)}
		if withCtx {
			go c.CallSSEApiWithCtx(ctx, params, req, rt, result, errs)
		} else {
			go c.CallSSEApi(params, req, rt, result, errs)
		}
		for range result {
		}
		select {
		case host := <-hosts:
			if host != "ecs.aliyuncs.alibaba" {
				t.Fatal(host)
			}
		default:
			t.Fatal("SSE transport not called")
		}
	}
}

func TestEndpointOverridesDoNotCrossRequests(t *testing.T) {
	hosts := make(chan string, 32)
	c, _ := NewClient(&Config{Endpoint: tea.String("shared.aliyuncs.com"), EndpointSuffix: tea.String("aliyuncs.alibaba"), HttpClient: &endpointCaptureClient{hosts: hosts}})
	var wg sync.WaitGroup
	for i := 0; i < 20; i++ {
		wg.Add(1)
		go func(i int) {
			defer wg.Done()
			host := "a.aliyuncs.com"
			if i%2 == 1 {
				host = "b.aliyuncs.com"
			}
			c.DoRequest(coverageParams("ROA", "json", "string", "/", "Anonymous"), &OpenApiRequest{EndpointOverride: tea.String(host)}, &dara.RuntimeOptions{Autoretry: tea.Bool(false), MaxAttempts: tea.Int(1)})
		}(i)
	}
	wg.Wait()
	close(hosts)
	counts := map[string]int{}
	for host := range hosts {
		counts[host]++
	}
	if counts["a.aliyuncs.alibaba"] != 10 || counts["b.aliyuncs.alibaba"] != 10 || len(counts) != 2 {
		t.Fatal(counts)
	}
	if tea.StringValue(c.Endpoint) != "shared.aliyuncs.com" {
		t.Fatal("shared endpoint changed")
	}
}
