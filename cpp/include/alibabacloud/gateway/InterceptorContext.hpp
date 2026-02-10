#ifndef ALIBABACLOUD_GATEWAY_INTERCEPTORCONTEXT_H_
#define ALIBABACLOUD_GATEWAY_INTERCEPTORCONTEXT_H_

#include <darabonba/Type.hpp>
#include <alibabacloud/credentials/Client.hpp>
#include <darabonba/Model.hpp>
#include <darabonba/Stream.hpp>
#include <darabonba/http/Header.hpp>
#include <darabonba/http/MCurlResponse.hpp>
#include <darabonba/http/Query.hpp>

namespace AlibabaCloud {
  namespace Gateway {
    namespace Models {

      class InterceptorContextRequest : public Darabonba::Model {
        friend void to_json(Darabonba::Json &j, const InterceptorContextRequest &obj) {
          DARABONBA_PTR_TO_JSON(action, action_);
          DARABONBA_PTR_TO_JSON(authType, authType_);
          DARABONBA_PTR_TO_JSON(bodyType, bodyType_);
          DARABONBA_ANY_TO_JSON(body, body_);
          DARABONBA_PTR_TO_JSON(headers, headers_);
          DARABONBA_PTR_TO_JSON(hostMap, hostMap_);
          DARABONBA_PTR_TO_JSON(method, method_);
          DARABONBA_PTR_TO_JSON(pathname, pathname_);
          DARABONBA_PTR_TO_JSON(productId, productId_);
          DARABONBA_PTR_TO_JSON(protocol, protocol_);
          DARABONBA_PTR_TO_JSON(query, query_);
          DARABONBA_PTR_TO_JSON(reqBodyType, reqBodyType_);
          DARABONBA_PTR_TO_JSON(signatureAlgorithm, signatureAlgorithm_);
          DARABONBA_PTR_TO_JSON(signatureVersion, signatureVersion_);
          // DARABONBA_PTR_TO_JSON(stream, stream_);
          DARABONBA_PTR_TO_JSON(style, style_);
          DARABONBA_PTR_TO_JSON(userAgent, userAgent_);
          DARABONBA_PTR_TO_JSON(version, version_);
        }

        friend void from_json(const Darabonba::Json &j, InterceptorContextRequest &obj) {
          DARABONBA_PTR_FROM_JSON(action, action_);
          DARABONBA_PTR_FROM_JSON(authType, authType_);
          DARABONBA_PTR_FROM_JSON(bodyType, bodyType_);
          DARABONBA_ANY_FROM_JSON(body, body_);
          DARABONBA_PTR_FROM_JSON(headers, headers_);
          DARABONBA_PTR_FROM_JSON(hostMap, hostMap_);
          DARABONBA_PTR_FROM_JSON(method, method_);
          DARABONBA_PTR_FROM_JSON(pathname, pathname_);
          DARABONBA_PTR_FROM_JSON(productId, productId_);
          DARABONBA_PTR_FROM_JSON(protocol, protocol_);
          DARABONBA_PTR_FROM_JSON(query, query_);
          DARABONBA_PTR_FROM_JSON(reqBodyType, reqBodyType_);
          DARABONBA_PTR_FROM_JSON(signatureAlgorithm, signatureAlgorithm_);
          DARABONBA_PTR_FROM_JSON(signatureVersion, signatureVersion_);
          // DARABONBA_PTR_FROM_JSON(stream, stream_);
          DARABONBA_PTR_FROM_JSON(style, style_);
          DARABONBA_PTR_FROM_JSON(userAgent, userAgent_);
          DARABONBA_PTR_FROM_JSON(version, version_);
        }

      public:
        InterceptorContextRequest() = default;

        InterceptorContextRequest(const InterceptorContextRequest &) = default;

        InterceptorContextRequest(InterceptorContextRequest &&) = default;

        InterceptorContextRequest(const Darabonba::Json &obj) { from_json(obj, *this); }

