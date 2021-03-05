// This file is auto-generated, don't edit it. Thanks.

#include <alibabacloud/credential.hpp>
#include <alibabacloud/open_api.hpp>
#include <alibabacloud/open_api_util.hpp>
#include <boost/any.hpp>
#include <boost/lexical_cast.hpp>
#include <boost/throw_exception.hpp>
#include <darabonba/core.hpp>
#include <darabonba/util.hpp>
#include <iostream>
#include <map>
#include <vector>

using namespace std;

using namespace Alibabacloud_OpenApi;

Alibabacloud_OpenApi::Client::Client(const shared_ptr<Config> &config) {
  if (Darabonba_Util::Client::isUnset<Config>(config)) {
    BOOST_THROW_EXCEPTION(Darabonba::Error(
        map<string, string>({{"code", "ParameterMissing"},
                             {"message", "'config' can not be unset"}})));
  }
  if (!Darabonba_Util::Client::empty(config->accessKeyId) &&
      !Darabonba_Util::Client::empty(config->accessKeySecret)) {
    if (!Darabonba_Util::Client::empty(config->securityToken)) {
      config->type = make_shared<string>("sts");
    } else {
      config->type = make_shared<string>("access_key");
    }
    shared_ptr<Alibabacloud_Credential::Config> credentialConfig =
        make_shared<Alibabacloud_Credential::Config>(map<string, boost::any>(
            {{"accessKeyId", !config->accessKeyId
                                 ? boost::any()
                                 : boost::any(*config->accessKeyId)},
             {"type", !config->type ? boost::any() : boost::any(*config->type)},
             {"accessKeySecret", !config->accessKeySecret
                                     ? boost::any()
                                     : boost::any(*config->accessKeySecret)},
             {"securityToken", !config->securityToken
                                   ? boost::any()
                                   : boost::any(*config->securityToken)}}));
    _credential =
        make_shared<Alibabacloud_Credential::Client>(credentialConfig);
  } else if (!Darabonba_Util::Client::isUnset<Alibabacloud_Credential::Client>(
                 config->credential)) {
    _credential = config->credential;
  }
  _endpoint = config->endpoint;
  _endpointType = config->endpointType;
  _protocol = config->protocol;
  _regionId = config->regionId;
  _userAgent = config->userAgent;
  _readTimeout = config->readTimeout;
  _connectTimeout = config->connectTimeout;
  _httpProxy = config->httpProxy;
  _httpsProxy = config->httpsProxy;
  _noProxy = config->noProxy;
  _socks5Proxy = config->socks5Proxy;
  _socks5NetWork = config->socks5NetWork;
  _maxIdleConns = config->maxIdleConns;
  _signatureAlgorithm = config->signatureAlgorithm;
};

