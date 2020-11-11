// This file is auto-generated, don't edit it. Thanks.

#ifndef ALIBABACLOUD_OPENAPI_H_
#define ALIBABACLOUD_OPENAPI_H_

#include <alibabacloud/credential.hpp>
#include <alibabacloud/open_api_util.hpp>
#include <boost/any.hpp>
#include <darabonba/core.hpp>
#include <darabonba/util.hpp>
#include <iostream>
#include <map>

using namespace std;

namespace Alibabacloud_OpenApi {
class Config : public Darabonba::Model {
public:
  Config() {}
  explicit Config(const std::map<string, boost::any> &config) : Darabonba::Model(config) {
    fromMap(config);
  };

  void validate() override {}

  map<string, boost::any> toMap() override {
    map<string, boost::any> res;
    if (accessKeyId) {
      res["accessKeyId"] = boost::any(*accessKeyId);
    }
    if (accessKeySecret) {
      res["accessKeySecret"] = boost::any(*accessKeySecret);
    }
    if (securityToken) {
      res["securityToken"] = boost::any(*securityToken);
    }
    if (protocol) {
      res["protocol"] = boost::any(*protocol);
    }
    if (regionId) {
      res["regionId"] = boost::any(*regionId);
    }
    if (readTimeout) {
      res["readTimeout"] = boost::any(*readTimeout);
    }
    if (connectTimeout) {
      res["connectTimeout"] = boost::any(*connectTimeout);
    }
    if (httpProxy) {
      res["httpProxy"] = boost::any(*httpProxy);
    }
    if (httpsProxy) {
      res["httpsProxy"] = boost::any(*httpsProxy);
    }
    if (endpoint) {
      res["endpoint"] = boost::any(*endpoint);
    }
    if (noProxy) {
      res["noProxy"] = boost::any(*noProxy);
    }
    if (maxIdleConns) {
      res["maxIdleConns"] = boost::any(*maxIdleConns);
    }
    if (network) {
      res["network"] = boost::any(*network);
    }
    if (userAgent) {
      res["userAgent"] = boost::any(*userAgent);
    }
    if (suffix) {
      res["suffix"] = boost::any(*suffix);
    }
    if (socks5Proxy) {
      res["socks5Proxy"] = boost::any(*socks5Proxy);
    }
    if (socks5NetWork) {
      res["socks5NetWork"] = boost::any(*socks5NetWork);
    }
    if (endpointType) {
      res["endpointType"] = boost::any(*endpointType);
    }
    if (openPlatformEndpoint) {
      res["openPlatformEndpoint"] = boost::any(*openPlatformEndpoint);
    }
    if (type) {
      res["type"] = boost::any(*type);
    }
    return res;
  }

  void fromMap(map<string, boost::any> m) override {
    if (m.find("accessKeyId") != m.end()) {
      accessKeyId = make_shared<string>(boost::any_cast<string>(m["accessKeyId"]));
    }
    if (m.find("accessKeySecret") != m.end()) {
      accessKeySecret = make_shared<string>(boost::any_cast<string>(m["accessKeySecret"]));
    }
    if (m.find("securityToken") != m.end()) {
      securityToken = make_shared<string>(boost::any_cast<string>(m["securityToken"]));
    }
    if (m.find("protocol") != m.end()) {
      protocol = make_shared<string>(boost::any_cast<string>(m["protocol"]));
    }
    if (m.find("regionId") != m.end()) {
      regionId = make_shared<string>(boost::any_cast<string>(m["regionId"]));
    }
    if (m.find("readTimeout") != m.end()) {
      readTimeout = make_shared<int>(boost::any_cast<int>(m["readTimeout"]));
    }
    if (m.find("connectTimeout") != m.end()) {
      connectTimeout = make_shared<int>(boost::any_cast<int>(m["connectTimeout"]));
    }
    if (m.find("httpProxy") != m.end()) {
      httpProxy = make_shared<string>(boost::any_cast<string>(m["httpProxy"]));
    }
    if (m.find("httpsProxy") != m.end()) {
      httpsProxy = make_shared<string>(boost::any_cast<string>(m["httpsProxy"]));
    }
    if (m.find("endpoint") != m.end()) {
      endpoint = make_shared<string>(boost::any_cast<string>(m["endpoint"]));
    }
    if (m.find("noProxy") != m.end()) {
      noProxy = make_shared<string>(boost::any_cast<string>(m["noProxy"]));
    }
    if (m.find("maxIdleConns") != m.end()) {
      maxIdleConns = make_shared<int>(boost::any_cast<int>(m["maxIdleConns"]));
    }
    if (m.find("network") != m.end()) {
      network = make_shared<string>(boost::any_cast<string>(m["network"]));
    }
    if (m.find("userAgent") != m.end()) {
      userAgent = make_shared<string>(boost::any_cast<string>(m["userAgent"]));
    }
    if (m.find("suffix") != m.end()) {
      suffix = make_shared<string>(boost::any_cast<string>(m["suffix"]));
    }
    if (m.find("socks5Proxy") != m.end()) {
      socks5Proxy = make_shared<string>(boost::any_cast<string>(m["socks5Proxy"]));
    }
    if (m.find("socks5NetWork") != m.end()) {
      socks5NetWork = make_shared<string>(boost::any_cast<string>(m["socks5NetWork"]));
    }
    if (m.find("endpointType") != m.end()) {
      endpointType = make_shared<string>(boost::any_cast<string>(m["endpointType"]));
    }
    if (m.find("openPlatformEndpoint") != m.end()) {
      openPlatformEndpoint = make_shared<string>(boost::any_cast<string>(m["openPlatformEndpoint"]));
    }
    if (m.find("type") != m.end()) {
      type = make_shared<string>(boost::any_cast<string>(m["type"]));
    }
  }

