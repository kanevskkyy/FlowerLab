using System.ComponentModel.DataAnnotations;

namespace UsersService.BLL.Models
{
    public class RegistrationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}