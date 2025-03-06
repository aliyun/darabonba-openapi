using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AlibabaCloud.OpenApiClient.Exceptions
{
    public class ClientException : AlibabaCloudException {
        public Dictionary<string, object> AccessDeniedDetail { get; set; }

        public ClientException() : base()
        {
        }
    }

}

