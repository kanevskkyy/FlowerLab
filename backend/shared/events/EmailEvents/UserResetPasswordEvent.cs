using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.events.EmailEvents
{
    public class UserResetPasswordEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? ResetPasswordURL { get; set; }
    }
}
