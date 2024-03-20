package client

import (
	"io"

	"github.com/alibabacloud-go/tea/v2/tea"
	credential "github.com/aliyun/credentials-go/credentials"
)

type GlobalParameters struct {
	Headers map[string]*string `json:"headers,omitempty" xml:"headers,omitempty"`
	Queries map[string]*string `json:"queries,omitempty" xml:"queries,omitempty"`
}

func (s GlobalParameters) String() string {
	return tea.Prettify(s)
}

func (s GlobalParameters) GoString() string {
	return s.String()
}

func (s *GlobalParameters) SetHeaders(v map[string]*string) *GlobalParameters {
	s.Headers = v
	return s
}

func (s *GlobalParameters) SetQueries(v map[string]*string) *GlobalParameters {
	s.Queries = v
	return s
}

// Description:
//
// Model for initing client
type Config struct {
	// accesskey id
	AccessKeyId *string `json:"accessKeyId,omitempty" xml:"accessKeyId,omitempty"`
	// accesskey secret
	AccessKeySecret *string `json:"accessKeySecret,omitempty" xml:"accessKeySecret,omitempty"`
	// security token
	SecurityToken *string `json:"securityToken,omitempty" xml:"securityToken,omitempty"`
	// http protocol
	//
	// example:
	//
	// http
	Protocol *string `json:"protocol,omitempty" xml:"protocol,omitempty"`
	// http method
	//
	// example:
	//
	// GET
	Method *string `json:"method,omitempty" xml:"method,omitempty"`
	// region id
	//
	// example:
	//
	// cn-hangzhou
	RegionId *string `json:"regionId,omitempty" xml:"regionId,omitempty"`
	// read timeout
	//
	// example:
	//
	// 10
	ReadTimeout *int `json:"readTimeout,omitempty" xml:"readTimeout,omitempty"`
	// connect timeout
	//
	// example:
	//
	// 10
	ConnectTimeout *int `json:"connectTimeout,omitempty" xml:"connectTimeout,omitempty"`
	// http proxy
	//
	// example:
	//
	// http://localhost
	HttpProxy *string `json:"httpProxy,omitempty" xml:"httpProxy,omitempty"`
	// https proxy
	//
	// example:
	//
	// https://localhost
	HttpsProxy *string `json:"httpsProxy,omitempty" xml:"httpsProxy,omitempty"`
	// credential
	Credential credential.Credential `json:"credential,omitempty" xml:"credential,omitempty"`
	// endpoint
	//
	// example:
	//
	// cs.aliyuncs.com
	Endpoint *string `json:"endpoint,omitempty" xml:"endpoint,omitempty"`
	// proxy white list
	//
	// example:
	//
	// http://localhost
	NoProxy *string `json:"noProxy,omitempty" xml:"noProxy,omitempty"`
	// max idle conns
	//
	// example:
	//
	// 3
	MaxIdleConns *int `json:"maxIdleConns,omitempty" xml:"maxIdleConns,omitempty"`
	// network for endpoint
	//
	// example:
	//
	// public
	Network *string `json:"network,omitempty" xml:"network,omitempty"`
	// user agent
	//
	// example:
	//
	// Alibabacloud/1
	UserAgent *string `json:"userAgent,omitempty" xml:"userAgent,omitempty"`
	// suffix for endpoint
	//
	// example:
	//
	// aliyun
	Suffix *string `json:"suffix,omitempty" xml:"suffix,omitempty"`
	// socks5 proxy
	Socks5Proxy *string `json:"socks5Proxy,omitempty" xml:"socks5Proxy,omitempty"`
	// socks5 network
	//
	// example:
	//
	// TCP
	Socks5NetWork *string `json:"socks5NetWork,omitempty" xml:"socks5NetWork,omitempty"`
	// endpoint type
	//
	// example:
	//
	// internal
	EndpointType *string `json:"endpointType,omitempty" xml:"endpointType,omitempty"`
	// OpenPlatform endpoint
	//
	// example:
	//
	// openplatform.aliyuncs.com
	OpenPlatformEndpoint *string `json:"openPlatformEndpoint,omitempty" xml:"openPlatformEndpoint,omitempty"`
	// Deprecated
	//
	// credential type
	//
	// example:
	//
	// access_key
	Type *string `json:"type,omitempty" xml:"type,omitempty"`
	// Signature Version
	//
	// example:
	//
	// v1
	SignatureVersion *string `json:"signatureVersion,omitempty" xml:"signatureVersion,omitempty"`
	// Signature Algorithm
	//
	// example:
	//
	// ACS3-HMAC-SHA256
	SignatureAlgorithm *string `json:"signatureAlgorithm,omitempty" xml:"signatureAlgorithm,omitempty"`
	// Global Parameters
	GlobalParameters *GlobalParameters `json:"globalParameters,omitempty" xml:"globalParameters,omitempty"`
	// privite key for client certificate
	//
	// example:
	//
	// MIIEvQ
	Key *string `json:"key,omitempty" xml:"key,omitempty"`
	// client certificate
	//
	// example:
	//
	// -----BEGIN CERTIFICATE-----
	//
	// xxx-----END CERTIFICATE-----
	Cert *string `json:"cert,omitempty" xml:"cert,omitempty"`
	// server certificate
	//
	// example:
	//
	// -----BEGIN CERTIFICATE-----
	//
	// xxx-----END CERTIFICATE-----
	Ca *string `json:"ca,omitempty" xml:"ca,omitempty"`
	// disable HTTP/2
	//
	// example:
	//
	// false
	DisableHttp2 *bool `json:"disableHttp2,omitempty" xml:"disableHttp2,omitempty"`
	// retry options
	RetryOptions *tea.RetryOptions `json:"retryOptions,omitempty" xml:"retryOptions,omitempty"`
}

