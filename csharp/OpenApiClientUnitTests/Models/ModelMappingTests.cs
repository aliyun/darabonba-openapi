using System;
using System.Collections;
using System.Collections.Generic;
using AlibabaCloud.OpenApiClient.Models;
using Darabonba;
using Xunit;

namespace OpenApiClientUnitTests.Models
{
    public class ModelMappingTests
    {
        private class ConfigProbe : Config
        {
            public string ReflectionOnly { get { throw new Exception("Config was reflected"); } }
        }

        [Fact]
        public void ConfigMapsNestedModelsWithoutReflection()
        {
            var config = Config.FromMap(new Dictionary<string, object>
            {
                { "readTimeout", 123L },
                { "globalParameters", new Dictionary<string, object>
                    { { "headers", new Dictionary<string, object> { { "name", "value" }, { "null", null } } },
                      { "queries", new Dictionary<string, object>() } } },
                { "retryOptions", new Dictionary<string, object> { { "retryable", true } } }
            });
            Assert.Equal(123, config.ReadTimeout);
            Assert.True(config.RetryOptions.Retryable);
            Assert.Null(config.GlobalParameters.Headers["null"]);
            Assert.Empty(config.GlobalParameters.Queries);
            var copy = Config.FromMap(config.ToMap());
            Assert.Equal("value", copy.GlobalParameters.Headers["name"]);
            Model probe = new ConfigProbe { Endpoint = "example" };
            Assert.Equal("example", probe.ToMap()["endpoint"]);
        }

        [Fact]
        public void ResponseMapsBuiltinEventAndPreservesEmptyCollections()
        {
            var response = SSEResponse.FromMap(new Dictionary<string, object>
            {
                { "statusCode", 200L }, { "headers", new Dictionary<string, object>() },
                { "event", new Dictionary<string, object> { { "data", "message" }, { "retry", 5L } } }
            });
            Assert.Equal(200, response.StatusCode);
            Assert.Empty(response.Headers);
            Assert.Equal(5, response.Event.Retry);
            Assert.Equal("message", SSEResponse.FromMap(response.ToMap()).Event.Data);
        }

        [Fact]
        public void ExistingDictionaryOverloadsUseTheNewConversion()
        {
            var request = OpenApiRequest.FromMap(new Dictionary<string, object>
            {
                { "headers", new Dictionary<string, object> { { "name", "value" } } },
                { "query", new Dictionary<string, object>() }
            });
            Assert.Equal("value", request.Headers["name"]);
            Assert.Empty(request.Query);
            Assert.Equal("value", request.Copy().Headers["name"]);
            Assert.Null(Config.FromMap(null));
        }
    }
}
