using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Helpers;
using ReviewService.Domain.Interfaces;

namespace ReviewService.Application.Features.Reviews.Query.GetReviews
{
    public class GetReviewsQueryHandler : IRequestHandler<GetReviewsQuery, PagedList<Review>>
    {
        private IReviewRepository reviewRepository;

        public GetReviewsQueryHandler(IReviewRepository reviewRepository)
        {
            this.reviewRepository = reviewRepository;
        }

        public async Task<PagedList<Review>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
        {
            return await reviewRepository.GetReviewsAsync(request.QueryParameters, cancellationToken);
        }
    }
}
