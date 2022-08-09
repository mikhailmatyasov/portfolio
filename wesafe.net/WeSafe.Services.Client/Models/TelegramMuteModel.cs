using System;

namespace WeSafe.Services.Client.Models
{
    public class TelegramMuteModel
    {
        public long TelegramId { get; set; }

        public DateTimeOffset? Mute { get; set; }
    }
}