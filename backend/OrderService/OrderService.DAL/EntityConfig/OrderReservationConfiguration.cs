using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.DAL.EntityConfig
{
    public class OrderReservationConfiguration : IEntityTypeConfiguration<OrderReservation>
    {
        public void Configure(EntityTypeBuilder<OrderReservation> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedNever();

            builder.HasOne(x => x.Order)
                   .WithMany(o => o.Reservations)
                   .HasForeignKey(x => x.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.BouquetId)
                   .IsRequired();

            builder.Property(x => x.BouquetName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.SizeId)
                   .IsRequired();

            builder.Property(x => x.SizeName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Quantity)
                   .IsRequired();

            builder.Property(x => x.ReservedAt)
                   .IsRequired();

            builder.Property(x => x.ExpiresAt)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true)
                   .IsRequired();

            builder.HasIndex(x => new { x.BouquetId, x.SizeId, x.IsActive });
            builder.HasIndex(x => x.ExpiresAt);
        }
    }
}