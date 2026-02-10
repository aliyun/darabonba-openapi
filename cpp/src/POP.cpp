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
#include <set>
#include <sstream>
#include <unordered_set>

using namespace AlibabaCloud::OpenApi::Utils;
using namespace AlibabaCloud::OpenApi::Exceptions;

namespace AlibabaCloud {

namespace Gateway {

POP::POP() {
  this->_sha256 = "ACS4-HMAC-SHA256";
  this->_sm3 = "ACS4-HMAC-SM3";
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
  InterceptorContextRequest request = context.getRequest();
  InterceptorContextConfiguration config = context.getConfiguration();
  config.setEndpoint(getEndpoint(request.getProductId(), config.getRegionId(),
                                 config.getEndpointRule(), config.getNetwork(),
                                 config.getSuffix(), config.getEndpointMap(),
                                 config.getEndpoint()));
}

void POP::modifyRequest(InterceptorContext &context,
                        AttributeMap &attributeMap) {
  (void)attributeMap;
  if(!context.hasInterceptorContextConfiguration() && !context.hasInterceptorContextRequest()) {
    throw ClientException(json({
                                   {"code" , "ParameterMissing"},
                                   {"message" , "'config' or 'request' can not be unset"}
                               }));
  }
  InterceptorContextRequest request = context.getRequest();
  InterceptorContextConfiguration config = context.getConfiguration();
  auto date = Utils::getTimestamp();
  request.setHeaders(Darabonba::Core::merge(
                         Darabonba::Json({{"host", config.getEndpoint()},
                                          {"x-acs-version", request.getVersion()},
                                          {"x-acs-action", request.getAction()},
                                          {"user-agent", request.getUserAgent()},
                                          {"x-acs-date", date},
                                          {"x-acs-signature-nonce",
                                           Utils::getNonce()},
                                          {"accept", "application/json"}}),
                         request.getHeaders())
                         .get<std::map<std::string, std::string>>());

  std::string signatureAlgorithm =
      Darabonba::Convert::stringVal(Darabonba::defaultVal(request.getSignatureAlgorithm(), _sha256));
  std::string hashedRequestPayload =
      Darabonba::Encode::Encoder::hexEncode(Darabonba::Encode::Encoder::hash(
          Darabonba::BytesUtil::toBytes(""), signatureAlgorithm));
  // if (!Darabonba::Util::isUnset(request.getStream())) {
  if (request.hasStream()) {
    auto tmp = Darabonba::Stream::readAsBytes(request.getStream());
    hashedRequestPayload = Darabonba::Encode::Encoder::hexEncode(
        Darabonba::Encode::Encoder::hash(tmp, signatureAlgorithm));
    request.setStream(std::make_shared<Darabonba::ISStream>(tmp));
    request.getHeaders()["content-type"] = "application/octet-stream";
  } else {
    // if (!Darabonba::Util::isUnset(request.getBody())) {
    if (request.hasBody()) {
      if (request.getReqBodyType() == "json") {
        string jsonObj = request.getBody().dump();
        hashedRequestPayload = Darabonba::Encode::Encoder::hexEncode(
            Darabonba::Encode::Encoder::hash(Darabonba::BytesUtil::toBytes(jsonObj),
                                             signatureAlgorithm));
        request.setStream(std::make_shared<Darabonba::ISStream>(jsonObj));
        request.getHeaders()["content-type"] = "application/json; charset=utf-8";
      } else {
        json m = json(request.getBody());
        auto formObj = Utils::toForm(m);
        hashedRequestPayload = Darabonba::Encode::Encoder::hexEncode(
            Darabonba::Encode::Encoder::hash(Darabonba::BytesUtil::toBytes(formObj),
                                             signatureAlgorithm));
        request.setStream(std::make_shared<Darabonba::ISStream>(formObj));
        request.getHeaders()["content-type"] = "application/x-www-form-urlencoded";
      }
    }
  }

  if (signatureAlgorithm == _sm3) {
    request.getHeaders()["x-acs-content-sm3"] = hashedRequestPayload;
  } else {
    request.getHeaders()["x-acs-content-sha256"] = hashedRequestPayload;
  }

  if (request.getAuthType() != "Anonymous") {
    auto credential = request.getCredential();
    auto accessKeyId = credential->getAccessKeyId();
    auto accessKeySecret = credential->getAccessKeySecret();
    auto securityToken = credential->getSecurityToken();
    if (!securityToken.empty()) {
      request.getHeaders()["x-acs-accesskey-id"] = accessKeyId;
      request.getHeaders()["x-acs-security-token"] = securityToken;
    }

    auto dateNew = Darabonba::String::subString(date, 0, 10);
    dateNew = Darabonba::String::replace(dateNew, "-", "");
    auto region = getRegion(request.getProductId(), config.getEndpoint());
    auto signingkey = getSigningkey(signatureAlgorithm, accessKeySecret,
                                    request.getProductId(), region, dateNew);
    request.getHeaders()["Authorization"] = getAuthorization(
        request.getPathname(), request.getMethod(), request.getQuery(),
        request.getHeaders(), signatureAlgorithm, hashedRequestPayload,
        accessKeyId, signingkey, request.getProductId(), region, dateNew);
  }
}

void POP::modifyResponse(InterceptorContext &context,
                         AttributeMap &attributeMap) {
  (void)attributeMap;
  auto request = context.getRequest();
  auto response = context.getResponse();
  if ((response.getStatusCode() >= 400) && (response.getStatusCode() < 600)) {
    auto _res = Darabonba::Stream::readAsJSON(response.getBody());
    json err = json(_res);
    Darabonba::Json requestId = Darabonba::defaultVal(err.value("RequestId", ""), err.value("requestId", ""));
    Darabonba::Json code = Darabonba::defaultVal(err.value("Code", ""), err.value("code", ""));
    // if (!Darabonba::Util::isUnset(response.getHeaders()["x-acs-request-id"])) {
    if (response.getHeaders().count("x-acs-request-id")) {
      requestId = response.getHeaders()["x-acs-request-id"];
    }

    err["statusCode"] = response.getStatusCode();
    throw ClientException(json({
         {"statusCode" , response.getStatusCode()},
         {"code" , DARA_STRING_TEMPLATE("" , code)},
         {"message" , DARA_STRING_TEMPLATE("code: " , response.getStatusCode() , ", " , Darabonba::defaultVal(err.value("Message", ""), err.value("message", "")) , " request id: " , requestId)},
         {"description" , DARA_STRING_TEMPLATE("" , Darabonba::defaultVal(err.value("Description", ""), err.value("description", "")))},
         {"data" , err},
         {"requestId" , DARA_STRING_TEMPLATE("" , requestId)}
     }));
  }

  if (response.getStatusCode() == 204) {
    Darabonba::Stream::readAsString(response.getBody());
  } else if (request.getBodyType() == "binary") {
    response.setBody(response.getBody());
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
}

std::string
POP::getEndpoint(const std::string &productId, const std::string &regionId,
                 const std::string &endpointRule, const std::string &network,
                 const std::string &suffix,
                 const std::map<std::string, std::string> &endpointMap,
                 const std::string &endpoint) {
  if (!endpoint.empty()) {
    return endpoint;
  }

  if (!endpointMap.empty()) {
    auto it = endpointMap.find(regionId);
    if (it != endpointMap.end())
      return it->second;
  }

  return Utils::getEndpointRules(productId, regionId, endpointRule,
                                        network, suffix);
}

Darabonba::Json POP::defaultAny(Darabonba::Json &inputValue,
                                Darabonba::Json &defaultValue) {
  if (Darabonba::isNull(inputValue)) {
    return defaultValue;
  }

  return inputValue;
}

std::string POP::getAuthorization(
    const std::string &pathname, const std::string &method,
    const Darabonba::Http::Query &query, const Darabonba::Http::Header &headers,
    const std::string &signatureAlgorithm, const std::string &payload,
    const std::string &ak, const Darabonba::Bytes &signingkey,
    const std::string &product, const std::string &region,
    const std::string &date) {
  auto signature = getSignature(pathname, method, query, headers,
                                signatureAlgorithm, payload, signingkey);
  auto signedHeaders = getSignedHeaders(headers);
  auto signedHeadersStr = Darabonba::Array::join(signedHeaders, ";");
  return signatureAlgorithm + " Credential=" + ak + "/" + date + "/" + region +
         "/" + product +
         "/aliyun_v4_request,SignedHeaders=" + signedHeadersStr +
         ",Signature=" + signature;
}

std::string POP::getSignature(const std::string &pathname,
                              const std::string &method,
                              const Darabonba::Http::Query &query,
                              const Darabonba::Http::Header &headers,
                              const std::string &signatureAlgorithm,
                              const std::string &payload,
                              const Darabonba::Bytes &signingkey) {
  std::string canonicalURI = "/";
  if (!pathname.empty()) {
    canonicalURI = pathname;
  }

  std::string stringToSign = "";
  std::string canonicalizedResource = buildCanonicalizedResource(query);
  std::string canonicalizedHeaders = buildCanonicalizedHeaders(headers);
  std::vector<std::string> signedHeaders = getSignedHeaders(headers);
  std::string signedHeadersStr = Darabonba::Array::join(signedHeaders, ";");
  stringToSign = method + "\n" + canonicalURI + "\n" + canonicalizedResource +
                 "\n" + canonicalizedHeaders + "\n" + signedHeadersStr + "\n" +
                 payload;
  std::string hex =
      Darabonba::Encode::Encoder::hexEncode(Darabonba::Encode::Encoder::hash(
          Darabonba::BytesUtil::toBytes(stringToSign), signatureAlgorithm));
  stringToSign = signatureAlgorithm + "\n" + hex;

  Darabonba::Bytes signature = Darabonba::BytesUtil::toBytes("");
  if (signatureAlgorithm == _sha256) {
    signature = Darabonba::Signature::Signer::HmacSHA256SignByBytes(
        stringToSign, signingkey);
  } else if (signatureAlgorithm == _sm3) {
    signature = Darabonba::Signature::Signer::HmacSM3SignByBytes(stringToSign,
                                                                 signingkey);
  }

  return Darabonba::Encode::Encoder::hexEncode(signature);
}

Darabonba::Bytes POP::getSigningkey(const std::string &signatureAlgorithm,
                                    const std::string &secret,
                                    const std::string &product,
                                    const std::string &region,
                                    const std::string &date) {
  std::string sc1 = "aliyun_v4" + secret;
  Darabonba::Bytes sc2 = Darabonba::BytesUtil::toBytes("");
  if (signatureAlgorithm == _sha256) {
    sc2 = Darabonba::Signature::Signer::HmacSHA256Sign(date, sc1);
  } else if (signatureAlgorithm == _sm3) {
    sc2 = Darabonba::Signature::Signer::HmacSM3Sign(date, sc1);
  }

  Darabonba::Bytes sc3 = Darabonba::BytesUtil::toBytes("");
  if (signatureAlgorithm == _sha256) {
    sc3 = Darabonba::Signature::Signer::HmacSHA256SignByBytes(region, sc2);
  } else if (signatureAlgorithm == _sm3) {
    sc3 = Darabonba::Signature::Signer::HmacSM3SignByBytes(region, sc2);
  }

  Darabonba::Bytes sc4 = Darabonba::BytesUtil::toBytes("");
  if (signatureAlgorithm == _sha256) {
    sc4 = Darabonba::Signature::Signer::HmacSHA256SignByBytes(product, sc3);
  } else if (signatureAlgorithm == _sm3) {
    sc4 = Darabonba::Signature::Signer::HmacSM3SignByBytes(product, sc3);
  }

  Darabonba::Bytes hmac = Darabonba::BytesUtil::toBytes("");
  if (signatureAlgorithm == _sha256) {
    hmac = Darabonba::Signature::Signer::HmacSHA256SignByBytes(
        "aliyun_v4_request", sc4);
  } else if (signatureAlgorithm == _sm3) {
    hmac = Darabonba::Signature::Signer::HmacSM3SignByBytes("aliyun_v4_request",
                                                            sc4);
  }

  return hmac;
}

std::string POP::getRegion(const std::string &product,
                           const std::string &endpoint) {
  if (product.empty() || endpoint.empty()) {
    return "center";
  }

  std::string popcode = Darabonba::String::toLower(product);
  std::string region = Darabonba::String::replace(endpoint, popcode, "");
  region = Darabonba::String::replace(region, "aliyuncs.com", "");
  region = Darabonba::String::replace(region, ".", "");
  if (!region.empty()) {
    return region;
  }
  return "center";
}

std::string
POP::buildCanonicalizedResource(const Darabonba::Http::Query &query) {
  std::string canonicalizedResource = "";
  for (const auto &p : query) {
    canonicalizedResource += Darabonba::Encode::Encoder::percentEncode(p.first);
    if (!p.second.empty()) {
      canonicalizedResource +=
          "=" + Darabonba::Encode::Encoder::percentEncode(p.second) + "&";
    }
  }
  canonicalizedResource.pop_back(); // remove '&'
  return canonicalizedResource;
}

std::string
POP::buildCanonicalizedHeaders(const Darabonba::Http::Header &headers) {
  std::string canonicalizedHeaders = "";
  auto sortedHeaders = getSignedHeaders(headers);
  for (const auto &key : sortedHeaders) {
    auto it = headers.find(key);
    if (it != headers.end()) {
      canonicalizedHeaders +=
          key + ':' + Darabonba::String::trim(it->second) + '\n';
    }
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