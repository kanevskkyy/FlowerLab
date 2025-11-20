namespace UsersService.BLL.Models;

public class UsersFilterDto
{
    public string? SearchTerm { get; set; } // Для загального пошуку (по імені/прізвищу)
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}