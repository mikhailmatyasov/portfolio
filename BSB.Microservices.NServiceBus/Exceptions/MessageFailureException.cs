using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents an unrecoverable exception thrown that will not be retried.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MessageFailureException : Exception
    {
        public MessageFailureException()
        {
        }

        public MessageFailureException(string message)
            : base(message)
        {
        }

        public MessageFailureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MessageFailureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
