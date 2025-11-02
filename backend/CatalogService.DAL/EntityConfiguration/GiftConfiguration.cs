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
    public class GiftConfiguration : IEntityTypeConfiguration<Gift>
    {
        public void Configure(EntityTypeBuilder<Gift> builder)
        {
            builder.ToTable("Gifts");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.GiftType)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(g => g.Name);

            builder.Property(g => g.ImageUrl)
                .HasMaxLength(255);
        }
    }
}
