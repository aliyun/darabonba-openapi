#include <alibabacloud/gateway/POP.hpp>
#include <alibabacloud/Utils.hpp>
#include <alibabacloud/OpenapiException.hpp>
#include <darabonba/Array.hpp>
#include <darabonba/Core.hpp>
#include <darabonba/Map.hpp>
#include <darabonba/Convert.hpp>
#include <darabonba/Stream.hpp>
#include <darabonba/Bytes.hpp>
#include <darabonba/String.hpp>
#include <darabonba/encode/Encoder.hpp>
#include <darabonba/signature/Signer.hpp>
#include <darabonba/XML.hpp>
#include <algorithm>
#include <cstdlib>
#include <set>
#include <sstream>
#include <unordered_set>
#include <utility>

using namespace AlibabaCloud::OpenApi::Utils;
using namespace AlibabaCloud::OpenApi::Exceptions;

namespace AlibabaCloud {

namespace Gateway {

namespace {
Darabonba::Bytes assertAsBytes(const Darabonba::Json &value);

std::string checkedEndpointSuffix(const std::string &value) {
  const auto first = value.find_first_not_of(" \t\r\n\f\v");
  if (first == std::string::npos) {
    return "";
  }
  const auto last = value.find_last_not_of(" \t\r\n\f\v");
  std::string suffix = value.substr(first, last - first + 1);
  for (char &c : suffix) {
    if (c >= 'A' && c <= 'Z') {
      c = static_cast<char>(c - 'A' + 'a');
    }
  }
  if (suffix != "aliyuncs.com" && suffix != "aliyuncs.alibaba") {
    throw ClientException(json({
        {"code", "InvalidEndpointSuffix"},
        {"message", "endpointSuffix must be aliyuncs.com or aliyuncs.alibaba"}}));
  }
  return suffix;
}
} // namespace

POP::POP() {
  this->_signatureTypePrefix = "ACS4-";
  this->_signPrefix = "aliyun_v4";
  this->_endpointSuffix = "aliyuncs.com";
  this->_sha256 = this->_signatureTypePrefix + "HMAC-SHA256";
  this->_sm3 = this->_signatureTypePrefix + "HMAC-SM3";
}

void POP::modifyConfiguration(InterceptorContext &context,
                              AttributeMap &attributeMap) {
  (void)attributeMap;
  if(!context.hasInterceptorContextConfiguration() && !context.hasInterceptorContextRequest()) {
    throw ClientException(json({
         {"code" , "ParameterMissing"},
         {"message" , "'config' or 'request' can not be unset"}
     }));
  }
  std::string endpointSuffix = resolveEndpointSuffix(attributeMap);
  InterceptorContextRequest request = context.getRequest();
  InterceptorContextConfiguration config = context.getConfiguration();
  config.setEndpoint(getEndpoint(request.getProductId(), config.getRegionId(),
                                 config.getEndpointRule(), config.getNetwork(),
                                 config.getSuffix(), config.getEndpointMap(),
                                 config.getEndpoint(), endpointSuffix));
  context.setInterceptorContextConfiguration(config);
}

void POP::modifyRequest(InterceptorContext &context,
                        AttributeMap &attributeMap) {
  if(!context.hasInterceptorContextConfiguration() && !context.hasInterceptorContextRequest()) {
    throw ClientException(json({
                                   {"code" , "ParameterMissing"},
                                   {"message" , "'config' or 'request' can not be unset"}
                               }));
  }
  InterceptorContextRequest request = context.getRequest();
  InterceptorContextConfiguration config = context.getConfiguration();
  RequestSignature names = requestSignature(attributeMap);
  auto date = Utils::getTimestamp();
  // Getters return copies. Keep the header map local and write the request back.
  Darabonba::Http::Header headers = Darabonba::Core::merge(
                         Darabonba::Json({{"host", config.getEndpoint()},
                                          {"x-acs-version", request.getVersion()},
                                          {"user-agent", request.getUserAgent()},
                                          {"x-acs-date", date},
                                          {"x-acs-signature-nonce",
                                           Utils::getNonce()},
                                          {"accept", "application/json"}}),
                         request.getHeaders())
                         .get<std::map<std::string, std::string>>();
  if (!request.getAction().empty()) {
    headers["x-acs-action"] = request.getAction();
  }

  std::string signatureAlgorithm =
      Darabonba::Convert::stringVal(Darabonba::defaultVal(request.getSignatureAlgorithm(), names.sha256));
  std::string hashedRequestPayload =
      Darabonba::Encode::Encoder::hexEncode(Darabonba::Encode::Encoder::hash(
          Darabonba::BytesUtil::toBytes(""), signatureAlgorithm));
  if (request.hasStream()) {
    auto tmp = Darabonba::Stream::readAsBytes(request.getStream());
    hashedRequestPayload = Darabonba::Encode::Encoder::hexEncode(
        Darabonba::Encode::Encoder::hash(tmp, signatureAlgorithm));
    request.setStream(std::make_shared<Darabonba::ISStream>(tmp));
    headers["content-type"] = "application/octet-stream";
  } else if (request.hasBody()) {
    if (request.getReqBodyType() == "byte") {
      Darabonba::Bytes byteObj = assertAsBytes(request.getBody());
      hashedRequestPayload = Darabonba::Encode::Encoder::hexEncode(
          Darabonba::Encode::Encoder::hash(byteObj, signatureAlgorithm));
      request.setStream(std::make_shared<Darabonba::ISStream>(byteObj));
    } else if (request.getReqBodyType() == "json") {
      string jsonObj = request.getBody().dump();
      hashedRequestPayload = Darabonba::Encode::Encoder::hexEncode(
          Darabonba::Encode::Encoder::hash(Darabonba::BytesUtil::toBytes(jsonObj),
                                           signatureAlgorithm));
      request.setStream(std::make_shared<Darabonba::ISStream>(jsonObj));
      headers["content-type"] = "application/json; charset=utf-8";
    } else {
      json m = json(request.getBody());
      auto formObj = Utils::toForm(m);
      hashedRequestPayload = Darabonba::Encode::Encoder::hexEncode(
          Darabonba::Encode::Encoder::hash(Darabonba::BytesUtil::toBytes(formObj),
                                           signatureAlgorithm));
      request.setStream(std::make_shared<Darabonba::ISStream>(formObj));
      headers["content-type"] = "application/x-www-form-urlencoded";
    }
  }

  if (signatureAlgorithm == names.sm3) {
    headers["x-acs-content-sm3"] = hashedRequestPayload;
  } else {
    headers["x-acs-content-sha256"] = hashedRequestPayload;
  }

  if (request.getAuthType() != "Anonymous") {
    if (!request.hasCredential() || request.getCredential() == nullptr) {
      throw ClientException(json({
          {"code", "ParameterMissing"},
          {"message", "'config.credential' can not be unset"}}));
    }
    AlibabaCloud::Credentials::Models::CredentialModel credentialModel =
        request.getCredential()->getCredential();
    if (credentialModel.hasProviderName() &&
        !credentialModel.getProviderName().empty()) {
      headers["x-acs-credentials-provider"] = credentialModel.getProviderName();
    }
    std::string authType = credentialModel.getType();
    if (authType == "bearer") {
      std::string bearerToken = credentialModel.getBearerToken();
      headers["x-acs-bearer-token"] = bearerToken;
      headers["x-acs-signature-type"] = "BEARERTOKEN";
      headers["Authorization"] = "Bearer " + bearerToken;
    } else if (authType == "id_token") {
      headers["x-acs-zero-trust-idtoken"] = credentialModel.getSecurityToken();
    } else {
      std::string accessKeyId = credentialModel.getAccessKeyId();
      std::string accessKeySecret = credentialModel.getAccessKeySecret();
      std::string securityToken = credentialModel.getSecurityToken();
      if (!securityToken.empty()) {
        headers["x-acs-accesskey-id"] = accessKeyId;
        headers["x-acs-security-token"] = securityToken;
      }

      auto dateNew = Darabonba::String::subString(date, 0, 10);
      dateNew = Darabonba::String::replace(dateNew, "-", "");
      auto region = regionFor(request.getProductId(), config.getEndpoint(),
                              config.getRegionId(), _endpointSuffix);
      auto signingkey =
          signingKeyWith(signatureAlgorithm, accessKeySecret,
                         request.getProductId(), region, dateNew, names);
      headers["Authorization"] = authorizationWith(
          request.getPathname(), request.getMethod(), request.getQuery(),
          headers, signatureAlgorithm, hashedRequestPayload, accessKeyId,
          signingkey, request.getProductId(), region, dateNew, names);
    }
  }
  request.setHeaders(headers);
  context.setRequest(request);
}

void POP::modifyResponse(InterceptorContext &context,
                         AttributeMap &attributeMap) {
  (void)attributeMap;
  auto request = context.getRequest();
  auto response = context.getResponse();
  if ((response.getStatusCode() >= 400) && (response.getStatusCode() < 600)) {
    json err;
    bool xml = false;
    for (const auto &header : response.getHeaders()) {
      if (Darabonba::String::toLower(header.first) == "content-type" &&
          header.second.find("text/xml") != std::string::npos) {
        xml = true;
        break;
      }
    }
    if (xml) {
      std::string body = Darabonba::Stream::readAsString(response.getBody());
      json parsed = Darabonba::XML::parseXml(body, nullptr);
      if (!parsed.is_object() || !parsed.contains("Error") ||
          !parsed["Error"].is_object()) {
        throw ClientException(json({
            {"code", "InvalidResponse"},
            {"message", "POP error response is missing the Error element"}}));
      }
      err = parsed["Error"];
    } else {
      auto _res = Darabonba::Stream::readAsJSON(response.getBody());
      err = json(_res);
    }
    Darabonba::Json requestId = Darabonba::defaultVal(err.value("RequestId", ""), err.value("requestId", ""));
    Darabonba::Json code = Darabonba::defaultVal(err.value("Code", ""), err.value("code", ""));
    if (response.getHeaders().count("x-acs-request-id")) {
      requestId = response.getHeaders()["x-acs-request-id"];
    }

    Darabonba::Json accessDeniedDetail = nullptr;
    if (err.contains("AccessDeniedDetail")) {
      accessDeniedDetail = err["AccessDeniedDetail"];
    } else if (err.contains("accessDeniedDetail")) {
      accessDeniedDetail = err["accessDeniedDetail"];
    }

    err["statusCode"] = response.getStatusCode();
    throw ClientException(json({
         {"statusCode" , response.getStatusCode()},
         {"code" , DARA_STRING_TEMPLATE("" , code)},
         {"message" , DARA_STRING_TEMPLATE("code: " , response.getStatusCode() , ", " , Darabonba::defaultVal(err.value("Message", ""), err.value("message", "")) , " request id: " , requestId)},
         {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err.value("Description", ""), err.value("description", "")))},
         {"data" , err},
         {"accessDeniedDetail" , accessDeniedDetail},
         {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
     }));
  }

