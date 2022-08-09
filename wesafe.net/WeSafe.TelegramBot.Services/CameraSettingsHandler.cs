using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeSafe.TelegramBot.Services
{
    public class CameraSettingsHandler : CallbackHandler
    {
        private readonly IApiClient _apiClient;

        public CameraSettingsHandler(IApiClient apiClient) : base(apiClient)
        {
            _apiClient = apiClient;
        }

        protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var parts = update.CallbackQuery.Data.Split(' ');

            if ( parts.Length > 1 && Int32.TryParse(parts[1], out var cameraId) )
            {
                await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id,
                    update.CallbackQuery.Message.MessageId, null, cancellationToken);

                var settings = await _apiClient.GetUserSettings(update.CallbackQuery.Message.Chat.Id, cancellationToken);
                var camera = await _apiClient.GetCameraById(cameraId, cancellationToken);

                if ( camera == null )
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat,
                        "Camera not found", cancellationToken: cancellationToken);

                    return;
                }

                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat,
                    Statuses.GetCameraStatusText(camera, settings),
                    cancellationToken: cancellationToken,
                    replyMarkup: Keyboards.GetCameraKeyboard(camera));
            }
        }
    }
}