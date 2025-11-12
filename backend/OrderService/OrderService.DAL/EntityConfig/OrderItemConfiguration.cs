using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;

namespace OrderService.Domain.EntityConfig
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.BouquetName).IsRequired().HasMaxLength(100);
            builder.Property(i => i.BouquetImage).IsRequired().HasMaxLength(2000);
            builder.Property(i => i.BouquetId).IsRequired();
            builder.Property(i => i.Price).HasColumnType("decimal(10,2)");
            builder.Property(i => i.Count).IsRequired();
        }
    }
}
