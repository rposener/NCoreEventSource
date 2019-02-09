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

        public DefaultMetadataService(IMetadataStore metadataStore)
        {
            this.metadataStore = metadataStore ?? throw new ArgumentNullException(nameof(metadataStore));
        }

        public async Task AutoDiscoverEventsAsync(EventMessage eventMessage)
        {
            var topic = await metadataStore.GetTopicAsync(eventMessage.Topic);
            if (topic == null)
            {
                await metadataStore.AddTopicAsync(eventMessage.Topic);
                await metadataStore.AddEventToTopicAsync(eventMessage.Topic, eventMessage.Event);
            }
            else if (!topic.RegisteredEvents.Any(e => e.Equals(eventMessage.Event, StringComparison.OrdinalIgnoreCase)))
            {
                await metadataStore.AddEventToTopicAsync(eventMessage.Topic, eventMessage.Event);
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
