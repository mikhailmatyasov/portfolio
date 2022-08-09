using BSB.Microservices.NServiceBus.Internal;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Transport;
using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test.BSB.Microservices.NServiceBus")]
namespace BSB.Microservices.NServiceBus
{
    internal static class InternalIServiceCollectionExtensions
    {
        internal static IServiceCollection RegisterServices<TTransport, TConfiguration>(this IServiceCollection services, TConfiguration configuration)
            where TTransport : TransportDefinition
            where TConfiguration : INServiceBusConfiguration<TTransport>
        {
            var serviceRegistration = GetServiceRegistration<TTransport, TConfiguration>();

            if(configuration == null)
            {
                serviceRegistration.RegisterServices(services);
            }
            else
            {
                serviceRegistration.RegisterServices(services, configuration);
            }

            return services;
        }

        internal static IServiceRegistration<TTransport, TConfiguration> GetServiceRegistration<TTransport, TConfiguration>()
            where TTransport : TransportDefinition
            where TConfiguration : INServiceBusConfiguration<TTransport>
        {
            var serviceRegistrationType = typeof(IBus).Assembly
                            .DefinedTypes
                            .Where(x => x.IsClass)
                            .Where(x => !x.IsAbstract)
                            .Where(x => typeof(IServiceRegistration<TTransport, TConfiguration>).IsAssignableFrom(x))
                            .FirstOrDefault();

            if (serviceRegistrationType == null)
            {
                throw new NotImplementedException($"Missing implentation for {typeof(IServiceRegistration<TTransport, TConfiguration>).FullName}");
            }

            var serviceRegistration = (IServiceRegistration<TTransport, TConfiguration>)Activator.CreateInstance(serviceRegistrationType);

            return serviceRegistration;
        }

#if NET452
        internal static IServiceCollection AddHttpClient(this IServiceCollection services)
        {
            return services.AddSingleton<IHttpClientFactory, HttpClientFactory>();
        }
#endif
    }
}
