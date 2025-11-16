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
    public class FlowerConfiguration : IEntityTypeConfiguration<Flower>
    {
        public void Configure(EntityTypeBuilder<Flower> builder)
        {
            builder.ToTable("Flowers");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(f => f.Name)
                .IsUnique();

            builder.Property(f => f.Color)
                .HasMaxLength(50);

            builder.Property(f => f.Size)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(f => f.Description)
                .HasMaxLength(500);

            builder.Property(f => f.Quantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasCheckConstraint("CK_Flower_Quantity_Positive", "\"Quantity\" >= 0");

            builder.Property(f => f.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(f => f.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
