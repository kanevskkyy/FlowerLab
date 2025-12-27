using CatalogService.BLL.DTO;
using CatalogService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.API.Controllers
{
    [ApiController]
    [Route("api/recipients")]
    public class RecipientsController : ControllerBase
    {
        private IRecipientService recipientService;

        public RecipientsController(IRecipientService recipientService)
        {
            this.recipientService = recipientService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
            Ok(await recipientService.GetAllAsync(cancellationToken));

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken) =>
            Ok(await recipientService.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] RecipientCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await recipientService.CreateAsync(dto.Name, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RecipientUpdateDto dto, CancellationToken cancellationToken)
        {
            var updated = await recipientService.UpdateAsync(id, dto.Name, cancellationToken);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await recipientService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
