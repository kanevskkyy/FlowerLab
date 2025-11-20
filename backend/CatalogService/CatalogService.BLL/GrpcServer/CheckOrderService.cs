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
                            ErrorMessage = "Недійсний формат GUID",
                            BouquetName = "",
                            BouquetImage = "",
                            Price = "0"
                        });
                        continue;
                    }

                    _logger.LogInformation("Перевірка букета ID: {BouquetId} з кількістю {Count}", bouquetId, item.Count);

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
                            Price = "0"
                        });
                        continue;
                    }

                    if (bouquet.BouquetFlowers == null || !bouquet.BouquetFlowers.Any())
                    {
                        _logger.LogWarning("Букет {BouquetId} не має налаштованих квітів", bouquetId);
                        responseList.OrderedResponseList_.Add(new OrderedRequest
                        {
                            IsValid = false,
                            ErrorMessage = "Букет не має налаштованих квітів",
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
                            _logger.LogWarning("У BouquetFlower порожня посилання на квітку в букеті {BouquetId}", bouquetId);
                            enoughStock = false;
                            stockError = "Недійсна конфігурація букета";
                            break;
                        }

                        int requiredQuantity = bf.Quantity * item.Count;
                        int availableQuantity = bf.Flower.Quantity;

                        _logger.LogInformation("Квітка {FlowerName}: потрібно {Required}, доступно {Available}",
                            bf.Flower.Name, requiredQuantity, availableQuantity);

                        if (availableQuantity < requiredQuantity)
                        {
                            enoughStock = false;
                            stockError = $"Недостатньо квіток '{bf.Flower.Name}' на складі. Потрібно: {requiredQuantity}, доступно: {availableQuantity}";
                            _logger.LogWarning("Недостатньо квіток: {Error}", stockError);
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