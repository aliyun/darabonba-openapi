/// <term><b>Description:</b></term>
/// <description>
/// <para>This is for OpenApi SDK</para>
/// </description>
// This file is auto-generated, don't edit it. Thanks.

using System;
using System.Collections.Generic;
using System.IO;

using Tea;

namespace AlibabaCloud.OpenApiClient.Models
{
    public class GlobalParameters : TeaModel {
        [NameInMap("headers")]
        [Validation(Required=false)]
        public Dictionary<string, string> Headers { get; set; }

        [NameInMap("queries")]
        [Validation(Required=false)]
        public Dictionary<string, string> Queries { get; set; }

    }

}
