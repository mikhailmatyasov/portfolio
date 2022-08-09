using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeZoneConverter;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;
using WeSafe.Shared;
using WeSafe.Shared.Enumerations;
using WeSafe.Shared.Extensions;
using WeSafe.Shared.Results;
using WeSafe.Shared.Services;

namespace WeSafe.Services
{
    public class DeviceService : BaseService, IDeviceService
    {
        private readonly DeviceMapper _mapper;
        private ITelegramClient _telegramClient;

        public DeviceService(WeSafeDbContext context, DeviceMapper mapper, ILoggerFactory loggerFactory, ITelegramClient telegramClient) : base(context, loggerFactory)
        {
            _mapper = mapper;
            _telegramClient = telegramClient;
        }

        public async Task<IEnumerable<DeviceModel>> GetClientDevices(int clientId)
        {
            var result = await DbContext.Devices
                                        .Where(c => c.ClientId == clientId)
                                        .Select(_mapper.Projection)
                                        .ToListAsync();

            return result;
        }

        public async Task<PageResponse<DeviceModel>> GetDevices(DeviceRequest request)
        {
            var query = DbContext.Devices.Select(_mapper.Projection);

            if (request.ClientId != null) query = query.Where(c => c.ClientId == request.ClientId);

            if (!String.IsNullOrWhiteSpace(request.SearchText))
            {
                query = FilterRequest (request, query);
            }

            var result = await query.OrderBy(c => c.Id).ApplyPageRequest(request);

            return new PageResponse<DeviceModel>
            {
                Items = await result.Query.ToListAsync(),
                Total = result.Total
            };
        }

        public async Task<IEnumerable<DeviceShortModel>> GetAllDevices(bool activatedOnly, string status)
        {
            var query = DbContext.Devices.Select(_mapper.Projection);

            if (activatedOnly) query = query.Where(c => c.ClientId != null);
            if (!String.IsNullOrWhiteSpace(status)) query = query.Where(c => c.Status == status);

            return await query.Select(c => new DeviceShortModel
            {
                Id = c.Id,
                MACAddress = c.MACAddress,
                ClientId = c.ClientId,
                ClientName = c.ClientName,
                Status = c.Status
            })
                              .ToListAsync();
        }

        public async Task<DeviceModel> GetDeviceById(int deviceId)
        {
            var device = await DbContext.Devices.Include(c => c.Client).FirstOrDefaultAsync(c => c.Id == deviceId);

            if (device == null) return null;

            var result = _mapper.ToDeviceModel(device);

            return result;
        }

        public async Task<DeviceModel> GetDeviceByMAC(string mac)
        {
            if (String.IsNullOrWhiteSpace(mac)) throw new ArgumentNullException(nameof(mac));

            mac = mac.Trim().ToLower();

            var device = await DbContext.Devices.SingleOrDefaultAsync(c => c.MACAddress.ToLower() == mac);

            if (device == null) return null;

            var result = _mapper.ToDeviceModel(device);

            return result;
        }

        public async Task<DeviceModel> GetDeviceByToken(string token)
        {
            if (String.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));

            token = token.Trim().ToLower();

            var device = await DbContext.Devices.SingleOrDefaultAsync(c => c.Token.ToLower() == token);

            if (device == null) return null;

            var result = _mapper.ToDeviceModel(device);

            return result;
        }

        public async Task<IExecutionResult> UpdateDeviceStatus(DeviceUpdateStatusModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var device = await DbContext.Devices.FindAsync(model.Id);

            if (device == null)
                throw new InvalidOperationException("Device with id " + model.Id + " is not found.");

            device.Status = model.Status;

            await SaveChangesAsync();

            return ExecutionResult.Success();
        }

        public async Task<bool> UpdateDeviceNetworkStatus(DeviceStatusModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var device = await DbContext.Devices
                                        .Include(c => c.Client)
                                        .Include(c => c.Cameras)
                                        .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (device == null)
                throw new InvalidOperationException("Device with id " + model.Id + " is not found.");

            bool statUpdate = device.NetworkStatus != model.NetworkStatus;

            device.NetworkStatus = model.NetworkStatus;

            foreach (var camera in model.Cameras)
            {
                var deviceCamera = device.Cameras.FirstOrDefault(c => c.Id == camera.Id);

                if (deviceCamera == null) continue;

                statUpdate = statUpdate || deviceCamera.NetworkStatus != camera.NetworkStatus;

                deviceCamera.NetworkStatus = camera.NetworkStatus;
            }

            await SaveChangesAsync();

            if (statUpdate)
            {
                var status = new DeviceStatusModel
                {
                    Id = device.Id,
                    Status = device.Status,
                    NetworkStatus = device.NetworkStatus,
                    Name = device.Name,
                    MACAddress = device.MACAddress,
                    IsArmed = device.IsArmed,
                    Cameras = device.Cameras.Select(c => new CameraStatusModel
                    {
                        Id = c.Id,
                        CameraName = c.CameraName,
                        Status = c.Status,
                        NetworkStatus = c.NetworkStatus
                    })
                                    .ToList()
                };

                await _telegramClient.SendToStatChatAsync(status, device.Client?.Name);
            }

            return statUpdate;
        }

