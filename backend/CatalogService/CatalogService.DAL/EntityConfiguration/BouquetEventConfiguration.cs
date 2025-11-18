using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.DAL.EntityConfiguration
{
    public class BouquetEventConfiguration : IEntityTypeConfiguration<BouquetEvent>
    {
        public void Configure(EntityTypeBuilder<BouquetEvent> builder)
        {
            builder.ToTable("BouquetEvents");
            builder.HasKey(be => new { be.BouquetId, be.EventId });

            builder.HasOne(be => be.Bouquet)
                .WithMany(b => b.BouquetEvents)
                .HasForeignKey(be => be.BouquetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(be => be.Event)
                .WithMany(e => e.BouquetEvents)
                .HasForeignKey(be => be.EventId);
        }
    }
}
