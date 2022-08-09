using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;

namespace WeSafe.Services
{
    /// <summary>
    /// Represents API to manage device logs.
    /// </summary>
    public class DeviceLogService : BaseService, IDeviceLogService
    {
        #region Fields

        private readonly DeviceLogMapper _deviceLogMapper;

        #endregion

        /// <summary>
        /// Initialize new instance <see cref="DeviceLogService"/>.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="deviceLogMapper">Device log mappers.</param>
        /// <param name="clientDeviceLogMapper">Client device log mappers.</param>
        /// <param name="loggerFactory">Logger factory.</param>
        public DeviceLogService(WeSafeDbContext context, DeviceLogMapper deviceLogMapper,
            ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
            _deviceLogMapper = deviceLogMapper;
        }

        #region IDeviceLogService

        /// <summary>
        /// Get device logs.
        /// </summary>
        /// <param name="pageRequest">Request that contains query parameters.</param>
        /// <returns>Page response that contains device logs collection.</returns>
        public async Task<PageResponse<ClientDeviceLogModel>> GetClientDeviceLogsAsync(ClientDeviceLogRecordQuery pageRequest)
        {
            var clientDeviceLogs = GetDeviceLogsQuery(pageRequest).OrderByDescending(d => d.DateTime);
            var result = await clientDeviceLogs.ApplyPageRequest(pageRequest);

            return new PageResponse<ClientDeviceLogModel>
            {
                Items = GetDeviceLogs(result.Query),
                Total = result.Total
            };
        }

        /// <summary>
        /// Inserts device logs.
        /// </summary>
        /// <param name="deviceLogModels">Device logs model collection.</param>
        public void InsertDevicesLogs(IEnumerable<DeviceLogModel> deviceLogModels)
        {
            IEnumerable<DeviceLog> deviceLogs = deviceLogModels.Select(_deviceLogMapper.ToDevice).ToList();
            List<int> validDeviceIds = DbContext.Devices.Select(d => d.Id).ToList();
            List<int> validCameraIds = DbContext.Cameras.Select(c => c.Id).ToList();

            foreach (DeviceLog deviceLog in deviceLogs)
            {
                if (validDeviceIds.Any(d => d == deviceLog.DeviceId) && (deviceLog.CameraId == null || validCameraIds.Any(c => c == deviceLog.CameraId)))
                    DbContext.DeviceLogs.Add(deviceLog);
            }

            SaveChangesAsync().Wait();
        }

        #endregion

        #region Private Methods

        private IEnumerable<ClientDeviceLogModel> GetDeviceLogs(IQueryable<DeviceLog> resultQuery)
        {
            //[LS] : To optimize select query to database.
            return resultQuery?.Select(deviceLog => new ClientDeviceLogModel
            {
                DeviceName = deviceLog.Device.Name,
                LogLevel = deviceLog.LogLevel,
                ErrorMessage = deviceLog.ErrorMessage,
                DateTime = deviceLog.DateTime,
                ClientName = deviceLog.Device.Client == null
                    ? null
                    : deviceLog.Device.Client.Name,
                CameraName = deviceLog.Camera == null
                    ? null
                    : deviceLog.Camera.CameraName

            }).AsNoTracking().ToList();
        }

        private IQueryable<DeviceLog> GetDeviceLogsQuery(ClientDeviceLogRecordQuery pageRequest)
        {
            var clientDeviceLogs = DbContext.DeviceLogs.Include(d => d.Device).ThenInclude(d => d.Client)
                .Include(d => d.Camera).AsQueryable();

            if (pageRequest.ClientId != null)
                clientDeviceLogs = clientDeviceLogs.Where(c => c.Device.Client.Id == pageRequest.ClientId);

            if (pageRequest.DeviceId != null)
                clientDeviceLogs = clientDeviceLogs.Where(c => c.Device.Id == pageRequest.DeviceId);

            if (pageRequest.CameraId != null)
                if (pageRequest.CameraId == -1)
                    clientDeviceLogs = clientDeviceLogs.Where(c => c.Camera == null);
                else
                    clientDeviceLogs = clientDeviceLogs.Where(c => c.Camera.Id == pageRequest.CameraId);

            if (pageRequest.LogLevels != null)
                clientDeviceLogs = clientDeviceLogs.Where(c => pageRequest.LogLevels.Contains(c.LogLevel));

            if (pageRequest.FromDate.HasValue)
                clientDeviceLogs = clientDeviceLogs.Where(c => c.DateTime > pageRequest.FromDate.Value);

            if (pageRequest.ToDate.HasValue)
                clientDeviceLogs = clientDeviceLogs.Where(c => c.DateTime <= pageRequest.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(pageRequest.SearchText))
                clientDeviceLogs = clientDeviceLogs.Where(c => c.ErrorMessage.ToLower().Contains(pageRequest.SearchText.ToLower()));

            return clientDeviceLogs;
        }

        #endregion
    }
}
