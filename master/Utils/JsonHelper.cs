using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;

namespace Utils
{
    public static class JsonHelper
    {
        public static T GetEntity<T>(string json,string path=null)
        {
            JObject jObject = JObject.Parse(json);
            if (path != null && path.Length > 0)
            {
                JToken jToken= jObject.SelectToken(path);

                return jToken.ToObject<T>();
            }

            return jObject.ToObject<T>();
        }
    }
}
