using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using WeSafe.DAL;
using WeSafe.Services.Client;

namespace WeSafe.Services
{
    public class CleanupLogsService : BaseService, ICleanupLogsService
    {
        public CleanupLogsService(WeSafeDbContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
        }

        public async Task DeleteCameraLogsOlderThan(TimeSpan time)
        {
            var dateTime = DateTime.UtcNow.Add(-time);

            await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"CameraLogs\" WHERE \"Time\" <= @maxDateTime",
                new NpgsqlParameter("@maxDateTime", dateTime));
        }

        public async Task DeleteDeviceLogsOlderThan(TimeSpan time)
        {
            //[LS] To delete records without loading data from entity framework.
            var dateTime = DateTime.UtcNow.Add(-time);
            await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"DeviceLogs\" WHERE \"DateTime\" <= @maxDateTime",
                new NpgsqlParameter("@maxDateTime", dateTime));
        }
    }
}