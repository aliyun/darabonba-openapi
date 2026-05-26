#ifndef ALIBABACLOUD_GATEWAY_POP_HPP_H_
#define ALIBABACLOUD_GATEWAY_POP_HPP_H_

#include <alibabacloud/gateway/AttributeMap.hpp>
#include <alibabacloud/gateway/InterceptorContext.hpp>
#include <alibabacloud/gateway/SPI.hpp>
#include <cstdint>
#include <darabonba/Type.hpp>

#include <string>
#include <vector>

namespace AlibabaCloud {
namespace Gateway {
class POP : public SPI {
public:
  POP();
  void modifyConfiguration(InterceptorContext &context,
                           AttributeMap &attributeMap);

  void modifyRequest(InterceptorContext &context, AttributeMap &attributeMap);

  void modifyResponse(InterceptorContext &context, AttributeMap &attributeMap);

  std::string getEndpoint(const std::string &productId,
                          const std::string &regionId,
                          const std::string &endpointRule,
                          const std::string &network, const std::string &suffix,
                          const std::map<std::string, std::string> &endpointMap,
                          const std::string &endpoint);

  Darabonba::Json defaultAny(Darabonba::Json &inputValue,
                             Darabonba::Json &defaultValue);

  std::string
  getAuthorization(const std::string &pathname, const std::string &method,
                   const Darabonba::Http::Query &query,
                   const Darabonba::Http::Header &headers,
                   const std::string &signatureAlgorithm,
                   const std::string &payload, const std::string &ak,
                   const Darabonba::Bytes &signingkey,
                   const std::string &product, const std::string &region,
                   const std::string &date);

  std::string getSignature(const std::string &pathname,
                           const std::string &method,
                           const Darabonba::Http::Query &query,
                           const Darabonba::Http::Header &headers,
                           const std::string &signatureAlgorithm,
                           const std::string &payload,
                           const Darabonba::Bytes &signingkey);

  Darabonba::Bytes getSigningkey(const std::string &signatureAlgorithm,
                                 const std::string &secret,
                                 const std::string &product,
                                 const std::string &region,
                                 const std::string &date);

  std::string getRegion(const std::string &product,
                        const std::string &endpoint);

  std::string buildCanonicalizedResource(const Darabonba::Http::Query &query);

  std::string buildCanonicalizedHeaders(const Darabonba::Http::Header &headers);

  std::vector<std::string>
  getSignedHeaders(const Darabonba::Http::Header &headers);

  std::shared_ptr<std::map<std::string, std::string>> ptr_;

protected:
  std::string _sha256;

  std::string _sm3;
};
} // namespace Gateway
} // namespace AlibabaCloud
#endif
