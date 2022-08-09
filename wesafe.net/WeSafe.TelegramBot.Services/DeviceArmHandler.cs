using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeSafe.TelegramBot.Services
{
    public class DeviceArmHandler : CallbackHandler
    {
        private readonly IApiClient _apiClient;

        public DeviceArmHandler(IApiClient apiClient) : base(apiClient)
        {
            _apiClient = apiClient;
        }

        protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var parts = update.CallbackQuery.Data.Split(' ');

            if ( parts.Length > 1 && Int32.TryParse(parts[1], out var deviceId) )
            {
                var command = parts[0];

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

                await _apiClient.ArmDevice(deviceId, command == "arm", cancellationToken);

                deviceStatus.IsArmed = command == "arm";

                await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                    update.CallbackQuery.Message.MessageId, Statuses.GetDeviceStatusText(deviceStatus, settings),
                    cancellationToken: cancellationToken);

                await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id,
                    update.CallbackQuery.Message.MessageId, Keyboards.GetDeviceSettingsKeyboard(deviceStatus), cancellationToken);
            }
        }
    }
}