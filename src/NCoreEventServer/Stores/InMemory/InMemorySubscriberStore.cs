using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        public Task<IEnumerable<SubscriptionDetails>> GetSubscriptionsToObjectType(string ObjectType)
        {
            var matches = subscribers
                .Where(sub => sub.Value.Subscriptions.Any(s => s.Type == SubscriptionTypes.Object && s.Topic == ObjectType))
                .Select(sub => sub.Value).ToArray();

            List<SubscriptionDetails> results = new List<SubscriptionDetails>();
            foreach (var subscriber in matches)
            {
                var subscription = subscriber.Subscriptions
                    .First(s => s.Type == SubscriptionTypes.Object && s.Topic == ObjectType);
                results.Add(new SubscriptionDetails
                {
                    SubscriberId = subscriber.SubscriberId,
                    BaseUri = subscriber.BaseUri,
                    RelativePath = subscription.RelativePath
                });
            }
            return Task.FromResult(results.AsEnumerable());
        }

        public Task<IEnumerable<SubscriptionDetails>> GetSubscriptionsToTopic(string Topic)
        {
            var matches = subscribers
                .Where(sub => sub.Value.Subscriptions.Any(s => s.Type == SubscriptionTypes.Event && s.Topic == Topic))
                .Select(sub => sub.Value).ToArray();
            List<SubscriptionDetails> results = new List<SubscriptionDetails>();
            foreach (var subscriber in matches)
            {
                var subscription = subscriber.Subscriptions
                    .First(s => s.Type == SubscriptionTypes.Event && s.Topic == Topic);
                results.Add(new SubscriptionDetails
                {
                    SubscriberId = subscriber.SubscriberId,
                    BaseUri = subscriber.BaseUri,
                    RelativePath = subscription.RelativePath
                });
            }
            return Task.FromResult(results.AsEnumerable());
        }

        public Task UpdateSubscriberAsync(Subscriber subscriber)
        {
            subscribers.AddOrUpdate(subscriber.SubscriberId, subscriber, (id, sub) => subscriber);
            return Task.CompletedTask;
        }
    }
}
