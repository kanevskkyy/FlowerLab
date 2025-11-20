using AggregatorService.Clients;
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