map<string, boost::any> Alibabacloud_OpenApi::Client::doRPCRequest(
    shared_ptr<string> action, shared_ptr<string> version,
    shared_ptr<string> protocol, shared_ptr<string> method,
    shared_ptr<string> authType, shared_ptr<string> bodyType,
    shared_ptr<OpenApiRequest> request,
    shared_ptr<Darabonba_Util::RuntimeOptions> runtime) {
  request->validate();
  runtime->validate();
  shared_ptr<map<string, boost::any>> runtime_ = make_shared<
      map<string, boost::any>>(map<string, boost::any>(
      {{"timeouted", boost::any(string("retry"))},
       {"readTimeout", boost::any(Darabonba_Util::Client::defaultNumber(
                           runtime->readTimeout, _readTimeout))},
       {"connectTimeout", boost::any(Darabonba_Util::Client::defaultNumber(
                              runtime->connectTimeout, _connectTimeout))},
       {"httpProxy", boost::any(string(Darabonba_Util::Client::defaultString(
                         runtime->httpProxy, _httpProxy)))},
       {"httpsProxy", boost::any(string(Darabonba_Util::Client::defaultString(
                          runtime->httpsProxy, _httpsProxy)))},
       {"noProxy", boost::any(string(Darabonba_Util::Client::defaultString(
                       runtime->noProxy, _noProxy)))},
       {"maxIdleConns", boost::any(Darabonba_Util::Client::defaultNumber(
                            runtime->maxIdleConns, _maxIdleConns))},
       {"retry", boost::any(map<string, boost::any>(
                     {{"retryable", !runtime->autoretry
                                        ? boost::any()
                                        : boost::any(*runtime->autoretry)},
                      {"maxAttempts",
                       boost::any(Darabonba_Util::Client::defaultNumber(
                           runtime->maxAttempts, make_shared<int>(3)))}}))},
       {"backoff",
        boost::any(map<string, boost::any>(
            {{"policy",
              boost::any(string(Darabonba_Util::Client::defaultString(
                  runtime->backoffPolicy, make_shared<string>("no"))))},
             {"period", boost::any(Darabonba_Util::Client::defaultNumber(
                            runtime->backoffPeriod, make_shared<int>(1)))}}))},
       {"ignoreSSL", !runtime->ignoreSSL ? boost::any()
                                         : boost::any(*runtime->ignoreSSL)}}));
  shared_ptr<Darabonba::Request> _lastRequest;
  shared_ptr<std::exception> _lastException;
  shared_ptr<int> _now = make_shared<int>(0);
  shared_ptr<int> _retryTimes = make_shared<int>(0);
  while (Darabonba::Core::allowRetry(
      make_shared<boost::any>((*runtime_)["retry"]), _retryTimes, _now)) {
    if (*_retryTimes > 0) {
      shared_ptr<int> _backoffTime =
          make_shared<int>(Darabonba::Core::getBackoffTime(
              make_shared<boost::any>((*runtime_)["backoff"]), _retryTimes));
      if (*_backoffTime > 0) {
        Darabonba::Core::sleep(_backoffTime);
      }
    }
    _retryTimes = make_shared<int>(*_retryTimes + 1);
    try {
      shared_ptr<Darabonba::Request> request_ =
          make_shared<Darabonba::Request>();
      request_->protocol =
          Darabonba_Util::Client::defaultString(_protocol, protocol);
      request_->method = *method;
      request_->pathname = "/";
      request_->query = Darabonba::Converter::merge(
          map<string, string>(
              {{"Action", !action ? string() : *action},
               {"Format", "json"},
               {"Version", !version ? string() : *version},
               {"Timestamp", Alibabacloud_OpenApiUtil::Client::getTimestamp()},
               {"SignatureNonce", Darabonba_Util::Client::getNonce()}}),
          !request->query ? map<string, string>() : *request->query);
      shared_ptr<map<string, string>> headers =
          make_shared<map<string, string>>(getRpcHeaders());
      if (Darabonba_Util::Client::isUnset<map<string, string>>(headers)) {
        // endpoint is setted in product client
        request_->headers = {{"host", !_endpoint ? string() : *_endpoint},
                             {"x-acs-version", !version ? string() : *version},
                             {"x-acs-action", !action ? string() : *action},
                             {"user-agent", getUserAgent()}};
      } else {
        request_->headers = Darabonba::Converter::merge(
            map<string, string>(
                {{"host", !_endpoint ? string() : *_endpoint},
                 {"x-acs-version", !version ? string() : *version},
                 {"x-acs-action", !action ? string() : *action},
                 {"user-agent", getUserAgent()}}),
            !headers ? map<string, string>() : *headers);
      }
      if (!Darabonba_Util::Client::isUnset<boost::any>(request->body)) {
        shared_ptr<map<string, boost::any>> m =
            make_shared<map<string, boost::any>>(
                Darabonba_Util::Client::assertAsMap(request->body));
        shared_ptr<map<string, boost::any>> tmp =
            make_shared<map<string, boost::any>>(
                Darabonba_Util::Client::anyifyMapValue(
                    make_shared<map<string, string>>(
                        Alibabacloud_OpenApiUtil::Client::query(m))));
        request_->body = Darabonba::Converter::toStream(
            Darabonba_Util::Client::toFormString(tmp));
        request_->headers.insert(pair<string, string>(
            "content-type", "application/x-www-form-urlencoded"));
      }
      if (!Darabonba_Util::Client::equalString(
              authType, make_shared<string>("Anonymous"))) {
        shared_ptr<string> accessKeyId = make_shared<string>(getAccessKeyId());
        shared_ptr<string> accessKeySecret =
            make_shared<string>(getAccessKeySecret());
        shared_ptr<string> securityToken =
            make_shared<string>(getSecurityToken());
        if (!Darabonba_Util::Client::empty(securityToken)) {
          request_->query.insert(
              pair<string, string>("SecurityToken", *securityToken));
        }
        request_->query.insert(
            pair<string, string>("SignatureMethod", "HMAC-SHA1"));
        request_->query.insert(pair<string, string>("SignatureVersion", "1.0"));
        request_->query.insert(
            pair<string, string>("AccessKeyId", *accessKeyId));
        shared_ptr<map<string, boost::any>> t;
        if (!Darabonba_Util::Client::isUnset<boost::any>(request->body)) {
          t = make_shared<map<string, boost::any>>(
              Darabonba_Util::Client::assertAsMap(request->body));
        }
        shared_ptr<map<string, string>> signedParam =
            make_shared<map<string, string>>(Darabonba::Converter::merge(
                map<string, string>(), request_->query,
                Alibabacloud_OpenApiUtil::Client::query(t)));
        request_->query.insert(pair<string, string>(
            "Signature", Alibabacloud_OpenApiUtil::Client::getRPCSignature(
                             signedParam, make_shared<string>(request_->method),
                             accessKeySecret)));
      }
      _lastRequest = request_;
      shared_ptr<Darabonba::Response> response_ =
          make_shared<Darabonba::Response>(
              Darabonba::Core::doAction(request_, runtime_));
      if (Darabonba_Util::Client::is4xx(
              make_shared<int>(response_->statusCode)) ||
          Darabonba_Util::Client::is5xx(
              make_shared<int>(response_->statusCode))) {
        shared_ptr<boost::any> _res = make_shared<boost::any>(
            Darabonba_Util::Client::readAsJSON(response_->body));
        shared_ptr<map<string, boost::any>> err =
            make_shared<map<string, boost::any>>(
                Darabonba_Util::Client::assertAsMap(_res));
        shared_ptr<boost::any> requestId = make_shared<boost::any>(
            Client::defaultAny(make_shared<boost::any>((*err)["RequestId"]),
                               make_shared<boost::any>((*err)["requestId"])));
        BOOST_THROW_EXCEPTION(Darabonba::Error(map<string, boost::any>(
            {{"code", boost::any(string(
                          Darabonba::Converter::toString(Client::defaultAny(
                              make_shared<boost::any>((*err)["Code"]),
                              make_shared<boost::any>((*err)["code"])))))},
             {"message",
              boost::any(
                  string("code: ") +
                  string(boost::lexical_cast<string>(response_->statusCode)) +
                  string(", ") +
                  string(Darabonba::Converter::toString(Client::defaultAny(
                      make_shared<boost::any>((*err)["Message"]),
                      make_shared<boost::any>((*err)["message"])))) +
                  string(" request id: ") +
                  string(*Darabonba::Converter::toString(requestId)))},
             {"data", !err ? boost::any() : boost::any(*err)}})));
      }
      if (Darabonba_Util::Client::equalString(bodyType,
                                              make_shared<string>("binary"))) {
        shared_ptr<map<string, boost::any>> resp =
            make_shared<map<string, boost::any>>(map<string, boost::any>(
                {{"body", !response_->body ? boost::any()
                                           : boost::any(*response_->body)},
                 {"headers", boost::any(response_->headers)}}));
        return *resp;
      } else if (Darabonba_Util::Client::equalString(
                     bodyType, make_shared<string>("byte"))) {
        shared_ptr<vector<uint8_t>> byt = make_shared<vector<uint8_t>>(
            Darabonba_Util::Client::readAsBytes(response_->body));
        return {{"body", !byt ? boost::any() : boost::any(*byt)},
                {"headers", boost::any(response_->headers)}};
      } else if (Darabonba_Util::Client::equalString(
                     bodyType, make_shared<string>("string"))) {
        shared_ptr<string> str = make_shared<string>(
            Darabonba_Util::Client::readAsString(response_->body));
        return {{"body", !str ? boost::any() : boost::any(*str)},
                {"headers", boost::any(response_->headers)}};
      } else if (Darabonba_Util::Client::equalString(
                     bodyType, make_shared<string>("json"))) {
        shared_ptr<boost::any> obj = make_shared<boost::any>(
            Darabonba_Util::Client::readAsJSON(response_->body));
        shared_ptr<map<string, boost::any>> res =
            make_shared<map<string, boost::any>>(
                Darabonba_Util::Client::assertAsMap(obj));
        return {{"body", !res ? boost::any() : boost::any(*res)},
                {"headers", boost::any(response_->headers)}};
      } else if (Darabonba_Util::Client::equalString(
                     bodyType, make_shared<string>("array"))) {
        shared_ptr<boost::any> arr = make_shared<boost::any>(
            Darabonba_Util::Client::readAsJSON(response_->body));
        return {{"body", !arr ? boost::any() : boost::any(*arr)},
                {"headers", boost::any(response_->headers)}};
      } else {
        return {{"headers", response_->headers}};
      }
    } catch (std::exception &e) {
      if (Darabonba::Core::isRetryable(e)) {
        _lastException = make_shared<std::exception>(e);
        continue;
      }
      BOOST_THROW_EXCEPTION(e);
    }
  }
  BOOST_THROW_EXCEPTION(
      Darabonba::UnretryableError(_lastRequest, _lastException));
}

