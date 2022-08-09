using WeSafe.Bus.Contracts.User;

namespace WeSafe.Bus.Components.Models.User
{
    public class CreateUserValidationContract : ICreateUserValidationContract
    {
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Phone { get; set; }

        public string RoleName { get; set; }

        public string Password { get; set; }
    }
}
