using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Client
{
    public interface IDeviceMetadataService
    {
        Task<string> GetDeviceMetadataAsync(int deviceId);

        Task UpdateDeviceMetadataAsync(int deviceId, MetadataModel metadata);

        Task UpdateCamerasMetadataAsync(int deviceId, IEnumerable<CameraMetadataModel> metadata);
    }
}