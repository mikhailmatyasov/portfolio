using NServiceBus.Unicast;
using System.Collections.Concurrent;

namespace BSB.Microservices.NServiceBus
{
    public class MessageHandlerRegistryContainer
    {
        private readonly ConcurrentDictionary<string, MessageHandlerRegistry> _registries;

        public MessageHandlerRegistryContainer()
        {
            _registries = new ConcurrentDictionary<string, MessageHandlerRegistry>();
        }

        public void Add(EndpointData endpoint, MessageHandlerRegistry instance)
        {
            _registries[GetEndpointKey(endpoint)] = instance;
        }

        public MessageHandlerRegistry Get(EndpointData endpoint)
        {
            return _registries.TryGetValue(GetEndpointKey(endpoint), out MessageHandlerRegistry registry)
                ? registry
                : null;
        }

        public void Delete(EndpointData endpoint)
        {
            _registries[GetEndpointKey(endpoint)] = null;
        }

        private string GetEndpointKey(EndpointData endpointData)
        {
            return $"{endpointData.Type}:{endpointData.Suffix ?? string.Empty}:{endpointData.TopicPattern ?? string.Empty}";
        }
    }
}
