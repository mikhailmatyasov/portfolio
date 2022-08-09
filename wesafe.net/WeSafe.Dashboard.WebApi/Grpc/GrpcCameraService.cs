using Grpc.Core;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Dashboard.WebApi.Commands.Camera;
using WeSafe.Dashboard.WebApi.Proto;
using static WeSafe.Dashboard.WebApi.Proto.CameraGrpc;

namespace WeSafe.Dashboard.WebApi.Grpc
{
    public class GrpcCameraService : CameraGrpcBase
    {
        private IMediator _mediator;

        public GrpcCameraService(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override async Task<CameraNamesReponse> GetCamerasNames(CameraRequest request, ServerCallContext context)
        {
            var devicesNames = await _mediator.Send(new GetCameraNamesByIdCommand()
            {
                Ids = request.CameraIds
            });

            var response = new CameraNamesReponse();
            response.CamerasNames.Add(devicesNames.Select(d => new CameraName()
            {
                Id = d.Id,
                Name = d.CameraName
            }).ToList());

            return response;
        }
    }
}
