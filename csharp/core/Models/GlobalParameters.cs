using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Darabonba;

namespace AlibabaCloud.OpenApiClient.Models
{
    /// <term><b>Description:</b></term>
    /// <description>
    /// <para>This is for OpenApi Util</para>
    /// </description>
    public class GlobalParameters : Model {
        [NameInMap("headers")]
        [Validation(Required=false)]
        public Dictionary<string, string> Headers { get; set; }

        [NameInMap("queries")]
        [Validation(Required=false)]
        public Dictionary<string, string> Queries { get; set; }

        public GlobalParameters Copy()
        {
            GlobalParameters copy = FromMap(ToMap());
            return copy;
        }

        public GlobalParameters CopyWithoutStream()
        {
            GlobalParameters copy = FromMap(ToMap(true));
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

            if (Queries != null)
            {
                var dict = new Dictionary<string, string>();
                foreach (var item1 in Queries) 
                {
                    dict[item1.Key] = item1.Value;
                }
                map["queries"] = dict;
            }

            return map;
        }

        public static GlobalParameters FromMap(Dictionary<string, object> map)
        {
            var model = new GlobalParameters();
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

            if (map.ContainsKey("queries"))
            {
                var dict = map["queries"] as Dictionary<string, string>;
                if (dict != null && dict.Count > 0)
                {
                    var modelMap1 = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> entry1 in dict)
                    {
                        modelMap1[entry1.Key] = (string)entry1.Value;
                    }
                    model.Queries = modelMap1;
                }
            }

            return model;
        }
    }

}

