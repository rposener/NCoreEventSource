using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NCoreEventServer.Models;

namespace NCoreEventServer.Stores
{
    public class InMemorySubscriberStore : ISubscriberStore
    {

        private readonly ConcurrentDictionary<string, Subscriber> subscribers;

        public InMemorySubscriberStore()
        {
            subscribers = new ConcurrentDictionary<string, Subscriber>();
        }

        public Task AddSubscriberAsync(Subscriber subscriber)
        {
            subscribers.AddOrUpdate(subscriber.SubscriberId, subscriber, (id, sub) => subscriber);
            return Task.CompletedTask;
        }

        public Task DeleteSubscriberAsync(string SubscriberId)
        {
            subscribers.TryRemove(SubscriberId, out _);
            return Task.CompletedTask;
        }

        public Task<Subscriber> GetSubscriber(string SubscriberId)
        {
            return Task.FromResult(subscribers[SubscriberId]);
        }

        public Task UpdateSubscriberAsync(Subscriber subscriber)
        {
            subscribers.AddOrUpdate(subscriber.SubscriberId, subscriber, (id, sub) => subscriber);
            return Task.CompletedTask;
        }
    }
}
