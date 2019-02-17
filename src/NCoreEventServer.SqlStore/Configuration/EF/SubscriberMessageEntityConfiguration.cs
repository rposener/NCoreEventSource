using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NCoreEventServer.SqlStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.SqlStore.Configuration
{
    public class SubscriberMessageEntityConfiguration : IEntityTypeConfiguration<SubscriberMessageEntity>
    {
        public void Configure(EntityTypeBuilder<SubscriberMessageEntity> builder)
        {
            builder.HasKey(m => m.MessageId);
            builder.Property(m => m.MessageId).UseSqlServerIdentityColumn();
            builder.HasOne(m => m.Subscriber)
                .WithMany(sub => sub.SubscriberMessages)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(m => m.SubscriberId)
                .IsRequired();
        }
    }
}
