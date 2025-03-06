using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AlibabaCloud.OpenApiClient.Exceptions
{
    public class AlibabaCloudException :  {
        public int? StatusCode { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public string RequestId { get; set; }

        public AlibabaCloudException() : base()
        {
        }
    }

}

