package client

import (
	"sync"
	"testing"

	"github.com/alibabacloud-go/tea/dara"
)

func TestRpcHeadersOneShot(t *testing.T) {
	c := &Client{}
	key := dara.String("x-acs-debug")
	val := dara.String("1")
	if err := c.SetRpcHeaders(map[string]*string{dara.StringValue(key): val}); err != nil {
		t.Fatalf("SetRpcHeaders: %v", err)
	}
	got, err := c.GetRpcHeaders()
	if err != nil {
		t.Fatalf("GetRpcHeaders: %v", err)
	}
	if got == nil || dara.StringValue(got[dara.StringValue(key)]) != "1" {
		t.Fatalf("expected one-shot headers, got %#v", got)
	}
	got2, err := c.GetRpcHeaders()
	if err != nil {
		t.Fatalf("second GetRpcHeaders: %v", err)
	}
	if got2 != nil {
		t.Fatalf("expected nil after consume, got %#v", got2)
	}
}

func TestGetRpcHeadersRace(t *testing.T) {
	c := &Client{}
	var wg sync.WaitGroup
	for i := 0; i < 100; i++ {
		wg.Add(1)
		go func() {
			defer wg.Done()
			for j := 0; j < 1000; j++ {
				_, _ = c.GetRpcHeaders()
			}
		}()
	}
	wg.Wait()
}

func TestSetGetRpcHeadersRace(t *testing.T) {
	c := &Client{}
	var wg sync.WaitGroup
	for i := 0; i < 50; i++ {
		wg.Add(2)
		go func() {
			defer wg.Done()
			for j := 0; j < 200; j++ {
				_ = c.SetRpcHeaders(map[string]*string{"x": dara.String("v")})
			}
		}()
		go func() {
			defer wg.Done()
			for j := 0; j < 200; j++ {
				_, _ = c.GetRpcHeaders()
			}
		}()
	}
	wg.Wait()
}
