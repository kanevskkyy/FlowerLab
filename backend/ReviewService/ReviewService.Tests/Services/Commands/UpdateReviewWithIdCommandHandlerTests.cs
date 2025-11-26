using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ReviewService.Application.Features.Reviews.Commands.UpdateReview;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Helpers;
using ReviewService.Domain.Interfaces;
using ReviewService.Domain.ValueObjects;
using Xunit;

namespace ReviewService.Tests.Services.Commands
{
    public class UpdateReviewWithIdCommandHandlerTests
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly UpdateReviewWithIdCommandHandler _sut;

        public UpdateReviewWithIdCommandHandlerTests()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _sut = new UpdateReviewWithIdCommandHandler(_reviewRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidOwner_UpdatesReview()
        {
            var reviewId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();
            var review = new Review(Guid.NewGuid(), new UserInfo(userId, "First", "Last", "PhotoUrl"), 3, "Old comment");

            var command = new UpdateReviewWithIdCommand(reviewId, "New comment", 5)
            {
                RequesterId = userId,
                RequesterRole = "Client"
            };

            _reviewRepositoryMock
                .Setup(r => r.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var result = await _sut.Handle(command, CancellationToken.None);

            Assert.Equal(MediatR.Unit.Value, result);
            Assert.Equal("New comment", review.Comment);
            Assert.Equal(5, review.Rating);
            _reviewRepositoryMock.Verify(r => r.UpdateAsync(review, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReviewNotFound_ThrowsNotFoundException()
        {
            var command = new UpdateReviewWithIdCommand(Guid.NewGuid().ToString(), "Comment", 5)
            {
                RequesterId = Guid.NewGuid()
            };

            _reviewRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Review)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_NotOwner_ThrowsUnauthorizedAccessException()
        {
            var reviewId = Guid.NewGuid().ToString();
            var review = new Review(Guid.NewGuid(), new UserInfo(Guid.NewGuid(), "First", "Last", "PhotoUrl"), 3, "Old comment");

            var command = new UpdateReviewWithIdCommand(reviewId, "New comment", 5)
            {
                RequesterId = Guid.NewGuid(),
                RequesterRole = "Client"
            };

            _reviewRepositoryMock
                .Setup(r => r.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.Handle(command, CancellationToken.None));
        }
    }
}
