using System;

namespace WeSafe.Web.Core.Models
{
    public class ClientDeviceLogPresentationModel
    {
        public string ClientName { get; set; }

        public string DeviceName { get; set; }

        public string CameraName { get; set; }

        public string LogLevel { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime DateTime { get; set; }
    }
}
