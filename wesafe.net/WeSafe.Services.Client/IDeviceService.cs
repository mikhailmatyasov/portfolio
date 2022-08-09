using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using WeSafe.Shared.Enumerations;
using WeSafe.Shared.Results;

namespace WeSafe.Services.Client
{
    public interface IDeviceService
    {
        Task<IEnumerable<DeviceModel>> GetClientDevices(int clientId);

        Task<PageResponse<DeviceModel>> GetDevices(DeviceRequest request);

        Task<IEnumerable<DeviceShortModel>> GetAllDevices(bool activatedOnly, string status);

        Task<DeviceModel> GetDeviceById(int deviceId);

        Task<DeviceModel> GetDeviceByMAC(string mac);

        Task<DeviceModel> GetDeviceByToken(string token);

        Task<IExecutionResult> BindDeviceToClient(string token, int clientId, string timeZone);

        Task<IExecutionResult> UpdateDeviceName(int deviceId, string newDeviceName);

        Task<IExecutionResult> UpdateDeviceStatus(DeviceUpdateStatusModel model);

        Task<bool> UpdateDeviceNetworkStatus(DeviceStatusModel model);

        Task<IExecutionResult> CreateDevice(DeviceModel model);

        Task<IExecutionResult> UpdateDevice(DeviceModel model);

        Task<IExecutionResult> RemoveDevice(int deviceId);

        Task<IExecutionResult> DeactivateDevice(int deviceId);

        Task<IExecutionResult> ClearPreviousDeviceSshPassword(string mac);

        Task UpdateDevicesSshPassword(int expiredPeriod);

        Task<DeviceAuthToken> GetDeviceAuthToken(string mac);

        Task UpdateAuthToken(int deviceId, string token);

        Task ResetAuthToken(int deviceId);

        Task UpdateDeviceIpAddress(int deviceId, string ipAddress);

        Task<IExecutionResult> ChangeDeviceType(int deviceId, DeviceType deviceType);

        Task ChangeTimeZone(int deviceId, string timeZone);
    }
}