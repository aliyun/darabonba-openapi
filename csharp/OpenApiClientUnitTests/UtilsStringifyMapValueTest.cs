using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using AlibabaCloud.OpenApiClient;
using Xunit;

namespace OpenApiClientUnitTests
{
    public class UtilsStringifyMapValueTest
    {
        [Theory]
        [InlineData("en-US", "1.5")]
        [InlineData("fr-FR", "1,5")]
        public void StringifyMapValue_PreservesCSharpConversion(string culture, string decimalValue)
        {
            var originalCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                Assert.Null(Utils.StringifyMapValue(null));
                Assert.Empty(Utils.StringifyMapValue(new Dictionary<string, object>()));
                var input = new Dictionary<string, object>
                {
                    { "text", "hello" }, { "empty", "" }, { "null", null },
                    { "integer", 123L }, { "decimal", 1.5m }, { "boolean", true },
                    { "bytes", Encoding.UTF8.GetBytes("hello") },
                    { "invalid", new InvalidStringValue() }
                };

                var result = Utils.StringifyMapValue(input);
                Assert.Equal(input.Count, result.Count);
                Assert.Equal("hello", result["text"]);
                Assert.Equal("", result["empty"]);
                Assert.Null(result["null"]);
                Assert.Equal("123", result["integer"]);
                Assert.Equal(decimalValue, result["decimal"]);
                Assert.Equal("True", result["boolean"]);
                Assert.Equal("System.Byte[]", result["bytes"]);
                Assert.Null(result["invalid"]);
                result["text"] = "changed";
                Assert.Equal("hello", input["text"]);
                Assert.IsType<byte[]>(input["bytes"]);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCulture;
            }
        }

        private class InvalidStringValue
        {
            public override string ToString()
            {
                throw new InvalidOperationException();
            }
        }
    }
}
