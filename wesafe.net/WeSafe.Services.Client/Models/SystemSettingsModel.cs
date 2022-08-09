using System.Collections.Generic;

namespace WeSafe.Services.Client.Models
{
    public class SystemSettingsModel
    {
        public IEnumerable<DeviceSettingsModel> Devices { get; set; }
    }

    public class ClientSystemSettingsModel : SystemSettingsModel
    {
        public bool Mute { get; set; }
    }
}