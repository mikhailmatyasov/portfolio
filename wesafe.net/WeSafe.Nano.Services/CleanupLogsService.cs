using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WeSafe.Nano.DAL;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Nano.Services
{
    public class CleanupLogsService : ICleanupLogsService
    {
        private readonly PhysicalFileStorageOptions _fileStorageOptions;

        private WeSafeNanoDbContext DbContext { get; }

        public CleanupLogsService(WeSafeNanoDbContext dbContext, IOptions<PhysicalFileStorageOptions> options)
        {
            DbContext = dbContext;
            _fileStorageOptions = options.Value;
        }

        public async Task DeleteCameraLogsOlderThan(TimeSpan time)
        {
            var dateTime = DateTime.UtcNow.Add(-time);

            CleanupFileStorage(dateTime);

            await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"CameraLogs\" WHERE \"Time\" <= {0}", dateTime);
        }

        public async Task DeleteDeviceLogsOlderThan(TimeSpan time)
        {
            var dateTime = DateTime.UtcNow.Add(-time);
            await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"DeviceLogs\" WHERE \"DateTime\" <= {0}", dateTime);
        }

        private void CleanupFileStorage(DateTime utcTime)
        {
            if ( !Directory.Exists(_fileStorageOptions.Root) )
            {
                return;
            }

            string[] files = Directory.GetFiles(_fileStorageOptions.Root);

            foreach (string file in files)
            {
                var fi = new FileInfo(file);

                if ( fi.CreationTimeUtc < utcTime )
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }
    }
}