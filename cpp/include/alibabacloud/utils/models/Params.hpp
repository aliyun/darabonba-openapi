// This file is auto-generated, don't edit it. Thanks.
#ifndef ALIBABACLOUD_UTILS_MODELS_PARAMS_HPP_
#define ALIBABACLOUD_UTILS_MODELS_PARAMS_HPP_
#include <darabonba/Core.hpp>
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
  class Params : public Darabonba::Model {
  public:
    friend void to_json(Darabonba::Json& j, const Params& obj) { 
      DARABONBA_PTR_TO_JSON(action, action_);
      DARABONBA_PTR_TO_JSON(version, version_);
      DARABONBA_PTR_TO_JSON(protocol, protocol_);
      DARABONBA_PTR_TO_JSON(pathname, pathname_);
      DARABONBA_PTR_TO_JSON(method, method_);
      DARABONBA_PTR_TO_JSON(authType, authType_);
      DARABONBA_PTR_TO_JSON(bodyType, bodyType_);
      DARABONBA_PTR_TO_JSON(reqBodyType, reqBodyType_);
      DARABONBA_PTR_TO_JSON(style, style_);
    };
    friend void from_json(const Darabonba::Json& j, Params& obj) { 
      DARABONBA_PTR_FROM_JSON(action, action_);
      DARABONBA_PTR_FROM_JSON(version, version_);
      DARABONBA_PTR_FROM_JSON(protocol, protocol_);
      DARABONBA_PTR_FROM_JSON(pathname, pathname_);
      DARABONBA_PTR_FROM_JSON(method, method_);
      DARABONBA_PTR_FROM_JSON(authType, authType_);
      DARABONBA_PTR_FROM_JSON(bodyType, bodyType_);
      DARABONBA_PTR_FROM_JSON(reqBodyType, reqBodyType_);
      DARABONBA_PTR_FROM_JSON(style, style_);
    };
    Params() = default ;
    Params(const Params &) = default ;
    Params(Params &&) = default ;
    Params(const Darabonba::Json & obj) { from_json(obj, *this); };
    virtual ~Params() = default ;
    Params& operator=(const Params &) = default ;
    Params& operator=(Params &&) = default ;
    virtual void validate() const override {
        DARABONBA_VALIDATE_REQUIRED(action_);
        DARABONBA_VALIDATE_REQUIRED(version_);
        DARABONBA_VALIDATE_REQUIRED(protocol_);
        DARABONBA_VALIDATE_REQUIRED(pathname_);
        DARABONBA_VALIDATE_REQUIRED(method_);
        DARABONBA_VALIDATE_REQUIRED(authType_);
        DARABONBA_VALIDATE_REQUIRED(bodyType_);
        DARABONBA_VALIDATE_REQUIRED(reqBodyType_);
    };
    virtual void fromMap(const Darabonba::Json &obj) override { from_json(obj, *this); validate(); };
    virtual Darabonba::Json toMap() const override { Darabonba::Json obj; to_json(obj, *this); return obj; };
    virtual bool empty() const override { return this->action_ == nullptr
        && this->version_ == nullptr && this->protocol_ == nullptr && this->pathname_ == nullptr && this->method_ == nullptr && this->authType_ == nullptr
        && this->bodyType_ == nullptr && this->reqBodyType_ == nullptr && this->style_ == nullptr; };
    // action Field Functions 
    bool hasAction() const { return this->action_ != nullptr;};
    void deleteAction() { this->action_ = nullptr;};
    inline string getAction() const { DARABONBA_PTR_GET_DEFAULT(action_, "") };
    inline Params& setAction(string action) { DARABONBA_PTR_SET_VALUE(action_, action) };


    // version Field Functions 
    bool hasVersion() const { return this->version_ != nullptr;};
    void deleteVersion() { this->version_ = nullptr;};
    inline string getVersion() const { DARABONBA_PTR_GET_DEFAULT(version_, "") };
    inline Params& setVersion(string version) { DARABONBA_PTR_SET_VALUE(version_, version) };


    // protocol Field Functions 
    bool hasProtocol() const { return this->protocol_ != nullptr;};
    void deleteProtocol() { this->protocol_ = nullptr;};
    inline string getProtocol() const { DARABONBA_PTR_GET_DEFAULT(protocol_, "") };
    inline Params& setProtocol(string protocol) { DARABONBA_PTR_SET_VALUE(protocol_, protocol) };


    // pathname Field Functions 
    bool hasPathname() const { return this->pathname_ != nullptr;};
    void deletePathname() { this->pathname_ = nullptr;};
    inline string getPathname() const { DARABONBA_PTR_GET_DEFAULT(pathname_, "") };
    inline Params& setPathname(string pathname) { DARABONBA_PTR_SET_VALUE(pathname_, pathname) };


    // method Field Functions 
    bool hasMethod() const { return this->method_ != nullptr;};
    void deleteMethod() { this->method_ = nullptr;};
    inline string getMethod() const { DARABONBA_PTR_GET_DEFAULT(method_, "") };
    inline Params& setMethod(string method) { DARABONBA_PTR_SET_VALUE(method_, method) };


    // authType Field Functions 
    bool hasAuthType() const { return this->authType_ != nullptr;};
    void deleteAuthType() { this->authType_ = nullptr;};
    inline string getAuthType() const { DARABONBA_PTR_GET_DEFAULT(authType_, "") };
    inline Params& setAuthType(string authType) { DARABONBA_PTR_SET_VALUE(authType_, authType) };


    // bodyType Field Functions 
    bool hasBodyType() const { return this->bodyType_ != nullptr;};
    void deleteBodyType() { this->bodyType_ = nullptr;};
    inline string getBodyType() const { DARABONBA_PTR_GET_DEFAULT(bodyType_, "") };
    inline Params& setBodyType(string bodyType) { DARABONBA_PTR_SET_VALUE(bodyType_, bodyType) };


    // reqBodyType Field Functions 
    bool hasReqBodyType() const { return this->reqBodyType_ != nullptr;};
    void deleteReqBodyType() { this->reqBodyType_ = nullptr;};
    inline string getReqBodyType() const { DARABONBA_PTR_GET_DEFAULT(reqBodyType_, "") };
    inline Params& setReqBodyType(string reqBodyType) { DARABONBA_PTR_SET_VALUE(reqBodyType_, reqBodyType) };


    // style Field Functions 
    bool hasStyle() const { return this->style_ != nullptr;};
    void deleteStyle() { this->style_ = nullptr;};
    inline string getStyle() const { DARABONBA_PTR_GET_DEFAULT(style_, "") };
    inline Params& setStyle(string style) { DARABONBA_PTR_SET_VALUE(style_, style) };


  protected:
    shared_ptr<string> action_ {};
    shared_ptr<string> version_ {};
    shared_ptr<string> protocol_ {};
    shared_ptr<string> pathname_ {};
    shared_ptr<string> method_ {};
    shared_ptr<string> authType_ {};
    shared_ptr<string> bodyType_ {};
    shared_ptr<string> reqBodyType_ {};
    shared_ptr<string> style_ {};
  };

  } // namespace Models
} // namespace AlibabaCloud
} // namespace OpenApi
} // namespace Utils
#endif
