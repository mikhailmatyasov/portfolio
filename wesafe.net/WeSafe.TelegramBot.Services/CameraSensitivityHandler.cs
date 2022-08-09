using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using WeSafe.Services.Client.Models;

namespace WeSafe.TelegramBot.Services
{
    public class CameraSensitivityHandler : CallbackHandler
    {
        private readonly IApiClient _apiClient;

        public CameraSensitivityHandler(IApiClient apiClient) : base(apiClient)
        {
            _apiClient = apiClient;
        }

        protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var parts = update.CallbackQuery.Data.Split(' ');

            if ( parts.Length > 2 && Int32.TryParse(parts[1], out var sens) && Int32.TryParse(parts[2], out var cameraId) )
            {
                var camera = await _apiClient.GetCameraById(cameraId, cancellationToken);

                if ( camera == null )
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat,
                        "Camera not found", cancellationToken: cancellationToken);

                    // Device stat
                    return;
                }


                RecognitionSettings recognition = new RecognitionSettings { Confidence = 90, Sensitivity = 7, AlertFrequency = 30 };

                if ( !String.IsNullOrEmpty(camera.RecognitionSettings) )
                {
                    try
                    {
                        recognition = JsonConvert.DeserializeObject<RecognitionSettings>(camera.RecognitionSettings);
                    }
                    catch ( Exception e )
                    {
                    }
                }

                recognition.Sensitivity = sens;

                camera.RecognitionSettings = JsonConvert.SerializeObject(recognition);

                await _apiClient.SaveCamera(camera, cancellationToken);

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