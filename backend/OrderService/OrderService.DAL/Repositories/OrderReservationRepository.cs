using Microsoft.EntityFrameworkCore;
using OrderService.DAL.Repositories.Interfaces;
using OrderService.Domain.Database;
using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.DAL.Repositories
{
    public class OrderReservationRepository : GenericRepository<OrderReservation>, IOrderReservationRepository
    {
        public OrderReservationRepository(OrderDbContext context) : base(context)
        {

        }

        public async Task<IReadOnlyList<OrderReservation>> GetActiveAsync(DateTime now, CancellationToken cancellationToken = default)
        {
            return await context.OrderReservations
                            .Where(r => r.IsActive && r.ExpiresAt > now)
                            .AsNoTracking()
                            .ToListAsync(cancellationToken);
        }
    }
}
