using System;
using System.Collections.Generic;

namespace WeSafe.Services.Client.Models
{
    public class DeviceIndicatorsModel : Model, IDeviceIndicators
    {
        public double? CpuUtilization { get; set; }

        public double? GpuUtilization { get; set; }

        public double? MemoryUtilization { get; set; }

        public double? Temperature { get; set; }

        public Dictionary<int, double> CamerasFps { get; set; }

        public double? Traffic { get; set; }

        public DateTimeOffset? Time { get; set; }
    }
}