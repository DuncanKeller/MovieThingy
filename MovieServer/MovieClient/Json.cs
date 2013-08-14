using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace MovieClient
{
    static class Json
    {
        public static JObject ParseJson(string json)
        {
            return JObject.Parse(json);
        }

        public static string GetField(string key, JObject json)
        {
            return (string)json[key];
        }
    }
}
