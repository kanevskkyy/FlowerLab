// UsersService.BLL/Models/AdminUserDto.cs

using UsersService.BLL.Models.Adresess;

namespace UsersService.BLL.Models.Users
{
    public class AdminUserDto
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PhotoUrl { get; set; }
        public int PersonalDiscountPercentage { get; set; }
        public string? Role { get; set; }
        public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();
    }
}