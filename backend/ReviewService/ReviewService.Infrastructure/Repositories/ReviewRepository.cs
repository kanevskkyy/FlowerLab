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
using Microsoft.Extensions.Logging;

namespace ReviewService.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly ILogger<ReviewRepository> _logger;

        public ReviewRepository(
            IMongoDatabase database,
            ILogger<ReviewRepository> logger,
            IClientSessionHandle? session = null)
            : base(database, session)
        {
            _logger = logger;
        }

        public async Task<PagedList<Review>> GetReviewsAsync(
            ReviewQueryParameters queryParameters,
            CancellationToken cancellationToken = default)
        {
            var filterBuilder = Builders<Review>.Filter;
            FilterDefinition<Review> filter = filterBuilder.Empty;

            _logger.LogInformation("=== Виклик GetReviewsAsync ===");
            _logger.LogInformation($"UserId: {queryParameters.UserId}");
            _logger.LogInformation($"BouquetId: {queryParameters.BouquetId}");
            _logger.LogInformation($"Rating: {queryParameters.Rating}");
            _logger.LogInformation($"Status: {queryParameters.Status}");

            if (queryParameters.UserId.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.User.UserId, queryParameters.UserId.Value);
                _logger.LogInformation("Додано фільтр UserId");
            }

            if (queryParameters.BouquetId.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.BouquetId, queryParameters.BouquetId.Value);
                _logger.LogInformation($"Додано фільтр BouquetId: {queryParameters.BouquetId.Value}");
            }

            if (queryParameters.Rating.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.Rating, queryParameters.Rating.Value);
                _logger.LogInformation("Додано фільтр Rating");
            }

            if (queryParameters.Status.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.Status, queryParameters.Status.Value);
                _logger.LogInformation($"Додано фільтр Status: {queryParameters.Status.Value}");
            }
            else if (queryParameters.BouquetId.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.Status, ReviewStatus.Confirmed);
                _logger.LogInformation("Додано фільтр за замовчуванням Status: Confirmed");
            }

            var renderedFilter = filter.Render(collection.DocumentSerializer, collection.Settings.SerializerRegistry);
            _logger.LogInformation($"Фінальний фільтр MongoDB: {renderedFilter}");

            var findFluent = collection.Find(filter);

            var totalCount = await findFluent.CountDocumentsAsync(cancellationToken);
            _logger.LogInformation($"Знайдено всього документів: {totalCount}");

            if (!string.IsNullOrWhiteSpace(queryParameters.OrderBy))
            {
                var sortHelper = new MongoSortHelper<Review>();
                findFluent = findFluent.Sort(sortHelper.ApplySort(queryParameters.OrderBy));
            }

            var result = await PagedList<Review>.ToPagedListAsync(
                findFluent,
                queryParameters.PageNumber,
                queryParameters.PageSize,
                cancellationToken
            );

            _logger.LogInformation($"Повертається {result.Items.Count} елементів зі сторінки {queryParameters.PageNumber}");

            return result;
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