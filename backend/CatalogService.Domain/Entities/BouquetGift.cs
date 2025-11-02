using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class BouquetGift
    {
        public Guid BouquetId { get; set; }
        public Bouquet Bouquet { get; set; } = null!;

        public Guid GiftId { get; set; }
        public Gift Gift { get; set; } = null!;
    }
}
