using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.DAL.Repositories.Interfaces;
using OrderService.Domain.Database;
using OrderService.Domain.Entities;

namespace OrderService.DAL.Repositories
{
    public class GiftRepository : GenericRepository<Gift>, IGiftRepository
    {
        public GiftRepository(OrderDbContext context) : base(context)
        {

        }

        public async Task<bool> IsNameDuplicatedAsync(string name, Guid? ignoreId = null, CancellationToken cancellationToken = default)
        {
            // Fetch names to avoid LINQ translation issues with JSONB dictionary indexers and .ToLower()
            var gifts = await dbSet
                .AsNoTracking()
                .Where(g => ignoreId == null || g.Id != ignoreId)
                .Select(g => g.Name)
                .ToListAsync(cancellationToken);

            return gifts.Any(n => n.Values.Any(v => v != null && v.Trim().Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)));
        }
    }
}
