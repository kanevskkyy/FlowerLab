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
            var query = _dbSet.AsQueryable();

            if (ignoreId.HasValue)
                query = query.Where(g => g.Id != ignoreId.Value);

            return await query.AnyAsync(g => g.Name.ToLower() == name.ToLower(), cancellationToken);
        }
    }
}
