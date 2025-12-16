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
        private IUnitOfWork unitOfWork;
        private ILogger<CheckIdInReviewsService> logger;

        public CheckIdInReviewsService(IUnitOfWork unitOfWork, ILogger<CheckIdInReviewsService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public override async Task<ReviewCheckIdResponse> CheckId(ReviewCheckIdRequest request, ServerCallContext context)
        {
            logger.LogInformation("Checking bouquet with ID: {Id}", request.Id);

            if (string.IsNullOrWhiteSpace(request.Id))
            {
                return new ReviewCheckIdResponse
                {
                    IsValid = false,
                    ErrorMessage = "ID не може бути порожнім"
                };
            }

            if (!Guid.TryParse(request.Id, out Guid bouquetId))
            {
                return new ReviewCheckIdResponse
                {
                    IsValid = false,
                    ErrorMessage = "ID не є дійсним GUID"
                };
            }

            try
            {
                Bouquet? bouquet = await unitOfWork.Bouquets.GetByIdAsync(bouquetId);

                return new ReviewCheckIdResponse
                {
                    IsValid = bouquet != null,
                    ErrorMessage = bouquet != null ? "" : "Букет з цим ID не існує"
                };
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, "Error occurred while checking bouquet with ID {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.Internal, "Внутрішня помилка сервера"));
            }
        }
    }
}