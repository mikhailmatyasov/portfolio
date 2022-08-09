using NServiceBus.Unicast;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSB.Microservices.NServiceBus
{
    public abstract class EndpointContainer
    {
        public abstract bool HasService(Type type);

        public abstract object BuildService(Type type);

        public abstract IEnumerable<object> BuildServices(Type type);

        public abstract void ReleaseService(object instance);

        public abstract void RegisterSingletonService(Type type, object instance);

        protected EndpointData Endpoint { get; set; }

        protected readonly object Lock = new { };

        public object Build(Type type)
        {
            if(type == typeof(MessageHandlerRegistry))
            {
                return GetMessageHandlerRegistryContainer()?.Get(Endpoint);
            }
            else
            {
                return BuildService(type);
            }
        }

        public IEnumerable<object> BuildAll(Type type)
        {
            if (type == typeof(MessageHandlerRegistry))
            {
                var registry = GetMessageHandlerRegistryContainer()?.Get(Endpoint);

                return registry == null
                    ? Enumerable.Empty<object>()
                    : new List<object> { registry };
            }
            else
            {
                return BuildServices(type);
            }
        }

        public void RegisterSingleton(Type type, object instance)
        {
            if (type == typeof(MessageHandlerRegistry))
            {
                AddMessageHandlerRegistry(instance);
            }
            else
            {
                RegisterSingletonService(type, instance);
            }
        }

        public bool HasComponent(Type type)
        {
            if(type == typeof(MessageHandlerRegistry))
            {
                var hasComponent =
                    HasService(typeof(MessageHandlerRegistryContainer)) &&
                    GetMessageHandlerRegistryContainer()?.Get(Endpoint) != null;

                if(Endpoint.Type == EndpointType.Default)
                {
                    var messageTypes = GetMessageHandlerRegistryContainer()?.Get(Endpoint)?.GetMessageTypes()?.ToList() ?? new List<Type>();
                }

                return hasComponent;
            }
            else
            {
                return HasService(type);
            }
        }

        public void Release(object instance)
        {
            if (instance.GetType() == typeof(MessageHandlerRegistry))
            {
                GetMessageHandlerRegistryContainer()?.Delete(Endpoint);
            }
            else
            {
                ReleaseService(instance);
            }
        }

        private void AddMessageHandlerRegistry(object instance)
        {
            var container = GetMessageHandlerRegistryContainer();

            if (container == null)
            {
                container = new MessageHandlerRegistryContainer();

                RegisterSingleton(typeof(MessageHandlerRegistryContainer), container);
            }

            (container as MessageHandlerRegistryContainer).Add(Endpoint, instance as MessageHandlerRegistry);
        }

        private MessageHandlerRegistryContainer GetMessageHandlerRegistryContainer() => HasService(typeof(MessageHandlerRegistryContainer))
            ? BuildService(typeof(MessageHandlerRegistryContainer)) as MessageHandlerRegistryContainer
            : null;
    }
}
