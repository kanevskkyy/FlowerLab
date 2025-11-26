using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class BouquetSizeFlower
    {
        public Guid BouquetId { get; set; }
        public Guid SizeId { get; set; }
        public BouquetSize BouquetSize { get; set; } = null!;

        public Guid FlowerId { get; set; }
        public Flower Flower { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
