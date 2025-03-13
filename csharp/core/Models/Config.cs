using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Darabonba;
using CredentialClient = Aliyun.Credentials.Client;
using Darabonba.RetryPolicy;

namespace AlibabaCloud.OpenApiClient.Models
{
    /// <term><b>Description:</b></term>
    /// <description>
    /// <para>Model for initing client</para>
    /// </description>
    public class Config : Model {
        /// <summary>
        /// <para>accesskey id</para>
        /// </summary>
        [NameInMap("accessKeyId")]
        [Validation(Required=false)]
        public string AccessKeyId { get; set; }

        /// <summary>
        /// <para>accesskey secret</para>
        /// </summary>
        [NameInMap("accessKeySecret")]
        [Validation(Required=false)]
        public string AccessKeySecret { get; set; }

        /// <summary>
        /// <para>security token</para>
        /// </summary>
        [NameInMap("securityToken")]
        [Validation(Required=false)]
        public string SecurityToken { get; set; }

        /// <summary>
        /// <para>bearer token</para>
        /// 
        /// <b>Example:</b>
        /// <para>the-bearer-token</para>
        /// </summary>
        [NameInMap("bearerToken")]
        [Validation(Required=false)]
        public string BearerToken { get; set; }

        /// <summary>
        /// <para>http protocol</para>
        /// 
        /// <b>Example:</b>
        /// <para>http</para>
        /// </summary>
        [NameInMap("protocol")]
        [Validation(Required=false)]
        public string Protocol { get; set; }

        /// <summary>
        /// <para>http method</para>
        /// 
        /// <b>Example:</b>
        /// <para>GET</para>
        /// </summary>
        [NameInMap("method")]
        [Validation(Required=false)]
        public string Method { get; set; }

        /// <summary>
        /// <para>region id</para>
        /// 
        /// <b>Example:</b>
        /// <para>cn-hangzhou</para>
        /// </summary>
        [NameInMap("regionId")]
        [Validation(Required=false)]
        public string RegionId { get; set; }

        /// <summary>
        /// <para>read timeout</para>
        /// 
        /// <b>Example:</b>
        /// <para>10</para>
        /// </summary>
        [NameInMap("readTimeout")]
        [Validation(Required=false)]
        public int? ReadTimeout { get; set; }

        /// <summary>
        /// <para>connect timeout</para>
        /// 
        /// <b>Example:</b>
        /// <para>10</para>
        /// </summary>
        [NameInMap("connectTimeout")]
        [Validation(Required=false)]
        public int? ConnectTimeout { get; set; }

        /// <summary>
        /// <para>http proxy</para>
        /// 
        /// <b>Example:</b>
        /// <para><a href="http://localhost">http://localhost</a></para>
        /// </summary>
        [NameInMap("httpProxy")]
        [Validation(Required=false)]
        public string HttpProxy { get; set; }

        /// <summary>
        /// <para>https proxy</para>
        /// 
        /// <b>Example:</b>
        /// <para><a href="https://localhost">https://localhost</a></para>
        /// </summary>
        [NameInMap("httpsProxy")]
        [Validation(Required=false)]
        public string HttpsProxy { get; set; }

        /// <summary>
        /// <para>credential</para>
        /// </summary>
        [NameInMap("credential")]
        [Validation(Required=false)]
        public CredentialClient Credential { get; set; }

        /// <summary>
        /// <para>endpoint</para>
        /// 
        /// <b>Example:</b>
        /// <para>cs.aliyuncs.com</para>
        /// </summary>
        [NameInMap("endpoint")]
        [Validation(Required=false)]
        public string Endpoint { get; set; }

        /// <summary>
        /// <para>proxy white list</para>
        /// 
        /// <b>Example:</b>
        /// <para><a href="http://localhost">http://localhost</a></para>
        /// </summary>
        [NameInMap("noProxy")]
        [Validation(Required=false)]
        public string NoProxy { get; set; }

