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
    public class GlobalParameters : Model  {
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

        public override Dictionary<string, object> ToMapCore(bool noStream = false)
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

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            return ToMapCore(noStream);
        }

        public new static GlobalParameters FromMap(IDictionary map)
        {
            if (map == null)
            {
                return null;
            }
            var model = new GlobalParameters();
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
            if (map.Contains("queries"))
            {
                Dictionary<string, string> value0 = null;
                if (map["queries"] != null)
                {
                    value0 = new Dictionary<string, string>();
                    foreach (DictionaryEntry item0 in (IDictionary)map["queries"])
                    {
                        value0.Add((string)item0.Key, (string)item0.Value);
                    }
                }
                model.Queries = value0;
            }
            return model;
        }

        public static GlobalParameters FromMap(Dictionary<string, object> map)
        {
            return FromMap((IDictionary)map);
        }
    }
}
