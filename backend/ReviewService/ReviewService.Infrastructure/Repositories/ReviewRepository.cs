using MongoDB.Driver;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Entities.QueryParameters;
using ReviewService.Domain.Helpers;
using ReviewService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using ReviewService.Infrastructure.DB;

namespace ReviewService.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly ILogger<ReviewRepository> _logger;
        private readonly IMongoCollection<Review> _reviews;

        public ReviewRepository(
            IMongoDatabase database,
            ILogger<ReviewRepository> logger,
            IClientSessionHandle? session = null)
            : base(database, session)
        {
            _logger = logger;
            _reviews = database.GetCollection<Review>("reviews");
        }

        public async Task<PagedList<Review>> GetReviewsAsync(ReviewQueryParameters queryParameters, CancellationToken cancellationToken = default)
        {
            var filterBuilder = Builders<Review>.Filter;
            var filter = filterBuilder.Empty;

            _logger.LogInformation("=== GetReviewsAsync START ===");
            _logger.LogInformation($"UserId: {queryParameters.UserId}");
            _logger.LogInformation($"BouquetId: {queryParameters.BouquetId}");
            _logger.LogInformation($"Rating: {queryParameters.Rating}");
            _logger.LogInformation($"Status: {queryParameters.Status}");
            _logger.LogInformation($"PageNumber: {queryParameters.PageNumber}");
            _logger.LogInformation($"PageSize: {queryParameters.PageSize}");

            if (queryParameters.UserId.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.User.UserId, queryParameters.UserId.Value);
                _logger.LogInformation("✓ Фільтр UserId додано");
            }

            if (queryParameters.BouquetId.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.BouquetId, queryParameters.BouquetId.Value);
                _logger.LogInformation("✓ Фільтр BouquetId додано");

                if (!queryParameters.Status.HasValue)
                {
                    filter &= filterBuilder.Eq(r => r.Status, ReviewStatus.Confirmed);
                    _logger.LogInformation("✓ Статус за замовчуванням = Confirmed");
                }
            }

            if (queryParameters.Rating.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.Rating, queryParameters.Rating.Value);
                _logger.LogInformation("✓ Фільтр Rating додано");
            }

            if (queryParameters.Status.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.Status, queryParameters.Status.Value);
                _logger.LogInformation("✓ Фільтр Status додано явно");
            }

            try
            {
                var rendered = filter.Render(_reviews.DocumentSerializer, _reviews.Settings.SerializerRegistry);
                _logger.LogInformation($"MongoDB Filter JSON: {rendered}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка рендерингу фільтра: {ex.Message}");
            }

            var totalCount = await _reviews.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            _logger.LogInformation($"Загальна кількість документів за фільтром: {totalCount}");

            var findFluent = _reviews.Find(filter);

            if (!string.IsNullOrWhiteSpace(queryParameters.OrderBy))
            {
                var sortHelper = new MongoSortHelper<Review>();
                findFluent = findFluent.Sort(sortHelper.ApplySort(queryParameters.OrderBy));
                _logger.LogInformation($"Сортування: {queryParameters.OrderBy}");
            }
            else
            {
                findFluent = findFluent.SortByDescending(r => r.CreatedAt);
                _logger.LogInformation("Сортування: CreatedAt DESC (за замовчуванням)");
            }

            var result = await PagedList<Review>.ToPagedListAsync(
                findFluent,
                queryParameters.PageNumber,
                queryParameters.PageSize,
                cancellationToken
            );

            _logger.LogInformation($"PagedList - TotalCount: {result.TotalCount}, Items.Count: {result.Items.Count}, CurrentPage: {result.CurrentPage}, TotalPages: {result.TotalPages}");
            _logger.LogInformation("=== GetReviewsAsync END ===");

            return result;
        }

        public async Task DeleteByBouquetIdAsync(Guid bouquetId, CancellationToken cancellationToken)
        {
            var filter = Builders<Review>.Filter.Eq(r => r.BouquetId, bouquetId);
            await _reviews.DeleteManyAsync(filter, cancellationToken);
        }

        public async Task<bool> HasUserReviewedBouquetAsync(Guid userId, Guid bouquetId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Review>.Filter.Eq(r => r.User.UserId, userId) &
                         Builders<Review>.Filter.Eq(r => r.BouquetId, bouquetId);

            long count = await _reviews.CountDocumentsAsync(filter);
            return count > 0;
        }
    }
}