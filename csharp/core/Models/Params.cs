// This file is auto-generated, don't edit it. Thanks.

using System;
using System.Collections.Generic;
using System.IO;

using Tea;

namespace AlibabaCloud.OpenApiClient.Models
{
    public class Params : TeaModel {
        [NameInMap("action")]
        [Validation(Required=true)]
        public string Action { get; set; }

        [NameInMap("version")]
        [Validation(Required=true)]
        public string Version { get; set; }

        [NameInMap("protocol")]
        [Validation(Required=true)]
        public string Protocol { get; set; }

        [NameInMap("pathname")]
        [Validation(Required=true)]
        public string Pathname { get; set; }

        [NameInMap("method")]
        [Validation(Required=true)]
        public string Method { get; set; }

        [NameInMap("authType")]
        [Validation(Required=true)]
        public string AuthType { get; set; }

        [NameInMap("bodyType")]
        [Validation(Required=true)]
        public string BodyType { get; set; }

        [NameInMap("reqBodyType")]
        [Validation(Required=true)]
        public string ReqBodyType { get; set; }

        [NameInMap("style")]
        [Validation(Required=false)]
        public string Style { get; set; }

    }

}