func (s Config) String() string {
	return tea.Prettify(s)
}

func (s Config) GoString() string {
	return s.String()
}

func (s *Config) SetAccessKeyId(v string) *Config {
	s.AccessKeyId = &v
	return s
}

func (s *Config) SetAccessKeySecret(v string) *Config {
	s.AccessKeySecret = &v
	return s
}

func (s *Config) SetSecurityToken(v string) *Config {
	s.SecurityToken = &v
	return s
}

func (s *Config) SetProtocol(v string) *Config {
	s.Protocol = &v
	return s
}

func (s *Config) SetMethod(v string) *Config {
	s.Method = &v
	return s
}

func (s *Config) SetRegionId(v string) *Config {
	s.RegionId = &v
	return s
}

func (s *Config) SetReadTimeout(v int) *Config {
	s.ReadTimeout = &v
	return s
}

func (s *Config) SetConnectTimeout(v int) *Config {
	s.ConnectTimeout = &v
	return s
}

func (s *Config) SetHttpProxy(v string) *Config {
	s.HttpProxy = &v
	return s
}

func (s *Config) SetHttpsProxy(v string) *Config {
	s.HttpsProxy = &v
	return s
}

func (s *Config) SetCredential(v credential.Credential) *Config {
	s.Credential = v
	return s
}

func (s *Config) SetEndpoint(v string) *Config {
	s.Endpoint = &v
	return s
}

func (s *Config) SetNoProxy(v string) *Config {
	s.NoProxy = &v
	return s
}

func (s *Config) SetMaxIdleConns(v int) *Config {
	s.MaxIdleConns = &v
	return s
}

func (s *Config) SetNetwork(v string) *Config {
	s.Network = &v
	return s
}

func (s *Config) SetUserAgent(v string) *Config {
	s.UserAgent = &v
	return s
}

func (s *Config) SetSuffix(v string) *Config {
	s.Suffix = &v
	return s
}

func (s *Config) SetSocks5Proxy(v string) *Config {
	s.Socks5Proxy = &v
	return s
}

func (s *Config) SetSocks5NetWork(v string) *Config {
	s.Socks5NetWork = &v
	return s
}

func (s *Config) SetEndpointType(v string) *Config {
	s.EndpointType = &v
	return s
}

func (s *Config) SetOpenPlatformEndpoint(v string) *Config {
	s.OpenPlatformEndpoint = &v
	return s
}

func (s *Config) SetType(v string) *Config {
	s.Type = &v
	return s
}

func (s *Config) SetSignatureVersion(v string) *Config {
	s.SignatureVersion = &v
	return s
}

