// This file is auto-generated, don't edit it. Thanks.

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Darabonba;
using Darabonba.Utils;
using Tea;

namespace AlibabaCloud.OpenApiClient
{
    
    /// <term><b>Description:</b></term>
    /// <description>
    /// <para>This is for OpenApi Util</para>
    /// </description>
    public class Utils 
    {
        internal static readonly string SEPARATOR = "&";
        internal static readonly string PEM_BEGIN = "-----BEGIN RSA PRIVATE KEY-----\n";
        internal static readonly string PEM_END = "\n-----END RSA PRIVATE KEY-----";

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Convert all params of body other than type of readable into content</para>
        /// </description>
        /// 
        /// <param name="body">
        /// source Model
        /// </param>
        /// <param name="content">
        /// target Model
        /// </param>
        /// 
        /// <returns>
        /// void
        /// </returns>
        public static void Convert(TeaModel body, TeaModel content)
        {
            if (body == null || content == null)
            {
                return;
            }
            Dictionary<string, object> dict = body.ToMap();
            dict = (Dictionary<string, object>)ExceptStream(dict);
            Model.ToObject(dict, content);
        }

        internal static object ExceptStream(object obj)
        {
            if (typeof(IList).IsAssignableFrom(obj.GetType()) && !typeof(Array).IsAssignableFrom(obj.GetType()))
            {
                List<object> array = new List<object>();
                foreach (var temp in (IList)obj)
                {
                    if (temp != null)
                    {
                        object item = ExceptStream(temp);
                        if (item != null)
                        {
                            array.Add(item);
                        }
                    }
                    else {
                        array.Add(temp);
                    }
                }
                return array;
            }
            else if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
            {
                Dictionary<string, object> dict = ((IDictionary)obj).Keys.Cast<string>()
                    .ToDictionary(key => key, key => ((IDictionary)obj)[key]);
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (var keypair in dict)
                {
                    if (keypair.Value != null)
                    {
                        object item = ExceptStream(keypair.Value);
                        if (item != null)
                        {
                            result.Add(keypair.Key, item);
                        }
                    }
                    else {
                        result.Add(keypair.Key, keypair.Value);
                    }
                }
                return result;
            }
            else if (typeof(Stream).IsAssignableFrom(obj.GetType()))
            {
                return null;
            }
            return obj;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>If endpointType is internal, use internal endpoint
        /// If serverUse is true and endpointType is accelerate, use accelerate endpoint
        /// Default return endpoint</para>
        /// </description>
        /// 
        /// <param name="serverUse">
        /// whether use accelerate endpoint
        /// </param>
        /// <param name="endpointType">
        /// value must be internal or accelerate
        /// </param>
        /// 
        /// <returns>
        /// the final endpoint
        /// </returns>
        public static string GetEndpoint(string endpoint, bool? useAccelerate, string endpointType)
        {
            if (endpointType == "internal")
            {
                string[] strs = endpoint.Split('.');
                strs[0] += "-internal";
                endpoint = string.Join(".", strs);
            }

            if (useAccelerate == true && endpointType == "accelerate")
            {
                return "oss-accelerate.aliyuncs.com";
            }

            return endpoint;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Hash the raw data with signatureAlgorithm</para>
        /// </description>
        /// 
        /// <param name="raw">
        /// hashing data
        /// </param>
        /// <param name="signatureAlgorithm">
        /// the autograph method
        /// </param>
        /// 
        /// <returns>
        /// hashed bytes
        /// </returns>
        public static byte[] Hash(byte[] raw, string signatureAlgorithm)
        {
            if (signatureAlgorithm == "ACS3-HMAC-SHA256" || signatureAlgorithm == "ACS3-RSA-SHA256")
            {
                byte[] signData;
                using (SHA256 sha256 = new SHA256Managed())
                {
                    signData = sha256.ComputeHash(raw);
                }

                return signData;
            }

            return null;
        }

        public static int GetThrottlingTimeLeft(Dictionary<string, string> headers)
        {
            string rateLimitForUserApi = headers.ContainsKey("x-ratelimit-user-api") ? headers["x-ratelimit-user-api"] : null;
            string rateLimitForUser = headers.ContainsKey("x-ratelimit-user") ? headers["x-ratelimit-user"] : null;

            int timeLeftForUserApi = GetTimeLeft(rateLimitForUserApi);
            int timeLeftForUser = GetTimeLeft(rateLimitForUser);

            return Math.Max(timeLeftForUserApi, timeLeftForUser);
        }

        internal static int GetTimeLeft(string rateLimit)
        {
            if (!string.IsNullOrEmpty(rateLimit))
            {
                string[] pairs = rateLimit.Split(',');

                foreach (string pair in pairs)
                {
                    string[] kv = pair.Split(':');
                    if (kv.Length == 2)
                    {
                        string key = kv[0].Trim();
                        string value = kv[1].Trim();
                        if (key == "TimeLeft")
                        {
                            int timeLeftValue;
                            if (int.TryParse(value, out timeLeftValue))
                            {
                                return timeLeftValue;
                            }
                            return 0; // 返回0作为默认值，如果不能解析
                        }
                    }
                }
            }
            return 0; // 返回0作为默认值，如果字符串为空或没有TimeLeft
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get throttling param</para>
        /// </description>
        /// 
        /// <param name="the">
        /// response headers
        /// </param>
        /// 
        /// <returns>
        /// time left
        /// </returns>
        // public static Dictionary<string, string> FlatMap(Dictionary<string, object> params_, string prefix)
        // {
        //     throw new NotImplementedException();
        // }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Generate a nonce string</para>
        /// </description>
        /// 
        /// <returns>
        /// the nonce string
        /// </returns>
        public static string GetNonce()
        {
            return Guid.NewGuid().ToString();
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get the string to be signed according to request</para>
        /// </description>
        /// 
        /// <param name="request">
        /// which contains signed messages
        /// </param>
        /// 
        /// <returns>
        /// the signed string
        /// </returns>
        public static string GetStringToSign(Request request)
        {
            string method = request.Method;
            string pathname = request.Pathname;
            Dictionary<string, string> headers = request.Headers;
            Dictionary<string, string> query = request.Query;
            string accept = headers.ContainsKey("accept") ? headers["accept"] : "";
            string contentMD5 = headers.ContainsKey("content-md5") ? headers["content-md5"] : "";
            string contentType = headers.ContainsKey("content-type") ? headers["content-type"] : "";
            string date = headers.ContainsKey("date") ? headers["date"] : "";
            string header = method + "\n" + accept + "\n" + contentMD5 + "\n" + contentType + "\n" + date + "\n";
            string canonicalizedHeaders = GetCanonicalizedHeaders(headers);
            string canonicalizedResource = GetCanonicalizedResource(pathname, query);
            string stringToSign = header + canonicalizedHeaders + canonicalizedResource;
            return stringToSign;
        }

        internal static string GetCanonicalizedHeaders(Dictionary<string, string> headers)
        {
            string prefix = "x-acs-";
            List<string> canonicalizedKeys = new List<string>();
            canonicalizedKeys = headers.Where(p => p.Key.StartsWith(prefix))
                .Select(p => p.Key).ToList();
            canonicalizedKeys.Sort(StringComparer.Ordinal);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < canonicalizedKeys.Count; i++)
            {
                string key = canonicalizedKeys[i];
                result.Append(key);
                result.Append(":");
                result.Append(headers[key].Trim());
                result.Append("\n");
            }

            return result.ToString();
        }

        internal static string GetCanonicalizedResource(string pathname, Dictionary<string, string> query)
        {
            if (query == null || query.Count <= 0)
            {
                return pathname;
            }

            List<string> keys = query.Keys.ToList();
            keys.Sort(StringComparer.Ordinal);
            string key;
            List<string> result = new List<string>();
            for (int i = 0; i < keys.Count; i++)
            {
                key = keys[i];
                if (query[key] == null)
                {
                    continue;
                }

                if (query[key] == "")
                {
                    result.Add(key);
                }
                else
                {
                    result.Add(key + "=" + query[key]);
                }
            }

            return pathname + "?" + string.Join("&", result);
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get signature according to stringToSign, secret</para>
        /// </description>
        /// 
        /// <param name="stringToSign">
        /// the signed string
        /// </param>
        /// <param name="secret">
        /// accesskey secret
        /// </param>
        /// 
        /// <returns>
        /// the signature
        /// </returns>
        public static string GetROASignature(string stringToSign, string secret)
        {
            byte[] signData;
            using (KeyedHashAlgorithm algorithm = CryptoConfig.CreateFromName("HMACSHA1") as KeyedHashAlgorithm)
            {
                algorithm.Key = Encoding.UTF8.GetBytes(secret);
                signData = algorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToSign.ToCharArray()));
            }

            return System.Convert.ToBase64String(signData);
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Parse filter into a form string</para>
        /// </description>
        /// 
        /// <param name="filter">
        /// object
        /// </param>
        /// 
        /// <returns>
        /// the string
        /// </returns>
        public static string ToForm(Dictionary<string, object> filter)
        {
            if (filter == null)
            {
                return string.Empty;
            }

            Dictionary<string, object> dict = filter.Keys.Cast<string>().ToDictionary(key => key, key => filter[key]);
            Dictionary<string, string> outDict = new Dictionary<string, string>();
            TileDict(outDict, dict);
            List<string> listStr = new List<string>();
            foreach (var keypair in outDict)
            {
                if (string.IsNullOrWhiteSpace(keypair.Value))
                {
                    continue;
                }

                listStr.Add(PercentEncode(keypair.Key) + "=" + PercentEncode(keypair.Value));
            }

            return string.Join("&", listStr);
        }

        internal static string PercentEncode(string value)
        {
            if (value == null)
            {
                return null;
            }

            var stringBuilder = new StringBuilder();
            var text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            var bytes = Encoding.UTF8.GetBytes(value);
            foreach (char c in bytes)
            {
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
                }
            }

            return stringBuilder.ToString().Replace("+", "%20")
                .Replace("*", "%2A").Replace("%7E", "~");
        }

        internal static object ToJsonObject(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
            {
                Dictionary<string, object> dicIn = ((IDictionary)obj).Keys.Cast<string>()
                    .ToDictionary(key => key, key => ((IDictionary)obj)[key]);
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (var keypair in dicIn)
                {
                    if (keypair.Value == null)
                    {
                        continue;
                    }
                    result.Add(keypair.Key, ToJsonObject(keypair.Value));
                }
                return result;
            }
            else if (typeof(Model).IsAssignableFrom(obj.GetType()))
            {
                Dictionary<string, object> dicIn = ((Dictionary<string, object>)((Model)obj).ToMap());
                return ToJsonObject(dicIn);
            }
            else if (typeof(IList).IsAssignableFrom(obj.GetType()) && !typeof(Array).IsAssignableFrom(obj.GetType()))
            {
                List<object> array = new List<object>();
                foreach (var temp in (IList)obj)
                {
                    array.Add(ToJsonObject(temp));
                }
                return array;
            }
            return obj;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get timestamp</para>
        /// </description>
        /// 
        /// <returns>
        /// the timestamp string
        /// </returns>
        public static string GetTimestamp()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get UTC string</para>
        /// </description>
        /// 
        /// <returns>
        /// the UTC string
        /// </returns>
        public static string GetDateUTCString()
        {
            return DateTime.UtcNow.ToUniversalTime().GetDateTimeFormats('r')[0];
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Parse filter into a object which&#39;s type is map[string]string</para>
        /// </description>
        /// 
        /// <param name="filter">
        /// query param
        /// </param>
        /// 
        /// <returns>
        /// the object
        /// </returns>
        public static Dictionary<string, string> Query(IDictionary filter)
        {
            Dictionary<string, string> outDict = new Dictionary<string, string>();
            TileDict(outDict, filter);
            return outDict;
        }

        internal static void TileDict(Dictionary<string, string> dicOut, object obj, string parentKey = "")
        {
            if (obj == null)
            {
                return;
            }

            if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
            {
                Dictionary<string, object> dicIn = ((IDictionary)obj).Keys.Cast<string>()
                    .ToDictionary(key => key, key => ((IDictionary)obj)[key]);
                foreach (var keypair in dicIn)
                {
                    string keyName = parentKey + "." + keypair.Key;
                    if (keypair.Value == null)
                    {
                        continue;
                    }

                    TileDict(dicOut, keypair.Value, keyName);
                }
            }
            else if (typeof(Model).IsAssignableFrom(obj.GetType()))
            {
                Dictionary<string, object> dicIn = ((Dictionary<string, object>)((Model)obj).ToMap());
                foreach (var keypair in dicIn)
                {
                    string keyName = parentKey + "." + keypair.Key;
                    if (keypair.Value == null)
                    {
                        continue;
                    }

                    TileDict(dicOut, keypair.Value, keyName);
                }
            }
            else if (typeof(IList).IsAssignableFrom(obj.GetType()) && !typeof(Array).IsAssignableFrom(obj.GetType()))
            {
                int index = 1;
                foreach (var temp in (IList)obj)
                {
                    TileDict(dicOut, temp, parentKey + "." + index.ToSafeString());
                    index++;
                }
            }
            else
            {
                if (obj.GetType() == typeof(byte[]))
                {
                    dicOut.Add(parentKey.TrimStart('.'), Encoding.UTF8.GetString((byte[])obj));

                }
                else
                {
                    dicOut.Add(parentKey.TrimStart('.'), obj.ToSafeString(""));
                }
            }
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get signature according to signedParams, method and secret</para>
        /// </description>
        /// 
        /// <param name="signedParams">
        /// params which need to be signed
        /// </param>
        /// <param name="method">
        /// http method e.g. GET
        /// </param>
        /// <param name="secret">
        /// AccessKeySecret
        /// </param>
        /// 
        /// <returns>
        /// the signature
        /// </returns>
        public static string GetRPCSignature(Dictionary<string, string> signedParams, string method, string secret)
        {
            List<string> sortedKeys = signedParams.Keys.ToList();
            sortedKeys.Sort(StringComparer.Ordinal);
            StringBuilder canonicalizedQueryString = new StringBuilder();

            foreach (string key in sortedKeys)
            {
                if (signedParams[key] != null)
                {
                    canonicalizedQueryString.Append("&")
                        .Append(PercentEncode(key)).Append("=")
                        .Append(PercentEncode(signedParams[key]));
                }
            }

            StringBuilder stringToSign = new StringBuilder();
            stringToSign.Append(method);
            stringToSign.Append(SEPARATOR);
            stringToSign.Append(PercentEncode("/"));
            stringToSign.Append(SEPARATOR);
            stringToSign.Append(PercentEncode(
                canonicalizedQueryString.ToString().Substring(1)));
            System.Diagnostics.Debug.WriteLine("GetRPCSignature:stringToSign is " + stringToSign.ToString());
            byte[] signData;
            using (KeyedHashAlgorithm algorithm = CryptoConfig.CreateFromName("HMACSHA1") as KeyedHashAlgorithm)
            {
                algorithm.Key = Encoding.UTF8.GetBytes(secret + SEPARATOR);
                signData = algorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToSign.ToString().ToCharArray()));
            }

            string signedStr = System.Convert.ToBase64String(signData);
            return signedStr;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Parse array into a string with specified style</para>
        /// </description>
        /// 
        /// <param name="array">
        /// the array
        /// </param>
        /// <param name="prefix">
        /// the prefix string
        /// </param>
        /// 
        /// <returns>
        /// the string
        /// </returns>
        public static string ArrayToStringWithSpecifiedStyle(object array, string prefix, string style)
        {
            if (array == null)
            {
                return string.Empty;
            }

            switch (style.ToLower())
            {
                case "repeatlist":
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    map.Add(prefix, array);
                    return ToForm(map);
                case "simple":
                case "spacedelimited":
                case "pipedelimited":
                    return FlatArray((IList)array, style);
                case "json":
                    return JsonConvert.SerializeObject(ToJsonObject(array));
                default:
                    return string.Empty;
            }
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Parse map with flat style</para>
        /// </description>
        /// 
        /// <param name="any">
        /// the input
        /// </param>
        /// 
        /// <returns>
        /// any
        /// </returns>
        // public static object MapToFlatStyle(object input)
        // {
        //     throw new NotImplementedException();
        // }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Transform input as map.</para>
        /// </description>
        public static Dictionary<string, object> ParseToMap(object input)
        {
            if (input == null)
            {
                return null;
            }

            Type type = input.GetType();
            var map = (Dictionary<string, object>)ModelExtensions.ToMapFactory(type, input);

            return map;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get the authorization</para>
        /// </description>
        /// 
        /// <param name="request">
        /// request params
        /// </param>
        /// <param name="signatureAlgorithm">
        /// the autograph method
        /// </param>
        /// <param name="payload">
        /// the hashed request
        /// </param>
        /// <param name="accesskey">
        /// the accesskey string
        /// </param>
        /// <param name="accessKeySecret">
        /// the accessKeySecret string
        /// </param>
        /// 
        /// <returns>
        /// authorization string
        /// </returns>
        public static string GetAuthorization(Request request, string signatureAlgorithm, string payload, string accesskey, string accessKeySecret)
        {
            string canonicalURI = request.Pathname.ToSafeString("") == ""
                ? "/"
                : request.Pathname.Replace("+", "%20").Replace("*", "%2A").Replace("%7E", "~");
            string method = request.Method;
            string canonicalQueryString = GetAuthorizationQueryString(request.Query);
            Tuple<string, List<string>> tuple = GetAuthorizationHeaders(request.Headers);
            string canonicalheaders = tuple.Item1;
            var signedHeaders = tuple.Item2;

            string canonicalRequest = method + "\n" + canonicalURI + "\n" + canonicalQueryString + "\n" +
                                      canonicalheaders + "\n" +
                                      string.Join(";", signedHeaders) + "\n" + payload;
            byte[] raw = Encoding.UTF8.GetBytes(canonicalRequest);
            string StringToSign = signatureAlgorithm + "\n" + BytesUtils.ToHex(Hash(raw, signatureAlgorithm));
            System.Diagnostics.Debug.WriteLine("GetAuthorization:stringToSign is " + StringToSign);
            var signature = BytesUtils.ToHex(SignatureMethod(accessKeySecret, StringToSign, signatureAlgorithm));
            string auth = signatureAlgorithm + " Credential=" + accesskey + ",SignedHeaders=" +
                          string.Join(";", signedHeaders) + ",Signature=" + signature;

            return auth;
        }

        internal static byte[] SignatureMethod(string secret, string source, string signatureAlgorithm)
        {
            if (signatureAlgorithm == "ACS3-HMAC-SHA256")
            {
                byte[] signData;
                using (KeyedHashAlgorithm algorithm = CryptoConfig.CreateFromName("HMACSHA256") as KeyedHashAlgorithm)
                {
                    algorithm.Key = Encoding.UTF8.GetBytes(secret);
                    signData = algorithm.ComputeHash(Encoding.UTF8.GetBytes(source.ToSafeString().ToCharArray()));
                }

                return signData;
            }

            return null;
        }

        internal static string GetAuthorizationQueryString(Dictionary<string, string> query)
        {
            string canonicalQueryString = string.Empty;
            if (query == null || query.Count <= 0)
            {
                return canonicalQueryString;
            }
            var hs = query.OrderBy(p => p.Key, StringComparer.Ordinal).ToDictionary(p => p.Key, p => p.Value);
            foreach (var keypair in hs)
            {
                if (keypair.Value != null)
                {
                    canonicalQueryString += string.Format("&{0}={1}", keypair.Key, PercentEncode(keypair.Value));
                }
            }

            if (!string.IsNullOrEmpty(canonicalQueryString))
            {
                canonicalQueryString = canonicalQueryString.TrimStart('&');
            }

            return canonicalQueryString;
        }

        internal static Tuple<string, List<string>> GetAuthorizationHeaders(Dictionary<string, string> headers)
        {
            string canonicalheaders = string.Empty;
            var tmp = new Dictionary<string, List<string>>();
            foreach (var keypair in headers)
            {
                var lowerKey = keypair.Key.ToLower();
                if (lowerKey.StartsWith("x-acs-") || lowerKey == "host" || lowerKey == "content-type")
                {
                    if (tmp.ContainsKey(lowerKey))
                    {
                        tmp[lowerKey].Add(keypair.Value.ToSafeString().Trim());
                    }
                    else
                    {
                        tmp[lowerKey] = new List<string> { keypair.Value.ToSafeString().Trim() };
                    }
                }
            }

            var hs = tmp.OrderBy(p => p.Key, StringComparer.Ordinal).ToDictionary(p => p.Key, p => p.Value);

            foreach (var keypair in hs)
            {
                var listSort = new List<string>(keypair.Value);
                listSort.Sort(StringComparer.Ordinal);
                canonicalheaders += string.Format("{0}:{1}\n", keypair.Key, string.Join(", ", listSort));
            }

            return new Tuple<string, List<string>>(canonicalheaders, hs.Keys.ToList());
        }

        public static string GetUserAgent(string userAgent)
        {
            var defaultUserAgent = GetDefaultUserAgent();
            if (!string.IsNullOrEmpty(userAgent))
            {
                return defaultUserAgent + " " + userAgent;
            }
            return defaultUserAgent;
        }


        internal static string GetDefaultUserAgent()
        {
            string OSVersion = Environment.OSVersion.ToString();
            string ClientVersion = GetRuntimeRegexValue(RuntimeEnvironment.GetRuntimeDirectory());
            string CoreVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string fileVersion = GetFileVersion();
            return string.Format("AlibabaCloud ({0}) {1} Core/{2} TeaDSL/2", OSVersion, ClientVersion, fileVersion);
        }
        
        private static string GetFileVersion()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            return string.Format("{0}.{1}.{2}", 
                versionInfo.FileMajorPart, 
                versionInfo.FileMinorPart, 
                versionInfo.FileBuildPart);
        }

        internal static string FlatArray(IList array, string sty)
        {
            List<string> strs = new List<string>();
            for (int i = 0; i < array.Count; i++)
            {
                strs.Add(array[i].ToSafeString());
            }

            if (sty.ToSafeString().ToLower() == "simple".ToLower())
            {
                return string.Join(",", strs);
            }
            else if (sty.ToSafeString().ToLower() == "spaceDelimited".ToLower())
            {
                return string.Join(" ", strs);
            }
            else
            {
                return string.Join("|", strs);
            }
        }

        internal static string GetRuntimeRegexValue(string value)
        {
            var rx = new Regex(@"(\.NET).*(\\|\/).*(\d)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matches = rx.Match(value);
            char[] separator = { '\\', '/' };

            if (matches.Success)
            {
                var clientValueArray = matches.Value.Split(separator);
                return BuildClientVersion(clientValueArray);
            }

            return "RuntimeNotFound";
        }

        internal static string BuildClientVersion(string[] value)
        {
            var finalValue = "";
            for (var i = 0; i < value.Length - 1; ++i)
            {
                finalValue += value[i].Replace(".", "").ToLower();
            }

            finalValue += "/" + value[value.Length - 1];

            return finalValue;
        }
        
        public static List<Dictionary<string, object>> ToArray(object input)
        {
            try
            {
                var listModel = (IList)input;
                var listResult = new List<Dictionary<string, object>>();
                foreach (var model in listModel)
                {
                    if (model != null)
                    {
                        listResult.Add(((Model)model).ToMap());
                    }
                }
                return listResult;
            }
            catch
            {
                return null;
            }
        }
        
        public static string GetEndpointRules(string product, string regionId, string endpointType, string network, string suffix)
        {
            string result;
        
            if (!string.IsNullOrEmpty(network) && network != "public")
            {
                network = "-" + network;
            }
            else
            {
                network = "";
            }

            suffix = suffix ?? "";
            if (suffix.Length > 0)
            {
                suffix = "-" + suffix;
            }

            if (endpointType == "regional")
            {
                if (string.IsNullOrEmpty(regionId))
                {
                    throw new ArgumentException("RegionId is empty, please set a valid RegionId");
                }
                result = string.Format("{0}{1}{2}.{3}.aliyuncs.com", product, suffix, network, regionId);
            }
            else
            {
                result = string.Format("{0}{1}{2}.aliyuncs.com", product, suffix, network);
            }

            return result;
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Transform a map to a flat style map where keys are prefixed with length info.
        /// Map keys are transformed from "key" to "#length#key" format.</para>
        /// </description>
        /// 
        /// <param name="obj">
        /// the input object (can be a TeaModel, List, Dictionary, or other types)
        /// </param>
        /// 
        /// <returns>
        /// the transformed object
        /// </returns>
        public static object MapToFlatStyle(object obj)
        {
            if (obj == null)
            {
                return obj;
            }

            if (typeof(IList).IsAssignableFrom(obj.GetType()) && !typeof(Array).IsAssignableFrom(obj.GetType()))
            {
                List<object> list = new List<object>();
                foreach (var item in (IList)obj)
                {
                    list.Add(MapToFlatStyle(item));
                }
                return list;
            }
            else if (typeof(TeaModel).IsAssignableFrom(obj.GetType()))
            {
                // Modify the original TeaModel object's fields
                TeaModel model = (TeaModel)obj;
                FieldInfo[] fields = model.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                
                foreach (FieldInfo field in fields)
                {
                    object value = field.GetValue(model);
                    
                    if (value != null && typeof(IDictionary).IsAssignableFrom(value.GetType()))
                    {
                        // This is a dictionary, apply flat style to keys
                        Dictionary<string, object> flatMap = new Dictionary<string, object>();
                        Dictionary<string, object> valueDict = ((IDictionary)value).Keys.Cast<string>()
                            .ToDictionary(key => key, key => ((IDictionary)value)[key]);
                        foreach (var entry in valueDict)
                        {
                            flatMap.Add(string.Format("#{0}#{1}", entry.Key.Length, entry.Key), MapToFlatStyle(entry.Value));
                        }
                        field.SetValue(model, flatMap);
                    }
                    else
                    {
                        // Recursively process other fields
                        field.SetValue(model, MapToFlatStyle(value));
                    }
                }
                return model;  // Return the modified original TeaModel
            }
            else if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
            {
                Dictionary<string, object> flatMap = new Dictionary<string, object>();
                Dictionary<string, object> dicIn = ((IDictionary)obj).Keys.Cast<string>()
                    .ToDictionary(key => key, key => ((IDictionary)obj)[key]);
                foreach (var keypair in dicIn)
                {
                    flatMap.Add(string.Format("#{0}#{1}", keypair.Key.Length, keypair.Key), MapToFlatStyle(keypair.Value));
                }
                return flatMap;
            }

            return obj;
        }
    }
}

