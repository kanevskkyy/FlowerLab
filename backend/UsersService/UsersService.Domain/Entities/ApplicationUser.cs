namespace UsersService.Domain.Entities;

using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Поля згідно з дизайном (Sign Up page)
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    
    public int PersonalDiscountPercentage { get; set; } = 0;
    // Зв'язок 1:N з Refresh Токенами
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    
    // Зв'язок 1:N з Адресами
    public ICollection<UserAddress> Addresses { get; set; } 
}