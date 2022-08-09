using System;

namespace WeSafe.Services.Client.Models
{
    public class ClientSubscriberModel : Model
    {
        public int ClientId { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Permissions { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}