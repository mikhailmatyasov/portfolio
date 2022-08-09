using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WeSafe.TelegramBot.Services
{
    public abstract class CallbackHandler : IUpdateHandler
    {
        protected CallbackHandler(IApiClient apiClient)
        {
            ApiClient = apiClient;
        }

        protected IApiClient ApiClient { get; }

        public virtual async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var chatId = update.CallbackQuery.Message.Chat.Id;
            var user = await ApiClient.GetUserByChatId(chatId, cancellationToken);

            if ( user == null || !user.IsActive )
            {
                await botClient.SendTextMessageAsync(
                    update.Message.Chat,
                    "You are not authorized in the system! To verify your Telegram account in " +
                    "the system please use mobile app, and share your contact via the button below.",
                    cancellationToken: cancellationToken,
                    replyMarkup: new ReplyKeyboardMarkup(
                        new[] { KeyboardButton.WithRequestContact("Send my phone number") }, true, true));

                return;
            }

            await HandleCallbackAsync(botClient, update, cancellationToken);
        }

        protected abstract Task HandleCallbackAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
    }
}