        /// <summary>
        /// <para>max idle conns</para>
        /// 
        /// <b>Example:</b>
        /// <para>3</para>
        /// </summary>
        [NameInMap("maxIdleConns")]
        [Validation(Required=false)]
        public int? MaxIdleConns { get; set; }

        /// <summary>
        /// <para>network for endpoint</para>
        /// 
        /// <b>Example:</b>
        /// <para>public</para>
        /// </summary>
        [NameInMap("network")]
        [Validation(Required=false)]
        public string Network { get; set; }

        /// <summary>
        /// <para>user agent</para>
        /// 
        /// <b>Example:</b>
        /// <para>Alibabacloud/1</para>
        /// </summary>
        [NameInMap("userAgent")]
        [Validation(Required=false)]
        public string UserAgent { get; set; }

        /// <summary>
        /// <para>suffix for endpoint</para>
        /// 
        /// <b>Example:</b>
        /// <para>aliyun</para>
        /// </summary>
        [NameInMap("suffix")]
        [Validation(Required=false)]
        public string Suffix { get; set; }

        /// <summary>
        /// <para>socks5 proxy</para>
        /// </summary>
        [NameInMap("socks5Proxy")]
        [Validation(Required=false)]
        public string Socks5Proxy { get; set; }

        /// <summary>
        /// <para>socks5 network</para>
        /// 
        /// <b>Example:</b>
        /// <para>TCP</para>
        /// </summary>
        [NameInMap("socks5NetWork")]
        [Validation(Required=false)]
        public string Socks5NetWork { get; set; }

        /// <summary>
        /// <para>endpoint type</para>
        /// 
        /// <b>Example:</b>
        /// <para>internal</para>
        /// </summary>
        [NameInMap("endpointType")]
        [Validation(Required=false)]
        public string EndpointType { get; set; }

        /// <summary>
        /// <para>OpenPlatform endpoint</para>
        /// 
        /// <b>Example:</b>
        /// <para>openplatform.aliyuncs.com</para>
        /// </summary>
        [NameInMap("openPlatformEndpoint")]
        [Validation(Required=false)]
        public string OpenPlatformEndpoint { get; set; }

        /// <term><b>Obsolete</b></term>
        /// 
        /// <summary>
        /// <para>credential type</para>
        /// 
        /// <b>Example:</b>
        /// <para>access_key</para>
        /// </summary>
        [NameInMap("type")]
        [Validation(Required=false)]
        [Obsolete]
        public string Type { get; set; }

        /// <summary>
        /// <para>Signature Version</para>
        /// 
        /// <b>Example:</b>
        /// <para>v1</para>
        /// </summary>
        [NameInMap("signatureVersion")]
        [Validation(Required=false)]
        public string SignatureVersion { get; set; }

        /// <summary>
        /// <para>Signature Algorithm</para>
        /// 
        /// <b>Example:</b>
        /// <para>ACS3-HMAC-SHA256</para>
        /// </summary>
        [NameInMap("signatureAlgorithm")]
        [Validation(Required=false)]
        public string SignatureAlgorithm { get; set; }

        /// <summary>
        /// <para>Global Parameters</para>
        /// </summary>
        [NameInMap("globalParameters")]
        [Validation(Required=false)]
        public GlobalParameters GlobalParameters { get; set; }

        /// <summary>
        /// <para>privite key for client certificate</para>
        /// 
        /// <b>Example:</b>
        /// <para>MIIEvQ</para>
        /// </summary>
        [NameInMap("key")]
        [Validation(Required=false)]
        public string Key { get; set; }

        /// <summary>
        /// <para>client certificate</para>
        /// 
        /// <b>Example:</b>
        /// <para>-----BEGIN CERTIFICATE-----
        /// xxx-----END CERTIFICATE-----</para>
        /// </summary>
        [NameInMap("cert")]
        [Validation(Required=false)]
        public string Cert { get; set; }

