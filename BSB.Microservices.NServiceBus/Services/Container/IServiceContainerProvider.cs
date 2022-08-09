using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BSB.Microservices.NServiceBus
{
    public interface IServiceContainerProvider
    {
        IServiceContainer Get();
    }

    public class ServiceContainerProvider : IServiceContainerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceContainerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceContainer Get()
        {
            var lifetimeScope = _serviceProvider.GetService(typeof(ILifetimeScope));

            if(lifetimeScope != null)
            {
                return new AutofacServiceContainer((ILifetimeScope)lifetimeScope);
            }

            return _serviceProvider.GetRequiredService<IServiceContainer>();
        }
    }
}
