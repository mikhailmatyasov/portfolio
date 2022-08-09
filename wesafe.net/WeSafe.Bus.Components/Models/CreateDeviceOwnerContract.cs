using System;
using WeSafe.Bus.Contracts.Register;

namespace WeSafe.Bus.Components.Models
{
    public class CreateDeviceOwnerContract : ICreateDeviceOwnerContract
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string ContractNumber { get; set; }

        public string Token { get; set; }

        public string Info { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public bool SendToDevChat { get; set; }
    }
}
