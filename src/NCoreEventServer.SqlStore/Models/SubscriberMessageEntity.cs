using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.SqlStore.Models
{
    /// <summary>
    /// A Message Destined for a Subscriber
    /// </summary>
    public class SubscriberMessageEntity
    {
        /// <summary>
        /// The Id Assigned to this Message
        /// </summary>
        public long MessageId { get; set; }

        /// <summary>
        /// The Subscriber this is being sent to
        /// </summary>
        public string SubscriberId { get; set; }

        /// <summary>
        /// The Absolute Destination Url
        /// </summary>
        public string DestinationUri { get; set; }

        /// <summary>
        /// The Message Body to send
        /// </summary>
        public string JsonBody { get; set; }

        /// <summary>
        /// The Related Subscriber
        /// </summary>
        public SubscriberEntity Subscriber { get; set; }
    }
}
