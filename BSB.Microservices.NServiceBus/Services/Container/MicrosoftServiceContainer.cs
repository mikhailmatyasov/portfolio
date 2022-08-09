using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.Common;
using NServiceBus.Unicast;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// An implementation of <see cref="IServiceContainer"/> using the registered <see cref="IServiceCollection"/> and <see cref="IServiceProvider"/>.
    /// </summary>
    public class MicrosoftServiceContainer : EndpointContainer, IServiceContainer
    {
        private IServiceProvider ServiceProvider { get; set; }

        private readonly object _lock = new { };
        private readonly IServiceCollection _services;

        public MicrosoftServiceContainer(IServiceCollection services, IServiceProvider serviceProvider)
        {
            _services = services;
            ServiceProvider = serviceProvider ?? _services.BuildServiceProvider();
        }

        public void SetEndpointContext(EndpointData endpoint)
        {
            Endpoint = endpoint;
        }

        public TService GetRequiredService<TService>()
        {
            return ServiceProvider.GetRequiredService<TService>();
        }

        public object GetRequiredService(Type serviceType)
        {
            return ServiceProvider.GetRequiredService(serviceType);
        }

        public override bool HasService(Type type)
        {
            return ServiceProvider.GetService(type) != null;
        }

        public override object BuildService(Type type)
        {
            return ServiceProvider.GetService(type);
        }

        public override IEnumerable<object> BuildServices(Type type)
        {
            return ServiceProvider.GetServices(type);
        }

        public IContainer BuildChildContainer()
        {
            return new MicrosoftServiceContainer(_services, ServiceProvider.CreateScope().ServiceProvider)
            {
                Endpoint = Endpoint
            };
        }

        public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
        {
            lock (_lock)
            {
                if (dependencyLifecycle == DependencyLifecycle.SingleInstance)
                {
                    _services.AddSingleton(component);
                }
                else if (dependencyLifecycle == DependencyLifecycle.InstancePerUnitOfWork)
                {
                    _services.AddScoped(component);
                }
                else
                {
                    _services.AddTransient(component);
                }

                ServiceProvider = _services.BuildServiceProvider();
            }
        }

        public void Configure<T>(Func<T> component, DependencyLifecycle dependencyLifecycle)
        {
            lock (_lock)
            {
                if (dependencyLifecycle == DependencyLifecycle.SingleInstance)
                {
                    _services.AddSingleton(typeof(T), p => component());
                }
                else if (dependencyLifecycle == DependencyLifecycle.InstancePerUnitOfWork)
                {
                    _services.AddScoped(typeof(T), p => component());
                }
                else
                {
                    _services.AddTransient(typeof(T), p => component());
                }

                ServiceProvider = _services.BuildServiceProvider();
            }
        }

        public override void RegisterSingletonService(Type type, object instance)
        {
            lock (_lock)
            {
                _services.AddSingleton(type, instance);
                ServiceProvider = _services.BuildServiceProvider();
            }
        }

        public override void ReleaseService(object instance)
        {
            lock (_lock)
            {
                var serviceDescriptor = _services.FirstOrDefault(x => x.ImplementationInstance == instance);

                if (serviceDescriptor != null)
                {
                    _services.Remove(serviceDescriptor);
                    ServiceProvider = _services.BuildServiceProvider();
                }
            }
        }

        public void Dispose()
        {
            _services?.Clear();
            ServiceProvider = null;
        }
    }
}
