using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;
using WeSafe.Web.Core.Hubs;
using WeSafe.Web.Core.Models;
using Camera = WeSafe.DAL.Entities.Camera;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents recognition event operations.
    /// </summary>
    [ApiController]
    public class FrameController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly IClientService _clientService;
        private readonly ICameraService _cameraService;
        private readonly ICameraLogService _cameraLogService;
        private readonly ITelegramService _telegramService;
        private readonly ILogger<FrameController> _logger;
        private readonly ITelegramClient _telegramClient;
        private readonly TelegramOptions _telegramOptions;
        private readonly IFileStorage _fileStorage;
        private readonly ICloudStorage _cloudStorage;
        private readonly IMobileService _mobileService;
        private readonly IHubContext<EventsHub> _hubContext;
        private readonly UserManager<User> _userManager;

        public FrameController(IDeviceService deviceService, IClientService clientService,
            ICameraService cameraService, ICameraLogService cameraLogService,
            ITelegramService telegramService, ILogger<FrameController> logger, ITelegramClient telegramClient,
            IOptionsSnapshot<TelegramOptions> options, IFileStorage fileStorage, ICloudStorage cloudStorage,
            IMobileService mobileService, IHubContext<EventsHub> hubContext, UserManager<User> userManager)
        {
            _deviceService = deviceService;
            _clientService = clientService;
            _cameraService = cameraService;
            _cameraLogService = cameraLogService;
            _telegramService = telegramService;
            _logger = logger;
            _telegramClient = telegramClient;
            _telegramOptions = options.Value;
            _fileStorage = fileStorage;
            _cloudStorage = cloudStorage;
            _mobileService = mobileService;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        /// <summary>
        /// Adds event to the storage.
        /// </summary>
        /// <param name="model">The event model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("api/frame")]
        public async Task<IActionResult> CreateFrameAsync([FromForm] RecognitionEventModel model)
        {
            if (model?.Frame == null) return BadRequest();

            var frames = new RecognitionEventsModel
            {
                DeviceMAC = model.DeviceMAC,
                CameraId = model.CameraId,
                CameraIP = model.CameraIP,
                Alert = model.Alert,
                Message = model.Message,
                Frames = new FormFileCollection { model.Frame }
            };

            return await ProcessFrames(frames);
        }

        /// <summary>
        /// Adds events to the storage.
        /// </summary>
        /// <param name="model">The event collection.</param>
        /// <returns>The action result.</returns>
        [HttpPost("api/frames")]
        public async Task<IActionResult> CreateFramesAsync([FromForm] RecognitionEventsModel model)
        {
            return await ProcessFrames(model);
        }

        private async Task<IActionResult> ProcessFrames(RecognitionEventsModel model)
        {
            if (model?.Frames == null || model.Frames.Count == 0) return BadRequest();

            var device = await _deviceService.GetDeviceByMAC(model.DeviceMAC);

            if (device?.ClientId == null) return BadRequest($"Couldn't get device with MAC={model.DeviceMAC}");

            var client = await _clientService.GetClientById(device.ClientId.Value);

            if (client == null) return BadRequest("Couldn't get client for device");

            Camera camera;

            if ( model.CameraId != 0 )
            {
                camera = await _cameraService.Cameras.SingleOrDefaultAsync(c => c.DeviceId == device.Id && c.Id == model.CameraId);
            }
            else
            {
                camera = await _cameraService.Cameras.SingleOrDefaultAsync(c => c.DeviceId == device.Id && c.Ip == model.CameraIP);
            }

            if (camera == null) return BadRequest("Couldn't get any camera for device");

            var alert = model.Alert == "on" || model.Alert == "object" || model.Alert == "motion";
            bool active = camera.IsActive && client.IsActive && device.IsArmed && camera.IsActiveSchedule(DateTime.UtcNow, device.TimeZone);

            _logger.LogInformation(
                "Frame request: Device ID={5}, Device MAC={0}, Camera ID={6}, Camera IP={2}, Alert={3}, Message={4}",
                model.DeviceMAC, model.CameraIP, model.Alert, model.Message, device.Id, camera.Id);

            if (active && alert)
            {
                var fileList = new List<CameraLogEntryModel>();

                foreach (var file in model.Frames)
                {
                    var fileName = _cloudStorage.GetRandomFileName("jpg");

                    var url = await _cloudStorage.CreateFileAsync(fileName, file.ContentType, file.OpenReadStream());

                    fileList.Add(new CameraLogEntryModel
                    {
                        ImageUrl = url,
                        UrlExpiration = _cloudStorage.GetExpirationTime()
                    });
                }

                var notificationText = $"Alert from Camera {camera.CameraName}!";

                if (model.Alert == "object") notificationText += " Object detected!";
                else if (model.Alert == "motion") notificationText += " Motion detected!";

                var message = new { received = model.Message, meta = model.Alert };
                var log = new CameraLogModel
                {
                    CameraId = camera.Id,
                    Alert = true,
                    Parameters = JsonConvert.SerializeObject(message),
                    Message = notificationText,
                    Entries = fileList
                };

                var logId = await _cameraLogService.CreateCameraLog(log);

                try
                {
                    await SendTelegramNotification(fileList.Select(c => c.ImageUrl), client, camera, notificationText);

                    var users = await _mobileService.GetClientMobileSubscribers(client.Id);

                    await _mobileService.SendNotifications(users, new MobileNotificationParams
                    {
                        DeviceId = camera.DeviceId,
                        CameraId = camera.Id,
                        LogId = logId,
                        Title = client.Name,
                        NotificationText = notificationText,
                        ImageUrls = fileList.Select(c => c.ImageUrl)
                    });

                    var user = await _userManager.Users.FirstOrDefaultAsync(c => c.ClientId == client.Id);

                    await NotifyEventsHubConnections(users, user?.Id, await _mobileService.GetEvent(logId));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);

                    return BadRequest("Couldn't notify all users");
                }
            }

            if (!alert)
            {
                try
                {
                    _fileStorage.DeleteFile(camera.LastImagePath);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }

                var fileName = _fileStorage.GetRandomFileName("jpg");

                await _fileStorage.CreateFileAsync(fileName, model.Frames[0].OpenReadStream());

                camera.LastImagePath = _fileStorage.GetFileUrl(fileName);
            }

            camera.LastActivityTime = DateTimeOffset.UtcNow;

            await _cameraService.UpdateAsync(camera);

            return Ok();
        }

        private async Task NotifyEventsHubConnections(IEnumerable<ClientMobileSubscriberModel> users, string ownUserId, CameraLogModel log)
        {
            var ids = new List<string>();

            foreach (var user in users)
            {
                var cameraMute = user.CameraSettings
                                     .Where(c => c.CameraId == log.CameraId)
                                     .Select(c => c.Mute)
                                     .FirstOrDefault();

                if ((user.Mute == null || DateTimeOffset.UtcNow > user.Mute) &&
                     (cameraMute == null || DateTimeOffset.UtcNow > cameraMute) &&
                     user.Assignments.IsAssignmentAllowed(log.DeviceId, log.CameraId))
                {
                    ids.Add(user.Phone);
                }
            }

            if (ids.Any()) await _hubContext.Clients.Users(ids).SendAsync("ReceiveEvent", log);
            if (!String.IsNullOrEmpty(ownUserId)) await _hubContext.Clients.User(ownUserId).SendAsync("ReceiveEvent", log);
        }

        private async Task SendTelegramNotification(IEnumerable<string> fileUrls, ClientModel client, Camera camera,
            string notificationText)
        {
            var users = await _telegramService.GetClientTelegramSubscribers(client.Id);

            foreach (var user in users)
            {
                var cameraMute = user.CameraSettings
                                     .Where(c => c.CameraId == camera.Id)
                                     .Select(c => c.Mute)
                                     .FirstOrDefault();

                if ((user.Mute == null || DateTimeOffset.UtcNow > user.Mute) &&
                     (cameraMute == null || DateTimeOffset.UtcNow > cameraMute) &&
                     user.Assignments.IsAssignmentAllowed(camera.DeviceId, camera.Id))
                {
                    //foreach ( var file in model.Frames )
                    //{
                    //    await _telegramClient.SendPhotoAsync(user.TelegramId, file.OpenReadStream(),
                    //        notificationText);
                    //}
                    await _telegramClient.SendMediaGroupAsync(user.TelegramId, fileUrls, notificationText);
                }
            }

            if (_telegramOptions.SendToDevChat && client.SendToDevChat)
            {
                //foreach ( var file in model.Frames )
                //{
                //    await _telegramClient.SendPhotoAsync(_telegramOptions.DevChatId,
                //        file.OpenReadStream(),
                //        $"Client name {client.Name}. {notificationText}");
                //}
                await _telegramClient.SendMediaGroupAsync(_telegramOptions.DevChatId, fileUrls, $"Client name {client.Name}. {notificationText}");
            }
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader =
                MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);

            // UTF-7 is insecure and shouldn't be honored. UTF-8 succeeds in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }
    }
}