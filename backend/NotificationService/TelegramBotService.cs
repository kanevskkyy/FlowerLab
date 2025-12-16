using Telegram.Bot;
using System;
using System.Threading.Tasks;

namespace Notify
{
    public class TelegramBotService : ITelegramBotService
    {
        private TelegramBotClient botClient;
        private long adminChatId;

        public TelegramBotService(string botToken, long adminChatId)
        {
            if (string.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentNullException(nameof(botToken), "Bot token cannot be null or empty");
            }

            botClient = new TelegramBotClient(botToken);
            this.adminChatId = adminChatId;
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                await botClient.SendMessage(
                    chatId: adminChatId,
                    text: message
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send Telegram message: {ex.Message}");
                throw;
            }
        }
    }
}