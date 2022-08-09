using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Logger.WebApi.Services.Abstract;

namespace WeSafe.Logger.WebApi.Services
{
    public class DeviceLogFilter : IDeviceLogFilter
    {
        public Task<IEnumerable<DeviceLogModel>> Filter(IEnumerable<DeviceLogModel> logs)
        {
            return Task.FromResult<IEnumerable<DeviceLogModel>>(logs
                .Where(l => l != null && !string.IsNullOrWhiteSpace(l.DeviceName)).ToList());
        }
    }
}
