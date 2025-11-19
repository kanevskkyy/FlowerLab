using AutoMapper;
using UsersService.BLL.Models;
using UsersService.Domain.Entities;

namespace UsersService.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, AdminUserDto>()
                .ForMember(dest => dest.PhotoURL, opt => opt.MapFrom(opt => opt.PhotoUrl))
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<ApplicationUser, UserResponseDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(opt => opt.PhotoUrl));
        }
    }
}