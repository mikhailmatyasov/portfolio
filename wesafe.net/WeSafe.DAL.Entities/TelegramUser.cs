using System;

namespace WeSafe.DAL.Entities
{
    public class TelegramUser
    {
        public int Id { get; set; }

        public string Phone { get; set; }

        public long TelegramId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Settings { get; set; }

        public string Status { get; set; }

        public DateTimeOffset? Mute { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}