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
        private readonly List<string> topics;

        public InMemoryMetadataStore()
        {
            objectTypes = new List<string>();
            topics = new List<string>();
        }

        public Task AddObjectTypeAsync(string ObjectType)
        {
            objectTypes.Add(ObjectType);
            return Task.CompletedTask;
        }

        public Task AddTopicAsync(string Topic)
        {
            topics.Add(Topic);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> GetObjectTypesAsync()
        {
            return Task.FromResult(objectTypes.AsEnumerable());
        }

        public Task<IEnumerable<string>> GetTopicsAsync()
        {
            return Task.FromResult(topics.AsEnumerable());
        }
        
        public Task RemoveObjectTypeAsync(string ObjectType)
        {
            objectTypes.Remove(ObjectType);
            return Task.CompletedTask;
        }

        public Task RemoveTopicAsync(string Topic)
        {
            topics.Remove(Topic);
            return Task.CompletedTask;
        }
    }
}
