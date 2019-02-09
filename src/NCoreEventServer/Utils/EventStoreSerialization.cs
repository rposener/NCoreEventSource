using Newtonsoft.Json;
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

        static EventStoreSerialization()
        {
            Settings = new JsonSerializerSettings();
        }

        public static T DeSerializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        public static object DeSerializeObject(string json)
        {
            return JsonConvert.DeserializeObject(json, Settings);
        }

        public static string SerializeObject(Object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }
    }
}
