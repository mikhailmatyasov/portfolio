using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BSB.Microservices.Consul;
using BSB.Microservices.NServiceBus.Attributes;
using BSB.Microservices.NServiceBus.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Encryption.MessageProperty;
using NServiceBus.MessageMutator;
using NServiceBus.Persistence.Sql;
using IEncryptionService = NServiceBus.Encryption.MessageProperty.IEncryptionService;

namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    public class EndpointBuilder : IEndpointBuilder
    {
        private readonly ILogger<IBus> _logger;
        private readonly IRetryPolicy _retryPolicy;
        private readonly ICachedConsulClient _consulClient;
        private readonly INamingConventions _namingConventions;
        private readonly IEncryptionService _encryptionService;
        private readonly ICustomRoutingToplogy _routingTopology;
        private readonly IServiceContainerProvider _serviceContainerProvider;
        private readonly IConfigurationProvider<RabbitMQTransport, IConfiguration> _configurationBuilder;
        private readonly IServiceIdentity _serviceIdentity;
        private readonly IBusStartup _busStartup;
        private readonly ExecutionEnvironment _executionEnvironment;

        public EndpointBuilder(
            ILogger<IBus> logger,
            IRetryPolicy retryPolicy,
            ICachedConsulClient consulClient,
            INamingConventions namingConventions,
            IEncryptionService encryptionService,
            ICustomRoutingToplogy routingTopology,
            IServiceContainerProvider serviceContainerProvider,
            IConfigurationProvider<RabbitMQTransport, IConfiguration> configurationBuilder,
            IServiceIdentity serviceIdentity,
            IBusStartup assemblyScanningOptions,
            ExecutionEnvironment executionEnvironment)
        {
            _logger = logger;
            _retryPolicy = retryPolicy;
            _consulClient = consulClient;
            _routingTopology = routingTopology;
            _namingConventions = namingConventions;
            _encryptionService = encryptionService;
            _serviceContainerProvider = serviceContainerProvider;
            _configurationBuilder = configurationBuilder;
            _serviceIdentity = serviceIdentity;
            _busStartup = assemblyScanningOptions;
            _executionEnvironment = executionEnvironment;
        }

        public async Task<EndpointConfiguration> BuildEndpointAsync(EndpointData endpoint)
        {
            var configuration = await _configurationBuilder.GetAdvancedConfigurationAsync();

            var endpointName = GetEndpointName(endpoint);

            var endpointConfig = new EndpointConfiguration(endpointName);

            ConfigureContainer(endpoint, endpointConfig);

            await ConfigureEndpointAsync(configuration, endpointConfig, endpoint);

            var transport = endpointConfig.UseTransport<RabbitMQTransport>();

            ConfigureTransport(configuration, transport, endpoint);

            RegisterExplicitHandlers(configuration, endpoint, endpointConfig);

            ConfigureOverrides(configuration, endpointConfig, transport);

            _busStartup.ConfigureEndpoint(endpointConfig, _executionEnvironment);

            return endpointConfig;
        }

        private void ConfigureContainer(EndpointData endpoint, EndpointConfiguration endpointConfig)
        {
            var container = _serviceContainerProvider.Get().BuildChildContainer() as IServiceContainer;

            container.SetEndpointContext(endpoint);

            endpointConfig.UseContainer(container);
        }

        private async Task ConfigureEndpointAsync(INServiceBusConfiguration config, EndpointConfiguration endpointConfig, EndpointData endpoint)
        {
            if (endpoint.Type == EndpointType.SendOnly)
            {
                endpointConfig.SendOnly();
            }

            endpointConfig.EnableInstallers();
            ConfigureAssemblyScanning(endpointConfig);
            endpointConfig.SendFailedMessagesTo(_namingConventions.GetErrorQueueName());
            ConfigureEndpointSerializers(endpointConfig, endpoint.Type);

            if (_busStartup.LimitMessageProcessingConcurrencyTo != null)
            {
                endpointConfig.LimitMessageProcessingConcurrencyTo(_busStartup.LimitMessageProcessingConcurrencyTo.Value);
            }

            _busStartup.PipelineConfigurator?.Invoke(endpointConfig.Pipeline);
            endpointConfig.Recoverability()
                .Delayed(x => x.NumberOfRetries(0))
                .Immediate(x => x.NumberOfRetries(0))
                .AddUnrecoverableException<MessageFailureException>()
                .CustomPolicy((c, e) => _retryPolicy.Invoke(c, e));

            endpointConfig.CustomDiagnosticsWriter(x => Task.Run(() => _logger.LogDebug(x)));
            endpointConfig.EnableMessagePropertyEncryption(_encryptionService);

            if (!string.IsNullOrEmpty(config.RawLicenseConsulKey))
            {
                var licenseText = await _consulClient.GetValueAsync(config.RawLicenseConsulKey);

                if (!string.IsNullOrEmpty(licenseText))
                {
                    endpointConfig.License(licenseText);
                }
            }

            ConfigureServiceControl(config, endpointConfig, endpoint.Type);
        }

        private void ConfigureAssemblyScanning(EndpointConfiguration endpointConfig)
        {
            var assemblyScanner = endpointConfig.AssemblyScanner();

            if (_busStartup?.AssembliesToInclude?.Invoke(new AssemblyPredicateBuilder())?.Patterns?.Any() ?? false)
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                var exclusions = _busStartup.BuildAssemblyScanExclusion(baseDirectory);
                if (exclusions?.Any() == true)
                {
                    assemblyScanner.ExcludeAssemblies(exclusions.ToArray());
                }
                assemblyScanner.ThrowExceptions = _busStartup.ThrowExceptions;
                assemblyScanner.ScanAssembliesInNestedDirectories = _busStartup.ScanAssembliesInNestedDirectories;
                assemblyScanner.ScanAppDomainAssemblies = _busStartup.ScanAppDomainAssemblies;
            }
        }

        private void ConfigureServiceControl(INServiceBusConfiguration config, EndpointConfiguration endpointConfig, EndpointType endpointType)
        {
            if (_busStartup.DisableServiceControl)
            {
                _logger.LogWarning("ServiceControl is currently disabled.");
                return;
            }
            if (string.IsNullOrEmpty(_busStartup.EcosystemName))
            {
                _logger.LogWarning($"No Ecosystem name specified.  ServiceControl will be disabled.");
                return;
            }
            if (_busStartup.DisableServiceControlForLocalDevelopment == true && _executionEnvironment.IsLocal())
            {
                _logger.LogInformation($"Running in " + _executionEnvironment.Name + " mode.  ServiceControl will be disabled.");
                return;
            }

            if (config.EnableMonitoring && endpointType != EndpointType.SendOnly)
            {
                var metrics = endpointConfig.EnableMetrics();
                metrics.SendMetricDataToServiceControl(_namingConventions.GetSharedMonitoringQueueName(), TimeSpan.FromSeconds(config.SendMetricDataIntervalInSeconds));
            }

            if (config.EnableAuditing)
            {
                endpointConfig.AuditProcessedMessagesTo(_namingConventions.GetSharedAuditQueueName());
            }
            if (config.EnableHeartbeats)
            {
                endpointConfig.SendHeartbeatTo(_namingConventions.GetSharedServiceControlQueueName(), TimeSpan.FromSeconds(config.SendHeartbeatIntervalInSeconds), TimeSpan.FromSeconds(config.HeartbeatTimeToLiveInSeconds));
                endpointConfig.ReportCustomChecksTo(_namingConventions.GetSharedServiceControlQueueName(), TimeSpan.FromSeconds(config.CustomChecksTimeToLiveInSeconds));
            }
            if (config.EnableSqlPersistence)
            {
                var cs = _consulClient.GetPersistenceConnectionString(_serviceIdentity);

                if (string.IsNullOrEmpty(cs))
                {
                    _logger.LogWarning($"Unable to determine SqlPersistence Connection String. Will NOT enable persistence.");
                }
                else
                {
                    var persistence = endpointConfig.UsePersistence<SqlPersistence>();

                    persistence.SqlDialect<SqlDialect.MsSqlServer>();

                    persistence.ConnectionBuilder(connectionBuilder: () => new SqlConnection(cs));

                    persistence.SubscriptionSettings().CacheFor(TimeSpan.FromMinutes(1));
                }
            }
        }

        protected void ConfigureEndpointSerializers(EndpointConfiguration endpointConfig, EndpointType endpointType)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            var jsonSerializer = endpointConfig.UseSerialization<NewtonsoftSerializer>();
            jsonSerializer.Settings(jsonSettings);
            jsonSerializer.ContentTypeKey("NewtonsoftJson");
            jsonSerializer.ContentTypeKey("application/json");

            var jsonDeSerializer = endpointConfig.AddDeserializer<NewtonsoftSerializer>();
            jsonDeSerializer.Settings(jsonSettings);
            jsonDeSerializer.ContentTypeKey("NewtonsoftJson");
            jsonDeSerializer.ContentTypeKey("application/json");

            if (endpointType == EndpointType.SendOnly &&
                _busStartup.UseOutgoingPartialTypeRenaming)
            {
                endpointConfig.RegisterMessageMutator(new OutgoingMessageIdentityMutator(_busStartup, _logger));
            }
            else if (endpointType != EndpointType.SendOnly &&
                    _busStartup.UseIncomingPartialTypeResolution)
            {
                endpointConfig.RegisterMessageMutator(new IncomingMessageIdentityMutator(_busStartup, _logger));
            }

            if (_busStartup.UseOutgoingHeaderAttributes)
            {
                endpointConfig.RegisterMessageMutator(new OutgoingMessageHeaderMutator());
            }
        }

        private void ConfigureTransport(IConfiguration configuration, TransportExtensions<RabbitMQTransport> transport, EndpointData endpoint)
        {
            transport
                .UseDurableExchangesAndQueues(true)
                .PrefetchCount(configuration.PrefetchCount.Value)
                .ConnectionString(GetConnectionString(configuration))
                .UseRoutingTopology(useDurableExchangesAndQueues => _routingTopology.Create(useDurableExchangesAndQueues, endpoint));
        }

        private static void ConfigureOverrides(
            IConfiguration configuration,
            EndpointConfiguration endpointConfig,
            TransportExtensions<RabbitMQTransport> transport)
        {
            configuration.TransportOverrides?.Invoke(transport);
            configuration.EndpointOverrides?.Invoke(endpointConfig);
        }

        private static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = $"host={configuration.Host};username={configuration.Username};password={configuration.Password}";

