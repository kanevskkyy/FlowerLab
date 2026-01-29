using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Interfaces;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.DAL.Repositories.Implementations
{
    public class FlowerRepository : GenericRepository<Flower>, IFlowerRepository
    {
        public FlowerRepository(CatalogDbContext context) : base(context) { }

        public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var flowers = await dbSet.ToListAsync(cancellationToken);
            return flowers.Any(f => (f.Name.GetValueOrDefault("ua") == name || f.Name.GetValueOrDefault("en") == name) && (!excludeId.HasValue || f.Id != excludeId.Value));
        }
    }
}