map<string, boost::any> Alibabacloud_OpenApi::Client::doROARequest(
    shared_ptr<string> action, shared_ptr<string> version,
    shared_ptr<string> protocol, shared_ptr<string> method,
    shared_ptr<string> authType, shared_ptr<string> pathname,
    shared_ptr<string> bodyType, shared_ptr<OpenApiRequest> request,
    shared_ptr<Darabonba_Util::RuntimeOptions> runtime) {
  request->validate();
  runtime->validate();
  shared_ptr<map<string, boost::any>> runtime_ = make_shared<
      map<string, boost::any>>(map<string, boost::any>(
      {{"timeouted", boost::any(string("retry"))},
       {"readTimeout", boost::any(Darabonba_Util::Client::defaultNumber(
                           runtime->readTimeout, _readTimeout))},
       {"connectTimeout", boost::any(Darabonba_Util::Client::defaultNumber(
                              runtime->connectTimeout, _connectTimeout))},
       {"httpProxy", boost::any(string(Darabonba_Util::Client::defaultString(
                         runtime->httpProxy, _httpProxy)))},
       {"httpsProxy", boost::any(string(Darabonba_Util::Client::defaultString(
                          runtime->httpsProxy, _httpsProxy)))},
       {"noProxy", boost::any(string(Darabonba_Util::Client::defaultString(
                       runtime->noProxy, _noProxy)))},
       {"maxIdleConns", boost::any(Darabonba_Util::Client::defaultNumber(
                            runtime->maxIdleConns, _maxIdleConns))},
       {"retry", boost::any(map<string, boost::any>(
                     {{"retryable", !runtime->autoretry
                                        ? boost::any()
                                        : boost::any(*runtime->autoretry)},
                      {"maxAttempts",
                       boost::any(Darabonba_Util::Client::defaultNumber(
                           runtime->maxAttempts, make_shared<int>(3)))}}))},
       {"backoff",
        boost::any(map<string, boost::any>(
            {{"policy",
              boost::any(string(Darabonba_Util::Client::defaultString(
                  runtime->backoffPolicy, make_shared<string>("no"))))},
             {"period", boost::any(Darabonba_Util::Client::defaultNumber(
                            runtime->backoffPeriod, make_shared<int>(1)))}}))},
       {"ignoreSSL", !runtime->ignoreSSL ? boost::any()
                                         : boost::any(*runtime->ignoreSSL)}}));
  shared_ptr<Darabonba::Request> _lastRequest;
  shared_ptr<std::exception> _lastException;
  shared_ptr<int> _now = make_shared<int>(0);
  shared_ptr<int> _retryTimes = make_shared<int>(0);
  while (Darabonba::Core::allowRetry(
      make_shared<boost::any>((*runtime_)["retry"]), _retryTimes, _now)) {
    if (*_retryTimes > 0) {
      shared_ptr<int> _backoffTime =
          make_shared<int>(Darabonba::Core::getBackoffTime(
              make_shared<boost::any>((*runtime_)["backoff"]), _retryTimes));
      if (*_backoffTime > 0) {
        Darabonba::Core::sleep(_backoffTime);
      }
    }
    _retryTimes = make_shared<int>(*_retryTimes + 1);
    try {
      shared_ptr<Darabonba::Request> request_ =
          make_shared<Darabonba::Request>();
      request_->protocol =
          Darabonba_Util::Client::defaultString(_protocol, protocol);
      request_->method = *method;
      request_->pathname = *pathname;
      request_->headers = Darabonba::Converter::merge(
          map<string, string>(
              {{"date", Darabonba_Util::Client::getDateUTCString()},
               {"host", !_endpoint ? string() : *_endpoint},
               {"accept", "application/json"},
               {"x-acs-signature-nonce", Darabonba_Util::Client::getNonce()},
               {"x-acs-signature-method", "HMAC-SHA1"},
               {"x-acs-signature-version", "1.0"},
               {"x-acs-version", !version ? string() : *version},
               {"x-acs-action", !action ? string() : *action},
               {"user-agent",
                Darabonba_Util::Client::getUserAgent(_userAgent)}}),
          !request->headers ? map<string, string>() : *request->headers);
      if (!Darabonba_Util::Client::isUnset<boost::any>(request->body)) {
        request_->body = Darabonba::Converter::toStream(
            Darabonba_Util::Client::toJSONString(request->body));
        request_->headers.insert(pair<string, string>(
            "content-type", "application/json; charset=utf-8"));
      }
      if (!Darabonba_Util::Client::isUnset<map<string, string>>(
              request->query)) {
        request_->query = *request->query;
      }
      if (!Darabonba_Util::Client::equalString(
              authType, make_shared<string>("Anonymous"))) {
        shared_ptr<string> accessKeyId = make_shared<string>(getAccessKeyId());
        shared_ptr<string> accessKeySecret =
            make_shared<string>(getAccessKeySecret());
        shared_ptr<string> securityToken =
            make_shared<string>(getSecurityToken());
        if (!Darabonba_Util::Client::empty(securityToken)) {
          request_->headers.insert(
              pair<string, string>("x-acs-accesskey-id", *accessKeyId));
          request_->headers.insert(
              pair<string, string>("x-acs-security-token", *securityToken));
        }
        shared_ptr<string> stringToSign = make_shared<string>(
            Alibabacloud_OpenApiUtil::Client::getStringToSign(request_));
        request_->headers.insert(pair<string, string>(
            "authorization",
            string("acs ") + string(*accessKeyId) + string(":") +
                string(Alibabacloud_OpenApiUtil::Client::getROASignature(
                    stringToSign, accessKeySecret))));
      }
      _lastRequest = request_;
      shared_ptr<Darabonba::Response> response_ =
          make_shared<Darabonba::Response>(
              Darabonba::Core::doAction(request_, runtime_));
      if (Darabonba_Util::Client::equalNumber(
              make_shared<int>(response_->statusCode), make_shared<int>(204))) {
        return {{"headers", response_->headers}};
      }
      if (Darabonba_Util::Client::is4xx(
              make_shared<int>(response_->statusCode)) ||
          Darabonba_Util::Client::is5xx(
              make_shared<int>(response_->statusCode))) {
        shared_ptr<boost::any> _res = make_shared<boost::any>(
            Darabonba_Util::Client::readAsJSON(response_->body));
        shared_ptr<map<string, boost::any>> err =
            make_shared<map<string, boost::any>>(
                Darabonba_Util::Client::assertAsMap(_res));
        shared_ptr<boost::any> requestId = make_shared<boost::any>(
            Client::defaultAny(make_shared<boost::any>((*err)["RequestId"]),
                               make_shared<boost::any>((*err)["requestId"])));
        requestId = make_shared<boost::any>(Client::defaultAny(
            requestId, make_shared<boost::any>((*err)["requestid"])));
        BOOST_THROW_EXCEPTION(Darabonba::Error(map<string, boost::any>(
            {{"code", boost::any(string(
                          Darabonba::Converter::toString(Client::defaultAny(
                              make_shared<boost::any>((*err)["Code"]),
                              make_shared<boost::any>((*err)["code"])))))},
             {"message",
              boost::any(
                  string("code: ") +
                  string(boost::lexical_cast<string>(response_->statusCode)) +
                  string(", ") +
                  string(Darabonba::Converter::toString(Client::defaultAny(
                      make_shared<boost::any>((*err)["Message"]),
                      make_shared<boost::any>((*err)["message"])))) +
                  string(" request id: ") +
                  string(*Darabonba::Converter::toString(requestId)))},
             {"data", !err ? boost::any() : boost::any(*err)}})));
      }
      if (Darabonba_Util::Client::equalString(bodyType,
                                              make_shared<string>("binary"))) {
        shared_ptr<map<string, boost::any>> resp =
            make_shared<map<string, boost::any>>(map<string, boost::any>(
                {{"body", !response_->body ? boost::any()
                                           : boost::any(*response_->body)},
                 {"headers", boost::any(response_->headers)}}));
        return *resp;
      } else if (Darabonba_Util::Client::equalString(
                     bodyType, make_shared<string>("byte"))) {
        shared_ptr<vector<uint8_t>> byt = make_shared<vector<uint8_t>>(
            Darabonba_Util::Client::readAsBytes(response_->body));
        return {{"body", !byt ? boost::any() : boost::any(*byt)},
                {"headers", boost::any(response_->headers)}};
      } else if (Darabonba_Util::Client::equalString(
                     bodyType, make_shared<string>("string"))) {
        shared_ptr<string> str = make_shared<string>(
            Darabonba_Util::Client::readAsString(response_->body));
        return {{"body", !str ? boost::any() : boost::any(*str)},
                {"headers", boost::any(response_->headers)}};
      } else if (Darabonba_Util::Client::equalString(
                     bodyType, make_shared<string>("json"))) {
        shared_ptr<boost::any> obj = make_shared<boost::any>(
            Darabonba_Util::Client::readAsJSON(response_->body));
        shared_ptr<map<string, boost::any>> res =
            make_shared<map<string, boost::any>>(
                Darabonba_Util::Client::assertAsMap(obj));
        return {{"body", !res ? boost::any() : boost::any(*res)},
                {"headers", boost::any(response_->headers)}};
      } else if (Darabonba_Util::Client::equalString(
                     bodyType, make_shared<string>("array"))) {
        shared_ptr<boost::any> arr = make_shared<boost::any>(
            Darabonba_Util::Client::readAsJSON(response_->body));
        return {{"body", !arr ? boost::any() : boost::any(*arr)},
                {"headers", boost::any(response_->headers)}};
      } else {
        return {{"headers", response_->headers}};
      }
    } catch (std::exception &e) {
      if (Darabonba::Core::isRetryable(e)) {
        _lastException = make_shared<std::exception>(e);
        continue;
      }
      BOOST_THROW_EXCEPTION(e);
    }
  }
  BOOST_THROW_EXCEPTION(
      Darabonba::UnretryableError(_lastRequest, _lastException));
}

