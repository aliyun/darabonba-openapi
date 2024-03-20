package client

import (
	"testing"

	"github.com/alibabacloud-go/tea/v2/tea"
	"github.com/alibabacloud-go/tea/v2/utils"
)

func TestThrottlingError(t *testing.T) {
	var err tea.BaseError
	err = &ThrottlingError{
		Code:        tea.String("Throttling"),
		Message:     tea.String("message"),
		Description: tea.String("Throttling"),
		StatusCode:  tea.Int(int(429)),
		RetryAfter:  tea.Int64(int64(2000)),
	}
	utils.AssertNotNil(t, err)
	utils.AssertEqual(t, tea.Prettify(err), err.Error())
	utils.AssertEqual(t, "Throttling", tea.StringValue(err.ErrorCode()))
	utils.AssertEqual(t, "ThrottlingError", tea.StringValue(err.ErrorName()))
	utils.AssertEqual(t, int64(2000), tea.Int64Value(err.RetryAfterTimeMillis()))
	throttlingError, ok := err.(*ThrottlingError)
	utils.AssertEqual(t, true, ok)

	utils.AssertEqual(t, tea.Prettify(throttlingError), throttlingError.Error())
	utils.AssertEqual(t, "Throttling", tea.StringValue(throttlingError.ErrorCode()))
	utils.AssertEqual(t, "ThrottlingError", tea.StringValue(throttlingError.ErrorName()))
	utils.AssertEqual(t, int64(2000), tea.Int64Value(throttlingError.RetryAfterTimeMillis()))
	utils.AssertEqual(t, "Throttling", tea.StringValue(throttlingError.Code))
	utils.AssertEqual(t, "message", tea.StringValue(throttlingError.Message))
	utils.AssertEqual(t, "Throttling", tea.StringValue(throttlingError.Description))
	utils.AssertEqual(t, int(429), tea.IntValue(throttlingError.StatusCode))
	utils.AssertEqual(t, int64(2000), tea.Int64Value(throttlingError.RetryAfter))

	throttlingError = &ThrottlingError{}
	utils.AssertEqual(t, tea.Prettify(throttlingError), throttlingError.Error())
	utils.AssertNil(t, throttlingError.ErrorCode())
	utils.AssertEqual(t, "ThrottlingError", tea.StringValue(throttlingError.ErrorName()))
	utils.AssertNil(t, throttlingError.RetryAfterTimeMillis())
	utils.AssertNil(t, throttlingError.Code)
	utils.AssertNil(t, throttlingError.Message)
	utils.AssertNil(t, throttlingError.Description)
	utils.AssertNil(t, throttlingError.StatusCode)
	utils.AssertNil(t, throttlingError.RetryAfter)
}
