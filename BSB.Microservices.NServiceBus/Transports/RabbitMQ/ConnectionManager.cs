using Microsoft.Extensions.Logging;
using NServiceBus;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    public interface IConnectionManager
    {
        Task<IModel> CreateChannelAsync();

        void EnqueueChannelOp(Action<IModel> operation);
    }

    public class ConnectionManager : IConnectionManager, IDisposable
    {
        public IConnection Connection { get; set; }

        public IConnectionFactory ConnectionFactory { get; set; }

        private Task ChannelOperator { get; set; }

        private readonly object _lock = new {};
        private readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1);
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ConcurrentQueue<Action<IModel>> _channelOps = new ConcurrentQueue<Action<IModel>>();

        private readonly ILogger<IBus> _logger;
        private readonly IConfigurationProvider<RabbitMQTransport, IConfiguration> _configurationProvider;

        public ConnectionManager(
            ILogger<IBus> logger,
            IConfigurationProvider<RabbitMQTransport, IConfiguration> configurationProvider)
        {
            _logger = logger;
            _configurationProvider = configurationProvider;
        }

        public async Task<IModel> CreateChannelAsync()
        {
            await EnsureConnectedAsync();

            return Connection.CreateModel();
        }

        public void EnqueueChannelOp(Action<IModel> operation)
        {
            _channelOps.Enqueue(operation);

            if(ChannelOperator == null || ChannelOperator.IsCompleted)
            {
                lock (_lock)
                {
                    ChannelOperator = Task.Factory.StartNew(async () =>
                    {
                        using(var channel = await CreateChannelAsync())
                        {
                            while (_channelOps.TryDequeue(out Action<IModel> action))
                            {
                                try
                                {
                                    action(channel);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"{nameof(ConnectionManager)} ChannelOp Failed.", ex);
                                }
                            }
                        }
                    }, _cancellationTokenSource.Token);
                }
            }
        }

        private async Task EnsureConnectedAsync()
        {
            await _asyncLock.WaitAsync(_cancellationTokenSource.Token);

            try
            {
                if (ConnectionFactory == null)
                {
                    var configuration = await _configurationProvider.GetAdvancedConfigurationAsync();

                    ConnectionFactory = new ConnectionFactory
                    {
                        HostName = configuration.Host,
                        UserName = configuration.Username,
                        Password = configuration.Password
                    };
                }

                if (Connection == null)
                {
                    Connection = ConnectionFactory.CreateConnection(nameof(ConnectionManager));
                }
                else if (!Connection.IsOpen)
                {
                    Connection.Close();
                    Connection.Dispose();

                    Connection = ConnectionFactory.CreateConnection(nameof(ConnectionManager));
                }
            }
            catch(Exception) { throw; }
            finally
            {
                _asyncLock.Release();
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();

            Connection?.Close();
            Connection?.Dispose();
        }
    }
}
