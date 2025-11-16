// UsersService.BLL/Models/UpdateDiscountDto.cs

using System.ComponentModel.DataAnnotations;

namespace UsersService.BLL.Models
{
    public class UpdateDiscountDto
    {
        [Range(0, 100)]
        public int PersonalDiscountPercentage { get; set; }
    }
}