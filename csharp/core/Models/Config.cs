// This file is auto-generated, don't edit it. Thanks.

using System;
using System.Collections.Generic;
using System.IO;

using Tea;

namespace AlibabaCloud.OpenApiClient.Models
{
    /**
     * Model for initing client
     */
    public class Config : TeaModel {
        /// <summary>
        /// accesskey id
        /// </summary>
        [NameInMap("accessKeyId")]
        [Validation(Required=false)]
        public string AccessKeyId { get; set; }

        /// <summary>
        /// accesskey secret
        /// </summary>
        [NameInMap("accessKeySecret")]
        [Validation(Required=false)]
        public string AccessKeySecret { get; set; }

        /// <summary>
        /// security token
        /// </summary>
        [NameInMap("securityToken")]
        [Validation(Required=false)]
        public string SecurityToken { get; set; }

        /// <summary>
        /// bearer token
        /// </summary>
        [NameInMap("bearerToken")]
        [Validation(Required=false)]
        public string BearerToken { get; set; }

        /// <summary>
        /// http protocol
        /// </summary>
        [NameInMap("protocol")]
        [Validation(Required=false)]
        public string Protocol { get; set; }

        /// <summary>
        /// http method
        /// </summary>
        [NameInMap("method")]
        [Validation(Required=false)]
        public string Method { get; set; }

        /// <summary>
        /// region id
        /// </summary>
        [NameInMap("regionId")]
        [Validation(Required=false)]
        public string RegionId { get; set; }

        /// <summary>
        /// read timeout
        /// </summary>
        [NameInMap("readTimeout")]
        [Validation(Required=false)]
        public int? ReadTimeout { get; set; }

        /// <summary>
        /// connect timeout
        /// </summary>
        [NameInMap("connectTimeout")]
        [Validation(Required=false)]
        public int? ConnectTimeout { get; set; }

        /// <summary>
        /// http proxy
        /// </summary>
        [NameInMap("httpProxy")]
        [Validation(Required=false)]
        public string HttpProxy { get; set; }

        /// <summary>
        /// https proxy
        /// </summary>
        [NameInMap("httpsProxy")]
        [Validation(Required=false)]
        public string HttpsProxy { get; set; }

        /// <summary>
        /// credential
        /// </summary>
        [NameInMap("credential")]
        [Validation(Required=false)]
        public Aliyun.Credentials.Client Credential { get; set; }

        /// <summary>
        /// endpoint
        /// </summary>
        [NameInMap("endpoint")]
        [Validation(Required=false)]
        public string Endpoint { get; set; }

        /// <summary>
        /// proxy white list
        /// </summary>
        [NameInMap("noProxy")]
        [Validation(Required=false)]
        public string NoProxy { get; set; }

        /// <summary>
        /// max idle conns
        /// </summary>
        [NameInMap("maxIdleConns")]
        [Validation(Required=false)]
        public int? MaxIdleConns { get; set; }

        /// <summary>
        /// network for endpoint
        /// </summary>
        [NameInMap("network")]
        [Validation(Required=false)]
        public string Network { get; set; }

        /// <summary>
        /// user agent
        /// </summary>
        [NameInMap("userAgent")]
        [Validation(Required=false)]
        public string UserAgent { get; set; }

        /// <summary>
        /// suffix for endpoint
        /// </summary>
        [NameInMap("suffix")]
        [Validation(Required=false)]
        public string Suffix { get; set; }

        /// <summary>
        /// socks5 proxy
        /// </summary>
        [NameInMap("socks5Proxy")]
        [Validation(Required=false)]
        public string Socks5Proxy { get; set; }

        /// <summary>
        /// socks5 network
        /// </summary>
        [NameInMap("socks5NetWork")]
        [Validation(Required=false)]
        public string Socks5NetWork { get; set; }

        /// <summary>
        /// endpoint type
        /// </summary>
        [NameInMap("endpointType")]
        [Validation(Required=false)]
        public string EndpointType { get; set; }

        /// <summary>
        /// OpenPlatform endpoint
        /// </summary>
        [NameInMap("openPlatformEndpoint")]
        [Validation(Required=false)]
        public string OpenPlatformEndpoint { get; set; }

        /// <summary>
        /// credential type
        /// </summary>
        [NameInMap("type")]
        [Validation(Required=false)]
        [Obsolete]
        public string Type { get; set; }

        /// <summary>
        /// Signature Version
        /// </summary>
        [NameInMap("signatureVersion")]
        [Validation(Required=false)]
        public string SignatureVersion { get; set; }

        /// <summary>
        /// Signature Algorithm
        /// </summary>
        [NameInMap("signatureAlgorithm")]
        [Validation(Required=false)]
        public string SignatureAlgorithm { get; set; }

        /// <summary>
        /// Global Parameters
        /// </summary>
        [NameInMap("globalParameters")]
        [Validation(Required=false)]
        public GlobalParameters GlobalParameters { get; set; }

        /// <summary>
        /// privite key for client certificate
        /// </summary>
        [NameInMap("key")]
        [Validation(Required=false)]
        public string Key { get; set; }

        /// <summary>
        /// client certificate
        /// </summary>
        [NameInMap("cert")]
        [Validation(Required=false)]
        public string Cert { get; set; }

        /// <summary>
        /// server certificate
        /// </summary>
        [NameInMap("ca")]
        [Validation(Required=false)]
        public string Ca { get; set; }

        /// <summary>
        /// disable HTTP/2
        /// </summary>
        [NameInMap("disableHttp2")]
        [Validation(Required=false)]
        public bool? DisableHttp2 { get; set; }

    }

}
