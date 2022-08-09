using System;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;

namespace WeSafe.Services.Client
{
    public interface ICameraLogService
    {
        Task<int> CreateCameraLog(CameraLogModel model);

        Task<PageResponse<CameraLogModel>> GetEvents(EventRequest request);
    }
}