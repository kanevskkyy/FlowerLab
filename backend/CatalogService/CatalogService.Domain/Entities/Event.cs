using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class Event : BaseEntity
    {
        public Dictionary<string, string> Name { get; set; } = new();
        public ICollection<BouquetEvent> BouquetEvents { get; set; } = new List<BouquetEvent>();
    }
}
