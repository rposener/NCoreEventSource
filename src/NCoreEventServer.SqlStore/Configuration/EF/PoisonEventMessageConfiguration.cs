using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NCoreEventServer.SqlStore.Models;

namespace NCoreEventServer.Configuration
{
    public class PoisonEventMessageConfiguration : IEntityTypeConfiguration<PoisonEventMessageEntity>
    {
        public void Configure(EntityTypeBuilder<PoisonEventMessageEntity> builder)
        {
            builder.HasKey(t => t.LogId);
            builder.HasIndex(t => t.Topic);
            builder.HasIndex(t => new { t.ObjectType, t.ObjectId });
        }
    }
}
