using System;

namespace WeSafe.Services.Client.Models
{
    public class TelegramOptions
    {
        public string Token { get; set; }

        public Int64 DevChatId { get; set; }

        public Int64 StatChatId { get; set; }

        public bool SendToDevChat { get; set; }
    }
}