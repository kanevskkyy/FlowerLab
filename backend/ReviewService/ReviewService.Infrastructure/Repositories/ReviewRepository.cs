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

            _logger.LogInformation("=== GetReviewsAsync called ===");
            _logger.LogInformation($"UserId: {queryParameters.UserId}");
            _logger.LogInformation($"BouquetId: {queryParameters.BouquetId}");
            _logger.LogInformation($"Rating: {queryParameters.Rating}");
            _logger.LogInformation($"Status: {queryParameters.Status}");

            // --- UserId ---
            if (queryParameters.UserId.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.User.UserId, queryParameters.UserId.Value);
                _logger.LogInformation("Added UserId filter");
            }

            // --- BouquetId - використовуємо lambda вираз, щоб MongoDB driver сам визначив правильний формат ---
            if (queryParameters.BouquetId.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.BouquetId, queryParameters.BouquetId.Value);
                _logger.LogInformation($"Added BouquetId filter: {queryParameters.BouquetId.Value}");
            }

            // --- Rating ---
            if (queryParameters.Rating.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.Rating, queryParameters.Rating.Value);
                _logger.LogInformation("Added Rating filter");
            }

            // --- Status ---
            if (queryParameters.Status.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.Status, queryParameters.Status.Value);
                _logger.LogInformation($"Added Status filter: {queryParameters.Status.Value}");
            }
            else if (queryParameters.BouquetId.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.Status, ReviewStatus.Confirmed);
                _logger.LogInformation("Added default Status filter: Confirmed");
            }

            // --- Лог фінального фільтру ---
            var renderedFilter = filter.Render(collection.DocumentSerializer, collection.Settings.SerializerRegistry);
            _logger.LogInformation($"Final MongoDB filter: {renderedFilter}");

            // --- Запит ---
            var findFluent = collection.Find(filter);

            // --- Підрахунок всіх документів ---
            var totalCount = await findFluent.CountDocumentsAsync(cancellationToken);
            _logger.LogInformation($"Total documents found: {totalCount}");

            // --- Сортування ---
            if (!string.IsNullOrWhiteSpace(queryParameters.OrderBy))
            {
                var sortHelper = new MongoSortHelper<Review>();
                findFluent = findFluent.Sort(sortHelper.ApplySort(queryParameters.OrderBy));
            }

            // --- Пагінація ---
            var result = await PagedList<Review>.ToPagedListAsync(
                findFluent,
                queryParameters.PageNumber,
                queryParameters.PageSize,
                cancellationToken
            );

            _logger.LogInformation($"Returning {result.Items.Count} items from page {queryParameters.PageNumber}");

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