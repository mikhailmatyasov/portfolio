using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WeSafe.Services.Client.Models;

namespace WeSafe.TelegramBot.Services
{
    public class RegisterCommand : IUpdateHandler
    {
        private readonly IApiClient _apiClient;

        public RegisterCommand(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var model = new RegisterTelegramUserModel
            {
                TelegramId = update.Message.Chat.Id,
                Phone = update.Message.Contact.PhoneNumber,
                FirstName = update.Message.Contact.FirstName,
                LastName = update.Message.Contact.LastName
            };

            if ( !model.Phone.StartsWith("+") ) model.Phone = "+" + model.Phone;

            var result = await _apiClient.RegisterTelegramUser(model, cancellationToken);

            if ( result == null )
            {
                await botClient.SendTextMessageAsync(update.Message.Chat,
                    "Sorry. Your phone number is not in the list of users.", cancellationToken: cancellationToken);

                return;
            }

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