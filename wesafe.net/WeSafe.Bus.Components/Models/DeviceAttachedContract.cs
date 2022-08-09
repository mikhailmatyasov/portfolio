using WeSafe.Bus.Contracts.Register;

namespace WeSafe.Bus.Components.Models
{
    public class DeviceAttachedContract : IDeviceAttachedContract
    {
        public string DeviceToken { get; set; }
    }
}
