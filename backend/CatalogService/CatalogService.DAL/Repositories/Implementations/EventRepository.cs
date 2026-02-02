using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Interfaces;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.DAL.Repositories.Implementations
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(CatalogDbContext context) : base(context) { }

        public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var lowercaseName = name.Trim().ToLower();

            var events = await dbSet.AsNoTracking().ToListAsync(cancellationToken);
            
            return events.Any(e => 
                (e.Name != null && e.Name.Values.Any(v => v != null && v.Trim().ToLower() == lowercaseName)) && 
                (!excludeId.HasValue || e.Id != excludeId.Value));
        }
    }
}
