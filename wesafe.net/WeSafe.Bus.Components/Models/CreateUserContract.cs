using WeSafe.Bus.Contracts.Register;

namespace WeSafe.Bus.Components.Models
{
    public class CreateUserContract : ICreateUserContract
    {
        public string UserName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public bool IsActive { get; set; }

        public string RoleName { get; set; }

        public string Password { get; set; }

        public int? ClientId { get; set; }

        public string DeviceToken { get; set; }
    }
}
