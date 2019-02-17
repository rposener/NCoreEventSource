using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NCoreEventServer.SqlStore.Models;

namespace NCoreEventServer.Configuration
{
    public class ServerEventMessageConfiguration : IEntityTypeConfiguration<ServerEventMessageEntity>
    {
        public void Configure(EntityTypeBuilder<ServerEventMessageEntity> builder)
        {
            builder.HasKey(t => t.LogId);
            builder.Property(t => t.LogId).UseSqlServerIdentityColumn();
            builder.HasIndex(t => t.Topic);
            builder.HasIndex(t => new { t.ObjectType, t.ObjectId });
        }
    }
}
