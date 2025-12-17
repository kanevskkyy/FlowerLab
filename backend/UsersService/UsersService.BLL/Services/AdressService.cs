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
using System.Data.SqlTypes;
using UsersService.BLL.Exceptions;

namespace UsersService.BLL.Services
{
    public class AddressService : IAddressService
    {
        private ApplicationDbContext context;
        private IMapper mapper;

        public AddressService(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<List<AddressDto>> GetUserAddressesAsync(string userId)
        {
            var addresses = await context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            return mapper.Map<List<AddressDto>>(addresses);
        }

        public async Task<AddressDto> CreateAsync(string userId, CreateAddressDto dto)
        {
            var exists = await context.Addresses.AnyAsync(a =>
                a.UserId == userId &&
                a.Address.ToLower() == dto.Address.ToLower());

            if (exists)
                throw new AlreadyExistsException("Така адреса вже існує!");

            var address = mapper.Map<UserAddresses>(dto);
            address.UserId = userId;

            if (dto.IsDefault)
            {
                var others = await context.Addresses
                    .Where(a => a.UserId == userId && a.IsDefault)
                    .ToListAsync();

                foreach (var a in others)
                    a.IsDefault = false;
            }
            else
            {
                bool hasDefault = await context.Addresses.AnyAsync(a => a.UserId == userId && a.IsDefault);
                if (!hasDefault)
                    address.IsDefault = true;
            }

            context.Add(address);
            await context.SaveChangesAsync();

            return mapper.Map<AddressDto>(address);
        }

        public async Task UpdateAsync(string userId, Guid addressId, CreateAddressDto dto)
        {
            var address = await context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId)
                ?? throw new NotFoundException("Адресу не знайдено!");

            var exists = await context.Addresses.AnyAsync(a =>
                a.UserId == userId &&
                a.Address.ToLower() == dto.Address.ToLower()
                && a.Id != addressId);

            if (exists)
                throw new AlreadyExistsException("Така адреса вже існує!");

            mapper.Map(dto, address);

            if (dto.IsDefault)
            {
                var others = await context.Addresses
                    .Where(a => a.UserId == userId && a.Id != addressId && a.IsDefault)
                    .ToListAsync();

                foreach (var a in others)
                    a.IsDefault = false;

                address.IsDefault = true;
            }
            else
            {
                bool hasOtherDefault = await context.Addresses.AnyAsync(a => a.UserId == userId && a.Id != addressId && a.IsDefault);
                if (!hasOtherDefault)
                    address.IsDefault = true; 
            }

            await context.SaveChangesAsync();
        }


        public async Task DeleteAsync(string userId, Guid addressId)
        {
            var address = await context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId)
                ?? throw new NotFoundException("Адресу не знайдено");

            context.Addresses.Remove(address);
            await context.SaveChangesAsync();
        }
    }
}
