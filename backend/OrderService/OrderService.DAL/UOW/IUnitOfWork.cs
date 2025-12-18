using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.DAL.Repositories.Interfaces;

namespace OrderService.DAL.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IOrderRepository Orders { get; }
        IGiftRepository Gifts { get; }
        IOrderReservationRepository OrderReservations { get; }
        IOrderStatusRepository OrderStatuses { get; }
        IGiftReservationRepository GiftReservations { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
