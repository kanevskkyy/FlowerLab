using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Interfaces;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.DAL.Repositories.Implementations
{
    public class RecipientRepository : GenericRepository<Recipient>, IRecipientRepository
    {
        public RecipientRepository(CatalogDbContext context) : base(context) { }

        public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            return await dbSet.AnyAsync(r => r.Name == name && (!excludeId.HasValue || r.Id != excludeId.Value), cancellationToken);
        }
    }
}
