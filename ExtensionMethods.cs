using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ScreepsAPI_NET
{
    static class ExtensionMethods
    {

        public static T GetValue<T>(this JObject obj, String value)
        {
            JToken t;
            if (obj.TryGetValue(value, out t))
                return t.Value<T>();

            return default(T);
        }

        public static T[] ToArray<T>(this JArray arr)
        {
            List<T> nArr = new List<T>();
            foreach (JValue o in arr)
                nArr.Add((T)o.Value);

            return nArr.ToArray();
        }

    }
}
