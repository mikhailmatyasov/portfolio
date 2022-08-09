using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeSafe.Services.Client;

namespace WeSafe.Nano.Services
{
    /// <summary>
    /// Host service that contains API to start timer for clearing old device logs.
    /// </summary>
    public class ClearDeviceLogsService : IHostedService
    {
        #region Fields

        private Timer _timer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ClearDeviceLogsService> _logger;

        #endregion

        /// <summary>
        /// Initialize new instance <see cref="ClearDeviceLogsService"/>.
        /// </summary>
        /// <param name="serviceScopeFactory">Service scope factory.</param>
        /// <param name="logger">Current logger.</param>
        public ClearDeviceLogsService(IServiceScopeFactory serviceScopeFactory, ILogger<ClearDeviceLogsService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        #region IHostedService

        /// <summary>
        /// Starts service.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DeleteDeviceLogs, null, TimeSpan.Zero,
                TimeSpan.FromHours(3));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops service.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        private async void DeleteDeviceLogs(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var deviceLogService = scope.ServiceProvider.GetRequiredService<ICleanupLogsService>();
                var globalSettingsService = scope.ServiceProvider.GetRequiredService<IGlobalSettingsService>();
                var settings = await globalSettingsService.GetGlobalSettings();

                await deviceLogService.DeleteDeviceLogsOlderThan(TimeSpan.FromDays(settings.KeepingDeviceLogsDays));
            }
        }

        #endregion
    }
}
