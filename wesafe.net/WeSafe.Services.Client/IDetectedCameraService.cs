using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;

namespace WeSafe.Services.Client
{
    public interface IDetectedCameraService
    {
        Task<IReadOnlyCollection<DetectedCameraModel>> GetDetectedCamerasAsync(int deviceId);

        Task<(IQueryable<DetectedCameraModel> Query, int Total)> GetDetectedCamerasAsync(int deviceId,
            PageRequest request);

        Task<DetectedCameraModel> GetDetectedCameraByIdAsync(int id);

        Task CreateDetectedCameraAsync(string macAddress, CreateDetectedCameraModel model);

        Task ConnectingDetectedCameraAsync(ConnectingDetectedCameraModel model);

        Task ConnectDetectedCameraAsync(ConnectDetectingCameraModel model);

        Task FailureDetectedCameraAsync(FailureDetectingCameraModel model);

        Task RemoveDetectedCameraAsync(int id);
    }
}