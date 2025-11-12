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
    public class DeliveryInformationConfiguration : IEntityTypeConfiguration<DeliveryInformation>
    {
        public void Configure(EntityTypeBuilder<DeliveryInformation> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Address).IsRequired().HasMaxLength(200);
        }
    }

}
