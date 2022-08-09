using BSB.Microservices.NServiceBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using System.Runtime.CompilerServices;
using IEncryptionService = NServiceBus.Encryption.MessageProperty.IEncryptionService;
using System.Linq;
using BSB.Microservices.Vault;
using Microsoft.AspNetCore.Hosting;
using BSB.Microservices.Consul;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

[assembly: InternalsVisibleTo("Test.BSB.Microservices.NServiceBus")]
namespace BSB.Microservices.NServiceBus.Internal
{
    internal class RabbitMQServiceRegistration : IServiceRegistration<RabbitMQTransport, IConfiguration>
    {
        public void RegisterServices(IServiceCollection services)
        {
            RegisterServicesImpl(services);            
        }

        public void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            RegisterServicesImpl(services, configuration);            
        }

        private void RegisterServicesImpl(IServiceCollection services, IConfiguration configuration = null)
        {
            services
                .AddHttpClient()
                .AddSingleton(services)
                .AddLogging(x => x.AddConsole())
                .AddSingleton<ISendBus, SendBus>()
                .AddSingleton<IReceiveBus, ReceiveBus>()
                .AddSingleton<IRetryPolicy, RetryPolicy>()
                .AddSingleton<IRetryHandler, RetryHandler>()
                .AddSingleton<ICommandHandler, CommandHandler>()
                .AddSingleton<IEndpointBuilder, EndpointBuilder>()
                .AddSingleton<IEndpointManager, EndpointManager>()
                .AddSingleton<IEndpointProvider, EndpointProvider>()
                .AddSingleton<IExceptionHandler, ExceptionHandler>()
                .AddSingleton<IConnectionManager, ConnectionManager>()
                .AddSingleton<INamingConventions, NamingConventions>()
                .AddSingleton<IEncryptionService, EncryptionService>()
                .AddSingleton<IBusRecoveryManager, BusRecoveryManager>()
                .AddSingleton<IQueueCleanupService, QueueCleanupService>()
                .AddSingleton<ICredentialsProvider, CredentialsProvider>()
                .AddSingleton<ICustomRoutingToplogy, CustomRoutingTopology>()
                .AddSingleton<IServiceContainer, MicrosoftServiceContainer>()
                .AddSingleton<IConfigurationProvider, ConfigurationProvider>()
                .AddSingleton<IEndpointNamingConventions, NamingConventions>()
                .AddSingleton<ISendBusRecoveryManager, SendBusRecoveryManager>()
                .AddSingleton<ITopologyRecoveryService, TopologyRecoveryService>()
                .AddSingleton<IServiceContainerProvider, ServiceContainerProvider>()
                .AddSingleton<IReceiveBusStartupService>(p => p.GetRequiredService<ITopologyRecoveryService>())
                .AddSingleton<IReceiveBusStartupService>(p => p.GetRequiredService<IQueueCleanupService>())
                .AddSingleton<IConfigurationProvider<RabbitMQTransport, IConfiguration>, ConfigurationProvider>();

            RegisterTopicBusDependencies(services);
            RegisterOptionalDependencies(services);

            if (configuration != null)
            {
                services.AddSingleton(configuration);
            }
            else
            {
                services.AddSingleton<IConfiguration, Configuration>();
            }
        }

        private static void RegisterTopicBusDependencies(IServiceCollection services)
        {
            services
                .AddSingleton<BusFactory>(p =>
                {
                    ITopicReceiveBus factory(string topic, string endpointSuffix) => new TopicReceiveBus(
                        topic,
                        endpointSuffix,
                        p.GetRequiredService<ILogger<IBus>>(),
                        p.GetRequiredService<IEndpointManager>(),
                        p.GetRequiredService<IBusRecoveryManager>(),
                        p.GetRequiredService<IEnumerable<IReceiveBusStartupService>>());

                    return factory;
                })
                .AddSingleton<ITopicBusContainer, TopicBusContainer>();
        }

        private static void RegisterOptionalDependencies(IServiceCollection services)
        {
            if (services.Any(x => x.ServiceType == typeof(IApplicationLifetime)))
            {
                services.AddSingleton<ICircuitBreaker, ApplicationCircuitBreaker>();
            }
            else
            {
                services.AddSingleton<ICircuitBreaker, NullCircuitBreaker>();
            }

            if (!services.Any(x => x.ServiceType == typeof(IVaultClient)))
            {
                services.AddSingleton<IVaultClient, NullVaultClient>();
            }

            if (!services.Any(x => x.ServiceType == typeof(ICachedConsulClient)))
            {
                services.AddSingleton<ICachedConsulClient, NullConsulClient>();
            }

            if (!services.Any(x => x.ServiceType == typeof(IFeatureToggleClient)))
            {
                services.AddSingleton<IFeatureToggleClient, NullFeatureToggleClient>();
            }
        }
    }
}
