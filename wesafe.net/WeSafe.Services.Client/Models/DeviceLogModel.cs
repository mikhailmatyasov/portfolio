using System;
using WeSafe.Shared.Enumerations;

namespace WeSafe.Services.Client.Models
{
    public class DeviceLogModel
    {
        public int DeviceId { get; set; }

        public int? CameraId { get; set; }

        public LogLevel LogLevel { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime DateTime { get; set; }
    }
}
