using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ReviewService.Application.Features.Reviews.Query.GerReviewById;
using ReviewService.Application.Features.Reviews.Query.GetReviews;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Helpers;
using ReviewService.Domain.Interfaces;
using Xunit;
using ReviewService.Domain.Entities.QueryParameters;
using MongoDB.Bson;
using ReviewService.Domain.ValueObjects;
using ReviewService.Application.Features.Reviews.Query.GetReviewById;

namespace ReviewService.Tests.Services.Query
{
    public class ReviewQueryHandlerTests
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;

        public ReviewQueryHandlerTests()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
        }

        #region GetReviewByIdQueryHandler Tests


        [Fact]
        public async Task GetReviewByIdQueryHandler_ReviewDoesNotExist_ThrowsNotFoundException()
        {
            var reviewId = Guid.NewGuid();
            _reviewRepositoryMock.Setup(r => r.GetByIdAsync(reviewId.ToString(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync((Review?)null);

            var handler = new GetReviewByIdQueryHandler(_reviewRepositoryMock.Object);
            var query = new GetReviewByIdQuery(reviewId.ToString());

            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query, CancellationToken.None));
        }

        #endregion

        #region GetReviewsQueryHandler Tests

        [Fact]
        public async Task GetReviewsQueryHandler_ReturnsPagedReviews()
        {
            var reviews = new List<Review>
            {
                new Review(Guid.NewGuid(), new Domain.ValueObjects.UserInfo(Guid.NewGuid(), "First1", "Last1", "Photo1"), 5, "Nice"),
                new Review(Guid.NewGuid(), new Domain.ValueObjects.UserInfo(Guid.NewGuid(), "First2", "Last2", "Photo2"), 4, "Good")
            };
            var pagedList = new PagedList<Review>(reviews, reviews.Count, 1, 10);

            _reviewRepositoryMock.Setup(r => r.GetReviewsAsync(It.IsAny<ReviewQueryParameters>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(pagedList);

            var handler = new GetReviewsQueryHandler(_reviewRepositoryMock.Object);
            var query = new GetReviewsQuery(new ());

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count());
        }

        #endregion
    }
}
