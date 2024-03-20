package client

import (
	"github.com/alibabacloud-go/tea/v2/tea"
)

type AlibabaCloudError struct {
	tea.BaseError
	// HTTP Status Code
	StatusCode *int
	// Error Code
	Code *string
	// Error Message
	Message *string
	// Error Description
	Description *string
	// Request ID
	RequestId *string
}

func (err *AlibabaCloudError) Error() string {
	return tea.Prettify(err)
}

func (err *AlibabaCloudError) ErrorName() *string {
	return tea.String("AlibabaCloudError")
}

func (err *AlibabaCloudError) ErrorCode() *string {
	return err.Code
}

func (err *AlibabaCloudError) RetryAfterTimeMillis() *int64 {
	return nil
}

type ClientError struct {
	*AlibabaCloudError
	// HTTP Status Code
	StatusCode *int
	// Error Code
	Code *string
	// Error Message
	Message *string
	// Error Description
	Description *string
	// Request ID
	RequestId *string
	// Access Denied Detail
	AccessDeniedDetail map[string]interface{}
}

func (err *ClientError) Error() string {
	return tea.Prettify(err)
}

func (err *ClientError) ErrorName() *string {
	return tea.String("ClientError")
}

func (err *ClientError) ErrorCode() *string {
	return err.Code
}

func (err *ClientError) RetryAfterTimeMillis() *int64 {
	return nil
}

type ServerError struct {
	*AlibabaCloudError
	// HTTP Status Code
	StatusCode *int
	// Error Code
	Code *string
	// Error Message
	Message *string
	// Error Description
	Description *string
	// Request ID
	RequestId *string
}

func (err *ServerError) Error() string {
	return tea.Prettify(err)
}

func (err *ServerError) ErrorName() *string {
	return tea.String("ServerError")
}

func (err *ServerError) ErrorCode() *string {
	return err.Code
}

func (err *ServerError) RetryAfterTimeMillis() *int64 {
	return nil
}

type ThrottlingError struct {
	*AlibabaCloudError
	// HTTP Status Code
	StatusCode *int
	// Error Code
	Code *string
	// Error Message
	Message *string
	// Error Description
	Description *string
	// Request ID
	RequestId *string
	// Retry After(ms)
	RetryAfter *int64
}

func (err *ThrottlingError) Error() string {
	return tea.Prettify(err)
}

func (err *ThrottlingError) ErrorName() *string {
	return tea.String("ThrottlingError")
}

func (err *ThrottlingError) ErrorCode() *string {
	return err.Code
}

func (err *ThrottlingError) RetryAfterTimeMillis() *int64 {
	return err.RetryAfter
}
