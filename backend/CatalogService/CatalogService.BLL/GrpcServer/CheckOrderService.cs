using System;
using System.Linq;
using System.Threading.Tasks;
using CatalogService.DAL.UnitOfWork;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace CatalogService.BLL.GrpcServer
{
    public class CheckOrderService : CheckOrder.CheckOrderBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CheckOrderService> _logger;

        public CheckOrderService(IUnitOfWork unitOfWork, ILogger<CheckOrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public override async Task<OrderedResponseList> CheckOrderItems(OrderedBouquetsIdList request, ServerCallContext context)
        {
            if (request == null)
            {
                _logger.LogWarning("Received null request");
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Request cannot be null"));
            }

            var responseList = new OrderedResponseList();

            foreach (var item in request.OrderedBouquets)
            {
                try
                {
                    if (!Guid.TryParse(item.Id, out Guid bouquetId))
                    {
                        _logger.LogWarning("Invalid GUID format for bouquet: {BouquetId}", item.Id);
                        responseList.OrderedResponseList_.Add(new OrderedRequest
                        {
                            IsValid = false,
                            ErrorMessage = "Invalid GUID format",
                            BouquetName = "",
                            BouquetImage = "",
                            Price = "0"
                        });
                        continue;
                    }

                    _logger.LogInformation("Checking bouquet ID: {BouquetId} with count {Count}", bouquetId, item.Count);

                    var bouquet = await _unitOfWork.Bouquets.GetWithDetailsAsync(bouquetId);

                    if (bouquet == null)
                    {
                        _logger.LogWarning("Bouquet not found: {BouquetId}", bouquetId);
                        responseList.OrderedResponseList_.Add(new OrderedRequest
                        {
                            IsValid = false,
                            ErrorMessage = "Bouquet not found",
                            BouquetName = "",
                            BouquetImage = "",
                            Price = "0"
                        });
                        continue;
                    }

                    if (bouquet.BouquetFlowers == null || !bouquet.BouquetFlowers.Any())
                    {
                        _logger.LogWarning("Bouquet {BouquetId} has no flowers configured", bouquetId);
                        responseList.OrderedResponseList_.Add(new OrderedRequest
                        {
                            IsValid = false,
                            ErrorMessage = "Bouquet has no flowers configured",
                            BouquetName = bouquet.Name,
                            BouquetImage = bouquet.MainPhotoUrl,
                            Price = bouquet.Price.ToString("F2")
                        });
                        continue;
                    }

                    bool enoughStock = true;
                    string stockError = "";

                    foreach (var bf in bouquet.BouquetFlowers)
                    {
                        if (bf.Flower == null)
                        {
                            _logger.LogWarning("BouquetFlower has null Flower reference in bouquet {BouquetId}", bouquetId);
                            enoughStock = false;
                            stockError = "Invalid bouquet configuration";
                            break;
                        }

                        int requiredQuantity = bf.Quantity * item.Count;
                        int availableQuantity = bf.Flower.Quantity;

                        _logger.LogInformation("Flower {FlowerName}: required {Required}, available {Available}",
                            bf.Flower.Name, requiredQuantity, availableQuantity);

                        if (availableQuantity < requiredQuantity)
                        {
                            enoughStock = false;
                            stockError = $"Not enough stock for flower '{bf.Flower.Name}'. Required: {requiredQuantity}, available: {availableQuantity}";
                            _logger.LogWarning("Not enough stock: {Error}", stockError);
                            break;
                        }
                    }

                    responseList.OrderedResponseList_.Add(new OrderedRequest
                    {
                        IsValid = enoughStock,
                        ErrorMessage = enoughStock ? "" : stockError,
                        BouquetName = bouquet.Name,
                        BouquetImage = bouquet.MainPhotoUrl,
                        Price = bouquet.Price.ToString("F2")
                    });
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Request was cancelled for bouquet {BouquetId}", item.Id);
                    throw new RpcException(new Status(StatusCode.Cancelled, "Request was cancelled"));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while checking bouquet {BouquetId}", item.Id);
                    throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
                }
            }

            _logger.LogInformation("Finished checking {Count} bouquets", request.OrderedBouquets.Count);
            return responseList;
        }
    }
}