using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Core.Models
{
    public class DeviceSettingsModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("api_version")]
        public string ApiVersion { get; set; }

        [JsonProperty("sw_version")]
        public string SwVersion { get; set; }

        [JsonProperty("hw_version")]
        public string HwVersion { get; set; }

        [JsonProperty("ssh_password")]
        public string SshPassword { get; set; }

        public Dictionary<string, CameraSettingsModel> Cameras { get; set; }

        public IEnumerable<DetectedCameraModel> DetectedCameras { get; set; }

        public DateTimeOffset? LastIndicatorsTime { get; set; }

        public string Metadata { get; set; }
    }
}