map<string, boost::any> Alibabacloud_OpenApi::Client::doROARequestWithForm(
    shared_ptr<string> action, shared_ptr<string> version,
    shared_ptr<string> protocol, shared_ptr<string> method,
    shared_ptr<string> authType, shared_ptr<string> pathname,
    shared_ptr<string> bodyType, shared_ptr<OpenApiRequest> request,
    shared_ptr<Darabonba_Util::RuntimeOptions> runtime) {
  request->validate();
  runtime->validate();
  shared_ptr<map<string, boost::any>> runtime_ = make_shared<
      map<string, boost::any>>(map<string, boost::any>(
      {{"timeouted", boost::any(string("retry"))},
       {"readTimeout", boost::any(Darabonba_Util::Client::defaultNumber(
                           runtime->readTimeout, _readTimeout))},
       {"connectTimeout", boost::any(Darabonba_Util::Client::defaultNumber(
                              runtime->connectTimeout, _connectTimeout))},
       {"httpProxy", boost::any(string(Darabonba_Util::Client::defaultString(
                         runtime->httpProxy, _httpProxy)))},
       {"httpsProxy", boost::any(string(Darabonba_Util::Client::defaultString(
                          runtime->httpsProxy, _httpsProxy)))},
       {"noProxy", boost::any(string(Darabonba_Util::Client::defaultString(
                       runtime->noProxy, _noProxy)))},
       {"maxIdleConns", boost::any(Darabonba_Util::Client::defaultNumber(
                            runtime->maxIdleConns, _maxIdleConns))},
       {"retry", boost::any(map<string, boost::any>(
                     {{"retryable", !runtime->autoretry
                                        ? boost::any()
                                        : boost::any(*runtime->autoretry)},
                      {"maxAttempts",
                       boost::any(Darabonba_Util::Client::defaultNumber(
                           runtime->maxAttempts, make_shared<int>(3)))}}))},
       {"backoff",
        boost::any(map<string, boost::any>(
            {{"policy",
              boost::any(string(Darabonba_Util::Client::defaultString(
                  runtime->backoffPolicy, make_shared<string>("no"))))},
             {"period", boost::any(Darabonba_Util::Client::defaultNumber(
                            runtime->backoffPeriod, make_shared<int>(1)))}}))},
       {"ignoreSSL", !runtime->ignoreSSL ? boost::any()
                                         : boost::any(*runtime->ignoreSSL)}}));
  shared_ptr<Darabonba::Request> _lastRequest;
  shared_ptr<std::exception> _lastException;
  shared_ptr<int> _now = make_shared<int>(0);
  shared_ptr<int> _retryTimes = make_shared<int>(0);
  while (Darabonba::Core::allowRetry(
      make_shared<boost::any>((*runtime_)["retry"]), _retryTimes, _now)) {
    if (*_retryTimes > 0) {
      shared_ptr<int> _backoffTime =
          make_shared<int>(Darabonba::Core::getBackoffTime(
              make_shared<boost::any>((*runtime_)["backoff"]), _retryTimes));
      if (*_backoffTime > 0) {
        Darabonba::Core::sleep(_backoffTime);
      }
    }
    _retryTimes = make_shared<int>(*_retryTimes + 1);
    try {
      shared_ptr<Darabonba::Request> request_ =
          make_shared<Darabonba::Request>();
      request_->protocol =
          Darabonba_Util::Client::defaultString(_protocol, protocol);
      request_->method = *method;
      request_->pathname = *pathname;
      request_->headers = Darabonba::Converter::merge(
          map<string, string>(
              {{"date", Darabonba_Util::Client::getDateUTCString()},
               {"host", !_endpoint ? string() : *_endpoint},
               {"accept", "application/json"},
               {"x-acs-signature-nonce", Darabonba_Util::Client::getNonce()},
               {"x-acs-signature-method", "HMAC-SHA1"},
               {"x-acs-signature-version", "1.0"},
               {"x-acs-version", !version ? string() : *version},
               {"x-acs-action", !action ? string() : *action},
               {"user-agent",
                Darabonba_Util::Client::getUserAgent(_userAgent)}}),
          !request->headers ? map<string, string>() : *request->headers);
      if (!Darabonba_Util::Client::isUnset<boost::any>(request->body)) {
        shared_ptr<map<string, boost::any>> m =
            make_shared<map<string, boost::any>>(
                Darabonba_Util::Client::assertAsMap(request->body));
        request_->body = Darabonba::Converter::toStream(
            Alibabacloud_OpenApiUtil::Client::toForm(m));
        request_->headers.insert(pair<string, string>(
            "content-type", "application/x-www-form-urlencoded"));
      }
      if (!Darabonba_Util::Client::isUnset<map<string, string>>(
              request->query)) {
        request_->query = *request->query;
      }
      if (!Darabonba_Util::Client::equalString(
              authType, make_shared<string>("Anonymous"))) {
        shared_ptr<string> accessKeyId = make_shared<string>(getAccessKeyId());
        shared_ptr<string> accessKeySecret =
            make_shared<string>(getAccessKeySecret());
        shared_ptr<string> securityToken =
            make_shared<string>(getSecurityToken());
        if (!Darabonba_Util::Client::empty(securityToken)) {
          request_->headers.insert(
              pair<string, string>("x-acs-accesskey-id", *accessKeyId));
          request_->headers.insert(
              pair<string, string>("x-acs-security-token", *securityToken));
        }
        shared_ptr<string> stringToSign = make_shared<string>(
            Alibabacloud_OpenApiUtil::Client::getStringToSign(request_));
        request_->headers.insert(pair<string, string>(
            "authorization",
            string("acs ") + string(*accessKeyId) + string(":") +
                string(Alibabacloud_OpenApiUtil::Client::getROASignature(
                    stringToSign, accessKeySecret))));
      }
      _lastRequest = request_;
      shared_ptr<Darabonba::Response> response_ =
          make_shared<Darabonba::Response>(
              Darabonba::Core::doAction(request_, runtime_));
      if (Darabonba_Util::Client::equalNumber(
              make_shared<int>(response_->statusCode), make_shared<int>(204))) {
        return {{"headers", response_->headers}};
      }
      if (Darabonba_Util::Client::is4xx(
              make_shared<int>(response_->statusCode)) ||
          Darabonba_Util::Client::is5xx(
              make_shared<int>(response_->statusCode))) {
        shared_ptr<boost::any> _res = make_shared<boost::any>(
            Darabonba_Util::Client::readAsJSON(response_->body));
        shared_ptr<map<string, boost::any>> err =
            make_shared<map<string, boost::any>>(
                Darabonba_Util::Client::assertAsMap(_res));
        BOOST_THROW_EXCEPTION(Darabonba::Error(map<string, boost::any>(
            {{"code", boost::any(string(
                          Darabonba::Converter::toString(Client::defaultAny(
                              make_shared<boost::any>((*err)["Code"]),
                              make_shared<boost::any>((*err)["code"])))))},
             {"message",
              boost::any(
                  string("code: ") +
                  string(boost::lexical_cast<string>(response_->statusCode)) +
                  string(", ") +
                  string(Darabonba::Converter::toString(Client::defaultAny(
                      make_shared<boost::any>((*err)["Message"]),
                      make_shared<boost::any>((*err)["message"])))) +
                  string(" request id: ") +
                  string(Darabonba::Converter::toString(Client::defaultAny(
                      make_shared<boost::any>((*err)["RequestId"]),
                      make_shared<boost::any>((*err)["requestId"])))))},
             {"data", !err ? boost::any() : boost::any(*err)}})));
      }
      if (Darabonba_Util::Client::equalString(bodyType,
                                              make_shared<string>("binary"))) {
        shared_ptr<map<string, boost::any>> resp =
            make_shared<map<string, boost::any>>(map<string, boost::any>(
                {{"body", !response_->body ? boost::any()
                                           : boost::any(*response_->body)},
                 {"headers", boost::any(response_->headers)}}));
        return *resp;
      } else if (Darabonba_Util::Client::equalString(
                     bodyType, make_shared<string>("byte"))) {
        shared_ptr<vector<uint8_t>> byt = make_shared<vector<uint8_t>>(
            Darabonba_Util::Client::readAsBytes(response_->body));
        return {{"body", !byt ? boost::any() : boost::any(*byt)},
                {"headers", boost::any(response_->headers)}};
      } else if (Darabonba_Util::Client::equalString(
                     bodyType, make_shared<string>("string"))) {
        shared_ptr<string> str = make_shared<string>(
            Darabonba_Util::Client::readAsString(response_->body));
        return {{"body", !str ? boost::any() : boost::any(*str)},
                {"headers", boost::any(response_->headers)}};
      } else if (Darabonba_Util::Client::equalString(
                     bodyType, make_shared<string>("json"))) {
        shared_ptr<boost::any> obj = make_shared<boost::any>(
            Darabonba_Util::Client::readAsJSON(response_->body));
        shared_ptr<map<string, boost::any>> res =
            make_shared<map<string, boost::any>>(
                Darabonba_Util::Client::assertAsMap(obj));
        return {{"body", !res ? boost::any() : boost::any(*res)},
                {"headers", boost::any(response_->headers)}};
      } else if (Darabonba_Util::Client::equalString(
                     bodyType, make_shared<string>("array"))) {
        shared_ptr<boost::any> arr = make_shared<boost::any>(
            Darabonba_Util::Client::readAsJSON(response_->body));
        return {{"body", !arr ? boost::any() : boost::any(*arr)},
                {"headers", boost::any(response_->headers)}};
      } else {
        return {{"headers", response_->headers}};
      }
    } catch (std::exception &e) {
      if (Darabonba::Core::isRetryable(e)) {
        _lastException = make_shared<std::exception>(e);
        continue;
      }
      BOOST_THROW_EXCEPTION(e);
    }
  }
  BOOST_THROW_EXCEPTION(
      Darabonba::UnretryableError(_lastRequest, _lastException));
}

