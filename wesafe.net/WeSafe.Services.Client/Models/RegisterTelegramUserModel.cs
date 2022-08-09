namespace WeSafe.Services.Client.Models
{
    public class RegisterTelegramUserModel
    {
        public string Phone { get; set; }

        public long TelegramId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}