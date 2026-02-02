using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using CatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.DAL.Repositories.Implementations
{
    public class SizeRepository : GenericRepository<Size>, ISizeRepository
    {
        public SizeRepository(CatalogDbContext context) : base(context) { }

        public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var lowercaseName = name.Trim().ToLower();
            
            var items = await dbSet.AsNoTracking().ToListAsync(cancellationToken);
            
            return items.Any(s => 
                (s.Name != null && s.Name.Values.Any(v => v != null && v.Trim().ToLower() == lowercaseName)) && 
                (!excludeId.HasValue || s.Id != excludeId.Value));
        }
    }
}
