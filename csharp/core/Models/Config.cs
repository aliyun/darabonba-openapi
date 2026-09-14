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
    public class Config : Darabonba.Model  {
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

        /// <summary>
        /// <para>TLS Minimum Version</para>
        ///
        /// <b>Example:</b>
        /// <para>TLSv1, TLSv1.1, TLSv1.2, TLSv1.3</para>
        /// </summary>
        [NameInMap("tlsMinVersion")]
        [Validation(Required=false)]
        public string TlsMinVersion { get; set; }

        public override Dictionary<string, object> ToMapCore(bool noStream = false)
        {
            var map = new Dictionary<string, object>();
            map["accessKeyId"] = this.AccessKeyId;
            map["accessKeySecret"] = this.AccessKeySecret;
            map["securityToken"] = this.SecurityToken;
            map["bearerToken"] = this.BearerToken;
            map["protocol"] = this.Protocol;
            map["method"] = this.Method;
            map["regionId"] = this.RegionId;
            map["readTimeout"] = this.ReadTimeout;
            map["connectTimeout"] = this.ConnectTimeout;
            map["httpProxy"] = this.HttpProxy;
            map["httpsProxy"] = this.HttpsProxy;
            map["credential"] = this.Credential;
            map["endpoint"] = this.Endpoint;
            map["noProxy"] = this.NoProxy;
            map["maxIdleConns"] = this.MaxIdleConns;
            map["network"] = this.Network;
            map["userAgent"] = this.UserAgent;
            map["suffix"] = this.Suffix;
            map["socks5Proxy"] = this.Socks5Proxy;
            map["socks5NetWork"] = this.Socks5NetWork;
            map["endpointType"] = this.EndpointType;
            map["openPlatformEndpoint"] = this.OpenPlatformEndpoint;
            map["type"] = this.Type;
            map["signatureVersion"] = this.SignatureVersion;
            map["signatureAlgorithm"] = this.SignatureAlgorithm;
            map["globalParameters"] = this.GlobalParameters == null ? null : this.GlobalParameters.ToMap();
            map["key"] = this.Key;
            map["cert"] = this.Cert;
            map["ca"] = this.Ca;
            map["disableHttp2"] = this.DisableHttp2;
            map["retryOptions"] = this.RetryOptions == null ? null : this.RetryOptions.ToMap();
            map["tlsMinVersion"] = this.TlsMinVersion;
            return map;
        }

        public new static Config FromMap(IDictionary map)
        {
            if (map == null)
            {
                return null;
            }
            var model = new Config();
            if (map.Contains("accessKeyId"))
            {
                model.AccessKeyId = (string)map["accessKeyId"];
            }
            if (map.Contains("accessKeySecret"))
            {
                model.AccessKeySecret = (string)map["accessKeySecret"];
            }
            if (map.Contains("securityToken"))
            {
                model.SecurityToken = (string)map["securityToken"];
            }
            if (map.Contains("bearerToken"))
            {
                model.BearerToken = (string)map["bearerToken"];
            }
            if (map.Contains("protocol"))
            {
                model.Protocol = (string)map["protocol"];
            }
            if (map.Contains("method"))
            {
                model.Method = (string)map["method"];
            }
            if (map.Contains("regionId"))
            {
                model.RegionId = (string)map["regionId"];
            }
            if (map.Contains("readTimeout"))
            {
                model.ReadTimeout = map["readTimeout"] == null ? (int?)null : System.Convert.ToInt32(map["readTimeout"]);
            }
            if (map.Contains("connectTimeout"))
            {
                model.ConnectTimeout = map["connectTimeout"] == null ? (int?)null : System.Convert.ToInt32(map["connectTimeout"]);
            }
            if (map.Contains("httpProxy"))
            {
                model.HttpProxy = (string)map["httpProxy"];
            }
            if (map.Contains("httpsProxy"))
            {
                model.HttpsProxy = (string)map["httpsProxy"];
            }
            if (map.Contains("credential"))
            {
                model.Credential = (CredentialClient)map["credential"];
            }
            if (map.Contains("endpoint"))
            {
                model.Endpoint = (string)map["endpoint"];
            }
            if (map.Contains("noProxy"))
            {
                model.NoProxy = (string)map["noProxy"];
            }
            if (map.Contains("maxIdleConns"))
            {
                model.MaxIdleConns = map["maxIdleConns"] == null ? (int?)null : System.Convert.ToInt32(map["maxIdleConns"]);
            }
            if (map.Contains("network"))
            {
                model.Network = (string)map["network"];
            }
            if (map.Contains("userAgent"))
            {
                model.UserAgent = (string)map["userAgent"];
            }
            if (map.Contains("suffix"))
            {
                model.Suffix = (string)map["suffix"];
            }
            if (map.Contains("socks5Proxy"))
            {
                model.Socks5Proxy = (string)map["socks5Proxy"];
            }
            if (map.Contains("socks5NetWork"))
            {
                model.Socks5NetWork = (string)map["socks5NetWork"];
            }
            if (map.Contains("endpointType"))
            {
                model.EndpointType = (string)map["endpointType"];
            }
            if (map.Contains("openPlatformEndpoint"))
            {
                model.OpenPlatformEndpoint = (string)map["openPlatformEndpoint"];
            }
            if (map.Contains("type"))
            {
                model.Type = (string)map["type"];
            }
            if (map.Contains("signatureVersion"))
            {
                model.SignatureVersion = (string)map["signatureVersion"];
            }
            if (map.Contains("signatureAlgorithm"))
            {
                model.SignatureAlgorithm = (string)map["signatureAlgorithm"];
            }
            if (map.Contains("globalParameters"))
            {
                model.GlobalParameters = map["globalParameters"] == null ? null : AlibabaCloud.OpenApiClient.Models.GlobalParameters.FromMap((IDictionary)map["globalParameters"]);
            }
            if (map.Contains("key"))
            {
                model.Key = (string)map["key"];
            }
            if (map.Contains("cert"))
            {
                model.Cert = (string)map["cert"];
            }
            if (map.Contains("ca"))
            {
                model.Ca = (string)map["ca"];
            }
            if (map.Contains("disableHttp2"))
            {
                model.DisableHttp2 = map["disableHttp2"] == null ? (bool?)null : System.Convert.ToBoolean(map["disableHttp2"]);
            }
            if (map.Contains("retryOptions"))
            {
                model.RetryOptions = map["retryOptions"] == null ? null : Darabonba.RetryPolicy.RetryOptions.FromMap((IDictionary)map["retryOptions"]);
            }
            if (map.Contains("tlsMinVersion"))
            {
                model.TlsMinVersion = (string)map["tlsMinVersion"];
            }
            return model;
        }
    }
}
