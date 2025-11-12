using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.BLL.DTOs.GiftsDTOs;

namespace OrderService.BLL.Services.Interfaces
{
    public interface IGiftService
    {
        Task<IEnumerable<GiftReadDto>> GetAllAsync();
        Task<GiftReadDto> GetByIdAsync(Guid id);
        Task<GiftReadDto> CreateAsync(GiftCreateDto dto);
        Task<GiftReadDto> UpdateAsync(Guid id, GiftUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}
