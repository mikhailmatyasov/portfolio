using System;

namespace WeSafe.Services.Client.Models
{
    public class TrafficChartModel
    {
        public string Mark { get; set; }

        public int CameraId { get; set; }

        public int Entered { get; set; }

        public int Exited { get; set; }
    }
}