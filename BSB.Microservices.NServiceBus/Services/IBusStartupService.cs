using System.Threading;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a registered service that will be started when a <see cref="IBus"/> instance is started for the first time.
    /// </summary>
    public interface IBusStartupService
    {
        /// <summary>
        /// Starts the service.
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
