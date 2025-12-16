using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.Models.Adresess;

namespace UsersService.BLL.Services.Interfaces
{
    public interface IAddressService
    {
        Task<List<AddressDto>> GetUserAddressesAsync(string userId);
        Task<AddressDto> CreateAsync(string userId, CreateAddressDto dto);
        Task UpdateAsync(Guid addressId, CreateAddressDto dto);
        Task DeleteAsync(Guid addressId);
    }
}
