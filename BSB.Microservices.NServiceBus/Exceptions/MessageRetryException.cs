using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a recoverable exception thrown that will be retried with the specified delay in milliseconds.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MessageRetryException : Exception
    {
        public TimeSpan? Delay { get; set; }

        public MessageRetryException()
        {
        }

        public MessageRetryException(ushort delayMs)
        {
            Delay = TimeSpan.FromMilliseconds(delayMs);
        }

        public MessageRetryException(TimeSpan timeSpan)
        {
            Delay = timeSpan;
        }

        public MessageRetryException(string message) : base(message)
        {
        }

        public MessageRetryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MessageRetryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
