using System;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using WeSafe.Bus.Components.Models;
using WeSafe.Bus.Contracts.Register;
using WeSafe.Dashboard.WebApi.Commands.Devices;

namespace WeSafe.Dashboard.WebApi.Consumers
{
    public class UpdateDeviceTypeConsumer : IConsumer<IUpdateDeviceTypeContract>
    {
        #region Fields

        private readonly IMediator _mediator;

        #endregion

        #region Constructors

        public UpdateDeviceTypeConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #endregion

        public async Task Consume(ConsumeContext<IUpdateDeviceTypeContract> context)
        {
            var device = await _mediator.Send(new GetDeviceByTokenCommand
            {
                DeviceToken = context.Message.DeviceToken
            });

            await _mediator.Send(new UpdateDeviceTypeCommand
            {
                DeviceId = device.Id,
                DeviceType = context.Message.DeviceType
            });

            await context.Publish<IDeviceTypeUpdatedContract>(new DeviceTypeUpdatedContract()
            {
                DeviceToken = context.Message.DeviceToken
            });
        }
    }
}