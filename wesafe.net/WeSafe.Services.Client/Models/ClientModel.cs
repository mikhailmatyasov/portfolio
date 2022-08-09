using System;
using WeSafe.Shared.Enumerations;

namespace WeSafe.Services.Client.Models
{
    public class ClientModel : Model
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string ContractNumber { get; set; }

        public string Token { get; set; }

        public string Info { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public bool SendToDevChat { get; set; }

        public DeviceType DeviceType { get; set; }
    }
}