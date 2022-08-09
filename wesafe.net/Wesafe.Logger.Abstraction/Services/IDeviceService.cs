using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Models;

namespace WeSafe.Logger.Abstraction.Services
{
    public interface IDeviceService
    {
        Task<IEnumerable<DeviceName>> GetDevicesNames(DevicesNamesRequest request);
    }
}
