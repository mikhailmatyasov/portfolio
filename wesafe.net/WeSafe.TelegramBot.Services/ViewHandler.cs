using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeSafe.TelegramBot.Services
{
    public class ViewHandler : CallbackHandler
    {
        private readonly IApiClient _apiClient;
        private readonly IFileStorage _fileStorage;

        public ViewHandler(IApiClient apiClient, IFileStorage fileStorage) : base(apiClient)
        {
            _apiClient = apiClient;
            _fileStorage = fileStorage;
        }

        protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var parts = update.CallbackQuery.Data.Split(' ');

            if ( parts.Length > 1 && Int32.TryParse(parts[1], out var cameraId) )
            {
                var camera = await _apiClient.GetCameraById(cameraId, cancellationToken);

                if ( camera == null )
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat,
                        "Camera not found.", cancellationToken: cancellationToken);

                    return;
                }

                InputMedia image = null;

#if DEBUG
                if ( !String.IsNullOrEmpty(camera.LastImagePath) )
                {
                    var filePath = _fileStorage.GetFilePath(camera.LastImagePath);

                    image = new InputMedia(System.IO.File.OpenRead(filePath),
                        Path.GetFileName(filePath));
                }
                else
                {
                    image = new InputMedia(System.IO.File.OpenRead("e:\\photo.jpg"),
                        "photo.jpg");
                }
#else
                if ( !String.IsNullOrEmpty(camera.LastImagePath) )
                {
                    var filePath = _fileStorage.GetFilePath(camera.LastImagePath);

                    image = new InputMedia(System.IO.File.OpenRead(filePath),
                        Path.GetFileName(filePath));
                }
                else
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat,
                        "Camera has not sent any frames yet.", cancellationToken: cancellationToken);

                    return;
                }
#endif

                await botClient.SendPhotoAsync(update.CallbackQuery.Message.Chat, image,
                    cancellationToken: cancellationToken);

                // TODO: See DeviceStatHandler (need refactoring)

                await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id,
                    update.CallbackQuery.Message.MessageId, null, cancellationToken);

                var status = await _apiClient.GetSystemStatus(update.CallbackQuery.Message.Chat.Id, cancellationToken);
                var settings = await _apiClient.GetUserSettings(update.CallbackQuery.Message.Chat.Id, cancellationToken);
                var deviceStatus = status.FirstOrDefault(c => c.Id == camera.DeviceId);

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