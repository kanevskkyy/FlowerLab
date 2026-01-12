using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class Flower : BaseEntity
    {
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }  

        public ICollection<BouquetFlower> BouquetFlowers { get; set; } = new List<BouquetFlower>();
        public ICollection<BouquetSizeFlower> BouquetSizeFlowers { get; set; } = new List<BouquetSizeFlower>();
    }
}
