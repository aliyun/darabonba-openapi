// This file is auto-generated, don't edit it. Thanks.
#ifndef ALIBABACLOUD_OPENAPI_HPP_
#define ALIBABACLOUD_OPENAPI_HPP_
#include <darabonba/Core.hpp>
#include <alibabacloud/OpenapiModel.hpp>
#include <alibabacloud/OpenapiException.hpp>
#include <alibabacloud/Utils.hpp>
#include <darabonba/Runtime.hpp>
#include <alibabacloud/gateway/SPI.hpp>
#include <map>
#include <alibabacloud/credentials/Client.hpp>
#include <darabonba/policy/Retry.hpp>
using namespace std;
using json = nlohmann::json;
namespace AlibabaCloud
{
namespace OpenApi
{
  class Client {
    public:

    /**
     * Init client with Config
     * @param config config contains the necessary information to create a client
     */
      Client(AlibabaCloud::OpenApi::Utils::Models::Config &config);

      Darabonba::Json doRPCRequest(const string &action, const string &version, const string &protocol, const string &method, const string &authType, const string &bodyType, const Utils::Models::OpenApiRequest &request, const Darabonba::RuntimeOptions &runtime);

      Darabonba::Json doROARequest(const string &action, const string &version, const string &protocol, const string &method, const string &authType, const string &pathname, const string &bodyType, const Utils::Models::OpenApiRequest &request, const Darabonba::RuntimeOptions &runtime);

      Darabonba::Json doROARequestWithForm(const string &action, const string &version, const string &protocol, const string &method, const string &authType, const string &pathname, const string &bodyType, const Utils::Models::OpenApiRequest &request, const Darabonba::RuntimeOptions &runtime);

      Darabonba::Json doRequest(const Utils::Models::Params &params, const Utils::Models::OpenApiRequest &request, const Darabonba::RuntimeOptions &runtime);

      Darabonba::Json execute(const Utils::Models::Params &params, const Utils::Models::OpenApiRequest &request, const Darabonba::RuntimeOptions &runtime);

      Darabonba::Json callApi(const Utils::Models::Params &params, const Utils::Models::OpenApiRequest &request, const Darabonba::RuntimeOptions &runtime);

      /**
       * Get accesskey id by using credential
       * @return accesskey id
       */
      string getAccessKeyId();

      /**
       * Get accesskey secret by using credential
       * @return accesskey secret
       */
      string getAccessKeySecret();

      /**
       * Get security token by using credential
       * @return security token
       */
      string getSecurityToken();

      /**
       * Get bearer token by credential
       * @return bearer token
       */
      string getBearerToken();

      /**
       * Get credential type by credential
       * @return credential type e.g. access_key
       */
      string getType();

      /**
       * If the endpointRule and config.endpoint are empty, throw error
       * @param config config contains the necessary information to create a client
       */
      void checkConfig(const AlibabaCloud::OpenApi::Utils::Models::Config &config);

      /**
       * set gateway client
       * @param spi.
       */
      void setGatewayClient(const shared_ptr<AlibabaCloud::Gateway::SPI> &spi);

      /**
       * set RPC header for debug
       * @param headers headers for debug, this header can be used only once.
       */
      void setRpcHeaders(const map<string, string> &headers);

      /**
       * get RPC header for debug
       */
      map<string, string> getRpcHeaders();

      json getAccessDeniedDetail(const json &err);
    protected:
      string _endpoint;

      string _regionId;

      string _protocol;

      string _method;

      string _userAgent;

      string _endpointRule;

      map<string, string> _endpointMap;

      string _suffix;

      int32_t _readTimeout;

      int32_t _connectTimeout;

      string _httpProxy;

      string _httpsProxy;

      string _socks5Proxy;

      string _socks5NetWork;

      string _noProxy;

      string _network;

      string _productId;

      int32_t _maxIdleConns;

      string _endpointType;

      string _openPlatformEndpoint;

      shared_ptr<AlibabaCloud::Credentials::Client> _credential;

      string _signatureVersion;

      string _signatureAlgorithm;

      map<string, string> _headers;

      shared_ptr<AlibabaCloud::Gateway::SPI> _spi;

      Utils::Models::GlobalParameters _globalParameters;

      string _key;

      string _cert;

      string _ca;

      bool _disableHttp2;

      Darabonba::Policy::RetryOptions _retryOptions;

      string _tlsMinVersion;

      AlibabaCloud::Gateway::Models::AttributeMap _attributeMap;
  };
} // namespace AlibabaCloud
} // namespace OpenApi
#endif
