using MediatR;
using System.Collections.Generic;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Commands.Camera
{
    public class GetCameraNamesByIdCommand : IRequest<IEnumerable<CameraModelResponse>>
    {
        public IEnumerable<int> Ids { get; set; }
    }
}
