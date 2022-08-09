using NServiceBus;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a service that creates and starts NServiceBus endpoints using the provided configuration.
    /// </summary>
    public interface IEndpointProvider
    {
        /// <summary>
        /// Creates a new startable endpoint based on the provided configuration.
        /// </summary>
        /// <param name="configuration">The provided configuration.</param>
        Task<IStartableEndpoint> CreateAsync(EndpointConfiguration configuration);

        /// <summary>
        /// Creates and starts a new endpoint based on the provided configuration.
        /// </summary>
        /// <param name="configuration">The provided configuration.</param>
        Task<IEndpointInstance> StartAsync(EndpointConfiguration configuration);
    }

    [ExcludeFromCodeCoverage]
    public class EndpointProvider : IEndpointProvider
    {
        public async Task<IStartableEndpoint> CreateAsync(EndpointConfiguration configuration)
        {
            return await Endpoint.Create(configuration);
        }

        public async Task<IEndpointInstance> StartAsync(EndpointConfiguration configuration)
        {
            return await Endpoint.Start(configuration);
        }
    }
}
