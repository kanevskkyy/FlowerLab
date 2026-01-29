using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using CatalogService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.DAL.Repositories.Implementations
{
    public class SizeRepository : GenericRepository<Size>, ISizeRepository
    {
        public SizeRepository(CatalogDbContext context) : base(context) { }

        public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            return await dbSet.AnyAsync(s => (s.Name["ua"] == name || s.Name["en"] == name) && (!excludeId.HasValue || s.Id != excludeId.Value), cancellationToken);
        }
    }
}
