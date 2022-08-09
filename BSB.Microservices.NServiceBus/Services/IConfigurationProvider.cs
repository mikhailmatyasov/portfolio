using NServiceBus.Transport;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a service that provides the registered configuration
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Gets the registered configuration
        /// </summary>
        Task<INServiceBusConfiguration> GetConfigurationAsync();
    }

    /// <summary>
    /// Represents a typed service that provides the registered configuration for a specific transport
    /// </summary>
    public interface IConfigurationProvider<TTransport, TConfiguration> : IConfigurationProvider
        where TTransport : TransportDefinition
        where TConfiguration : INServiceBusConfiguration<TTransport>
    {
        /// <summary>
        /// Gets the registered configuration
        /// </summary>
        Task<TConfiguration> GetAdvancedConfigurationAsync();
    }
}
