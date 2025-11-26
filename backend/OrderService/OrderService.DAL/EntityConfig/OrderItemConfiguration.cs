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
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.BouquetName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(oi => oi.BouquetImage)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(oi => oi.SizeName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(oi => oi.Price)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(oi => oi.Count)
                .IsRequired();

            builder.HasCheckConstraint("CK_OrderItem_Count_Positive", "\"Count\" > 0");

            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
