namespace UsersService.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using static System.Net.WebRequestMethods;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public string? PhotoUrl { get; set; } = "https://res.cloudinary.com/dg9clyn4k/image/upload/v1763712578/order-service/gifts/mpfiss97mfebcqwm6elb.jpg";
    public int PersonalDiscountPercentage { get; set; } = 0;
    public ICollection<RefreshToken> RefreshTokens { get; set; }
}