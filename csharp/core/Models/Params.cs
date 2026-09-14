using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Darabonba;

namespace AlibabaCloud.OpenApiClient.Models
{
    public class Params : Model  {
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

        public Params Copy()
        {
            Params copy = FromMap(ToMap());
            return copy;
        }

        public Params CopyWithoutStream()
        {
            Params copy = FromMap(ToMap(true));
            return copy;
        }

        public override Dictionary<string, object> ToMapCore(bool noStream = false)
        {
            var map = new Dictionary<string, object>();
            if (Action != null)
            {
                map["action"] = Action;
            }

            if (Version != null)
            {
                map["version"] = Version;
            }

            if (Protocol != null)
            {
                map["protocol"] = Protocol;
            }

            if (Pathname != null)
            {
                map["pathname"] = Pathname;
            }

            if (Method != null)
            {
                map["method"] = Method;
            }

            if (AuthType != null)
            {
                map["authType"] = AuthType;
            }

            if (BodyType != null)
            {
                map["bodyType"] = BodyType;
            }

            if (ReqBodyType != null)
            {
                map["reqBodyType"] = ReqBodyType;
            }

            if (Style != null)
            {
                map["style"] = Style;
            }

            return map;
        }

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            return ToMapCore(noStream);
        }

        public new static Params FromMap(IDictionary map)
        {
            if (map == null)
            {
                return null;
            }
            var model = new Params();
            if (map.Contains("action"))
            {
                model.Action = (string)map["action"];
            }
            if (map.Contains("version"))
            {
                model.Version = (string)map["version"];
            }
            if (map.Contains("protocol"))
            {
                model.Protocol = (string)map["protocol"];
            }
            if (map.Contains("pathname"))
            {
                model.Pathname = (string)map["pathname"];
            }
            if (map.Contains("method"))
            {
                model.Method = (string)map["method"];
            }
            if (map.Contains("authType"))
            {
                model.AuthType = (string)map["authType"];
            }
            if (map.Contains("bodyType"))
            {
                model.BodyType = (string)map["bodyType"];
            }
            if (map.Contains("reqBodyType"))
            {
                model.ReqBodyType = (string)map["reqBodyType"];
            }
            if (map.Contains("style"))
            {
                model.Style = (string)map["style"];
            }
            return model;
        }

        public static Params FromMap(Dictionary<string, object> map)
        {
            return FromMap((IDictionary)map);
        }
    }
}
