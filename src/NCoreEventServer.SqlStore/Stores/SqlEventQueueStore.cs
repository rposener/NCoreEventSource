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
    public class SqlEventQueueStore : IEventQueueStore
    {
        private readonly IMapper mapper;
        private readonly ILogger<SqlEventQueueStore> logger;
        private readonly EventServerContext context;

        public SqlEventQueueStore(
            IMapper mapper,
            ILogger<SqlEventQueueStore> logger,
            EventServerContext context)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<long> EnqueueEventAsync(ServerEventMessage message)
        {
            logger.LogInformation("Adding Event To Queue");
            var entity = mapper.Map<ServerEventMessageEntity>(message);
            context.EventMessages.Add(entity);
            await context.SaveChangesAsync();
            return entity.LogId;
        }

        public async Task DequeueEventAsync(long id)
        {
            logger.LogInformation("Clearing Event from Queue");
            context.EventMessages.Remove(new ServerEventMessageEntity { LogId = id });
            await context.SaveChangesAsync();
        }

        public async Task<ServerEventMessage> PeekEventAsync()
        {
            logger.LogInformation("Getting next Events from Queue");
            var firstEvent = await context.EventMessages.FirstOrDefaultAsync();
            return mapper.Map<ServerEventMessage>(firstEvent);
        }

        public async Task PoisonedEventAsync(long id)
        {
            logger.LogInformation("Marking an Event as Poisoned");
            var entity = await context.EventMessages.FindAsync(id);
            context.EventMessages.Remove(entity);
            context.PoisonMessages.Add(mapper.Map<PoisonEventMessageEntity>(entity));
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ServerEventMessage>> PoisonEventsAsync(int Max)
        {
            logger.LogInformation("Loading Poison Events");
            var entities = await context.PoisonMessages.Take(Max).ToArrayAsync();
            return mapper.Map<IEnumerable<ServerEventMessage>>(entities);
        }
    }
}
