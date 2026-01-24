using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.events
{
    public class OrderBouquetItem
    {
        public Guid BouquetId { get; set; }
        public Guid SizeId { get; set; }
        public int Count { get; set; }
    }
}
