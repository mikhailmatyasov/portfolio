using WeSafe.Shared.Enumerations;

namespace WeSafe.Bus.Contracts.Register
{
    public interface IUpdateDeviceTypeContract
    {
        string DeviceToken { get; set; }

        DeviceType DeviceType { get; set; }
    }
}