        virtual ~InterceptorContextRequest() = default;

        InterceptorContextRequest &operator=(const InterceptorContextRequest &) = default;

        InterceptorContextRequest &operator=(InterceptorContextRequest &&) = default;

        virtual void validate() const override {}

        virtual void fromMap(const Darabonba::Json &obj) override {
          from_json(obj, *this);
          validate();
        }

        virtual Darabonba::Json toMap() const override {
          Darabonba::Json obj;
          to_json(obj, *this);
          return obj;
        }

        virtual bool empty() const override {
          return action_ == nullptr && authType_ == nullptr &&
                 bodyType_ == nullptr && body_ == nullptr &&
                 credential_ == nullptr && headers_ == nullptr &&
                 hostMap_ == nullptr && method_ == nullptr &&
                 pathname_ == nullptr && productId_ == nullptr &&
                 protocol_ == nullptr && query_ == nullptr &&
                 reqBodyType_ == nullptr && signatureAlgorithm_ == nullptr &&
                 signatureVersion_ == nullptr && stream_ == nullptr &&
                 style_ == nullptr && userAgent_ == nullptr && version_ == nullptr;
        }

        bool hasAction() const { return this->action_ != nullptr; }

        std::string getAction() const { DARABONBA_PTR_GET_DEFAULT(action_, ""); }

        InterceptorContextRequest &setAction(const std::string &action) {
          DARABONBA_PTR_SET_VALUE(action_, action);
        }

        InterceptorContextRequest &setAction(std::string &&action) {
          DARABONBA_PTR_SET_RVALUE(action_, action);
        }

        bool hasAuthType() const { return this->authType_ != nullptr; }

        std::string getAuthType() const { DARABONBA_PTR_GET_DEFAULT(authType_, ""); }

        InterceptorContextRequest &setAuthType(const std::string &authType) {
          DARABONBA_PTR_SET_VALUE(authType_, authType);
        }

        InterceptorContextRequest &setAuthType(std::string &&authType) {
          DARABONBA_PTR_SET_RVALUE(authType_, authType);
        }

        bool hasBodyType() const { return this->bodyType_ != nullptr; }

        std::string getBodyType() const { DARABONBA_PTR_GET_DEFAULT(bodyType_, ""); }

        InterceptorContextRequest &setBodyType(const std::string &bodyType) {
          DARABONBA_PTR_SET_VALUE(bodyType_, bodyType);
        }

        InterceptorContextRequest &setBodyType(std::string &&bodyType) {
          DARABONBA_PTR_SET_RVALUE(bodyType_, bodyType);
        }

        bool hasBody() const { return this->body_ != nullptr; }

        const Darabonba::Json &getBody() const { DARABONBA_GET(body_); }

        Darabonba::Json &getBody() { DARABONBA_GET(body_); }

        InterceptorContextRequest &setBody(const Darabonba::Json &body) {
          DARABONBA_SET_VALUE(body_, body);
        }

        InterceptorContextRequest &setBody(Darabonba::Json &&body) {
          DARABONBA_SET_RVALUE(body_, body);
        }

        bool hasCredential() const { return this->credential_ != nullptr; }

        const std::shared_ptr<Credentials::Client> &getCredential() const {
          DARABONBA_GET(credential_);
        }

        std::shared_ptr<Credentials::Client> &getCredential() { DARABONBA_GET(credential_); }

        InterceptorContextRequest &setCredential(const Credentials::Client &credential) {
          DARABONBA_PTR_SET_VALUE(credential_, credential);
        }

        InterceptorContextRequest &setCredential(Credentials::Client &&credential) {
          DARABONBA_PTR_SET_RVALUE(credential_, credential);
        }

        bool hasHeaders() const { return this->headers_ != nullptr; }

        const Darabonba::Http::Header &getHeaders() const {
          DARABONBA_PTR_GET_CONST(headers_, Darabonba::Http::Header);
        }

