using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Interfaces;
using CatalogService.DAL.Specification;
using CatalogService.Domain.Entities;
using CatalogService.Domain.QueryParametrs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.DAL.Helpers;

namespace CatalogService.DAL.Repositories.Implementations
{
    public class BouquetRepository : GenericRepository<Bouquet>, IBouquetRepository
    {
        public BouquetRepository(CatalogDbContext context) : base(context) { }

        public override async Task<IEnumerable<Bouquet>> GetAllAsync()
        {
            return await _dbSet
                .Include(b => b.BouquetFlowers).ThenInclude(f => f.Flower)
                .Include(b => b.BouquetSizes).ThenInclude(s => s.Size)
                .Include(b => b.BouquetEvents).ThenInclude(e => e.Event)
                .Include(b => b.BouquetRecipients).ThenInclude(r => r.Recipient)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Bouquet?> GetWithDetailsAsync(Guid id)
        {
            return await _context.Bouquets
                .Include(b => b.BouquetSizes)
                    .ThenInclude(bs => bs.Size)
                .Include(b => b.BouquetSizes)
                    .ThenInclude(bs => bs.BouquetSizeFlowers)
                        .ThenInclude(bsf => bsf.Flower)
                .Include(b => b.BouquetEvents)
                    .ThenInclude(be => be.Event)
                .Include(b => b.BouquetRecipients)
                    .ThenInclude(br => br.Recipient)
                .Include(b => b.BouquetImages)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<PagedList<Bouquet>> GetBySpecificationPagedAsync(BouquetQueryParameters parameters)
        {
            var spec = new BouquetSpecification(parameters);
            var query = SpecificationEvaluator<Bouquet>.GetQuery(_dbSet.AsQueryable(), spec);

            query = query
                .Include(b => b.BouquetFlowers).ThenInclude(bf => bf.Flower)
                .Include(b => b.BouquetSizes).ThenInclude(bs => bs.Size)
                .Include(b => b.BouquetSizes).ThenInclude(bs => bs.BouquetSizeFlowers).ThenInclude(bsf => bsf.Flower);

            query = parameters.SortBy switch
            {
                "price_asc" => query.OrderBy(b => b.BouquetSizes.Min(bs => bs.Price)),
                "price_desc" => query.OrderByDescending(b => b.BouquetSizes.Min(bs => bs.Price)),
                _ => query.OrderByDescending(b => b.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedList<Bouquet>(items, totalCount, parameters.Page, parameters.PageSize);
        }

    }
}
