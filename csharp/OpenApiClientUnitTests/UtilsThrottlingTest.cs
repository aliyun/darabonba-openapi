using System.Collections.Generic;
using AlibabaCloud.OpenApiClient;
using Xunit;

namespace OpenApiClientUnitTests
{
    public class UtilsThrottlingTest
    {
        [Fact]
        public void GetThrottlingTimeLeft_OnlyPositiveWaitTimes()
        {
            Assert.Equal(2000L, Utils.GetThrottlingTimeLeft(new Dictionary<string, string>
            {
                { "x-acs-retry-after", "2000" }
            }));
            Assert.Null(Utils.GetThrottlingTimeLeft(new Dictionary<string, string>
            {
                { "x-acs-retry-after", "0" }
            }));
            Assert.Null(Utils.GetThrottlingTimeLeft(new Dictionary<string, string>
            {
                { "x-acs-retry-after", "" }
            }));
            Assert.Null(Utils.GetThrottlingTimeLeft(new Dictionary<string, string>
            {
                { "x-acs-retry-after", "-1" }
            }));
            Assert.Null(Utils.GetThrottlingTimeLeft(new Dictionary<string, string>
            {
                { "x-acs-retry-after", "invalid" }
            }));
            Assert.Null(Utils.GetThrottlingTimeLeft(new Dictionary<string, string>()));
        }
    }
}
