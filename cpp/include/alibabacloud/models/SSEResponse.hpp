// This file is auto-generated, don't edit it. Thanks.
#ifndef ALIBABACLOUD_MODELS_SSERESPONSE_HPP_
#define ALIBABACLOUD_MODELS_SSERESPONSE_HPP_
#include <darabonba/Core.hpp>
#include <map>
#include <darabonba/http/SSEEvent.hpp>
using namespace std;
using json = nlohmann::json;
namespace AlibabaCloud
{
namespace OpenApi
{
namespace Models
{
  class SSEResponse : public Darabonba::Model {
  public:
    friend void to_json(Darabonba::Json& j, const SSEResponse& obj) { 
      DARABONBA_PTR_TO_JSON(headers, headers_);
      DARABONBA_PTR_TO_JSON(statusCode, statusCode_);
      DARABONBA_PTR_TO_JSON(event, event_);
    };
    friend void from_json(const Darabonba::Json& j, SSEResponse& obj) { 
      DARABONBA_PTR_FROM_JSON(headers, headers_);
      DARABONBA_PTR_FROM_JSON(statusCode, statusCode_);
      DARABONBA_PTR_FROM_JSON(event, event_);
    };
    SSEResponse() = default ;
    SSEResponse(const SSEResponse &) = default ;
    SSEResponse(SSEResponse &&) = default ;
    SSEResponse(const Darabonba::Json & obj) { from_json(obj, *this); };
    virtual ~SSEResponse() = default ;
    SSEResponse& operator=(const SSEResponse &) = default ;
    SSEResponse& operator=(SSEResponse &&) = default ;
    virtual void validate() const override {
        DARABONBA_VALIDATE_REQUIRED(headers_);
        DARABONBA_VALIDATE_REQUIRED(statusCode_);
        DARABONBA_VALIDATE_REQUIRED(event_);
    };
    virtual void fromMap(const Darabonba::Json &obj) override { from_json(obj, *this); validate(); };
    virtual Darabonba::Json toMap() const override { Darabonba::Json obj; to_json(obj, *this); return obj; };
    virtual bool empty() const override { return this->headers_ == nullptr
        && this->statusCode_ == nullptr && this->event_ == nullptr; };
    // headers Field Functions 
    bool hasHeaders() const { return this->headers_ != nullptr;};
    void deleteHeaders() { this->headers_ = nullptr;};
    inline const map<string, string> & getHeaders() const { DARABONBA_PTR_GET_CONST(headers_, map<string, string>) };
    inline map<string, string> getHeaders() { DARABONBA_PTR_GET(headers_, map<string, string>) };
    inline SSEResponse& setHeaders(const map<string, string> & headers) { DARABONBA_PTR_SET_VALUE(headers_, headers) };
    inline SSEResponse& setHeaders(map<string, string> && headers) { DARABONBA_PTR_SET_RVALUE(headers_, headers) };


    // statusCode Field Functions 
    bool hasStatusCode() const { return this->statusCode_ != nullptr;};
    void deleteStatusCode() { this->statusCode_ = nullptr;};
    inline int64_t getStatusCode() const { DARABONBA_PTR_GET_DEFAULT(statusCode_, 0) };
    inline SSEResponse& setStatusCode(int64_t statusCode) { DARABONBA_PTR_SET_VALUE(statusCode_, statusCode) };


    // event Field Functions 
    bool hasEvent() const { return this->event_ != nullptr;};
    void deleteEvent() { this->event_ = nullptr;};
    inline const Darabonba::Http::SSEEvent & getEvent() const { DARABONBA_PTR_GET_CONST(event_, Darabonba::Http::SSEEvent) };
    inline Darabonba::Http::SSEEvent getEvent() { DARABONBA_PTR_GET(event_, Darabonba::Http::SSEEvent) };
    inline SSEResponse& setEvent(const Darabonba::Http::SSEEvent & event) { DARABONBA_PTR_SET_VALUE(event_, event) };
    inline SSEResponse& setEvent(Darabonba::Http::SSEEvent && event) { DARABONBA_PTR_SET_RVALUE(event_, event) };


  protected:
    shared_ptr<map<string, string>> headers_ {};
    // HTTP Status Code
    shared_ptr<int64_t> statusCode_ {};
    shared_ptr<Darabonba::Http::SSEEvent> event_ {};
  };

  } // namespace Models
} // namespace AlibabaCloud
} // namespace OpenApi
#endif
