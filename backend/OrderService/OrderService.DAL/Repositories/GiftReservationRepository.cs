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
    public class GiftReservationRepository : GenericRepository<GiftReservation>, IGiftReservationRepository
    {
        public GiftReservationRepository(OrderDbContext context) : base(context)
        {

        }

        public async Task<List<GiftReservation>> GetActiveByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await context.Set<GiftReservation>()
                        .Where(gr => gr.OrderId == orderId && gr.IsActive)
                        .ToListAsync(cancellationToken);
        }

        public async Task<List<GiftReservation>> GetActiveAsync(DateTime activeAt, CancellationToken cancellationToken = default)
        {
            return await context.Set<GiftReservation>()
                .Where(r => r.IsActive && r.ExpiresAt > activeAt)
                .ToListAsync(cancellationToken);
        }
    }
}
