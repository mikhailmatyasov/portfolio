using System;

namespace WeSafe.Bus.Contracts.Register
{
    public interface ICreateDeviceOwnerContract
    {
        int Id { get; set; }

        string Name { get; set; }

        string Phone { get; set; }

        string ContractNumber { get; set; }

        string Token { get; set; }

        string Info { get; set; }

        DateTimeOffset CreatedAt { get; set; }

        bool IsActive { get; set; }

        bool SendToDevChat { get; set; }
    }
}
