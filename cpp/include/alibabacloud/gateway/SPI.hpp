#ifndef ALIBABACLOUD_GATEWAP_SPI_H_
#define ALIBABACLOUD_GATEWAP_SPI_H_

#include <alibabacloud/gateway/AttributeMap.hpp>
#include <alibabacloud/gateway/InterceptorContext.hpp>
#include <memory>

using namespace AlibabaCloud::Gateway::Models;

namespace AlibabaCloud {
namespace Gateway {
class SPI {
public:
  virtual ~SPI() {}
  virtual void modifyConfiguration(InterceptorContext &context,
                                   AttributeMap &attributeMap) = 0;

  virtual void modifyRequest(InterceptorContext &context,
                             AttributeMap &attributeMap) = 0;

  virtual void modifyResponse(InterceptorContext &context,
                              AttributeMap &attributeMap) = 0;
};

} // namespace Gateway
} // namespace AlibabaCloud

namespace nlohmann {
  template <>
  struct adl_serializer<std::shared_ptr<AlibabaCloud::Gateway::SPI>> {
    static void to_json(json &j, const std::shared_ptr<AlibabaCloud::Gateway::SPI> &body) {
      j = reinterpret_cast<uintptr_t>(body.get());
    }

    static std::shared_ptr<AlibabaCloud::Gateway::SPI> from_json(const json &j) {
      if (j.is_number_unsigned()) {
        AlibabaCloud::Gateway::SPI *ptr = reinterpret_cast<AlibabaCloud::Gateway::SPI *>(j.get<uintptr_t>());
        return std::shared_ptr<AlibabaCloud::Gateway::SPI>(ptr);
      }
      return nullptr;
    }
  };
}
#endif
