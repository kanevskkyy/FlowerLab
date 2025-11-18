using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class BouquetEvent
    {
        public Guid BouquetId { get; set; }
        public Bouquet Bouquet { get; set; } = null!;

        public Guid EventId { get; set; }
        public Event Event { get; set; } = null!;
    }
}
