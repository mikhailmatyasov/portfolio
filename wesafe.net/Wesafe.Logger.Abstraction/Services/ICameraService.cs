using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Models;

namespace WeSafe.Logger.Abstraction.Services
{
    public interface ICameraService
    {
        Task<IEnumerable<CameraName>> GetCamerasNames(CamerasNamesRequest request);
    }
}