        Darabonba::Http::Header getHeaders() { DARABONBA_PTR_GET(headers_, Darabonba::Http::Header); }

        InterceptorContextRequest &setHeaders(const Darabonba::Http::Header &headers) {
          DARABONBA_PTR_SET_VALUE(headers_, headers);
        }

        InterceptorContextRequest &setHeaders(Darabonba::Http::Header &&headers) {
          DARABONBA_PTR_SET_RVALUE(headers_, headers);
        }

        bool hasHostMap() const { return this->hostMap_ != nullptr; }

        const std::map<std::string, std::string> &getHostMap() const {
          DARABONBA_PTR_GET_CONST(hostMap_, std::map<std::string, std::string>);
        }

        std::map<std::string, std::string> getHostMap() {
          DARABONBA_PTR_GET(hostMap_, std::map<std::string, std::string>);
        }

        InterceptorContextRequest &setHostMap(const std::map<std::string, std::string> &hostMap) {
          DARABONBA_PTR_SET_VALUE(hostMap_, hostMap);
        }

        InterceptorContextRequest &setHostMap(std::map<std::string, std::string> &&hostMap) {
          DARABONBA_PTR_SET_RVALUE(hostMap_, hostMap);
        }

        bool hasMethod() const { return this->method_ != nullptr; }

        std::string getMethod() const { DARABONBA_PTR_GET_DEFAULT(method_, ""); }

        InterceptorContextRequest &setMethod(const std::string &method) {
          DARABONBA_PTR_SET_VALUE(method_, method);
        }

        InterceptorContextRequest &setMethod(std::string &&method) {
          DARABONBA_PTR_SET_RVALUE(method_, method);
        }

        bool hasPathname() const { return this->pathname_ != nullptr; }

        std::string getPathname() const { DARABONBA_PTR_GET_DEFAULT(pathname_, ""); }

        InterceptorContextRequest &setPathname(const std::string &pathname) {
          DARABONBA_PTR_SET_VALUE(pathname_, pathname);
        }

        InterceptorContextRequest &setPathname(std::string &&pathname) {
          DARABONBA_PTR_SET_RVALUE(pathname_, pathname);
        }

        bool hasProductId() const { return this->productId_ != nullptr; }

        std::string getProductId() const { DARABONBA_PTR_GET_DEFAULT(productId_, ""); }

        InterceptorContextRequest &setProductId(const std::string &productId) {
          DARABONBA_PTR_SET_VALUE(productId_, productId);
        }

        InterceptorContextRequest &setProductId(std::string &&productId) {
          DARABONBA_PTR_SET_RVALUE(productId_, productId);
        }

        bool hasProtocol() const { return this->protocol_ != nullptr; }

        std::string getProtocol() const { DARABONBA_PTR_GET_DEFAULT(protocol_, ""); }

        InterceptorContextRequest &setProtocol(const std::string &protocol) {
          DARABONBA_PTR_SET_VALUE(protocol_, protocol);
        }

        InterceptorContextRequest &setProtocol(std::string &&protocol) {
          DARABONBA_PTR_SET_RVALUE(protocol_, protocol);
        }

        bool hasQuery() const { return this->query_ != nullptr; }

        const Darabonba::Http::Query &getQuery() const { DARABONBA_PTR_GET_CONST(query_, Darabonba::Http::Query); }

        Darabonba::Http::Query getQuery() { DARABONBA_PTR_GET(query_, Darabonba::Http::Query); }

        InterceptorContextRequest &setQuery(const Darabonba::Http::Query &query) {
          DARABONBA_PTR_SET_VALUE(query_, query);
        }

        InterceptorContextRequest &setQuery(Darabonba::Http::Query &&query) {
          DARABONBA_PTR_SET_RVALUE(query_, query);
        }

        bool hasReqBodyType() const { return this->reqBodyType_ != nullptr; }

        std::string getReqBodyType() const {
          DARABONBA_PTR_GET_DEFAULT(reqBodyType_, "");
        }

