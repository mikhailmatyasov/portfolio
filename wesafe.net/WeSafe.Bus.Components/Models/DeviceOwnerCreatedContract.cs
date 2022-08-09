using WeSafe.Bus.Contracts.Register;

namespace WeSafe.Bus.Components.Models
{
    public class DeviceOwnerCreatedContract : IDeviceOwnerCreatedContract
    {
        public int DeviceOwnerId { get; set; }

        public string DeviceToken { get; set; }
    }
}
