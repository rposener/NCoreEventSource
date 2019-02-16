using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer
{
    /// <summary>
    /// Wrapper for Serialization to ensure we use settings as we expect
    /// </summary>
    public static class EventStoreSerialization
    {
        private static JsonSerializerSettings Settings;
        private static JsonLoadSettings LoadSettings;

        static EventStoreSerialization()
        {
            Settings = new JsonSerializerSettings();
            LoadSettings = new JsonLoadSettings();
        }

        public static T DeSerializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        public static JObject DeSerializeObject(string json)
        {
            return JObject.Parse(json, LoadSettings);
        }

        public static string SerializeObject(JObject value)
        {
            return value.ToString(Formatting.None);
        }
    }
}