        InterceptorContextRequest &setReqBodyType(const std::string &reqBodyType) {
          DARABONBA_PTR_SET_VALUE(reqBodyType_, reqBodyType);
        }

        InterceptorContextRequest &setReqBodyType(std::string &&reqBodyType) {
          DARABONBA_PTR_SET_RVALUE(reqBodyType_, reqBodyType);
        }

        bool hasSignatureAlgorithm() const {
          return this->signatureAlgorithm_ != nullptr;
        }

        std::string getSignatureAlgorithm() const {
          DARABONBA_PTR_GET_DEFAULT(signatureAlgorithm_, "");
        }

        InterceptorContextRequest &setSignatureAlgorithm(const std::string &signatureAlgorithm) {
          DARABONBA_PTR_SET_VALUE(signatureAlgorithm_, signatureAlgorithm);
        }

        InterceptorContextRequest &setSignatureAlgorithm(std::string &&signatureAlgorithm) {
          DARABONBA_PTR_SET_RVALUE(signatureAlgorithm_, signatureAlgorithm);
        }

        bool hasSignatureVersion() const {
          return this->signatureVersion_ != nullptr;
        }

        std::string getSignatureVersion() const {
          DARABONBA_PTR_GET_DEFAULT(signatureVersion_, "");
        }

        InterceptorContextRequest &setSignatureVersion(const std::string &signatureVersion) {
          DARABONBA_PTR_SET_VALUE(signatureVersion_, signatureVersion);
        }

        InterceptorContextRequest &setSignatureVersion(std::string &&signatureVersion) {
          DARABONBA_PTR_SET_RVALUE(signatureVersion_, signatureVersion);
        }

        bool hasStream() const { return this->stream_ != nullptr; }

        std::shared_ptr<Darabonba::IStream> getStream() const {
          DARABONBA_GET(stream_);
        }

        InterceptorContextRequest &setStream(std::shared_ptr<Darabonba::IStream> stream) {
          DARABONBA_SET_VALUE(stream_, stream);
        }

        bool hasStyle() const { return this->style_ != nullptr; }

        std::string getStyle() const { DARABONBA_PTR_GET_DEFAULT(style_, ""); }

        InterceptorContextRequest &setStyle(const std::string &style) {
          DARABONBA_PTR_SET_VALUE(style_, style);
        }

        InterceptorContextRequest &setStyle(std::string &&style) {
          DARABONBA_PTR_SET_RVALUE(style_, style);
        }

        bool hasUserAgent() const { return this->userAgent_ != nullptr; }

        std::string getUserAgent() const { DARABONBA_PTR_GET_DEFAULT(userAgent_, ""); }

        InterceptorContextRequest &setUserAgent(const std::string &userAgent) {
          DARABONBA_PTR_SET_VALUE(userAgent_, userAgent);
        }

        InterceptorContextRequest &setUserAgent(std::string &&userAgent) {
          DARABONBA_PTR_SET_RVALUE(userAgent_, userAgent);
        }

        bool hasVersion() const { return this->version_ != nullptr; }

        std::string getVersion() const { DARABONBA_PTR_GET_DEFAULT(version_, ""); }

        InterceptorContextRequest &setVersion(const std::string &version) {
          DARABONBA_PTR_SET_VALUE(version_, version);
        }

        InterceptorContextRequest &setVersion(std::string &&version) {
          DARABONBA_PTR_SET_RVALUE(version_, version);
        }

