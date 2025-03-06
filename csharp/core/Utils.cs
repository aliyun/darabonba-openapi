using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Darabonba;
using Darabonba.Utils;

namespace AlibabaCloud.OpenApiClient
{
    public class Utils 
    {

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
        public static void Convert(DaraModel body, DaraModel content)
        {
            throw new NotImplementedException();
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
        public static string GetEndpoint(string endpoint, bool? serverUse, string endpointType)
        {
            throw new NotImplementedException();
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
        public static long? GetThrottlingTimeLeft(Dictionary<string, string> headers)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
        public static Dictionary<string, string> FlatMap(Dictionary<string, object> params_, string prefix)
        {
            throw new NotImplementedException();
        }

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
            throw new NotImplementedException();
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
        public static string GetStringToSign(DaraRequest request)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
        public static Dictionary<string, string> Query(Dictionary<string, object> filter)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Stringify the value of map</para>
        /// </description>
        /// 
        /// <returns>
        /// the new stringified map
        /// </returns>
        public static Dictionary<string, string> StringifyMapValue(Dictionary<string, object> m)
        {
            throw new NotImplementedException();
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Transform input as array.</para>
        /// </description>
        public static List<Dictionary<string, object>> ToArray(object input)
        {
            throw new NotImplementedException();
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
        public static object MapToFlatStyle(object input)
        {
            throw new NotImplementedException();
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Transform input as map.</para>
        /// </description>
        public static Dictionary<string, object> ParseToMap(object input)
        {
            throw new NotImplementedException();
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
        public static string GetAuthorization(DaraRequest request, string signatureAlgorithm, string payload, string accesskey, string accessKeySecret)
        {
            throw new NotImplementedException();
        }

        public static string GetUserAgent(string userAgent)
        {
            throw new NotImplementedException();
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>Get endpoint according to productId, regionId, endpointType, network and suffix</para>
        /// </description>
        /// 
        /// <returns>
        /// endpoint
        /// </returns>
        public static string GetEndpointRules(string product, string regionId, string endpointType, string network, string suffix)
        {
            throw new NotImplementedException();
        }

    }
}

