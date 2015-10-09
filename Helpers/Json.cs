using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace RemoteFileBrowser
{
    public class JsonSerializer
    {
        private static readonly DefaultContractResolver Resolver = new DefaultContractResolver
        {
            IgnoreSerializableAttribute = true,
            DefaultMembersSearchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
        };

        private static readonly Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings
        {
            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Populate
        };

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }

        public object Deserialize(string json)
        {
            return JsonConvert.DeserializeObject(json, settings);
        }
    }
}