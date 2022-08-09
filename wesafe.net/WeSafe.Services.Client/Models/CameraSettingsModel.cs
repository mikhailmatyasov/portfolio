using System;

namespace WeSafe.Services.Client.Models
{
    public class CameraSettingsModel
    {
        public int CameraId { get; set; }

        public DateTimeOffset? Mute { get; set; }
    }
}