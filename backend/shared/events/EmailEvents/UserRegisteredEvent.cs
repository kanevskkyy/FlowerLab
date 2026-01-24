using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.events.EmailEvents
{
    public class UserRegisteredEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public string? UserFirstName { get; set; }
        public string? UserEmail { get; set; }
        public string? ConfirmURL { get; set; }
    }
}
