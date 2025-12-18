using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.DAL.Repositories.Interfaces
{
    public interface IGiftReservationRepository : IGenericRepository<GiftReservation>
    {
        Task<List<GiftReservation>> GetActiveByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}
