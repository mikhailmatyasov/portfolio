using WeSafe.Shared.Enumerations;

namespace WeSafe.Bus.Contracts.Register
{
    public interface IRegisterContract
    {
        string UserName { get; set; }

        string Password { get; set; }

        string Name { get; set; }

        string Phone { get; set; }

        string DeviceToken { get; set; }

        DeviceType DeviceType { get; set; }
    }
}
