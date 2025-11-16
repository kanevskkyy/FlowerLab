using AutoMapper;
using UsersService.BLL.Models;
using UsersService.Domain.Entities;

namespace UsersService.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Мапінг DTO <-> Entity
            CreateMap<UserAddress, AddressDto>().ReverseMap();
            CreateMap<ApplicationUser, AdminUserDto>()
                .ForMember(dest => dest.Role, opt => opt.Ignore()); // Ігноруємо, оскільки це поле заповнюється вручну в сервісі
            // Мапінг для User (додамо пізніше)
            // CreateMap<ApplicationUser, UserResponseDto>();
        }
    }
}