using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a service used to manage all existing endpoint connections
    /// </summary>
    public interface IEndpointManager : IDisposable
    {
        /// <summary>
        /// Creates and starts an endpoint connection
        /// </summary>
        /// <param name="endpoint">The configured endpoint.</param>
        /// <returns>The endpoint connection identifier.</returns>
        Task<Guid> StartAsync(EndpointData endpoint);

        /// <summary>
        /// Starts the connection for the given endpoint identifier.
        /// </summary>
        /// <param name="id">The endpoint identifier</param>
        /// <param name="endpoint">The configured endpoint.</param>
        /// <returns>The endpoint connection identifier.</returns>
        Task StartAsync(Guid id, EndpointData endpoint);

        /// <summary>
        /// Stops the connection for the given endpoint identifier.
        /// </summary>
        /// <param name="id">The endpoint identifier</param>
        Task StopAsync(Guid id);

        /// <summary>
        /// Stops the connection and releases the identifier for the given endpoint.
        /// </summary>
        /// <param name="id">The endpoint identifier</param>
        Task DisposeEndpointAsync(Guid id);

        /// <summary>
        /// Gets the connection's <see cref="IEndpointInstance"/> for the given endpoint identifier.
        /// </summary>
        /// <param name="id">The endpoint identifier</param>
        IEndpointInstance Get(Guid id);
    }

    public class EndpointManager : AsyncDisposable, IEndpointManager
    {
        private readonly ILogger<IBus> _logger;
        private readonly IEndpointBuilder _endpointBuilder;
        private readonly IEndpointProvider _endpointProvider;

        private readonly ConcurrentDictionary<Guid, IEndpointInstance> _endpointInstances;        

        public EndpointManager(
            ILogger<IBus> logger,
            IEndpointBuilder endpointBuilder,
            IEndpointProvider endpointProvider)
        {
            _logger = logger;
            _endpointBuilder = endpointBuilder;
            _endpointProvider = endpointProvider;
            _endpointInstances = new ConcurrentDictionary<Guid, IEndpointInstance>();
        }

        public async Task<Guid> StartAsync(EndpointData endpoint)
        {
            ThrowIfDisposed();

            var id = Guid.NewGuid();

            _endpointInstances[id] = await StartEndpointAsync(endpoint);

            return id;
        }

        public async Task StartAsync(Guid id, EndpointData endpoint)
        {
            ThrowIfDisposed();

            if (!_endpointInstances.TryGetValue(id, out IEndpointInstance endpointInstance) || endpointInstance == null)
            {
                _endpointInstances[id] = await StartEndpointAsync(endpoint);
            }
        }

        public async Task StopAsync(Guid id)
        {
            ThrowIfDisposed();

            if (_endpointInstances.TryGetValue(id, out IEndpointInstance endpointInstance) && endpointInstance != null)
            {
                try
                {
                    await endpointInstance.Stop();

                    _endpointInstances[id] = null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(default(EventId), ex, $"Failed to stop endpoint: {ex.Message}");
                }
            }
        }

        public async Task DisposeEndpointAsync(Guid id)
        {
            ThrowIfDisposed();

            await StopAsync(id);

            _endpointInstances.TryRemove(id, out IEndpointInstance endpointInstance);
        }

        public IEndpointInstance Get(Guid id)
        {
            ThrowIfDisposed();

            return _endpointInstances.TryGetValue(id, out IEndpointInstance endpoint)
                ? endpoint
                : null;
        }

        protected override async Task DisposeAsync()
        {
            var stopTasks = _endpointInstances.Values.Select(async x => await x.Stop());

            await Task.WhenAll(stopTasks);

            _endpointInstances.Clear();
        }

        private async Task<IEndpointInstance> StartEndpointAsync(EndpointData endpoint)
        {
            try
            {
                var endpointConfig = await _endpointBuilder.BuildEndpointAsync(endpoint);

                var startableEndpoint = await _endpointProvider.CreateAsync(endpointConfig);

                if (string.IsNullOrEmpty(endpoint.Suffix))
                {
                    _logger.LogInformation($"Endpoint Configured: {endpoint.Type}");
                }
                else
                {
                    _logger.LogInformation($"Endpoint Configured: {endpoint.Type} Suffix={endpoint.Suffix ?? string.Empty}");
                }

                var endpointInstance = await startableEndpoint.Start();

                if (string.IsNullOrEmpty(endpoint.Suffix))
                {
                    _logger.LogInformation($"Endpoint Started: {endpoint.Type}");
                }
                else
                {
                    _logger.LogInformation($"Endpoint Started: {endpoint.Type} Suffix={endpoint.Suffix ?? string.Empty}");
                }

                return endpointInstance;
            }
            catch(Exception ex)
            {
                _logger.LogError(default(EventId), ex, $"Failed to start endpoint: {ex.Message}.  {ex.StackTrace}. {ex?.InnerException?.StackTrace}");

                return null;
            }
        }
    }
}
