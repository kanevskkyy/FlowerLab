using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ReviewService.Application.Features.Reviews.Commands.DeleteReview;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Helpers;
using ReviewService.Domain.Interfaces;
using ReviewService.Domain.ValueObjects;
using Xunit;

namespace ReviewService.Tests.Services.Commands
{
    public class DeleteReviewCommandHandlerTests
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly DeleteReviewCommandHandler _sut;

        public DeleteReviewCommandHandlerTests()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _sut = new DeleteReviewCommandHandler(_reviewRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidOwner_DeletesReview()
        {
            var reviewId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();
            var review = new Review(Guid.NewGuid(), new UserInfo(userId, "TestUser", "LastName", "PhotoURL"), 5, "Comment");

            var command = new DeleteReviewCommand(reviewId) { RequesterId = userId, RequesterRole = "Client" };

            _reviewRepositoryMock
                .Setup(r => r.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            
            var result = await _sut.Handle(command, CancellationToken.None);

            
            Assert.Equal(MediatR.Unit.Value, result);
            _reviewRepositoryMock.Verify(r => r.DeleteAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidAdmin_DeletesReview()
        {
            var reviewId = Guid.NewGuid().ToString();
            var review = new Review(Guid.NewGuid(), new UserInfo(Guid.NewGuid(), "OtherUser", "LastName", "PhotoURL"), 5, "Comment");

            var command = new DeleteReviewCommand(reviewId) { RequesterId = Guid.NewGuid(), RequesterRole = "Admin" };

            _reviewRepositoryMock
                .Setup(r => r.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var result = await _sut.Handle(command, CancellationToken.None);

            Assert.Equal(MediatR.Unit.Value, result);
            _reviewRepositoryMock.Verify(r => r.DeleteAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReviewNotFound_ThrowsNotFoundException()
        {
            var command = new DeleteReviewCommand(Guid.NewGuid().ToString())
            {
                RequesterId = Guid.NewGuid(),
                RequesterRole = "Client"
            };

            _reviewRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Review)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_NotOwnerOrAdmin_ThrowsUnauthorizedAccessException()
        {
            var reviewId = Guid.NewGuid().ToString();
            var review = new Review(Guid.NewGuid(), new UserInfo(Guid.NewGuid(), "OtherUser", "OtheUserSurname", "PHOTOUTL"), 5, "Comment");

            var command = new DeleteReviewCommand(reviewId)
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
