using System;
using WeSafe.Authentication.WebApi.Enumerations;

namespace WeSafe.Authentication.WebApi.Models
{
    public class MobileUserModel
    {
        public int Id { get; set; }

        public string Phone { get; set; }

        public MobileUserStatus Status { get; set; }

        public DateTimeOffset? Mute { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}