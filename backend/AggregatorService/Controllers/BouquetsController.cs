using AggregatorService.Clients.Interfaces;
using AggregatorService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AggregatorService.Controllers
{
    [ApiController]
    [Route("api/bouquet")]
    public class BouquetsController : ControllerBase
    {
        private readonly IBouquetWithReviewsService _bouquetService;

        public BouquetsController(IBouquetWithReviewsService bouquetService)
        {
            _bouquetService = bouquetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBouquets([FromQuery] BouquetFilterDTO filter)
        {
            try
            {
                var request = new GetBouquetsRequest
                {
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    SortBy = filter.SortBy ?? "",
                    Name = filter.Name ?? "",
                    MinPrice = filter.MinPrice ?? 0,
                    MaxPrice = filter.MaxPrice ?? 0
                };

                if (filter.FlowerIds != null) request.FlowerIds.AddRange(filter.FlowerIds);
                if (filter.EventIds != null) request.EventIds.AddRange(filter.EventIds);
                if (filter.RecipientIds != null) request.RecipientIds.AddRange(filter.RecipientIds);
                if (filter.SizeIds != null) request.SizeIds.AddRange(filter.SizeIds);
                if (filter.Quantities != null) request.Quantities.AddRange(filter.Quantities);

                var result = await _bouquetService.GetBouquetsAsync(request);
                
                // Map Proto to DTO
                var responseDto = new BouquetListDto
                {
                    TotalCount = result.TotalCount,
                    TotalPages = result.TotalPages,
                    CurrentPage = result.CurrentPage,
                    Items = result.Items.Select(b => new BouquetSimpleDto
                    {
                        Id = Guid.TryParse(b.Id, out var gId) ? gId : Guid.Empty,
                        Name = b.Name,
                        Price = b.Price,
                        MainPhotoUrl = b.MainPhotoUrl,
                        Sizes = b.Sizes.Select(s => new BouquetSizeDto
                        {
                            SizeId = Guid.TryParse(s.SizeId, out var sId) ? sId : Guid.Empty,
                            SizeName = s.SizeName,
                            Price = s.Price,
                            MaxAssemblableCount = s.MaxAssemblableCount,
                            IsAvailable = s.IsAvailable,
                            Flowers = s.Flowers.Select(f => new FlowerInBouquetDto
                            {
                                Id = Guid.TryParse(f.Id, out var fId) ? fId : Guid.Empty,
                                Name = f.Name,
                                Quantity = f.Quantity
                            }).ToList()
                        }).ToList()
                    }).ToList()
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                // Simple logging if ILogger is not injected (it wasn't in original file). 
                // Better to return 500 with message.
                Console.WriteLine($"[Error] GetBouquets failed: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching bouquets via Aggregator", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BouquetInfoWithReviewsDTO>> GetBouquetWithReviews(Guid id)
        {
            try
            {
                var bouquetInfo = await _bouquetService.GetAggegatedBouquetInfo(id);

                if (bouquetInfo == null)
                    return NotFound(new { message = "Букет не знайдено" });

                return Ok(bouquetInfo);
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                return NotFound(new { message = ex.Status.Detail });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера", detail = ex.Message });
            }
        }
    }
}
