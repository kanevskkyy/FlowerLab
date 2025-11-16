using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.DAL.Helpers;
using OrderService.Domain.Entities;
using OrderService.Domain.QueryParams;

namespace OrderService.DAL.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<PagedList<Order>> GetPagedOrdersAsync(OrderSpecificationParameters parameters, CancellationToken cancellationToken = default);
        Task<Order?> GetByIdWithIncludesAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
