namespace WeSafe.Bus.Contracts.User
{
    public interface ICreateUserValidationContract
    {
        string UserName { get; set; }

        string DisplayName { get; set; }

        string Phone { get; set; }

        string RoleName { get; set; }

        string Password { get; set; }
    }
}
