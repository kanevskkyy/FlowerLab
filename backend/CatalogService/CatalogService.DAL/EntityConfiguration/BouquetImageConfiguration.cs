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
    public class BouquetImageConfiguration : IEntityTypeConfiguration<BouquetImage>
    {
        public void Configure(EntityTypeBuilder<BouquetImage> builder)
        {
            builder.ToTable("BouquetImages");
            builder.HasKey(bi => bi.Id);

            builder.Property(bi => bi.ImageUrl)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(bi => bi.Position)
                .IsRequired();

            builder.Property(bi => bi.IsMain)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(bi => bi.Bouquet)
                .WithMany()
                .HasForeignKey(bi => bi.BouquetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bi => bi.BouquetSize)
                .WithMany(bs => bs.BouquetImages)
                .HasForeignKey(bi => new { bi.BouquetId, bi.SizeId })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(bi => new { bi.BouquetId, bi.SizeId, bi.IsMain });
        }
    }
}
