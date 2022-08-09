using System;
using System.Text.Json.Serialization;
using WeSafe.Logger.Abstraction.Enums;

namespace WeSafe.Logger.WebApi.Models
{
    public class DeviceLogModelRequest
    {
        public int DeviceId { get; set; }

        public string DeviceName { get; set; }

        public int? CameraId { get; set; }

        public string CameraName { get; set; }

        [JsonPropertyName("ErrorMessage")]
        public string Message { get; set; }

        public DateTime DateTime { get; set; }

        public WeSafeLogLevel LogLevel { get; set; }
    }
}
