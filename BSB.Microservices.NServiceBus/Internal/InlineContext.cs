using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.Extensibility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Test.BSB.Microservices.NServiceBus")]
namespace BSB.Microservices.NServiceBus
{
    internal class InlineContext : InProcessContext
    {
        private readonly Guid _endpointId;
        private readonly IMessage _message;
        private readonly ILogger<IBus> _logger;
        private readonly INServiceBusConfiguration _config;
        private readonly IEndpointManager _endpointManager;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ISendBusRecoveryManager _recoveryManager;
        private readonly IEndpointNamingConventions _namingConventions;

        public InlineContext(
            Guid endpointId,
            IMessage message,
            ILogger<IBus> logger,
            INServiceBusConfiguration config,
            IEndpointManager endpointManager,
            IExceptionHandler exceptionHandler,
            ISendBusRecoveryManager recoveryManager,
            IEndpointNamingConventions namingConventions)
        {
            _endpointId = endpointId;
            _message = message;
            _logger = logger;
            _config = config;
            _exceptionHandler = exceptionHandler;
            _recoveryManager = recoveryManager;
            _endpointManager = endpointManager;
            _namingConventions = namingConventions;
        }

        public override async Task ForwardCurrentMessageTo(string destination)
        {
            await TryInvokeAsync(
               () => _endpointManager.Get(_endpointId).Send(destination, _message),
               nameof(ForwardCurrentMessageTo),
               _message.GetType(),
               _config);
        }

        public override async Task Publish(object message, PublishOptions options)
        {
            await TryInvokeAsync(
               () => _endpointManager.Get(_endpointId).Publish(message, options),
               nameof(Publish),
               message.GetType(),
               _config);
        }

        public override async Task Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions)
        {
            await TryInvokeAsync(
               () => _endpointManager.Get(_endpointId).Publish(messageConstructor, publishOptions),
               nameof(Publish),
               typeof(T),
               _config);
        }

        public override async Task Send(object message, SendOptions options)
        {
            await TryInvokeAsync(
               () => _endpointManager.Get(_endpointId).Send(message, options),
               nameof(Send),
               message.GetType(),
               _config);
        }

        public override async Task Send<T>(Action<T> messageConstructor, SendOptions options)
        {
            await TryInvokeAsync(
                () => _endpointManager.Get(_endpointId).Send(messageConstructor, options),
                nameof(Send),
                typeof(T),
                _config);
        }

        internal async Task TryInvokeAsync(
            Func<Task> invoke,
            string actionName,
            Type messageType,
            INServiceBusConfiguration config,
            bool attemptRecovery = true)
        {
            try
            {
                await invoke();
            }
            catch (Exception ex)
            {
                if (!attemptRecovery || !ShouldRecover(messageType, config, ex))
                {
                    _logger.LogError(default(EventId), ex, $"{actionName} failed for message {messageType.Name}.");

                    return;
                }

                _recoveryManager.TryStartRecovery(_endpointId);

                _logger.LogWarning(default(EventId), ex, $"{actionName} failed for message {messageType.Name}. Recovery in progress.");

                await WaitForRecoveryAsync(config);

                await TryInvokeAsync(invoke, actionName, messageType, config, false);
            }
        }

        private async Task WaitForRecoveryAsync(INServiceBusConfiguration config)
        {
            var timeoutMs = config.PublishRecoveryTimeoutMs.GetValueOrDefault();

            await _recoveryManager.WaitForRecoveryAsync(_endpointId, timeoutMs);
        }

        private bool ShouldRecover(Type messageType, INServiceBusConfiguration config, Exception exception)
        {
            if (config.PublishRecoveryDisabled)
            {
                return false;
            }

            return _exceptionHandler.ShouldRecoverOnPublishException(messageType, exception);
        }
    }
}
