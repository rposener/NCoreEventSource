using NCoreEventServer.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCoreEventServer.Stores
{
    public class InMemoryEventQueueStore : IEventQueueStore
    {
        private long nextId;
        private readonly ConcurrentDictionary<long, ServerEventMessage> store;
        private readonly ConcurrentDictionary<long, ServerEventMessage> poison;

        public InMemoryEventQueueStore()
        {
            nextId = 0;
            store = new ConcurrentDictionary<long, ServerEventMessage>();
            poison = new ConcurrentDictionary<long, ServerEventMessage>();
        }

        public Task<long> AddEventAsync(ServerEventMessage message)
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

        public Task<IEnumerable<ServerEventMessage>> NextEventsAsync(int Max)
        {
            var nextItems = store.Values.Take(Max).ToArray();
            return Task.FromResult(nextItems.AsEnumerable());
        }

        public Task PoisonedEventAsync(long id)
        {
            ServerEventMessage poisonEvent;
            if (store.TryRemove(id, out poisonEvent))
                poison.AddOrUpdate(id, poisonEvent, (_, m) => poisonEvent);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ServerEventMessage>> PoisonEventsAsync(int Max)
        {
            var nextItems = poison.Values.Take(Max).ToArray();
            return Task.FromResult(nextItems.AsEnumerable());
        }
    }
}
