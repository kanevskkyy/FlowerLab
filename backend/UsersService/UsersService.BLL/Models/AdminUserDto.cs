// UsersService.BLL/Models/AdminUserDto.cs

namespace UsersService.BLL.Models
{
    public class AdminUserDto
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhotoUrl { get; set; }
        public int PersonalDiscountPercentage { get; set; }
        public string? Role { get; set; } 
    }
}