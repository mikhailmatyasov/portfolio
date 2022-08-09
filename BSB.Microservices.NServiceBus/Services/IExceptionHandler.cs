using System;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a services used to handle caught exceptions.
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Returns true if the <see cref="ISendBus"/> should initiate a recovery task.
        /// </summary>
        bool ShouldRecoverOnPublishException(Type messageType, Exception exception);
    }
}
