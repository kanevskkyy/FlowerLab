using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ReviewService.Domain.Entities;

namespace ReviewService.Application.Features.Reviews.Query.GerReviewById
{
    public class GetReviewByIdQuery : IRequest<Review?>
    {
        public string ReviewId { get; }

        public GetReviewByIdQuery(string reviewId)
        {
            ReviewId = reviewId;
        }
    }
}
