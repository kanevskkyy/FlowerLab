namespace UsersService.BLL.Models;

public class AddressDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public bool IsDefault { get; set; }
}