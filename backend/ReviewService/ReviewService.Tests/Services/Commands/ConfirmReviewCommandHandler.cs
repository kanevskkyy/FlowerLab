using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ReviewService.Application.Features.Reviews.Commands.UpdateReviewStatus;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Helpers;
using ReviewService.Domain.Interfaces;
using ReviewService.Domain.ValueObjects;
using Xunit;

namespace ReviewService.Tests.Services.Commands
{
    public class ConfirmReviewCommandHandlerTests
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly ConfirmReviewCommandHandler _sut;

        public ConfirmReviewCommandHandlerTests()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _sut = new ConfirmReviewCommandHandler(_reviewRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidReview_ConfirmsReview()
        {
            var reviewId = Guid.NewGuid().ToString();
            var review = new Review(Guid.NewGuid(), new UserInfo(Guid.NewGuid(), "First", "Last", "PhotoUrl"), 5, "Great!");

            _reviewRepositoryMock
                .Setup(r => r.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var command = new ConfirmReviewCommand(reviewId);

            var result = await _sut.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Status.ToString() == "Confirmed");
            _reviewRepositoryMock.Verify(r => r.UpdateAsync(review, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReviewNotFound_ThrowsNotFoundException()
        {
            var reviewId = Guid.NewGuid().ToString();
            _reviewRepositoryMock
                .Setup(r => r.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Review)null);

            var command = new ConfirmReviewCommand(reviewId);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.Handle(command, CancellationToken.None));
        }
    }
}
