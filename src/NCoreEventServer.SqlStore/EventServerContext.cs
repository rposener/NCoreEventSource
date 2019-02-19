using Microsoft.EntityFrameworkCore;
using NCoreEventServer.SqlStore.Models;

namespace NCoreEventServer.SqlStore
{
    public class EventServerContext : DbContext
    {

        public EventServerContext()
        {
        }

        public EventServerContext(DbContextOptions dbContextOptions)
            :base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventServerContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ServerEventMessageEntity> EventMessages { get; set; }

        public DbSet<PoisonEventMessageEntity> PoisonMessages { get; set; }

        public DbSet<ObjectMetadataEntity> ObjectMetadata { get; set; }

        public DbSet<ObjectEntity> ObjectEntities { get; set; }

        public DbSet<SubscriberEntity> Subscribers { get; set; }

        public DbSet<SubscriberMessageEntity> SubscriberMessages { get; set; }

        public DbSet<SubscriptionEntity> Subscriptions { get; set; }

        public DbSet<TopicMetadataEntity> TopicMetadata { get; set; }
    }
}
