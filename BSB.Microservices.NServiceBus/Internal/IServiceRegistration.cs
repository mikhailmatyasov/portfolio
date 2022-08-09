using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Transport;

namespace BSB.Microservices.NServiceBus.Internal
{
    internal interface IServiceRegistration<TTransport, TConfiguration>
        where TTransport : TransportDefinition
        where TConfiguration : INServiceBusConfiguration<TTransport>
    {
        void RegisterServices(IServiceCollection services);

        void RegisterServices(IServiceCollection services, TConfiguration configuration);
    }
}
