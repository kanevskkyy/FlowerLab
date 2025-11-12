using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class BouquetRecipient
    {
        public Guid BouquetId { get; set; }
        public Bouquet Bouquet { get; set; } = null!;

        public Guid RecipientId { get; set; }
        public Recipient Recipient { get; set; } = null!;
    }
}
