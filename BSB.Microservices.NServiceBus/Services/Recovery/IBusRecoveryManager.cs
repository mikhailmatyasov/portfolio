using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a service that manages <see cref="IBus"/> endpoint connection recovery tasks.
    /// </summary>
    public interface IBusRecoveryManager
    {
        /// <summary>
        /// Recovers a connection for the <see cref="IBus"/> endpoint
        /// </summary>
        /// <param name="endpoint">The configured endpoint.</param>
        /// <param name="onNewEndpoint">An action to perform with a new endpoint identifier made during recovery.</param>
        Task RecoverConnectionAsync(EndpointData endpoint, Func<Guid, Task> onNewEndpoint);

        /// <summary>
        /// Starts a task that monitors and recovers connections for the <see cref="IBus"/> endpoint
        /// </summary>
        /// <param name="endpoint">The configured endpoint.</param>
        /// <param name="setEndpointId">An action to perform with a new endpoint identifier made during recovery.</param>
        /// <param name="cancellationToken">The cancellation token for the monitor task.</param>
        Task StartEndpointRecoveryMonitorAsync(EndpointData endpoint, Func<Guid, Task> setEndpointId, CancellationToken cancellationToken);
    }

    public class BusRecoveryManager : IBusRecoveryManager
    {
        private readonly ILogger<IBus> _logger;
        private readonly ICircuitBreaker _circuitBreaker;
        private readonly IEndpointManager _endpointManager;
        private readonly IConfigurationProvider _configurationProvider;

        public BusRecoveryManager(
            ILogger<IBus> logger,
            ICircuitBreaker circuitBreaker,
            IEndpointManager endpointManager,
            IConfigurationProvider configurationProvider)
        {
            _logger = logger;
            _circuitBreaker = circuitBreaker;
            _endpointManager = endpointManager;
            _configurationProvider = configurationProvider;
        }

        public async Task StartEndpointRecoveryMonitorAsync(
            EndpointData endpoint,
            Func<Guid, Task> onNewEndpoint,
            CancellationToken cancellationToken)
        {
            var configuration = await _configurationProvider.GetConfigurationAsync();

            if (configuration.ReceiveEndpointRotationDisabled && endpoint.Type != EndpointType.SendOnly)
            {
                return;
            }

            var healthEndpointId = await _endpointManager.StartAsync(EndpointData.SendOnly());

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var rotationDelaySecs = configuration.ReceiveEndpointRotationSeconds.GetValueOrDefault();

                    rotationDelaySecs = rotationDelaySecs >= 1 ? rotationDelaySecs : 10;

                    _logger.LogTrace($"{nameof(IBusRecoveryManager)}: Delaying for {rotationDelaySecs} seconds.");

                    await Task.Delay(TimeSpan.FromSeconds(rotationDelaySecs), cancellationToken);

                    if (await ShouldRecoverAsync(healthEndpointId))
                    {
                        await _circuitBreaker.ArmAsync();

                        await RecoverConnectionAsync(endpoint, onNewEndpoint);
                    }
                    else
                    {
                        await _circuitBreaker.DisarmAsync();
                    }
                }
            }
            catch (TaskCanceledException)
            { }
        }

        public async Task RecoverConnectionAsync(EndpointData endpoint, Func<Guid, Task> onNewEndpoint)
        {
            _logger.LogInformation($"{nameof(IBusRecoveryManager)}: Starting new connection...");

            var newEndpointId = await _endpointManager.StartAsync(endpoint);

            _logger.LogInformation($"{nameof(IBusRecoveryManager)}: Connection started. Rotating...");

            await onNewEndpoint(newEndpointId);

            _logger.LogInformation($"{nameof(IBusRecoveryManager)}: Connection rotated.");
        }

        private async Task<bool> ShouldRecoverAsync(Guid healthEndpointId)
        {
            try
            {
                await _endpointManager.Get(healthEndpointId).Publish(new Checkup());

                return false;
            }
            catch (Exception ex)
            {
                try
                {
                    await _endpointManager.StopAsync(healthEndpointId);
                    await _endpointManager.StartAsync(healthEndpointId, EndpointData.SendOnly());

                    return true;
                }
                catch (Exception)
                {
                    _logger.LogError(default(EventId), ex, $"{nameof(IBusRecoveryManager)}: Unrecoverable connection. Will not rotate.");

                    return false;
                }
            }
        }

        public class Checkup : IEvent { }
    }
}
