using AggregatorService.Clients;
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
        public async Task<ActionResult<FilterResponse>> GetAllFilters(CancellationToken ct)
        {
            FilterResponse? filters = await filterClient.GetAllFiltersAsync();
            if (filters == null)
            {
                logger.LogWarning("Не вдалося отримати фільтри");
                return StatusCode(500, "Не вдалося отримати фільтри");
            }

            return Ok(filters);
        }
    }
}
