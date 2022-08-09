using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using WeSafe.TelegramBot.Services;

namespace WeSafe.TelegramBot.NetCore
{
    public class TelegramHostService : BackgroundService
    {
        private readonly ILogger<TelegramHostService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _services;

        public TelegramHostService(IServiceProvider services, ILogger<TelegramHostService> logger,
            IConfiguration configuration)
        {
            _services = services;
            _logger = logger;
            _configuration = configuration;
        }

        private TelegramBotClient _bot;
        private bool _connected;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Telegram bot hosted service running.");

            return DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            await CreateAndConnect(stoppingToken);

            if ( stoppingToken.IsCancellationRequested ) return;

            var updateReceiver = new QueuedUpdateReceiver(_bot);

            updateReceiver.StartReceiving(cancellationToken: stoppingToken, errorHandler: HandleErrorAsync);

            await foreach ( var update in updateReceiver.YieldUpdatesAsync().WithCancellation(stoppingToken) )
            {
                using var scope = _services.CreateScope();

                var handler = scope.ServiceProvider.GetRequiredService<ITelegramHandleUpdate>();

                try
                {
                    await handler.HandleUpdateAsync(updateReceiver.BotClient, update, stoppingToken);
                }
                catch ( OperationCanceledException )
                {
                    // Do nothing
                }
                catch ( Exception e )
                {
                    _logger.LogError(e, "Unhandled exception in HandleUpdateAsync: {Error} Id:{UpdateId} Type:{UpdateType}",
                        e.Message, update.Id, update.Type);
                }

                /*if ( update.Message != null )
                {
                    await _bot.SendTextMessageAsync(
                        update.Message.Chat,
                        $"Still have to process {updateReceiver.PendingUpdates} updates",
                        cancellationToken: stoppingToken);
                }*/
            }
        }

        private Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
        {
            if ( exception is ApiRequestException apiRequestException )
            {
                _logger.LogError(exception, "Telegram bot api request error: {Error} {Params}", apiRequestException.Message,
                    apiRequestException.Parameters);
            }
            else
            {
                _logger.LogError(exception, "Telegram bot api request error: {Error}", exception.Message);
            }

            return Task.CompletedTask;
        }

        private async Task CreateAndConnect(CancellationToken stoppingToken)
        {
            _connected = false;

            while ( !stoppingToken.IsCancellationRequested && !_connected )
            {
                var token = _configuration["Token"];

                try
                {
                    _logger.LogInformation($"Telegram bot is connecting...");

                    _bot = new TelegramBotClient(token);

                    var me = await _bot.GetMeAsync(stoppingToken);

                    _logger.LogInformation("Telegram bot start listening for @{Username}", me.Username);

                    _connected = true;
                }
                catch ( OperationCanceledException )
                {
                    // Do nothing
                }
                catch ( Exception e )
                {
                    _logger.LogError(e, "Could not connect to telegram bot api.");

                    await Task.Delay(5000, stoppingToken); // Do delay and retry
                }
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Telegram bot hosted service is stopping.");

            return base.StopAsync(stoppingToken);
        }
    }
}