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
    public class BouquetFlowerConfiguration : IEntityTypeConfiguration<BouquetFlower>
    {
        public void Configure(EntityTypeBuilder<BouquetFlower> builder)
        {
            builder.ToTable("BouquetFlowers");

            builder.HasKey(bf => new { bf.BouquetId, bf.FlowerId });

            builder.Property(bf => bf.Quantity)
                .HasDefaultValue(1)
                .IsRequired();

            builder.HasCheckConstraint("CK_BouquetFlower_Quantity_Positive", "\"Quantity\" > 0");

            builder.HasOne(bf => bf.Bouquet)
                .WithMany(b => b.BouquetFlowers)
                .HasForeignKey(bf => bf.BouquetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bf => bf.Flower)
                .WithMany(f => f.BouquetFlowers)
                .HasForeignKey(bf => bf.FlowerId);
        }
    }
}
