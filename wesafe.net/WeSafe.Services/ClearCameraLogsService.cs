using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    /// <summary>
    /// Host service that contains API to start timer for clearing old device logs.
    /// </summary>
    public class ClearCameraLogsService : IHostedService
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
        public ClearCameraLogsService(IServiceScopeFactory serviceScopeFactory, ILogger<ClearDeviceLogsService> logger)
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
            _timer = new Timer(DeleteCameraLogs, null, TimeSpan.Zero,
                TimeSpan.FromHours(12));

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

        private async void DeleteCameraLogs(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<ICleanupLogsService>();
                var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<CleanupLogsOptions>>();

                await service.DeleteCameraLogsOlderThan(TimeSpan.FromDays(options.Value.KeepingCameraLogsDays));
            }
        }

        #endregion
    }
}
