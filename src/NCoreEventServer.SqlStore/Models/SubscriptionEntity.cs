namespace NCoreEventServer.SqlStore.Models
{
    public class SubscriptionEntity
    {
        /// <summary>
        /// Unique SubscriberId
        /// </summary>
        public string SubscriberId { get; set; }

        /// <summary>
        /// Type of Subscription
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Relative Url to Send the Message To 
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// Event or Object Name
        /// </summary>
        public string Topic { get; set; }

        public SubscriberEntity Subscriber { get; set;}
    }
}
