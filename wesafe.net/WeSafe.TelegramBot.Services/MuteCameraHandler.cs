using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using WeSafe.Services.Client.Models;

namespace WeSafe.TelegramBot.Services
{
    public class MuteCameraHandler : CallbackHandler
    {
        private readonly IApiClient _apiClient;

        public MuteCameraHandler(IApiClient apiClient) : base(apiClient)
        {
            _apiClient = apiClient;
        }

        protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var parts = update.CallbackQuery.Data.Split(';');

            if ( parts.Length > 2 && DateTimeOffset.TryParse(parts[1], out var mute) && Int32.TryParse(parts[2], out var cameraId) )
            {
                var camera = await _apiClient.GetCameraById(cameraId, cancellationToken);

                if ( camera == null )
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat,
                        "Camera not found", cancellationToken: cancellationToken);

                    // Device stat
                    return;
                }

                // API call
                await _apiClient.MuteCamera(update.CallbackQuery.Message.Chat.Id, new CameraSettingsModel
                    {
                        CameraId = cameraId,
                        Mute = mute
                    },
                    cancellationToken);

                var settings = await _apiClient.GetUserSettings(update.CallbackQuery.Message.Chat.Id, cancellationToken);

                await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                    update.CallbackQuery.Message.MessageId, Statuses.GetCameraStatusText(camera, settings),
                    cancellationToken: cancellationToken);

                await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id,
                    update.CallbackQuery.Message.MessageId, Keyboards.GetCameraKeyboard(camera), cancellationToken);
            }
        }
    }
}