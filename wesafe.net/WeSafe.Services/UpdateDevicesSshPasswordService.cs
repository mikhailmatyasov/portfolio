using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Services.Client;

namespace WeSafe.Services
{
    public class UpdateDevicesSshPasswordService : IHostedService
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private const int expiredDaysPeriod = 14;

        public UpdateDevicesSshPasswordService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(UpdatedExpiredDevicesSshPasswords, null, TimeSpan.Zero,
                TimeSpan.FromHours(6));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void UpdatedExpiredDevicesSshPasswords(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IDeviceService deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();
                deviceService.UpdateDevicesSshPassword(expiredDaysPeriod).Wait();
            }
        }
    }
}
