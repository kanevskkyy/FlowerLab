using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.Models.Adresess;

namespace UsersService.BLL.Models.Users
{
    public class UserResponseDto
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? PhotoUrl { get; set; }
        public int PersonalDiscountPercentage { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();
    }

}
