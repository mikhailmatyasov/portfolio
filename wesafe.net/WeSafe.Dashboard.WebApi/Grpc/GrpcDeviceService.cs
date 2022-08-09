using Grpc.Core;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Proto;

namespace WeSafe.Dashboard.WebApi.Grpc
{
    public class GrpcDeviceService : DeviceGrpc.DeviceGrpcBase
    {
        private IMediator _mediator;

        public GrpcDeviceService(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override async Task<DeviceNamesReponse> GetDevicesNames(DeviceRequest request, ServerCallContext context)
        {
            var devicesNames = await _mediator.Send(new GetDevicesNamesByIdCommand()
            {
                Ids = request.DeviceIds
            });

            var response = new DeviceNamesReponse();
            response.DevicesNames.Add(devicesNames.Select(d => new DeviceName()
            {
                Id = d.Id,
                Name = d.Name
            }).ToList());

            return response;
        }
    }
}
