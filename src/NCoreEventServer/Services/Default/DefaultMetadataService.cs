using Microsoft.Extensions.Logging;
using NCoreEventServer.Models;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    public class DefaultMetadataService : IMetadataService
    {
        private readonly ILogger<DefaultMetadataService> logger;
        private readonly IMetadataStore metadataStore;

        public DefaultMetadataService(
            ILogger<DefaultMetadataService> logger,
            IMetadataStore metadataStore)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.metadataStore = metadataStore ?? throw new ArgumentNullException(nameof(metadataStore));
        }

        public async Task AutoDiscoverEventsAsync(ServerEventMessage eventMessage)
        {
            // Check to see if this Topic Already Exists
            var topics = await metadataStore.GetTopicsAsync();
            if (!topics.Any(t => t.Equals(eventMessage.Topic, StringComparison.OrdinalIgnoreCase)))
            {
                logger.LogInformation(nameof(DefaultMetadataService) + " discovered a new Event Topic");
                await metadataStore.AddTopicAsync(eventMessage.Topic);
            }
        }

        public async Task AutoDiscoverObjectsAsync(ServerEventMessage eventMessage)
        {
            // Check to see if this ObjectType Already Exists
            var objectTypes = await metadataStore.GetObjectTypesAsync();
            if (!objectTypes.Any(t => t.Equals(eventMessage.ObjectType, StringComparison.OrdinalIgnoreCase)))
            {
                logger.LogInformation(nameof(DefaultMetadataService) + " discovered a new ObjectType");
                await metadataStore.AddObjectTypeAsync(eventMessage.ObjectType);
            }
        }
        
    }
}
