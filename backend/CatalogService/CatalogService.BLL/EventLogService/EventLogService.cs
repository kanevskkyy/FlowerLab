using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.DAL.Context;
using Microsoft.EntityFrameworkCore;
using shared.events;

namespace CatalogService.BLL.EventLogService
{
    public class EventLogService : IEventLogService
    {
        public CatalogDbContext context;

        public EventLogService(CatalogDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> HasEventProcessedAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await context.Set<ProcessedEvent>().AnyAsync(property => property.EventId == eventId, cancellationToken);

        }

        public async Task MarkEventAsProcessedAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            context.Set<ProcessedEvent>().Add(new ProcessedEvent
            {
                EventId = eventId
            });
            await context.SaveChangesAsync(cancellationToken);
        }
    }

}
