using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Darabonba;

namespace AlibabaCloud.OpenApiClient.Models
{
    public class OpenApiRequest : DaraModel {
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

        public static OpenApiRequest FromMap(Dictionary<string, object> map)
        {
            var model = new OpenApiRequest();
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

            if (map.ContainsKey("query"))
            {
                var dict = map["query"] as Dictionary<string, string>;
                if (dict != null && dict.Count > 0)
                {
                    var modelMap1 = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> entry1 in dict)
                    {
                        modelMap1[entry1.Key] = (string)entry1.Value;
                    }
                    model.Query = modelMap1;
                }
            }

            if (map.ContainsKey("body"))
            {
                model.Body = (object)map["body"];
            }

            if (map.ContainsKey("stream"))
            {
                model.Stream = (Stream)map["stream"];
            }

            if (map.ContainsKey("hostMap"))
            {
                var dict = map["hostMap"] as Dictionary<string, string>;
                if (dict != null && dict.Count > 0)
                {
                    var modelMap1 = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> entry1 in dict)
                    {
                        modelMap1[entry1.Key] = (string)entry1.Value;
                    }
                    model.HostMap = modelMap1;
                }
            }

            if (map.ContainsKey("endpointOverride"))
            {
                model.EndpointOverride = (string)map["endpointOverride"];
            }

            return model;
        }
    }

}

