using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        public Task AddEventToTopicAsync(string Topic, string Event)
        {
            topics.AddOrUpdate(Topic, 
                new TopicMetadata
                {
                    Topic = Topic,
                    RegisteredEvents = new[] { Event }
                },
                (_, m) =>
                {
                    m.RegisteredEvents = m.RegisteredEvents.Union(new[] { Event });
                    return m;
                });
            return Task.CompletedTask;
        }

        public Task AddObjectTypeAsync(string ObjectType)
        {
            objectTypes.Add(ObjectType);
            return Task.CompletedTask;
        }

        public Task AddTopicAsync(string Topic)
        {
            topics.AddOrUpdate(Topic, new TopicMetadata { Topic = Topic }, (_, m) => { return m; });
            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> GetObjectTypesAsync()
        {
            return Task.FromResult(objectTypes.AsEnumerable());
        }

        public Task<TopicMetadata> GetTopicAsync(string Topic)
        {
            return Task.FromResult(topics[Topic]);
        }

        public Task RemoveEventFromTopicAsync(string Topic, string Event)
        {
            var current = topics[Topic];
            var newValue = new TopicMetadata
            {
                Topic = current.Topic,
                RegisteredEvents = current.RegisteredEvents.Except(new[] { Event })
            };
            topics.TryUpdate(Topic, current, newValue);
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
    }
}
