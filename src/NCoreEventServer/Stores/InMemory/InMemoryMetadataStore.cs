using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NCoreEventServer.Models;

namespace NCoreEventServer.Stores
{
    public class InMemoryMetadataStore : IMetadataStore
    {
        private readonly List<string> objectTypes;
        private readonly ConcurrentDictionary<string, TopicMetadata> topics;

        public InMemoryMetadataStore()
        {
            objectTypes = new List<string>();
            topics = new ConcurrentDictionary<string, TopicMetadata>();
        }

        public Task AddObjectTypeAsync(string ObjectType)
        {
            objectTypes.Add(ObjectType);
            return Task.CompletedTask;
        }

        public Task RemoveObjectTypeAsync(string ObjectType)
        {
            objectTypes.Remove(ObjectType);
            return Task.CompletedTask;
        }

        public Task RemoveTopicAsync(string Topic)
        {
            topics.TryRemove(Topic, out _);
            return Task.CompletedTask;
        }

        public Task SaveTopicAsync(TopicMetadata topicMetadata)
        {
            topics.AddOrUpdate(topicMetadata.Topic, topicMetadata, (topic,metaData) => topicMetadata);
            return Task.CompletedTask;
        }
    }
}
