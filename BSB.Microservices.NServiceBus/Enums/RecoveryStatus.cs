using System.Diagnostics.CodeAnalysis;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents the current connection recovery status for a bus.
    /// </summary>
    public enum RecoveryStatus
    {
        /// <summary>
        /// Connection recovery has not started and is not running.
        /// </summary>
        NotStarted,

        /// <summary>
        /// Connection recovery is in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// Connection recovery completed successfully.
        /// </summary>
        Complete,

        /// <summary>
        /// Connection recovery failed.
        /// </summary>
        Failed
    }
}
