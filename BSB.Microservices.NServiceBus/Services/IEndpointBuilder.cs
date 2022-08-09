using NServiceBus;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a service used to build bus connection endpoints.
    /// </summary>
    public interface IEndpointBuilder
    {
        /// <summary>
        /// Builds an <see cref="EndpointConfiguration"/> using the registered <see cref="INServiceBusConfiguration"/>.
        /// </summary>
        /// <param name="endpoint">The configured endpoint.</param>
        Task<EndpointConfiguration> BuildEndpointAsync(EndpointData endpoint);
    }
}
