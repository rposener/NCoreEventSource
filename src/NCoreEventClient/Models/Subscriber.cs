using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventClient.Models
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
    }
}
