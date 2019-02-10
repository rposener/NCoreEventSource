using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCoreEventServer.Models;
using NCoreEventServer.Stores;

namespace NCoreEventServer.Services
{
    public class DefaultMetadataService : IMetadataService
    {
        private readonly IMetadataStore metadataStore;
        private IEnumerable<string> objectTypes;
        private IEnumerable<string> topics;

        public DefaultMetadataService(IMetadataStore metadataStore)
        {
            this.metadataStore = metadataStore ?? throw new ArgumentNullException(nameof(metadataStore));
        }

        public async Task AutoDiscoverEventsAsync(EventMessage eventMessage)
        {
            // Check to see if this instance already knows of the ObjectType
            if (topics != null && topics.Any(t => t.Equals(eventMessage.Topic, StringComparison.OrdinalIgnoreCase)))
                return;

            topics = await metadataStore.GetTopicsAsync();
            if (!topics.Any(t => t.Equals(eventMessage.Topic, StringComparison.OrdinalIgnoreCase)))
            {
                await metadataStore.AddTopicAsync(eventMessage.Topic);
            }
        }

        public async Task AutoDiscoverObjectsAsync(EventMessage eventMessage)
        {
            // Check to see if this instance already knows of the ObjectType
            if (objectTypes != null && objectTypes.Any(t => t.Equals(eventMessage.ObjectType, StringComparison.OrdinalIgnoreCase)))
                return;

            objectTypes = await metadataStore.GetObjectTypesAsync();
            if (!objectTypes.Any(t => t.Equals(eventMessage.ObjectType, StringComparison.OrdinalIgnoreCase)))
            {
                await metadataStore.AddObjectTypeAsync(eventMessage.ObjectType);
            }
        }
        
    }
}
