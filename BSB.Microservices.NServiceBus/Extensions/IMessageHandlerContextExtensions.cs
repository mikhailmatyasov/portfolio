using BSB.Microservices.NServiceBus;
using System;

namespace NServiceBus
{
    public static class IMessageHandlerContextExtensions
    {
        public static void RetryNow<T>(this T context) where T : IMessageHandlerContext
        {
            throw new MessageRetryException();
        }

        public static void RetryLater<T>(this T context, ushort delayMs) where T : IMessageHandlerContext
        {
            throw new MessageRetryException(delayMs);
        }

        public static void RetryLater<T>(this T context, TimeSpan delay) where T : IMessageHandlerContext
        {
            throw new MessageRetryException(delay);
        }
    }
}
