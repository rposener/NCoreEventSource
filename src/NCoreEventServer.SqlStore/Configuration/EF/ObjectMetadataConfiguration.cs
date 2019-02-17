using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NCoreEventServer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.Configuration
{
    public class ObjectMetadataConfiguration : IEntityTypeConfiguration<ObjectMetadata>
    {
        public void Configure(EntityTypeBuilder<ObjectMetadata> builder)
        {
            builder.HasKey(t => t.ObjectType);
        }
    }
}
