using UsersService.Domain.Entities;
using System.Collections.Generic;
using UsersService.BLL.Models.Auth;

namespace UsersService.BLL.Services.Interfaces
{
    public interface IJwtService
    {
        TokenResponseDto CreateTokens(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();
    }
}