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
        private readonly OrderDbContext _context;
        private IDbContextTransaction? _transaction;

        public IOrderRepository Orders { get; }
        public IGiftRepository Gifts { get; }
        public IOrderStatusRepository OrderStatuses { get; }  

        public UnitOfWork(OrderDbContext context, IOrderRepository orders, IGiftRepository gifts, IOrderStatusRepository orderStatuses)
        {
            _context = context;
            Orders = orders;
            Gifts = gifts;
            OrderStatuses = orderStatuses;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_context.Database.CurrentTransaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            }

            try
            {
                var result = await _context.SaveChangesAsync(cancellationToken);
                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
                return result;
            }
            catch
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync(cancellationToken);
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
                throw;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}