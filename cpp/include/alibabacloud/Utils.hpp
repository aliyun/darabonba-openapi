// This file is auto-generated, don't edit it. Thanks.
#ifndef ALIBABACLOUD_UTILS_HPP_
#define ALIBABACLOUD_UTILS_HPP_
#include <darabonba/Core.hpp>
#include <alibabacloud/UtilsModel.hpp>
#include <darabonba/Model.hpp>
#include <map>
#include <darabonba/http/Request.hpp>
using namespace std;
using json = nlohmann::json;
namespace AlibabaCloud
{
namespace OpenApi
{
namespace Utils
{
  class Utils {
    public:
      Utils() {}
      /**
       * Convert all params of body other than type of readable into content 
       * @param body source Model
       * @param content target Model
       * @return void
       */
      static void convert(const Darabonba::Model &body, const Darabonba::Model &content);

      /**
       * If endpointType is internal, use internal endpoint
       * If serverUse is true and endpointType is accelerate, use accelerate endpoint
       * Default return endpoint
       * @param serverUse whether use accelerate endpoint
       * @param endpointType value must be internal or accelerate
       * @return the final endpoint
       */
      static string getEndpoint(const string &endpoint, const bool &serverUse, const string &endpointType);

      /**
       * Get throttling param
       * @param the response headers
       * @return time left
       */
      static int64_t getThrottlingTimeLeft(const map<string, string> &headers);

      /**
       * Hash the raw data with signatureAlgorithm
       * @param raw hashing data
       * @param signatureAlgorithm the autograph method
       * @return hashed bytes
       */
      static Darabonba::Bytes hash(const Darabonba::Bytes &raw, const string &signatureAlgorithm);

      /**
       * Get throttling param
       * @param the response headers
       * @return time left
       */
      static map<string, string> flatMap(const json &params, const string &prefix);

      /**
       * Generate a nonce string
       * @return the nonce string
       */
      static string getNonce();

      /**
       * Get the string to be signed according to request
       * @param request  which contains signed messages
       * @return the signed string
       */
      static string getStringToSign(const Darabonba::Http::Request &request);

      /**
       * Get signature according to stringToSign, secret
       * @param stringToSign  the signed string
       * @param secret accesskey secret
       * @return the signature
       */
      static string getROASignature(const string &stringToSign, const string &secret);

      /**
       * Parse filter into a form string
       * @param filter object
       * @return the string
       */
      static string toForm(const Darabonba::Json &filter);

      /**
       * Get timestamp
       * @return the timestamp string
       */
      static string getTimestamp();

      /**
       * Get UTC string
       * @return the UTC string
       */
      static string getDateUTCString();

      /**
       * Parse filter into a object which's type is map[string]string
       * @param filter query param
       * @return the object
       */
      static map<string, string> query(const Darabonba::Json &filter);

      /**
       * Get signature according to signedParams, method and secret
       * @param signedParams params which need to be signed
       * @param method http method e.g. GET
       * @param secret AccessKeySecret
       * @return the signature
       */
      static string getRPCSignature(const map<string, string> &signedParams, const string &method, const string &secret);

      /**
       * Parse array into a string with specified style
       * @param array the array
       * @param prefix the prefix string
       * @style specified style e.g. repeatList
       * @return the string
       */
      static string arrayToStringWithSpecifiedStyle(const Darabonba::Json &array, const string &prefix, const string &style);

      /**
       * Stringify the value of map
       * @return the new stringified map
       */
      static map<string, string> stringifyMapValue(const json &m);

      /**
       * Transform input as array.
       */
      static vector<json> toArray(const Darabonba::Json &input);

      /**
       * Parse map with flat style
       *
       * @param any the input
       * @return any
       */
      static Darabonba::Json mapToFlatStyle(const Darabonba::Json &input);

      /**
       * Transform input as map.
       */
      static json parseToMap(const Darabonba::Json &input);

      /**
       * Get the authorization 
       * @param request request params
       * @param signatureAlgorithm the autograph method
       * @param payload the hashed request
       * @param accesskey the accesskey string
       * @param accessKeySecret the accessKeySecret string
       * @return authorization string
       */
      static string getAuthorization(const Darabonba::Http::Request &request, const string &signatureAlgorithm, const string &payload, const string &accesskey, const string &accessKeySecret);

      static string getUserAgent(const string &userAgent);

      /**
       * Get endpoint according to productId, regionId, endpointType, network and suffix
       * @return endpoint
       */
      static string getEndpointRules(const string &product, const string &regionId, const string &endpointType, const string &network, const string &suffix);
  };
} // namespace AlibabaCloud
} // namespace OpenApi
} // namespace Utils
#endif
