using System;

namespace WeSafe.Services.Client.Models
{
    public class TelegramUserModel : Model
    {
        public string Phone { get; set; }

        public long TelegramId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Settings { get; set; }

        public string Status { get; set; }

        public DateTimeOffset? Mute { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}