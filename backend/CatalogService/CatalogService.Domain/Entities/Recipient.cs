using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class Recipient : BaseEntity
    {
        public Dictionary<string, string> Name { get; set; } = new();
        public ICollection<BouquetRecipient> BouquetRecipients { get; set; } = new List<BouquetRecipient>();
    }
}
