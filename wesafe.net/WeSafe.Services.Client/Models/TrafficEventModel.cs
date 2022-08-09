using System;

namespace WeSafe.Services.Client.Models
{
    public class TrafficEventModel
    {
        public string DeviceMAC { get; set; }

        public string CameraIp { get; set; }

        public DateTime UtcDateTime { get; set; }

        public Direction Direction { get; set; }

        public string ObjectId { get; set; }
    }

    public enum Direction
    {
        In, Out
    }
}
