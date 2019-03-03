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
    public class RedisObjectStore : IObjectStore
    {
        const string NCORE_OBJECTS = "urn:ncore:objects";
        private readonly IMapper mapper;
        private readonly ILogger<RedisObjectStore> logger;
        private readonly IRedisClientsManager clientsManager;

        public RedisObjectStore(
            IMapper mapper,
            ILogger<RedisObjectStore> logger,
            IRedisClientsManager clientsManager)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.clientsManager = clientsManager ?? throw new ArgumentNullException(nameof(clientsManager));
        }

        private string GetId(string ObjectType, string ObjectId)
        {
            return $"{NCORE_OBJECTS}:{ObjectType}:{ObjectId}";
        }

        public Task<string> GetObjectAsync(string ObjectType, string ObjectId)
        {
            var Id = GetId(ObjectType, ObjectId);
            string result;
            using (IRedisClient redis = clientsManager.GetClient())
            {
                result = redis.GetValue(Id);
            }
            return Task.FromResult(result);
        }

        public Task<IEnumerable<string>> GetObjectsAsync(string ObjectType)
        {
            IEnumerable<string> results;
            using (IRedisClient redis = clientsManager.GetClient())
            {
                var keys = redis.SearchKeys(NCORE_OBJECTS+$":{ObjectType}:*");
                results = redis.GetValues(keys);
            }
            return Task.FromResult(results);
        }

        public Task RemoveAllObjectsOfTypeAsync(string ObjectType)
        {
            using (IRedisClient redis = clientsManager.GetClient())
            {
                var keys = redis.SearchKeys(NCORE_OBJECTS + $":{ObjectType}:*");
                foreach (var key in keys)
                {
                    redis.Remove(key);
                }
            }
            return Task.CompletedTask;
        }

        public Task RemoveObjectAsync(string ObjectType, string ObjectId)
        {
            var key = GetId(ObjectType, ObjectId);
            using (IRedisClient redis = clientsManager.GetClient())
            {
                redis.Remove(key);
            }
            return Task.CompletedTask;
        }

        public Task SetObjectAsync(string ObjectType, string ObjectId, string ObjectJson)
        {
            var key = GetId(ObjectType, ObjectId);
            using (IRedisClient redis = clientsManager.GetClient())
            {
                redis.SetValue(key, ObjectJson);
            }
            return Task.CompletedTask;
        }
    }
}
