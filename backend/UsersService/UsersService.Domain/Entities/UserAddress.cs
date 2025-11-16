namespace UsersService.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public class UserAddress
{
    public int Id { get; set; }
    public string FullName { get; set; } // Ім'я одержувача (як у дизайні "Addresses")
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public bool IsDefault { get; set; } // Якщо клієнт може мати основну адресу

    // Зв'язок з ApplicationUser
    [ForeignKey("User")]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}