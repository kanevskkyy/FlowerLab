using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Interfaces;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.DAL.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected CatalogDbContext context;
        protected DbSet<T> dbSet;

        public GenericRepository(CatalogDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await dbSet.AnyAsync(predicate, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }
    }
}
