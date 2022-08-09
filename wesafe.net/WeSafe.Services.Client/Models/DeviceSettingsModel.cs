using System.Collections.Generic;

namespace WeSafe.Services.Client.Models
{
    public class DeviceSettingsModel
    {
        public int Id { get; set; }

        public string MACAddress { get; set; }

        public string LocalIp { get; set; }

        public string Status { get; set; }

        public string NetworkStatus { get; set; }

        public bool IsArmed { get; set; }

        public string Name { get; set; }

        public IEnumerable<SettingsBaseModel> Cameras { get; set; }
    }

    public class SettingsBaseModel
    {
        public int CameraId { get; set; }

        public string CameraName { get; set; }

        public string Status { get; set; }

        public string NetworkStatus { get; set; }

        public bool IsActive { get; set; }
    }

    public class SettingsModel : SettingsBaseModel
    {
        public bool Mute { get; set; }
    }

    public class SettingsModelEx : SettingsBaseModel
    {
        public string CameraIp { get; set; }

        public string Rtsp { get; set; }
    }
}