using AutoMapper;
using MassTransit;
using MediatR;
using System;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Commands.Users;
using WeSafe.Bus.Components.Models;
using WeSafe.Bus.Contracts.Register;
using WeSafe.Bus.Contracts.User;

namespace WeSafe.Authentication.WebApi.Consumers
{
    public class CreateUserConsumer : IConsumer<ICreateUserContract>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public CreateUserConsumer(IMediator mediator, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<ICreateUserContract> context)
        {
            var userCommand = _mapper.Map<CreateUserCommand>(context.Message);

            await _mediator.Send(userCommand);

            await context.Publish<IUserCreatedContract>(new UserCreatedContract()
            {
                DeviceToken = context.Message.DeviceToken,
                DeviceOwnerId = context.Message.ClientId ?? -1
            });
        }
    }
}
