using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.DAL.Repositories.Interfaces
{
    public interface IOrderReservationRepository : IGenericRepository<OrderReservation>
    {
        public Task<IReadOnlyList<OrderReservation>> GetActiveAsync(DateTime now, CancellationToken cancellationToken = default);
    }
}
