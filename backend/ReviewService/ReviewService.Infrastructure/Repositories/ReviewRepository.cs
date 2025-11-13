using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using ReviewService.Domain.Entities.QueryParameters;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Helpers;
using ReviewService.Domain.Interfaces;
using ReviewService.Infrastructure.DB;

namespace ReviewService.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(MongoDbContext context, IClientSessionHandle? session = null) : base(context, session)
        {

        }

        public async Task<PagedList<Review>> GetReviewsAsync(ReviewQueryParameters queryParameters, CancellationToken cancellationToken = default)
        {
            var filterBuilder = Builders<Review>.Filter;
            var filter = filterBuilder.Empty;

            if (queryParameters.UserId.HasValue)
                filter &= filterBuilder.Eq(r => r.User.UserId, queryParameters.UserId.Value);

            if (queryParameters.BouquetId.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.BouquetId, queryParameters.BouquetId.Value);

                if (!queryParameters.Status.HasValue)
                    filter &= filterBuilder.Eq(r => r.Status, ReviewStatus.Confirmed);
            }

            if (queryParameters.Rating.HasValue)
                filter &= filterBuilder.Eq(r => r.Rating, queryParameters.Rating.Value);

            if (queryParameters.Status.HasValue)
                filter &= filterBuilder.Eq(r => r.Status, queryParameters.Status.Value);

            IFindFluent<Review, Review> findFluent = collection.Find(filter);

            if (!string.IsNullOrWhiteSpace(queryParameters.OrderBy))
            {
                MongoSortHelper<Review> sortHelper = new MongoSortHelper<Review>();
                findFluent = findFluent.Sort(sortHelper.ApplySort(queryParameters.OrderBy));
            }

            return await PagedList<Review>.ToPagedListAsync(findFluent, queryParameters.PageNumber, queryParameters.PageSize, cancellationToken);
        }

        public async Task<bool> HasUserReviewedBouquetAsync(Guid userId, Guid bouquetId, CancellationToken cancellationToken = default)
        {
            var filterBuilder = Builders<Review>.Filter;
            var filter = filterBuilder.Eq(r => r.User.UserId, userId) &
                         filterBuilder.Eq(r => r.BouquetId, bouquetId);

            long count = await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            return count > 0;
        }
    }
}