      protected:
        std::shared_ptr<std::string> action_ = nullptr;
        std::shared_ptr<std::string> authType_ = nullptr;
        std::shared_ptr<std::string> bodyType_ = nullptr;
        Darabonba::Json body_ = nullptr;
        std::shared_ptr<Credentials::Client> credential_ = nullptr;
        std::shared_ptr<Darabonba::Http::Header> headers_ = nullptr;
        std::shared_ptr<std::map<std::string, std::string>> hostMap_ = nullptr;
        std::shared_ptr<std::string> method_ = nullptr;
        std::shared_ptr<std::string> pathname_ = nullptr;
        std::shared_ptr<std::string> productId_ = nullptr;
        std::shared_ptr<std::string> protocol_ = nullptr;
        std::shared_ptr<Darabonba::Http::Query> query_ = nullptr;
        std::shared_ptr<std::string> reqBodyType_ = nullptr;
        std::shared_ptr<std::string> signatureAlgorithm_ = nullptr;
        std::shared_ptr<std::string> signatureVersion_ = nullptr;
        std::shared_ptr<Darabonba::IStream> stream_ = nullptr;
        std::shared_ptr<std::string> style_ = nullptr;
        std::shared_ptr<std::string> userAgent_ = nullptr;
        std::shared_ptr<std::string> version_ = nullptr;
      };

      class InterceptorContextConfiguration : public Darabonba::Model {
        friend void to_json(Darabonba::Json &j, const InterceptorContextConfiguration &obj) {
          DARABONBA_PTR_TO_JSON(endpointMap, endpointMap_);
          DARABONBA_PTR_TO_JSON(endpointRule, endpointRule_);
          DARABONBA_PTR_TO_JSON(endpointType, endpointType_);
          DARABONBA_PTR_TO_JSON(endpoint, endpoint_);
          DARABONBA_PTR_TO_JSON(network, network_);
          DARABONBA_PTR_TO_JSON(regionId, regionId_);
          DARABONBA_PTR_TO_JSON(suffix, suffix_);
        }

        friend void from_json(const Darabonba::Json &j, InterceptorContextConfiguration &obj) {
          DARABONBA_PTR_FROM_JSON(endpointMap, endpointMap_);
          DARABONBA_PTR_FROM_JSON(endpointRule, endpointRule_);
          DARABONBA_PTR_FROM_JSON(endpointType, endpointType_);
          DARABONBA_PTR_FROM_JSON(endpoint, endpoint_);
          DARABONBA_PTR_FROM_JSON(network, network_);
          DARABONBA_PTR_FROM_JSON(regionId, regionId_);
          DARABONBA_PTR_FROM_JSON(suffix, suffix_);
        }

      public:
        InterceptorContextConfiguration() = default;

        InterceptorContextConfiguration(const InterceptorContextConfiguration &) = default;

        InterceptorContextConfiguration(InterceptorContextConfiguration &&) = default;

        InterceptorContextConfiguration(const Darabonba::Json &obj) { from_json(obj, *this); }

        InterceptorContextConfiguration &operator=(const InterceptorContextConfiguration &) = default;

        InterceptorContextConfiguration &operator=(InterceptorContextConfiguration &&) = default;

        virtual ~InterceptorContextConfiguration() = default;

        virtual void validate() const override {}

        virtual void fromMap(const Darabonba::Json &obj) override {
          from_json(obj, *this);
          validate();
        }

        virtual Darabonba::Json toMap() const override {
          Darabonba::Json obj;
          to_json(obj, *this);
          return obj;
        }

        virtual bool empty() const override {
          return endpointMap_ == nullptr && endpointRule_ == nullptr &&
                 endpointType_ == nullptr && endpoint_ == nullptr &&
                 network_ == nullptr && regionId_ == nullptr && suffix_ == nullptr;
        }

        bool hasEndpointMap() const { return this->endpointMap_ != nullptr; }

        const std::map<std::string, std::string> &getEndpointMap() const {
          DARABONBA_PTR_GET_CONST(endpointMap_, std::map<std::string, std::string>);
        }

        std::map<std::string, std::string> getEndpointMap() {
          DARABONBA_PTR_GET(endpointMap_, std::map<std::string, std::string>);
        }

        InterceptorContextConfiguration &
        setEndpointMap(const std::map<std::string, std::string> &endpointMap) {
          DARABONBA_PTR_SET_VALUE(endpointMap_, endpointMap);
        }

