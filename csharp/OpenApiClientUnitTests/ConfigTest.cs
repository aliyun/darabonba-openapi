using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AlibabaCloud.GatewaySpi.Models;
using AlibabaCloud.OpenApiClient.Exceptions;
using AlibabaCloud.OpenApiClient.Models;
using Xunit;

namespace OpenApiClientUnitTests
{
    public class ConfigTest
    {
        [Fact]
        public void TestConfig()
        {
            var globalParameters = new GlobalParameters
            {
                Headers = new Dictionary<string, string> { { "global-key", "global-value" } },
                Queries = new Dictionary<string, string> { { "global-query", "global-value" } }
            };

            var config = new Config
            {
                Endpoint = "config.endpoint",
                EndpointType = "public",
                Network = "config.network",
                Suffix = "config.suffix",
                Protocol = "config.protocol",
                Method = "config.method",
                RegionId = "config.regionId",
                UserAgent = "config.userAgent",
                ReadTimeout = 3000,
                ConnectTimeout = 3000,
                HttpProxy = "config.httpProxy",
                HttpsProxy = "config.httpsProxy",
                NoProxy = "config.noProxy",
                Socks5Proxy = "config.socks5Proxy",
                Socks5NetWork = "config.socks5NetWork",
                MaxIdleConns = 128,
                SignatureVersion = "config.signatureVersion",
                SignatureAlgorithm = "config.signatureAlgorithm",
                GlobalParameters = globalParameters,
                Key = "config.key",
                Cert = "config.cert",
                Ca = "config.ca",
                DisableHttp2 = true,
                AccessKeyId = "accessKeyId",
                AccessKeySecret = "accessKeySecret",
                SecurityToken = "securityToken",
                Type = "sts"
            };

            var client = new TestClient(config);
            Assert.Equal("accessKeyId", client.GetAccessKeyId());
            Assert.Equal("accessKeySecret", client.GetAccessKeySecret());
            Assert.Equal("securityToken", client.GetSecurityToken());
            Assert.Equal("sts", client.GetType());

            config = new Config
            {
                BearerToken = "token",
                Type = "bearer",
                Endpoint = "config.endpoint",
                GlobalParameters = globalParameters
            };
            client = new TestClient(config);
            Assert.True(string.IsNullOrEmpty(client.GetAccessKeyId()));
            Assert.True(string.IsNullOrEmpty(client.GetAccessKeySecret()));
            Assert.True(string.IsNullOrEmpty(client.GetSecurityToken()));
            Assert.Equal("token", client.GetBearerToken());
            Assert.Equal("bearer", client.GetType());

            config = new Config
            {
                AccessKeyId = "ak",
                AccessKeySecret = "secret",
                SecurityToken = "token",
                Type = "sts",
                Endpoint = "config.endpoint",
                GlobalParameters = globalParameters
            };
            client = new TestClient(config);
            Assert.Equal("ak", client.GetAccessKeyId());
            Assert.Equal("secret", client.GetAccessKeySecret());
            Assert.Equal("token", client.GetSecurityToken());
            Assert.Equal("sts", client.GetType());

            config = new Config
            {
                BearerToken = "token",
                Type = "bearer",
                Endpoint = "config.endpoint",
                GlobalParameters = globalParameters
            };
            client = new TestClient(config);
            Assert.Equal("token", client.GetBearerToken());
            Assert.Equal("bearer", client.GetType());

            config = new Config
            {
                AccessKeyId = "ak",
                AccessKeySecret = "secret",
                Type = "access_key",
                Endpoint = "config.endpoint",
                Protocol = "config.protocol",
                Method = "config.method",
                RegionId = "config.regionId",
                UserAgent = "config.userAgent",
                ReadTimeout = 3000,
                ConnectTimeout = 3000,
                HttpProxy = "config.httpProxy",
                HttpsProxy = "config.httpsProxy",
                NoProxy = "config.noProxy",
                Socks5Proxy = "config.socks5Proxy",
                Socks5NetWork = "config.socks5NetWork",
                MaxIdleConns = 128,
                SignatureVersion = "config.signatureVersion",
                SignatureAlgorithm = "config.signatureAlgorithm",
                GlobalParameters = globalParameters,
                Key = "config.key",
                Cert = "config.cert",
                Ca = "config.ca",
                DisableHttp2 = true,
                Network = "config.network",
                Suffix = "config.suffix",
                EndpointType = "public"
            };
            client = new TestClient(config);
            Assert.Equal("ak", client.GetAccessKeyId());
            Assert.Equal("secret", client.GetAccessKeySecret());
            Assert.Equal("access_key", client.GetType());
            Assert.Equal("config.endpoint", client.Endpoint);
            Assert.Equal("public", client.EndpointType);
            Assert.Equal("config.network", client.Network);
            Assert.Equal("config.suffix", client.Suffix);
            Assert.Equal("config.protocol", client.Protocol);
            Assert.Equal("config.method", client.Method);
            Assert.Equal("config.regionId", client.RegionId);
            Assert.Equal("config.userAgent", client.UserAgent);
            Assert.Equal(3000, client.ReadTimeout);
            Assert.Equal(3000, client.ConnectTimeout);
            Assert.Equal("config.httpProxy", client.HttpProxy);
            Assert.Equal("config.httpsProxy", client.HttpsProxy);
            Assert.Equal("config.noProxy", client.NoProxy);
            Assert.Equal("config.socks5Proxy", client.Socks5Proxy);
            Assert.Equal("config.socks5NetWork", client.Socks5NetWork);
            Assert.Equal(128, client.MaxIdleConns);
            Assert.Equal("config.signatureVersion", client.SignatureVersion);
            Assert.Equal("config.signatureAlgorithm", client.SignatureAlgorithm);
            Assert.Equal("global-value", client.GlobalParameters.Headers["global-key"]);
            Assert.Equal("global-value", client.GlobalParameters.Queries["global-query"]);
            Assert.Equal("config.key", client.Key);
            Assert.Equal("config.cert", client.Cert);
            Assert.Equal("config.ca", client.Ca);
            Assert.Equal(true, client.DisableHttp2);

            Assert.Throws<ClientException>(() => new TestClient(null));
            var emptyConfig = new Config();
            Assert.Throws<ClientException>(() => client.CheckConfig(emptyConfig));
        }

