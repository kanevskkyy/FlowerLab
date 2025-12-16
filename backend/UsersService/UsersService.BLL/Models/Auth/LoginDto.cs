using System.ComponentModel.DataAnnotations;

namespace UsersService.BLL.Models.Auth
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}   