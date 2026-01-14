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

        public async Task<bool> HasUserOrderedBouquetAsync(Guid userId, Guid bouquetId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Include(o => o.Status)
                .Include(o => o.Items)
                .Where(o => o.UserId == userId && o.Status.Name == "Completed"
                            && o.Items.Any(i => i.BouquetId == bouquetId))
                .AnyAsync(cancellationToken);
        }


        public async Task<List<Order>> GetExpiredAwaitingPaymentOrdersAsync(DateTime now, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Include(o => o.Status)
                .Include(o => o.Reservations)
                .Include(o => o.GiftReservations)  
                .Include(o => o.Items)
                    .ThenInclude(i => i.Flowers)
                .Include(o => o.OrderGifts)
                    .ThenInclude(og => og.Gift)
                .Include(o => o.DeliveryInformation)
                .Where(o => o.Status.Name == "AwaitingPayment"
                    && (o.Reservations.Any(r => r.ExpiresAt <= now && r.IsActive)
                        || o.GiftReservations.Any(gr => gr.ExpiresAt <= now && gr.IsActive))) 
                .ToListAsync(cancellationToken);
        }

        public async Task<Order?> GetByIdWithIncludesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Include(o => o.Status)
                .Include(o => o.Reservations)
                .Include(o => o.GiftReservations)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Flowers) 
                .Include(o => o.OrderGifts)
                    .ThenInclude(og => og.Gift)
                .Include(o => o.DeliveryInformation)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<PagedList<Order>> GetPagedOrdersAsync(OrderSpecificationParameters parameters, CancellationToken cancellationToken = default)
        {
            OrderSpecification spec = new OrderSpecification(parameters);
            var query = ApplySpecification(spec, cancellationToken);

            int totalCount = await query.CountAsync(cancellationToken);

            List<Order> items = await query
                .Include(o => o.Status)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Flowers) 
                .Include(o => o.OrderGifts)
                    .ThenInclude(og => og.Gift)
                .Include(o => o.DeliveryInformation)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return PagedList<Order>.Create(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }
    }
}