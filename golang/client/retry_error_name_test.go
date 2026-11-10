package client

import (
	"fmt"
	"net/http"
	"net/http/httptest"
	"strings"
	"sync/atomic"
	"testing"

	"github.com/alibabacloud-go/tea/dara"
)

func TestCallApiRetriesByHTTPExceptionName(t *testing.T) {
	for _, tc := range []struct {
		status    int
		category  string
		code      string
		attempts  int32
		throttled bool
	}{
		{http.StatusServiceUnavailable, "ServerError", "ServiceUnavailable", 3, false},
		{http.StatusTooManyRequests, "ThrottlingError", "Throttling", 3, true},
		{http.StatusBadRequest, "ClientError", "InvalidParameter", 3, false},
		{http.StatusBadRequest, "ServerError", "InvalidParameter", 1, false},
	} {
		t.Run(fmt.Sprintf("%d/%s", tc.status, tc.category), func(t *testing.T) {
			var attempts int32
			server := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
				atomic.AddInt32(&attempts, 1)
				w.Header().Set("Content-Type", "application/json")
				if tc.throttled {
					w.Header().Set("x-acs-retry-after", "1")
				}
				w.WriteHeader(tc.status)
				fmt.Fprintf(w, "{\"Code\":%q,\"Message\":\"test error\"}", tc.code)
			}))
			defer server.Close()

			config := CreateConfig()
			config.Protocol = dara.String("HTTP")
			config.Endpoint = dara.String(strings.TrimPrefix(server.URL, "http://"))
			config.RetryOptions = &dara.RetryOptions{
				Retryable: true,
				RetryCondition: []*dara.RetryCondition{
					{Exception: []string{tc.category}, MaxAttempts: 3},
				},
			}
			client, err := NewClient(config)
			if err != nil {
				t.Fatal(err)
			}
			params := &Params{
				Action: dara.String("Test"), Version: dara.String("2020-05-10"),
				Protocol: dara.String("HTTP"), Pathname: dara.String("/"),
				Method: dara.String("POST"), AuthType: dara.String("AK"), Style: dara.String("RPC"),
				ReqBodyType: dara.String("formData"), BodyType: dara.String("json"),
			}
			if _, err := client.CallApi(params, &OpenApiRequest{}, CreateRuntimeOptions()); err == nil {
				t.Fatal("expected the final HTTP error")
			}
			if got := atomic.LoadInt32(&attempts); got != tc.attempts {
				t.Fatalf("got %d requests, want %d", got, tc.attempts)
			}
		})
	}
}

func TestExceptionNamePreservesExplicitValue(t *testing.T) {
	for _, name := range []string{"", "CustomError"} {
		value := dara.String(name)
		for _, err := range []dara.BaseError{
			&AlibabaCloudError{Name: value},
			&ClientError{Name: value},
			&ServerError{Name: value},
			&ThrottlingError{Name: value},
		} {
			if got := err.GetName(); got != value {
				t.Errorf("%T changed explicitly assigned name %q", err, name)
			}
		}
	}
	if got := dara.StringValue((&AlibabaCloudError{}).GetName()); got != "AlibabaCloudError" {
		t.Errorf("got %q, want AlibabaCloudError", got)
	}
}
