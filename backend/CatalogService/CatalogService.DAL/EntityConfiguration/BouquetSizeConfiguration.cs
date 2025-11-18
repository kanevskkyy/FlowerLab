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
    public class BouquetSizeConfiguration : IEntityTypeConfiguration<BouquetSize>
    {
        public void Configure(EntityTypeBuilder<BouquetSize> builder)
        {
            builder.ToTable("BouquetSizes");
            builder.HasKey(bs => new { bs.BouquetId, bs.SizeId });
        }
    }
}
