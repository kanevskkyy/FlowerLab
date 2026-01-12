using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class BouquetSize
    {
        public Guid BouquetId { get; set; }
        public Bouquet Bouquet { get; set; } = null!;

        public Guid SizeId { get; set; }
        public Size Size { get; set; } = null!;

        public decimal Price { get; set; }

        public ICollection<BouquetSizeFlower> BouquetSizeFlowers { get; set; } = new List<BouquetSizeFlower>();
        public ICollection<BouquetImage> BouquetImages { get; set; } = new List<BouquetImage>();
    }
}
