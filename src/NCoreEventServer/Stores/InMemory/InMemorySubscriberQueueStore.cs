using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCoreEventServer.Models;

namespace NCoreEventServer.Stores
{
    public class InMemorySubscriberQueueStore : ISubscriberQueueStore
    {
        private long nextId;
        private readonly ConcurrentDictionary<long, SubscriberMessage> store;

        public InMemorySubscriberQueueStore()
        {
            nextId = 0;
            store = new ConcurrentDictionary<long, SubscriberMessage>();
        }

        public Task AddSubscriberMessageAsync(SubscriberMessage message)
        {
            message.MessageId = nextId++;
            store.TryAdd(message.MessageId, message);
            return Task.FromResult(message.MessageId);
        }

        public Task ClearMessageAsync(long MessageId)
        {
            store.TryRemove(MessageId, out _);
            return Task.CompletedTask;
        }

        public Task<SubscriberMessage> NextMessageForAsync(string SubscriberId)
        {
            var nextItem = store.Values.First();
            return Task.FromResult(nextItem);
        }

        public Task<IEnumerable<string>> SubscriberIdsWithPendingMessages()
        {
            var subscriberIds = store.Values.Select(m => m.SubscriberId).Distinct().ToArray();
            return Task.FromResult(subscriberIds.AsEnumerable());
        }
    }
}