        /// <summary>
        /// <para>server certificate</para>
        /// 
        /// <b>Example:</b>
        /// <para>-----BEGIN CERTIFICATE-----
        /// xxx-----END CERTIFICATE-----</para>
        /// </summary>
        [NameInMap("ca")]
        [Validation(Required=false)]
        public string Ca { get; set; }

        /// <summary>
        /// <para>disable HTTP/2</para>
        /// 
        /// <b>Example:</b>
        /// <para>false</para>
        /// </summary>
        [NameInMap("disableHttp2")]
        [Validation(Required=false)]
        public bool? DisableHttp2 { get; set; }

        /// <summary>
        /// <para>retry options</para>
        /// </summary>
        [NameInMap("retryOptions")]
        [Validation(Required=false)]
        public RetryOptions RetryOptions { get; set; }

        public Config Copy()
        {
            Config copy = FromMap(ToMap());
            return copy;
        }

        public Config CopyWithoutStream()
        {
            Config copy = FromMap(ToMap(true));
            return copy;
        }

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            var map = new Dictionary<string, object>();
            if (AccessKeyId != null)
            {
                map["accessKeyId"] = AccessKeyId;
            }

            if (AccessKeySecret != null)
            {
                map["accessKeySecret"] = AccessKeySecret;
            }

            if (SecurityToken != null)
            {
                map["securityToken"] = SecurityToken;
            }

            if (BearerToken != null)
            {
                map["bearerToken"] = BearerToken;
            }

            if (Protocol != null)
            {
                map["protocol"] = Protocol;
            }

            if (Method != null)
            {
                map["method"] = Method;
            }

            if (RegionId != null)
            {
                map["regionId"] = RegionId;
            }

            if (ReadTimeout != null)
            {
                map["readTimeout"] = ReadTimeout;
            }

            if (ConnectTimeout != null)
            {
                map["connectTimeout"] = ConnectTimeout;
            }

            if (HttpProxy != null)
            {
                map["httpProxy"] = HttpProxy;
            }

            if (HttpsProxy != null)
            {
                map["httpsProxy"] = HttpsProxy;
            }

            if (Credential != null)
            {
                map["credential"] = Credential;
            }

            if (Endpoint != null)
            {
                map["endpoint"] = Endpoint;
            }

            if (NoProxy != null)
            {
                map["noProxy"] = NoProxy;
            }

            if (MaxIdleConns != null)
            {
                map["maxIdleConns"] = MaxIdleConns;
            }

            if (Network != null)
            {
                map["network"] = Network;
            }

            if (UserAgent != null)
            {
                map["userAgent"] = UserAgent;
            }

            if (Suffix != null)
            {
                map["suffix"] = Suffix;
            }

            if (Socks5Proxy != null)
            {
                map["socks5Proxy"] = Socks5Proxy;
            }

            if (Socks5NetWork != null)
            {
                map["socks5NetWork"] = Socks5NetWork;
            }

            if (EndpointType != null)
            {
                map["endpointType"] = EndpointType;
            }

            if (OpenPlatformEndpoint != null)
            {
                map["openPlatformEndpoint"] = OpenPlatformEndpoint;
            }

            if (Type != null)
            {
                map["type"] = Type;
            }

            if (SignatureVersion != null)
            {
                map["signatureVersion"] = SignatureVersion;
            }

            if (SignatureAlgorithm != null)
            {
                map["signatureAlgorithm"] = SignatureAlgorithm;
            }

            if (GlobalParameters != null)
            {
                map["globalParameters"] = GlobalParameters != null ? GlobalParameters.ToMap(noStream) : null;
            }

            if (Key != null)
            {
                map["key"] = Key;
            }

            if (Cert != null)
            {
                map["cert"] = Cert;
            }

            if (Ca != null)
            {
                map["ca"] = Ca;
            }

            if (DisableHttp2 != null)
            {
                map["disableHttp2"] = DisableHttp2;
            }

            if (RetryOptions != null)
            {
                map["retryOptions"] = RetryOptions != null ? RetryOptions.ToMap(noStream) : null;
            }

            return map;
        }

