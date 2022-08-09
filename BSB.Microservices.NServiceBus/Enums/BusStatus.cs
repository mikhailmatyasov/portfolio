namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents the current connection status for a bus.
    /// </summary>
    public enum BusStatus
    {
        /// <summary>
        /// Bus is disconnected and stopped.
        /// </summary>
        Stopped,

        /// <summary>
        /// Bus is connecting and starting.
        /// </summary>
        Starting,

        /// <summary>
        /// Bus is connected and started.
        /// </summary>
        Started,

        /// <summary>
        /// Bus is disconnecting and stopping.
        /// </summary>
        Stopping,

        /// <summary>
        /// Bus is disconnecting and reconnecting.
        /// </summary>
        Restarting
    }
}
