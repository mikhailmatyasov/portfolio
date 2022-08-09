using System.Collections.Generic;

namespace BSB.Microservices.NServiceBus
{
    public interface ITopicBusContainer
    {
        ITopicReceiveBus GetOrAdd(string topic, string endpointSuffix);
    }

    public delegate ITopicReceiveBus BusFactory(string topic, string endpointSuffix);

    public class TopicBusContainer : ITopicBusContainer
    {
        private readonly BusFactory _busFactory;

        public TopicBusContainer(BusFactory busFactory)
        {
            _busFactory = busFactory;
        }

        private readonly Dictionary<string, ITopicReceiveBus> _buses = new Dictionary<string, ITopicReceiveBus>();

        public ITopicReceiveBus GetOrAdd(string topic, string endpointSuffix)
        {
            if(!_buses.ContainsKey(topic))
            {
                _buses[topic] = _busFactory(topic, endpointSuffix);
            }

            return _buses[topic];
        }
    }
}
