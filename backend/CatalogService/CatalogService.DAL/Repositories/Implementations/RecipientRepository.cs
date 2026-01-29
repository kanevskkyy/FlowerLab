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
            var recipients = await dbSet.ToListAsync(cancellationToken);
            return recipients.Any(r => (r.Name.GetValueOrDefault("ua") == name || r.Name.GetValueOrDefault("en") == name) && (!excludeId.HasValue || r.Id != excludeId.Value));
        }
    }
}
