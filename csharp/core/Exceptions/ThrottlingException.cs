using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AlibabaCloud.OpenApiClient.Exceptions
{
    public class ThrottlingException : AlibabaCloudException {
        public long? RetryAfter { get; set; }
    }
}

