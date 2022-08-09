using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using WeSafe.Services.Client.Models;

namespace WeSafe.Monitoring.Services
{
    public class MonitoringService : IMonitoringService
    {
        private readonly ILogger<MonitoringService> _logger;
        private readonly IApiClient _apiClient;
        private readonly MonitoringOptions _options;
        private readonly ITelegramBotClient _botClient;

        public MonitoringService(ILogger<MonitoringService> logger, IApiClient apiClient, IOptionsSnapshot<MonitoringOptions> options)
        {
            _logger = logger;
            _apiClient = apiClient;
            _options = options.Value;
            _botClient = new TelegramBotClient(_options.TelegramToken);
        }

        public async Task ProcessAsync(bool notification, CancellationToken cancellationToken)
        {
            var devices = await _apiClient.GetDevices(true, null, cancellationToken);

            foreach ( var device in devices )
            {
                bool statUpdate = false;
                bool deviceOnline = false;

                var timeMark = DateTimeOffset.UtcNow.AddMinutes(-_options.Delay);
                var cameras = await _apiClient.GetDeviceCameras(device.Id, timeMark, false, cancellationToken);

                foreach ( var camera in cameras )
                {
                    if ( camera.HasRecent ) deviceOnline = true;

                    if ( await UpdateCameraStatus(device, camera, cancellationToken) ) statUpdate = true;
                }

                if ( await UpdateDeviceStatus(deviceOnline, device, cancellationToken) ) statUpdate = true;

                if ( statUpdate )
                {
                    await _apiClient.StatusChanged(new DeviceUpdateStatusModel { Id = device.Id }, cancellationToken);

                    await _botClient.SendTextMessageAsync(_options.TelegramChannelId,
                        await GetDeviceStatusMessage(device, cancellationToken), cancellationToken: cancellationToken);
                }
            }

            if ( notification )
            {
                devices = await _apiClient.GetDevices(false, "offline", cancellationToken);

                var message = "";

                foreach ( var device in devices )
                {
                    string status;

                    if ( device.Status == "online" ) status = "✅";
                    else if ( device.Status == "offline" ) status = "❌";
                    else status = "❔";

                    message += $"C={device.ClientName}, d={device.MACAddress} {status}\n";
                }

                if ( !String.IsNullOrWhiteSpace(message) )
                {
                    await _botClient.SendTextMessageAsync(_options.TelegramChannelId, message,
                        cancellationToken: cancellationToken);
                }
            }
        }

        private async Task<bool> UpdateDeviceStatus(bool deviceOnline, DeviceShortModel device, CancellationToken cancellationToken)
        {
            string deviceStatus = null;

            if ( deviceOnline && device.Status != "online" )
            {
                deviceStatus = "online";
            }
            else if ( !deviceOnline && device.Status != "offline" )
            {
                deviceStatus = "offline";
            }

            if ( deviceStatus != null )
            {
                _logger.LogInformation(
                    $"Device mac={{DeviceMacAddress}} status change: \"{{DeviceStatus}}\" -> \"{deviceStatus}\"",
                    device.MACAddress, device.Status);

                device.Status = deviceStatus;

                await _apiClient.UpdateDeviceStatus(new DeviceUpdateStatusModel
                {
                    Id = device.Id,
                    Status = deviceStatus
                }, cancellationToken);

                return true;
            }

            _logger.LogDebug(
                "Device mac={DeviceMacAddress} same status: \"{DeviceStatus}\"",
                device.MACAddress, device.Status);

            return false;
        }

        private async Task<bool> UpdateCameraStatus(DeviceShortModel device, CameraMonitoringModel camera,
            CancellationToken cancellationToken)
        {
            string cameraStatus;

            if ( camera.HasRecent )
            {
                cameraStatus = camera.Status != "online" ? "online" : null;
            }
            else
            {
                cameraStatus = camera.Status != "offline" ? "offline" : null;
            }

            if ( cameraStatus != null )
            {
                _logger.LogInformation(
                    $"Device mac={{DeviceMacAddress}}, camera {{CameraName}} status change: \"{{CameraStatus}}\" -> \"{cameraStatus}\"",
                    device.MACAddress, camera.CameraName, camera.Status);

                camera.Status = cameraStatus;

                await _apiClient.UpdateCameraStatus(new CameraUpdateStatusModel
                {
                    Id = camera.Id,
                    Status = cameraStatus
                }, cancellationToken);

                return true;
            }

            _logger.LogDebug(
                "Device mac={DeviceMacAddress}, camera {CameraName} same status: \"{CameraStatus}\"",
                device.MACAddress, camera.CameraName, camera.Status);

            return false;
        }

        private async Task<string> GetDeviceStatusMessage(DeviceShortModel device, CancellationToken cancellationToken)
        {
            string status;
            string networkStatus;

            if ( device.Status == "online" ) status = "✅";
            else if ( device.Status == "offline" ) status = "❌";
            else status = "❔";

            if ( device.NetworkStatus == "online" ) networkStatus = "✅";
            else if ( device.NetworkStatus == "offline" ) networkStatus = "❌";
            else networkStatus = "❔";

            string message = $"Client {device.ClientName}\nDevice {device.MACAddress} Status: {status}, Network status: {networkStatus}\n";
            var cameras = await _apiClient.GetDeviceCameras(device.Id, null, true, cancellationToken);

            if ( !cameras.Any() )
            {
                _logger.LogInformation("No cameras for client id={ClientId}", device.ClientId);

                return message + "No cameras were configured.\n";
            }

            foreach ( var camera in cameras )
            {
                if ( camera.Status == "online" ) status = "✅";
                else if ( camera.Status == "offline" ) status = "❌";
                else status = "❔";

                if ( camera.NetworkStatus == "online" ) networkStatus = "✅";
                else if ( camera.NetworkStatus == "offline" ) networkStatus = "❌";
                else networkStatus = "❔";

                message += $"Camera {camera.CameraName} (id={camera.Id}) Status: {status}, Network status: {networkStatus}\n";
            }

            return message;
        }
    }
}