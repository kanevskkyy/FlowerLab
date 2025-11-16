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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.HasOne(o => o.Status)
                   .WithMany(s => s.Orders)
                   .HasForeignKey(o => o.StatusId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Items)
                   .WithOne(i => i.Order)
                   .HasForeignKey(i => i.OrderId);

            builder.HasOne(o => o.DeliveryInformation)
                   .WithOne(d => d.Order)
                   .HasForeignKey<DeliveryInformation>(d => d.OrderId);

            builder.HasMany(o => o.OrderGifts)
                   .WithOne(og => og.Order)
                   .HasForeignKey(og => og.OrderId);
        }
    }
}
