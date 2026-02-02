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
    public class FlowerRepository : GenericRepository<Flower>, IFlowerRepository
    {
        public FlowerRepository(CatalogDbContext context) : base(context) { }

        public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var lowercaseName = name.Trim().ToLower();
            
            // Fetch everything to memory first to avoid ANY translation issues with JSONB dictionaries.
            // AsNoTracking() is important for performance and to avoid caching issues here.
            var flowersList = await dbSet.AsNoTracking().ToListAsync(cancellationToken);
            
            // Explicitly cast to IEnumerable to force Linq-to-Objects (client evaluation).
            IEnumerable<Flower> flowers = flowersList;
            
            return flowers.Any(f => 
            {
                if (f.Name == null) return false;
                
                bool matchesName = f.Name.Values.Any(v => v != null && v.Trim().ToLower() == lowercaseName);
                bool isNotExcluded = !excludeId.HasValue || f.Id != excludeId.Value;
                
                return matchesName && isNotExcluded;
            });
        }
    }
}
