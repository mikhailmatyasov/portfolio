using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;

namespace WeSafe.Services.Client
{
    public interface IDeviceLogService
    {
        Task<PageResponse<ClientDeviceLogModel>> GetClientDeviceLogsAsync(ClientDeviceLogRecordQuery pageRequest);

        void InsertDevicesLogs(IEnumerable<DeviceLogModel> deviceLogModels);
    }
}