        InterceptorContextConfiguration &
        setEndpointMap(std::map<std::string, std::string> &&endpointMap) {
          DARABONBA_PTR_SET_RVALUE(endpointMap_, endpointMap);
        }

        bool hasEndpointRule() const { return this->endpointRule_ != nullptr; }

        std::string getEndpointRule() const {
          DARABONBA_PTR_GET_DEFAULT(endpointRule_, "");
        }

        InterceptorContextConfiguration &setEndpointRule(const std::string &endpointRule) {
          DARABONBA_PTR_SET_VALUE(endpointRule_, endpointRule);
        }

        InterceptorContextConfiguration &setEndpointRule(std::string &&endpointRule) {
          DARABONBA_PTR_SET_RVALUE(endpointRule_, endpointRule);
        }

        bool hasEndpointType() const { return this->endpointType_ != nullptr; }

        std::string getEndpointType() const {
          DARABONBA_PTR_GET_DEFAULT(endpointType_, "");
        }

        InterceptorContextConfiguration &setEndpointType(const std::string &endpointType) {
          DARABONBA_PTR_SET_VALUE(endpointType_, endpointType);
        }

        InterceptorContextConfiguration &setEndpointType(std::string &&endpointType) {
          DARABONBA_PTR_SET_RVALUE(endpointType_, endpointType);
        }

        bool hasEndpoint() const { return this->endpoint_ != nullptr; }

        std::string getEndpoint() const { DARABONBA_PTR_GET_DEFAULT(endpoint_, ""); }

        InterceptorContextConfiguration &setEndpoint(const std::string &endpoint) {
          DARABONBA_PTR_SET_VALUE(endpoint_, endpoint);
        }

        InterceptorContextConfiguration &setEndpoint(std::string &&endpoint) {
          DARABONBA_PTR_SET_RVALUE(endpoint_, endpoint);
        }

        bool hasNetwork() const { return this->network_ != nullptr; }

        std::string getNetwork() const { DARABONBA_PTR_GET_DEFAULT(network_, ""); }

        InterceptorContextConfiguration &setNetwork(const std::string &network) {
          DARABONBA_PTR_SET_VALUE(network_, network);
        }

        InterceptorContextConfiguration &setNetwork(std::string &&network) {
          DARABONBA_PTR_SET_RVALUE(network_, network);
        }

        bool hasRegionId() const { return this->regionId_ != nullptr; }

        std::string getRegionId() const { DARABONBA_PTR_GET_DEFAULT(regionId_, ""); }

        InterceptorContextConfiguration &setRegionId(const std::string &regionId) {
          DARABONBA_PTR_SET_VALUE(regionId_, regionId);
        }

        InterceptorContextConfiguration &setRegionId(std::string &&regionId) {
          DARABONBA_PTR_SET_RVALUE(regionId_, regionId);
        }

        bool hasSuffix() const { return this->suffix_ != nullptr; }

        std::string getSuffix() const { DARABONBA_PTR_GET_DEFAULT(suffix_, ""); }

        InterceptorContextConfiguration &setSuffix(const std::string &suffix) {
          DARABONBA_PTR_SET_VALUE(suffix_, suffix);
        }

        InterceptorContextConfiguration &setSuffix(std::string &&suffix) {
          DARABONBA_PTR_SET_RVALUE(suffix_, suffix);
        }

      protected:
        std::shared_ptr<std::map<std::string, std::string>> endpointMap_ = nullptr;
        std::shared_ptr<std::string> endpointRule_ = nullptr;
        std::shared_ptr<std::string> endpointType_ = nullptr;
        std::shared_ptr<std::string> endpoint_ = nullptr;
        std::shared_ptr<std::string> network_ = nullptr;
        std::shared_ptr<std::string> regionId_ = nullptr;
        std::shared_ptr<std::string> suffix_ = nullptr;
      };

