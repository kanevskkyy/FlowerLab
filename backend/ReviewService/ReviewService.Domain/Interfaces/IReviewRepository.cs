using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewService.Domain.Entities.QueryParameters;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Helpers;

namespace ReviewService.Domain.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task DeleteByBouquetIdAsync(Guid bouquetId, CancellationToken cancellationToken);
        Task<bool> HasUserReviewedBouquetAsync(Guid userId, Guid bouquetId, CancellationToken cancellationToken = default);
        Task<PagedList<Review>> GetReviewsAsync(ReviewQueryParameters queryParameters, CancellationToken cancellationToken = default);
    }
}
