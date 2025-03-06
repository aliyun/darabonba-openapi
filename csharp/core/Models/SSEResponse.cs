using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Darabonba;

namespace AlibabaCloud.OpenApiClient.Models
{
    /// <term><b>Description:</b></term>
    /// <description>
    /// <para>This is for OpenApi SDK</para>
    /// </description>
    public class SSEResponse : DaraModel {
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

        public SSEResponse Copy()
        {
            SSEResponse copy = FromMap(ToMap());
            return copy;
        }

        public SSEResponse CopyWithoutStream()
        {
            SSEResponse copy = FromMap(ToMap(true));
            return copy;
        }

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            var map = new Dictionary<string, object>();
            if (Headers != null)
            {
                var dict = new Dictionary<string, string>();
                foreach (var item1 in Headers) 
                {
                    dict[item1.Key] = item1.Value;
                }
                map["headers"] = dict;
            }

            if (StatusCode != null)
            {
                map["statusCode"] = StatusCode;
            }

            if (Event != null)
            {
                map["event"] = Event != null ? Event.ToMap(noStream) : null;
            }

            return map;
        }

        public static SSEResponse FromMap(Dictionary<string, object> map)
        {
            var model = new SSEResponse();
            if (map.ContainsKey("headers"))
            {
                var dict = map["headers"] as Dictionary<string, string>;
                if (dict != null && dict.Count > 0)
                {
                    var modelMap1 = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> entry1 in dict)
                    {
                        modelMap1[entry1.Key] = (string)entry1.Value;
                    }
                    model.Headers = modelMap1;
                }
            }

            if (map.ContainsKey("statusCode"))
            {
                model.StatusCode = (int?)map["statusCode"];
            }

            if (map.ContainsKey("event"))
            {
                var temp = (Dictionary<string, object>)map["event"];
                model.Event = SSEEvent.FromMap(temp);
            }

            return model;
        }
    }

}

