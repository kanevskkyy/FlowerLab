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
            _logger.LogInformation("Перевірка букета з ID: {Id}", request.Id);

            if (string.IsNullOrWhiteSpace(request.Id))
            {
                return new ReviewCheckIdResponse
                {
                    IsValid = false,
                    ErrorMessage = "ID не може бути порожнім"
                };
            }

            if (!Guid.TryParse(request.Id, out Guid boueuetId))
            {
                return new ReviewCheckIdResponse
                {
                    IsValid = false,
                    ErrorMessage = "ID не є дійсним GUID"
                };
            }

            try
            {
                Bouquet? bouquet = await _unitOfWork.Bouquets.GetByIdAsync(boueuetId);

                return new ReviewCheckIdResponse
                {
                    IsValid = bouquet != null,
                    ErrorMessage = bouquet != null ? "" : "Букет з цим ID не існує"
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Помилка при перевірці букета з ID {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.Internal, "Внутрішня помилка сервера"));
            }
        }
    }
}