map<string, boost::any> Alibabacloud_OpenApi::Client::doRequest(
    shared_ptr<Params> params, shared_ptr<OpenApiRequest> request,
    shared_ptr<Darabonba_Util::RuntimeOptions> runtime) {
  params->validate();
  request->validate();
  runtime->validate();
  shared_ptr<map<string, boost::any>> runtime_ = make_shared<
      map<string, boost::any>>(map<string, boost::any>(
      {{"timeouted", boost::any(string("retry"))},
       {"readTimeout", boost::any(Darabonba_Util::Client::defaultNumber(
                           runtime->readTimeout, _readTimeout))},
       {"connectTimeout", boost::any(Darabonba_Util::Client::defaultNumber(
                              runtime->connectTimeout, _connectTimeout))},
       {"httpProxy", boost::any(string(Darabonba_Util::Client::defaultString(
                         runtime->httpProxy, _httpProxy)))},
       {"httpsProxy", boost::any(string(Darabonba_Util::Client::defaultString(
                          runtime->httpsProxy, _httpsProxy)))},
       {"noProxy", boost::any(string(Darabonba_Util::Client::defaultString(
                       runtime->noProxy, _noProxy)))},
       {"maxIdleConns", boost::any(Darabonba_Util::Client::defaultNumber(
                            runtime->maxIdleConns, _maxIdleConns))},
       {"retry", boost::any(map<string, boost::any>(
                     {{"retryable", !runtime->autoretry
                                        ? boost::any()
                                        : boost::any(*runtime->autoretry)},
                      {"maxAttempts",
                       boost::any(Darabonba_Util::Client::defaultNumber(
                           runtime->maxAttempts, make_shared<int>(3)))}}))},
       {"backoff",
        boost::any(map<string, boost::any>(
            {{"policy",
              boost::any(string(Darabonba_Util::Client::defaultString(
                  runtime->backoffPolicy, make_shared<string>("no"))))},
             {"period", boost::any(Darabonba_Util::Client::defaultNumber(
                            runtime->backoffPeriod, make_shared<int>(1)))}}))},
       {"ignoreSSL", !runtime->ignoreSSL ? boost::any()
                                         : boost::any(*runtime->ignoreSSL)}}));
  shared_ptr<Darabonba::Request> _lastRequest;
  shared_ptr<std::exception> _lastException;
  shared_ptr<int> _now = make_shared<int>(0);
  shared_ptr<int> _retryTimes = make_shared<int>(0);
  while (Darabonba::Core::allowRetry(
      make_shared<boost::any>((*runtime_)["retry"]), _retryTimes, _now)) {
    if (*_retryTimes > 0) {
      shared_ptr<int> _backoffTime =
          make_shared<int>(Darabonba::Core::getBackoffTime(
              make_shared<boost::any>((*runtime_)["backoff"]), _retryTimes));
      if (*_backoffTime > 0) {
        Darabonba::Core::sleep(_backoffTime);
      }
    }
    _retryTimes = make_shared<int>(*_retryTimes + 1);
    try {
      shared_ptr<Darabonba::Request> request_ =
          make_shared<Darabonba::Request>();
      request_->protocol =
          Darabonba_Util::Client::defaultString(_protocol, params->protocol);
      request_->method = *params->method;
      request_->pathname =
          Alibabacloud_OpenApiUtil::Client::getEncodePath(params->pathname);
      request_->query = *request->query;
      // endpoint is setted in product client
      request_->headers = Darabonba::Converter::merge(
          map<string, string>(
              {{"host", !_endpoint ? string() : *_endpoint},
               {"x-acs-version",
                !params->version ? string() : *params->version},
               {"x-acs-action", !params->action ? string() : *params->action},
               {"user-agent", getUserAgent()},
               {"x-acs-date", Alibabacloud_OpenApiUtil::Client::getTimestamp()},
               {"x-acs-signature-nonce", Darabonba_Util::Client::getNonce()},
               {"accept", "application/json"}}),
          !request->headers ? map<string, string>() : *request->headers);
      shared_ptr<string> signatureAlgorithm =
          make_shared<string>(Darabonba_Util::Client::defaultString(
              _signatureAlgorithm, make_shared<string>("ACS3-HMAC-SHA256")));
      shared_ptr<string> hashedRequestPayload =
          make_shared<string>(Alibabacloud_OpenApiUtil::Client::hexEncode(
              make_shared<vector<uint8_t>>(
                  Alibabacloud_OpenApiUtil::Client::hash(
                      make_shared<vector<uint8_t>>(
                          Darabonba_Util::Client::toBytes(
                              make_shared<string>(""))),
                      signatureAlgorithm))));
      if (!Darabonba_Util::Client::isUnset<boost::any>(request->body)) {
        if (Darabonba_Util::Client::equalString(params->reqBodyType,
                                                make_shared<string>("json"))) {
          shared_ptr<string> jsonObj = make_shared<string>(
              Darabonba_Util::Client::toJSONString(request->body));
          hashedRequestPayload =
              make_shared<string>(Alibabacloud_OpenApiUtil::Client::hexEncode(
                  make_shared<vector<uint8_t>>(
                      Alibabacloud_OpenApiUtil::Client::hash(
                          make_shared<vector<uint8_t>>(
                              Darabonba_Util::Client::toBytes(jsonObj)),
                          signatureAlgorithm))));
          request_->body = Darabonba::Converter::toStream(*jsonObj);
        } else {
          shared_ptr<map<string, boost::any>> m =
              make_shared<map<string, boost::any>>(
                  Darabonba_Util::Client::assertAsMap(request->body));
          shared_ptr<string> formObj =
              make_shared<string>(Alibabacloud_OpenApiUtil::Client::toForm(m));
          hashedRequestPayload =
              make_shared<string>(Alibabacloud_OpenApiUtil::Client::hexEncode(
                  make_shared<vector<uint8_t>>(
                      Alibabacloud_OpenApiUtil::Client::hash(
                          make_shared<vector<uint8_t>>(
                              Darabonba_Util::Client::toBytes(formObj)),
                          signatureAlgorithm))));
          request_->body = Darabonba::Converter::toStream(*formObj);
          request_->headers.insert(pair<string, string>(
              "content-type", "application/x-www-form-urlencoded"));
        }
      }
      if (!Darabonba_Util::Client::isUnset<Darabonba::Stream>(
              request->stream)) {
        shared_ptr<vector<uint8_t>> tmp = make_shared<vector<uint8_t>>(
            Darabonba_Util::Client::readAsBytes(request->stream));
        hashedRequestPayload =
            make_shared<string>(Alibabacloud_OpenApiUtil::Client::hexEncode(
                make_shared<vector<uint8_t>>(
                    Alibabacloud_OpenApiUtil::Client::hash(
                        tmp, signatureAlgorithm))));
        request_->body = Darabonba::Converter::toStream(*tmp);
      }
      request_->headers.insert(
          pair<string, string>("x-acs-content-sha256", *hashedRequestPayload));
      if (!Darabonba_Util::Client::equalString(
              params->authType, make_shared<string>("Anonymous"))) {
        shared_ptr<string> accessKeyId = make_shared<string>(getAccessKeyId());
        shared_ptr<string> accessKeySecret =
            make_shared<string>(getAccessKeySecret());
        shared_ptr<string> securityToken =
            make_shared<string>(getSecurityToken());
        if (!Darabonba_Util::Client::empty(securityToken)) {
          request_->headers.insert(
              pair<string, string>("x-acs-security-token", *securityToken));
        }
        request_->headers.insert(pair<string, string>(
            "Authorization",
            Alibabacloud_OpenApiUtil::Client::getAuthorization(
                request_, signatureAlgorithm, hashedRequestPayload, accessKeyId,
                accessKeySecret)));
      }
      _lastRequest = request_;
      shared_ptr<Darabonba::Response> response_ =
          make_shared<Darabonba::Response>(
              Darabonba::Core::doAction(request_, runtime_));
      if (Darabonba_Util::Client::is4xx(
              make_shared<int>(response_->statusCode)) ||
          Darabonba_Util::Client::is5xx(
              make_shared<int>(response_->statusCode))) {
        shared_ptr<boost::any> _res = make_shared<boost::any>(
            Darabonba_Util::Client::readAsJSON(response_->body));
        shared_ptr<map<string, boost::any>> err =
            make_shared<map<string, boost::any>>(
                Darabonba_Util::Client::assertAsMap(_res));
        BOOST_THROW_EXCEPTION(Darabonba::Error(map<string, boost::any>(
            {{"code", boost::any(string(
                          Darabonba::Converter::toString(Client::defaultAny(
                              make_shared<boost::any>((*err)["Code"]),
                              make_shared<boost::any>((*err)["code"])))))},
             {"message",
              boost::any(
                  string("code: ") +
                  string(boost::lexical_cast<string>(response_->statusCode)) +
                  string(", ") +
                  string(Darabonba::Converter::toString(Client::defaultAny(
                      make_shared<boost::any>((*err)["Message"]),
                      make_shared<boost::any>((*err)["message"])))) +
                  string(" request id: ") +
                  string(Darabonba::Converter::toString(Client::defaultAny(
                      make_shared<boost::any>((*err)["RequestId"]),
                      make_shared<boost::any>((*err)["requestId"])))))},
             {"data", !err ? boost::any() : boost::any(*err)}})));
      }
      if (Darabonba_Util::Client::equalString(params->bodyType,
                                              make_shared<string>("binary"))) {
        shared_ptr<map<string, boost::any>> resp =
            make_shared<map<string, boost::any>>(map<string, boost::any>(
                {{"body", !response_->body ? boost::any()
                                           : boost::any(*response_->body)},
                 {"headers", boost::any(response_->headers)}}));
        return *resp;
      } else if (Darabonba_Util::Client::equalString(
                     params->bodyType, make_shared<string>("byte"))) {
        shared_ptr<vector<uint8_t>> byt = make_shared<vector<uint8_t>>(
            Darabonba_Util::Client::readAsBytes(response_->body));
        return {{"body", !byt ? boost::any() : boost::any(*byt)},
                {"headers", boost::any(response_->headers)}};
      } else if (Darabonba_Util::Client::equalString(
                     params->bodyType, make_shared<string>("string"))) {
        shared_ptr<string> str = make_shared<string>(
            Darabonba_Util::Client::readAsString(response_->body));
        return {{"body", !str ? boost::any() : boost::any(*str)},
                {"headers", boost::any(response_->headers)}};
      } else if (Darabonba_Util::Client::equalString(
                     params->bodyType, make_shared<string>("json"))) {
        shared_ptr<boost::any> obj = make_shared<boost::any>(
            Darabonba_Util::Client::readAsJSON(response_->body));
        shared_ptr<map<string, boost::any>> res =
            make_shared<map<string, boost::any>>(
                Darabonba_Util::Client::assertAsMap(obj));
        return {{"body", !res ? boost::any() : boost::any(*res)},
                {"headers", boost::any(response_->headers)}};
      } else if (Darabonba_Util::Client::equalString(
                     params->bodyType, make_shared<string>("array"))) {
        shared_ptr<boost::any> arr = make_shared<boost::any>(
            Darabonba_Util::Client::readAsJSON(response_->body));
        return {{"body", !arr ? boost::any() : boost::any(*arr)},
                {"headers", boost::any(response_->headers)}};
      } else {
        return {{"headers", response_->headers}};
      }
    } catch (std::exception &e) {
      if (Darabonba::Core::isRetryable(e)) {
        _lastException = make_shared<std::exception>(e);
        continue;
      }
      BOOST_THROW_EXCEPTION(e);
    }
  }
  BOOST_THROW_EXCEPTION(
      Darabonba::UnretryableError(_lastRequest, _lastException));
}

