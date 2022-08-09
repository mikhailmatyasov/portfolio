using Microsoft.Extensions.Logging;
using NServiceBus;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    public interface ITopologyRecoveryService : IReceiveBusStartupService
    {
        void QueueDeclared(Queue queue);

        void ExchangeDeclared(Exchange exchange);

        void QueueBound(QueueBinding queueBinding);

        void ExchangeBound(ExchangeBinding exchangeBinding);

        IReadOnlyDictionary<string, Queue> DeclaredQueues { get; }

        Task StartTopologyMonitorAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    public class TopologyRecoveryService : ITopologyRecoveryService
    {
#if NET452
        public IReadOnlyDictionary<string, Queue> DeclaredQueues => _declaredQueues.ToDictionary(x => x.Key, y => y.Value);
#else
        public IReadOnlyDictionary<string, Queue> DeclaredQueues => _declaredQueues;
#endif

        public int TaskDelayMs { get; set; } = 10000;

        private readonly ILogger<IBus> _logger;
        private readonly INamingConventions _namingConventions;
        private readonly IConnectionManager _connectionManager;
        private readonly IConfigurationProvider<RabbitMQTransport, IConfiguration> _configurationBuilder;

        private readonly ConcurrentDictionary<string, Queue> _declaredQueues;
        private readonly ConcurrentDictionary<string, Exchange> _declaredExchanges;
        private readonly ConcurrentBag<QueueBinding> _queueBindings;
        private readonly ConcurrentBag<ExchangeBinding> _exchangeBindings;

        public TopologyRecoveryService(
            ILogger<IBus> logger,
            INamingConventions namingConventions,
            IConnectionManager connectionManager,
            IConfigurationProvider<RabbitMQTransport, IConfiguration> configurationBuilder)
        {
            _logger = logger;
            _namingConventions = namingConventions;
            _connectionManager = connectionManager;
            _configurationBuilder = configurationBuilder;

            _declaredQueues = new ConcurrentDictionary<string, Queue>();
            _declaredExchanges = new ConcurrentDictionary<string, Exchange>();
            _queueBindings = new ConcurrentBag<QueueBinding>();
            _exchangeBindings = new ConcurrentBag<ExchangeBinding>();
        }

        public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                _logger.LogInformation($"{nameof(ITopologyRecoveryService)} Started.");

                await StartTopologyMonitorAsync();

                _logger.LogInformation($"{nameof(ITopologyRecoveryService)} Complete.");
            }
            catch (Exception ex)
            {
                _logger.LogError(default(EventId), ex, $"{nameof(ITopologyRecoveryService)} failed: ${ex.Message}");
            }
        }

        public void QueueDeclared(Queue queue)
        {
            _declaredQueues[queue.Name] = queue;
        }

        public void ExchangeDeclared(Exchange exchange)
        {
            _declaredExchanges[exchange.Name] = exchange;
        }

        public void QueueBound(QueueBinding queueBinding)
        {
            _queueBindings.Add(queueBinding);
        }

        public void ExchangeBound(ExchangeBinding exchangeBinding)
        {
            _exchangeBindings.Add(exchangeBinding);
        }

        public async Task StartTopologyMonitorAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var config = await _configurationBuilder.GetAdvancedConfigurationAsync();

            if (config.TopologyRecoveryDisabled)
            {
                return;
            }

            SetDelayTopology();

            var channel = await _connectionManager.CreateChannelAsync();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TaskDelayMs, cancellationToken);
                }
                catch (TaskCanceledException) { }

                try
                {
                    _logger.LogTrace($"{nameof(ITopologyRecoveryService)}: ensuring topology established...");

                    foreach (var queue in _declaredQueues.Values)
                    {
                        channel.QueueDeclare(queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.Arguments);
                    }

                    foreach (var exchange in _declaredExchanges.Values)
                    {
                        channel.ExchangeDeclare(exchange.Name, exchange.ExchangeType, exchange.Durable, exchange.AutoDelete);
                    }

                    foreach (var queueBinding in _queueBindings)
                    {
                        channel.QueueBind(queueBinding.QueueName, queueBinding.ExchangeName, queueBinding.RoutingKey);
                    }

                    foreach (var exchangeBinding in _exchangeBindings.Where(x => _declaredExchanges.ContainsKey(x.Destination) && _declaredExchanges.ContainsKey(x.Source)))
                    {
                        channel.ExchangeBind(exchangeBinding.Destination, exchangeBinding.Source, exchangeBinding.RoutingKey);
                    }

                    _logger.LogTrace($"{nameof(ITopologyRecoveryService)}: topology established.");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(default(EventId), ex, $"{nameof(ITopologyRecoveryService)} Failed. Reestablishing connection...");

                    channel.Close();
                    channel.Dispose();

                    channel = await _connectionManager.CreateChannelAsync();
                }
            }

            channel.Close();
            channel.Dispose();            
        }

        private void SetDelayTopology()
        {
            DelayTopology.GetExchanges().ForEach(x => _declaredExchanges[x.Name] = x);
            DelayTopology.GetQueues().ForEach(x => _declaredQueues[x.Name] = x);
            DelayTopology.GetExchangeBindings().ForEach(x => _exchangeBindings.Add(x));
            DelayTopology.GetQueueBindings().ForEach(x => _queueBindings.Add(x));
        }
    }
}