      class InterceptorContextResponse : public Darabonba::Model {
        friend void to_json(Darabonba::Json &j, const InterceptorContextResponse &obj) {
          DARABONBA_ANY_TO_JSON(deserializedBody, deserializedBody_);
          DARABONBA_TO_JSON(body, body_);
          DARABONBA_PTR_TO_JSON(headers, headers_);
          DARABONBA_PTR_TO_JSON(statusCode, statusCode_);
        }

        friend void from_json(const Darabonba::Json &j, InterceptorContextResponse &obj) {
          DARABONBA_ANY_FROM_JSON(deserializedBody, deserializedBody_);
          DARABONBA_FROM_JSON(body, body_);
          DARABONBA_PTR_FROM_JSON(headers, headers_);
          DARABONBA_PTR_FROM_JSON(statusCode, statusCode_);
        }

      public:
        InterceptorContextResponse() = default;

        InterceptorContextResponse(const InterceptorContextResponse &) = default;

        InterceptorContextResponse(InterceptorContextResponse &&) = default;

        InterceptorContextResponse(const Darabonba::Json &obj) { from_json(obj, *this); }

        virtual ~InterceptorContextResponse() = default;

        InterceptorContextResponse &operator=(const InterceptorContextResponse &) = default;

        InterceptorContextResponse &operator=(InterceptorContextResponse &&) = default;

        virtual void validate() const override {}

        virtual void fromMap(const Darabonba::Json &obj) override {
          from_json(obj, *this);
          validate();
        }

        virtual Darabonba::Json toMap() const override {
          Darabonba::Json obj;
          to_json(obj, *this);
          return obj;
        }

        virtual bool empty() const override {
          return body_ == nullptr && deserializedBody_ == nullptr &&
                 headers_ == nullptr && statusCode_ == nullptr;
        }

        bool hasBody() const { return this->body_ != nullptr; }

        std::shared_ptr<Darabonba::IStream> getBody() const {
          DARABONBA_GET(body_);
        }

        InterceptorContextResponse &
        setBody(std::shared_ptr<Darabonba::IStream> body) {
          DARABONBA_SET_VALUE(body_, body);
        }

        bool hasDeserializedBody() const {
          return this->deserializedBody_ != nullptr;
        }

        const Darabonba::Json &getDeserializedBody() const {
          DARABONBA_GET(deserializedBody_);
        }
        Darabonba::Json & getBody() { DARABONBA_GET(deserializedBody_) };

        InterceptorContextResponse &setDeserializedBody(const Darabonba::Json &deserializedBody) {
          DARABONBA_SET_VALUE(deserializedBody_, deserializedBody);
        }

        InterceptorContextResponse &setDeserializedBody(Darabonba::Json &&deserializedBody) {
          DARABONBA_SET_VALUE(deserializedBody_, deserializedBody);
        }

        bool hasHeaders() const { return this->headers_ != nullptr; }

        const Darabonba::Http::Header &headers() const {
          DARABONBA_PTR_GET_CONST(headers_, Darabonba::Http::Header);
        }

        Darabonba::Http::Header getHeaders() { DARABONBA_PTR_GET(headers_, Darabonba::Http::Header); }

        InterceptorContextResponse &setHeaders(const Darabonba::Http::Header &headers) {
          DARABONBA_PTR_SET_VALUE(headers_, headers);
        }

        InterceptorContextResponse &setHeaders(Darabonba::Http::Header &&headers) {
          DARABONBA_PTR_SET_RVALUE(headers_, headers);
        }

        bool hasStatusCode() const { return this->statusCode_ != nullptr; }

        int64_t getStatusCode() const { DARABONBA_PTR_GET_DEFAULT(statusCode_, 0); }

        InterceptorContextResponse &setStatusCode(int64_t statusCode) {
          DARABONBA_PTR_SET_VALUE(statusCode_, statusCode);
        }

