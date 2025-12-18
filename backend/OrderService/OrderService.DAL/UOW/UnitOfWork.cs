using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderService.DAL.Repositories;
using OrderService.DAL.Repositories.Interfaces;
using OrderService.Domain.Database;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.DAL.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private OrderDbContext context;
        private IDbContextTransaction? transaction;

        public IOrderRepository Orders { get; }
        public IGiftRepository Gifts { get; }
        public IOrderStatusRepository OrderStatuses { get; }
        public IOrderReservationRepository OrderReservations { get; }
        public IGiftReservationRepository GiftReservations { get; }


        public UnitOfWork(OrderDbContext context, IOrderRepository orders, IGiftRepository gifts, IOrderStatusRepository orderStatuses, IOrderReservationRepository orderReservations, IGiftReservationRepository giftReservationRepository)
        {
            this.context = context;
            GiftReservations = giftReservationRepository;
            Orders = orders;
            Gifts = gifts;
            OrderStatuses = orderStatuses;
            OrderReservations = orderReservations;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            transaction?.Dispose();
            context.Dispose();
        }
    }
}