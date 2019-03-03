using AutoMapper;
using Microsoft.Extensions.Logging;
using NCoreEventServer.Stores;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.RedisStore.Stores
{
    public class RedisMetadataStore : IMetadataStore
    {
        const string NCORE_TOPICS = "urn:ncore:metadata:topics";
        const string NCORE_OBJECTS = "urn:ncore:metadata:objects";
        private readonly IMapper mapper;
        private readonly ILogger<RedisMetadataStore> logger;
        private readonly IRedisClientsManager clientsManager;

        public RedisMetadataStore(
            IMapper mapper,
            ILogger<RedisMetadataStore> logger,
            IRedisClientsManager clientsManager)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.clientsManager = clientsManager ?? throw new ArgumentNullException(nameof(clientsManager));
        }

        public Task AddObjectTypeAsync(string ObjectType)
        {
            using (IRedisClient redis = clientsManager.GetClient())
            {
                redis.AddItemToList(NCORE_OBJECTS, ObjectType);
            }
            return Task.CompletedTask;
        }

        public Task AddTopicAsync(string Topic)
        {
            using (IRedisClient redis = clientsManager.GetClient())
            {
                redis.AddItemToList(NCORE_TOPICS, Topic);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> GetObjectTypesAsync()
        {
            IEnumerable<string> results;
            using (IRedisClient redis = clientsManager.GetClient())
            {
                results = redis.GetAllItemsFromList(NCORE_OBJECTS);
            }
            return Task.FromResult(results);
        }

        public Task<IEnumerable<string>> GetTopicsAsync()
        {
            IEnumerable<string> results;
            using (IRedisClient redis = clientsManager.GetClient())
            {
                results = redis.GetAllItemsFromList(NCORE_TOPICS);
            }
            return Task.FromResult(results);
        }

        public Task RemoveObjectTypeAsync(string ObjectType)
        {
            using (IRedisClient redis = clientsManager.GetClient())
            {
                redis.RemoveItemFromList(NCORE_OBJECTS, ObjectType);
            }
            return Task.CompletedTask;
        }

        public Task RemoveTopicAsync(string Topic)
        {
            using (IRedisClient redis = clientsManager.GetClient())
            {
                redis.RemoveItemFromList(NCORE_TOPICS, Topic);
            }
            return Task.CompletedTask;
        }
    }
}
