namespace WeSafe.Bus.Contracts.Register
{
    public interface ICreateUserContract
    {
        string UserName { get; set; }

        string Phone { get; set; }

        string Email { get; set; }

        string DisplayName { get; set; }

        bool IsActive { get; set; }

        string RoleName { get; set; }

        string Password { get; set; }

        int? ClientId { get; set; }

        string DeviceToken { get; set; }
    }
}
