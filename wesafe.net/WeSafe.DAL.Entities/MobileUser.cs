using System;
using System.Collections.Generic;

namespace WeSafe.DAL.Entities
{
    public class MobileUser
    {
        public int Id { get; set; }

        public string Phone { get; set; }

        public string Status { get; set; }

        public DateTimeOffset? Mute { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public ICollection<MobileDevice> Devices { get; set; }
    }
}