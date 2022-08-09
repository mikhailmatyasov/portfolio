using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;

namespace WeSafe.Services
{
    public class DeviceIndicatorsService : BaseService, IDeviceIndicatorsService
    {
        public DeviceIndicatorsService(WeSafeDbContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
        }

        public async Task<DateTimeOffset?> GetLastIndicatorsTime(int deviceId)
        {
            var indicator = await DbContext.DeviceIndicators
                                           .AsNoTracking()
                                           .Where(c => c.DeviceId == deviceId)
                                           .OrderByDescending(c => c.Time)
                                           .FirstOrDefaultAsync();

            return indicator?.Time;
        }

        public async Task<IEnumerable<DeviceIndicatorsModel>> GetDeviceIndicators(int deviceId, DateTimeOffset? from, DateTimeOffset? to)
        {
            if ( from != null && to != null && from > to )
            {
                throw new ArgumentException("Start time must be less than end time");
            }

            var indicators = await DbContext.DeviceIndicators
                                            .AsNoTracking()
                                            .Where(c => c.DeviceId == deviceId && (from == null || c.Time >= from)
                                                                               && (to == null || c.Time < to))
                                            .OrderBy(c => c.Time)
                                            .ToListAsync();

            return indicators.Select(c => new DeviceIndicatorsModel
                             {
                                 Id = c.Id,
                                 CpuUtilization = c.CpuUtilization,
                                 GpuUtilization = c.GpuUtilization,
                                 MemoryUtilization = c.MemoryUtilization,
                                 Temperature = c.Temperature,
                                 Traffic = c.Traffic,
                                 CamerasFps = JsonConvert.DeserializeObject<Dictionary<int, double>>(c.CamerasFps),
                                 Time = c.Time
                             })
                             .ToList();
        }

        public async Task UpdateDeviceIndicators(int deviceId, IDeviceIndicators indicators)
        {
            if ( indicators == null )
            {
                throw new ArgumentNullException(nameof(indicators));
            }

            if ( !await DbContext.Devices.AnyAsync(c => c.Id == deviceId) )
            {
                throw new InvalidOperationException($"Device with id {deviceId} is not found.");
            }

            if ( indicators.IsEmpty() )
            {
                return;
            }

            DbContext.DeviceIndicators.Add(new DeviceIndicators
            {
                DeviceId = deviceId,
                CpuUtilization = indicators.CpuUtilization,
                GpuUtilization = indicators.GpuUtilization,
                MemoryUtilization = indicators.MemoryUtilization,
                Temperature = indicators.Temperature,
                Traffic = indicators.Traffic,
                CamerasFps = JsonConvert.SerializeObject(indicators.CamerasFps),
                Time = DateTimeOffset.UtcNow
            });

            await SaveChangesAsync();
        }
    }
}