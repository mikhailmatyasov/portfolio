using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class GlobalSettingsService : BaseService, IGlobalSettingsService
    {
        private readonly CleanupLogsOptions _defaultOptions;

        public GlobalSettingsService(WeSafeDbContext context, ILoggerFactory loggerFactory,
            IOptionsSnapshot<CleanupLogsOptions> options) : base(context, loggerFactory)
        {
            _defaultOptions = options.Value;
        }

        public async Task<GlobalSettingsModel> GetGlobalSettings()
        {
            var settings = await DbContext.GlobalSettings.FirstOrDefaultAsync();
            var model = new GlobalSettingsModel();

            if ( settings == null )
            {
                model.KeepingDeviceLogsDays = _defaultOptions.KeepingDeviceLogsDays;
                model.KeepingCameraLogsDays = _defaultOptions.KeepingCameraLogsDays;
            }
            else
            {
                model.KeepingDeviceLogsDays = settings.KeepingDeviceLogsDays;
                model.KeepingCameraLogsDays = settings.KeepingCameraLogsDays;
            }

            return model;
        }

        public async Task UpdateGlobalSettings(GlobalSettingsModel model)
        {
            if ( model == null )
            {
                throw new ArgumentNullException(nameof(model));
            }

            var settings = await DbContext.GlobalSettings.FirstOrDefaultAsync();

            if ( settings == null )
            {
                settings = new GlobalSettings();

                DbContext.GlobalSettings.Add(settings);
            }

            settings.KeepingDeviceLogsDays = model.KeepingDeviceLogsDays;
            settings.KeepingCameraLogsDays = model.KeepingCameraLogsDays;

            await SaveChangesAsync();
        }
    }
}