        public static Config FromMap(Dictionary<string, object> map)
        {
            var model = new Config();
            if (map.ContainsKey("accessKeyId"))
            {
                model.AccessKeyId = (string)map["accessKeyId"];
            }

            if (map.ContainsKey("accessKeySecret"))
            {
                model.AccessKeySecret = (string)map["accessKeySecret"];
            }

            if (map.ContainsKey("securityToken"))
            {
                model.SecurityToken = (string)map["securityToken"];
            }

            if (map.ContainsKey("bearerToken"))
            {
                model.BearerToken = (string)map["bearerToken"];
            }

            if (map.ContainsKey("protocol"))
            {
                model.Protocol = (string)map["protocol"];
            }

            if (map.ContainsKey("method"))
            {
                model.Method = (string)map["method"];
            }

            if (map.ContainsKey("regionId"))
            {
                model.RegionId = (string)map["regionId"];
            }

            if (map.ContainsKey("readTimeout"))
            {
                model.ReadTimeout = (int?)map["readTimeout"];
            }

            if (map.ContainsKey("connectTimeout"))
            {
                model.ConnectTimeout = (int?)map["connectTimeout"];
            }

            if (map.ContainsKey("httpProxy"))
            {
                model.HttpProxy = (string)map["httpProxy"];
            }

            if (map.ContainsKey("httpsProxy"))
            {
                model.HttpsProxy = (string)map["httpsProxy"];
            }

            if (map.ContainsKey("credential"))
            {
                model.Credential = (CredentialClient)map["credential"];
            }

            if (map.ContainsKey("endpoint"))
            {
                model.Endpoint = (string)map["endpoint"];
            }

            if (map.ContainsKey("noProxy"))
            {
                model.NoProxy = (string)map["noProxy"];
            }

            if (map.ContainsKey("maxIdleConns"))
            {
                model.MaxIdleConns = (int?)map["maxIdleConns"];
            }

            if (map.ContainsKey("network"))
            {
                model.Network = (string)map["network"];
            }

            if (map.ContainsKey("userAgent"))
            {
                model.UserAgent = (string)map["userAgent"];
            }

            if (map.ContainsKey("suffix"))
            {
                model.Suffix = (string)map["suffix"];
            }

            if (map.ContainsKey("socks5Proxy"))
            {
                model.Socks5Proxy = (string)map["socks5Proxy"];
            }

            if (map.ContainsKey("socks5NetWork"))
            {
                model.Socks5NetWork = (string)map["socks5NetWork"];
            }

            if (map.ContainsKey("endpointType"))
            {
                model.EndpointType = (string)map["endpointType"];
            }

            if (map.ContainsKey("openPlatformEndpoint"))
            {
                model.OpenPlatformEndpoint = (string)map["openPlatformEndpoint"];
            }

            if (map.ContainsKey("type"))
            {
                model.Type = (string)map["type"];
            }

            if (map.ContainsKey("signatureVersion"))
            {
                model.SignatureVersion = (string)map["signatureVersion"];
            }

            if (map.ContainsKey("signatureAlgorithm"))
            {
                model.SignatureAlgorithm = (string)map["signatureAlgorithm"];
            }

            if (map.ContainsKey("globalParameters"))
            {
                var temp = (Dictionary<string, object>)map["globalParameters"];
                model.GlobalParameters = GlobalParameters.FromMap(temp);
            }

            if (map.ContainsKey("key"))
            {
                model.Key = (string)map["key"];
            }

            if (map.ContainsKey("cert"))
            {
                model.Cert = (string)map["cert"];
            }

            if (map.ContainsKey("ca"))
            {
                model.Ca = (string)map["ca"];
            }

            if (map.ContainsKey("disableHttp2"))
            {
                model.DisableHttp2 = (bool?)map["disableHttp2"];
            }

            if (map.ContainsKey("retryOptions"))
            {
                var temp = (Dictionary<string, object>)map["retryOptions"];
                model.RetryOptions = RetryOptions.FromMap(temp);
            }

            return model;
        }
    }

}

