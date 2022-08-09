namespace WeSafe.Bus.Contracts.Register
{
    public interface IAttachDeviceContract
    {
        int ClientId { get; set; }

        string DeviceToken { get; set; }
    }
}
