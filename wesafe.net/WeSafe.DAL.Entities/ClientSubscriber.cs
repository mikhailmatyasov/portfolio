using System;
using System.Collections.Generic;

namespace WeSafe.DAL.Entities
{
    public class ClientSubscriber
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Permissions { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public ICollection<ClientSubscriberSettings> Settings { get; set; }

        public ICollection<ClientSubscriberAssignment> Assignments { get; set; }
    }
}