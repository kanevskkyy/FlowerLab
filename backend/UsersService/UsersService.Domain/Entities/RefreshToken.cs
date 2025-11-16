namespace UsersService.Domain.Entities;

using System;
using System.ComponentModel.DataAnnotations.Schema;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } // Сам JWT Refresh Token
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; } // Для анулювання токена при виході

    // Зв'язок з ApplicationUser
    [ForeignKey("User")]
    public string UserId { get; set; } 
    public ApplicationUser User { get; set; }
}