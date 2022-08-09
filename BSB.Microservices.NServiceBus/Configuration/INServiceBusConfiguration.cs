using NServiceBus.Transport;
using System;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents common configuration values used for all supported messaging transports.
    /// </summary>
    public interface INServiceBusConfiguration
    {
        /// <summary>
        /// Disabled connection recovery for the registered <see cref="ISendBus"/>.
        /// </summary>
        bool PublishRecoveryDisabled { get; set; }

        /// <summary>
        /// The timeout period when awaiting <see cref="ISendBus"/> recovery/>.
        /// </summary>
        int? PublishRecoveryTimeoutMs { get; set; }

        /// <summary>
        /// Disabled connection rotation for the registered <see cref="IReceiveBus"/>.
        /// </summary>
        bool ReceiveEndpointRotationDisabled { get; set; }

        /// <summary>
        /// The timespan in minutes to rotate <see cref="IReceiveBus"/> connections.
        /// </summary>
        int? ReceiveEndpointRotationSeconds { get; set; }

        /// <summary>
        /// The consul key used to retreive the NServieBus raw license text (Default="nservicebus/license/raw"). Set to empty string to ignore.
        /// </summary>
        string RawLicenseConsulKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enabled monitoring].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enabled monitoring]; otherwise, <c>false</c>.
        /// </value>
        bool EnableMonitoring { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable heartbeats].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable heartbeats]; otherwise, <c>false</c>.
        /// </value>
        bool EnableHeartbeats { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable auditing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable auditing]; otherwise, <c>false</c>.
        /// </value>
        bool EnableAuditing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enabled SQL persistence].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enabled SQL persistence]; otherwise, <c>false</c>.
        /// </value>
        bool EnableSqlPersistence { get; set; }

        /// <summary>
        /// Gets or sets the send metric data interval in seconds.
        /// </summary>
        /// <value>
        /// The send metric data interval in seconds.
        /// </value>
        int SendMetricDataIntervalInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the send heartbeat interval in seconds.
        /// </summary>
        /// <value>
        /// The send heartbeat interval in seconds.
        /// </value>
        int SendHeartbeatIntervalInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the heartbeat time to live in seconds.
        /// </summary>
        /// <value>
        /// The heartbeat time to live in seconds.
        /// </value>
        int HeartbeatTimeToLiveInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the custom checks time to live in seconds.
        /// </summary>
        /// <value>
        /// The custom checks time to live in seconds.
        /// </value>
        int CustomChecksTimeToLiveInSeconds { get; set; }

        /// <summary>
        /// The time before an armed circuit breaker is invoked. Defaults to 5 minutes.
        /// </summary>
        TimeSpan? CircuitBreakerTimer { get; set; }
    }

    /// <summary>
    /// Represents common configuration values used for a specific messaging transport.
    /// </summary>
    /// <typeparam name="T">The transport definition used for the configuration</typeparam>
    public interface INServiceBusConfiguration<T> : INServiceBusConfiguration where T : TransportDefinition
    {
    }
}