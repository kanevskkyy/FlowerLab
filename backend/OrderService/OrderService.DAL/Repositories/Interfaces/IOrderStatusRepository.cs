using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Domain.Entities;

namespace OrderService.DAL.Repositories.Interfaces
{
    public interface IOrderStatusRepository : IGenericRepository<OrderStatus>
    {
        Task<OrderStatus> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> IsNameDuplicatedAsync(string name, Guid? ignoreId = null, CancellationToken cancellationToken = default);
    }
}
