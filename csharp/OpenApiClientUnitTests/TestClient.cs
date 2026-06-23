using AlibabaCloud.OpenApiClient.Models;

namespace OpenApiClientUnitTests
{
    /// <summary>
    /// Exposes protected members for unit tests.
    /// </summary>
    public class TestClient : AlibabaCloud.OpenApiClient.Client
    {
        public TestClient(Config config) : base(config)
        {
        }

        public string ProductId
        {
            get => _productId;
            set => _productId = value;
        }

        public string Endpoint => _endpoint;
        public string EndpointType => _endpointType;
        public string Network => _network;
        public string Suffix => _suffix;
        public string Protocol => _protocol;
        public string Method => _method;
        public string RegionId => _regionId;
        public string UserAgent => _userAgent;
        public int? ReadTimeout => _readTimeout;
        public int? ConnectTimeout => _connectTimeout;
        public string HttpProxy => _httpProxy;
        public string HttpsProxy => _httpsProxy;
        public string NoProxy => _noProxy;
        public string Socks5Proxy => _socks5Proxy;
        public string Socks5NetWork => _socks5NetWork;
        public int? MaxIdleConns => _maxIdleConns;
        public string SignatureVersion => _signatureVersion;
        public string SignatureAlgorithm => _signatureAlgorithm;
        public GlobalParameters GlobalParameters => _globalParameters;
        public string Key => _key;
        public string Cert => _cert;
        public string Ca => _ca;
        public bool? DisableHttp2 => _disableHttp2;
    }
}
