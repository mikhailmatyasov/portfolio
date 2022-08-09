using System;
using System.Collections.Generic;

namespace WeSafe.DAL.Entities
{
    public class Client
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string ContractNumber { get; set; }

        public string Token { get; set; }

        public string Info { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public ICollection<ClientSubscriber> Subscribers { get; set; }

        public bool SendToDevChat { get; set; }

        public ICollection<User> Users { get; set; }
    }
}