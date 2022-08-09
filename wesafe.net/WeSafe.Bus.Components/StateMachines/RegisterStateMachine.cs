using Automatonymous;
using Automatonymous.Binders;
using MassTransit;
using WeSafe.Bus.Components.Models;
using WeSafe.Bus.Components.States;
using WeSafe.Bus.Contracts.Register;
using WeSafe.Bus.Contracts.User;
using WeSafe.Shared.Roles;

namespace WeSafe.Bus.Components.StateMachines
{
    public partial class RegisterStateMachine : MassTransitStateMachine<RegisterState>
    {
        public RegisterStateMachine()
        {
            BuildStateMachine();

            Initially(Initialize());

            During(RegisterStartedState, WhenDeviceOwnerCreated());

            During(DeviceOwnerCreatedState, WhenUserCreated());

            During(UserCreatedState, WhenDeviceAttached());

            During(DeviceAttachedState, WhenDeviceTypeUpdated());
        }

        private EventActivityBinder<RegisterState, IDeviceTypeUpdatedContract> WhenDeviceTypeUpdated()
        {
            return When(DeviceTypeUpdated)
                .Finalize();
        }

        private EventActivityBinder<RegisterState, IDeviceAttachedContract> WhenDeviceAttached()
        {
            return When(DeviceAttached)
                   .PublishAsync(context =>
                       context.Init<IUpdateDeviceTypeContract>(new UpdateDeviceTypeContract()
                       {
                           DeviceToken = context.Instance.DeviceToken,
                           DeviceType = context.Instance.DeviceType
                       })
                   )
                   .TransitionTo(DeviceAttachedState);
        }

        private EventActivityBinder<RegisterState, IUserCreatedContract> WhenUserCreated()
        {
            return When(UserCreated).PublishAsync(context =>
                context.Init<IAttachDeviceContract>(new AttachDeviceContract()
                {
                    DeviceToken = context.Instance.DeviceToken,
                    ClientId = context.Data.DeviceOwnerId
                })
            ).TransitionTo(UserCreatedState);
        }

        private EventActivityBinder<RegisterState, IDeviceOwnerCreatedContract> WhenDeviceOwnerCreated()
        {
            return When(DeviceOwnerCreated).PublishAsync(context =>
                context.Init<ICreateUserContract>(new CreateUserContract()
                {
                    UserName = context.Instance.UserName,
                    Password = context.Instance.Password,
                    ClientId = context.Data.DeviceOwnerId,
                    DisplayName = context.Instance.Name,
                    IsActive = true,
                    Phone = context.Instance.Phone,
                    RoleName = UserRoles.Users,
                    DeviceToken = context.Instance.DeviceToken
                })).TransitionTo(DeviceOwnerCreatedState);
        }

        private EventActivityBinder<RegisterState, IRegisterContract> Initialize()
        {
            return When(RegisterStarted)
                .Then(context =>
                {
                    context.Instance.DeviceToken = context.Data.DeviceToken;
                    context.Instance.DeviceType = context.Data.DeviceType;
                    context.Instance.Password = context.Data.Password;
                    context.Instance.UserName = context.Data.UserName;
                    context.Instance.Phone = context.Data.Phone;
                    context.Instance.Name = context.Data.Name;
                }).PublishAsync(context => context.Init<ICreateDeviceOwnerContract>(new CreateDeviceOwnerContract()
                {
                    Name = context.Instance.Name,
                    Phone = context.Instance.Phone,
                    Token = context.Instance.DeviceToken,
                    IsActive = true,
                    SendToDevChat = false
                }))
                .TransitionTo(RegisterStartedState);
        }
    }
}
