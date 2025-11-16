using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class BouquetFlower
    {
        public Guid BouquetId { get; set; }
        public Bouquet Bouquet { get; set; } = null!;

        public Guid FlowerId { get; set; }
        public Flower Flower { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
