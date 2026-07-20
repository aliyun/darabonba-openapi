using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using AlibabaCloud.OpenApiClient.Models;
using Darabonba.Models;
using Darabonba.RetryPolicy;
using Xunit;

namespace OpenApiClientUnitTests
{
    public static class TestFixtures
    {
        public static Config CreateConfig()
        {
            return new Config
            {
                AccessKeyId = "ak",
                AccessKeySecret = "secret",
                SecurityToken = "token",
                Type = "sts",
                UserAgent = "config.userAgent",
                ReadTimeout = 3000,
                ConnectTimeout = 3000,
                MaxIdleConns = 128,
                SignatureVersion = "config.signatureVersion",
                SignatureAlgorithm = "ACS3-HMAC-SHA256",
                GlobalParameters = new GlobalParameters
                {
                    Headers = new Dictionary<string, string>
                    {
                        { "global-key", "global-value" }
                    },
                    Queries = new Dictionary<string, string>
                    {
                        { "global-query", "global-value" }
                    }
                }
            };
        }

        public static Config CreateBearerTokenConfig()
        {
            return new Config
            {
                BearerToken = "token",
                Type = "bearer"
            };
        }

        public static Config CreateAnonymousConfig()
        {
            return new Config();
        }

        public static RuntimeOptions CreateRuntimeOptions()
        {
            return new RuntimeOptions
            {
                ReadTimeout = 4000,
                ConnectTimeout = 4000,
                MaxIdleConns = 100,
                Autoretry = true,
                MaxAttempts = 1,
                BackoffPolicy = "no",
                BackoffPeriod = 1,
                IgnoreSSL = true,
                ExtendsParameters = new ExtendsParameters
                {
                    Headers = new Dictionary<string, string>
                    {
                        { "extends-key", "extends-value" }
                    },
                    Queries = new Dictionary<string, string>
                    {
                        { "extends-key", "extends-value" }
                    }
                }
            };
        }

        public static OpenApiRequest CreateOpenApiRequest()
        {
            var query = new Dictionary<string, object>
            {
                { "key1", "value" },
                { "key2", 1 },
                { "key3", true }
            };
            var body = new Dictionary<string, object>
            {
                { "key1", "value" },
                { "key2", 1 },
                { "key3", true }
            };
            return new OpenApiRequest
            {
                Headers = new Dictionary<string, string>
                {
                    { "for-test", "sdk" }
                },
                Query = AlibabaCloud.OpenApiClient.Utils.Query(query),
                Body = AlibabaCloud.OpenApiClient.Utils.ParseToMap(body)
            };
        }

        public static RetryOptions CreateThrottlingRetryOptions()
        {
            return new RetryOptions
            {
                Retryable = true,
                RetryCondition = new List<RetryCondition>
                {
                    new RetryCondition
                    {
                        // Darabonba >=1.0.2 uses RetriesAttempted >= MaxAttempts (total attempts).
                        MaxAttempts = 3,
                        ErrorCode = new List<string> { "Throttling", "Throttling.User", "Throttling.Api" },
                        MaxDelayTimeMillis = 60000
                    }
                }
            };
        }

        public static Dictionary<string, string> GetHeaders(Dictionary<string, object> result)
        {
            return (Dictionary<string, string>)result["headers"];
        }

        public static Dictionary<string, object> GetBodyMap(Dictionary<string, object> result)
        {
            return (Dictionary<string, object>)result["body"];
        }

        public static string GetBodyString(Dictionary<string, object> result)
        {
            return (string)result["body"];
        }

        public static int GetStatusCode(Dictionary<string, object> result)
        {
            return Convert.ToInt32(result["statusCode"]);
        }

        public static bool RegexMatch(string input, string pattern)
        {
            return Regex.IsMatch(input ?? "", pattern);
        }

        public static bool Contains(string input, string value)
        {
            return (input ?? "").Contains(value);
        }

        public static string GetHeader(Dictionary<string, string> headers, string key)
        {
            if (headers == null)
            {
                return null;
            }
            foreach (var entry in headers)
            {
                if (string.Equals(entry.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return entry.Value;
                }
            }
            return null;
        }

        public static void AssertQueryContains(string rawQuery, params string[] parts)
        {
            foreach (var part in parts)
            {
                Assert.Contains(part, rawQuery ?? "");
            }
        }
    }
}
