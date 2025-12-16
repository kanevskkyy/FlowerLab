using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderService.DAL.Repositories.Interfaces;
using OrderService.Domain.Database;

namespace OrderService.DAL.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private OrderDbContext context;
        private IDbContextTransaction? transaction;

        public IOrderRepository Orders { get; }
        public IGiftRepository Gifts { get; }
        public IOrderStatusRepository OrderStatuses { get; }  

        public UnitOfWork(OrderDbContext context, IOrderRepository orders, IGiftRepository gifts, IOrderStatusRepository orderStatuses)
        {
            this.context = context;
            Orders = orders;
            Gifts = gifts;
            OrderStatuses = orderStatuses;
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