  shared_ptr<string> accessKeyId{};
  shared_ptr<string> accessKeySecret{};
  shared_ptr<string> securityToken{};
  shared_ptr<string> protocol{};
  shared_ptr<string> regionId{};
  shared_ptr<int> readTimeout{};
  shared_ptr<int> connectTimeout{};
  shared_ptr<string> httpProxy{};
  shared_ptr<string> httpsProxy{};
  shared_ptr<Alibabacloud_Credential::Client> credential{};
  shared_ptr<string> endpoint{};
  shared_ptr<string> noProxy{};
  shared_ptr<int> maxIdleConns{};
  shared_ptr<string> network{};
  shared_ptr<string> userAgent{};
  shared_ptr<string> suffix{};
  shared_ptr<string> socks5Proxy{};
  shared_ptr<string> socks5NetWork{};
  shared_ptr<string> endpointType{};
  shared_ptr<string> openPlatformEndpoint{};
  shared_ptr<string> type{};

  ~Config() = default;
};
class OpenApiRequest : public Darabonba::Model {
public:
  OpenApiRequest() {}
  explicit OpenApiRequest(const std::map<string, boost::any> &config) : Darabonba::Model(config) {
    fromMap(config);
  };

  void validate() override {}

  map<string, boost::any> toMap() override {
    map<string, boost::any> res;
    if (headers) {
      res["headers"] = boost::any(*headers);
    }
    if (query) {
      res["query"] = boost::any(*query);
    }
    if (body) {
      res["body"] = boost::any(*body);
    }
    return res;
  }

  void fromMap(map<string, boost::any> m) override {
    if (m.find("headers") != m.end()) {
      map<string, boost::any> map1 = boost::any_cast<map<string, boost::any>>(m["headers"]);
      map<string, string> toMap1;
      for (auto item:map1) {
         toMap1[item.first] = boost::any_cast<string>(item.second);
      }
      headers = make_shared<map<string, string>>(toMap1);
    }
    if (m.find("query") != m.end()) {
      map<string, boost::any> map1 = boost::any_cast<map<string, boost::any>>(m["query"]);
      map<string, string> toMap1;
      for (auto item:map1) {
         toMap1[item.first] = boost::any_cast<string>(item.second);
      }
      query = make_shared<map<string, string>>(toMap1);
    }
    if (m.find("body") != m.end()) {
      body = make_shared<boost::any>(boost::any_cast<boost::any>(m["body"]));
    }
  }

  shared_ptr<map<string, string>> headers{};
  shared_ptr<map<string, string>> query{};
  shared_ptr<boost::any> body{};

  ~OpenApiRequest() = default;
};
class Client {
public:
  shared_ptr<string> _endpoint{};
  shared_ptr<string> _regionId{};
  shared_ptr<string> _protocol{};
  shared_ptr<string> _userAgent{};
  shared_ptr<string> _endpointRule{};
  shared_ptr<map<string, string>> _endpointMap{};
  shared_ptr<string> _suffix{};
  shared_ptr<int> _readTimeout{};
  shared_ptr<int> _connectTimeout{};
  shared_ptr<string> _httpProxy{};
  shared_ptr<string> _httpsProxy{};
  shared_ptr<string> _socks5Proxy{};
  shared_ptr<string> _socks5NetWork{};
  shared_ptr<string> _noProxy{};
  shared_ptr<string> _network{};
  shared_ptr<string> _productId{};
  shared_ptr<int> _maxIdleConns{};
  shared_ptr<string> _endpointType{};
  shared_ptr<string> _openPlatformEndpoint{};
  shared_ptr<Alibabacloud_Credential::Client> _credential{};
  explicit Client(const shared_ptr<Config>& config);
  map<string, boost::any> doRPCRequest(shared_ptr<string> action,
                                       shared_ptr<string> version,
                                       shared_ptr<string> protocol,
                                       shared_ptr<string> method,
                                       shared_ptr<string> authType,
                                       shared_ptr<string> bodyType,
                                       shared_ptr<OpenApiRequest> request,
                                       shared_ptr<Darabonba_Util::RuntimeOptions> runtime);
  map<string, boost::any> doROARequest(shared_ptr<string> action,
                                       shared_ptr<string> version,
                                       shared_ptr<string> protocol,
                                       shared_ptr<string> method,
                                       shared_ptr<string> authType,
                                       shared_ptr<string> pathname,
                                       shared_ptr<string> bodyType,
                                       shared_ptr<OpenApiRequest> request,
                                       shared_ptr<Darabonba_Util::RuntimeOptions> runtime);
  map<string, boost::any> doROARequestWithForm(shared_ptr<string> action,
                                               shared_ptr<string> version,
                                               shared_ptr<string> protocol,
                                               shared_ptr<string> method,
                                               shared_ptr<string> authType,
                                               shared_ptr<string> pathname,
                                               shared_ptr<string> bodyType,
                                               shared_ptr<OpenApiRequest> request,
                                               shared_ptr<Darabonba_Util::RuntimeOptions> runtime);
  string getUserAgent();
  string getAccessKeyId();
  string getAccessKeySecret();
  string getSecurityToken();
  static boost::any defaultAny(const boost::any &inputValue, const boost::any &defaultValue);
  void checkConfig(shared_ptr<Config> config);

  ~Client() = default;
};
} // namespace Alibabacloud_OpenApi

#endif
