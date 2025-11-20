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
using ReviewService.Infrastructure.DB;

namespace ReviewService.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly ILogger<ReviewRepository> _logger;
        // Додаємо прямий доступ до колекції, якщо GenericRepository його не надає публічно або protected
        // Якщо GenericRepository має protected IMongoCollection<T> _collection, використовуйте його.
        // Тут я припускаю, що у вас є доступ до колекції через base.collection або _reviews
        private readonly IMongoCollection<Review> _reviews; 

        public ReviewRepository(
            IMongoDatabase database,
            ILogger<ReviewRepository> logger,
            IClientSessionHandle? session = null)
            : base(database, session)
        {
            _logger = logger;
            _reviews = database.GetCollection<Review>("reviews"); // Або як називається ваша колекція
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

            // --- BouquetId ---
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
                // Якщо статус явно вказано - фільтруємо по ньому
                filter &= filterBuilder.Eq(r => r.Status, queryParameters.Status.Value);
                _logger.LogInformation($"Додано фільтр Status: {queryParameters.Status.Value}");
            }
            else if (queryParameters.BouquetId.HasValue)
            {
                // Логіка за замовчуванням: якщо ми дивимось відгуки конкретного букета,
                // то показуємо ТІЛЬКИ підтверджені (Confirmed).
                // Щоб клієнти не бачили спам або нові неперевірені відгуки.
                filter &= filterBuilder.Eq(r => r.Status, ReviewStatus.Confirmed);
                _logger.LogInformation("Додано фільтр за замовчуванням Status: Confirmed");
            }

            var renderedFilter = filter.Render(collection.DocumentSerializer, collection.Settings.SerializerRegistry);
            _logger.LogInformation($"Фінальний фільтр MongoDB: {renderedFilter}");

            var findFluent = collection.Find(filter);

            var totalCount = await findFluent.CountDocumentsAsync(cancellationToken);
            _logger.LogInformation($"Знайдено всього документів: {totalCount}");
                _logger.LogInformation("Added default Status filter: Confirmed (because BouquetId is present and Status is null)");
            }

            // --- Лог фінального фільтру (для дебагу) ---
            // Render може кинути виняток, якщо серіалізатори не налаштовані, тому краще обгорнути в try/catch або прибрати в продакшені
            try 
            {
                // В старих версіях драйвера Render приймає інші аргументи.
                // Для нових версій (2.x+):
                 var renderedFilter = filter.Render(_reviews.DocumentSerializer, _reviews.Settings.SerializerRegistry);
                  _logger.LogInformation($"Final MongoDB filter: {renderedFilter}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not render filter for logging: {ex.Message}");
            }

            // --- Запит ---
            // Використовуємо _reviews (або collection з базового класу)
            var findFluent = _reviews.Find(filter);

            if (!string.IsNullOrWhiteSpace(queryParameters.OrderBy))
            {
                var sortHelper = new MongoSortHelper<Review>();
                findFluent = findFluent.Sort(sortHelper.ApplySort(queryParameters.OrderBy));
            }
            else
            {
                // Сортування за замовчуванням: найновіші спочатку
                findFluent = findFluent.SortByDescending(r => r.CreatedAt);
            }

            // --- Пагінація ---
            // PagedList.ToPagedListAsync зазвичай приймає IQueryable, але у випадку MongoDriver це IFindFluent.
            // Вам потрібно перевірити, як реалізований ваш PagedList<T>.
            // Якщо він працює з IFindFluent, то все ок.
            
            var result = await PagedList<Review>.ToPagedListAsync(
                findFluent,
                queryParameters.PageNumber,
                queryParameters.PageSize,
                cancellationToken
            );

            _logger.LogInformation($"Повертається {result.Items.Count} елементів зі сторінки {queryParameters.PageNumber}");

            return result;
        }

        public async Task DeleteByBouquetIdAsync(Guid bouquetId, CancellationToken cancellationToken)
        {
            var filter = Builders<Review>.Filter.Eq(r => r.BouquetId, bouquetId);
            await _reviews.DeleteManyAsync(filter, cancellationToken);
        }

        public async Task<bool> HasUserReviewedBouquetAsync(Guid userId, Guid bouquetId, CancellationToken cancellationToken = default)
        {
            var filterBuilder = Builders<Review>.Filter;
            var filter = filterBuilder.Eq(r => r.User.UserId, userId) &
                         filterBuilder.Eq(r => r.BouquetId, bouquetId);

            // CountDocumentsAsync - це правильний метод
            long count = await _reviews.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            return count > 0;
        }
    }
}