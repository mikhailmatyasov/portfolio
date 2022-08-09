using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a bus used to publish messages.
    /// </summary>
    public interface ISendBus : IRestartableBus
    {
        /// <summary>
        /// Publishes a message on the bus.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="message">The event object instance.</param>
        /// <param name="publishOptions">The publish options.</param>
        /// <returns></returns>
        Task PublishAsync<TEvent>(TEvent message, PublishOptions publishOptions = null) where TEvent : IEvent;

        /// <summary>
        /// Sends a command on the bus.
        /// </summary>
        /// <typeparam name="TCommand">The command type.</typeparam>
        /// <param name="command">The command instance.</param>
        /// <param name="sendOptions">The options to apply to the send.</param>
        Task SendAsync<TCommand>(TCommand command, SendOptions sendOptions) where TCommand : ICommand;

        /// <summary>
        /// Invokes the registered command handler.
        /// </summary>
        /// <param name="command">The command instance.</param>
        /// <remarks>This is an inline operation - the pipeline will not be used.</remarks>
        Task HandleAsync(ICommand command);

        /// <summary>
        /// Invokes the registered command handler and returns the response.
        /// </summary>
        /// <param name="command">The command instance.</param>
        /// <remarks>This is an inline operation - the pipeline will not be used.</remarks>
        Task<TResponse> HandleAsync<TResponse>(ICommand command);
    }

    public class SendBus : BusBase, ISendBus
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ISendBusRecoveryManager _publishRecoveryManager;

        public SendBus(
            ILogger<IBus> logger,
            ICommandHandler commandHandler,
            IEndpointManager endpointManager,
            IExceptionHandler exceptionHandler,
            IBusRecoveryManager recoveryManager,
            IConfigurationProvider configurationProvider,
            ISendBusRecoveryManager publishRecoveryManager,
            IEnumerable<ISendBusStartupService> startupServices)
            : base(EndpointData.SendOnly(), logger, endpointManager, recoveryManager, startupServices)
        {
            _commandHandler = commandHandler;
            _exceptionHandler = exceptionHandler;
            _configurationProvider = configurationProvider;
            _publishRecoveryManager = publishRecoveryManager;
        }

        public async Task PublishAsync<TEvent>(TEvent message, PublishOptions publishOptions = null) where TEvent : IEvent
        {
            await InvokeAsync(() => EndpointInstance.Publish(message, publishOptions ?? new PublishOptions()), nameof(PublishAsync), message);
        }

        public async Task SendAsync<TCommand>(TCommand command, SendOptions sendOptions) where TCommand : ICommand
        {
            await InvokeAsync(() => EndpointInstance.Send(command, sendOptions), nameof(SendAsync), command);
        }

        public async Task HandleAsync(ICommand command)
        {
            await _commandHandler.HandleAsync(EndpointId, command);
        }

        public async Task<TResponse> HandleAsync<TResponse>(ICommand command)
        {
            return await _commandHandler.HandleAsync<TResponse>(EndpointId, command);
        }

        public async Task InvokeAsync<TMessage>(
            Func<Task> invoke,
            string actionName,
            TMessage message) where TMessage : IMessage
        {
            ThrowIfDisposed();

            var config = await _configurationProvider.GetConfigurationAsync();

            await WaitForRecoveryAsync(config);

            await TryInvokeAsync(invoke, actionName, message, config);
        }

        private async Task TryInvokeAsync<TMessage>(
            Func<Task> invoke,
            string actionName,
            TMessage message,
            INServiceBusConfiguration config,
            bool attemptRecovery = true) where TMessage : IMessage
        {
            try
            {
                await invoke();
            }
            catch (Exception ex)
            {
                if (!attemptRecovery || !ShouldRecover(message, config, ex))
                {
                    if (EndpointInstance == null)
                    {
                        Logger.LogError($"{actionName} failed for message {typeof(TMessage).Name}. Endpoint not connected.");
                    }
                    else
                    {
                        Logger.LogError(default(EventId), ex, $"{actionName} failed for message {typeof(TMessage).Name}.");
                    }

                    return;
                }

                _publishRecoveryManager.TryStartRecovery(EndpointId);

                if (EndpointInstance == null)
                {
                    Logger.LogWarning($"{actionName} failed for message {typeof(TMessage).Name}. Endpoint not connected. Recovery in progress.");
                }
                else
                {
                    Logger.LogWarning(default(EventId), ex, $"{actionName} failed for message {typeof(TMessage).Name}. Recovery in progress.");
                }

                await WaitForRecoveryAsync(config);

                await TryInvokeAsync(invoke, actionName, message, config, false);
            }
        }

        private async Task WaitForRecoveryAsync(INServiceBusConfiguration config)
        {
            var timeoutMs = config.PublishRecoveryTimeoutMs.GetValueOrDefault();

            await _publishRecoveryManager.WaitForRecoveryAsync(EndpointId, timeoutMs);
        }

        private bool ShouldRecover(object message, INServiceBusConfiguration config, Exception exception)
        {
            if (EndpointInstance == null)
            {
                return true;
            }

            if (config.PublishRecoveryDisabled)
            {
                return false;
            }

            return _exceptionHandler.ShouldRecoverOnPublishException(message.GetType(), exception);
        }
    }
}