map<string, boost::any> Alibabacloud_OpenApi::Client::callApi(
    shared_ptr<Params> params, shared_ptr<OpenApiRequest> request,
    shared_ptr<Darabonba_Util::RuntimeOptions> runtime) {
  if (Darabonba_Util::Client::isUnset<Params>(params)) {
    BOOST_THROW_EXCEPTION(Darabonba::Error(
        map<string, string>({{"code", "ParameterMissing"},
                             {"message", "'params' can not be unset"}})));
  }
  if (Darabonba_Util::Client::isUnset<string>(_signatureAlgorithm) ||
      !Darabonba_Util::Client::equalString(_signatureAlgorithm,
                                           make_shared<string>("v2"))) {
    return doRequest(params, request, runtime);
  } else if (Darabonba_Util::Client::equalString(params->style,
                                                 make_shared<string>("ROA")) &&
             Darabonba_Util::Client::equalString(params->reqBodyType,
                                                 make_shared<string>("json"))) {
    return doROARequest(params->action, params->version, params->protocol,
                        params->method, params->authType, params->pathname,
                        params->bodyType, request, runtime);
  } else if (Darabonba_Util::Client::equalString(params->style,
                                                 make_shared<string>("ROA"))) {
    return doROARequestWithForm(
        params->action, params->version, params->protocol, params->method,
        params->authType, params->pathname, params->bodyType, request, runtime);
  } else {
    return doRPCRequest(params->action, params->version, params->protocol,
                        params->method, params->authType, params->bodyType,
                        request, runtime);
  }
}

