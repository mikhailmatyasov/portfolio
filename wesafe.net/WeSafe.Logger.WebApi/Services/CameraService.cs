using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Dashboard.WebApi.Proto;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Logger.Abstraction.Services;
using CameraName = WeSafe.Logger.Abstraction.Models.CameraName;

namespace WeSafe.Logger.WebApi.Services
{
    public class CameraService : ICameraService
    {
        private readonly CameraGrpc.CameraGrpcClient _client;

        public CameraService(CameraGrpc.CameraGrpcClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<CameraName>> GetCamerasNames(CamerasNamesRequest request)
        {
            var cameraRequest = new CameraRequest();
            cameraRequest.CameraIds.AddRange(request.Ids);
            var devicesNames = await _client.GetCamerasNamesAsync(cameraRequest);

            return devicesNames.CamerasNames.Select(d => new CameraName()
            {
                Id = d.Id,
                Name = d.Name
            }).ToList();
        }
    }
}
