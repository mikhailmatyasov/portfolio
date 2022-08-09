using NServiceBus;
using System.Diagnostics.CodeAnalysis;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Defines the supported messaging transports.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Transport
    {
        /// <summary>
        /// Defines the RabbitMQ transport.
        /// </summary>
        public class RabbitMQ : RabbitMQTransport
        {

        }
    }
}
