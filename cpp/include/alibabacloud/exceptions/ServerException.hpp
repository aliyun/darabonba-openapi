// This file is auto-generated, don't edit it. Thanks.
#ifndef ALIBABACLOUD_EXCEPTIONS_SERVEREXCEPTION_HPP_
#define ALIBABACLOUD_EXCEPTIONS_SERVEREXCEPTION_HPP_
#include <darabonba/Core.hpp>
using namespace std;
using json = nlohmann::json;
namespace AlibabaCloud
{
namespace OpenApi
{
namespace Exceptions
{
  class ServerException : public AlibabaCloudException {
  public:
    friend void from_json(const Darabonba::Json& j, ServerException& obj) { 
      (void)j; (void)obj; 
    };
    ServerException() ;
    ServerException(const ServerException &) = default ;
    ServerException(ServerException &&) = default ;
    ServerException(const Darabonba::Json & obj) : AlibabaCloudException(obj) { from_json(obj, *this); };
    virtual ~ServerException() = default ;
    ServerException& operator=(const ServerException &) = default ;
    ServerException& operator=(ServerException &&) = default ;
  };
  
  } // namespace Exceptions
} // namespace AlibabaCloud
} // namespace OpenApi
#endif
