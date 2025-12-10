using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Moq;
using ReviewService.Application.Features.Reviews.Commands.CreateReview;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Helpers;
using ReviewService.Domain.Interfaces;
using ReviewService.Domain.ValueObjects;
using Xunit;

namespace ReviewService.Tests.Services.Commands
{
    public class CreateReviewCommandHandlerTests
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<CheckIdInReviews.CheckIdInReviewsClient> _grpcClientMock;
        private readonly CreateReviewCommandHandler _sut;
        private readonly Mock<CheckOrderServiceGRPC.CheckOrderServiceGRPCClient> _checkOrderGrpcClientMock;


        public CreateReviewCommandHandlerTests()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _grpcClientMock = new Mock<CheckIdInReviews.CheckIdInReviewsClient>();
            _checkOrderGrpcClientMock = new Mock<CheckOrderServiceGRPC.CheckOrderServiceGRPCClient>();

            _sut = new CreateReviewCommandHandler(
                _reviewRepositoryMock.Object,
                _grpcClientMock.Object,
                _checkOrderGrpcClientMock.Object
            );
        }


        [Fact]
        public async Task Handle_ValidRequest_CreatesReview()
        {

            var user = new UserInfo(Guid.NewGuid(), "TestUser", "LastName", "PhotoURL");
            var command = new CreateReviewCommand(Guid.NewGuid(), 5, "Nice bouquet") { User = user };

            _grpcClientMock
                .Setup(c => c.CheckIdAsync(It.IsAny<ReviewCheckIdRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(new AsyncUnaryCall<ReviewCheckIdResponse>(
                    Task.FromResult(new ReviewCheckIdResponse { IsValid = true }),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { }
                ));

            _reviewRepositoryMock
                .Setup(r => r.HasUserReviewedBouquetAsync(user.UserId, command.BouquetId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _sut.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(command.Comment, result.Comment);
            _reviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidBouquetId_ThrowsInvalidOperationException()
        {
            var command = new CreateReviewCommand(Guid.NewGuid(), 5, "Test") { User = new UserInfo(Guid.NewGuid(), "TestUser", "LastName", "PhotoURL") };

            _grpcClientMock
                .Setup(g => g.CheckIdAsync(It.IsAny<ReviewCheckIdRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(new AsyncUnaryCall<ReviewCheckIdResponse>(
                    Task.FromResult(new ReviewCheckIdResponse { IsValid = false, ErrorMessage = "Invalid" }),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { }
                ));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UserAlreadyReviewed_ThrowsAlreadyExistsException()
        {
            var user = new UserInfo(Guid.NewGuid(), "TestUser", "LastName", "PhotoURL");
            var command = new CreateReviewCommand(Guid.NewGuid(), 5, "Test") { User = user };

            _grpcClientMock
                 .Setup(g => g.CheckIdAsync(It.IsAny<ReviewCheckIdRequest>(), null, null, It.IsAny<CancellationToken>()))
                 .Returns(new AsyncUnaryCall<ReviewCheckIdResponse>(
                     Task.FromResult(new ReviewCheckIdResponse { IsValid = true }),
                     Task.FromResult(new Metadata()),
                     () => Status.DefaultSuccess,
                     () => new Metadata(),
                     () => { }
                 ));


            _reviewRepositoryMock
                .Setup(r => r.HasUserReviewedBouquetAsync(user.UserId, command.BouquetId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AlreadyExistsException>(() => _sut.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UserIsNull_ThrowsUnauthorizedAccessException()
        {
            var command = new CreateReviewCommand(Guid.NewGuid(), 5, "Test") { User = null };
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.Handle(command, CancellationToken.None));
        }
    }

}
