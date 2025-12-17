using AutoMapper;
using UsersService.BLL.Models.Adresess;
using UsersService.BLL.Models.Users;
using UsersService.Domain.Entities;

namespace UsersService.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, AdminUserDto>()
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<ApplicationUser, UserResponseDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(opt => opt.PhotoUrl));

            CreateMap<UserAddresses, AddressDto>();

            CreateMap<CreateAddressDto, UserAddresses>();
        }
    }
}