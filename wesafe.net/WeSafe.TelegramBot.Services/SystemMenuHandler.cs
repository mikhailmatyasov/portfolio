using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeSafe.TelegramBot.Services
{
    public class SystemMenuHandler : IUpdateHandler
    {
        private readonly IApiClient _apiClient;

        public SystemMenuHandler(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var chat = update.CallbackQuery?.Message.Chat ?? update.Message.Chat;

            if ( update.CallbackQuery != null )
            {
                try
                {
                    await botClient.EditMessageReplyMarkupAsync(chat.Id,
                        update.CallbackQuery.Message.MessageId, null, cancellationToken);
                }
                catch ( Exception e )
                {
                }
            }

            var status = await _apiClient.GetSystemStatus(chat.Id, cancellationToken);
            var settings = await _apiClient.GetUserSettings(chat.Id, cancellationToken);

            await botClient.SendTextMessageAsync(chat,
                Statuses.GetSystemStatusText(status, settings),
                cancellationToken: cancellationToken,
                replyMarkup: Keyboards.GetSystemKeyboard(status, settings));
        }
    }
}