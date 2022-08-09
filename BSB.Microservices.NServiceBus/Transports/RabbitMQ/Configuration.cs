using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BSB.Microservices.Vault;
using NServiceBus;

namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    /// <summary>
    /// Represents the configuration for the RabbitMQ transport.
    /// </summary>
    public interface IConfiguration : INServiceBusConfiguration<RabbitMQTransport>
    {
        /// <summary>
        /// The connection host. Defaults to localhost.
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// The connection username. Defaults to guest.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// The connection password. Defaults to guest.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// The consumer prefetch count. Defaults to 50.
        /// </summary>
        ushort? PrefetchCount { get; set; }

        /// <summary>
        /// Pulls configuration values from Consul via <see cref="ICachedConsulClientExtensions.GetRabbitConfigurationAsync(Consul.ICachedConsulClient, IServiceIdentity)"/> when true.
        /// </summary>
        bool UseConsul { get; set; }

        /// <summary>
        /// Pulls credentials via the registered <see cref="IVaultClient"/>
        /// </summary>
        bool UseVaultCredentials { get; set; }

        /// <summary>
        /// Disables the <see cref="ITopologyRecoveryService"/> when true.
        /// </summary>
        bool TopologyRecoveryDisabled { get; set; }

        /// <summary>
        /// When set to true: deletes all undeclared queues matching the default naming convention when the <see cref="IReceiveBus"/>.
        /// </summary>
        /// <remarks>This will only be invoked if <see cref="INamingConventions.IsDefaultQueueName"/> is true.</remarks>
        bool DeleteUndeclaredQueuesOnStartEnabled { get; set; }

        /// <summary>
        /// An action to perform configuration overrides for the transport.
        /// </summary>
        Action<TransportExtensions> TransportOverrides { get; set; }

        /// <summary>
        /// An action to perform configuration overrides for the endpoint.
        /// </summary>
        Action<EndpointConfiguration> EndpointOverrides { get; set; }

        /// <summary>
        /// Uses the default rabbit host and credentials in a development environment.
        /// </summary>
        bool UseLocalhostForDevelopment { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Configuration : IConfiguration
    {
        public Configuration()
        {
            EnableMonitoring = true;
            EnableAuditing = true;
            EnableSqlPersistence = true;
            EnableHeartbeats = true;
            SendMetricDataIntervalInSeconds = 2;
            SendHeartbeatIntervalInSeconds = 15;
            HeartbeatTimeToLiveInSeconds = 30;
            CustomChecksTimeToLiveInSeconds = 10;
        }

        public bool PublishRecoveryDisabled { get; set; }

        public int? PublishRecoveryTimeoutMs { get; set; }

        public bool ReceiveEndpointRotationDisabled { get; set; }

        public int? ReceiveEndpointRotationSeconds { get; set; }

        public string RawLicenseConsulKey { get; set; }

        public string Host { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public ushort? PrefetchCount { get; set; }

        public bool UseConsul { get; set; } = true;

        public bool UseVaultCredentials { get; set; }

        public bool TopologyRecoveryDisabled { get; set; }

        public bool DeleteUndeclaredQueuesOnStartEnabled { get; set; }

        public Action<TransportExtensions> TransportOverrides { get; set; }

        public Action<EndpointConfiguration> EndpointOverrides { get; set; }

        public IEnumerable<Assembly> HandleCommandsInAssemblies { get; set; }
        public bool EnableMonitoring { get; set; }
        public int SendMetricDataIntervalInSeconds { get; set; }
        public int SendHeartbeatIntervalInSeconds { get; set; }
        public int HeartbeatTimeToLiveInSeconds { get; set; }
        public int CustomChecksTimeToLiveInSeconds { get; set; }
        public bool EnableHeartbeats { get; set; }
        public bool EnableAuditing { get; set; }
        public bool EnableSqlPersistence { get; set; }
        public bool UseLocalhostForDevelopment { get; set; }
        public TimeSpan? CircuitBreakerTimer { get; set; }
    }
}