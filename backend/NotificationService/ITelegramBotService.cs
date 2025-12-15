using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify
{
    public interface ITelegramBotService
    {
        Task SendMessageAsync(string message);
    }
}
