using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Models;

namespace WeSafe.Logger.WebApi.Services.Abstract
{
    public interface IDeviceLogFilter
    {
        Task<IEnumerable<DeviceLogModel>> Filter(IEnumerable<DeviceLogModel> logs);
    }
}
