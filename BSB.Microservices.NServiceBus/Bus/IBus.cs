using System;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a generic messaging bus.
    /// </summary>
    public interface IBus : IDisposable
    {
        /// <summary>
        /// The current running status of the bus instance.
        /// </summary>
        BusStatus Status { get; }

        /// <summary>
        /// Starts the bus.
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Stops the bus.
        /// </summary>
        Task StopAsync();
    }

    public interface IRestartableBus : IBus
    {
        /// <summary>
        /// Restarts the bus.
        /// </summary>
        Task RestartAsync();
    }
}
