using System;

namespace WeSafe.DAL.Entities
{
    public class TrafficEvent
    {
        public int Id { get; set; }

        public string DeviceMAC { get; set; }

        public int CameraId { get; set; }

        public virtual Camera Camera { get; set; }

        public DateTime UtcDateTime { get; set; }

        public Direction Direction { get; set; }

        public string ObjectId { get; set; }
    }

    public enum Direction
    {
        In, Out
    }
}
