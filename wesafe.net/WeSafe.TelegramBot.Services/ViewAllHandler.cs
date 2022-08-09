using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeSafe.TelegramBot.Services
{
    public class ViewAllHandler : CallbackHandler
    {
        private readonly IApiClient _apiClient;
        private readonly IFileStorage _fileStorage;

        public ViewAllHandler(IApiClient apiClient, IFileStorage fileStorage) : base(apiClient)
        {
            _apiClient = apiClient;
            _fileStorage = fileStorage;
        }

        protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var parts = update.CallbackQuery.Data.Split(' ');

            if ( parts.Length > 1 && Int32.TryParse(parts[1], out var deviceId) )
            {
                var cameras = await _apiClient.GetDeviceCameras(deviceId, cancellationToken);

                if ( !cameras.Any() )
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat,
                        "No cameras are configured for device.\n\nUse command /start to try again.", cancellationToken: cancellationToken);

                    return;
                }

                var images = new List<IAlbumInputMedia>();

                foreach ( var camera in cameras )
                {
                    if ( !String.IsNullOrEmpty(camera.LastImagePath) )
                    {
                        var filePath = _fileStorage.GetFilePath(camera.LastImagePath);

                        images.Add(new InputMediaPhoto(new InputMedia(System.IO.File.OpenRead(filePath),
                            Path.GetFileName(filePath))));
                    }
#if DEBUG
                    else
                    {
                        images.Add(new InputMediaPhoto(new InputMedia(System.IO.File.OpenRead(/*camera.LastImagePath*/"e:\\photo.jpg"),
                            "photo.jpg")));
                    }
#endif
                }

                await botClient.SendMediaGroupAsync(images, update.CallbackQuery.Message.Chat,
                    cancellationToken: cancellationToken);

                // TODO: See DeviceStatHandler (need refactoring)

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