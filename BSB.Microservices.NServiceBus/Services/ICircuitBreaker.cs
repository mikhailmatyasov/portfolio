using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    public interface ICircuitBreaker
    {
        Task ArmAsync();

        Task DisarmAsync();

        Task InvokeAsync();
    }

    public class NullCircuitBreaker : ICircuitBreaker
    {
        public async Task ArmAsync()
        {
            await Task.Run(() => { });
        }

        public async Task DisarmAsync()
        {
            await Task.Run(() => { });
        }

        public async Task InvokeAsync()
        {
            await Task.Run(() => { });
        }
    }

    public class ApplicationCircuitBreaker : ICircuitBreaker
    {
        private Timer Timer { get; set; } = null;

        private readonly ILogger<ApplicationCircuitBreaker> _logger;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IConfigurationProvider _configurationProvider;

        public ApplicationCircuitBreaker(
            ILogger<ApplicationCircuitBreaker> logger,
            IApplicationLifetime applicationLifetime,
            IConfigurationProvider configurationProvider)
        {
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _configurationProvider = configurationProvider;
        }

        public virtual async Task ArmAsync()
        {
            if(Timer != null)
            {
                return;
            }

            var config = await _configurationProvider.GetConfigurationAsync();

            var timespan = config.CircuitBreakerTimer ?? TimeSpan.FromMinutes(5);

            Timer = new Timer(
                async x => await InvokeAsync(),
                null,
                timespan,
                TimeSpan.FromMilliseconds(-1));

            _logger.LogError($"{nameof(ICircuitBreaker)} Armed. Time Remaining: {timespan.Seconds}.");
        }

        public virtual async Task DisarmAsync()
        {
            await Task.Run(() =>
            {
                if (Timer == null)
                {
                    return;
                }

                Timer?.Dispose();
                Timer = null;

                _logger.LogWarning($"{nameof(ICircuitBreaker)} Disarmed.");
            });
        }

        public virtual async Task InvokeAsync()
        {
            await Task.Run(() =>
            {
                _logger.LogError($"{nameof(ICircuitBreaker)} Invoked. Stopping Application.");

                _applicationLifetime.StopApplication();
            });
        }
    }
}
