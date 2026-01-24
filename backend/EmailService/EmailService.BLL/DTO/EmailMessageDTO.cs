using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.BLL.DTO
{
    public class EmailMessageDTO
    {
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? HtmlBody { get; set; }
    }
}
