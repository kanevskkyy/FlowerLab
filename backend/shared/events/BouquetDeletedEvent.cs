using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerLab.Shared.Events
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
