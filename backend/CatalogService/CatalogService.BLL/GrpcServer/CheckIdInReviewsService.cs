using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.Entities;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace CatalogService.BLL.GrpcServer
{
    public class CheckIdInReviewsService : CheckIdInReviews.CheckIdInReviewsBase
    {
        private IUnitOfWork _unitOfWork;
        private ILogger<CheckIdInReviewsService> _logger;

        public CheckIdInReviewsService(IUnitOfWork unitOfWork, ILogger<CheckIdInReviewsService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public override async Task<ReviewCheckIdResponse> CheckId(ReviewCheckIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Checking bouquet with ID: {Id}", request.Id);

            if (string.IsNullOrWhiteSpace(request.Id))
            {
                return new ReviewCheckIdResponse
                {
                    IsValid = false,
                    ErrorMessage = "ID cannot be empty"
                };
            }

            if (!Guid.TryParse(request.Id, out Guid boueuetId))
            {
                return new ReviewCheckIdResponse
                {
                    IsValid = false,
                    ErrorMessage = "ID is not a valid GUID"
                };
            }

            try
            {
                Bouquet? bouquet = await _unitOfWork.Bouquets.GetByIdAsync(boueuetId);

                return new ReviewCheckIdResponse
                {
                    IsValid = bouquet != null,
                    ErrorMessage = bouquet != null ? "" : "Bouquet with this ID does not exist"
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error checking bouquet with ID {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }
    }
}
