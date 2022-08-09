using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WeSafe.Services.Client.Models;

namespace WeSafe.TelegramBot.Services
{
    public class Statuses
    {
        public static string GetSystemStatusText(IEnumerable<DeviceStatusModel> devices, UserSettingsModel settings)
        {
            var message = "System status\n\n";

            if ( !devices.Any() )
            {
                message += "No devices are available for you.\nUse command /start to try again.";

                return message;
            }

            foreach ( var device in devices )
            {
                message += GetDeviceStatusText(device, settings);

                if ( devices.Count() > 1 ) message += "\n";
            }

            return message;
        }

        public static string GetDeviceStatusText(DeviceStatusModel device, UserSettingsModel settings)
        {
            var status = "❔";
            var networkStatus = "❔";

            if ( device.Status == "online" ) status = "✅";
            else if ( device.Status == "offline" ) status = "❌";

            if ( device.NetworkStatus == "online" ) networkStatus = "✅";
            else if ( device.NetworkStatus == "offline" ) networkStatus = "❌";

            var message = $"Device: {device.Name ?? device.MACAddress} Status: {status}, Network status: {networkStatus}\nArmed status: {(device.IsArmed ? "✅" : "❌")}\n";

            if ( !device.Cameras.Any() )
            {
                return message + "No cameras were configured.\n";
            }

            foreach ( var camera in device.Cameras )
            {
                status = "❔";
                networkStatus = "❔";

                if ( camera.Status == "online" ) status = "✅";
                else if ( camera.Status == "offline" ) status = "❌";

                if ( camera.NetworkStatus == "online" ) networkStatus = "✅";
                else if ( camera.NetworkStatus == "offline" ) networkStatus = "❌";

                var cameraSettings = settings?.Cameras.FirstOrDefault(c => c.CameraId == camera.Id);

                if ( cameraSettings == null ) message += $"Camera {camera.CameraName} 🔔 Status: {status}, Network status: {networkStatus}\n";
                else
                {
                    if ( cameraSettings.Mute == null || DateTimeOffset.UtcNow > cameraSettings.Mute )
                    {
                        message += $"Camera: {camera.CameraName} 🔔 Status: {status}, Network status: {networkStatus}\n";
                    }
                    else
                    {
                        message += $"Camera: {camera.CameraName} 🔕 Status: {status}, Network status: {networkStatus}\n";
                    }
                }
            }

            return message;
        }

        public static string GetCameraStatusText(CameraModel camera, UserSettingsModel settings)
        {
            var message = $"Camera {camera.CameraName}\n\n";
            var cameraSettings = settings?.Cameras.FirstOrDefault(c => c.CameraId == camera.Id);

            if ( cameraSettings?.Mute == null || DateTimeOffset.UtcNow > cameraSettings.Mute ) message += "Mute status: 🔔\n";
            else message += "Mute status: 🔕\n";

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

            message += $"Confidence: {recognition.Confidence}%\nSensitivity: {recognition.Sensitivity}/10";

            return message;
        }
    }
}