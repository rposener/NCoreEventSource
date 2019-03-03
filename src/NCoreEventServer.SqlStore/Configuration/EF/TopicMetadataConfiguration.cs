using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NCoreEventServer.SqlStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.SqlStore.Configuration
{
    public class TopicMetadataConfiguration : IEntityTypeConfiguration<TopicMetadataEntity>
    {
        public void Configure(EntityTypeBuilder<TopicMetadataEntity> builder)
        {
            builder.HasKey(t => t.Topic);
        }
    }
}
