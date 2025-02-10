// This file is auto-generated, don't edit it. Thanks.

using System;
using System.Collections.Generic;
using System.IO;

using Tea;

namespace AlibabaCloud.OpenApiClient.Models
{
    /// <term><b>Description:</b></term>
    /// <description>
    /// <para>Model for initing client</para>
    /// </description>
    public class Config : TeaModel {
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
        /// 
        /// <b>Example:</b>
        /// <para>a.txt</para>
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
        public Aliyun.Credentials.Client Credential { get; set; }

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
        /// <para>TLS Minimum Version</para>
        /// 
        /// <b>Example:</b>
        /// <para>TLSv1, TLSv1.1, TLSv1.2, TLSv1.3</para>
        /// </summary>
        [NameInMap("tlsMinVersion")]
        [Validation(Required=false)]
        public string TlsMinVersion { get; set; }

    }

}
