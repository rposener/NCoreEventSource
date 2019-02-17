using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NCoreEventServer.SqlStore.Models;

namespace NCoreEventServer.Configuration
{
    public class SubscriberConfiguration : IEntityTypeConfiguration<SubscriberEntity>
    {
        public void Configure(EntityTypeBuilder<SubscriberEntity> builder)
        {
            builder.HasKey(b => b.SubscriberId);
        }
    }
}
