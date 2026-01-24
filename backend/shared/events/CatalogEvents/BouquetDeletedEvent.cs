using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.events.Catalog
{
    public record BouquetDeletedEvent
    {
        public Guid BouquetId { get; init; }
        public Guid EventId { get; init; } = Guid.NewGuid();

        public BouquetDeletedEvent(Guid bouquetId)
        {
            BouquetId = bouquetId;
        }
    }
}
