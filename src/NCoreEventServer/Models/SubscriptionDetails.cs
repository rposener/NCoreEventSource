using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.Models
{
    /// <summary>
    /// Details about a specific <seealso cref="Subscription"/> for a <seealso cref="Subscriber"/>
    /// </summary>
    public class SubscriptionDetails
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
        /// Relative Url to Send the Message To 
        /// </summary>
        public string RelativePath { get; set; }
    }
}
