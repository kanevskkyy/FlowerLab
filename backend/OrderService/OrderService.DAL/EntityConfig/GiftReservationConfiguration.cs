using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.DAL.EntityConfig
{
    public class GiftReservationConfiguration : IEntityTypeConfiguration<GiftReservation>
    {
        public void Configure(EntityTypeBuilder<GiftReservation> builder)
        {
            builder.HasKey(gr => gr.Id);

            builder.HasOne(gr => gr.Order)
                   .WithMany(o => o.GiftReservations)
                   .HasForeignKey(gr => gr.OrderId);

            builder.HasOne(gr => gr.Gift)
                   .WithMany()
                   .HasForeignKey(gr => gr.GiftId);

            builder.Property(gr => gr.IsActive).HasDefaultValue(true);
        }
    }
}
