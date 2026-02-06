#include <darabonba/Core.hpp>
#include <alibabacloud/Openapi.hpp>
#include <alibabacloud/Utils.hpp>
#include <map>
#include <alibabacloud/credentials/Client.hpp>
#include <darabonba/Runtime.hpp>
#include <darabonba/policy/Retry.hpp>
#include <darabonba/Exception.hpp>
#include <darabonba/Convert.hpp>
#include <darabonba/Stream.hpp>
#include <darabonba/http/Form.hpp>
#include <darabonba/Bytes.hpp>
#include <darabonba/encode/Encoder.hpp>
#include <darabonba/XML.hpp>
#include <alibabacloud/gateway/SPI.hpp>
using namespace std;
using json = nlohmann::json;
using namespace AlibabaCloud::OpenApi;
using namespace AlibabaCloud::Gateway;
using namespace AlibabaCloud::Gateway::Models;
using namespace AlibabaCloud::OpenApi::Exceptions;
using namespace AlibabaCloud::Credentials::Models;
using namespace AlibabaCloud::OpenApi::Utils::Models;
using CredentialClient = AlibabaCloud::Credentials::Client;
namespace AlibabaCloud
{
namespace OpenApi
{

/**
 * Init client with Config
 * @param config config contains the necessary information to create a client
 */
AlibabaCloud::OpenApi::Client::Client(AlibabaCloud::OpenApi::Utils::Models::Config &config){
  if (config.empty()) {
    throw ClientException(json({
      {"code" , "ParameterMissing"},
      {"message" , "'config' can not be unset"}
    }).get<map<string, string>>());
  }

  if ((!!config.hasAccessKeyId() && config.getAccessKeyId() != "") && (!!config.hasAccessKeySecret() && config.getAccessKeySecret() != "")) {
    if (!!config.hasSecurityToken() && config.getSecurityToken() != "") {
      config.setType("sts");
    } else {
      config.setType("access_key");
    }

    AlibabaCloud::Credentials::Models::Config credentialConfig = AlibabaCloud::Credentials::Models::Config(json({
      {"accessKeyId" , config.getAccessKeyId()},
      {"type" , config.getType()},
      {"accessKeySecret" , config.getAccessKeySecret()}
    }).get<map<string, string>>());
    credentialConfig.setSecurityToken(config.getSecurityToken());
    this->_credential = make_shared<CredentialClient>(credentialConfig);
  } else if (!!config.hasBearerToken() && config.getBearerToken() != "") {
    AlibabaCloud::Credentials::Models::Config cc = AlibabaCloud::Credentials::Models::Config(json({
      {"type" , "bearer"},
      {"bearerToken" , config.getBearerToken()}
    }).get<map<string, string>>());
    this->_credential = make_shared<CredentialClient>(cc);
  } else if (!!config.hasCredential()) {
    this->_credential = config.getCredential();
  }

  this->_endpoint = config.getEndpoint();
  this->_endpointType = config.getEndpointType();
  this->_network = config.getNetwork();
  this->_suffix = config.getSuffix();
  this->_protocol = config.getProtocol();
  this->_method = config.getMethod();
  this->_regionId = config.getRegionId();
  this->_userAgent = config.getUserAgent();
  this->_readTimeout = config.getReadTimeout();
  this->_connectTimeout = config.getConnectTimeout();
  this->_httpProxy = config.getHttpProxy();
  this->_httpsProxy = config.getHttpsProxy();
  this->_noProxy = config.getNoProxy();
  this->_socks5Proxy = config.getSocks5Proxy();
  this->_socks5NetWork = config.getSocks5NetWork();
  this->_maxIdleConns = config.getMaxIdleConns();
  this->_signatureVersion = config.getSignatureVersion();
  this->_signatureAlgorithm = config.getSignatureAlgorithm();
  this->_globalParameters = config.getGlobalParameters();
  this->_key = config.getKey();
  this->_cert = config.getCert();
  this->_ca = config.getCa();
  this->_disableHttp2 = config.getDisableHttp2();
  this->_retryOptions = config.getRetryOptions();
  this->_tlsMinVersion = config.getTlsMinVersion();
}


Darabonba::Json Client::doRPCRequest(const string &action, const string &version, const string &protocol, const string &method, const string &authType, const string &bodyType, const OpenApiRequest &request, const Darabonba::RuntimeOptions &runtime) {
  Darabonba::RuntimeOptions runtime_(json({
    {"key", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getKey(), _key))},
    {"cert", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getCert(), _cert))},
    {"ca", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getCa(), _ca))},
    {"readTimeout", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getReadTimeout(), _readTimeout))},
    {"connectTimeout", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getConnectTimeout(), _connectTimeout))},
    {"httpProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getHttpProxy(), _httpProxy))},
    {"httpsProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getHttpsProxy(), _httpsProxy))},
    {"noProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getNoProxy(), _noProxy))},
    {"socks5Proxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getSocks5Proxy(), _socks5Proxy))},
    {"socks5NetWork", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getSocks5NetWork(), _socks5NetWork))},
    {"maxIdleConns", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getMaxIdleConns(), _maxIdleConns))},
    {"retryOptions", _retryOptions},
    {"ignoreSSL", runtime.getIgnoreSSL()},
    {"tlsMinVersion", _tlsMinVersion}
    }));

  shared_ptr<Darabonba::Http::Request> _lastRequest = nullptr;
  shared_ptr<Darabonba::Http::MCurlResponse> _lastResponse = nullptr;
  int _retriesAttempted = 0;
  Darabonba::Policy::RetryPolicyContext _context = json({
    {"retriesAttempted" , _retriesAttempted}
  });
  while (Darabonba::allowRetry(runtime_.getRetryOptions(), _context)) {
    if (_retriesAttempted > 0) {
      int _backoffTime = Darabonba::getBackoffTime(runtime_.getRetryOptions(), _context);
      if (_backoffTime > 0) {
        Darabonba::sleep(_backoffTime);
      }
    }
    _retriesAttempted++;
    try {
      Darabonba::Http::Request request_ = Darabonba::Http::Request();
      request_.setProtocol(Darabonba::Convert::stringVal(Darabonba::defaultVal(_protocol, protocol)));
      request_.setMethod(method);
      request_.setPathname("/");
      map<string, string> globalQueries = {};
      map<string, string> globalHeaders = {};
      if (!Darabonba::isNull(_globalParameters)) {
        GlobalParameters globalParams = _globalParameters;
        if (!!globalParams.hasQueries()) {
          globalQueries = globalParams.getQueries();
        }

        if (!!globalParams.hasHeaders()) {
          globalHeaders = globalParams.getHeaders();
        }

      }

      map<string, string> extendsHeaders = {};
      map<string, string> extendsQueries = {};
      if (!!runtime.hasExtendsParameters()) {
        Darabonba::ExtendsParameters extendsParameters = runtime.getExtendsParameters();
        if (!!extendsParameters.hasHeaders()) {
          extendsHeaders = extendsParameters.getHeaders();
        }

        if (!!extendsParameters.hasQueries()) {
          extendsQueries = extendsParameters.getQueries();
        }

      }

      request_.setQuery(Darabonba::Core::merge(json({
          {"Action" , action},
          {"Format" , "json"},
          {"Version" , version},
          {"Timestamp" , Utils::Utils::getTimestamp()},
          {"SignatureNonce" , Utils::Utils::getNonce()}
        }),
        globalQueries,
        extendsQueries,
        request.getQuery()
      ).get<map<string, string>>());
      map<string, string> headers = getRpcHeaders();
      if (Darabonba::isNull(headers)) {
        // endpoint is setted in product client
        request_.setHeaders(Darabonba::Core::merge(json({
            {"host" , _endpoint},
            {"x-acs-version" , version},
            {"x-acs-action" , action},
            {"user-agent" , Utils::Utils::getUserAgent(_userAgent)}
          }),
          globalHeaders,
          extendsHeaders,
          request.getHeaders()
        ).get<map<string, string>>());
      } else {
        request_.setHeaders(Darabonba::Core::merge(json({
            {"host" , _endpoint},
            {"x-acs-version" , version},
            {"x-acs-action" , action},
            {"user-agent" , Utils::Utils::getUserAgent(_userAgent)}
          }),
          globalHeaders,
          extendsHeaders,
          request.getHeaders(),
          headers
        ).get<map<string, string>>());
      }

      if (!!request.hasBody()) {
        json m = json(request.getBody());
        json tmp = json(Utils::Utils::query(m));
        request_.setBody(Darabonba::Stream::toReadable(Darabonba::Http::Form::toFormString(tmp)));
        request_.getHeaders()["content-type"] = "application/x-www-form-urlencoded";
      }

      if (authType != "Anonymous") {
        if (Darabonba::isNull(_credential)) {
          throw ClientException(json({
            {"code" , DARA_STRING_TEMPLATE("InvalidCredentials")},
            {"message" , DARA_STRING_TEMPLATE("Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.")}
          }).get<map<string, string>>());
        }

        CredentialModel credentialModel = _credential->getCredential();
        if (!!credentialModel.hasProviderName()) {
          request_.getHeaders()["x-acs-credentials-provider"] = credentialModel.getProviderName();
        }

        string credentialType = credentialModel.getType();
        if (credentialType == "bearer") {
          string bearerToken = credentialModel.getBearerToken();
          request_.getQuery()["BearerToken"] = bearerToken;
          request_.getQuery()["SignatureType"] = "BEARERTOKEN";
        } else if (credentialType == "id_token") {
          string idToken = credentialModel.getSecurityToken();
          request_.getHeaders()["x-acs-zero-trust-idtoken"] = idToken;
        } else {
          string accessKeyId = credentialModel.getAccessKeyId();
          string accessKeySecret = credentialModel.getAccessKeySecret();
          string securityToken = credentialModel.getSecurityToken();
          if (!Darabonba::isNull(securityToken) && securityToken != "") {
            request_.getQuery()["SecurityToken"] = securityToken;
          }

          request_.getQuery()["SignatureMethod"] = "HMAC-SHA1";
          request_.getQuery()["SignatureVersion"] = "1.0";
          request_.getQuery()["AccessKeyId"] = accessKeyId;
          json t = nullptr;
          if (!!request.hasBody()) {
            t = json(request.getBody());
          }

          map<string, string> signedParam = Darabonba::Core::merge(request_.getQuery(),
            Utils::Utils::query(t)
          ).get<map<string, string>>();
          request_.getQuery()["Signature"] = Utils::Utils::getRPCSignature(signedParam, request_.getMethod(), accessKeySecret);
        }

      }

      _lastRequest = make_shared<Darabonba::Http::Request>(request_);
      auto futureResp_ = Darabonba::Core::doAction(request_, runtime_);
      shared_ptr<Darabonba::Http::MCurlResponse> response_ = futureResp_.get();
      _lastResponse  = response_;

      if ((response_->getStatusCode() >= 400) && (response_->getStatusCode() < 600)) {
        Darabonba::Json _res = Darabonba::Stream::readAsJSON(response_->getBody());
        json err = json(_res);
        Darabonba::Json requestId = Darabonba::defaultVal(err["RequestId"], err["requestId"]);
        Darabonba::Json code = Darabonba::defaultVal(err["Code"], err["code"]);
        if ((DARA_STRING_TEMPLATE("" , code) == "Throttling") || (DARA_STRING_TEMPLATE("" , code) == "Throttling.User") || (DARA_STRING_TEMPLATE("" , code) == "Throttling.Api")) {
          throw ThrottlingException(json({
            {"statusCode" , response_->getStatusCode()},
            {"code" , DARA_STRING_TEMPLATE("" , code)},
            {"message" , DARA_STRING_TEMPLATE("code: " , response_->getStatusCode() , ", " , Darabonba::defaultVal(err["Message"], err["message"]) , " request id: " , requestId)},
            {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err["Description"], err["description"]))},
            {"retryAfter" , Utils::Utils::getThrottlingTimeLeft(response_->getHeaders())},
            {"data" , err},
            {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
          }));
        } else if ((response_->getStatusCode() >= 400) && (response_->getStatusCode() < 500)) {
          throw ClientException(json({
            {"statusCode" , response_->getStatusCode()},
            {"code" , DARA_STRING_TEMPLATE("" , code)},
            {"message" , DARA_STRING_TEMPLATE("code: " , response_->getStatusCode() , ", " , Darabonba::defaultVal(err["Message"], err["message"]) , " request id: " , requestId)},
            {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err["Description"], err["description"]))},
            {"data" , err},
            {"accessDeniedDetail" , getAccessDeniedDetail(err)},
            {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
          }));
        } else {
          throw ServerException(json({
            {"statusCode" , response_->getStatusCode()},
            {"code" , DARA_STRING_TEMPLATE("" , code)},
            {"message" , DARA_STRING_TEMPLATE("code: " , response_->getStatusCode() , ", " , Darabonba::defaultVal(err["Message"], err["message"]) , " request id: " , requestId)},
            {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err["Description"], err["description"]))},
            {"data" , err},
            {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
          }));
        }

      }

      if (bodyType == "binary") {
        json resp = json({
          {"body" , response_->getBody()},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
        return resp;
      } else if (bodyType == "byte") {
        Darabonba::Bytes byt = Darabonba::Stream::readAsBytes(response_->getBody());
        return json({
          {"body" , byt},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else if (bodyType == "string") {
        string _str = Darabonba::Stream::readAsString(response_->getBody());
        return json({
          {"body" , _str},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else if (bodyType == "json") {
        Darabonba::Json obj = Darabonba::Stream::readAsJSON(response_->getBody());
        json res = json(obj);
        return json({
          {"body" , res},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else if (bodyType == "array") {
        Darabonba::Json arr = Darabonba::Stream::readAsJSON(response_->getBody());
        return json({
          {"body" , arr},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else {
        return json({
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      }

    } catch (const Darabonba::DaraException& ex) {
      _context = Darabonba::Policy::RetryPolicyContext(json({
        {"retriesAttempted" , _retriesAttempted},
        {"lastRequest" , _lastRequest},
        {"lastResponse" , _lastResponse},
        {"exception" , ex},
      }));
      continue;
    }
  }

  throw Darabonba::UnretryableException(_context);
}

Darabonba::Json Client::doROARequest(const string &action, const string &version, const string &protocol, const string &method, const string &authType, const string &pathname, const string &bodyType, const OpenApiRequest &request, const Darabonba::RuntimeOptions &runtime) {
  Darabonba::RuntimeOptions runtime_(json({
    {"key", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getKey(), _key))},
    {"cert", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getCert(), _cert))},
    {"ca", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getCa(), _ca))},
    {"readTimeout", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getReadTimeout(), _readTimeout))},
    {"connectTimeout", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getConnectTimeout(), _connectTimeout))},
    {"httpProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getHttpProxy(), _httpProxy))},
    {"httpsProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getHttpsProxy(), _httpsProxy))},
    {"noProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getNoProxy(), _noProxy))},
    {"socks5Proxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getSocks5Proxy(), _socks5Proxy))},
    {"socks5NetWork", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getSocks5NetWork(), _socks5NetWork))},
    {"maxIdleConns", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getMaxIdleConns(), _maxIdleConns))},
    {"retryOptions", _retryOptions},
    {"ignoreSSL", runtime.getIgnoreSSL()},
    {"tlsMinVersion", _tlsMinVersion}
    }));

  shared_ptr<Darabonba::Http::Request> _lastRequest = nullptr;
  shared_ptr<Darabonba::Http::MCurlResponse> _lastResponse = nullptr;
  int _retriesAttempted = 0;
  Darabonba::Policy::RetryPolicyContext _context = json({
    {"retriesAttempted" , _retriesAttempted}
  });
  while (Darabonba::allowRetry(runtime_.getRetryOptions(), _context)) {
    if (_retriesAttempted > 0) {
      int _backoffTime = Darabonba::getBackoffTime(runtime_.getRetryOptions(), _context);
      if (_backoffTime > 0) {
        Darabonba::sleep(_backoffTime);
      }
    }
    _retriesAttempted++;
    try {
      Darabonba::Http::Request request_ = Darabonba::Http::Request();
      request_.setProtocol(Darabonba::Convert::stringVal(Darabonba::defaultVal(_protocol, protocol)));
      request_.setMethod(method);
      request_.setPathname(pathname);
      map<string, string> globalQueries = {};
      map<string, string> globalHeaders = {};
      if (!Darabonba::isNull(_globalParameters)) {
        GlobalParameters globalParams = _globalParameters;
        if (!!globalParams.hasQueries()) {
          globalQueries = globalParams.getQueries();
        }

        if (!!globalParams.hasHeaders()) {
          globalHeaders = globalParams.getHeaders();
        }

      }

      map<string, string> extendsHeaders = {};
      map<string, string> extendsQueries = {};
      if (!!runtime.hasExtendsParameters()) {
        Darabonba::ExtendsParameters extendsParameters = runtime.getExtendsParameters();
        if (!!extendsParameters.hasHeaders()) {
          extendsHeaders = extendsParameters.getHeaders();
        }

        if (!!extendsParameters.hasQueries()) {
          extendsQueries = extendsParameters.getQueries();
        }

      }

      request_.setHeaders(Darabonba::Core::merge(json({
          {"date" , Utils::Utils::getDateUTCString()},
          {"host" , _endpoint},
          {"accept" , "application/json"},
          {"x-acs-signature-nonce" , Utils::Utils::getNonce()},
          {"x-acs-signature-method" , "HMAC-SHA1"},
          {"x-acs-signature-version" , "1.0"},
          {"x-acs-version" , version},
          {"x-acs-action" , action},
          {"user-agent" , Utils::Utils::getUserAgent(_userAgent)}
        }),
        globalHeaders,
        extendsHeaders,
        request.getHeaders()
      ).get<map<string, string>>());
      if (!!request.hasBody()) {
        request_.setBody(Darabonba::Stream::toReadable(json(request.getBody()).dump()));
        request_.getHeaders()["content-type"] = "application/json; charset=utf-8";
      }

      request_.setQuery(Darabonba::Core::merge(globalQueries,
        extendsQueries
      ).get<map<string, string>>());
      if (!!request.hasQuery()) {
        request_.setQuery(Darabonba::Core::merge(request_.getQuery(),
          request.getQuery()
        ).get<map<string, string>>());
      }

      if (authType != "Anonymous") {
        if (Darabonba::isNull(_credential)) {
          throw ClientException(json({
            {"code" , DARA_STRING_TEMPLATE("InvalidCredentials")},
            {"message" , DARA_STRING_TEMPLATE("Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.")}
          }).get<map<string, string>>());
        }

        CredentialModel credentialModel = _credential->getCredential();
        if (!!credentialModel.hasProviderName()) {
          request_.getHeaders()["x-acs-credentials-provider"] = credentialModel.getProviderName();
        }

        string credentialType = credentialModel.getType();
        if (credentialType == "bearer") {
          string bearerToken = credentialModel.getBearerToken();
          request_.getHeaders()["x-acs-bearer-token"] = bearerToken;
          request_.getHeaders()["x-acs-signature-type"] = "BEARERTOKEN";
        } else if (credentialType == "id_token") {
          string idToken = credentialModel.getSecurityToken();
          request_.getHeaders()["x-acs-zero-trust-idtoken"] = idToken;
        } else {
          string accessKeyId = credentialModel.getAccessKeyId();
          string accessKeySecret = credentialModel.getAccessKeySecret();
          string securityToken = credentialModel.getSecurityToken();
          if (!Darabonba::isNull(securityToken) && securityToken != "") {
            request_.getHeaders()["x-acs-accesskey-id"] = accessKeyId;
            request_.getHeaders()["x-acs-security-token"] = securityToken;
          }

          string stringToSign = Utils::Utils::getStringToSign(request_);
          request_.getHeaders()["authorization"] = DARA_STRING_TEMPLATE("acs " , accessKeyId , ":" , Utils::Utils::getROASignature(stringToSign, accessKeySecret));
        }

      }

      _lastRequest = make_shared<Darabonba::Http::Request>(request_);
      auto futureResp_ = Darabonba::Core::doAction(request_, runtime_);
      shared_ptr<Darabonba::Http::MCurlResponse> response_ = futureResp_.get();
      _lastResponse  = response_;

      if (response_->getStatusCode() == 204) {
        return json({
          {"headers" , response_->getHeaders()}
        }).get<map<string, map<string, string>>>();
      }

      if ((response_->getStatusCode() >= 400) && (response_->getStatusCode() < 600)) {
        Darabonba::Json _res = Darabonba::Stream::readAsJSON(response_->getBody());
        json err = json(_res);
        string requestId = Darabonba::Convert::stringVal(Darabonba::defaultVal(err["RequestId"], err["requestId"]));
        requestId = Darabonba::Convert::stringVal(Darabonba::defaultVal(requestId, err["requestid"]));
        string code = Darabonba::Convert::stringVal(Darabonba::defaultVal(err["Code"], err["code"]));
        if ((DARA_STRING_TEMPLATE("" , code) == "Throttling") || (DARA_STRING_TEMPLATE("" , code) == "Throttling.User") || (DARA_STRING_TEMPLATE("" , code) == "Throttling.Api")) {
          throw ThrottlingException(json({
            {"statusCode" , response_->getStatusCode()},
            {"code" , DARA_STRING_TEMPLATE("" , code)},
            {"message" , DARA_STRING_TEMPLATE("code: " , response_->getStatusCode() , ", " , Darabonba::defaultVal(err["Message"], err["message"]) , " request id: " , requestId)},
            {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err["Description"], err["description"]))},
            {"retryAfter" , Utils::Utils::getThrottlingTimeLeft(response_->getHeaders())},
            {"data" , err},
            {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
          }));
        } else if ((response_->getStatusCode() >= 400) && (response_->getStatusCode() < 500)) {
          throw ClientException(json({
            {"statusCode" , response_->getStatusCode()},
            {"code" , DARA_STRING_TEMPLATE("" , code)},
            {"message" , DARA_STRING_TEMPLATE("code: " , response_->getStatusCode() , ", " , Darabonba::defaultVal(err["Message"], err["message"]) , " request id: " , requestId)},
            {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err["Description"], err["description"]))},
            {"data" , err},
            {"accessDeniedDetail" , getAccessDeniedDetail(err)},
            {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
          }));
        } else {
          throw ServerException(json({
            {"statusCode" , response_->getStatusCode()},
            {"code" , DARA_STRING_TEMPLATE("" , code)},
            {"message" , DARA_STRING_TEMPLATE("code: " , response_->getStatusCode() , ", " , Darabonba::defaultVal(err["Message"], err["message"]) , " request id: " , requestId)},
            {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err["Description"], err["description"]))},
            {"data" , err},
            {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
          }));
        }

      }

      if (bodyType == "binary") {
        json resp = json({
          {"body" , response_->getBody()},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
        return resp;
      } else if (bodyType == "byte") {
        Darabonba::Bytes byt = Darabonba::Stream::readAsBytes(response_->getBody());
        return json({
          {"body" , byt},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else if (bodyType == "string") {
        string _str = Darabonba::Stream::readAsString(response_->getBody());
        return json({
          {"body" , _str},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else if (bodyType == "json") {
        Darabonba::Json obj = Darabonba::Stream::readAsJSON(response_->getBody());
        json res = json(obj);
        return json({
          {"body" , res},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else if (bodyType == "array") {
        Darabonba::Json arr = Darabonba::Stream::readAsJSON(response_->getBody());
        return json({
          {"body" , arr},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else {
        return json({
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      }

    } catch (const Darabonba::DaraException& ex) {
      _context = Darabonba::Policy::RetryPolicyContext(json({
        {"retriesAttempted" , _retriesAttempted},
        {"lastRequest" , _lastRequest},
        {"lastResponse" , _lastResponse},
        {"exception" , ex},
      }));
      continue;
    }
  }

  throw Darabonba::UnretryableException(_context);
}

Darabonba::Json Client::doROARequestWithForm(const string &action, const string &version, const string &protocol, const string &method, const string &authType, const string &pathname, const string &bodyType, const OpenApiRequest &request, const Darabonba::RuntimeOptions &runtime) {
  Darabonba::RuntimeOptions runtime_(json({
    {"key", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getKey(), _key))},
    {"cert", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getCert(), _cert))},
    {"ca", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getCa(), _ca))},
    {"readTimeout", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getReadTimeout(), _readTimeout))},
    {"connectTimeout", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getConnectTimeout(), _connectTimeout))},
    {"httpProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getHttpProxy(), _httpProxy))},
    {"httpsProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getHttpsProxy(), _httpsProxy))},
    {"noProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getNoProxy(), _noProxy))},
    {"socks5Proxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getSocks5Proxy(), _socks5Proxy))},
    {"socks5NetWork", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getSocks5NetWork(), _socks5NetWork))},
    {"maxIdleConns", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getMaxIdleConns(), _maxIdleConns))},
    {"retryOptions", _retryOptions},
    {"ignoreSSL", runtime.getIgnoreSSL()},
    {"tlsMinVersion", _tlsMinVersion}
    }));

  shared_ptr<Darabonba::Http::Request> _lastRequest = nullptr;
  shared_ptr<Darabonba::Http::MCurlResponse> _lastResponse = nullptr;
  int _retriesAttempted = 0;
  Darabonba::Policy::RetryPolicyContext _context = json({
    {"retriesAttempted" , _retriesAttempted}
  });
  while (Darabonba::allowRetry(runtime_.getRetryOptions(), _context)) {
    if (_retriesAttempted > 0) {
      int _backoffTime = Darabonba::getBackoffTime(runtime_.getRetryOptions(), _context);
      if (_backoffTime > 0) {
        Darabonba::sleep(_backoffTime);
      }
    }
    _retriesAttempted++;
    try {
      Darabonba::Http::Request request_ = Darabonba::Http::Request();
      request_.setProtocol(Darabonba::Convert::stringVal(Darabonba::defaultVal(_protocol, protocol)));
      request_.setMethod(method);
      request_.setPathname(pathname);
      map<string, string> globalQueries = {};
      map<string, string> globalHeaders = {};
      if (!Darabonba::isNull(_globalParameters)) {
        GlobalParameters globalParams = _globalParameters;
        if (!!globalParams.hasQueries()) {
          globalQueries = globalParams.getQueries();
        }

        if (!!globalParams.hasHeaders()) {
          globalHeaders = globalParams.getHeaders();
        }

      }

      map<string, string> extendsHeaders = {};
      map<string, string> extendsQueries = {};
      if (!!runtime.hasExtendsParameters()) {
        Darabonba::ExtendsParameters extendsParameters = runtime.getExtendsParameters();
        if (!!extendsParameters.hasHeaders()) {
          extendsHeaders = extendsParameters.getHeaders();
        }

        if (!!extendsParameters.hasQueries()) {
          extendsQueries = extendsParameters.getQueries();
        }

      }

      request_.setHeaders(Darabonba::Core::merge(json({
          {"date" , Utils::Utils::getDateUTCString()},
          {"host" , _endpoint},
          {"accept" , "application/json"},
          {"x-acs-signature-nonce" , Utils::Utils::getNonce()},
          {"x-acs-signature-method" , "HMAC-SHA1"},
          {"x-acs-signature-version" , "1.0"},
          {"x-acs-version" , version},
          {"x-acs-action" , action},
          {"user-agent" , Utils::Utils::getUserAgent(_userAgent)}
        }),
        globalHeaders,
        extendsHeaders,
        request.getHeaders()
      ).get<map<string, string>>());
      if (!!request.hasBody()) {
        json m = json(request.getBody());
        request_.setBody(Darabonba::Stream::toReadable(Utils::Utils::toForm(m)));
        request_.getHeaders()["content-type"] = "application/x-www-form-urlencoded";
      }

      request_.setQuery(Darabonba::Core::merge(globalQueries,
        extendsQueries
      ).get<map<string, string>>());
      if (!!request.hasQuery()) {
        request_.setQuery(Darabonba::Core::merge(request_.getQuery(),
          request.getQuery()
        ).get<map<string, string>>());
      }

      if (authType != "Anonymous") {
        if (Darabonba::isNull(_credential)) {
          throw ClientException(json({
            {"code" , DARA_STRING_TEMPLATE("InvalidCredentials")},
            {"message" , DARA_STRING_TEMPLATE("Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.")}
          }).get<map<string, string>>());
        }

        CredentialModel credentialModel = _credential->getCredential();
        if (!!credentialModel.hasProviderName()) {
          request_.getHeaders()["x-acs-credentials-provider"] = credentialModel.getProviderName();
        }

        string credentialType = credentialModel.getType();
        if (credentialType == "bearer") {
          string bearerToken = credentialModel.getBearerToken();
          request_.getHeaders()["x-acs-bearer-token"] = bearerToken;
          request_.getHeaders()["x-acs-signature-type"] = "BEARERTOKEN";
        } else if (credentialType == "id_token") {
          string idToken = credentialModel.getSecurityToken();
          request_.getHeaders()["x-acs-zero-trust-idtoken"] = idToken;
        } else {
          string accessKeyId = credentialModel.getAccessKeyId();
          string accessKeySecret = credentialModel.getAccessKeySecret();
          string securityToken = credentialModel.getSecurityToken();
          if (!Darabonba::isNull(securityToken) && securityToken != "") {
            request_.getHeaders()["x-acs-accesskey-id"] = accessKeyId;
            request_.getHeaders()["x-acs-security-token"] = securityToken;
          }

          string stringToSign = Utils::Utils::getStringToSign(request_);
          request_.getHeaders()["authorization"] = DARA_STRING_TEMPLATE("acs " , accessKeyId , ":" , Utils::Utils::getROASignature(stringToSign, accessKeySecret));
        }

      }

      _lastRequest = make_shared<Darabonba::Http::Request>(request_);
      auto futureResp_ = Darabonba::Core::doAction(request_, runtime_);
      shared_ptr<Darabonba::Http::MCurlResponse> response_ = futureResp_.get();
      _lastResponse  = response_;

      if (response_->getStatusCode() == 204) {
        return json({
          {"headers" , response_->getHeaders()}
        }).get<map<string, map<string, string>>>();
      }

      if ((response_->getStatusCode() >= 400) && (response_->getStatusCode() < 600)) {
        Darabonba::Json _res = Darabonba::Stream::readAsJSON(response_->getBody());
        json err = json(_res);
        string requestId = Darabonba::Convert::stringVal(Darabonba::defaultVal(err["RequestId"], err["requestId"]));
        string code = Darabonba::Convert::stringVal(Darabonba::defaultVal(err["Code"], err["code"]));
        if ((DARA_STRING_TEMPLATE("" , code) == "Throttling") || (DARA_STRING_TEMPLATE("" , code) == "Throttling.User") || (DARA_STRING_TEMPLATE("" , code) == "Throttling.Api")) {
          throw ThrottlingException(json({
            {"statusCode" , response_->getStatusCode()},
            {"code" , DARA_STRING_TEMPLATE("" , code)},
            {"message" , DARA_STRING_TEMPLATE("code: " , response_->getStatusCode() , ", " , Darabonba::defaultVal(err["Message"], err["message"]) , " request id: " , requestId)},
            {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err["Description"], err["description"]))},
            {"retryAfter" , Utils::Utils::getThrottlingTimeLeft(response_->getHeaders())},
            {"data" , err},
            {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
          }));
        } else if ((response_->getStatusCode() >= 400) && (response_->getStatusCode() < 500)) {
          throw ClientException(json({
            {"statusCode" , response_->getStatusCode()},
            {"code" , DARA_STRING_TEMPLATE("" , code)},
            {"message" , DARA_STRING_TEMPLATE("code: " , response_->getStatusCode() , ", " , Darabonba::defaultVal(err["Message"], err["message"]) , " request id: " , requestId)},
            {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err["Description"], err["description"]))},
            {"data" , err},
            {"accessDeniedDetail" , getAccessDeniedDetail(err)},
            {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
          }));
        } else {
          throw ServerException(json({
            {"statusCode" , response_->getStatusCode()},
            {"code" , DARA_STRING_TEMPLATE("" , code)},
            {"message" , DARA_STRING_TEMPLATE("code: " , response_->getStatusCode() , ", " , Darabonba::defaultVal(err["Message"], err["message"]) , " request id: " , requestId)},
            {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err["Description"], err["description"]))},
            {"data" , err},
            {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
          }));
        }

      }

      if (bodyType == "binary") {
        json resp = json({
          {"body" , response_->getBody()},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
        return resp;
      } else if (bodyType == "byte") {
        Darabonba::Bytes byt = Darabonba::Stream::readAsBytes(response_->getBody());
        return json({
          {"body" , byt},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else if (bodyType == "string") {
        string _str = Darabonba::Stream::readAsString(response_->getBody());
        return json({
          {"body" , _str},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else if (bodyType == "json") {
        Darabonba::Json obj = Darabonba::Stream::readAsJSON(response_->getBody());
        json res = json(obj);
        return json({
          {"body" , res},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else if (bodyType == "array") {
        Darabonba::Json arr = Darabonba::Stream::readAsJSON(response_->getBody());
        return json({
          {"body" , arr},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else {
        return json({
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      }

    } catch (const Darabonba::DaraException& ex) {
      _context = Darabonba::Policy::RetryPolicyContext(json({
        {"retriesAttempted" , _retriesAttempted},
        {"lastRequest" , _lastRequest},
        {"lastResponse" , _lastResponse},
        {"exception" , ex},
      }));
      continue;
    }
  }

  throw Darabonba::UnretryableException(_context);
}

Darabonba::Json Client::doRequest(const Params &params, const OpenApiRequest &request, const Darabonba::RuntimeOptions &runtime) {
  Darabonba::RuntimeOptions runtime_(json({
    {"key", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getKey(), _key))},
    {"cert", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getCert(), _cert))},
    {"ca", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getCa(), _ca))},
    {"readTimeout", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getReadTimeout(), _readTimeout))},
    {"connectTimeout", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getConnectTimeout(), _connectTimeout))},
    {"httpProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getHttpProxy(), _httpProxy))},
    {"httpsProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getHttpsProxy(), _httpsProxy))},
    {"noProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getNoProxy(), _noProxy))},
    {"socks5Proxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getSocks5Proxy(), _socks5Proxy))},
    {"socks5NetWork", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getSocks5NetWork(), _socks5NetWork))},
    {"maxIdleConns", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getMaxIdleConns(), _maxIdleConns))},
    {"retryOptions", _retryOptions},
    {"ignoreSSL", runtime.getIgnoreSSL()},
    {"tlsMinVersion", _tlsMinVersion}
    }));

  shared_ptr<Darabonba::Http::Request> _lastRequest = nullptr;
  shared_ptr<Darabonba::Http::MCurlResponse> _lastResponse = nullptr;
  int _retriesAttempted = 0;
  Darabonba::Policy::RetryPolicyContext _context = json({
    {"retriesAttempted" , _retriesAttempted}
  });
  while (Darabonba::allowRetry(runtime_.getRetryOptions(), _context)) {
    if (_retriesAttempted > 0) {
      int _backoffTime = Darabonba::getBackoffTime(runtime_.getRetryOptions(), _context);
      if (_backoffTime > 0) {
        Darabonba::sleep(_backoffTime);
      }
    }
    _retriesAttempted++;
    try {
      Darabonba::Http::Request request_ = Darabonba::Http::Request();
      request_.setProtocol(Darabonba::Convert::stringVal(Darabonba::defaultVal(_protocol, params.getProtocol())));
      request_.setMethod(params.getMethod());
      request_.setPathname(params.getPathname());
      map<string, string> globalQueries = {};
      map<string, string> globalHeaders = {};
      if (!Darabonba::isNull(_globalParameters)) {
        GlobalParameters globalParams = _globalParameters;
        if (!!globalParams.hasQueries()) {
          globalQueries = globalParams.getQueries();
        }

        if (!!globalParams.hasHeaders()) {
          globalHeaders = globalParams.getHeaders();
        }

      }

      map<string, string> extendsHeaders = {};
      map<string, string> extendsQueries = {};
      if (!!runtime.hasExtendsParameters()) {
        Darabonba::ExtendsParameters extendsParameters = runtime.getExtendsParameters();
        if (!!extendsParameters.hasHeaders()) {
          extendsHeaders = extendsParameters.getHeaders();
        }

        if (!!extendsParameters.hasQueries()) {
          extendsQueries = extendsParameters.getQueries();
        }

      }

      request_.setQuery(Darabonba::Core::merge(globalQueries,
        extendsQueries,
        request.getQuery()
      ).get<map<string, string>>());
      // endpoint is setted in product client
      request_.setHeaders(Darabonba::Core::merge(json({
          {"host" , _endpoint},
          {"x-acs-version" , params.getVersion()},
          {"x-acs-action" , params.getAction()},
          {"user-agent" , Utils::Utils::getUserAgent(_userAgent)},
          {"x-acs-date" , Utils::Utils::getTimestamp()},
          {"x-acs-signature-nonce" , Utils::Utils::getNonce()},
          {"accept" , "application/json"}
        }),
        globalHeaders,
        extendsHeaders,
        request.getHeaders()
      ).get<map<string, string>>());
      if (params.getStyle() == "RPC") {
        map<string, string> headers = getRpcHeaders();
        if (!Darabonba::isNull(headers)) {
          request_.setHeaders(Darabonba::Core::merge(request_.getHeaders(),
            headers
          ).get<map<string, string>>());
        }

      }

      string signatureAlgorithm = Darabonba::Convert::stringVal(Darabonba::defaultVal(_signatureAlgorithm, "ACS3-HMAC-SHA256"));
      Darabonba::Bytes hashedRequestPayload = Utils::Utils::hash(Darabonba::BytesUtil::from("", "utf-8"), signatureAlgorithm);
      if (!!request.hasStream()) {
        Darabonba::Bytes tmp = Darabonba::Stream::readAsBytes(request.getStream());
        hashedRequestPayload = Utils::Utils::hash(tmp, signatureAlgorithm);
        request_.setBody(Darabonba::Stream::toReadable(tmp));
        request_.getHeaders()["content-type"] = "application/octet-stream";
      } else {
        if (!!request.hasBody()) {
          if (params.getReqBodyType() == "byte") {
            Darabonba::Bytes byteObj = Darabonba::BytesUtil::toBytes(request.getBody());
            hashedRequestPayload = Utils::Utils::hash(byteObj, signatureAlgorithm);
            request_.setBody(Darabonba::Stream::toReadable(byteObj));
          } else if (params.getReqBodyType() == "json") {
            string jsonObj = json(request.getBody()).dump();
            hashedRequestPayload = Utils::Utils::hash(Darabonba::BytesUtil::toBytes(jsonObj), signatureAlgorithm);
            request_.setBody(Darabonba::Stream::toReadable(jsonObj));
            request_.getHeaders()["content-type"] = "application/json; charset=utf-8";
          } else {
            json m = json(request.getBody());
            string formObj = Utils::Utils::toForm(m);
            hashedRequestPayload = Utils::Utils::hash(Darabonba::BytesUtil::toBytes(formObj), signatureAlgorithm);
            request_.setBody(Darabonba::Stream::toReadable(formObj));
            request_.getHeaders()["content-type"] = "application/x-www-form-urlencoded";
          }

        }

      }

      request_.getHeaders()["x-acs-content-sha256"] = Darabonba::Encode::Encoder::hexEncode(hashedRequestPayload);
      if (params.getAuthType() != "Anonymous") {
        if (Darabonba::isNull(_credential)) {
          throw ClientException(json({
            {"code" , DARA_STRING_TEMPLATE("InvalidCredentials")},
            {"message" , DARA_STRING_TEMPLATE("Please set up the credentials correctly. If you are setting them through environment variables, please ensure that ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set correctly. See https://help.aliyun.com/zh/sdk/developer-reference/configure-the-alibaba-cloud-accesskey-environment-variable-on-linux-macos-and-windows-systems for more details.")}
          }).get<map<string, string>>());
        }

        CredentialModel credentialModel = _credential->getCredential();
        if (!!credentialModel.hasProviderName()) {
          request_.getHeaders()["x-acs-credentials-provider"] = credentialModel.getProviderName();
        }

        string authType = credentialModel.getType();
        if (authType == "bearer") {
          string bearerToken = credentialModel.getBearerToken();
          request_.getHeaders()["x-acs-bearer-token"] = bearerToken;
          if (params.getStyle() == "RPC") {
            request_.getQuery()["SignatureType"] = "BEARERTOKEN";
          } else {
            request_.getHeaders()["x-acs-signature-type"] = "BEARERTOKEN";
          }

        } else if (authType == "id_token") {
          string idToken = credentialModel.getSecurityToken();
          request_.getHeaders()["x-acs-zero-trust-idtoken"] = idToken;
        } else {
          string accessKeyId = credentialModel.getAccessKeyId();
          string accessKeySecret = credentialModel.getAccessKeySecret();
          string securityToken = credentialModel.getSecurityToken();
          if (!Darabonba::isNull(securityToken) && securityToken != "") {
            request_.getHeaders()["x-acs-accesskey-id"] = accessKeyId;
            request_.getHeaders()["x-acs-security-token"] = securityToken;
          }

          request_.getHeaders()["Authorization"] = Utils::Utils::getAuthorization(request_, signatureAlgorithm, Darabonba::Encode::Encoder::hexEncode(hashedRequestPayload), accessKeyId, accessKeySecret);
        }

      }

      _lastRequest = make_shared<Darabonba::Http::Request>(request_);
      auto futureResp_ = Darabonba::Core::doAction(request_, runtime_);
      shared_ptr<Darabonba::Http::MCurlResponse> response_ = futureResp_.get();
      _lastResponse  = response_;

      if ((response_->getStatusCode() >= 400) && (response_->getStatusCode() < 600)) {
        json err = {};
        if (!Darabonba::isNull(response_->getHeaders().at("content-type")) && response_->getHeaders().at("content-type") == "text/xml;charset=utf-8") {
          string _str = Darabonba::Stream::readAsString(response_->getBody());
          json respMap = Darabonba::XML::parseXml(_str, nullptr);
          err = json(respMap["Error"]);
        } else {
          Darabonba::Json _res = Darabonba::Stream::readAsJSON(response_->getBody());
          err = json(_res);
        }

        string requestId = Darabonba::Convert::stringVal(Darabonba::defaultVal(err["RequestId"], err["requestId"]));
        string code = Darabonba::Convert::stringVal(Darabonba::defaultVal(err["Code"], err["code"]));
        if ((DARA_STRING_TEMPLATE("" , code) == "Throttling") || (DARA_STRING_TEMPLATE("" , code) == "Throttling.User") || (DARA_STRING_TEMPLATE("" , code) == "Throttling.Api")) {
          throw ThrottlingException(json({
            {"statusCode" , response_->getStatusCode()},
            {"code" , DARA_STRING_TEMPLATE("" , code)},
            {"message" , DARA_STRING_TEMPLATE("code: " , response_->getStatusCode() , ", " , Darabonba::defaultVal(err["Message"], err["message"]) , " request id: " , requestId)},
            {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err["Description"], err["description"]))},
            {"retryAfter" , Utils::Utils::getThrottlingTimeLeft(response_->getHeaders())},
            {"data" , err},
            {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
          }));
        } else if ((response_->getStatusCode() >= 400) && (response_->getStatusCode() < 500)) {
          throw ClientException(json({
            {"statusCode" , response_->getStatusCode()},
            {"code" , DARA_STRING_TEMPLATE("" , code)},
            {"message" , DARA_STRING_TEMPLATE("code: " , response_->getStatusCode() , ", " , Darabonba::defaultVal(err["Message"], err["message"]) , " request id: " , requestId)},
            {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err["Description"], err["description"]))},
            {"data" , err},
            {"accessDeniedDetail" , getAccessDeniedDetail(err)},
            {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
          }));
        } else {
          throw ServerException(json({
            {"statusCode" , response_->getStatusCode()},
            {"code" , DARA_STRING_TEMPLATE("" , code)},
            {"message" , DARA_STRING_TEMPLATE("code: " , response_->getStatusCode() , ", " , Darabonba::defaultVal(err["Message"], err["message"]) , " request id: " , requestId)},
            {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err["Description"], err["description"]))},
            {"data" , err},
            {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
          }));
        }

      }

      if (params.getBodyType() == "binary") {
        json resp = json({
          {"body" , response_->getBody()},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
        return resp;
      } else if (params.getBodyType() == "byte") {
        Darabonba::Bytes byt = Darabonba::Stream::readAsBytes(response_->getBody());
        return json({
          {"body" , byt},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else if (params.getBodyType() == "string") {
        string respStr = Darabonba::Stream::readAsString(response_->getBody());
        return json({
          {"body" , respStr},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else if (params.getBodyType() == "json") {
        Darabonba::Json obj = Darabonba::Stream::readAsJSON(response_->getBody());
        json res = json(obj);
        return json({
          {"body" , res},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else if (params.getBodyType() == "array") {
        Darabonba::Json arr = Darabonba::Stream::readAsJSON(response_->getBody());
        return json({
          {"body" , arr},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      } else {
        string anything = Darabonba::Stream::readAsString(response_->getBody());
        return json({
          {"body" , anything},
          {"headers" , response_->getHeaders()},
          {"statusCode" , response_->getStatusCode()}
        });
      }

    } catch (const Darabonba::DaraException& ex) {
      _context = Darabonba::Policy::RetryPolicyContext(json({
        {"retriesAttempted" , _retriesAttempted},
        {"lastRequest" , _lastRequest},
        {"lastResponse" , _lastResponse},
        {"exception" , ex},
      }));
      continue;
    }
  }

  throw Darabonba::UnretryableException(_context);
}

Darabonba::Json Client::execute(const Params &params, const OpenApiRequest &request, const Darabonba::RuntimeOptions &runtime) {
  Darabonba::RuntimeOptions runtime_(json({
    {"key", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getKey(), _key))},
    {"cert", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getCert(), _cert))},
    {"ca", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getCa(), _ca))},
    {"readTimeout", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getReadTimeout(), _readTimeout))},
    {"connectTimeout", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getConnectTimeout(), _connectTimeout))},
    {"httpProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getHttpProxy(), _httpProxy))},
    {"httpsProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getHttpsProxy(), _httpsProxy))},
    {"noProxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getNoProxy(), _noProxy))},
    {"socks5Proxy", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getSocks5Proxy(), _socks5Proxy))},
    {"socks5NetWork", Darabonba::Convert::stringVal(Darabonba::defaultVal(runtime.getSocks5NetWork(), _socks5NetWork))},
    {"maxIdleConns", Darabonba::Convert::int64Val(Darabonba::defaultVal(runtime.getMaxIdleConns(), _maxIdleConns))},
    {"retryOptions", _retryOptions},
    {"ignoreSSL", runtime.getIgnoreSSL()},
    {"tlsMinVersion", _tlsMinVersion},
    {"disableHttp2", Darabonba::Convert::boolVal(Darabonba::defaultVal(_disableHttp2, false))}
    }));

  shared_ptr<Darabonba::Http::Request> _lastRequest = nullptr;
  shared_ptr<Darabonba::Http::MCurlResponse> _lastResponse = nullptr;
  int _retriesAttempted = 0;
  Darabonba::Policy::RetryPolicyContext _context = json({
    {"retriesAttempted" , _retriesAttempted}
  });
  while (Darabonba::allowRetry(runtime_.getRetryOptions(), _context)) {
    if (_retriesAttempted > 0) {
      int _backoffTime = Darabonba::getBackoffTime(runtime_.getRetryOptions(), _context);
      if (_backoffTime > 0) {
        Darabonba::sleep(_backoffTime);
      }
    }
    _retriesAttempted++;
    try {
      Darabonba::Http::Request request_ = Darabonba::Http::Request();
      // spi = new Gateway();//Gateway implements SPI，这一步在产品 SDK 中实例化
      map<string, string> headers = getRpcHeaders();
      map<string, string> globalQueries = {};
      map<string, string> globalHeaders = {};
      if (!Darabonba::isNull(_globalParameters)) {
        GlobalParameters globalParams = _globalParameters;
        if (!!globalParams.hasQueries()) {
          globalQueries = globalParams.getQueries();
        }

        if (!!globalParams.hasHeaders()) {
          globalHeaders = globalParams.getHeaders();
        }

      }

      map<string, string> extendsHeaders = {};
      map<string, string> extendsQueries = {};
      if (!!runtime.hasExtendsParameters()) {
        Darabonba::ExtendsParameters extendsParameters = runtime.getExtendsParameters();
        if (!!extendsParameters.hasHeaders()) {
          extendsHeaders = extendsParameters.getHeaders();
        }

        if (!!extendsParameters.hasQueries()) {
          extendsQueries = extendsParameters.getQueries();
        }

      }

      InterceptorContextRequest requestContext = InterceptorContextRequest(json({
        {"headers" , Darabonba::Core::merge(globalHeaders,
          extendsHeaders,
          request.getHeaders(),
          headers
        ).get<map<string, string>>()},
        {"query" , Darabonba::Core::merge(globalQueries,
          extendsQueries,
          request.getQuery()
        ).get<map<string, string>>()},
        {"body" , request.getBody()},
        {"stream" , request.getStream()},
        {"hostMap" , request.getHostMap()},
        {"pathname" , params.getPathname()},
        {"productId" , _productId},
        {"action" , params.getAction()},
        {"version" , params.getVersion()},
        {"protocol" , Darabonba::Convert::stringVal(Darabonba::defaultVal(_protocol, params.getProtocol()))},
        {"method" , Darabonba::Convert::stringVal(Darabonba::defaultVal(_method, params.getMethod()))},
        {"authType" , params.getAuthType()},
        {"bodyType" , params.getBodyType()},
        {"reqBodyType" , params.getReqBodyType()},
        {"style" , params.getStyle()},
        {"credential" , _credential},
        {"signatureVersion" , _signatureVersion},
        {"signatureAlgorithm" , _signatureAlgorithm},
        {"userAgent" , Utils::Utils::getUserAgent(_userAgent)}
      }));
      InterceptorContextConfiguration configurationContext = InterceptorContextConfiguration(json({
        {"regionId" , _regionId},
        {"endpoint" , Darabonba::Convert::stringVal(Darabonba::defaultVal(request.getEndpointOverride(), _endpoint))},
        {"endpointRule" , _endpointRule},
        {"endpointMap" , _endpointMap},
        {"endpointType" , _endpointType},
        {"network" , _network},
        {"suffix" , _suffix}
      }));
      InterceptorContext interceptorContext = InterceptorContext(json({
        {"request" , requestContext},
        {"configuration" , configurationContext}
      }));
      AttributeMap attributeMap = AttributeMap();
      if (!Darabonba::isNull(_attributeMap)) {
        attributeMap = _attributeMap;
      }

      // 1. spi.modifyConfiguration(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
      _spi->modifyConfiguration(interceptorContext, attributeMap);
      // 2. spi.modifyRequest(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
      _spi->modifyRequest(interceptorContext, attributeMap);
      request_.setProtocol(interceptorContext.getRequest().getProtocol());
      request_.setMethod(interceptorContext.getRequest().getMethod());
      request_.setPathname(interceptorContext.getRequest().getPathname());
      request_.setQuery(interceptorContext.getRequest().getQuery());
      request_.setBody(interceptorContext.getRequest().getStream());
      request_.setHeaders(interceptorContext.getRequest().getHeaders());
      _lastRequest = make_shared<Darabonba::Http::Request>(request_);
      auto futureResp_ = Darabonba::Core::doAction(request_, runtime_);
      shared_ptr<Darabonba::Http::MCurlResponse> response_ = futureResp_.get();
      _lastResponse  = response_;

      InterceptorContextResponse responseContext = InterceptorContextResponse(json({
        {"statusCode" , response_->getStatusCode()},
        {"headers" , response_->getHeaders()},
        {"body" , response_->getBody()}
      }));
      interceptorContext.setResponse(responseContext);
      // 3. spi.modifyResponse(context: SPI.InterceptorContext, attributeMap: SPI.AttributeMap);
      _spi->modifyResponse(interceptorContext, attributeMap);
      return json({
        {"headers" , interceptorContext.getResponse().getHeaders()},
        {"statusCode" , interceptorContext.getResponse().getStatusCode()},
        {"body" , interceptorContext.getResponse().getDeserializedBody()}
      });
    } catch (const Darabonba::DaraException& ex) {
      _context = Darabonba::Policy::RetryPolicyContext(json({
        {"retriesAttempted" , _retriesAttempted},
        {"lastRequest" , _lastRequest},
        {"lastResponse" , _lastResponse},
        {"exception" , ex},
      }));
      continue;
    }
  }

  throw Darabonba::UnretryableException(_context);
}


Darabonba::Json Client::callApi(const Params &params, const OpenApiRequest &request, const Darabonba::RuntimeOptions &runtime) {
  if (params.empty()) {
    throw ClientException(json({
      {"code" , "ParameterMissing"},
      {"message" , "'params' can not be unset"}
    }).get<map<string, string>>());
  }

  if (Darabonba::isNull(_signatureVersion) || _signatureVersion != "v4") {
    if (Darabonba::isNull(_signatureAlgorithm) || _signatureAlgorithm != "v2") {
      return doRequest(params, request, runtime);
    } else if ((params.getStyle() == "ROA") && (params.getReqBodyType() == "json")) {
      return doROARequest(params.getAction(), params.getVersion(), params.getProtocol(), params.getMethod(), params.getAuthType(), params.getPathname(), params.getBodyType(), request, runtime);
    } else if (params.getStyle() == "ROA") {
      return doROARequestWithForm(params.getAction(), params.getVersion(), params.getProtocol(), params.getMethod(), params.getAuthType(), params.getPathname(), params.getBodyType(), request, runtime);
    } else {
      return doRPCRequest(params.getAction(), params.getVersion(), params.getProtocol(), params.getMethod(), params.getAuthType(), params.getBodyType(), request, runtime);
    }

  } else {
    return execute(params, request, runtime);
  }

}

/**
 * Get accesskey id by using credential
 * @return accesskey id
 */
string Client::getAccessKeyId() {
  if (Darabonba::isNull(_credential)) {
    return "";
  }

  string accessKeyId = _credential->getAccessKeyId();
  return accessKeyId;
}

/**
 * Get accesskey secret by using credential
 * @return accesskey secret
 */
string Client::getAccessKeySecret() {
  if (Darabonba::isNull(_credential)) {
    return "";
  }

  string secret = _credential->getAccessKeySecret();
  return secret;
}

/**
 * Get security token by using credential
 * @return security token
 */
string Client::getSecurityToken() {
  if (Darabonba::isNull(_credential)) {
    return "";
  }

  string token = _credential->getSecurityToken();
  return token;
}

/**
 * Get bearer token by credential
 * @return bearer token
 */
string Client::getBearerToken() {
  if (Darabonba::isNull(_credential)) {
    return "";
  }

  string token = _credential->getBearerToken();
  return token;
}

/**
 * Get credential type by credential
 * @return credential type e.g. access_key
 */
string Client::getType() {
  if (Darabonba::isNull(_credential)) {
    return "";
  }

  string authType = _credential->getType();
  return authType;
}

/**
 * If the endpointRule and config.endpoint are empty, throw error
 * @param config config contains the necessary information to create a client
 */
void Client::checkConfig(const AlibabaCloud::OpenApi::Utils::Models::Config &config) {
  if (Darabonba::isNull(_endpointRule) && !config.hasEndpoint()) {
    throw ClientException(json({
      {"code" , "ParameterMissing"},
      {"message" , "'config.endpoint' can not be empty"}
    }).get<map<string, string>>());
  }

}

/**
 * set gateway client
 * @param spi.
 */
void Client::setGatewayClient(const shared_ptr<SPI> &spi) {
  this->_spi = spi;
}

/**
 * set RPC header for debug
 * @param headers headers for debug, this header can be used only once.
 */
void Client::setRpcHeaders(const map<string, string> &headers) {
  this->_headers = headers;
}

/**
 * get RPC header for debug
 */
map<string, string> Client::getRpcHeaders() {
  map<string, string> headers = _headers;
  this->_headers = map<string, string>();
  return headers;
}

json Client::getAccessDeniedDetail(const json &err) {
  json accessDeniedDetail = nullptr;
  if (!!err.contains("AccessDeniedDetail")) {
    json detail1 = json(err["AccessDeniedDetail"]);
    accessDeniedDetail = detail1;
  } else if (!!err.contains("accessDeniedDetail")) {
    json detail2 = json(err["accessDeniedDetail"]);
    accessDeniedDetail = detail2;
  }

  return accessDeniedDetail;
}
} // namespace AlibabaCloud
} // namespace OpenApi