using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WeSafe.Event.WebApi.Models
{
    public class EventsBaseRequestModel
    {
        [JsonProperty("mac")]
        public string DeviceMacAddress { get; set; }

        [JsonProperty("ip_cam")]
        public string CameraIp { get; set; }

        public int CameraId { get; set; }

        public string Alert { get; set; }

        public string Message { get; set; }
    }
}
