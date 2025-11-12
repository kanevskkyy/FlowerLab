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
    public class OrderStatusRepository : GenericRepository<OrderStatus>, IOrderStatusRepository
    {
        public OrderStatusRepository(OrderDbContext context) : base(context)
        {
        }

        public async Task<OrderStatus?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();
            return await query.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower(), cancellationToken);
        }

        public async Task<bool> IsNameDuplicatedAsync(string name, Guid? ignoreId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();

            if (ignoreId.HasValue)
                query = query.Where(s => s.Id != ignoreId.Value);

            return await query.AnyAsync(s => s.Name.ToLower() == name.ToLower(), cancellationToken);
        }
    }
}
