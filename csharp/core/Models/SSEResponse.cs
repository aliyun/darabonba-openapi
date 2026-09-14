// This file is auto-generated, don't edit it. Thanks.

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Darabonba;
using Darabonba.Utils;

namespace AlibabaCloud.OpenApiClient.Models
{
    /// <term><b>Description:</b></term>
    /// <description>
    /// <para>This is for OpenApi SDK</para>
    /// </description>
    public class SSEResponse : Darabonba.Model  {
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
        public Darabonba.Models.SSEEvent Event { get; set; }

        public override Dictionary<string, object> ToMapCore(bool noStream = false)
        {
            var map = new Dictionary<string, object>();
            {
                Dictionary<string, object> value0 = null;
                if (this.Headers != null)
                {
                    value0 = new Dictionary<string, object>();
                    foreach (var item0 in this.Headers)
                    {
                        value0.Add(item0.Key, item0.Value);
                    }
                }
                map["headers"] = value0;
            }
            map["statusCode"] = this.StatusCode;
            map["event"] = this.Event == null ? null : this.Event.ToMap();
            return map;
        }

        public new static SSEResponse FromMap(IDictionary map)
        {
            if (map == null)
            {
                return null;
            }
            var model = new SSEResponse();
            if (map.Contains("headers"))
            {
                Dictionary<string, string> value0 = null;
                if (map["headers"] != null)
                {
                    value0 = new Dictionary<string, string>();
                    foreach (DictionaryEntry item0 in (IDictionary)map["headers"])
                    {
                        value0.Add((string)item0.Key, (string)item0.Value);
                    }
                }
                model.Headers = value0;
            }
            if (map.Contains("statusCode"))
            {
                model.StatusCode = map["statusCode"] == null ? (int?)null : System.Convert.ToInt32(map["statusCode"]);
            }
            if (map.Contains("event"))
            {
                model.Event = map["event"] == null ? null : Darabonba.Models.SSEEvent.FromMap((IDictionary)map["event"]);
            }
            return model;
        }
    }
}
