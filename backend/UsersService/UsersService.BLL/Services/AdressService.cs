using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.Models.Adresess;
using UsersService.BLL.Services.Interfaces;
using UsersService.DAL.DbContext;
using UsersService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace UsersService.BLL.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AddressService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<AddressDto>> GetUserAddressesAsync(string userId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            return _mapper.Map<List<AddressDto>>(addresses);
        }

        public async Task<AddressDto> CreateAsync(string userId, CreateAddressDto dto)
        {
            var address = _mapper.Map<Address>(dto);
            address.UserId = userId;

            _context.Add(address);
            await _context.SaveChangesAsync();

            return _mapper.Map<AddressDto>(address);
        }

        public async Task UpdateAsync(Guid addressId, CreateAddressDto dto)
        {
            var address = await _context.Addresses.FindAsync(addressId)
                ?? throw new Exception("Адресу не знайдено");

            _mapper.Map(dto, address);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid addressId)
        {
            var address = await _context.Addresses.FindAsync(addressId)
                ?? throw new Exception("Адресу не знайдено");

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
        }
    }
}
