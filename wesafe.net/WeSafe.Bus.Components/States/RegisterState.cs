using Automatonymous;
using MassTransit.Saga;
using System;
using WeSafe.Bus.Contracts.Register;
using WeSafe.Shared.Enumerations;

namespace WeSafe.Bus.Components.States
{
    public class RegisterState : SagaStateMachineInstance, ISagaVersion, IRegisterContract
    {
        public Guid CorrelationId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string DeviceToken { get; set; }

        public DeviceType DeviceType { get; set; }

        public string State { get; set; }

        public int Version { get; set; }
    }
}
