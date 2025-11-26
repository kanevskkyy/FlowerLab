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
                _logger.LogWarning("Отримано порожній запит");
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Запит не може бути порожнім"));
            }

            var responseList = new OrderedResponseList();

            foreach (var item in request.OrderedBouquets)
            {
                try
                {
                    if (!Guid.TryParse(item.Id, out Guid bouquetId))
                    {
                        _logger.LogWarning("Недійсний формат GUID для букета: {BouquetId}", item.Id);
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
                        _logger.LogWarning("Недійсний формат GUID для розміру: {SizeId}", item.SizeId);
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

                    _logger.LogInformation("Перевірка букета ID: {BouquetId}, розмір: {SizeId}, кількість: {Count}",
                        bouquetId, sizeId, item.Count);

                    var bouquet = await _unitOfWork.Bouquets.GetWithDetailsAsync(bouquetId);

                    if (bouquet == null)
                    {
                        _logger.LogWarning("Букет не знайдено: {BouquetId}", bouquetId);
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
                        _logger.LogWarning("Розмір {SizeId} не знайдено для букета {BouquetId}", sizeId, bouquetId);
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
                        _logger.LogWarning("Розмір {SizeId} букета {BouquetId} не має налаштованих квітів",
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
                            _logger.LogWarning("У BouquetSizeFlower порожнє посилання на квітку в букеті {BouquetId}",
                                bouquetId);
                            enoughStock = false;
                            stockError = "Недійсна конфігурація букета";
                            break;
                        }

                        int requiredQuantity = bsf.Quantity * item.Count;
                        int availableQuantity = bsf.Flower.Quantity;

                        _logger.LogInformation("Квітка {FlowerName}: потрібно {Required}, доступно {Available}",
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
                            _logger.LogWarning("Недостатньо квіток: {Error}", stockError);
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
                    _logger.LogWarning("Запит був скасований для букета {BouquetId}", item.Id);
                    throw new RpcException(new Status(StatusCode.Cancelled, "Запит був скасований"));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Несподівана помилка при перевірці букета {BouquetId}", item.Id);
                    throw new RpcException(new Status(StatusCode.Internal, "Внутрішня помилка сервера"));
                }
            }

            _logger.LogInformation("Завершено перевірку {Count} букетів", request.OrderedBouquets.Count);
            return responseList;
        }
    }
}