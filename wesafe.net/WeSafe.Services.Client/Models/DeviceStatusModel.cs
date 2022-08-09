using System.Collections.Generic;

namespace WeSafe.Services.Client.Models
{
    public class DeviceStatusModel
    {
        public int Id { get; set; }

        public string MACAddress { get; set; }

        public string Status { get; set; }

        public string NetworkStatus { get; set; }

        public bool IsArmed { get; set; }

        public string Name { get; set; }

        public IEnumerable<CameraStatusModel> Cameras { get; set; }
    }
}