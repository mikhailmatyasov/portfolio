using BSB.Microservices.NServiceBus.RabbitMQ;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.Transport;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    public interface IRetryPolicy
    {
        RecoverabilityAction Invoke(RecoverabilityConfig config, ErrorContext errorContext);
    }

    public class RetryPolicy : IRetryPolicy
    {
        public static string ImmediateRetryKey = "BSB.FLRetries";
        public static string DelayedRetryKey = "BSB.Retries";

        private readonly ILogger<IBus> _logger;
        private readonly IConnectionManager _connectionManager;
        private readonly IEndpointNamingConventions _namingConventions;

        public RetryPolicy(
            ILogger<IBus> logger,
            IConnectionManager connectionManager,
            IEndpointNamingConventions namingConventions)
        {
            _logger = logger;
            _connectionManager = connectionManager;
            _namingConventions = namingConventions;
        }

        public RecoverabilityAction Invoke(RecoverabilityConfig config, ErrorContext errorContext)
        {
            if (IsMissingHandler(errorContext))
            {
                TryUnbindExchangeAsync(errorContext);

                return RecoverabilityAction.MoveToError(_namingConventions.GetErrorQueueName());
            }

            if (IsUnrecoverable(config, errorContext))
            {
                return RecoverabilityAction.MoveToError(_namingConventions.GetErrorQueueName());
            }

            if (errorContext.Exception is MessageRetryException messageRetryException)
            {
                if (messageRetryException.Delay != null)
                {
                    var delayedAttempts = errorContext.Message.Headers.TryGetValue(DelayedRetryKey, out string d)
                    ? ushort.Parse(d) + 1
                    : 1;

                    errorContext.Message.Headers[DelayedRetryKey] = delayedAttempts.ToString();

                    return RecoverabilityAction.DelayedRetry(messageRetryException.Delay.Value);
                }

                var immediateAttempts = errorContext.Message.Headers.TryGetValue(ImmediateRetryKey, out string i)
                    ? ushort.Parse(i) + 1
                    : 1;

                errorContext.Message.Headers[ImmediateRetryKey] = immediateAttempts.ToString();

                return RecoverabilityAction.DelayedRetry(TimeSpan.Zero);
            }

            return DefaultRecoverabilityPolicy.Invoke(config, errorContext);
        }

        private Task TryUnbindExchangeAsync(ErrorContext errorContext)
        {
            return Task.Factory.StartNew(() =>
            {
                var queue = _namingConventions.GetQueueName();
                var failedType = errorContext.Exception.Message.Replace(MissingHandlerMessage, string.Empty).Trim();

                if (errorContext.Message.Headers.TryGetValue("NServiceBus.EnclosedMessageTypes", out string header) &&
                    header.Split(',').Any(x => x.Equals(failedType)))
                {
                    _connectionManager.EnqueueChannelOp(x =>
                    {
                        _logger?.LogInformation($"Stale Queue/Exchange Binding Detected: queue={queue} exchange={failedType}.");

                        x.ExchangeUnbind(queue, failedType, string.Empty, null);

                        _logger?.LogInformation($"Queue/Exchange Binding Removed: queue={queue} exchange={failedType}.");
                    });
                }
            });
        }

        private static readonly string MissingHandlerMessage = "No handlers could be found for message type: ";

        private static bool IsMissingHandler(ErrorContext errorContext)
        {
            return errorContext.Exception != null &&
                   errorContext.Exception.GetType() == typeof(InvalidOperationException) &&
                   errorContext.Exception.Message.StartsWith(MissingHandlerMessage);
        }

        private static bool IsUnrecoverable(RecoverabilityConfig config, ErrorContext errorContext)
        {
            return errorContext.Exception is MessageFailureException ||
                config.Failed.UnrecoverableExceptionTypes.Any(e => e.IsInstanceOfType(errorContext.Exception));
        }
    }
}
