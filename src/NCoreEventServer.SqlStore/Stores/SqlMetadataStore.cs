using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NCoreEventServer.SqlStore.Models;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.SqlStore.Stores
{
    public class SqlMetadataStore : IMetadataStore
    {
        private readonly ILogger<SqlMetadataStore> logger;
        private readonly EventServerContext context;

        public SqlMetadataStore(
            ILogger<SqlMetadataStore> logger,
            EventServerContext context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddObjectTypeAsync(string ObjectType)
        {
            logger.LogDebug("Adding ObjectType");
            context.ObjectMetadata.Add(new ObjectMetadataEntity { ObjectType = ObjectType });
            await context.SaveChangesAsync();
        }

        public async Task AddTopicAsync(string Topic)
        {
            logger.LogDebug("Adding Topic");
            context.TopicMetadata.Add(new TopicMetadataEntity { Topic = Topic });
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetObjectTypesAsync()
        {
            logger.LogDebug("Listing ObjectTypes");
            var query = from m in context.ObjectMetadata
                        select m.ObjectType;
            var results = await query.ToArrayAsync();
            return results;
        }

        public async Task<IEnumerable<string>> GetTopicsAsync()
        {
            logger.LogDebug("Listing Topics");
            var query = from m in context.TopicMetadata
                        select m.Topic;
            var results = await query.ToArrayAsync();
            return results;
        }

        public async Task RemoveObjectTypeAsync(string ObjectType)
        {
            context.Remove(new ObjectMetadataEntity { ObjectType = ObjectType });
            await context.SaveChangesAsync();
        }

        public async Task RemoveTopicAsync(string Topic)
        {
            context.Remove(new TopicMetadataEntity { Topic = Topic });
            await context.SaveChangesAsync();
        }
    }
}
