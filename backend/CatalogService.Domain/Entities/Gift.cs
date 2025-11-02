using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class Gift : BaseEntity
    {
        public string GiftType { get; set; } = null!; 
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<BouquetGift> BouquetGifts { get; set; } = new List<BouquetGift>();
    }
}
