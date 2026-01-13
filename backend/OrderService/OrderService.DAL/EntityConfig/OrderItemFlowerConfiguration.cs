using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;

namespace OrderService.DAL.EntityConfig
{
    public class OrderItemFlowerConfiguration : IEntityTypeConfiguration<OrderItemFlower>
    {
        public void Configure(EntityTypeBuilder<OrderItemFlower> builder)
        {
            builder.ToTable("OrderItemFlowers");

            builder.HasKey(oif => new { oif.OrderItemId, oif.FlowerId });

            builder.Property(oif => oif.FlowerName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(oif => oif.Quantity)
                .IsRequired();

            builder.HasCheckConstraint("CK_OrderItemFlower_Quantity_Positive", "\"Quantity\" > 0");

            builder.HasOne(oif => oif.OrderItem)
                .WithMany(oi => oi.Flowers)
                .HasForeignKey(oif => oif.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