#if DEBUG
            // NServiceBus circuit break triggers on breakpoints very quickly. Increase heartbeat for debugging.
            connectionString += ";RequestedHeartbeat=1000";
#endif
            return connectionString;
        }

        private void RegisterExplicitHandlers(IConfiguration configuration, EndpointData endpoint, EndpointConfiguration endpointConfig)
        {
            if (endpoint.Type == EndpointType.SendOnly || !_busStartup.UseAttributeSubscriptions)
            {
                return;
            }

            var handlerTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .IncludeAssemblies(_busStartup)
                .SelectMany(x => x.GetLoadableTypes())
                .Select(x => x.GetTypeInfo())
                .Where(x => x.IsClass && !x.IsAbstract)
                .Where(x =>
                    x.ImplementedInterfaces
                        .Select(i => i.GetTypeInfo())
                        .Where(i => i.IsGenericType)
                        .Where(i => i.GetGenericTypeDefinition() == typeof(IHandleMessages<>))
                        .Any()
                );

            var disabledHandlerTypes = handlerTypes
                .Where(x =>
                {
                    var subscriptionAttribute = x.GetCustomAttribute<SubscriptionAttribute>();

                    return subscriptionAttribute != null && !subscriptionAttribute.Enabled(GetEndpointName(endpoint));
                })
                .ToArray();

            if (disabledHandlerTypes.Length > 0)
            {
                endpointConfig.AssemblyScanner().ExcludeTypes(disabledHandlerTypes);
            }
        }

        private string GetEndpointName(EndpointData endpoint)
        {
            return string.IsNullOrEmpty(endpoint.Suffix)
                ? _namingConventions.GetQueueName()
                : $"{_namingConventions.GetQueueName()}{endpoint.Suffix}";
        }
    }
}