func (s *Config) SetSignatureAlgorithm(v string) *Config {
	s.SignatureAlgorithm = &v
	return s
}

func (s *Config) SetGlobalParameters(v *GlobalParameters) *Config {
	s.GlobalParameters = v
	return s
}

func (s *Config) SetKey(v string) *Config {
	s.Key = &v
	return s
}

func (s *Config) SetCert(v string) *Config {
	s.Cert = &v
	return s
}

func (s *Config) SetCa(v string) *Config {
	s.Ca = &v
	return s
}

func (s *Config) SetDisableHttp2(v bool) *Config {
	s.DisableHttp2 = &v
	return s
}

func (s *Config) SetRetryOptions(v *tea.RetryOptions) *Config {
	s.RetryOptions = v
	return s
}

type OpenApiRequest struct {
	Headers          map[string]*string `json:"headers,omitempty" xml:"headers,omitempty"`
	Query            map[string]*string `json:"query,omitempty" xml:"query,omitempty"`
	Body             interface{}        `json:"body,omitempty" xml:"body,omitempty"`
	Stream           io.Reader          `json:"stream,omitempty" xml:"stream,omitempty"`
	HostMap          map[string]*string `json:"hostMap,omitempty" xml:"hostMap,omitempty"`
	EndpointOverride *string            `json:"endpointOverride,omitempty" xml:"endpointOverride,omitempty"`
}

func (s OpenApiRequest) String() string {
	return tea.Prettify(s)
}

func (s OpenApiRequest) GoString() string {
	return s.String()
}

func (s *OpenApiRequest) SetHeaders(v map[string]*string) *OpenApiRequest {
	s.Headers = v
	return s
}

func (s *OpenApiRequest) SetQuery(v map[string]*string) *OpenApiRequest {
	s.Query = v
	return s
}

func (s *OpenApiRequest) SetBody(v interface{}) *OpenApiRequest {
	s.Body = v
	return s
}

func (s *OpenApiRequest) SetStream(v io.Reader) *OpenApiRequest {
	s.Stream = v
	return s
}

func (s *OpenApiRequest) SetHostMap(v map[string]*string) *OpenApiRequest {
	s.HostMap = v
	return s
}

func (s *OpenApiRequest) SetEndpointOverride(v string) *OpenApiRequest {
	s.EndpointOverride = &v
	return s
}

type Params struct {
	Action      *string `json:"action,omitempty" xml:"action,omitempty" require:"true"`
	Version     *string `json:"version,omitempty" xml:"version,omitempty" require:"true"`
	Protocol    *string `json:"protocol,omitempty" xml:"protocol,omitempty" require:"true"`
	Pathname    *string `json:"pathname,omitempty" xml:"pathname,omitempty" require:"true"`
	Method      *string `json:"method,omitempty" xml:"method,omitempty" require:"true"`
	AuthType    *string `json:"authType,omitempty" xml:"authType,omitempty" require:"true"`
	BodyType    *string `json:"bodyType,omitempty" xml:"bodyType,omitempty" require:"true"`
	ReqBodyType *string `json:"reqBodyType,omitempty" xml:"reqBodyType,omitempty" require:"true"`
	Style       *string `json:"style,omitempty" xml:"style,omitempty"`
}

func (s Params) String() string {
	return tea.Prettify(s)
}

func (s Params) GoString() string {
	return s.String()
}

func (s *Params) SetAction(v string) *Params {
	s.Action = &v
	return s
}

func (s *Params) SetVersion(v string) *Params {
	s.Version = &v
	return s
}

func (s *Params) SetProtocol(v string) *Params {
	s.Protocol = &v
	return s
}

func (s *Params) SetPathname(v string) *Params {
	s.Pathname = &v
	return s
}

func (s *Params) SetMethod(v string) *Params {
	s.Method = &v
	return s
}

func (s *Params) SetAuthType(v string) *Params {
	s.AuthType = &v
	return s
}

func (s *Params) SetBodyType(v string) *Params {
	s.BodyType = &v
	return s
}

func (s *Params) SetReqBodyType(v string) *Params {
	s.ReqBodyType = &v
	return s
}

func (s *Params) SetStyle(v string) *Params {
	s.Style = &v
	return s
}
