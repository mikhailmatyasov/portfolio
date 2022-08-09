using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;

namespace WeSafe.Monitoring.Services
{
    public interface IApiClient
    {
        Task<IEnumerable<DeviceShortModel>> GetDevices(bool activatedOnly, string status, CancellationToken cancellationToken = default);

        Task<IEnumerable<CameraMonitoringModel>> GetDeviceCameras(int deviceId, DateTimeOffset? timeMark = null, bool activeOnly = false, CancellationToken cancellationToken = default);

        Task UpdateDeviceStatus(DeviceUpdateStatusModel model, CancellationToken cancellationToken = default);

        Task UpdateCameraStatus(CameraUpdateStatusModel model, CancellationToken cancellationToken = default);

        Task StatusChanged(DeviceUpdateStatusModel model, CancellationToken cancellationToken = default);
    }
}