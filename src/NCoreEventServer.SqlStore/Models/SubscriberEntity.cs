using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.SqlStore.Models
{

    public class SubscriberEntity
    {
        /// <summary>
        /// Unique SubscriberId
        /// </summary>
        public string SubscriberId { get; set; }

        /// <summary>
        /// Base Uri To Send the Event or Data To
        /// </summary>
        public string BaseUri { get; set; }

        /// <summary>
        /// Subscriptions that the Subscriber requires
        /// </summary>
        public ICollection<SubscriptionEntity> Subscriptions { get; set; }

        /// <summary>
        /// Current State of the Subscriber
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Last time a Failure Occurred
        /// </summary>
        public DateTime? LastFailure { get; set; }

        /// <summary>
        /// Count of Failure Attempts
        /// </summary>
        public int FailureCount { get; set; }

        public ICollection<SubscriberMessageEntity> SubscriberMessages { get; set; }
    }
}
