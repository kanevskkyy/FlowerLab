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
    public class BouquetSizeConfiguration : IEntityTypeConfiguration<BouquetSize>
    {
        public void Configure(EntityTypeBuilder<BouquetSize> builder)
        {
            builder.ToTable("BouquetSizes");
            builder.HasKey(bs => new { bs.BouquetId, bs.SizeId });

            builder.Property(b => b.Price)
                .HasColumnType("numeric(10,2)")
                .IsRequired();

            builder.HasOne(bs => bs.Bouquet)
                .WithMany(b => b.BouquetSizes)
                .HasForeignKey(bs => bs.BouquetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bs => bs.Size)
                .WithMany(s => s.BouquetSizes)
                .HasForeignKey(bs => bs.SizeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
