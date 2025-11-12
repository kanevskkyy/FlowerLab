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
    public class OrderGiftConfiguration : IEntityTypeConfiguration<OrderGift>
    {
        public void Configure(EntityTypeBuilder<OrderGift> builder)
        {
            builder.HasKey(og => new { og.OrderId, og.GiftId });

            builder.HasOne(og => og.Order)
                   .WithMany(o => o.OrderGifts)
                   .HasForeignKey(og => og.OrderId);

            builder.HasOne(og => og.Gift)
                   .WithMany(g => g.OrderGifts)
                   .HasForeignKey(og => og.GiftId);
        }
    }
}
