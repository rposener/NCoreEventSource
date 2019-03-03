using Microsoft.Extensions.Logging;
using NCoreEventServer.Models;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    public class DefaultEventProcessingService : IEventProcessingService
    {
        private readonly ILogger<DefaultEventProcessingService> logger;
        private readonly IMetadataStore metadataStore;
        private readonly ISubscriberStore subscriberStore;
        private readonly ISubscriberQueueStore subscriberQueueStore;

        public DefaultEventProcessingService(
            ILogger<DefaultEventProcessingService> logger,
            IMetadataStore metadataStore,
            ISubscriberStore subscriberStore,
            ISubscriberQueueStore subscriberQueueStore)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.metadataStore = metadataStore ?? throw new ArgumentNullException(nameof(metadataStore));
            this.subscriberStore = subscriberStore ?? throw new ArgumentNullException(nameof(subscriberStore));
            this.subscriberQueueStore = subscriberQueueStore ?? throw new ArgumentNullException(nameof(subscriberQueueStore));
        }
        
        /// <summary>
        /// Processes an Event and Queues Messages to Subscribers
        /// </summary>
        /// <param name="Topic"></param>
        /// <param name="Event"></param>
        /// <param name="EventData"></param>
        /// <returns></returns>
        public async Task ProcessEvent(string Topic, string EventData)
        {
            logger.LogDebug($"Processing ({Topic})");
            var topics = await metadataStore.GetTopicsAsync();
            if (!topics.Contains(Topic, StringComparer.OrdinalIgnoreCase))
            {
                logger.LogWarning($"Not a valid topic");
                return;
            }

            var subscriptionDetails = await subscriberStore.GetSubscriptionsToTopic(Topic);
            if (subscriptionDetails == null || subscriptionDetails.Count() == 0)
            {
                logger.LogWarning($"No Subscribers");
                return;
            }

            // Create Messages to Notify all Subscribers
            foreach (var subscriptionDetail in subscriptionDetails)
            {
                await subscriberQueueStore.EnqueueMessageAsync(new SubscriberMessage
                {
                    DestinationUri = new Uri(subscriptionDetail.BaseUri, subscriptionDetail.RelativePath),
                    SubscriberId = subscriptionDetail.SubscriberId,
                    JsonBody = EventData
                });
            }
        }
    }
}
