using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UsersService.BLL.Interfaces;
using UsersService.BLL.Models;
using UsersService.DAL.DbContext;
using UsersService.Domain.Entities;

namespace UsersService.BLL.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public AddressService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddressDto>> GetAddressesByUserIdAsync(string userId)
        {
            var addresses = await _dbContext.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<AddressDto>>(addresses);
        }

        // --- РЕАЛІЗАЦІЯ ВІДСУТНЬОГО МЕТОДУ (Read by ID) ---
        public async Task<AddressDto?> GetAddressByIdAsync(string userId, int addressId)
        {
            var address = await _dbContext.Addresses
                .Where(a => a.UserId == userId && a.Id == addressId)
                .FirstOrDefaultAsync();

            return _mapper.Map<AddressDto?>(address);
        }
        
        public async Task<AddressDto> CreateAddressAsync(string userId, AddressDto model)
        {
            var entity = _mapper.Map<UserAddress>(model);
            entity.UserId = userId;
            
            await _dbContext.Addresses.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            
            return _mapper.Map<AddressDto>(entity);
        }
        
        // --- РЕАЛІЗАЦІЯ ВІДСУТНЬОГО МЕТОДУ (Update) ---
        public async Task<bool> UpdateAddressAsync(string userId, int addressId, AddressDto model)
        {
            var entityToUpdate = await _dbContext.Addresses
                .Where(a => a.UserId == userId && a.Id == addressId)
                .FirstOrDefaultAsync();

            if (entityToUpdate == null)
            {
                return false; // Адреса не знайдена або не належить користувачеві
            }
            
            // Оновлення полів
            entityToUpdate.FullName = model.FullName;
            entityToUpdate.StreetAddress = model.StreetAddress;
            entityToUpdate.City = model.City;
            entityToUpdate.PostalCode = model.PostalCode;
            entityToUpdate.Country = model.Country;
            entityToUpdate.IsDefault = model.IsDefault;
            
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // --- РЕАЛІЗАЦІЯ ВІДСУТНЬОГО МЕТОДУ (Delete) ---
        public async Task<bool> DeleteAddressAsync(string userId, int addressId)
        {
            var entityToDelete = await _dbContext.Addresses
                .Where(a => a.UserId == userId && a.Id == addressId)
                .FirstOrDefaultAsync();

            if (entityToDelete == null)
            {
                return false; // Адреса не знайдена або не належить користувачеві
            }

            _dbContext.Addresses.Remove(entityToDelete);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}