        public async Task<IExecutionResult> CreateDevice(DeviceModel model)
        {
            ValidateModel(model);

            if (await DbContext.Devices.AnyAsync(c => c.MACAddress == model.MACAddress || c.Token == model.Token))
                throw new InvalidOperationException($"The device with mac address {model.MACAddress} is exists.");

            var device = _mapper.ToDevice(model);

            FillAutomaticallyGeneratedDeviceFields(device);
            await DbContext.Devices.AddAsync(device);
          
            await SaveChangesAsync();

            return ExecutionResult.Payload(device.Id);
        }

        public async Task<IExecutionResult> UpdateDevice(DeviceModel model)
        {
            ValidateModel(model);

            var device = await DbContext.Devices.FindAsync(model.Id);

            if (device == null)
                throw new InvalidOperationException($"Device with id {model.Id} is not found.");

            if (await DbContext.Devices.AnyAsync(c => c.MACAddress == model.MACAddress && c.Id != model.Id))
                throw new InvalidOperationException($"The device with mac address {model.MACAddress} is exists.");

            device = _mapper.ToDevice(device, model);

            DbContext.Devices.Update(device);

            await SaveChangesAsync();

            return ExecutionResult.Payload(device.Id);
        }

        public async Task<IExecutionResult> RemoveDevice(int deviceId)
        {
            var device = await DbContext.Devices.FindAsync(deviceId);

            if (device == null)
                throw new InvalidOperationException($"Device with id {deviceId} is not found.");

            DbContext.Devices.Remove(device);

            await SaveChangesAsync();

            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> DeactivateDevice(int deviceId)
        {
            var device = await DbContext.Devices.FindAsync(deviceId);

            if (device == null) return ExecutionResult.Failed("NOTFOUND");
            if (device.ClientId == null) return ExecutionResult.Failed("ALREADYDEACTIVATED");

            device.ClientId = null;
            device.ActivationDate = null;
            device.ClientNetworkIp = null;

            var cameras = await DbContext.Cameras.Where(c => c.DeviceId == device.Id).ToListAsync();

            if (cameras.Any()) DbContext.Cameras.RemoveRange(cameras);

            await SaveChangesAsync();

            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> BindDeviceToClient(string token, int clientId, string timeZone)
        {
            var device = await DbContext.Devices.SingleOrDefaultAsync(c => c.Token.ToLower() == token.ToLower());

            if (device == null)
                throw new InvalidOperationException("Device is't found. Please, verify token you use.");

            if (device.ClientId != null)
                throw new InvalidOperationException("Device was binded to a client already.");

            device.ClientId = clientId;
            device.TimeZone = GetTimeZoneId(timeZone);
            DbContext.Devices.Update(device);
            await SaveChangesAsync();

            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> UpdateDeviceName(int deviceId, string newDeviceName)
        {
            var device = await DbContext.Devices.FindAsync(deviceId);

            if (device == null)
                throw new InvalidOperationException("Device with id " + deviceId + " is not found.");

            device.Name = newDeviceName;
            await SaveChangesAsync();

            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> ClearPreviousDeviceSshPassword(string mac)
        {
            var device = DbContext.Devices.SingleOrDefault(d => d.MACAddress == mac);

            if (device == null)
                return ExecutionResult.Failed("Device with mac address " + mac + " is not found.");

            device.PreviousSshPassword = null;
            await SaveChangesAsync();

            return ExecutionResult.Success();
        }

        public async Task UpdateDevicesSshPassword(int expiredPeriod)
        {
            var devices = GetDevicesWithExpiredSshPassword(expiredPeriod);

            foreach (Device device in devices)
            {
                SetUpNewDeviceSshPassword(device);
            }

            await SaveChangesAsync();
        }

        public async Task<DeviceAuthToken> GetDeviceAuthToken(string mac)
        {
            if (String.IsNullOrWhiteSpace(mac)) throw new ArgumentNullException(nameof(mac));

            mac = mac.Trim().ToLower();

            var device = await DbContext.Devices.SingleOrDefaultAsync(c => c.MACAddress.ToLower() == mac);

            if (device == null) return null;

            return new DeviceAuthToken
            {
                DeviceId = device.Id,
                AuthToken = device.AuthToken,
                Name = device.Name,
                MACAddress = device.MACAddress
            };
        }

        public async Task UpdateAuthToken(int deviceId, string token)
        {
            var device = await DbContext.Devices.FindAsync(deviceId);

            if (device == null) return;

            device.AuthToken = token;

            await SaveChangesAsync();
        }

        public async Task ResetAuthToken(int deviceId)
        {
            var device = await DbContext.Devices.FindAsync(deviceId);

            if (device == null) return;

            device.AuthToken = null;

            await SaveChangesAsync();
        }

        public async Task UpdateDeviceIpAddress(int deviceId, string ipAddress)
        {
            if ( !String.IsNullOrEmpty(ipAddress) )
            {
                if ( !Regex.IsMatch(ipAddress, Consts.Consts.ipRegularString) )
                {
                    throw new InvalidOperationException($"IP address {ipAddress} is not valid format.");
                }

                var device = await DbContext.Devices.FindAsync(deviceId);

                if ( device == null )
                {
                    throw new InvalidOperationException($"Device with id {deviceId} is not found.");
                }

                device.ClientNetworkIp = ipAddress;

                await SaveChangesAsync();
            }
        }

        public void FillAutomaticallyGeneratedDeviceFields(Device device)
        {
            var deviceTokens = GetDevicesTokens();
            var token = UniqueStringService.GenerateUniqueString(8);

            while (deviceTokens.Contains(token))
            {
                token = UniqueStringService.GenerateUniqueString(8);
            }

            device.Token = token;
            SetUpNewDeviceSshPassword(device);
        }

        public async Task<IExecutionResult> ChangeDeviceType(int deviceId, DeviceType deviceType)
        {
            var device = await DbContext.Devices.FindAsync(deviceId);

            if (device == null)
                throw new InvalidOperationException("Device with id " + deviceId + " is not found.");

            device.DeviceType = deviceType;
            await SaveChangesAsync();

            return ExecutionResult.Success();
        }

        public async Task ChangeTimeZone(int deviceId, string timeZone)
        {
            var device = await DbContext.Devices.FindAsync(deviceId);

            if ( device == null )
                throw new InvalidOperationException("Device with id " + deviceId + " is not found.");

            if ( !TZConvert.TryGetTimeZoneInfo(timeZone, out _) )
            {
                throw new InvalidOperationException("Time zone " + timeZone + " is not found.");
            }

            device.TimeZone = timeZone;

            await SaveChangesAsync();
        }

        protected virtual IEnumerable<string> GetDevicesTokens()
        {
            return DbContext.Devices.Select(d => d.Token).ToList();
        }

        private IEnumerable<Device> GetDevicesWithExpiredSshPassword(int expiredPeriod)
        {
            var expiredDate = DateTime.Now.AddDays(-expiredPeriod);

            return DbContext.Devices.Where(d => d.PreviousSshPassword == null && d.LastUpdateDatePassword < expiredDate).ToList();
        }

        private void SetUpNewDeviceSshPassword(Device device)
        {
            device.PreviousSshPassword = device.CurrentSshPassword;
            device.CurrentSshPassword = UniqueStringService.GenerateUniqueString(12).Encrypt();
            device.LastUpdateDatePassword = DateTime.Now;
        }

        private void ValidateModel(DeviceModel deviceModel)
        {
            if (deviceModel == null)
                throw new ArgumentNullException(nameof(deviceModel));

            if (string.IsNullOrWhiteSpace(deviceModel.MACAddress))
                throw new InvalidOperationException($"{nameof(deviceModel.MACAddress)} is null.");

            if (string.IsNullOrWhiteSpace(deviceModel.Name))
                throw new InvalidOperationException($"{nameof(deviceModel.Name)} is null.");

            if (!deviceModel.MACAddress.IsValidMacAddress())
                throw new InvalidOperationException($"$ Mac address {deviceModel.MACAddress} is not valid format.");
        }

        private string GetTimeZoneId(string timeZone)
        {
            if ( String.IsNullOrEmpty(timeZone) )
            {
                return null;
            }

            var tz = TZConvert.GetTimeZoneInfo(timeZone);

            return tz.Id;
        }
        private IQueryable<DeviceModel> FilterRequest(DeviceRequest request, IQueryable<DeviceModel> query)
        {
            var res = query;
            var searchText = request.SearchText.ToUpper();
            switch (request.FilterBy)
            {
                case DeviceRequest.FilterType.DeviceName:
                    res = query.Where (c => c.Name.ToUpper().Contains (searchText));
                    break;
                case DeviceRequest.FilterType.MACAddress:
                    res = query.Where(c => c.MACAddress.ToUpper().Contains (searchText));
                    break;
                default:
                    res = query.Where(c =>
                    c.MACAddress.ToUpper().Contains (searchText) ||
                    c.SerialNumber.ToUpper().Contains (searchText) ||
                    c.Token.ToUpper().Contains (searchText) ||
                    c.NVIDIASn.ToUpper().Contains (searchText));
                    break;
            }

            return res;
        }
    }
}