  if (response.getStatusCode() == 204) {
    Darabonba::Stream::readAsString(response.getBody());
  } else if (request.getBodyType() == "binary") {
    // Hand the still-readable body to the product model. Do not buffer it.
    response.setDeserializedBody(Darabonba::Stream::retain(response.getBody()));
  } else if (request.getBodyType() == "byte") {
    auto byt = Darabonba::Stream::readAsBytes(response.getBody());
    response.setDeserializedBody(byt);
  } else if (request.getBodyType() == "string") {
    auto str = Darabonba::Stream::readAsString(response.getBody());
    response.setDeserializedBody(str);
  } else if (request.getBodyType() == "json") {
    auto obj = Darabonba::Stream::readAsJSON(response.getBody());
    auto res = json(obj);
    response.setDeserializedBody(res);
  } else if (request.getBodyType() == "array") {
    auto arr = Darabonba::Stream::readAsJSON(response.getBody());
    response.setDeserializedBody(arr);
  } else {
    response.setDeserializedBody(
        Darabonba::Stream::readAsString(response.getBody()));
  }
  context.setResponse(response);
}

namespace {

bool endsWith(const std::string &value, const std::string &suffix) {
  return !suffix.empty() && suffix.size() <= value.size() &&
         value.compare(value.size() - suffix.size(), suffix.size(), suffix) == 0;
}

std::string hostWithoutPort(const std::string &endpoint) {
  auto colon = endpoint.find(':');
  if (colon == std::string::npos) {
    return endpoint;
  }
  return endpoint.substr(0, colon);
}

Darabonba::Bytes assertAsBytes(const Darabonba::Json &value) {
  if (value.is_binary()) {
    return value.get_binary();
  }
  if (value.is_array()) {
    Darabonba::Bytes bytes;
    for (const auto &item : value) {
      if (!item.is_number_integer() || item.get<int>() < 0 || item.get<int>() > 255) {
        throw ClientException(json({
            {"code", "InvalidParameter"},
            {"message", "The value is not a byteArray"}}));
      }
      bytes.push_back(static_cast<uint8_t>(item.get<int>()));
    }
    return bytes;
  }
  throw ClientException(json({
      {"code", "InvalidParameter"},
      {"message", "The value is not a byteArray"}}));
}

std::string hostWithoutDomain(const std::string &host, const std::string &suffix) {
  const std::string domains[] = {suffix, "aliyuncs.com", "aliyuncs.alibaba"};
  for (size_t i = 0; i < 3; ++i) {
    if (domains[i].empty()) {
      continue;
    }
    bool seen = false;
    for (size_t j = 0; j < i; ++j) {
      if (domains[j] == domains[i]) {
        seen = true;
      }
    }
    if (seen) {
      continue;
    }
    const std::string needle = "." + domains[i];
    if (endsWith(host, needle)) {
      return host.substr(0, host.size() - needle.size());
    }
  }
  return host;
}

} // namespace

std::string
POP::getEndpoint(const std::string &productId, const std::string &regionId,
                 const std::string &endpointRule, const std::string &network,
                 const std::string &suffix,
                 const std::map<std::string, std::string> &endpointMap,
                 const std::string &endpoint,
                 const std::string &endpointSuffix) {
  std::string resolved = endpointSuffix.empty() ? _endpointSuffix : endpointSuffix;
  if (!endpoint.empty()) {
    return applyEndpointSuffix(endpoint, resolved);
  }

  if (!endpointMap.empty()) {
    auto it = endpointMap.find(regionId);
    if (it != endpointMap.end())
      return applyEndpointSuffix(it->second, resolved);
  }

  return applyEndpointSuffix(Utils::getEndpointRules(productId, regionId, endpointRule,
                                        network, suffix), resolved);
}

std::string POP::resolveEndpointSuffix(AttributeMap &attributeMap) const {
  if (attributeMap.hasKey()) {
    std::map<std::string, std::string> key = attributeMap.getKey();
    auto it = key.find("endpointSuffix");
    if (it != key.end()) {
      std::string suffix = checkedEndpointSuffix(it->second);
      if (!suffix.empty()) {
        return suffix;
      }
    }
  }
  const char *envSuffix = std::getenv("ALIBABA_CLOUD_ENDPOINT_SUFFIX");
  if (envSuffix != nullptr && envSuffix[0] != '\0') {
    std::string suffix = checkedEndpointSuffix(envSuffix);
    if (!suffix.empty()) {
      return suffix;
    }
  }
  return "aliyuncs.com";
}

std::string POP::applyEndpointSuffix(const std::string &endpoint) const {
  return applyEndpointSuffix(endpoint, _endpointSuffix);
}

std::string POP::applyEndpointSuffix(const std::string &endpoint,
                                     const std::string &endpointSuffix) const {
  std::string suffix = checkedEndpointSuffix(endpointSuffix);
  if (suffix.empty()) {
    suffix = "aliyuncs.com";
  }
  if (suffix == "aliyuncs.com" || endpoint.empty()) {
    return endpoint;
  }
  const std::string old = ".aliyuncs.com";
  if (endpoint.size() >= old.size() &&
      endpoint.compare(endpoint.size() - old.size(), old.size(), old) == 0) {
    return endpoint.substr(0, endpoint.size() - old.size()) + "." + suffix;
  }
  return endpoint;
}

Darabonba::Json POP::defaultAny(Darabonba::Json &inputValue,
                                Darabonba::Json &defaultValue) {
  if (Darabonba::isNull(inputValue)) {
    return defaultValue;
  }

  return inputValue;
}

POP::RequestSignature
POP::requestSignature(const AttributeMap &attributeMap) const {
  RequestSignature names;
  names.signPrefix = _signPrefix;
  names.sha256 = _sha256;
  names.sm3 = _sm3;
  if (!attributeMap.hasKey()) {
    return names;
  }
  const std::map<std::string, std::string> &keys = attributeMap.key();
  auto prefix = keys.find("signatureTypePrefix");
  if (prefix != keys.end() && !prefix->second.empty()) {
    names.sha256 = prefix->second + "HMAC-SHA256";
    names.sm3 = prefix->second + "HMAC-SM3";
  }
  auto signPrefix = keys.find("signPrefix");
  if (signPrefix != keys.end() && !signPrefix->second.empty()) {
    names.signPrefix = signPrefix->second;
  }
  return names;
}

std::string POP::getAuthorization(
    const std::string &pathname, const std::string &method,
    const Darabonba::Http::Query &query, const Darabonba::Http::Header &headers,
    const std::string &signatureAlgorithm, const std::string &payload,
    const std::string &ak, const Darabonba::Bytes &signingkey,
    const std::string &product, const std::string &region,
    const std::string &date) {
  RequestSignature names;
  names.signPrefix = _signPrefix;
  names.sha256 = _sha256;
  names.sm3 = _sm3;
  return authorizationWith(pathname, method, query, headers, signatureAlgorithm,
                           payload, ak, signingkey, product, region, date,
                           names);
}

std::string POP::authorizationWith(
    const std::string &pathname, const std::string &method,
    const Darabonba::Http::Query &query, const Darabonba::Http::Header &headers,
    const std::string &signatureAlgorithm, const std::string &payload,
    const std::string &ak, const Darabonba::Bytes &signingkey,
    const std::string &product, const std::string &region,
    const std::string &date, const RequestSignature &names) {
  auto signature = signatureWith(pathname, method, query, headers,
                                 signatureAlgorithm, payload, signingkey, names);
  auto signedHeaders = getSignedHeaders(headers);
  auto signedHeadersStr = Darabonba::Array::join(signedHeaders, ";");
  return signatureAlgorithm + " Credential=" + ak + "/" + date + "/" + region +
         "/" + product + "/" + names.signPrefix +
         "_request,SignedHeaders=" + signedHeadersStr +
         ",Signature=" + signature;
}

std::string POP::getSignature(const std::string &pathname,
                              const std::string &method,
                              const Darabonba::Http::Query &query,
                              const Darabonba::Http::Header &headers,
                              const std::string &signatureAlgorithm,
                              const std::string &payload,
                              const Darabonba::Bytes &signingkey) {
  RequestSignature names;
  names.signPrefix = _signPrefix;
  names.sha256 = _sha256;
  names.sm3 = _sm3;
  return signatureWith(pathname, method, query, headers, signatureAlgorithm,
                       payload, signingkey, names);
}

std::string POP::signatureWith(const std::string &pathname,
                               const std::string &method,
                               const Darabonba::Http::Query &query,
                               const Darabonba::Http::Header &headers,
                               const std::string &signatureAlgorithm,
                               const std::string &payload,
                               const Darabonba::Bytes &signingkey,
                               const RequestSignature &names) {
  std::string canonicalURI = "/";
  if (!pathname.empty()) {
    canonicalURI = pathname;
  }

  std::string canonicalizedResource = buildCanonicalizedResource(query);
  std::string canonicalizedHeaders = buildCanonicalizedHeaders(headers);
  std::vector<std::string> signedHeaders = getSignedHeaders(headers);
  validateSignedHeaders(signedHeaders);
  std::string signedHeadersStr = Darabonba::Array::join(signedHeaders, ";");
  std::string stringToSign = method + "\n" + canonicalURI + "\n" +
                             canonicalizedResource + "\n" + canonicalizedHeaders +
                             "\n" + signedHeadersStr + "\n" + payload;
  std::string hex =
      Darabonba::Encode::Encoder::hexEncode(Darabonba::Encode::Encoder::hash(
          Darabonba::BytesUtil::toBytes(stringToSign), signatureAlgorithm));
  stringToSign = signatureAlgorithm + "\n" + hex;

  Darabonba::Bytes signature = Darabonba::BytesUtil::toBytes("");
  if (signatureAlgorithm == names.sha256) {
    signature = Darabonba::Signature::Signer::HmacSHA256SignByBytes(
        stringToSign, signingkey);
  } else if (signatureAlgorithm == names.sm3) {
    signature = Darabonba::Signature::Signer::HmacSM3SignByBytes(stringToSign,
                                                                 signingkey);
  }

  return Darabonba::Encode::Encoder::hexEncode(signature);
}

void POP::validateSignedHeaders(
    const std::vector<std::string> &signedHeaders) const {
  bool hasHost = false;
  bool hasDate = false;
  for (const auto &key : signedHeaders) {
    if (key == "host") {
      hasHost = true;
    }
    if (key == "x-acs-date") {
      hasDate = true;
    }
  }
  if (!hasHost || !hasDate) {
    throw ClientException(json({
        {"code", "InvalidSignedHeaders"},
        {"message", "signed headers must include host and x-acs-date"}}));
  }
}

Darabonba::Bytes POP::getSigningkey(const std::string &signatureAlgorithm,
                                    const std::string &secret,
                                    const std::string &product,
                                    const std::string &region,
                                    const std::string &date) {
  RequestSignature names;
  names.signPrefix = _signPrefix;
  names.sha256 = _sha256;
  names.sm3 = _sm3;
  return signingKeyWith(signatureAlgorithm, secret, product, region, date,
                        names);
}

Darabonba::Bytes POP::signingKeyWith(const std::string &signatureAlgorithm,
                                     const std::string &secret,
                                     const std::string &product,
                                     const std::string &region,
                                     const std::string &date,
                                     const RequestSignature &names) {
  std::string sc1 = names.signPrefix + secret;
  Darabonba::Bytes sc2 = Darabonba::BytesUtil::toBytes("");
  if (signatureAlgorithm == names.sha256) {
    sc2 = Darabonba::Signature::Signer::HmacSHA256Sign(date, sc1);
  } else if (signatureAlgorithm == names.sm3) {
    sc2 = Darabonba::Signature::Signer::HmacSM3Sign(date, sc1);
  }

  Darabonba::Bytes sc3 = Darabonba::BytesUtil::toBytes("");
  if (signatureAlgorithm == names.sha256) {
    sc3 = Darabonba::Signature::Signer::HmacSHA256SignByBytes(region, sc2);
  } else if (signatureAlgorithm == names.sm3) {
    sc3 = Darabonba::Signature::Signer::HmacSM3SignByBytes(region, sc2);
  }

  Darabonba::Bytes sc4 = Darabonba::BytesUtil::toBytes("");
  if (signatureAlgorithm == names.sha256) {
    sc4 = Darabonba::Signature::Signer::HmacSHA256SignByBytes(product, sc3);
  } else if (signatureAlgorithm == names.sm3) {
    sc4 = Darabonba::Signature::Signer::HmacSM3SignByBytes(product, sc3);
  }

  Darabonba::Bytes hmac = Darabonba::BytesUtil::toBytes("");
  if (signatureAlgorithm == names.sha256) {
    hmac = Darabonba::Signature::Signer::HmacSHA256SignByBytes(
        names.signPrefix + "_request", sc4);
  } else if (signatureAlgorithm == names.sm3) {
    hmac = Darabonba::Signature::Signer::HmacSM3SignByBytes(
        names.signPrefix + "_request", sc4);
  }

  return hmac;
}

std::string POP::getRegion(const std::string &product,
                           const std::string &endpoint,
                           const std::string &endpointSuffix) {
  // The third argument stays the endpoint suffix. regionId is applied by the
  // request path through regionFor, not by this public signature.
  std::string suffix = endpointSuffix.empty() ? _endpointSuffix : endpointSuffix;
  return regionFor(product, endpoint, "", suffix);
}

std::string POP::regionFor(const std::string &product,
                           const std::string &endpoint,
                           const std::string &regionId,
                           const std::string &endpointSuffix) const {
  if (!regionId.empty()) {
    return regionId;
  }
  if (product.empty() || endpoint.empty()) {
    return "center";
  }

  std::string host = hostWithoutPort(endpoint);
  std::string preRegion = hostWithoutDomain(host, endpointSuffix);
  std::vector<std::string> nodes;
  std::string current;
  for (char ch : preRegion) {
    if (ch == '.') {
      nodes.push_back(current);
      current.clear();
    } else {
      current.push_back(ch);
    }
  }
  nodes.push_back(current);
  if (nodes.size() == 2) {
    return nodes[1];
  }
  return "center";
}

std::string
POP::buildCanonicalizedResource(const Darabonba::Http::Query &query) {
  std::vector<std::pair<std::string, std::string>> pairs;
  pairs.reserve(query.size());
  for (const auto &item : query) {
    std::string encodedValue;
    if (!item.second.empty()) {
      encodedValue = Darabonba::Encode::Encoder::percentEncode(item.second);
    }
    pairs.emplace_back(Darabonba::Encode::Encoder::percentEncode(item.first),
                       std::move(encodedValue));
  }
  std::sort(pairs.begin(), pairs.end(),
            [](const std::pair<std::string, std::string> &left,
               const std::pair<std::string, std::string> &right) {
              return left.first < right.first;
            });
  std::string canonicalizedResource;
  std::string separator;
  for (const auto &item : pairs) {
    canonicalizedResource += separator;
    canonicalizedResource += item.first;
    canonicalizedResource += "=";
    canonicalizedResource += item.second;
    separator = "&";
  }
  return canonicalizedResource;
}

std::string
POP::buildCanonicalizedHeaders(const Darabonba::Http::Header &headers) {
  std::map<std::string, std::string> folded;
  for (const auto &item : headers) {
    std::string lowerKey = Darabonba::String::toLower(item.first);
    std::string value = Darabonba::String::trim(item.second);
    auto found = folded.find(lowerKey);
    if (found == folded.end() || lowerKey == "host") {
      folded[lowerKey] = value;
    } else {
      found->second += "," + value;
    }
  }
  std::string canonicalizedHeaders;
  for (const auto &key : getSignedHeaders(headers)) {
    canonicalizedHeaders += key + ":" + folded[key] + "\n";
  }
  return canonicalizedHeaders;
}

std::vector<std::string>
POP::getSignedHeaders(const Darabonba::Http::Header &headers) {
  std::set<std::string> ret;

  for (const auto &p : headers) {
    auto lowerKey = Darabonba::String::toLower(p.first);
    if (Darabonba::String::hasPrefix(lowerKey, "x-acs-") ||
        Darabonba::String::equals(lowerKey, "host") ||
        Darabonba::String::equals(lowerKey, "content-type")) {
      ret.insert(lowerKey);
    }
  }
  return {ret.begin(), ret.end()};
}

} // namespace Gateway
} // namespace Alibabacloud
