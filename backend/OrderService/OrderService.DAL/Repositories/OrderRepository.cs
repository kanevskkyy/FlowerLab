using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.DAL.Helpers;
using OrderService.DAL.Repositories.Interfaces;
using OrderService.DAL.Specification;
using OrderService.Domain.Database;
using OrderService.Domain.Entities;
using OrderService.Domain.QueryParams;

namespace OrderService.DAL.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(OrderDbContext context) : base(context)
        {

        }

        public async Task<Order?> GetByIdWithIncludesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(o => o.Status)
                .Include(o => o.Items)
                .Include(o => o.OrderGifts)
                .Include(o => o.DeliveryInformation)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<PagedList<Order>> GetPagedOrdersAsync(OrderSpecificationParameters parameters, CancellationToken cancellationToken = default)
        {
            OrderSpecification spec = new OrderSpecification(parameters);

            var query = ApplySpecification(spec, cancellationToken);

            int totalCount = await query.CountAsync(cancellationToken);
            List<Order> items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedList<Order>(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }
    }
}
