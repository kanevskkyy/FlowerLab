using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class Bouquet : BaseEntity
    {
        public Dictionary<string, string> Name { get; set; } = new();
        public Dictionary<string, string>? Description { get; set; }
        public string MainPhotoUrl { get; set; } = null!;

        public ICollection<BouquetFlower> BouquetFlowers { get; set; } = new List<BouquetFlower>();
        public ICollection<BouquetSize> BouquetSizes { get; set; } = new List<BouquetSize>();
        public ICollection<BouquetEvent> BouquetEvents { get; set; } = new List<BouquetEvent>();
        public ICollection<BouquetRecipient> BouquetRecipients { get; set; } = new List<BouquetRecipient>();
    }
}
