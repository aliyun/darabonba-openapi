// This file is auto-generated, don't edit it. Thanks.
#ifndef ALIBABACLOUD_UTILS_MODELS_OPENAPIREQUEST_HPP_
#define ALIBABACLOUD_UTILS_MODELS_OPENAPIREQUEST_HPP_
#include <darabonba/Core.hpp>
#include <map>
using namespace std;
using json = nlohmann::json;
namespace AlibabaCloud
{
namespace OpenApi
{
namespace Utils
{
namespace Models
{
  class OpenApiRequest : public Darabonba::Model {
  public:
    friend void to_json(Darabonba::Json& j, const OpenApiRequest& obj) { 
      DARABONBA_PTR_TO_JSON(headers, headers_);
      DARABONBA_PTR_TO_JSON(query, query_);
      DARABONBA_ANY_TO_JSON(body, body_);
      // stream_ is stream
      DARABONBA_PTR_TO_JSON(hostMap, hostMap_);
      DARABONBA_PTR_TO_JSON(endpointOverride, endpointOverride_);
    };
    friend void from_json(const Darabonba::Json& j, OpenApiRequest& obj) { 
      DARABONBA_PTR_FROM_JSON(headers, headers_);
      DARABONBA_PTR_FROM_JSON(query, query_);
      DARABONBA_ANY_FROM_JSON(body, body_);
      // stream_ is stream
      DARABONBA_PTR_FROM_JSON(hostMap, hostMap_);
      DARABONBA_PTR_FROM_JSON(endpointOverride, endpointOverride_);
    };
    OpenApiRequest() = default ;
    OpenApiRequest(const OpenApiRequest &) = default ;
    OpenApiRequest(OpenApiRequest &&) = default ;
    OpenApiRequest(const Darabonba::Json & obj) { from_json(obj, *this); };
    virtual ~OpenApiRequest() = default ;
    OpenApiRequest& operator=(const OpenApiRequest &) = default ;
    OpenApiRequest& operator=(OpenApiRequest &&) = default ;
    virtual void validate() const override {
    };
    virtual void fromMap(const Darabonba::Json &obj) override { from_json(obj, *this); validate(); };
    virtual Darabonba::Json toMap() const override { Darabonba::Json obj; to_json(obj, *this); return obj; };
    virtual bool empty() const override { return this->headers_ == nullptr
        && this->query_ == nullptr && this->body_ == nullptr && this->stream_ == nullptr && this->hostMap_ == nullptr && this->endpointOverride_ == nullptr; };
    // headers Field Functions 
    bool hasHeaders() const { return this->headers_ != nullptr;};
    void deleteHeaders() { this->headers_ = nullptr;};
    inline const map<string, string> & getHeaders() const { DARABONBA_PTR_GET_CONST(headers_, map<string, string>) };
    inline map<string, string> getHeaders() { DARABONBA_PTR_GET(headers_, map<string, string>) };
    inline OpenApiRequest& setHeaders(const map<string, string> & headers) { DARABONBA_PTR_SET_VALUE(headers_, headers) };
    inline OpenApiRequest& setHeaders(map<string, string> && headers) { DARABONBA_PTR_SET_RVALUE(headers_, headers) };


    // query Field Functions 
    bool hasQuery() const { return this->query_ != nullptr;};
    void deleteQuery() { this->query_ = nullptr;};
    inline const map<string, string> & getQuery() const { DARABONBA_PTR_GET_CONST(query_, map<string, string>) };
    inline map<string, string> getQuery() { DARABONBA_PTR_GET(query_, map<string, string>) };
    inline OpenApiRequest& setQuery(const map<string, string> & query) { DARABONBA_PTR_SET_VALUE(query_, query) };
    inline OpenApiRequest& setQuery(map<string, string> && query) { DARABONBA_PTR_SET_RVALUE(query_, query) };


    // body Field Functions 
    bool hasBody() const { return this->body_ != nullptr;};
    void deleteBody() { this->body_ = nullptr;};
    inline     const Darabonba::Json & getBody() const { DARABONBA_GET(body_) };
    Darabonba::Json & getBody() { DARABONBA_GET(body_) };
    inline OpenApiRequest& setBody(const Darabonba::Json & body) { DARABONBA_SET_VALUE(body_, body) };
    inline OpenApiRequest& setBody(Darabonba::Json && body) { DARABONBA_SET_RVALUE(body_, body) };


    // stream Field Functions 
    bool hasStream() const { return this->stream_ != nullptr;};
    void deleteStream() { this->stream_ = nullptr;};
    inline shared_ptr<Darabonba::IStream> getStream() const { DARABONBA_GET(stream_) };
    inline OpenApiRequest& setStream(shared_ptr<Darabonba::IStream> stream) { DARABONBA_SET_VALUE(stream_, stream) };


    // hostMap Field Functions 
    bool hasHostMap() const { return this->hostMap_ != nullptr;};
    void deleteHostMap() { this->hostMap_ = nullptr;};
    inline const map<string, string> & getHostMap() const { DARABONBA_PTR_GET_CONST(hostMap_, map<string, string>) };
    inline map<string, string> getHostMap() { DARABONBA_PTR_GET(hostMap_, map<string, string>) };
    inline OpenApiRequest& setHostMap(const map<string, string> & hostMap) { DARABONBA_PTR_SET_VALUE(hostMap_, hostMap) };
    inline OpenApiRequest& setHostMap(map<string, string> && hostMap) { DARABONBA_PTR_SET_RVALUE(hostMap_, hostMap) };


    // endpointOverride Field Functions 
    bool hasEndpointOverride() const { return this->endpointOverride_ != nullptr;};
    void deleteEndpointOverride() { this->endpointOverride_ = nullptr;};
    inline string getEndpointOverride() const { DARABONBA_PTR_GET_DEFAULT(endpointOverride_, "") };
    inline OpenApiRequest& setEndpointOverride(string endpointOverride) { DARABONBA_PTR_SET_VALUE(endpointOverride_, endpointOverride) };


  protected:
    shared_ptr<map<string, string>> headers_ {};
    shared_ptr<map<string, string>> query_ {};
    Darabonba::Json body_ {};
    shared_ptr<Darabonba::IStream> stream_ {};
    shared_ptr<map<string, string>> hostMap_ {};
    shared_ptr<string> endpointOverride_ {};
  };

  } // namespace Models
} // namespace AlibabaCloud
} // namespace OpenApi
} // namespace Utils
#endif
