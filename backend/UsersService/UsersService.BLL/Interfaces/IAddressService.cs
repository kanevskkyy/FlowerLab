using UsersService.BLL.Models;
using System.Collections.Generic;

namespace UsersService.BLL.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAddressesByUserIdAsync(string userId);
        Task<AddressDto?> GetAddressByIdAsync(string userId, int addressId);
        Task<AddressDto> CreateAddressAsync(string userId, AddressDto model);
        Task<bool> UpdateAddressAsync(string userId, int addressId, AddressDto model);
        Task<bool> DeleteAddressAsync(string userId, int addressId);
    }
}