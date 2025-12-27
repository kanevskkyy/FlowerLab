using CatalogService.BLL.DTO;
using CatalogService.BLL.Services.Interfaces;
using CatalogService.Domain.QueryParametrs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.API.Controllers
{
    [ApiController]
    [Route("api/bouquets")]
    public class BouquetsController : ControllerBase
    {
        private IBouquetService bouquetService;

        public BouquetsController(IBouquetService bouquetService)
        {
            this.bouquetService = bouquetService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] BouquetQueryParameters query, CancellationToken cancellationToken)
        {
            var result = await bouquetService.GetAllAsync(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var bouquet = await bouquetService.GetByIdAsync(id, cancellationToken);
            return Ok(bouquet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] BouquetCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await bouquetService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromForm] BouquetUpdateDto dto, CancellationToken cancellationToken)
        {
            var updated = await bouquetService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await bouquetService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
