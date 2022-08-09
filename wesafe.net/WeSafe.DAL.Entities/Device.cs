using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WeSafe.Shared.Enumerations;

namespace WeSafe.DAL.Entities
{
    public class Device
    {
        public int Id { get; set; }

        public DeviceType DeviceType { get; set; }

        public string SerialNumber { get; set; }

        public string HWVersion { get; set; }

        public string SWVersion { get; set; }

        public string NVIDIASn { get; set; }

        public string MACAddress { get; set; }

        public int? ClientId { get; set; }

        public virtual Client Client { get; set; }

        public string ClientNetworkIp { get; set; }

        public DateTimeOffset? ActivationDate { get; set; }

        public DateTimeOffset? AssemblingDate { get; set; }

        public string CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public User User { get; set; }

        public string Info { get; set; }

        public string Token { get; set; }

        public string Status { get; set; }

        public string NetworkStatus { get; set; }

        public ICollection<Camera> Cameras { get; set; }

        public bool IsArmed { get; set; }

        public string Name { get; set; }

        public string CurrentSshPassword { get; set; }

        public string PreviousSshPassword { get; set; }

        public DateTime LastUpdateDatePassword { get; set; }

        public ICollection<DeviceLog> DeviceLogs { get; set; }

        public string AuthToken { get; set; }

        public int? MaxActiveCameras { get; set; }

        public string TimeZone { get; set; }

        public ICollection<ClientSubscriberAssignment> Assignments { get; set; }

        public ICollection<DeviceIndicators> Indicators { get; set; }

        public ICollection<DetectedCamera> DetectedCameras { get; set; }

        public string Metadata { get; set; }
    }
}