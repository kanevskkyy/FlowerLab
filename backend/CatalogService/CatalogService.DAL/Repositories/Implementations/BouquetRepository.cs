using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Interfaces;
using CatalogService.DAL.Specification;
using CatalogService.Domain.Entities;
using CatalogService.Domain.QueryParametrs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.DAL.Helpers;

namespace CatalogService.DAL.Repositories.Implementations
{
    public class BouquetRepository : GenericRepository<Bouquet>, IBouquetRepository
    {
        public BouquetRepository(CatalogDbContext context) : base(context) { }

        public override async Task<IEnumerable<Bouquet>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Include(b => b.BouquetFlowers).ThenInclude(f => f.Flower)
                .Include(b => b.BouquetSizes).ThenInclude(s => s.Size)
                .Include(b => b.BouquetEvents).ThenInclude(e => e.Event)
                .Include(b => b.BouquetRecipients).ThenInclude(r => r.Recipient)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Bouquet?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Bouquets
                .Include(b => b.BouquetSizes)
                    .ThenInclude(bs => bs.Size)
                .Include(b => b.BouquetSizes)
                    .ThenInclude(bs => bs.BouquetSizeFlowers)
                        .ThenInclude(bsf => bsf.Flower)
                .Include(b => b.BouquetSizes)
                    .ThenInclude(bs => bs.BouquetImages) 
                .Include(b => b.BouquetEvents)
                    .ThenInclude(be => be.Event)
                .Include(b => b.BouquetRecipients)
                    .ThenInclude(br => br.Recipient)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }


        public async Task<PagedList<Bouquet>> GetBySpecificationPagedAsync(BouquetQueryParameters parameters, CancellationToken cancellationToken = default)
        {
            var spec = new BouquetSpecification(parameters);
            var query = SpecificationEvaluator<Bouquet>.GetQuery(dbSet.AsQueryable(), spec);

            // Restoring necessary Includes that might not be handled by the simple Specification Evaluator
            query = query
                .Include(b => b.BouquetFlowers).ThenInclude(bf => bf.Flower)
                .Include(b => b.BouquetSizes).ThenInclude(bs => bs.Size)
                .Include(b => b.BouquetSizes).ThenInclude(bs => bs.BouquetSizeFlowers).ThenInclude(bsf => bsf.Flower)
                .Include(b => b.BouquetEvents).ThenInclude(be => be.Event)
                .Include(b => b.BouquetRecipients).ThenInclude(br => br.Recipient);
            
            var totalCount = await query.CountAsync(cancellationToken);
            
            var items = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return PagedList<Bouquet>.Create(items, totalCount, parameters.Page, parameters.PageSize);
        }

        public async Task<(decimal minPrice, decimal maxPrice)> GetMinAndMaxPriceAsync(CancellationToken cancellationToken = default)
        {
            var bouquetMinPrices = await context.BouquetSizes
                .AsNoTracking()
                .GroupBy(bs => bs.BouquetId)
                .Select(g => g.Min(bs => bs.Price)) 
                .ToListAsync(cancellationToken);

            if (!bouquetMinPrices.Any()) return (0m, 0m); 

            decimal minPrice = bouquetMinPrices.Min(); 
            decimal maxPrice = bouquetMinPrices.Max(); 

            return (minPrice, maxPrice);
        }

        public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var bouquets = await dbSet.AsNoTracking().ToListAsync(cancellationToken);
            return bouquets.Any(b => (b.Name.GetValueOrDefault("ua") == name || b.Name.GetValueOrDefault("en") == name) && (!excludeId.HasValue || b.Id != excludeId.Value));
        }

        public void DeleteImages(IEnumerable<BouquetImage> images)
        {
            context.BouquetImages.RemoveRange(images);
        }

        public void AddImage(BouquetImage image)
        {
            context.BouquetImages.Add(image);
        }
    }
}
