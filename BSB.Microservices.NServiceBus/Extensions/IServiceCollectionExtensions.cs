using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.CustomChecks;

namespace BSB.Microservices.NServiceBus
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="ISendBus" /> and <see cref="IReceiveBus" /> services using the RabbitMQ Tranpsort
        /// </summary>
        /// <param name="services">The application service collection.</param>
        /// <param name="configure">The fluent configuration overrides.</param>
        /// <param name="logger">The optional configuration logger.</param>
        public static IServiceCollection AddNServiceBus<TTransport>(
            this IServiceCollection services,
            Action<FluentBusStartup> configure = null,
            ILogger logger = null) where TTransport : Transport.RabbitMQ
        {
            var busStartup = FluentBusStartup.Build(configure).Validate();

            return services
                .AddSingleton<IBusStartup>(busStartup.Startup)
                .AddSingleton(busStartup.ExecutionEnvironment)
                .AddNServiceBusInternal(busStartup.Startup, busStartup.ExecutionEnvironment, logger, null, busStartup.RabbitOverrides.RabbitConfiguration);
        }

        /// <summary>
        /// Adds the <see cref="ISendBus" /> and <see cref="IReceiveBus" /> services using the RabbitMQ Tranpsort
        /// </summary>
        /// <typeparam name="TTransport">The type of the transport.</typeparam>
        /// <typeparam name="TBusStartup">The type of the bus startup. The bus startup class is any class extending BusStartup.  You this class to configure NServiceBus.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="environment">The environment. (local | development | testing | staging | production).  Should come from IHostingEnvironment</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static IServiceCollection AddNServiceBus<TTransport, TBusStartup>(this IServiceCollection services,
            string environment,
            IConfiguration configuration = null,
            ILogger logger = null)

            where TTransport : Transport.RabbitMQ
            where TBusStartup : class, IBusStartup
        {
            var env = new ExecutionEnvironment(environment);
            services.AddSingleton(env);
            var busStartup = CreateAndRegisterStartupClass<TBusStartup>(services);

            return AddNServiceBusInternal(services, busStartup, env, logger, configuration);
        }

        /// <summary>
        /// Adds the <see cref="ISendBus" /> and <see cref="IReceiveBus" /> services using the RabbitMQ Tranpsort
        /// </summary>
        /// <typeparam name="TTransport">The type of the transport.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the bus services to.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="ecosystemName">Name of the ecosystem.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static IServiceCollection AddNServiceBus<TTransport>(this IServiceCollection services,
            string environment,
            string serviceName = null,
            string ecosystemName = null,
            IConfiguration configuration = null,
            ILogger logger = null)
            where TTransport : Transport.RabbitMQ
        {
            var env = new ExecutionEnvironment(environment);
            services.AddSingleton(env);
            var busStartup = CreateAndRegisterStartupClass(services, new DefaultBusStartup(serviceName, ecosystemName));

            return services.AddNServiceBusInternal(busStartup, env, logger, configuration);
        }

        /// <summary>
        /// Registers implementations of <see cref="IHandleMessages{T}" />
        /// </summary>
        /// <typeparam name="TBusStartup">The type of the assembly scan configuration.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the bus services to.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterImplementationsOfIHandleMessages<TBusStartup>(this IServiceCollection services, ILogger logger = null)
             where TBusStartup : class, IBusStartup
        {
            var busStartup = CreateAndRegisterStartupClass<TBusStartup>(services);

            return services.RegisterImplementationsOfIHandleMessages(busStartup, logger);
        }

        /// <summary>
        /// Registers the implementations of i handle messages.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="busStartup">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterImplementationsOfIHandleMessages(this IServiceCollection services, IBusStartup busStartup, ILogger logger = null)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().IncludeAssemblies(busStartup))
            {
                logger?.LogDebug($"Scanning assembly: {assembly.GetName().Name}");
                foreach (var type in assembly.GetLoadableTypes(logger).Select(t => t.GetTypeInfo()).Where(t => t.IsClass && !t.IsAbstract))
                {
                    var interfaces = type.ImplementedInterfaces.Select(i => i.GetTypeInfo()).ToList();

                    foreach (var interfaceType in interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleMessages<>)))
                    {
                        logger?.LogDebug($"Registering Type: {type.FullName}.  Interface:{interfaceType.FullName}");
                        services.AddTransient(interfaceType.AsType(), type.AsType());
                    }
                    foreach (var interfaceType in interfaces.Where(i => i.AsType() == typeof(ICustomCheck)))
                    {
                        logger?.LogDebug($"Registering Type: {type.FullName}.  Interface:{interfaceType.FullName}");
                        services.AddTransient(interfaceType.AsType(), type.AsType());
                    }
                }
            }

            return services;
        }

        private static IServiceCollection AddNServiceBusInternal(this IServiceCollection services,
            IBusStartup busStartup,
            ExecutionEnvironment executionEnvironment,
            ILogger logger = null,
            IConfiguration configuration = null,
            RabbitMQ.IConfiguration rabbitConfiguration = null)
        {
            if (string.IsNullOrEmpty(busStartup.ServiceName) && string.IsNullOrEmpty(busStartup.EcosystemName))
            {
                services.AddSingleton<IServiceIdentity>(new EntryAssemblyServiceIdentity());
            }
            else
            {
                services.AddSingleton<IServiceIdentity>(new ServiceIdentity(busStartup.ServiceName, busStartup.EcosystemName));
            }

            var config = rabbitConfiguration ?? new RabbitMQ.Configuration();

            busStartup.ConfigureRabbitMq(config, executionEnvironment);

            configuration?.GetSection("NServiceBus")?.Bind(config);

            return services
                .RegisterImplementationsOfIHandleMessages(busStartup, logger)
                .RegisterServices<RabbitMQTransport, RabbitMQ.IConfiguration>(config);
        }

        private static IBusStartup CreateAndRegisterStartupClass<TStartup>(IServiceCollection services) where TStartup : class, IBusStartup
        {
            services.AddSingleton<IBusStartup, TStartup>();

            var sp = services.BuildServiceProvider();
            var instance = sp.GetService<IBusStartup>();

            sp = null;

            return instance;
        }

        private static IBusStartup CreateAndRegisterStartupClass(IServiceCollection services, IBusStartup busStartup)
        {
            services.AddSingleton(busStartup);

            return busStartup;
        }
    }
}