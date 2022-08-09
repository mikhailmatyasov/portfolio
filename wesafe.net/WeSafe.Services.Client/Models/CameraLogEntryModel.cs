using Newtonsoft.Json;
using System;

namespace WeSafe.Services.Client.Models
{
    public class CameraLogEntryModel : Model
    {
        public int CameraLogId { get; set; }

        public string TypeKey { get; set; }

        public string ImageUrl { get; set; }

        [JsonIgnore]
        public DateTimeOffset? UrlExpiration { get; set; }
    }
}