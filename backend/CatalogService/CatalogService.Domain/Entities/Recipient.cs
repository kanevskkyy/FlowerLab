using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class Recipient : BaseEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<BouquetRecipient> BouquetRecipients { get; set; } = new List<BouquetRecipient>();
    }
}
