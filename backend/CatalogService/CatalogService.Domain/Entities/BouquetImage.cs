using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class BouquetImage : BaseEntity
    {
        public Guid BouquetId { get; set; }
        public Bouquet Bouquet { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public short Position { get; set; }  
    }
}
