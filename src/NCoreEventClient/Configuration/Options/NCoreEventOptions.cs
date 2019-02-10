using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventClient
{
    public class NCoreEventOptions
    {
        public NCoreEventOptions()
        {
            InjestionUrl = "eventserver/injest";
            RegistrationUrl = "eventserver/register";
        }

        /// <summary>
        /// Id / Name for this Subscriber
        /// </summary>
        public string SubscriberId { get; set; }

        /// <summary>
        /// Url For this Subscriber
        /// </summary>
        public string SubscriberBaseUrl { get; set; }

        /// <summary>
        /// Url to the EventServer
        /// </summary>
        public string EventServerUrl { get; set; }

        /// <summary>
        /// Where to Send Events
        /// </summary>
        public string InjestionUrl { get; set; }

        /// <summary>
        /// Where to Register the Subscription
        /// </summary>
        public string RegistrationUrl { get; set; }
    }
}
