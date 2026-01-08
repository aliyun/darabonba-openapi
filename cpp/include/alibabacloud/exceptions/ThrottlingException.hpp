// This file is auto-generated, don't edit it. Thanks.
#ifndef ALIBABACLOUD_EXCEPTIONS_THROTTLINGEXCEPTION_HPP_
#define ALIBABACLOUD_EXCEPTIONS_THROTTLINGEXCEPTION_HPP_
#include <darabonba/Core.hpp>
using namespace std;
using json = nlohmann::json;
namespace AlibabaCloud
{
namespace OpenApi
{
namespace Exceptions
{
  class ThrottlingException : public AlibabaCloudException {
  public:
    friend void from_json(const Darabonba::Json& j, ThrottlingException& obj) { 
      DARABONBA_PTR_FROM_JSON(retryAfter, retryAfter_);
    };
    ThrottlingException() ;
    ThrottlingException(const ThrottlingException &) = default ;
    ThrottlingException(ThrottlingException &&) = default ;
    ThrottlingException(const Darabonba::Json & obj) : AlibabaCloudException(obj) { from_json(obj, *this); };
    virtual ~ThrottlingException() = default ;
    ThrottlingException& operator=(const ThrottlingException &) = default ;
    ThrottlingException& operator=(ThrottlingException &&) = default ;
    inline int64_t retryAfter() const { DARABONBA_PTR_GET_DEFAULT(retryAfter_, 0L) };
  protected:
    // Retry After(ms)
    shared_ptr<int64_t> retryAfter_ {};
  };
  
  } // namespace Exceptions
} // namespace AlibabaCloud
} // namespace OpenApi
#endif
