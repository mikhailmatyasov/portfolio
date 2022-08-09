namespace WeSafe.Bus.Contracts.Register
{
    public interface IDeviceOwnerCreatedContract
    {
        int DeviceOwnerId { get; set; }

        string DeviceToken { get; set; }
    }
}
