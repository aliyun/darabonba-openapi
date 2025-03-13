using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Darabonba.Exceptions;

namespace AlibabaCloud.OpenApiClient.Exceptions
{
    public class AlibabaCloudException : DaraResponseException {
        public string RequestId { get; set; }
    }

}