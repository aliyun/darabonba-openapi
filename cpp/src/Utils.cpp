#include <darabonba/Core.hpp>
#include <alibabacloud/Utils.hpp>
#include <darabonba/Model.hpp>
#include <darabonba/http/Request.hpp>
#include <darabonba/Stream.hpp>
#include <darabonba/http/Form.hpp>
#include <darabonba/encode/Encoder.hpp>
#include <openssl/hmac.h>
#include <openssl/sha.h>
#include <openssl/md5.h>
#include <openssl/evp.h>
#include <map>
#include <vector>
#include <string>
#include <sstream>
#include <iomanip>
#include <ctime>
#include <chrono>
#include <random>
#include <thread>
#include <algorithm>
#include <regex>

using namespace std;
using json = nlohmann::json;

namespace AlibabaCloud
{
namespace OpenApi
{
namespace Utils
{

// Helper function to URL encode
static string urlEncode(const string &str) {
  ostringstream escaped;
  escaped.fill('0');
  escaped << hex;

  for (char c : str) {
    if (isalnum(c) || c == '-' || c == '_' || c == '.' || c == '~') {
      escaped << c;
    } else {
      escaped << uppercase;
      escaped << '%' << setw(2) << int((unsigned char)c);
      escaped << nouppercase;
    }
  }

  return escaped.str();
}

// Helper function to flatten map recursively
static void flattenMap(const json &obj, map<string, string> &result, const string &prefix) {
  if (obj.is_null()) {
    return;
  }

  if (obj.is_object()) {
    for (auto it = obj.begin(); it != obj.end(); ++it) {
      string key = prefix.empty() ? it.key() : prefix + "." + it.key();
      flattenMap(it.value(), result, key);
    }
  } else if (obj.is_array()) {
    for (size_t i = 0; i < obj.size(); ++i) {
      string key = prefix + "." + to_string(i + 1);
      flattenMap(obj[i], result, key);
    }
  } else {
    if (obj.is_string()) {
      result[prefix] = obj.get<string>();
    } else {
      result[prefix] = obj.dump();
    }
  }
}

// Helper function to get time left from rate limit header
static int64_t getTimeLeft(const string &rateLimit) {
  if (rateLimit.empty()) {
    return 0;
  }

  vector<string> pairs;
  size_t start = 0;
  size_t end = rateLimit.find(',');
  while (end != string::npos) {
    pairs.push_back(rateLimit.substr(start, end - start));
    start = end + 1;
    end = rateLimit.find(',', start);
  }
  pairs.push_back(rateLimit.substr(start));

  for (const auto &pair : pairs) {
    size_t colonPos = pair.find(':');
    if (colonPos != string::npos) {
      string key = pair.substr(0, colonPos);
      string value = pair.substr(colonPos + 1);
      // Trim whitespace
      key.erase(0, key.find_first_not_of(" \t\n\r"));
      key.erase(key.find_last_not_of(" \t\n\r") + 1);
      value.erase(0, value.find_first_not_of(" \t\n\r"));
      value.erase(value.find_last_not_of(" \t\n\r") + 1);

      if (key == "TimeLeft") {
        try {
          return stoll(value);
        } catch (...) {
          return 0;
        }
      }
    }
  }
  return 0;
}

// Helper function to filter out Stream types from JSON
static json exceptStream(const json &obj) {
  if (obj.is_array()) {
    json result = json::array();
    for (const auto &item : obj) {
      if (!item.is_null()) {
        json filtered = exceptStream(item);
        if (!filtered.is_null()) {
          result.push_back(filtered);
        }
      } else {
        result.push_back(item);
      }
    }
    return result;
  } else if (obj.is_object()) {
    json result = json::object();
    for (auto it = obj.begin(); it != obj.end(); ++it) {
      if (!it.value().is_null()) {
        json filtered = exceptStream(it.value());
        if (!filtered.is_null()) {
          result[it.key()] = filtered;
        }
      } else {
        result[it.key()] = it.value();
      }
    }
    return result;
  }
  // Note: In C++, we cannot directly detect Stream types in JSON
  // Stream types should not be serialized to JSON in the first place
  // If a field contains a Stream, it should be handled before calling toMap()
  return obj;
}

/**
 * Convert all params of body other than type of readable into content 
 * @param body source Model
 * @param content target Model
 * @return void
 */
void Utils::convert(const Darabonba::Model &body, const Darabonba::Model &content) {
  // Convert body to JSON map
  json bodyMap = body.toMap();
  
  // Filter out Stream types
  json filteredMap = exceptStream(bodyMap);
  
  // Apply filtered map to content
  const_cast<Darabonba::Model &>(content).fromMap(filteredMap);
}

/**
 * If endpointType is internal, use internal endpoint
 * If serverUse is true and endpointType is accelerate, use accelerate endpoint
 * Default return endpoint
 * @param serverUse whether use accelerate endpoint
 * @param endpointType value must be internal or accelerate
 * @return the final endpoint
 */
string Utils::getEndpoint(const string &endpoint, const bool &serverUse, const string &endpointType) {
  string result = endpoint;
  
  if (endpointType == "internal") {
    size_t pos = result.find('.');
    if (pos != string::npos) {
      result = result.substr(0, pos) + "-internal" + result.substr(pos);
    }
  }
  
  if (serverUse && endpointType == "accelerate") {
    return "oss-accelerate.aliyuncs.com";
  }
  
  return result;
}

/**
 * Get throttling param
 * @param the response headers
 * @return time left
 */
int64_t Utils::getThrottlingTimeLeft(const map<string, string> &headers) {
  auto it1 = headers.find("x-ratelimit-user-api");
  auto it2 = headers.find("x-ratelimit-user");
  
  int64_t timeLeft1 = 0;
  int64_t timeLeft2 = 0;
  
  if (it1 != headers.end()) {
    timeLeft1 = getTimeLeft(it1->second);
  }
  
  if (it2 != headers.end()) {
    timeLeft2 = getTimeLeft(it2->second);
  }
  
  return max(timeLeft1, timeLeft2);
}

/**
 * Hash the raw data with signatureAlgorithm
 * @param raw hashing data
 * @param signatureAlgorithm the autograph method
 * @return hashed bytes
 */
Darabonba::Bytes Utils::hash(const Darabonba::Bytes &raw, const string &signatureAlgorithm) {
  if (signatureAlgorithm == "ACS3-HMAC-SHA256" || signatureAlgorithm == "ACS3-RSA-SHA256") {
    unsigned char hashData[SHA256_DIGEST_LENGTH];
    SHA256(reinterpret_cast<const unsigned char*>(raw.data()), raw.size(), hashData);
    Darabonba::Bytes result;
    result.assign(hashData, hashData + SHA256_DIGEST_LENGTH);
    return result;
  } else if (signatureAlgorithm == "ACS3-HMAC-SM3") {
    // SM3 is not commonly available in OpenSSL, would need external library
    // For now, return empty bytes
    return Darabonba::Bytes();
  }
  
  return Darabonba::Bytes();
}

/**
 * Flatten map with prefix
 * @param the response headers
 * @return time left
 */
map<string, string> Utils::flatMap(const json &params, const string &prefix) {
  map<string, string> result;
  flattenMap(params, result, prefix);
  return result;
}

/**
 * Generate a nonce string
 * @return the nonce string
 */
string Utils::getNonce() {
  static auto startTime = chrono::system_clock::now();
  auto now = chrono::system_clock::now();
  auto duration = chrono::duration_cast<chrono::milliseconds>(now.time_since_epoch());
  
  random_device rd;
  mt19937_64 gen(rd());
  uniform_int_distribution<uint64_t> dis;
  
  ostringstream oss;
  oss << chrono::duration_cast<chrono::milliseconds>(startTime.time_since_epoch()).count()
      << "-" << this_thread::get_id()
      << "-" << duration.count()
      << "-" << dis(gen);
  
  string msg = oss.str();
  
  // Use EVP API instead of deprecated MD5
  unsigned char hashData[MD5_DIGEST_LENGTH];
  EVP_MD_CTX *mdctx = EVP_MD_CTX_new();
  EVP_DigestInit_ex(mdctx, EVP_md5(), NULL);
  EVP_DigestUpdate(mdctx, msg.c_str(), msg.length());
  EVP_DigestFinal_ex(mdctx, hashData, NULL);
  EVP_MD_CTX_free(mdctx);
  
  ostringstream result;
  for (int i = 0; i < MD5_DIGEST_LENGTH; ++i) {
    result << hex << setw(2) << setfill('0') << static_cast<int>(hashData[i]);
  }
  
  return result.str();
}

/**
 * Get the string to be signed according to request
 * @param request  which contains signed messages
 * @return the signed string
 */
string Utils::getStringToSign(const Darabonba::Http::Request &request) {
  string method = request.getMethod();
  string pathname = request.getPathname();
  auto headers = request.getHeaders();
  auto query = request.getQuery();
  
  string accept = headers.count("accept") ? headers.at("accept") : "";
  string contentMd5 = headers.count("content-md5") ? headers.at("content-md5") : "";
  string contentType = headers.count("content-type") ? headers.at("content-type") : "";
  string date = headers.count("date") ? headers.at("date") : "";
  
  ostringstream header;
  header << method << "\n" << accept << "\n" << contentMd5 << "\n" 
         << contentType << "\n" << date << "\n";
  
  // Get canonicalized ACS headers
  map<string, string> acsHeaders;
  for (const auto &h : headers) {
    string lowerKey = h.first;
    transform(lowerKey.begin(), lowerKey.end(), lowerKey.begin(), ::tolower);
    if (lowerKey.find("x-acs-") == 0) {
      acsHeaders[lowerKey] = h.second;
    }
  }
  
  ostringstream canonHeaders;
  for (const auto &h : acsHeaders) {
    canonHeaders << h.first << ":" << h.second << "\n";
  }
  
  // Get canonicalized resource
  ostringstream resource;
  resource << pathname;
  
  if (!query.empty()) {
    vector<string> sortedKeys;
    for (const auto &q : query) {
      sortedKeys.push_back(q.first);
    }
    sort(sortedKeys.begin(), sortedKeys.end());
    
    resource << "?";
    bool first = true;
    for (const auto &key : sortedKeys) {
      if (!first) resource << "&";
      first = false;
      resource << key;
      if (!query.at(key).empty()) {
        resource << "=" << query.at(key);
      }
    }
  }
  
  return header.str() + canonHeaders.str() + resource.str();
}

/**
 * Get signature according to stringToSign, secret
 * @param stringToSign  the signed string
 * @param secret accesskey secret
 * @return the signature
 */
string Utils::getROASignature(const string &stringToSign, const string &secret) {
  unsigned char hashData[SHA_DIGEST_LENGTH];
  unsigned int hashLen;
  
  HMAC(EVP_sha1(), secret.c_str(), secret.length(),
       reinterpret_cast<const unsigned char*>(stringToSign.c_str()),
       stringToSign.length(), hashData, &hashLen);
  
  Darabonba::Bytes bytes;
  bytes.assign(hashData, hashData + hashLen);
  return Darabonba::Encode::Encoder::base64EncodeToString(bytes);
}

/**
 * Parse filter into a form string
 * @param filter object
 * @return the string
 */
string Utils::toForm(const Darabonba::Json &filter) {
  if (filter.is_null()) {
    return "";
  }
  
  map<string, string> flattened;
  flattenMap(json(filter), flattened, "");
  return Darabonba::Http::Form::toFormString(flattened);
}

/**
 * Get timestamp
 * @return the timestamp string
 */
string Utils::getTimestamp() {
  auto now = chrono::system_clock::now();
  time_t tt = chrono::system_clock::to_time_t(now);
  tm utc_tm;
  
#ifdef _WIN32
  // Windows uses gmtime_s with reversed parameter order
  gmtime_s(&utc_tm, &tt);
#else
  // Unix/Linux/macOS use gmtime_r
  gmtime_r(&tt, &utc_tm);
#endif
  
  ostringstream oss;
  oss << put_time(&utc_tm, "%Y-%m-%dT%H:%M:%SZ");
  return oss.str();
}

/**
 * Get UTC string
 * @return the UTC string
 */
string Utils::getDateUTCString() {
  auto now = chrono::system_clock::now();
  time_t tt = chrono::system_clock::to_time_t(now);
  tm utc_tm;
  
#ifdef _WIN32
  // Windows uses gmtime_s with reversed parameter order
  gmtime_s(&utc_tm, &tt);
#else
  // Unix/Linux/macOS use gmtime_r
  gmtime_r(&tt, &utc_tm);
#endif
  
  ostringstream oss;
  oss << put_time(&utc_tm, "%a, %d %b %Y %H:%M:%S GMT");
  return oss.str();
}

/**
 * Parse filter into a object which's type is map[string]string
 * @param filter query param
 * @return the object
 */
map<string, string> Utils::query(const Darabonba::Json &filter) {
  if (filter.is_null()) {
    return map<string, string>();
  }
  
  map<string, string> result;
  flattenMap(json(filter), result, "");
  return result;
}

/**
 * Get signature according to signedParams, method and secret
 * @param signedParams params which need to be signed
 * @param method http method e.g. GET
 * @param secret AccessKeySecret
 * @return the signature
 */
string Utils::getRPCSignature(const map<string, string> &signedParams, const string &method, const string &secret) {
  // Sort parameters
  vector<pair<string, string>> sortedParams(signedParams.begin(), signedParams.end());
  sort(sortedParams.begin(), sortedParams.end());
  
  // Build canonicalized query string
  ostringstream oss;
  bool first = true;
  for (const auto &p : sortedParams) {
    if (!first) oss << "&";
    first = false;
    oss << urlEncode(p.first) << "=" << urlEncode(p.second);
  }
  
  string canonicalized = oss.str();
  string stringToSign = method + "&" + urlEncode("/") + "&" + urlEncode(canonicalized);
  
  // HMAC-SHA1 signature
  string key = secret + "&";
  unsigned char hashData[SHA_DIGEST_LENGTH];
  unsigned int hashLen;
  
  HMAC(EVP_sha1(), key.c_str(), key.length(),
       reinterpret_cast<const unsigned char*>(stringToSign.c_str()),
       stringToSign.length(), hashData, &hashLen);
  
  Darabonba::Bytes bytes;
  bytes.assign(hashData, hashData + hashLen);
  return Darabonba::Encode::Encoder::base64EncodeToString(bytes);
}

/**
 * Parse array into a string with specified style
 * @param array the array
 * @param prefix the prefix string
 * @style specified style e.g. repeatList
 * @return the string
 */
string Utils::arrayToStringWithSpecifiedStyle(const Darabonba::Json &array, const string &prefix, const string &style) {
  if (array.is_null() || !array.is_array()) {
    return "";
  }
  
  json arr = json(array);
  
  if (style == "repeatList") {
    map<string, string> flattened;
    json obj = json::object();
    obj[prefix] = arr;
    flattenMap(obj, flattened, "");
    
    vector<pair<string, string>> sorted(flattened.begin(), flattened.end());
    sort(sorted.begin(), sorted.end());
    
    ostringstream oss;
    bool first = true;
    for (const auto &p : sorted) {
      if (!first) oss << "&&";
      first = false;
      oss << urlEncode(p.first) << "=" << urlEncode(p.second);
    }
    return oss.str();
  } else if (style == "json") {
    return arr.dump();
  } else if (style == "simple") {
    ostringstream oss;
    for (size_t i = 0; i < arr.size(); ++i) {
      if (i > 0) oss << ",";
      if (arr[i].is_string()) {
        oss << arr[i].get<string>();
      } else {
        oss << arr[i].dump();
      }
    }
    return oss.str();
  } else if (style == "spaceDelimited") {
    ostringstream oss;
    for (size_t i = 0; i < arr.size(); ++i) {
      if (i > 0) oss << " ";
      if (arr[i].is_string()) {
        oss << arr[i].get<string>();
      } else {
        oss << arr[i].dump();
      }
    }
    return oss.str();
  } else if (style == "pipeDelimited") {
    ostringstream oss;
    for (size_t i = 0; i < arr.size(); ++i) {
      if (i > 0) oss << "|";
      if (arr[i].is_string()) {
        oss << arr[i].get<string>();
      } else {
        oss << arr[i].dump();
      }
    }
    return oss.str();
  }
  
  return "";
}

/**
 * Stringify the value of map
 * @return the new stringified map
 */
map<string, string> Utils::stringifyMapValue(const json &m) {
  map<string, string> result;
  
  if (m.is_null() || !m.is_object()) {
    return result;
  }
  
  for (auto it = m.begin(); it != m.end(); ++it) {
    if (it.value().is_string()) {
      result[it.key()] = it.value().get<string>();
    } else if (!it.value().is_null()) {
      result[it.key()] = it.value().dump();
    }
  }
  
  return result;
}

/**
 * Transform input as array.
 */
vector<json> Utils::toArray(const Darabonba::Json &input) {
  vector<json> result;
  
  if (input.is_null()) {
    return result;
  }
  
  json arr = json(input);
  if (!arr.is_array()) {
    return result;
  }
  
  for (const auto &item : arr) {
    result.push_back(item);
  }
  
  return result;
}

/**
 * Parse map with flat style
 * Transform a map to a flat style map where dictionary keys are prefixed with length info.
 * Map keys are transformed from "key" to "#length#key" format.
 *
 * @param input the input
 * @return transformed json
 */
Darabonba::Json Utils::mapToFlatStyle(const Darabonba::Json &input) {
  if (input.is_null()) {
    return input;
  }
  
  json obj = json(input);
  
  // Handle array
  if (obj.is_array()) {
    json result = json::array();
    for (const auto &item : obj) {
      result.push_back(json(mapToFlatStyle(item)));
    }
    return result;
  }
  
  // Handle object/map - apply flat style to all keys
  if (obj.is_object()) {
    json result = json::object();
    
    for (auto it = obj.begin(); it != obj.end(); ++it) {
      const string &key = it.key();
      const json &value = it.value();
      
      // Transform key to flat style format: #length#key
      string flatKey = "#" + to_string(key.length()) + "#" + key;
      result[flatKey] = json(mapToFlatStyle(value));
    }
    
    return result;
  }
  
  // For primitive types, return as-is
  return obj;
}

/**
 * Transform input as map.
 */
json Utils::parseToMap(const Darabonba::Json &input) {
  if (input.is_null()) {
    return json();
  }
  
  return json(input);
}

/**
 * Get the authorization 
 * @param request request params
 * @param signatureAlgorithm the autograph method
 * @param payload the hashed request
 * @param accesskey the accesskey string
 * @param accessKeySecret the accessKeySecret string
 * @return authorization string
 */
string Utils::getAuthorization(const Darabonba::Http::Request &request, const string &signatureAlgorithm, 
                                const string &payload, const string &accesskey, const string &accessKeySecret) {
  string pathname = request.getPathname();
  if (pathname.empty()) {
    pathname = "/";
  }
  
  // URL encode pathname
  string canonicalURI = pathname;
  replace(canonicalURI.begin(), canonicalURI.end(), '+', ' ');
  canonicalURI = regex_replace(canonicalURI, regex("\\*"), "%2A");
  canonicalURI = regex_replace(canonicalURI, regex("%7E"), "~");
  
  string method = request.getMethod();
  
  // Get canonical query string
  auto query = request.getQuery();
  vector<string> sortedKeys;
  for (const auto &q : query) {
    sortedKeys.push_back(q.first);
  }
  sort(sortedKeys.begin(), sortedKeys.end());
  
  ostringstream canonicalQuery;
  bool first = true;
  for (const auto &key : sortedKeys) {
    if (!first) canonicalQuery << "&";
    first = false;
    canonicalQuery << key << "=";
    if (!query[key].empty()) {
      canonicalQuery << urlEncode(query[key]);
    }
  }
  
  // Get canonical headers
  auto headers = request.getHeaders();
  map<string, vector<string>> headerMap;
  for (const auto &h : headers) {
    string lowerKey = h.first;
    transform(lowerKey.begin(), lowerKey.end(), lowerKey.begin(), ::tolower);
    if (lowerKey.find("x-acs-") == 0 || lowerKey == "host" || lowerKey == "content-type") {
      headerMap[lowerKey].push_back(h.second);
    }
  }
  
  vector<string> signedHeaderKeys;
  ostringstream canonicalHeaders;
  for (auto &h : headerMap) {
    signedHeaderKeys.push_back(h.first);
    sort(h.second.begin(), h.second.end());
    canonicalHeaders << h.first << ":";
    for (size_t i = 0; i < h.second.size(); ++i) {
      if (i > 0) canonicalHeaders << ",";
      canonicalHeaders << h.second[i];
    }
    canonicalHeaders << "\n";
  }
  
  ostringstream signedHeaders;
  for (size_t i = 0; i < signedHeaderKeys.size(); ++i) {
    if (i > 0) signedHeaders << ";";
    signedHeaders << signedHeaderKeys[i];
  }
  
  // Build canonical request
  ostringstream canonicalRequest;
  canonicalRequest << method << "\n"
                   << canonicalURI << "\n"
                   << canonicalQuery.str() << "\n"
                   << canonicalHeaders.str() << "\n"
                   << signedHeaders.str() << "\n"
                   << payload;
  
  // Hash canonical request
  string canonicalRequestStr = canonicalRequest.str();
  Darabonba::Bytes requestBytes;
  requestBytes.assign(canonicalRequestStr.begin(), canonicalRequestStr.end());
  Darabonba::Bytes hashedRequest = hash(requestBytes, signatureAlgorithm);
  string hashedRequestHex = Darabonba::Encode::Encoder::hexEncode(hashedRequest);
  
  // Build string to sign
  string stringToSign = signatureAlgorithm + "\n" + hashedRequestHex;
  
  // Sign
  Darabonba::Bytes signature;
  if (signatureAlgorithm == "ACS3-HMAC-SHA256") {
    unsigned char hashData[SHA256_DIGEST_LENGTH];
    unsigned int hashLen;
    HMAC(EVP_sha256(), accessKeySecret.c_str(), accessKeySecret.length(),
         reinterpret_cast<const unsigned char*>(stringToSign.c_str()),
         stringToSign.length(), hashData, &hashLen);
    signature.assign(hashData, hashData + hashLen);
  } else if (signatureAlgorithm == "ACS3-RSA-SHA256") {
    // RSA signature - would need private key handling
    // For now, return empty
  }
  
  string signatureHex = Darabonba::Encode::Encoder::hexEncode(signature);
  
  // Build authorization header
  ostringstream auth;
  auth << signatureAlgorithm << " "
       << "Credential=" << accesskey << ","
       << "SignedHeaders=" << signedHeaders.str() << ","
       << "Signature=" << signatureHex;
  
  return auth.str();
}

string Utils::getUserAgent(const string &userAgent) {
  string defaultUA = "AlibabaCloud/cpp";
  
  if (userAgent.empty()) {
    return defaultUA;
  }
  
  return defaultUA + " " + userAgent;
}

/**
 * Get endpoint according to productId, regionId, endpointType, network and suffix
 * @return endpoint
 */
string Utils::getEndpointRules(const string &product, const string &regionId, const string &endpointType, const string &network, const string &suffix) {
  string result;
  
  if (endpointType == "regional") {
    if (regionId.empty()) {
      throw runtime_error("RegionId is empty, please set a valid RegionId");
    }
    result = "<product><suffix><network>." + regionId + ".aliyuncs.com";
  } else {
    result = "<product><suffix><network>.aliyuncs.com";
  }
  
  // Replace product
  string lowerProduct = product;
  transform(lowerProduct.begin(), lowerProduct.end(), lowerProduct.begin(), ::tolower);
  size_t pos = result.find("<product>");
  if (pos != string::npos) {
    result.replace(pos, 9, lowerProduct);
  }
  
  // Replace network
  string networkStr = (network.empty() || network == "public") ? "" : "-" + network;
  pos = result.find("<network>");
  if (pos != string::npos) {
    result.replace(pos, 9, networkStr);
  }
  
  // Replace suffix
  string suffixStr = suffix.empty() ? "" : "-" + suffix;
  pos = result.find("<suffix>");
  if (pos != string::npos) {
    result.replace(pos, 8, suffixStr);
  }
  
  return result;
}

} // namespace Utils
} // namespace OpenApi
} // namespace AlibabaCloud