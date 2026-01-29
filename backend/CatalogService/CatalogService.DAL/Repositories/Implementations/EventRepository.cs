using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Interfaces;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.DAL.Repositories.Implementations
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(CatalogDbContext context) : base(context) { }

        public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var events = await dbSet.ToListAsync(cancellationToken);
            return events.Any(e => (e.Name.GetValueOrDefault("ua") == name || e.Name.GetValueOrDefault("en") == name) && (!excludeId.HasValue || e.Id != excludeId.Value));
        }
    }
}
