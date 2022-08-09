using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a service that manages <see cref="ISendBus"/> endpoint connection recovery tasks.
    /// </summary>
    public interface ISendBusRecoveryManager : IDisposable
    {
        /// <summary>
        /// Returns an awaitable task that ends when the endpoint is recovered or the timeout is reached.
        /// </summary>
        /// <param name="endpointId">The endpoint connection identifier.</param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        Task WaitForRecoveryAsync(Guid endpointId, int timeoutMs);

        /// <summary>
        /// Attempts to start a recovery task for the a specific endpoint connection.
        /// </summary>
        /// <param name="endpointId">The endpoint connection identifier.</param>
        void TryStartRecovery(Guid endpointId);
    }

    public class SendBusRecoveryManager : ISendBusRecoveryManager
    {
        private readonly ILogger<IBus> _logger;
        private readonly IEndpointManager _endpointManager;
        private readonly ConcurrentDictionary<Guid, RecoveryTask> _recoveryTasks;

        public SendBusRecoveryManager(
            ILogger<IBus> logger,
            IEndpointManager endpointManager)
        {
            _logger = logger;
            _endpointManager = endpointManager;
            _recoveryTasks = new ConcurrentDictionary<Guid, RecoveryTask>();
        }

        public async Task WaitForRecoveryAsync(Guid endpointId, int timeoutMs)
        {
            timeoutMs = timeoutMs > 0 ? timeoutMs : 0;

            var task = _recoveryTasks.TryGetValue(endpointId, out RecoveryTask recoveryTask)
                ? recoveryTask.Task ?? Task.FromResult(false)
                : Task.FromResult(false);

            await Task.WhenAny(task, Task.Delay(timeoutMs));
        }

        public void TryStartRecovery(Guid endpointId)
        {
            if(_recoveryTasks.TryGetValue(endpointId, out RecoveryTask recoveryTask))
            {
                if(recoveryTask.Task?.IsCompleted == false)
                {
                    return;
                }

                _recoveryTasks.TryUpdate(endpointId, new RecoveryTask(endpointId, RecoverAsync(endpointId)), recoveryTask);
            }
            else
            {
                _recoveryTasks.TryAdd(endpointId, new RecoveryTask(endpointId, RecoverAsync(endpointId)));
            }
        }

        public void Dispose()
        {
            _recoveryTasks.Clear();
        }

        private async Task RecoverAsync(Guid endpointId)
        {
            if(endpointId == Guid.Empty)
            {
                await Task.Delay(3000);

                return;
            }

            _logger.LogInformation($"{nameof(ISendBusRecoveryManager)}: Stopping Send endpoint...");

            await _endpointManager.StopAsync(endpointId);

            _logger.LogInformation($"{nameof(ISendBusRecoveryManager)}: Send endpoint stopped. Restarting endpoint...");

            await _endpointManager.StartAsync(endpointId, EndpointData.SendOnly());

            _logger.LogInformation($"{nameof(ISendBusRecoveryManager)}: Send endpoint started.");

            _recoveryTasks[endpointId] = new RecoveryTask(endpointId);
        }
    }
}
