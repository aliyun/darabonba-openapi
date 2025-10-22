using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Darabonba.Utils;

namespace AlibabaCloud.OpenApiClient.Exceptions
{
    public class AlibabaCloudException : Darabonba.Exceptions.DaraResponseException
    {
        public string RequestId { get; set; }

    }

}