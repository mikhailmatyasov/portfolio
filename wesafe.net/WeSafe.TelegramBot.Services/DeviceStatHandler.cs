using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeSafe.TelegramBot.Services
{
    public class DeviceStatHandler : CallbackHandler
    {
        private readonly IApiClient _apiClient;

        public DeviceStatHandler(IApiClient apiClient) : base(apiClient)
        {
            _apiClient = apiClient;
        }

        protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            var parts = update.CallbackQuery.Data.Split(' ');

            if ( parts.Length > 1 && Int32.TryParse(parts[1], out var deviceId) )
            {
                await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id,
                    update.CallbackQuery.Message.MessageId, null, cancellationToken);

                var status = await _apiClient.GetSystemStatus(update.CallbackQuery.Message.Chat.Id, cancellationToken);
                var settings = await _apiClient.GetUserSettings(update.CallbackQuery.Message.Chat.Id, cancellationToken);
                var deviceStatus = status.FirstOrDefault(c => c.Id == deviceId);

                if ( deviceStatus == null )
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat,
                        "Device isn\'t available", cancellationToken: cancellationToken,
                        replyMarkup: Keyboards.GetSystemKeyboard(status, settings));

                    return;
                }

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat,
                    Statuses.GetDeviceStatusText(deviceStatus, settings),
                    cancellationToken: cancellationToken,
                    replyMarkup: Keyboards.GetDeviceKeyboard(deviceStatus, settings, status.Count() > 1));
            }
        }
    }
}