using AutoMapper;
using MassTransit;
using MediatR;
using System;
using System.Threading.Tasks;
using WeSafe.Bus.Components.Models;
using WeSafe.Bus.Contracts.Register;
using WeSafe.Dashboard.WebApi.Commands.Devices;

namespace WeSafe.Dashboard.WebApi.Consumers
{
    public class AttachDeviceConsumer : IConsumer<IAttachDeviceContract>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public AttachDeviceConsumer(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<IAttachDeviceContract> context)
        {
            var attachDeviceCommand = _mapper.Map<AttachDeviceCommand>(context.Message);

            await _mediator.Send(attachDeviceCommand);

            await context.Publish<IDeviceAttachedContract>(new DeviceAttachedContract()
            {
                DeviceToken = attachDeviceCommand.DeviceToken
            });
        }
    }
}
