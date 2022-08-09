using WeSafe.Bus.Contracts.Register;
using WeSafe.Bus.Contracts.User;

namespace WeSafe.Bus.Components.Models
{
    public class UserCreatedContract : IUserCreatedContract
    {
        public string DeviceToken { get; set; }

        public int DeviceOwnerId { get; set; }
    }
}
