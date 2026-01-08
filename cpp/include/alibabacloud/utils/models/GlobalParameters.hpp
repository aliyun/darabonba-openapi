// This file is auto-generated, don't edit it. Thanks.
#ifndef ALIBABACLOUD_UTILS_MODELS_GLOBALPARAMETERS_HPP_
#define ALIBABACLOUD_UTILS_MODELS_GLOBALPARAMETERS_HPP_
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
  class GlobalParameters : public Darabonba::Model {
  public:
    friend void to_json(Darabonba::Json& j, const GlobalParameters& obj) { 
      DARABONBA_PTR_TO_JSON(headers, headers_);
      DARABONBA_PTR_TO_JSON(queries, queries_);
    };
    friend void from_json(const Darabonba::Json& j, GlobalParameters& obj) { 
      DARABONBA_PTR_FROM_JSON(headers, headers_);
      DARABONBA_PTR_FROM_JSON(queries, queries_);
    };
    GlobalParameters() = default ;
    GlobalParameters(const GlobalParameters &) = default ;
    GlobalParameters(GlobalParameters &&) = default ;
    GlobalParameters(const Darabonba::Json & obj) { from_json(obj, *this); };
    virtual ~GlobalParameters() = default ;
    GlobalParameters& operator=(const GlobalParameters &) = default ;
    GlobalParameters& operator=(GlobalParameters &&) = default ;
    virtual void validate() const override {
    };
    virtual void fromMap(const Darabonba::Json &obj) override { from_json(obj, *this); validate(); };
    virtual Darabonba::Json toMap() const override { Darabonba::Json obj; to_json(obj, *this); return obj; };
    virtual bool empty() const override { return this->headers_ == nullptr
        && this->queries_ == nullptr; };
    // headers Field Functions 
    bool hasHeaders() const { return this->headers_ != nullptr;};
    void deleteHeaders() { this->headers_ = nullptr;};
    inline const map<string, string> & getHeaders() const { DARABONBA_PTR_GET_CONST(headers_, map<string, string>) };
    inline map<string, string> getHeaders() { DARABONBA_PTR_GET(headers_, map<string, string>) };
    inline GlobalParameters& setHeaders(const map<string, string> & headers) { DARABONBA_PTR_SET_VALUE(headers_, headers) };
    inline GlobalParameters& setHeaders(map<string, string> && headers) { DARABONBA_PTR_SET_RVALUE(headers_, headers) };


    // queries Field Functions 
    bool hasQueries() const { return this->queries_ != nullptr;};
    void deleteQueries() { this->queries_ = nullptr;};
    inline const map<string, string> & getQueries() const { DARABONBA_PTR_GET_CONST(queries_, map<string, string>) };
    inline map<string, string> getQueries() { DARABONBA_PTR_GET(queries_, map<string, string>) };
    inline GlobalParameters& setQueries(const map<string, string> & queries) { DARABONBA_PTR_SET_VALUE(queries_, queries) };
    inline GlobalParameters& setQueries(map<string, string> && queries) { DARABONBA_PTR_SET_RVALUE(queries_, queries) };


  protected:
    shared_ptr<map<string, string>> headers_ {};
    shared_ptr<map<string, string>> queries_ {};
  };

  } // namespace Models
} // namespace AlibabaCloud
} // namespace OpenApi
} // namespace Utils
#endif
