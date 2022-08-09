using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WeSafe.TelegramBot.Services
{
    public class StartCommand : IUpdateHandler
    {
        private readonly IApiClient _apiClient;

        public StartCommand(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            var chatId = update.Message.Chat.Id;
            var user = await _apiClient.GetUserByChatId(chatId, cancellationToken);

            if ( user == null )
            {
                await botClient.SendTextMessageAsync(
                    update.Message.Chat,
                    "Welcome to WeSmart Telegram bot. This bot allows authorized users to control " +
                    "WeSafe surveillance system and receive alerts.\nTo verify your Telegram account in " +
                    "the system please use mobile app, and share your contact via the button below."
                    ,
                    cancellationToken: cancellationToken,
                    replyMarkup: new ReplyKeyboardMarkup(
                        new[] { KeyboardButton.WithRequestContact("Send my phone number") }, true, true));
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat,
                    "You are authorized user! :)",
                    cancellationToken: cancellationToken,
                    replyMarkup: new ReplyKeyboardMarkup(
                        new[] { new KeyboardButton("System menu") }, true, true));

                var status = await _apiClient.GetSystemStatus(update.Message.Chat.Id, cancellationToken);
                var settings = await _apiClient.GetUserSettings(update.Message.Chat.Id, cancellationToken);

                await botClient.SendTextMessageAsync(update.Message.Chat,
                    Statuses.GetSystemStatusText(status, settings),
                    cancellationToken: cancellationToken,
                    replyMarkup: Keyboards.GetSystemKeyboard(status, settings));
            }
        }
    }
}