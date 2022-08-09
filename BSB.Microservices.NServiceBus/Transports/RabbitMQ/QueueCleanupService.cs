using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    public interface IQueueCleanupService : IReceiveBusStartupService
    {
        Task DeleteUndeclaredQueuesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    public class QueueCleanupService : IQueueCleanupService
    {
        public IConnectionFactory ConnectionFactory { get; set; }

        private readonly ILogger<IBus> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly INamingConventions _namingConventions;
        private readonly ITopologyRecoveryService _topologyService;
        private readonly IConfigurationProvider<RabbitMQTransport, IConfiguration> _configurationBuilder;

        public QueueCleanupService(
             ILogger<IBus> logger,
             IHttpClientFactory httpClientFactory,
             INamingConventions namingConventions,
             ITopologyRecoveryService topologyService,
             IConfigurationProvider<RabbitMQTransport, IConfiguration> configurationBuilder)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _topologyService = topologyService;
            _namingConventions = namingConventions;
            _configurationBuilder = configurationBuilder;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                _logger.LogInformation($"{nameof(DeleteUndeclaredQueuesAsync)} Started.");

                await DeleteUndeclaredQueuesAsync();

                _logger.LogInformation($"{nameof(DeleteUndeclaredQueuesAsync)} Complete.");
            }
            catch (Exception ex)
            {
                _logger.LogError(default(EventId), ex, $"{nameof(DeleteUndeclaredQueuesAsync)} failed: ${ex.Message}.  ");
            }
        }

        public async Task DeleteUndeclaredQueuesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var config = await _configurationBuilder.GetAdvancedConfigurationAsync();

            if (!_namingConventions.IsDefaultQueueName || !config.DeleteUndeclaredQueuesOnStartEnabled)
            {
                return;
            }

            var declaredQueues = _topologyService.DeclaredQueues;

            if (declaredQueues == null || declaredQueues.Count == 0)
            {
                _logger.LogInformation($"No declared queues found.");
            }

            ConnectionFactory = ConnectionFactory ?? new ConnectionFactory
            {
                HostName = config.Host,
                UserName = config.Username,
                Password = config.Password
            };

            List<Queue> foundQueues;

            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{config.Username}:{config.Password}"));

            using(var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

                const string columns = "name,durable,auto_delete,consumers";

                var res = await httpClient.GetAsync($"http://{config.Host}:15672/api/queues/{HttpUtility.UrlEncode(ConnectionFactory.VirtualHost)}?columns={columns}");

                res.EnsureSuccessStatusCode();

                var resJson = await res.Content.ReadAsStringAsync();

                foundQueues = JsonConvert.DeserializeObject<List<Queue>>(resJson);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (foundQueues.Count == 0)
            {
                return;
            }

            var defaultQueueName = _namingConventions.GetQueueName();

            var queuesToDelete = foundQueues
                .Where(x => !x.AutoDelete)
                .Where(x => x.Consumers < 1)
                .Where(x => x.Name.StartsWith(defaultQueueName))
                .Where(x => !declaredQueues.ContainsKey(x.Name))
                .ToList();

            if (queuesToDelete.Count == 0)
            {
                return;
            }

            using (var connection = ConnectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                foreach (var queue in queuesToDelete)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    try
                    {
                        _logger.LogInformation($"Deleting Queue ({queue.Name}): {JsonConvert.SerializeObject(queue)}");

                        channel.QueueDelete(queue.Name, true, false);

                        _logger.LogInformation($"Queue Deleted ({queue.Name})");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(default(EventId), ex, $"Failed to delete queue {queue.Name} with error: {ex.Message}");
                    }
                }

                channel.Close();
                connection.Close();
            }
        }
    }
}
