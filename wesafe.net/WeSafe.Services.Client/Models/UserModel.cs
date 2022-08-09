namespace WeSafe.Services.Client.Models
{
    public class UserModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public bool IsActive { get; set; }

        public string RoleName { get; set; }

        public bool IsLocked { get; set; }
    }

    public class UpsertUserModel : UserModel
    {
        public string Password { get; set; }

        public int? ClientId { get; set; }
    }
}