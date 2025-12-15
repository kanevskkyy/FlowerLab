using Telegram.Bot;

namespace Notify
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly TelegramBotClient botClient;
        private readonly long adminChatId;

        public TelegramBotService(string botToken, long adminChatId)
        {
            if (string.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentNullException(nameof(botToken));
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
                Console.WriteLine($"Помилка відправки в Telegram: {ex.Message}");
                throw;
            }
        }
    }
}