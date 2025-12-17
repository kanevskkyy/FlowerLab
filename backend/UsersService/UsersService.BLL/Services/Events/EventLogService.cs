using Microsoft.EntityFrameworkCore;
using shared.events.EventService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.DAL.DbContext;

namespace UsersService.BLL.Services.Events
{
    public class EventLogService : IEventLogService
    {
        public ApplicationDbContext dbContext;

        public EventLogService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> HasEventProcessedAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<ProcessedEvent>().AnyAsync(property => property.EventId == eventId, cancellationToken);

        }

        public async Task MarkEventAsProcessedAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            dbContext.Set<ProcessedEvent>().Add(new ProcessedEvent
            {
                EventId = eventId
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
