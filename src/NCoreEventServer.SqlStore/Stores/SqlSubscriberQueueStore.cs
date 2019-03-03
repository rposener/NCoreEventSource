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
    public class SqlSubscriberQueueStore : ISubscriberQueueStore
    {
        private readonly IMapper mapper;
        private readonly ILogger<SqlSubscriberQueueStore> logger;
        private readonly EventServerContext context;

        public SqlSubscriberQueueStore(
            IMapper mapper,
            ILogger<SqlSubscriberQueueStore> logger,
            EventServerContext context)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task EnqueueMessageAsync(SubscriberMessage message)
        {
            logger.LogInformation("Adding Message To Queue");
            var entity = mapper.Map<SubscriberMessageEntity>(message);
            context.SubscriberMessages.Add(entity);
            await context.SaveChangesAsync();
        }

        public async Task ClearMessageAsync(long MessageId)
        {
            logger.LogInformation("Clearing Message from Queue");
            context.SubscriberMessages.Remove(new SubscriberMessageEntity { MessageId = MessageId });
            await context.SaveChangesAsync();
        }

        public async Task<SubscriberMessage> PeekMessageAsync(string SubscriberId)
        {
            logger.LogInformation("Getting Next Message from Queue for Subscriber");
            var entity = await context.SubscriberMessages.Where(m => m.SubscriberId == SubscriberId).FirstOrDefaultAsync();
            return mapper.Map<SubscriberMessage>(entity);
        }

        public async Task<IEnumerable<string>> SubscriberIdsWithPendingMessages()
        {
            logger.LogInformation("Finding Subscribers with Pending Messages");
            return await context.SubscriberMessages.Select(m => m.SubscriberId).Distinct().ToArrayAsync();
        }
    }
}
