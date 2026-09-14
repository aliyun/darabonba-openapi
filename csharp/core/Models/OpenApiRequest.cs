using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Darabonba;

namespace AlibabaCloud.OpenApiClient.Models
{
    public class OpenApiRequest : Model  {
        [NameInMap("headers")]
        [Validation(Required=false)]
        public Dictionary<string, string> Headers { get; set; }

        [NameInMap("query")]
        [Validation(Required=false)]
        public Dictionary<string, string> Query { get; set; }

        [NameInMap("body")]
        [Validation(Required=false)]
        public object Body { get; set; }

        [NameInMap("stream")]
        [Validation(Required=false)]
        public Stream Stream { get; set; }

        [NameInMap("hostMap")]
        [Validation(Required=false)]
        public Dictionary<string, string> HostMap { get; set; }

        [NameInMap("endpointOverride")]
        [Validation(Required=false)]
        public string EndpointOverride { get; set; }

        public OpenApiRequest Copy()
        {
            OpenApiRequest copy = FromMap(ToMap());
            return copy;
        }

        public OpenApiRequest CopyWithoutStream()
        {
            OpenApiRequest copy = FromMap(ToMap(true));
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

            if (Query != null)
            {
                var dict = new Dictionary<string, string>();
                foreach (var item1 in Query)
                {
                    dict[item1.Key] = item1.Value;
                }
                map["query"] = dict;
            }

            if (Body != null)
            {
                map["body"] = Body;
            }

            if (Stream != null)
            {
                map["stream"] = Stream;
            }

            if (HostMap != null)
            {
                var dict = new Dictionary<string, string>();
                foreach (var item1 in HostMap)
                {
                    dict[item1.Key] = item1.Value;
                }
                map["hostMap"] = dict;
            }

            if (EndpointOverride != null)
            {
                map["endpointOverride"] = EndpointOverride;
            }

            return map;
        }

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            return ToMapCore(noStream);
        }

        public new static OpenApiRequest FromMap(IDictionary map)
        {
            if (map == null)
            {
                return null;
            }
            var model = new OpenApiRequest();
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
            if (map.Contains("query"))
            {
                Dictionary<string, string> value0 = null;
                if (map["query"] != null)
                {
                    value0 = new Dictionary<string, string>();
                    foreach (DictionaryEntry item0 in (IDictionary)map["query"])
                    {
                        value0.Add((string)item0.Key, (string)item0.Value);
                    }
                }
                model.Query = value0;
            }
            if (map.Contains("body"))
            {
                model.Body = (object)map["body"];
            }
            if (map.Contains("stream"))
            {
                model.Stream = (Stream)map["stream"];
            }
            if (map.Contains("hostMap"))
            {
                Dictionary<string, string> value0 = null;
                if (map["hostMap"] != null)
                {
                    value0 = new Dictionary<string, string>();
                    foreach (DictionaryEntry item0 in (IDictionary)map["hostMap"])
                    {
                        value0.Add((string)item0.Key, (string)item0.Value);
                    }
                }
                model.HostMap = value0;
            }
            if (map.Contains("endpointOverride"))
            {
                model.EndpointOverride = (string)map["endpointOverride"];
            }
            return model;
        }

        public static OpenApiRequest FromMap(Dictionary<string, object> map)
        {
            return FromMap((IDictionary)map);
        }
    }
}
