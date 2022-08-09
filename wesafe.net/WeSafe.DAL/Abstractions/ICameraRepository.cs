using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Repositories.Extended;

namespace WeSafe.DAL.Abstractions
{
    public interface ICameraRepository : IExtendedRepository<Camera>
    {
        Task<IEnumerable<Camera>> GetCamerasByDeviceIdAsync(int deviceId);

        Task<IEnumerable<Camera>> GetCamerasNames(IEnumerable<int> ids);
    }
}
