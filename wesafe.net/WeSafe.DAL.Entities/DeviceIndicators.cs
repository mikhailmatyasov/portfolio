using System;

namespace WeSafe.DAL.Entities
{
    public class DeviceIndicators
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public virtual Device Device { get; set; }

        public double? CpuUtilization { get; set; }

        public double? GpuUtilization { get; set; }

        public double? MemoryUtilization { get; set; }

        public double? Temperature { get; set; }

        public string CamerasFps { get; set; }

        public double? Traffic { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}