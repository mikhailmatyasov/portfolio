using Automatonymous;
using MassTransit;
using WeSafe.Bus.Contracts.Register;
using WeSafe.Bus.Contracts.User;

namespace WeSafe.Bus.Components.StateMachines
{
    public partial class RegisterStateMachine
    {
        public Event<IRegisterContract> RegisterStarted { get; private set; }

        public Event<IDeviceOwnerCreatedContract> DeviceOwnerCreated { get; private set; }

        public Event<IUserCreatedContract> UserCreated { get; private set; }

        public Event<IDeviceAttachedContract> DeviceAttached { get; set; }

        public Event<IDeviceTypeUpdatedContract> DeviceTypeUpdated { get; set; }

        public State DeviceOwnerCreatedState { get; private set; }

        public State RegisterStartedState { get; private set; }

        public State UserCreatedState { get; private set; }

        public State DeviceAttachedState { get; private set; }

        private void BuildStateMachine()
        {
            Event(() => RegisterStarted,
                x => x.CorrelateBy((state, context) => context.Message.DeviceToken == state.DeviceToken)
                    .SelectId(f => NewId.NextGuid()));

            Event(() => DeviceOwnerCreated,
                x => x.CorrelateBy((state, context) => context.Message.DeviceToken == state.DeviceToken));

            Event(() => UserCreated,
                x => x.CorrelateBy((state, context) => context.Message.DeviceToken == state.DeviceToken));

            Event(() => DeviceAttached,
                x => x.CorrelateBy((state, context) => context.Message.DeviceToken == state.DeviceToken));

            Event(() => DeviceTypeUpdated,
                x => x.CorrelateBy((state, context) => context.Message.DeviceToken == state.DeviceToken));

            InstanceState(x => x.State);

            SetCompletedWhenFinalized();
        }
    }
}
