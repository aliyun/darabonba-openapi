#include <darabonba/Core.hpp>
#include <alibabacloud/Utils.hpp>
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

/**
 * Convert all params of body other than type of readable into content 
 * @param body source Model
 * @param content target Model
 * @return void
 */
void Utils::convert(const Darabonba::Model &body, const Darabonba::Model &content) {}

/**
 * If endpointType is internal, use internal endpoint
 * If serverUse is true and endpointType is accelerate, use accelerate endpoint
 * Default return endpoint
 * @param serverUse whether use accelerate endpoint
 * @param endpointType value must be internal or accelerate
 * @return the final endpoint
 */
string Utils::getEndpoint(const string &endpoint, const bool &serverUse, const string &endpointType) {}

/**
 * Get throttling param
 * @param the response headers
 * @return time left
 */
int64_t Utils::getThrottlingTimeLeft(const map<string, string> &headers) {}

/**
 * Hash the raw data with signatureAlgorithm
 * @param raw hashing data
 * @param signatureAlgorithm the autograph method
 * @return hashed bytes
 */
Darabonba::Bytes Utils::hash(const Darabonba::Bytes &raw, const string &signatureAlgorithm) {}

/**
 * Get throttling param
 * @param the response headers
 * @return time left
 */
map<string, string> Utils::flatMap(const json &params, const string &prefix) {}

/**
 * Generate a nonce string
 * @return the nonce string
 */
string Utils::getNonce() {}

/**
 * Get the string to be signed according to request
 * @param request  which contains signed messages
 * @return the signed string
 */
string Utils::getStringToSign(const Darabonba::Http::Request &request) {}

/**
 * Get signature according to stringToSign, secret
 * @param stringToSign  the signed string
 * @param secret accesskey secret
 * @return the signature
 */
string Utils::getROASignature(const string &stringToSign, const string &secret) {}

/**
 * Parse filter into a form string
 * @param filter object
 * @return the string
 */
string Utils::toForm(const Darabonba::Json &filter) {}

/**
 * Get timestamp
 * @return the timestamp string
 */
string Utils::getTimestamp() {}

/**
 * Get UTC string
 * @return the UTC string
 */
string Utils::getDateUTCString() {}

/**
 * Parse filter into a object which's type is map[string]string
 * @param filter query param
 * @return the object
 */
map<string, string> Utils::query(const Darabonba::Json &filter) {}

/**
 * Get signature according to signedParams, method and secret
 * @param signedParams params which need to be signed
 * @param method http method e.g. GET
 * @param secret AccessKeySecret
 * @return the signature
 */
string Utils::getRPCSignature(const map<string, string> &signedParams, const string &method, const string &secret) {}

/**
 * Parse array into a string with specified style
 * @param array the array
 * @param prefix the prefix string
 * @style specified style e.g. repeatList
 * @return the string
 */
string Utils::arrayToStringWithSpecifiedStyle(const Darabonba::Json &array, const string &prefix, const string &style) {}

/**
 * Stringify the value of map
 * @return the new stringified map
 */
map<string, string> Utils::stringifyMapValue(const json &m) {}

/**
 * Transform input as array.
 */
vector<json> Utils::toArray(const Darabonba::Json &input) {}

/**
 * Parse map with flat style
 *
 * @param any the input
 * @return any
 */
Darabonba::Json Utils::mapToFlatStyle(const Darabonba::Json &input) {}

/**
 * Transform input as map.
 */
json Utils::parseToMap(const Darabonba::Json &input) {}

/**
 * Get the authorization 
 * @param request request params
 * @param signatureAlgorithm the autograph method
 * @param payload the hashed request
 * @param accesskey the accesskey string
 * @param accessKeySecret the accessKeySecret string
 * @return authorization string
 */
string Utils::getAuthorization(const Darabonba::Http::Request &request, const string &signatureAlgorithm, const string &payload, const string &accesskey, const string &accessKeySecret) {}

string Utils::getUserAgent(const string &userAgent) {}

/**
 * Get endpoint according to productId, regionId, endpointType, network and suffix
 * @return endpoint
 */
string Utils::getEndpointRules(const string &product, const string &regionId, const string &endpointType, const string &network, const string &suffix) {}
} // namespace AlibabaCloud
} // namespace OpenApi
} // namespace Utils