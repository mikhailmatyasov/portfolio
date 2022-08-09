using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;

namespace WeSafe.Services
{
    public class CameraLogService : BaseService, ICameraLogService
    {
        public CameraLogService(WeSafeDbContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
        }

        public async Task<int> CreateCameraLog(CameraLogModel model)
        {
            if ( model == null ) throw new ArgumentNullException(nameof(model));

            var log = new CameraLog
            {
                CameraId = model.CameraId,
                Alert = model.Alert,
                Parameters = model.Parameters,
                Message = model.Message,
                Time = DateTimeOffset.UtcNow
            };

            if ( model.Entries != null )
            {
                log.Entries = model.Entries.Select(c => new CameraLogEntry
                                   {
                                       TypeKey = c.TypeKey,
                                       ImageUrl = c.ImageUrl,
                                       UrlExpiration = c.UrlExpiration
                                   })
                                   .ToList();
            }

            DbContext.CameraLogs.Add(log);

            await SaveChangesAsync();

            return log.Id;
        }

        public async Task<PageResponse<CameraLogModel>> GetEvents(EventRequest request)
        {
            if ( request == null )
                throw new ArgumentNullException(nameof(request));

            var query = from cameraLog in DbContext.CameraLogs
                        join camera in DbContext.Cameras on cameraLog.CameraId equals camera.Id
                        join device in DbContext.Devices on camera.DeviceId equals device.Id
                        where (request.DeviceId == null || device.Id == request.DeviceId)
                              && (request.CameraId == null || camera.Id == request.CameraId)
                              && (request.FromDate == null || cameraLog.Time >= request.FromDate)
                              && (request.ToDate == null || cameraLog.Time <= request.ToDate)
                        orderby cameraLog.Id descending
                        select new CameraLogModel
                        {
                            Id = cameraLog.Id,
                            CameraId = cameraLog.CameraId,
                            CameraName = camera.CameraName,
                            DeviceId = device.Id,
                            DeviceName = device.Name,
                            Message = cameraLog.Message,
                            Time = cameraLog.Time,
                            Alert = cameraLog.Alert,
                            Entries = cameraLog.Entries
                                               .Select(c => new CameraLogEntryModel
                                               {
                                                   Id = c.Id,
                                                   CameraLogId = c.CameraLogId,
                                                   TypeKey = c.TypeKey,
                                                   ImageUrl = c.ImageUrl
                                               })
                                               .OrderBy(c => c.Id)
                                               .ToList()
                        };

            var page = await query.ApplyPageRequest(request);

            return new PageResponse<CameraLogModel>(await page.Query.ToListAsync(), page.Total);
        }
    }
}