using System;
using WeSafe.Shared.Enumerations;

namespace WeSafe.Services.Client.Models
{
    public class ClientDeviceLogModel
    {
        public string ClientName { get; set; }

        public string DeviceName { get; set; }

        public string CameraName { get; set; }

        public LogLevel LogLevel { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime DateTime { get; set; }
    }
}
