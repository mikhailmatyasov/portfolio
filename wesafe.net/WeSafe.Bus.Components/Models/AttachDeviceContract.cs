using WeSafe.Bus.Contracts.Register;

namespace WeSafe.Bus.Components.Models
{
    public class AttachDeviceContract : IAttachDeviceContract
    {
        public int ClientId { get; set; }

        public string DeviceToken { get; set; }
    }
}
