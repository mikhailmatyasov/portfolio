using AutoMapper;
using MassTransit;
using MediatR;
using System;
using System.Threading.Tasks;
using WeSafe.Bus.Components.Models;
using WeSafe.Bus.Contracts.Register;
using WeSafe.Dashboard.WebApi.Commands.Clients;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Consumers
{
    public class CreateDeviceOwnerConsumer : IConsumer<ICreateDeviceOwnerContract>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public CreateDeviceOwnerConsumer(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<ICreateDeviceOwnerContract> context)
        {
            var deviceOwnerModel = _mapper.Map<ClientModel>(context.Message);

            var result = await _mediator.Send(new CreateClientCommand()
            {
                Client = deviceOwnerModel
            });

            await context.Publish<IDeviceOwnerCreatedContract>(new DeviceOwnerCreatedContract()
            {
                DeviceOwnerId = result,
                DeviceToken = context.Message.Token
            });
        }
    }
}
