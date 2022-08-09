using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Dashboard.WebApi.Proto;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Logger.Abstraction.Services;
using DeviceName = WeSafe.Logger.Abstraction.Models.DeviceName;

namespace WeSafe.Logger.WebApi.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly DeviceGrpc.DeviceGrpcClient _client;

        public DeviceService(DeviceGrpc.DeviceGrpcClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<DeviceName>> GetDevicesNames(DevicesNamesRequest request)
        {
            var deviceRequest = new DeviceRequest();
            deviceRequest.DeviceIds.AddRange(request.Ids);

            var devicesNames = await _client.GetDevicesNamesAsync(deviceRequest);

            return devicesNames.DevicesNames.Select(d => new DeviceName()
            {
                Id = d.Id,
                Name = d.Name
            }).ToList();
        }
    }
}
