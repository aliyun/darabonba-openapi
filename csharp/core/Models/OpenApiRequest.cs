// This file is auto-generated, don't edit it. Thanks.

using System;
using System.Collections.Generic;
using System.IO;

using Tea;

namespace AlibabaCloud.OpenApiClient.Models
{
    public class OpenApiRequest : TeaModel {
        [NameInMap("headers")]
        [Validation(Required=false)]
        public Dictionary<string, string> Headers { get; set; }

        [NameInMap("query")]
        [Validation(Required=false)]
        public Dictionary<string, string> Query { get; set; }

        [NameInMap("body")]
        [Validation(Required=false)]
        public object Body { get; set; }

    }

}
