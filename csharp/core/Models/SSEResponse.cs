using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Darabonba;
using Darabonba.Models;

namespace AlibabaCloud.OpenApiClient.Models
{
    /// <term><b>Description:</b></term>
    /// <description>
    /// <para>This is for OpenApi SDK</para>
    /// </description>
    public class SSEResponse : Darabonba.Model {
        [NameInMap("headers")]
        [Validation(Required=true)]
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// <para>HTTP Status Code</para>
        /// </summary>
        [NameInMap("statusCode")]
        [Validation(Required=true)]
        public int? StatusCode { get; set; }

        [NameInMap("event")]
        [Validation(Required=true)]
        public SSEEvent Event { get; set; }
    }

}

