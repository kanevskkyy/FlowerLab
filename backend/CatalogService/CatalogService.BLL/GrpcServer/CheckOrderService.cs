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
        private IUnitOfWork unitOfWork;
        private ILogger<CheckOrderService> logger;

        public CheckOrderService(IUnitOfWork unitOfWork, ILogger<CheckOrderService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public override async Task<OrderedResponseList> CheckOrderItems(OrderedBouquetsIdList request, ServerCallContext context)
        {
            if (request == null)
            {
                logger.LogWarning("Received empty request");
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Запит не може бути порожнім"));
            }

            var responseList = new OrderedResponseList();

            foreach (var item in request.OrderedBouquets)
            {
                try
                {
                    if (!Guid.TryParse(item.Id, out Guid bouquetId))
                    {
                        logger.LogWarning("Invalid GUID format for bouquet: {BouquetId}", item.Id);
                        responseList.OrderedResponseList_.Add(new OrderedRequest
                        {
                            IsValid = false,
                            ErrorMessage = "Недійсний формат GUID букета",
                            BouquetName = "",
                            BouquetImage = "",
                            Price = "0",
                            SizeName = ""
                        });
                        continue;
                    }

                    if (!Guid.TryParse(item.SizeId, out Guid sizeId))
                    {
                        logger.LogWarning("Invalid GUID format for size: {SizeId}", item.SizeId);
                        responseList.OrderedResponseList_.Add(new OrderedRequest
                        {
                            IsValid = false,
                            ErrorMessage = "Недійсний формат GUID розміру",
                            BouquetName = "",
                            BouquetImage = "",
                            Price = "0",
                            SizeName = ""
                        });
                        continue;
                    }

                    logger.LogInformation("Checking bouquet ID: {BouquetId}, size: {SizeId}, count: {Count}",
                        bouquetId, sizeId, item.Count);

                    var bouquet = await unitOfWork.Bouquets.GetWithDetailsAsync(bouquetId);

                    if (bouquet == null)
                    {
                        logger.LogWarning("Bouquet not found: {BouquetId}", bouquetId);
                        responseList.OrderedResponseList_.Add(new OrderedRequest
                        {
                            IsValid = false,
                            ErrorMessage = "Букет не знайдено",
                            BouquetName = "",
                            BouquetImage = "",
                            Price = "0",
                            SizeName = ""
                        });
                        continue;
                    }

                    var bouquetSize = bouquet.BouquetSizes.FirstOrDefault(bs => bs.SizeId == sizeId);

                    if (bouquetSize == null)
                    {
                        logger.LogWarning("Size {SizeId} not found for bouquet {BouquetId}", sizeId, bouquetId);
                        responseList.OrderedResponseList_.Add(new OrderedRequest
                        {
                            IsValid = false,
                            ErrorMessage = "Вказаний розмір недоступний для цього букета",
                            BouquetName = bouquet.Name,
                            BouquetImage = bouquet.MainPhotoUrl,
                            Price = "0",
                            SizeName = ""
                        });
                        continue;
                    }

                    if (bouquetSize.BouquetSizeFlowers == null || !bouquetSize.BouquetSizeFlowers.Any())
                    {
                        logger.LogWarning("Size {SizeId} of bouquet {BouquetId} has no configured flowers",
                            sizeId, bouquetId);
                        responseList.OrderedResponseList_.Add(new OrderedRequest
                        {
                            IsValid = false,
                            ErrorMessage = "Розмір букета не має налаштованих квітів",
                            BouquetName = bouquet.Name,
                            BouquetImage = bouquet.MainPhotoUrl,
                            Price = bouquetSize.Price.ToString("F2"),
                            SizeName = bouquetSize.Size?.Name ?? ""
                        });
                        continue;
                    }

                    bool enoughStock = true;
                    string stockError = "";
                    var flowerInfoList = new System.Collections.Generic.List<FlowerInfo>();

                    foreach (var bsf in bouquetSize.BouquetSizeFlowers)
                    {
                        if (bsf.Flower == null)
                        {
                            logger.LogWarning("BouquetSizeFlower has null reference to flower in bouquet {BouquetId}",
                                bouquetId);
                            enoughStock = false;
                            stockError = "Недійсна конфігурація букета";
                            break;
                        }

                        int requiredQuantity = bsf.Quantity * item.Count;
                        int availableQuantity = bsf.Flower.Quantity;

                        logger.LogInformation("Flower {FlowerName}: required {Required}, available {Available}",
                            bsf.Flower.Name, requiredQuantity, availableQuantity);

                        flowerInfoList.Add(new FlowerInfo
                        {
                            FlowerId = bsf.Flower.Id.ToString(),
                            FlowerName = bsf.Flower.Name,
                            FlowerColor = bsf.Flower.Color,
                            Quantity = bsf.Quantity
                        });

                        if (availableQuantity < requiredQuantity)
                        {
                            enoughStock = false;
                            stockError = $"Недостатньо квіток '{bsf.Flower.Name}' на складі. " +
                                       $"Потрібно: {requiredQuantity}, доступно: {availableQuantity}";
                            logger.LogWarning("Not enough flowers: {Error}", stockError);
                            break;
                        }
                    }

                    var response = new OrderedRequest
                    {
                        IsValid = enoughStock,
                        ErrorMessage = enoughStock ? "" : stockError,
                        BouquetName = bouquet.Name,
                        BouquetImage = bouquet.MainPhotoUrl,
                        Price = bouquetSize.Price.ToString("F2"),
                        SizeName = bouquetSize.Size?.Name ?? ""
                    };

                    response.Flowers.AddRange(flowerInfoList);
                    responseList.OrderedResponseList_.Add(response);
                }
                catch (OperationCanceledException)
                {
                    logger.LogWarning("Request was cancelled for bouquet {BouquetId}", item.Id);
                    throw new RpcException(new Status(StatusCode.Cancelled, "Запит був скасований"));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error while checking bouquet {BouquetId}", item.Id);
                    throw new RpcException(new Status(StatusCode.Internal, "Внутрішня помилка сервера"));
                }
            }

            logger.LogInformation("Completed checking {Count} bouquets", request.OrderedBouquets.Count);
            return responseList;
        }
    }
}