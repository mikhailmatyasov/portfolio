using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Models;

namespace WeSafe.Logger.WebApi.Services.Abstract
{
    public interface IDeviceLogMapper
    {
        Task Map(IEnumerable<DeviceLogModel> logs);
    }
}
