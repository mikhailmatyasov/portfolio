using Autofac;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IContainer = NServiceBus.ObjectBuilder.Common.IContainer;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// An implementation of <see cref="IServiceContainer"/> using the registered <see cref="ILifetimeScope"/>.
    /// </summary>
    public class AutofacServiceContainer : EndpointContainer, IServiceContainer
    {
        private readonly ILifetimeScope _rootScope;

        private ILifetimeScope CurrentScope { get; set; }

        public AutofacServiceContainer(ILifetimeScope scope)
        {
            _rootScope = scope.BeginLifetimeScope();

            CurrentScope = _rootScope;
        }

        public void SetEndpointContext(EndpointData endpoint)
        {
            Endpoint = endpoint;
        }

        public TService GetRequiredService<TService>()
        {
            return (TService)Build(typeof(TService));
        }

        public object GetRequiredService(Type type)
        {
            return Build(type);
        }

        public override object BuildService(Type type)
        {
            return CurrentScope.Resolve(type);
        }

        public override IEnumerable<object> BuildServices(Type type)
        {
            return (IEnumerable<object>)ResolveAllMethodInfo.MakeGenericMethod(type).Invoke(this, new object[0]);
        }

        public static MethodInfo ResolveAllMethodInfo = typeof(AutofacServiceContainer).GetMethod(nameof(ResolveAll));

        public IEnumerable<object> ResolveAll<T>()
        {
            return CurrentScope.Resolve<IEnumerable<T>>().Cast<object>();
        }

        public IContainer BuildChildContainer()
        {
            return new AutofacServiceContainer(CurrentScope.BeginLifetimeScope())
            {
                Endpoint = Endpoint
            };
        }

        public void Configure(Type type, DependencyLifecycle dependencyLifecycle)
        {
            if (dependencyLifecycle == DependencyLifecycle.SingleInstance)
            {
                CurrentScope = CurrentScope.BeginLifetimeScope(builder => builder.RegisterType(type).SingleInstance());
            }
            else if (dependencyLifecycle == DependencyLifecycle.InstancePerUnitOfWork)
            {
                CurrentScope = CurrentScope.BeginLifetimeScope(builder => builder.RegisterType(type).InstancePerLifetimeScope());
            }
            else
            {
                CurrentScope = CurrentScope.BeginLifetimeScope(builder => builder.RegisterType(type).InstancePerDependency());
            }
        }

        public void Configure<T>(Func<T> component, DependencyLifecycle dependencyLifecycle)
        {
            if (dependencyLifecycle == DependencyLifecycle.SingleInstance)
            {
                CurrentScope = CurrentScope.BeginLifetimeScope(builder => builder.Register(x => component()).SingleInstance());
            }
            else if (dependencyLifecycle == DependencyLifecycle.InstancePerUnitOfWork)
            {
                CurrentScope = CurrentScope.BeginLifetimeScope(builder => builder.Register(x => component()).InstancePerLifetimeScope());
            }
            else
            {
                CurrentScope = CurrentScope.BeginLifetimeScope(builder => builder.Register(x => component()).InstancePerDependency());
            }
        }

        public override bool HasService(Type type)
        {
            return CurrentScope.IsRegistered(type);
        }

        public override void RegisterSingletonService(Type type, object instance)
        {
            CurrentScope = CurrentScope.BeginLifetimeScope(builder => builder.RegisterInstance(instance).As(type));
        }

        public override void ReleaseService(object instance)
        {
            if(instance == null)
            {
                return;
            }

            if(instance is IDisposable disposable)
            {
                disposable.Dispose();
                CurrentScope.Disposer.AddInstanceForDisposal(disposable);
            }
        }

        public void Dispose()
        {
            _rootScope.Dispose();
        }
    }
}
