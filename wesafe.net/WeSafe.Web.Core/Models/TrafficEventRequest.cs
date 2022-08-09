using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WeSafe.Web.Core.Models
{
    public class TrafficEventRequest
    {
        [Required]
        [JsonProperty("mac")]
        public string DeviceMAC { get; set; }

        [Required]
        [JsonProperty("ip_cam")]
        public string CameraIp { get; set; }

        [Required]
        [JsonProperty("utc_datetime")]
        public DateTime UtcDateTime { get; set; }

        [Required]
        [JsonProperty("direction")]
        public Direction Direction { get; set; }

        [JsonProperty("id")]
        public string ObjectId { get; set; }
    }

    public enum Direction
    {
        In, Out
    }
}