        [Fact]
        public void TestEndpointSuffix()
        {
            Environment.SetEnvironmentVariable("ALIBABA_CLOUD_ENDPOINT_SUFFIX", "example.com");
            try
            {
                var fromEnv = new TestClient(new Config { Endpoint = "example.com" });
                Assert.Equal("example.com", fromEnv.EndpointSuffix);
                var mapped = fromEnv.BuildAttributeMap();
                Assert.Equal("example.com", mapped.Key["endpointSuffix"]);

                var explicitConfig = new Config
                {
                    Endpoint = "example.com",
                    EndpointSuffix = "example.net"
                };
                var explicitClient = new TestClient(explicitConfig);
                Assert.Equal("example.net", explicitClient.EndpointSuffix);
                explicitClient.AttributeMap = new AttributeMap
                {
                    Key = new Dictionary<string, string> { { "signatureTypePrefix", "ACS4-" } }
                };
                mapped = explicitClient.BuildAttributeMap();
                Assert.Equal("example.net", mapped.Key["endpointSuffix"]);
                Assert.Equal("ACS4-", mapped.Key["signatureTypePrefix"]);
                Assert.False(explicitClient.AttributeMap.Key.ContainsKey("endpointSuffix"));

                foreach (var callerSuffix in new[] { null, "", " \t\r\n", "caller.example" })
                {
                    var original = new AttributeMap
                    {
                        Key = new Dictionary<string, string> { { "endpointSuffix", callerSuffix } },
                        Attributes = new Dictionary<string, object> { { "kept", "value" } }
                    };
                    explicitClient.AttributeMap = original;
                    mapped = explicitClient.BuildAttributeMap();
                    Assert.Equal(string.IsNullOrWhiteSpace(callerSuffix) ? "example.net" : callerSuffix, mapped.Key["endpointSuffix"]);
                    Assert.Equal(callerSuffix, original.Key["endpointSuffix"]);
                    Assert.Equal("value", mapped.Attributes["kept"]);
                    if (string.IsNullOrWhiteSpace(callerSuffix))
                    {
                        Assert.NotSame(original, mapped);
                        mapped.Attributes["kept"] = "changed";
                        Assert.Equal("value", original.Attributes["kept"]);
                    }
                    else
                    {
                        Assert.Same(original, mapped);
                    }
                }
            }
            finally
            {
                Environment.SetEnvironmentVariable("ALIBABA_CLOUD_ENDPOINT_SUFFIX", null);
            }

            var bare = new TestClient(new Config { Endpoint = "example.com" });
            Assert.True(string.IsNullOrEmpty(bare.EndpointSuffix));
            Assert.Null(bare.BuildAttributeMap().Key);
        }
    }
}
