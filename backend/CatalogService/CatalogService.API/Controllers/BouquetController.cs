using CatalogService.BLL.DTO;
using CatalogService.BLL.Services.Interfaces;
using CatalogService.Domain.QueryParametrs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace CatalogService.API.Controllers
{
    [ApiController]
    [Route("api/bouquets")]
    public class BouquetsController : ControllerBase
    {
        private readonly IBouquetService _bouquetService;

        public BouquetsController(IBouquetService bouquetService)
        {
            _bouquetService = bouquetService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] BouquetQueryParameters query)
        {
            var result = await _bouquetService.GetAllAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var bouquet = await _bouquetService.GetByIdAsync(id);
            return Ok(bouquet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] BouquetCreateDto dto)
        {
            
            var created = await _bouquetService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromForm] BouquetUpdateDto dto)
        {
            var updated = await _bouquetService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _bouquetService.DeleteAsync(id);
            return NoContent();
        }
    }
}
