using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventClient.Models
{
    public enum SubscriptionTypes { Event, Object }

    public class Subscription
    {
        /// <summary>
        /// Type of <seealso cref="SubscriptionTypes"/>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SubscriptionTypes Type { get; set; }

        /// <summary>
        /// Relative Url to Send the Message To 
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// Event or Object Name, depending on <seealso cref="Type"/>
        /// </summary>
        public string Topic { get; set; }
    }
}
