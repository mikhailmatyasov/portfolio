namespace WeSafe.Bus.Contracts.User
{
    public interface IUserCreatedContract
    {
        string DeviceToken { get; set; }

        int DeviceOwnerId { get; set; }
    }
}
