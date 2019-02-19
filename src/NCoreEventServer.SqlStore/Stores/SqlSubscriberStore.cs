using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NCoreEventServer.Models;
using NCoreEventServer.SqlStore.Models;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.SqlStore.Stores
{
    public class SqlSubscriberStore : ISubscriberStore
    {
        private readonly IMapper mapper;
        private readonly ILogger<SqlSubscriberStore> logger;
        private readonly EventServerContext context;

        public SqlSubscriberStore(
            IMapper mapper,
            ILogger<SqlSubscriberStore> logger,
            EventServerContext context)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddSubscriberAsync(Subscriber subscriber)
        {
            logger.LogDebug("Adding Subscriber");
            context.Subscribers.Add(mapper.Map<SubscriberEntity>(subscriber));
            await context.SaveChangesAsync();
        }

        public async Task DeleteSubscriberAsync(string SubscriberId)
        {
            logger.LogDebug("Deleting Subscriber");
            context.Subscribers.Remove(new SubscriberEntity { SubscriberId = SubscriberId });
            await context.SaveChangesAsync();
        }

        public async Task<Subscriber> GetSubscriber(string SubscriberId)
        {
            logger.LogDebug("Getting Subscriber");
            var entity = await context.Subscribers.FirstAsync(s => s.SubscriberId == SubscriberId);
            var subscriptions = await context.Subscriptions.Where(s => s.SubscriberId == SubscriberId).ToArrayAsync();
            var subscriber = mapper.Map<Subscriber>(entity);
            subscriber.Subscriptions = mapper.Map<ICollection<Subscription>>(subscriptions);
            return subscriber;
        }

        public async Task<IEnumerable<SubscriptionDetails>> GetSubscriptionsToObjectType(string ObjectType)
        {
            logger.LogDebug("Getting Subscribers for ObjectType");
            var query = from sub in context.Subscriptions
                        where sub.Type == SubscriptionTypes.Object.ToString() &&
                        sub.Topic == ObjectType
                        select new SubscriptionDetails
                        {
                            BaseUri = new Uri(sub.Subscriber.BaseUri),
                            SubscriberId = sub.SubscriberId,
                            RelativePath = sub.RelativePath
                        };
            return await query.ToArrayAsync();
        }

        public async Task<IEnumerable<SubscriptionDetails>> GetSubscriptionsToTopic(string Topic)
        {
            logger.LogDebug("Getting Subscribers for Topic");
            var query = from sub in context.Subscriptions
                        where sub.Type == SubscriptionTypes.Object.ToString() &&
                        sub.Topic == Topic
                        select new SubscriptionDetails
                        {
                            BaseUri = new Uri(sub.Subscriber.BaseUri),
                            SubscriberId = sub.SubscriberId,
                            RelativePath = sub.RelativePath
                        };
            return await query.ToArrayAsync();
        }

        public async Task UpdateSubscriberAsync(Subscriber subscriber)
        {
            logger.LogDebug("Updating Subscriber");
            var entity = await context.Subscribers.FirstAsync(s => s.SubscriberId == subscriber.SubscriberId);
            mapper.Map(subscriber, entity);
            logger.LogDebug("Updating Subscriptions");
            var subscriptions = await context.Subscriptions.Where(sub => sub.SubscriberId == subscriber.SubscriberId).ToArrayAsync();
            context.Subscriptions.RemoveRange(subscriptions);
            var newEntities = mapper.Map<IEnumerable<SubscriptionEntity>>(subscriber.Subscriptions);
            context.Subscriptions.AddRange(newEntities);
            await context.SaveChangesAsync();
        }
    }
}
