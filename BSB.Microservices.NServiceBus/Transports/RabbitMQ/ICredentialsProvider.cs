using BSB.Microservices.Consul;
using BSB.Microservices.Vault;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public interface ICredentialsProvider
    {
        Task<Credentials> GetAsync(IConfiguration configuration);
    }

    public class CredentialsProvider : ICredentialsProvider
    {
        private const string DefaultUsername = "guest";
        private const string DefaultPassword = "guest";

        private readonly ILogger<IBus> _logger;
        private readonly IVaultClient _vaultClient;
        private readonly IServiceIdentity _serviceIdentity;
        private readonly ICachedConsulClient _consulClient;
        private readonly ExecutionEnvironment _environment;
        private readonly IFeatureToggleClient _featureToggleClient;

        public CredentialsProvider(
            ILogger<IBus> logger,
            IVaultClient vaultClient,
            IServiceIdentity serviceIdentity,
            ICachedConsulClient consulClient,
            ExecutionEnvironment environment,
            IFeatureToggleClient featureToggleClient)
        {
            _logger = logger;
            _vaultClient = vaultClient;
            _serviceIdentity = serviceIdentity;
            _consulClient = consulClient;
            _environment = environment;
            _featureToggleClient = featureToggleClient;
        }

        public async Task<Credentials> GetAsync(IConfiguration configuration)
        {
            if (configuration.UseLocalhostForDevelopment && _environment.IsLocal())
            {
                _logger.LogDebug("Default RabbitMQ Credentials Configured.");

                return new Credentials
                {
                    Username = DefaultUsername,
                    Password = DefaultPassword
                };
            }

            if (configuration.UseVaultCredentials)
            {
                return await GetVaultCredentialsAsync();
            }
            else if (configuration.UseConsul)
            {
                return await GetConsulCredentialsAsync();
            }

            _logger.LogDebug("Default RabbitMQ Credentials Configured.");

            return new Credentials
            {
                Username = DefaultUsername,
                Password = DefaultPassword
            };
        }

        private async Task<Credentials> GetVaultCredentialsAsync()
        {
            string username = null;
            string password = null;

            if (await _featureToggleClient.AnyAsync(FeatureToggle.Vault) && !await _featureToggleClient.EnabledAsync(FeatureToggle.Vault))
            {
                return await GetConsulCredentialsAsync();
            }

            var secretData = await _vaultClient.GetServiceSecretAsync();

            if (secretData.TryGetValue("rabbitmq-username", out object vaultUsername))
            {
                username = vaultUsername?.ToString();
            }

            if (secretData.TryGetValue("rabbitmq-password", out object vaultPassword))
            {
                password = vaultPassword?.ToString();
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                username = DefaultUsername;
            }
            else
            {
                _logger.LogDebug("RabbitMQ Username Configured from Vault.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                password = DefaultUsername;
            }
            else
            {
                _logger.LogDebug("RabbitMQ Password Configured from Vault.");
            }

            return new Credentials
            {
                Username = username,
                Password = password
            };
        }

        private async Task<Credentials> GetConsulCredentialsAsync()
        {
            var consulConfig = await _consulClient.GetRabbitConfigurationAsync(_serviceIdentity);

            string username = null;
            string password = null;

            if (consulConfig.TryGetValue("username", out string consulUsername))
            {
                username = consulUsername;
            }

            if (consulConfig.TryGetValue("password", out string consulPassword))
            {
                password = consulPassword;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                username = DefaultUsername;
            }
            else
            {
                _logger.LogDebug("RabbitMQ Username Configured from Consul.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                password = DefaultUsername;
            }
            else
            {
                _logger.LogDebug("RabbitMQ Password Configured from Consul.");
            }

            return new Credentials
            {
                Username = username,
                Password = password
            };
        }
    }
}
