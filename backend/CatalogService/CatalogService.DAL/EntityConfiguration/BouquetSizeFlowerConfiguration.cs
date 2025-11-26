using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.DAL.EntityConfiguration
{
    public class BouquetSizeFlowerConfiguration : IEntityTypeConfiguration<BouquetSizeFlower>
    {
        public void Configure(EntityTypeBuilder<BouquetSizeFlower> builder)
        {
            builder.ToTable("BouquetSizeFlowers");

            builder.HasKey(bsf => new { bsf.BouquetId, bsf.SizeId, bsf.FlowerId });

            builder.Property(bsf => bsf.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasCheckConstraint("CK_BouquetSizeFlower_Quantity_Positive", "\"Quantity\" > 0");

            builder.HasOne(bsf => bsf.BouquetSize)
                .WithMany(bs => bs.BouquetSizeFlowers)
                .HasForeignKey(bsf => new { bsf.BouquetId, bsf.SizeId })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bsf => bsf.Flower)
                .WithMany(f => f.BouquetSizeFlowers)
                .HasForeignKey(bsf => bsf.FlowerId)
                .OnDelete(DeleteBehavior.Cascade);  
        }
    }
}
