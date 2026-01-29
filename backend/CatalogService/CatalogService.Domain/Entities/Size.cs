using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class Size : BaseEntity
    {
        public Dictionary<string, string> Name { get; set; } = new();
        public ICollection<BouquetSize> BouquetSizes { get; set; } = new List<BouquetSize>();
    }
}
