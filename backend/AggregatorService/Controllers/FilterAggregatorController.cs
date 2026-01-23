using AggregatorService.Clients.Interfaces;
using AggregatorService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AggregatorService.Controllers
{
    [ApiController]
    [Route("api/filters")]
    public class FilterAggregatorController : ControllerBase
    {
        private IFilterGrpcClient filterClient;
        private ILogger<FilterAggregatorController> logger;

        public FilterAggregatorController(IFilterGrpcClient filterClient, ILogger<FilterAggregatorController> logger)
        {
            this.filterClient = filterClient;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<FilterResponseDto>> GetAllFilters(CancellationToken ct)
        {
            try
            {
                // Client now returns DTO (cached or fetched & mapped)
                var filters = await filterClient.GetAllFiltersAsync();
                
                if (filters == null)
                {
                    logger.LogWarning("Не вдалося отримати фільтри");
                    return StatusCode(500, "Не вдалося отримати фільтри");
                }

                return Ok(filters);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching filters");
                return StatusCode(500, new { message = "Error fetching filters", details = ex.Message });
            }
        }
    }
}
