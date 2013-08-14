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
            json = json.Substring(1, json.Length - 2);

            return JObject.Parse(json);
        }

        public static string GetField(string key, JObject json)
        {
            return (string)json[key];
        }

        public static string[] GetFieldArr(string key, JObject json)
        {
            JArray jArr = (JArray)json[key];
            string[] arr = new string[jArr.Count];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = (string)jArr[i];
            }
            return arr;
        }

        public static string GetDescription(JObject json)
        {
            return GetField("plot_simple", json);
        }

        public static string[] GetActors(JObject json)
        {
            return GetFieldArr("actors", json);
        }
    }
}
