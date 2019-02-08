using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Stores
{
    public class InMemoryObjectStore : IObjectStore
    {
        private readonly ConcurrentDictionary<string, string> objects;

        public InMemoryObjectStore()
        {
            objects = new ConcurrentDictionary<string, string>();
        }

        public Task<string> GetObjectAsync(string ObjectType, string ObjectId)
        {
            return Task.FromResult(objects[$"{ObjectType}.{ObjectId}"]);
        }

        public Task<IEnumerable<string>> GetObjectsAsync(string ObjectType)
        {
            var results = objects.Where(o => o.Key.StartsWith($"{ObjectType}.")).Select(o => o.Value).ToArray();
            return Task.FromResult(results.AsEnumerable());
        }

        public Task RemoveAllObjectsOfTypeAsync(string ObjectType)
        {
            foreach(var key in objects.Keys)
            {
                if (key.StartsWith($"{ObjectType}."))
                    objects.TryRemove(key, out _);
            }
            return Task.CompletedTask;
        }

        public Task RemoveObjectAsync(string ObjectType, string ObjectId)
        {
            objects.TryRemove($"{ObjectType}.{ObjectId}", out _);
            return Task.CompletedTask;
        }

        public Task SetObjectAsync(string ObjectType, string ObjectId, string ObjectJson)
        {
            objects.AddOrUpdate($"{ObjectType}.{ObjectId}", ObjectJson, (k, v) => ObjectJson);
            return Task.CompletedTask;
        }
    }
}
