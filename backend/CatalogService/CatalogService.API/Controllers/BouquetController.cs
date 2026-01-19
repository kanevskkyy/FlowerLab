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
            if (dto.Sizes != null && Request.Form.Files.Count > 0)
            {
                var regex = new System.Text.RegularExpressions.Regex(@"(?i)Sizes\[(\d+)\]\.AdditionalImages");
                
                foreach (var file in Request.Form.Files)
                {
                    var match = regex.Match(file.Name);
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int index))
                    {
                        if (index >= 0 && index < dto.Sizes.Count)
                        {
                            if (!dto.Sizes[index].AdditionalImages.Contains(file))
                            {
                                dto.Sizes[index].AdditionalImages.Add(file);
                            }
                        }
                    }
                }
            }
            
            var created = await bouquetService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromForm] BouquetUpdateDto dto, CancellationToken cancellationToken)
        {
            // Manual binding for Files (existing logic)
            if (dto.Sizes != null && Request.Form.Files.Count > 0)
            {
                var regex = new System.Text.RegularExpressions.Regex(@"Sizes\[(\d+)\]\.AdditionalImages");
                
                foreach (var file in Request.Form.Files)
                {
                    var match = regex.Match(file.Name);
                    if (match.Success)
                    {
                        if (int.TryParse(match.Groups[1].Value, out int index))
                        {
                            if (index >= 0 && index < dto.Sizes.Count)
                            {
                                dto.Sizes[index].AdditionalImages.Add(file);
                            }
                        }
                    }
                }
            }

            // Manual binding for ImageIdsToDelete (New logic)
            if (dto.Sizes != null && Request.Form.Keys.Count > 0)
            {
                var regex = new System.Text.RegularExpressions.Regex(@"Sizes\[(\d+)\]\.ImageIdsToDelete\[(\d+)\]");
                foreach (var key in Request.Form.Keys)
                {
                    var match = regex.Match(key);
                    if (match.Success)
                    {
                        if (int.TryParse(match.Groups[1].Value, out int sizeIndex))
                        {
                            if (sizeIndex >= 0 && sizeIndex < dto.Sizes.Count)
                            {
                                var val = Request.Form[key];
                                if (Guid.TryParse(val, out Guid guidVal))
                                {
                                     // Ensure we don't add duplicates if binder worked partially
                                     if (!dto.Sizes[sizeIndex].ImageIdsToDelete.Contains(guidVal))
                                     {
                                         dto.Sizes[sizeIndex].ImageIdsToDelete.Add(guidVal);
                                     }
                                }
                            }
                        }
                    }
                }
            }

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
