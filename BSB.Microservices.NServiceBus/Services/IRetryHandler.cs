using NServiceBus;
using System;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a service that will attempt to handle a message while applying the provided retry policy.
    /// </summary>
    public interface IRetryHandler
    {
        /// <summary>
        /// Invokes handle and immediately retries failed message attempts.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="message">The message instance.</param>
        /// <param name="context">The message context.</param>
        /// <param name="handle">The action to invoke.</param>
        /// <param name="maxImmediateRetries">The maximum number of retry attempts.</param>
        Task TryHandleAsync<T>(
            T message,
            IMessageHandlerContext context,
            Func<T, IMessageHandlerContext, Task> handle,
            ushort maxImmediateRetries) where T : IMessage;

        /// <summary>z
        /// Invokes handle and retries failed message attempts with a delay.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="message">The message instance.</param>
        /// <param name="context">The message context.</param>
        /// <param name="handle">The action to invoke.</param>
        /// <param name="maxDelayedRetries">The maximum number of retry attempts.</param>
        /// <param name="getRetryDelay">A function to get the time to delay before retrying message using the number of performed attempts.</param>
        Task TryHandleAsync<T>(
            T message,
            IMessageHandlerContext context,
            Func<T, IMessageHandlerContext, Task> handle,
            ushort maxDelayedRetries,
            Func<ushort, TimeSpan> getRetryDelay) where T : IMessage;

        /// <summary>
        /// Invokes handle and retries failed message attempts according to the specified policy.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="message">The message instance.</param>
        /// <param name="context">The message context.</param>
        /// <param name="handle">The action to invoke.</param>
        /// <param name="maxImmediateRetries">The maximum number of immediate retry attempts.</param>
        /// <param name="maxDelayedRetries">The maximum number of delayed retry attempts.</param>
        /// <param name="getRetryDelay">A function to get the time to delay before retrying message using the number of performed attempts.</param>
        Task TryHandleAsync<T>(
            T message,
            IMessageHandlerContext context,
            Func<T, IMessageHandlerContext, Task> handle,
            ushort maxImmediateRetries = 0,
            ushort maxDelayedRetries = 0,
            Func<ushort, TimeSpan> getRetryDelay = null) where T : IMessage;
    }

    public class RetryHandler : IRetryHandler
    {
        public const ushort DefaultRetryDelayMs = 3000;

        public async Task TryHandleAsync<T>(
            T message,
            IMessageHandlerContext context,
            Func<T, IMessageHandlerContext, Task> handle,
            ushort maxImmediateRetries) where T : IMessage
        {
            await TryHandleAsync(message, context, handle, maxImmediateRetries, 0, x => TimeSpan.FromMilliseconds(0));
        }

        public async Task TryHandleAsync<T>(
            T message,
            IMessageHandlerContext context,
            Func<T, IMessageHandlerContext, Task> handle,
            ushort maxDelayedRetries,
            Func<ushort, TimeSpan> getRetryDelayMs) where T : IMessage
        {
            await TryHandleAsync(message, context, handle, 0, maxDelayedRetries, getRetryDelayMs);
        }

        public async Task TryHandleAsync<T>(
            T message,
            IMessageHandlerContext context,
            Func<T, IMessageHandlerContext, Task> handle,
            ushort maxImmediateRetries = 0,
            ushort maxDelayedRetries = 0,
            Func<ushort, TimeSpan> getRetryDelay = null) where T : IMessage
        {
            try
            {
                await handle(message, context);
            }
            catch (MessageFailureException)
            {
                throw;
            }
            catch (MessageRetryException)
            {
                throw;
            }
            catch (Exception)
            {
                ushort immediateAttempts = 0;
                ushort delayedAttempts = 0;

                if (context.MessageHeaders.TryGetValue(RetryPolicy.ImmediateRetryKey, out string i) && !ushort.TryParse(i, out immediateAttempts))
                {
                    throw;
                }

                if (immediateAttempts < maxImmediateRetries)
                {
                    context.RetryNow();
                }

                if (context.MessageHeaders.TryGetValue(RetryPolicy.DelayedRetryKey, out string d) && !ushort.TryParse(d, out delayedAttempts))
                {
                    throw;
                }

                if (delayedAttempts < maxDelayedRetries)
                {
                    if(getRetryDelay == null)
                    {
                        getRetryDelay = (numAttempts) => TimeSpan.FromMilliseconds(DefaultRetryDelayMs);
                    }

                    context.RetryLater(getRetryDelay(delayedAttempts));
                }

                throw;
            }
        }
    }
}
