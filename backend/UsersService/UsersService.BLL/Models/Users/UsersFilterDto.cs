namespace UsersService.BLL.Models.Users;

public class UsersFilterDto
{
    public string? SearchTerm { get; set; } 
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}