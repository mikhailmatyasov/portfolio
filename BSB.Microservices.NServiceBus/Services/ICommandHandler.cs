using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    public interface ICommandHandler
    {
        /// <summary>
        /// Invokes the registered command handler.
        /// </summary>
        /// <param name="command">The command instance.</param>
        /// <param name="endpointId">The endpoint identifier.</param>
        Task HandleAsync(Guid endpointId, ICommand command);

        /// <summary>
        /// Invokes the registered command handler and returns the response.
        /// </summary>
        /// <param name="command">The command instance.</param>
        /// <param name="endpointId">The endpoint identifier.</param>
        Task<TResponse> HandleAsync<TResponse>(Guid endpointId, ICommand command);
    }

    public class CommandHandler : ICommandHandler
    {
        private readonly ILogger<IBus> _logger;
        private readonly IConfigurationProvider _configProvider;
        private readonly IEndpointManager _endpointManager;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IServiceContainer _serviceContainer;
        private readonly ISendBusRecoveryManager _recoveryManager;
        private readonly IEndpointNamingConventions _namingConventions;

        public CommandHandler(
            ILogger<IBus> logger,
            IConfigurationProvider configProvider,
            IEndpointManager endpointManager,
            IExceptionHandler exceptionHandler,
            IServiceContainer serviceContainer,
            ISendBusRecoveryManager recoveryManager,
            IEndpointNamingConventions namingConventions)
        {
            _logger = logger;
            _configProvider = configProvider;
            _exceptionHandler = exceptionHandler;
            _endpointManager = endpointManager;
            _serviceContainer = serviceContainer;
            _recoveryManager = recoveryManager;
            _namingConventions = namingConventions;
        }

        public async Task HandleAsync(Guid endpointId, ICommand command)
        {
            await InvokeHandlerAsync(endpointId, command);
        }

        public async Task<TResponse> HandleAsync<TResponse>(Guid endpointId, ICommand command)
        {
            var response = await InvokeHandlerAsync(endpointId, command);

            return response == null
                ? default(TResponse)
                : (TResponse)response;
        }

        private async Task<object> InvokeHandlerAsync(Guid endpointId, ICommand command)
        {
            var handlerType = typeof(IHandleMessages<>).MakeGenericType(command.GetType());

            var handler = _serviceContainer.GetRequiredService(handlerType);

            InProcessContext context;

            if (endpointId != Guid.Empty)
            {
                var config = await _configProvider.GetConfigurationAsync();

                context = new InlineContext(
                    endpointId,
                    command,
                    _logger,
                    config,
                    _endpointManager,
                    _exceptionHandler,
                    _recoveryManager,
                    _namingConventions);
            }
            else
            {
                context = new InProcessContext();
            }
            
            var handle = handlerType
                .GetMethod(nameof(IHandleMessages<int>.Handle))
                .Invoke(handler, new object[] { command, context });

            await (Task)handle;

            return context.Response;
        }
    }
}
