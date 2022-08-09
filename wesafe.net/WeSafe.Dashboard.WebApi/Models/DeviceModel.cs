using System;
using WeSafe.Shared.Enumerations;

namespace WeSafe.Dashboard.WebApi.Models
{
    public class DeviceModel
    {
        public int Id { get; set; }

        public string MACAddress { get; set; }

        public int? ClientId { get; set; }

        public string ClientName { get; set; }

        public string Status { get; set; }

        public string NetworkStatus { get; set; }

        public DeviceType DeviceType { get; set; }

        public string SerialNumber { get; set; }

        public string HWVersion { get; set; }

        public string SWVersion { get; set; }

        public string NVIDIASn { get; set; }

        public string ClientNetworkIp { get; set; }

        public DateTimeOffset? ActivationDate { get; set; }

        public DateTimeOffset? AssemblingDate { get; set; }

        public string CreatedBy { get; set; }

        public string Info { get; set; }

        public string Token { get; set; }

        public bool IsArmed { get; set; }

        public int CamerasNumber { get; set; }

        public string Name { get; set; }

        public string CurrentSshPassword { get; set; }

        public int? MaxActiveCameras { get; set; }
    }
}