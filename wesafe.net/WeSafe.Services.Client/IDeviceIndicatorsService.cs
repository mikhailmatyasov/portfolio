using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Client
{
    public interface IDeviceIndicatorsService
    {
        Task<DateTimeOffset?> GetLastIndicatorsTime(int deviceId);

        Task<IEnumerable<DeviceIndicatorsModel>> GetDeviceIndicators(int deviceId, DateTimeOffset? from, DateTimeOffset? to);

        Task UpdateDeviceIndicators(int deviceId, IDeviceIndicators indicators);
    }
}