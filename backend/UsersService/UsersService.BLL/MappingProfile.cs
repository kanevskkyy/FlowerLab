// UsersService.BLL/MappingProfile.cs

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
                // ВИДАЛЕНО: .ForMember(dest => dest.PhotoURL, ...) - більше не потрібен
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<ApplicationUser, UserResponseDto>()
                // Тут теж, якщо імена однакові, можна видалити цей рядок
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(opt => opt.PhotoUrl));
        }
    }
}