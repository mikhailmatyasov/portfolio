using System;
using WeSafe.Logger.Abstraction.Enums;
using WeSafe.Logger.Abstraction.Interfaces;

namespace WeSafe.Logger.Abstraction.Models
{
    public class DeviceLogModel : IWeSafeLog
    {
        public int DeviceId { get; set; }

        public string DeviceName { get; set; }

        public int? CameraId { get; set; }

        public string CameraName { get; set; }

        public string Message { get; set; }

        public DateTime DateTime { get; set; }

        public WeSafeLogLevel LogLevel { get; set; }
    }
}
