using System;
using System.Threading.Tasks;
using NServiceBus;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// A base message handler that exposes a handler-specific retry implementation.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <remarks>Immediate retries always precede delayed retries.</remarks>
    public abstract class RecoverableHandler<T> : IHandleMessages<T> where T : IMessage
    {
        /// <summary>
        /// The default timespan in milliseconds for delayed retry attempts.
        /// </summary>
        protected const ushort DefaultRetryDelayMs = 3000;

        /// <summary>
        /// The maximum number of immediate retry attempts.
        /// </summary>
        protected virtual ushort ImmediateRetries { get; set; }

        /// <summary>
        /// The maximum number of delayed retry attempts.
        /// </summary>
        protected virtual ushort DelayedRetries { get; set;  }

        public abstract Task HandleAsync(T message, IMessageHandlerContext context);

        /// <summary>
        /// Returns the number of milliseconds to delay for a retry attempt.
        /// </summary>
        /// <param name="delayedRetryAttempts">The number of attempted delayed retries performed.</param>
        protected virtual ushort GetRetryDelayMs(ushort delayedRetryAttempts)
        {
            return DefaultRetryDelayMs;
        }

        /// <summary>
        /// Transforms unhandled exceptions into retries based on the <see cref="ImmediateRetries"/> and <see cref="DelayedRetries"/> configuration.
        /// </summary>
        public async Task Handle(T message, IMessageHandlerContext context)
        {
            try
            {
                await HandleAsync(message, context);
            }
            catch (MessageFailureException)
            {
                throw;
            }
            catch (MessageRetryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ushort immediateAttempts = 0;
                ushort delayedAttempts = 0;

                if (context.MessageHeaders.TryGetValue(RetryPolicy.ImmediateRetryKey, out string i) && !ushort.TryParse(i, out immediateAttempts))
                {
                    throw;
                }

                if (immediateAttempts < ImmediateRetries)
                {
                    context.RetryNow();
                }

                if (context.MessageHeaders.TryGetValue(RetryPolicy.DelayedRetryKey, out string d) && !ushort.TryParse(d, out delayedAttempts))
                {
                    throw;
                }

                if (delayedAttempts < DelayedRetries)
                {
                    context.RetryLater(GetRetryDelayMs(delayedAttempts));
                }

                await AfterFinalAttemptAsync(ex);
            }
        }

        /// <summary>
        /// Async Method called after the final retry attempt has failed. Throws the final exception by default.
        /// </summary>
        /// <param name="ex">The final exception</param>
        public virtual Task AfterFinalAttemptAsync(Exception ex)
        {
#if NET452
            return Task.Run(() => throw ex);
#else
            return Task.FromException(ex);
#endif
        }
    }
}
