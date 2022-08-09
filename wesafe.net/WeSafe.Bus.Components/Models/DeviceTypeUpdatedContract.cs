using WeSafe.Bus.Contracts.Register;

namespace WeSafe.Bus.Components.Models
{
    public class DeviceTypeUpdatedContract : IDeviceTypeUpdatedContract
    {
        public string DeviceToken { get; set; }
    }
}