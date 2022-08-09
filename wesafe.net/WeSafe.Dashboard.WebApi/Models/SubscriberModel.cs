using System;
using WeSafe.Dashboard.WebApi.Enumerations;

namespace WeSafe.Dashboard.WebApi.Models
{
    public class SubscriberModel
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string Phone { get; set; }

        public string Name { get; set; }

        public SubscriberPermission Permissions { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}