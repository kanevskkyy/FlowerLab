using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.DTOs
{
    public class LiqPaySettings
    {
        public string PublicKey { get; set; } = null!;
        public string PrivateKey { get; set; } = null!;
        public string ServerUrl { get; set; } = string.Empty;
    }
}
