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
    public class BouquetGiftConfiguration : IEntityTypeConfiguration<BouquetGift>
    {
        public void Configure(EntityTypeBuilder<BouquetGift> builder)
        {
            builder.ToTable("BouquetGifts");
            builder.HasKey(bg => new { bg.BouquetId, bg.GiftId });
        }
    }
}
