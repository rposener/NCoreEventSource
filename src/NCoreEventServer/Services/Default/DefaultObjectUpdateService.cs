using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using NCoreEventServer.Models;
using NCoreEventServer.Stores;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    public class DefaultObjectUpdateService : IObjectUpdateService
    {
        private readonly ILogger<DefaultObjectUpdateService> logger;
        private readonly IMetadataStore metadataStore;
        private readonly IObjectStore objectStore;
        private readonly ISubscriberStore subscriberStore;
        private readonly ISubscriberQueueStore subscriberQueueStore;

        public DefaultObjectUpdateService(
            ILogger<DefaultObjectUpdateService> logger,
            IMetadataStore metadataStore,
            IObjectStore objectStore,
            ISubscriberStore subscriberStore,
            ISubscriberQueueStore subscriberQueueStore)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.metadataStore = metadataStore ?? throw new ArgumentNullException(nameof(metadataStore));
            this.objectStore = objectStore ?? throw new ArgumentNullException(nameof(objectStore));
            this.subscriberStore = subscriberStore ?? throw new ArgumentNullException(nameof(subscriberStore));
            this.subscriberQueueStore = subscriberQueueStore ?? throw new ArgumentNullException(nameof(subscriberQueueStore));
        }

        /// <summary>
        /// Processes an Update to an Object and Notifies any Subscribers of the Update
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <param name="ObjectId"></param>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public async Task UpdateObject(string ObjectType, string ObjectId, string jsonObject)
        {
            logger.LogDebug($"Processing ({ObjectType},{ObjectId})");

            // Check for valid Metadata Type
            var objectTypes = await metadataStore.GetObjectTypesAsync();
            if (!objectTypes.Any(ot => ot.Equals(ObjectType, StringComparison.OrdinalIgnoreCase)))
            {
                logger.LogWarning($"Not a valid ObjectType");
                return;
            }

            // Update the Object
            var existingObject = await objectStore.GetObjectAsync(ObjectType, ObjectId);
            var storedObject = EventStoreSerialization.DeSerializeObject(existingObject ?? "{}");
            var updatedObject = EventStoreSerialization.DeSerializeObject(jsonObject);
            storedObject["_id"] = ObjectId;
            foreach(var update in updatedObject)
            {
                storedObject[update.Key] = update.Value;
            }
            var newJson = EventStoreSerialization.SerializeObject(storedObject);
            await objectStore.SetObjectAsync(ObjectType, ObjectId, newJson);

            // Check for Subscribers
            var subscriptionDetails = await subscriberStore.GetSubscriptionsToObjectType(ObjectType);
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
                    JsonBody = newJson
                });
            }
        }
    }
}
