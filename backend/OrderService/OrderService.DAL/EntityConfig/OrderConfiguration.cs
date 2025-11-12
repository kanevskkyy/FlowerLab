using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Domain.EntityConfig
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.UserFirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(o => o.UserLastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(o => o.Notes)
                   .HasMaxLength(500);

            builder.Property(o => o.GiftMessage)
                   .HasMaxLength(300);

            builder.Property(o => o.TotalPrice)
                   .HasColumnType("decimal(18,2)");

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