using System;

namespace WeSafe.Services.Client.Models
{
    public class TrafficHourlyChartRequest
    {
        public int DeviceId { get; set; }

        public int CameraId { get; set; }

        public DateTime Date { get; set; }
    }
}