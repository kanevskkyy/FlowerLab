using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using shared.events;

namespace CatalogService.DAL.EntityConfiguration
{
    public class ProcessedEventConfiguration : IEntityTypeConfiguration<ProcessedEvent>
    {
        public void Configure(EntityTypeBuilder<ProcessedEvent> builder)
        {
            builder.ToTable("ProcessedEvents");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.EventId)
                .IsRequired();

            builder.Property(p => p.ProcessedAt)
                .IsRequired();

            builder.HasIndex(p => p.EventId)
                .IsUnique();
        }
    }
}
