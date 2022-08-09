using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeSafe.DAL;
using WeSafe.Services;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;

namespace WeSafe.Nano.Services
{
    public class NanoClientService: ClientService
    {
        public NanoClientService(WeSafeDbContext context, ClientMapper mapper, ILoggerFactory loggerFactory) : base(context, mapper, loggerFactory)
        {
        }

        protected override async Task<IEnumerable<CameraLogModel>> GetFilteredEventsAsync(int clientId, EventBaseRequest request)
        {
            var entityQuery = await GetBaseEntityQuery(clientId).ApplyEventsFilterQuery(request).ToListAsync();

            var events = entityQuery.ApplyDateFilterQuery(request).Select(cameraLog => new CameraLogModel
            {
                Id = cameraLog.Id,
                CameraId = cameraLog.CameraId,
                CameraName = cameraLog.Camera.CameraName,
                DeviceId = cameraLog.Camera.Device.Id,
                DeviceName = cameraLog.Camera.Device.Name,
                Message = cameraLog.Message,
                Time = cameraLog.Time,
                Alert = cameraLog.Alert,
                Entries = cameraLog.Entries.Select(c => new CameraLogEntryModel
                {
                    Id = c.Id,
                    CameraLogId = c.CameraLogId,
                    TypeKey = c.TypeKey,
                    ImageUrl = c.ImageUrl
                })
            }).ToList();

            return events;
        }
    }
}
