using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.BLL.Models.Adresess
{
    public class CreateAddressDto
    {
        public string Address { get; set; } = null!;
        public bool IsDefault { get; set; }
    }
}
