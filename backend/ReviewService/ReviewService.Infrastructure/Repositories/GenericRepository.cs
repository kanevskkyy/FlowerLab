using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Interfaces;

namespace ReviewService.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected IMongoCollection<T> collection;
        protected IClientSessionHandle? session;

        public GenericRepository(IMongoDatabase database, IClientSessionHandle? session = null)
        {
            collection = typeof(T).Name switch
            {
                nameof(Review) => database.GetCollection<T>("reviews"),
                _ => throw new ArgumentException($"Не знайдено відповідності колекції для типу {typeof(T).Name}")
            };
            this.session = session;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (session != null)
                await collection.InsertOneAsync(session, entity, cancellationToken: cancellationToken);
            else
                await collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await collection.Find(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await collection.ReplaceOneAsync(e => e.Id == entity.Id, entity, cancellationToken: cancellationToken);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await collection.DeleteOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await collection.Find(_ => true).ToListAsync(cancellationToken);
        }
    }
}