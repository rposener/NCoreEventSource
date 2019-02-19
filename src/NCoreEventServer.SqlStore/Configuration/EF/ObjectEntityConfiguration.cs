using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NCoreEventServer.Models;
using NCoreEventServer.SqlStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.Configuration
{
    public class ObjectEntityConfiguration : IEntityTypeConfiguration<ObjectEntity>
    {
        public void Configure(EntityTypeBuilder<ObjectEntity> builder)
        {
            builder.HasKey(t => new { t.ObjectType, t.ObjectId });
            builder.Property(t => t.ObjectJson).HasDefaultValue("{}");
        }
    }
}