      protected:
        std::shared_ptr<Darabonba::IStream> body_ = nullptr;
        Darabonba::Json deserializedBody_ = nullptr;
        std::shared_ptr<Darabonba::Http::Header> headers_ = nullptr;
        std::shared_ptr<int64_t> statusCode_ = nullptr;
      };

      class InterceptorContext : public Darabonba::Model {
      public:

        friend void to_json(Darabonba::Json &j, const InterceptorContext &obj) {
          DARABONBA_PTR_TO_JSON(configuration, configuration_);
          DARABONBA_PTR_TO_JSON(request, request_);
          DARABONBA_PTR_TO_JSON(response, response_);
        }

        friend void from_json(const Darabonba::Json &j, InterceptorContext &obj) {
          DARABONBA_PTR_FROM_JSON(configuration, configuration_);
          DARABONBA_PTR_FROM_JSON(request, request_);
          DARABONBA_PTR_FROM_JSON(response, response_);
        }

      public:
        InterceptorContext() = default;

        InterceptorContext(const InterceptorContext &) = default;

        InterceptorContext(InterceptorContext &&) = default;

        InterceptorContext(const Darabonba::Json &obj) { from_json(obj, *this); }

        virtual ~InterceptorContext() = default;

        virtual void validate() const override {}

        virtual void fromMap(const Darabonba::Json &obj) override {
          from_json(obj, *this);
          validate();
        }

        virtual Darabonba::Json toMap() const override {
          Darabonba::Json obj;
          to_json(obj, *this);
          return obj;
        }

        virtual bool empty() const override {
          return configuration_ == nullptr && request_ == nullptr &&
                 response_ == nullptr;
        }

        bool hasInterceptorContextConfiguration() const { return this->configuration_ != nullptr; }

        const InterceptorContextConfiguration &getConfiguration() const {
          DARABONBA_PTR_GET_CONST(configuration_, InterceptorContextConfiguration);
        }

        InterceptorContextConfiguration getConfiguration() { DARABONBA_PTR_GET(configuration_, InterceptorContextConfiguration); }

        InterceptorContext &setInterceptorContextConfiguration(const InterceptorContextConfiguration &configuration) {
          DARABONBA_PTR_SET_VALUE(configuration_, configuration);
        }

        InterceptorContext &setInterceptorContextConfiguration(InterceptorContextConfiguration &&configuration) {
          DARABONBA_PTR_SET_RVALUE(configuration_, configuration);
        }

        bool hasInterceptorContextRequest() const { return this->request_ != nullptr; }

        const InterceptorContextRequest &getRequest() const { DARABONBA_PTR_GET_CONST(request_, InterceptorContextRequest); }

        InterceptorContextRequest getRequest() { DARABONBA_PTR_GET(request_, InterceptorContextRequest); }

        InterceptorContext &setRequest(const InterceptorContextRequest &request) {
          DARABONBA_PTR_SET_VALUE(request_, request);
        }

        InterceptorContext &setRequest(InterceptorContextRequest &&request) {
          DARABONBA_PTR_SET_RVALUE(request_, request);
        }

        bool hasResponse() const { return this->response_ != nullptr; }

        const InterceptorContextResponse &getResponse() const { DARABONBA_PTR_GET_CONST(response_, InterceptorContextResponse); }

        InterceptorContextResponse getResponse() { DARABONBA_PTR_GET(response_, InterceptorContextResponse); }

        InterceptorContext &setResponse(const InterceptorContextResponse &response) {
          DARABONBA_PTR_SET_VALUE(response_, response);
        }

        InterceptorContext &setResponse(InterceptorContextResponse &&response) {
          DARABONBA_PTR_SET_RVALUE(response_, response);
        }

      protected:
        std::shared_ptr<InterceptorContextConfiguration> configuration_ = nullptr;
        std::shared_ptr<InterceptorContextRequest> request_ = nullptr;
        std::shared_ptr<InterceptorContextResponse> response_ = nullptr;
      };
    }
  } // namespace Gateway
} // namespace AlibabaCloud

#endif