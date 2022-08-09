using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Results;

namespace WeSafe.Services
{
    public class TelegramClient : ITelegramClient
    {
        private readonly TelegramBotClient _botClient;
        private readonly ILogger<TelegramClient> _logger;
        private readonly TelegramOptions _options;

        public TelegramClient(IHttpClientFactory clientFactory, IOptions<TelegramOptions> options, ILogger<TelegramClient> logger)
        {
            var token = options.Value.Token;

            if (String.IsNullOrEmpty(token)) throw new Exception("Telegram token can not be null");

            _botClient = new TelegramBotClient(token, clientFactory.CreateClient("telegram"));
            _logger = logger;
            _options = options.Value;
        }

        public Task<IExecutionResult> SendPhotoAsync(string userId, Stream file, string caption)
        {
            return SendPhotoAsync(userId, new InputOnlineFile(file), caption);
        }

        public Task<IExecutionResult> SendPhotoAsync(long chatId, Stream file, string caption)
        {
            return SendPhotoAsync(chatId, new InputOnlineFile(file), caption);
        }

        public async Task SendMediaGroupAsync(Int64 chatId, IEnumerable<string> fileUrls, string caption)
        {
            try
            {
                _logger.LogDebug("Send to telegram. ChatId: {0}, Caption: {1}", chatId, caption);

                var media = fileUrls.Select(c => new InputMediaPhoto(new InputMedia(c))).ToList();
                var result = await _botClient.SendMediaGroupAsync(media, chatId);

                int messageId = result == null || result.Length == 0 ? 0 : result[0].MessageId;

                await _botClient.SendTextMessageAsync(chatId, caption);

                _logger.LogDebug("End sending to telegram. {0}", result.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error by sending photo to telegram bot: {e.Message}");
            }
        }

        public async Task SendToStatChatAsync(DeviceStatusModel device, string clientName)
        {
            var message = GetDeviceStatusMessage(device, clientName);

            await _botClient.SendTextMessageAsync(_options.StatChatId, message);
        }

        private string GetDeviceStatusMessage(DeviceStatusModel device, string clientName)
        {
            string status;
            string networkStatus;

            if (device.Status == "online") status = "✅";
            else if (device.Status == "offline") status = "❌";
            else status = "❔";

            if (device.NetworkStatus == "online") networkStatus = "✅";
            else if (device.NetworkStatus == "offline") networkStatus = "❌";
            else networkStatus = "❔";

            string message = $"Client {clientName}\nDevice {device.MACAddress} Status: {status}, Network status: {networkStatus}\n";

            if (!device.Cameras.Any())
            {
                return message + "No cameras were configured.\n";
            }

            foreach (var camera in device.Cameras)
            {
                if (camera.Status == "online") status = "✅";
                else if (camera.Status == "offline") status = "❌";
                else status = "❔";

                if (camera.NetworkStatus == "online") networkStatus = "✅";
                else if (camera.NetworkStatus == "offline") networkStatus = "❌";
                else networkStatus = "❔";

                message += $"Camera {camera.CameraName} (id={camera.Id}) Status: {status}, Network status: {networkStatus}\n";
            }

            return message;
        }

        private async Task<IExecutionResult> SendPhotoAsync(ChatId chatId, InputOnlineFile file, string caption)
        {
            try
            {
                _logger.LogDebug("Send to telegram. ChatId: {0}, Caption: {1}", chatId, caption);

                var result = await _botClient.SendPhotoAsync(chatId, file, caption);

                _logger.LogDebug("End sending to telegram. {0}", result.ToString());

                if (result.Photo != null && result.Photo.Length > 0)
                    return ExecutionResult.Payload(result.Photo[0].FileId);
                else return ExecutionResult.Failed();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error by sending photo to telegram bot: {e.Message}");

                return ExecutionResult.Failed(e.Message);
            }
        }
    }
}