using UsersService.Domain.Entities;
using UsersService.BLL.Models;
using System.Collections.Generic;

namespace UsersService.BLL.Interfaces
{
    public interface IJwtService
    {
        TokenResponseDto CreateTokens(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();
    }
}