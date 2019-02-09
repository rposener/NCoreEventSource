using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.Models
{
    public enum SubscriberStates { Active, Inactive, Down }

    public class Subscriber
    {
        /// <summary>
        /// Unique SubscriberId
        /// </summary>
        public string SubscriberId { get; set; }

        /// <summary>
        /// Base Uri To Send the Event or Data To
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// Subscriptions that the Subscriber requires
        /// </summary>
        public ICollection<Subscription> Subscriptions { get; set; }

        /// <summary>
        /// Current State of the Subscriber
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SubscriberStates State { get; set; }

        /// <summary>
        /// Last time a Failure Occurred
        /// </summary>
        public DateTime? LastFailure { get; set; }

        /// <summary>
        /// Count of Failure Attempts
        /// </summary>
        public int FailureCount { get; set; }
    }
}
