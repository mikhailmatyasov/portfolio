using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents an endpoint connection recovery task.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RecoveryTask
    {
        /// <summary>
        /// The unique endpoint identifier.
        /// </summary>
        public Guid EndpointId { get; }

        /// <summary>
        /// The recovery task.
        /// </summary>
        public Task Task { get; }

        public RecoveryTask(Guid endpointId, Task task = null)
        {
            EndpointId = endpointId;
            Task = task;
        }
    }
}
