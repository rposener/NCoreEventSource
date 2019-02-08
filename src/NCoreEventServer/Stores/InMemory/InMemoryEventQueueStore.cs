using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCoreEventServer.Models;

namespace NCoreEventServer.Stores
{
    public class InMemoryEventQueueStore : IEventQueueStore
    {
        private long nextId;
        private readonly ConcurrentDictionary<long, EventMessage> store;

        public InMemoryEventQueueStore()
        {
            nextId = 0;
            store = new ConcurrentDictionary<long, EventMessage>();
        }

        public Task<long> AddEventAsync(EventMessage message)
        {
            message.LogId = nextId++;
            store.TryAdd(message.LogId, message);
            return Task.FromResult(message.LogId);
        }

        public Task ClearEventAsync(long id)
        {
            store.TryRemove(id,out _);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<EventMessage>> NextEventsAsync(int Max)
        {
            var nextItems = store.Values.Take(Max).ToArray();
            return Task.FromResult(nextItems.AsEnumerable());
        }
    }
}