string Alibabacloud_OpenApi::Client::getUserAgent() {
  shared_ptr<string> userAgent =
      make_shared<string>(Darabonba_Util::Client::getUserAgent(_userAgent));
  return *userAgent;
}

string Alibabacloud_OpenApi::Client::getAccessKeyId() {
  if (Darabonba_Util::Client::isUnset<Alibabacloud_Credential::Client>(
          _credential)) {
    return string("");
  }
  shared_ptr<string> accessKeyId =
      make_shared<string>(_credential->getAccessKeyId());
  return *accessKeyId;
}

string Alibabacloud_OpenApi::Client::getAccessKeySecret() {
  if (Darabonba_Util::Client::isUnset<Alibabacloud_Credential::Client>(
          _credential)) {
    return string("");
  }
  shared_ptr<string> secret =
      make_shared<string>(_credential->getAccessKeySecret());
  return *secret;
}

string Alibabacloud_OpenApi::Client::getSecurityToken() {
  if (Darabonba_Util::Client::isUnset<Alibabacloud_Credential::Client>(
          _credential)) {
    return string("");
  }
  shared_ptr<string> token =
      make_shared<string>(_credential->getSecurityToken());
  return *token;
}

boost::any
Alibabacloud_OpenApi::Client::defaultAny(const boost::any &inputValue,
                                         const boost::any &defaultValue) {
  if (Darabonba_Util::Client::isUnset<boost::any>(inputValue)) {
    return defaultValue;
  }
  return inputValue;
}

void Alibabacloud_OpenApi::Client::checkConfig(shared_ptr<Config> config) {
  if (Darabonba_Util::Client::empty(_endpointRule) &&
      Darabonba_Util::Client::empty(config->endpoint)) {
    BOOST_THROW_EXCEPTION(Darabonba::Error(map<string, string>(
        {{"code", "ParameterMissing"},
         {"message", "'config.endpoint' can not be empty"}})));
  }
}

void Alibabacloud_OpenApi::Client::setRpcHeaders(
    shared_ptr<map<string, string>> headers) {
  _headers = headers;
}

map<string, string> Alibabacloud_OpenApi::Client::getRpcHeaders() {
  shared_ptr<map<string, string>> headers = _headers;
  _headers = nullptr;
  return *headers;
}
