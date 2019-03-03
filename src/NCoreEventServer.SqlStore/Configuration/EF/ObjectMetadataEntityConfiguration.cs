using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NCoreEventServer.Models;
using NCoreEventServer.SqlStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.Configuration
{
    public class ObjectMetadataEntityConfiguration : IEntityTypeConfiguration<ObjectMetadataEntity>
    {
        public void Configure(EntityTypeBuilder<ObjectMetadataEntity> builder)
        {
            builder.HasKey(t => t.ObjectType);
        }
    }
}
