using Newtonsoft.Json;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Core.Models
{
    public class CameraSettingsModel
    {
        public int Id { get; set; }

        public string Password { get; set; }

        public string Login { get; set; }

        public string Port { get; set; }

        public string Ip { get; set; }

        public object Roi { get; set; }

        [JsonProperty("roi_v2")]
        public object RoiV2 { get; set; }

        public string Rtsp { get; set; }

        public bool Active { get; set; }

        public RecognitionSettings Settings { get; set; }

        public string Metadata { get; set; }
    }
}