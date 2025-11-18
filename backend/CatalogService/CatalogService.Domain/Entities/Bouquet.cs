using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class Bouquet : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string MainPhotoUrl { get; set; } = null!;
        public decimal Price { get; set; }

        public ICollection<BouquetFlower> BouquetFlowers { get; set; } = new List<BouquetFlower>();
        public ICollection<BouquetImage> BouquetImages { get; set; } = new List<BouquetImage>();
        public ICollection<BouquetSize> BouquetSizes { get; set; } = new List<BouquetSize>();
        public ICollection<BouquetEvent> BouquetEvents { get; set; } = new List<BouquetEvent>();
        public ICollection<BouquetRecipient> BouquetRecipients { get; set; } = new List<BouquetRecipient>();
    }
}
