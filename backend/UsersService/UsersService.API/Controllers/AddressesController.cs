using k8s.KubeConfigModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsersService.BLL.Models.Adresess;
using UsersService.BLL.Services.Interfaces;

namespace UsersService.API.Controllers
{
    [Authorize]
    [Route("api/me/addresses")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private IAddressService addressService;

        public AddressesController(IAddressService addressService)
        {
            this.addressService = addressService;
        }

        private string UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await addressService.GetUserAddressesAsync(UserId));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAddressDto dto)
        {
            var address = await addressService.CreateAsync(UserId, dto);
            return Ok(address);
        }

        [HttpPut("{addressId}")]
        public async Task<IActionResult> Update(Guid addressId, [FromBody] CreateAddressDto dto)
        {
            await addressService.UpdateAsync(UserId, addressId, dto);
            return NoContent();
        }

        [HttpDelete("{addressId}")]
        public async Task<IActionResult> Delete(Guid addressId)
        {
            await addressService.DeleteAsync(UserId, addressId);
            return NoContent();
        }
    }
}
