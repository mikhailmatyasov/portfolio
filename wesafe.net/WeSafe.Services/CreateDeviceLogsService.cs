using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;

namespace WeSafe.Services
{
    public class CreateDeviceLogsService : IHostedService
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<CreateDeviceLogsService> _logger;       
        private ConcurrentQueue<DeviceLogModel> DeviceLogModels { get; }
        private const int maxStoredDeviceLogsNumber = 1000;
        private bool isProcessing;

        public CreateDeviceLogsService(ILogger<CreateDeviceLogsService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            DeviceLogModels = new ConcurrentQueue<DeviceLogModel>();
        }

        public void AddLogsToTheList(IEnumerable<DeviceLogModel> deviceLogModels)
        {
            DeviceLogModels.EnqueueRange(deviceLogModels);
        }

        public void InsertLastLogs()
        {
            if (isProcessing)
                return;

            isProcessing = true;

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    IDeviceLogService deviceLogService = scope.ServiceProvider.GetRequiredService<IDeviceLogService>();
                    IEnumerable<DeviceLogModel> devices = DeviceLogModels.DequeueRange();
                    deviceLogService.InsertDevicesLogs(devices);
                }
            }

            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }

            finally
            {
                isProcessing = false;
            }        
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(InsertDeviceLogs, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private void InsertDeviceLogs(object state)
        {
            if (DeviceLogModels.Count > maxStoredDeviceLogsNumber)
                InsertLastLogs();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
