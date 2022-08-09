using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a bus that receives incoming messages for a given topic.
    /// </summary>
    public interface ITopicReceiveBus : IRestartableBus
    {

    }

    public class TopicReceiveBus : BusBase, ITopicReceiveBus
    {
        public TopicReceiveBus(
            string topic,
            string endpointSuffix,
            ILogger<IBus> logger,
            IEndpointManager endpointManager,
            IBusRecoveryManager recoveryManager,
            IEnumerable<IBusStartupService> startupServices)
            : base(EndpointData.Topic(topic, endpointSuffix), logger, endpointManager, recoveryManager, startupServices)
        {
        }
    }
}
