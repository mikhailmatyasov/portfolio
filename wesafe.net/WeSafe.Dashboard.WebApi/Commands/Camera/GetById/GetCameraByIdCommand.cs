using MediatR;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Commands.Camera
{
    public class GetCameraByIdCommand : IRequest<CameraModelResponse>
    {
        public int Id { get; }

        public GetCameraByIdCommand(int id)
        {
            Id = id;
        }
    }
}
