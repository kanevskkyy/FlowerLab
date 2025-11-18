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

            builder.HasKey(i => i.Id);

            builder.Property(i => i.ImageUrl)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(i => i.Position)
                .IsRequired();

            builder.HasCheckConstraint("CK_BouquetImage_Position", "\"Position\" BETWEEN 1 AND 3");

            builder.HasOne(i => i.Bouquet)
                .WithMany(b => b.BouquetImages)
                .HasForeignKey(i => i.BouquetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
