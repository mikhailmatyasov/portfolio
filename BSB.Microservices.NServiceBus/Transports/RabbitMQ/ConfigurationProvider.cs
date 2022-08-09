using System.Threading.Tasks;
using BSB.Microservices.Consul;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus;

namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    public class ConfigurationProvider : IConfigurationProvider<RabbitMQTransport, IConfiguration>
    {
        private readonly ILogger<IBus> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICachedConsulClient _consulClient;
        private readonly ExecutionEnvironment _environment;
        private readonly IServiceIdentity _serviceIdentity;
        private readonly ICredentialsProvider _credentialsProvider;

        public ConfigurationProvider(
            ILogger<IBus> logger, 
            IConfiguration configuration,
            ICachedConsulClient consulClient,
            ICredentialsProvider credentialsProvider,
            ExecutionEnvironment environment,
            IServiceIdentity serviceIdentity)
        {
            _logger = logger;
            _configuration = configuration;
            _consulClient = consulClient;
            _environment = environment;
            _serviceIdentity = serviceIdentity;
            _credentialsProvider = credentialsProvider;
        }

        public async Task<INServiceBusConfiguration> GetConfigurationAsync()
        {
            return await GetAdvancedConfigurationAsync();
        }

        public async Task<IConfiguration> GetAdvancedConfigurationAsync()
        {
            if (_configuration.UseConsul)
            {
                await ConfigureDefaultConsul();
            }
            else
            {
                ConfigureDefaultLocalhost();
            }

            var credentials = await _credentialsProvider.GetAsync(_configuration);

            _configuration.Username = credentials.Username;
            _configuration.Password = credentials.Password;

            ConfigureDefaultValues();

            _logger.LogDebug($"RabbitMQ Host Configured: {_configuration.Host}");

            return _configuration;
        }

        private async Task ConfigureDefaultConsul()
        {
            var consulConfig = await _consulClient.GetRabbitConfigurationAsync(_serviceIdentity);

            if (_configuration.Host == null && consulConfig.TryGetValue("host", out string host))
            {
                _configuration.Host = host;
            }

            if (
                _configuration.PrefetchCount == null &&
                consulConfig.TryGetValue("prefetchcount", out string prefetchcountVal)
                && ushort.TryParse(prefetchcountVal, out ushort prefetchcount))
            {
                _configuration.PrefetchCount = prefetchcount;
            }

            if (consulConfig.TryGetValue("enableAuditing", out string enableAuditing))
            {
                _configuration.EnableAuditing = StringToBool(enableAuditing, false);
            }
            if (consulConfig.TryGetValue("enableHeartbeats", out string enableHeartbeats))
            {
                _configuration.EnableHeartbeats = StringToBool(enableHeartbeats, false);
            }
            if (consulConfig.TryGetValue("enableMonitoring", out string enableMonitoring))
            {
                _configuration.EnableMonitoring = StringToBool(enableMonitoring, false);
            }
            if (consulConfig.TryGetValue("enableSqlPersistence", out string enableSqlPersistence))
            {
                _configuration.EnableSqlPersistence = StringToBool(enableSqlPersistence, false);
            }

            if (consulConfig.TryGetValue("customChecksTimeToLiveInSeconds", out string customChecksTimeToLiveInSeconds))
            {
                _configuration.CustomChecksTimeToLiveInSeconds = StringToInt(customChecksTimeToLiveInSeconds, 10);
            }
            if (consulConfig.TryGetValue("heartbeatTimeToLiveInSeconds", out string heartbeatTimeToLiveInSeconds))
            {
                _configuration.HeartbeatTimeToLiveInSeconds = StringToInt(heartbeatTimeToLiveInSeconds, 10);
            }
            if (consulConfig.TryGetValue("sendHeartbeatIntervalInSeconds", out string sendHeartbeatIntervalInSeconds))
            {
                _configuration.SendHeartbeatIntervalInSeconds = StringToInt(sendHeartbeatIntervalInSeconds, 10);
            }
            if (consulConfig.TryGetValue("sendMetricDataIntervalInSeconds", out string sendMetricDataIntervalInSeconds))
            {
                _configuration.SendMetricDataIntervalInSeconds = StringToInt(sendMetricDataIntervalInSeconds, 10);
            }

            _logger?.LogDebug($"{JsonConvert.SerializeObject(_configuration, Formatting.Indented)}");
        }

        private bool StringToBool(string value, bool defaultValue)
        {
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            return defaultValue;
        }

        private int StringToInt(string value, int defaultValue)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return defaultValue;
        }

        private void ConfigureDefaultLocalhost()
        {
            _configuration.EnableAuditing = false;
            _configuration.EnableHeartbeats = false;
            _configuration.EnableMonitoring = false;
            _configuration.EnableSqlPersistence = false;
        }

        private void ConfigureDefaultValues()
        {
            _configuration.Host = _configuration.Host ?? "localhost";
            _configuration.PrefetchCount = _configuration.PrefetchCount ?? 50;
            _configuration.PublishRecoveryTimeoutMs = _configuration.PublishRecoveryTimeoutMs ?? 5000;
            _configuration.ReceiveEndpointRotationSeconds = _configuration.ReceiveEndpointRotationSeconds ?? 10;
            _configuration.RawLicenseConsulKey = _configuration.RawLicenseConsulKey ?? "nservicebus/license/raw";

            if (_configuration.UseLocalhostForDevelopment && _environment.IsLocal())
            {
                _configuration.Host = "localhost";
            }
        }
    }
}