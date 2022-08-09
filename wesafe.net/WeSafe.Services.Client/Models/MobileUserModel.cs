using System;

namespace WeSafe.Services.Client.Models
{
    public class MobileUserModel : Model
    {
        public string Phone { get; set; }

        public string Status { get; set; }

        public DateTimeOffset? Mute { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}