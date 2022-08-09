using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Logger.Abstraction.Services;
using WeSafe.Logger.WebApi.Services.Abstract;

namespace WeSafe.Logger.WebApi.Services
{
    public class CameraNameLogMapper : IDeviceLogMapper
    {
        private readonly ICameraService _cameraService;

        public CameraNameLogMapper(ICameraService cameraService)
        {
            _cameraService = cameraService;
        }

        public async Task Map(IEnumerable<DeviceLogModel> logs)
        {
            if (logs == null)
            {
                throw new ArgumentNullException(nameof(logs));
            }

            if (!logs.Any())
            {
                return;
            }

            var filteredLogs = logs.Where(l => l != null).ToList();

            var camerasNames = await _cameraService.GetCamerasNames(new CamerasNamesRequest()
            {
                Ids = filteredLogs.Where(l => l?.CameraId != null).Select(l => (int) l.CameraId).Distinct().ToList()
            });

            foreach (var deviceLogModel in filteredLogs)
            {
                FillCameraName(deviceLogModel, camerasNames);
            }
        }

        private void FillCameraName(DeviceLogModel log, IEnumerable<CameraName> camerasNames)
        {
            if (log.CameraId == null)
                return;

            var cameraName = camerasNames.FirstOrDefault(n => n.Id == (int)log.CameraId);
            if (cameraName == null)
                return;

            log.CameraName = cameraName.Name;
        }
    }
}
