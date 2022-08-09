using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Logger.Abstraction.Services;
using WeSafe.Logger.WebApi.Services.Abstract;

namespace WeSafe.Logger.WebApi.Services
{
    public class DeviceNameLogMapper : IDeviceLogMapper
    {
        private readonly IDeviceService _deviceService;

        public DeviceNameLogMapper(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        public async Task Map(IEnumerable<DeviceLogModel> logs)
        {
            if (logs == null)
                throw new ArgumentNullException(nameof(logs));

            if(!logs.Any())
                return;

            var filteredLogs = logs.Where(l => l != null).ToList();

            var devicesNames = await _deviceService.GetDevicesNames(new DevicesNamesRequest()
            {
                Ids = filteredLogs.Select(l => l.DeviceId).Distinct().ToList()
            });

            foreach (var deviceLogModel in filteredLogs)
            {
                FillDeviceName(deviceLogModel, devicesNames);
            }
        }

        private void FillDeviceName(DeviceLogModel log, IEnumerable<DeviceName> devicesNames)
        {
            var deviceName = devicesNames.FirstOrDefault(n => n.Id == log.DeviceId);
            if (deviceName == null)
                return;

            log.DeviceName = deviceName.Name;
        }
    }
}
