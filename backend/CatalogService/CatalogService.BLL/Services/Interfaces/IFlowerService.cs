using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.BLL.DTO;


namespace CatalogService.BLL.Services.Interfaces
{
    public interface IFlowerService
    {
        Task<IEnumerable<FlowerDto>> GetAllAsync();
        Task<FlowerDto> GetByIdAsync(Guid id);
        Task<FlowerDto> CreateAsync(FlowerCreateUpdateDto dto);
        Task<FlowerDto> UpdateAsync(Guid id, FlowerCreateUpdateDto dto);
        Task DeleteAsync(Guid id);
        Task<FlowerDto> UpdateStockAsync(Guid id, int quantity);
    }
}
