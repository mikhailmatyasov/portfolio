using WeSafe.Bus.Contracts.Register;
using WeSafe.Shared.Enumerations;

namespace WeSafe.Bus.Components.Models
{
    public class UpdateDeviceTypeContract : IUpdateDeviceTypeContract
    {
        public string DeviceToken { get; set; }

        public DeviceType DeviceType { get; set; }
    }
}