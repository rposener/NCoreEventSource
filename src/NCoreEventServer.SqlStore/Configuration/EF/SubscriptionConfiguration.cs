using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NCoreEventServer.SqlStore.Models;

namespace NCoreEventServer.Configuration
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<SubscriptionEntity>
    {
        public void Configure(EntityTypeBuilder<SubscriptionEntity> builder)
        {
            builder.HasKey(s => new { s.SubscriberId, s.Type, s.Topic });
            builder.HasOne(s => s.Subscriber)
                .WithMany(sub => sub.Subscriptions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(s => s.SubscriberId)
                .HasPrincipalKey(sub => sub.SubscriberId)
                .IsRequired();
        }
    }
}
