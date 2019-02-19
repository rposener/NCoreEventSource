using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NCoreEventServer.SqlStore.Models;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.SqlStore.Stores
{
    public class SqlObjectStore : IObjectStore
    {
        private readonly ILogger<SqlObjectStore> logger;
        private readonly EventServerContext context;

        public SqlObjectStore(
            ILogger<SqlObjectStore> logger,
            EventServerContext context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<string> GetObjectAsync(string ObjectType, string ObjectId)
        {
            logger.LogInformation("Loading Object");
            var query = context.ObjectEntities
                .Where(e => e.ObjectType == ObjectType && e.ObjectId == e.ObjectId)
                .Select(e => e.ObjectJson);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<string>> GetObjectsAsync(string ObjectType)
        {
            logger.LogInformation("Loading Objects By Type");
            var query = context.ObjectEntities
                .Where(e => e.ObjectType == ObjectType)
                .Select(e => e.ObjectJson);
            return await query.ToArrayAsync();
        }

        public async Task RemoveAllObjectsOfTypeAsync(string ObjectType)
        {
            logger.LogInformation("Removing Objects By Type");
            var toRemove = await context.ObjectEntities
                .Where(e => e.ObjectType == ObjectType)
                .ToArrayAsync();
            context.ObjectEntities.RemoveRange(toRemove);
            await context.SaveChangesAsync();
        }

        public async Task RemoveObjectAsync(string ObjectType, string ObjectId)
        {
            logger.LogInformation("Removing an Object");
            context.ObjectEntities.Remove(new ObjectEntity { ObjectType = ObjectType, ObjectId = ObjectId });
            await context.SaveChangesAsync();
        }

        public async Task SetObjectAsync(string ObjectType, string ObjectId, string ObjectJson)
        {
            logger.LogInformation("Saving an Object");
            var existing = await context.ObjectEntities.FindAsync(new { ObjectType, ObjectId });
            if (existing == null)
            {
                context.ObjectEntities.Add(new ObjectEntity { ObjectType = ObjectType, ObjectId = ObjectId, ObjectJson = ObjectJson });
            }
            else
            {
                existing.ObjectJson = ObjectJson;
            }
            await context.SaveChangesAsync();
        }
    }
}
