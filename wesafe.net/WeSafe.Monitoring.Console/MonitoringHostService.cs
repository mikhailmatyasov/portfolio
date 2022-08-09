using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeSafe.Monitoring.Services;

namespace WeSafe.Monitoring.Console
{
    public class MonitoringHostService : BackgroundService
    {
        private readonly ILogger<MonitoringHostService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _services;

        public MonitoringHostService(IServiceProvider services, ILogger<MonitoringHostService> logger,
            IConfiguration configuration)
        {
            _services = services;
            _logger = logger;
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Monitoring hosted service running.");

            return DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var notificationTime = DateTimeOffset.UtcNow;
            var notificationPeriod = TimeSpan.FromHours(1);

            while ( !stoppingToken.IsCancellationRequested )
            {
                try
                {
                    using var scope = _services.CreateScope();

                    var monitoringService = scope.ServiceProvider.GetRequiredService<IMonitoringService>();
                    var notification = false;

                    if ( DateTimeOffset.UtcNow > notificationTime + notificationPeriod )
                    {
                        notificationTime = DateTimeOffset.UtcNow;
                        notification = true;
                    }

                    await monitoringService.ProcessAsync(notification, stoppingToken);

                    await Task.Delay(3000, stoppingToken);
                }
                catch ( OperationCanceledException )
                {
                    // Do nothing
                }
                catch ( Exception e )
                {
                    _logger.LogError(e, "Unhandled exception in MonitoringHostService: {Error}",
                        e.Message);
                }
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Monitoring hosted service is stopping.");

            return base.StopAsync(stoppingToken);
        }
    }
}