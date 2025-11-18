using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class Event : BaseEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<BouquetEvent> BouquetEvents { get; set; } = new List<BouquetEvent>();
    }
}
