using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a bus that receives incoming messages.
    /// </summary>
    public interface IReceiveBus : IRestartableBus
    {

    }

    public class ReceiveBus : BusBase, IReceiveBus
    {
        public ReceiveBus(
            ILogger<IBus> logger,
            IEndpointManager endpointManager,
            IBusRecoveryManager recoveryManager,
            IEnumerable<IReceiveBusStartupService> startupServices)
            : base(EndpointData.Default(), logger, endpointManager, recoveryManager, startupServices)
        {
        }
    }
}
