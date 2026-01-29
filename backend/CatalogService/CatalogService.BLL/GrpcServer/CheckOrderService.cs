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
        private shared.localization.ILanguageProvider languageProvider;

        public CheckOrderService(IUnitOfWork unitOfWork, ILogger<CheckOrderService> logger, shared.localization.ILanguageProvider languageProvider)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.languageProvider = languageProvider;
        }

        private void SetCurrentLanguage(ServerCallContext context)
        {
            var acceptLang = context.RequestHeaders.FirstOrDefault(h => h.Key == "accept-language")?.Value;
            if (!string.IsNullOrEmpty(acceptLang))
            {
                var lang = acceptLang.Split(',')[0].Split(';')[0].Split('-')[0].ToLower();
                if (lang == "uk" || lang == "ua" || lang == "ukr") lang = "ua";
                else if (lang == "en" || lang == "eng") lang = "en";
                else lang = null;

                if (lang != null) languageProvider.CurrentLanguage = lang;
            }
        }

        public override async Task<OrderedResponseList> CheckOrderItems(OrderedBouquetsIdList request, ServerCallContext context)
        {
            SetCurrentLanguage(context);
            if (request == null)
            {
                logger.LogWarning("Received empty request");
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Request cannot be empty"));
            }

            var responseList = new OrderedResponseList();
            var currentLang = languageProvider.CurrentLanguage ?? "ua";

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
                            ErrorMessage = "Invalid bouquet GUID format",
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
                            ErrorMessage = "Invalid size GUID format",
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
                            ErrorMessage = "Bouquet not found",
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
                            ErrorMessage = "The specified size is not available for this bouquet",
                            BouquetName = bouquet.Name.GetValueOrDefault(currentLang, bouquet.Name.GetValueOrDefault("ua", "")),
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
                            ErrorMessage = "The bouquet size has no configured flowers",
                            BouquetName = bouquet.Name.GetValueOrDefault(currentLang, bouquet.Name.GetValueOrDefault("ua", "")),
                            BouquetImage = bouquet.MainPhotoUrl,
                            Price = bouquetSize.Price.ToString("F2"),
                            SizeName = bouquetSize.Size?.Name.GetValueOrDefault(currentLang, bouquetSize.Size?.Name.GetValueOrDefault("ua", "")) ?? ""
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
                            stockError = "Invalid bouquet configuration";
                            break;
                        }

                        int requiredQuantity = bsf.Quantity * item.Count;
                        int availableQuantity = bsf.Flower.Quantity;

                        logger.LogInformation("Flower {FlowerName}: required {Required}, available {Available}",
                            bsf.Flower.Name, requiredQuantity, availableQuantity);

                        flowerInfoList.Add(new FlowerInfo
                        {
                            FlowerId = bsf.Flower.Id.ToString(),
                            FlowerName = bsf.Flower.Name.GetValueOrDefault(currentLang, bsf.Flower.Name.GetValueOrDefault("ua", "")),
                            Quantity = bsf.Quantity
                        });

                        if (availableQuantity < requiredQuantity)
                        {
                            enoughStock = false;
                            stockError = $"Not enough flowers '{bsf.Flower.Name.GetValueOrDefault(currentLang, bsf.Flower.Name.GetValueOrDefault("ua", ""))}' in stock. " +
                                       $"Required: {requiredQuantity}, available: {availableQuantity}";
                            logger.LogWarning("Not enough flowers: {Error}", stockError);
                            break;
                        }
                    }

                    var response = new OrderedRequest
                    {
                        IsValid = enoughStock,
                        ErrorMessage = enoughStock ? "" : stockError,
                        BouquetName = bouquet.Name.GetValueOrDefault(currentLang, bouquet.Name.GetValueOrDefault("ua", "")),
                        BouquetImage = bouquet.MainPhotoUrl,
                        Price = bouquetSize.Price.ToString("F2"),
                        SizeName = bouquetSize.Size?.Name.GetValueOrDefault(currentLang, bouquetSize.Size?.Name.GetValueOrDefault("ua", "")) ?? ""
                    };

                    response.Flowers.AddRange(flowerInfoList);
                    responseList.OrderedResponseList_.Add(response);
                }
                catch (OperationCanceledException)
                {
                    logger.LogWarning("Request was cancelled for bouquet {BouquetId}", item.Id);
                    throw new RpcException(new Status(StatusCode.Cancelled, "Request was cancelled"));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error while checking bouquet {BouquetId}", item.Id);
                    throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
                }
            }

            logger.LogInformation("Completed checking {Count} bouquets", request.OrderedBouquets.Count);
            return responseList;
        }
    }
}
