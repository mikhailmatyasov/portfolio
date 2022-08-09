using System;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a service used for determining the naming structure for the bus endpoint.
    /// </summary>
    public interface IEndpointNamingConventions
    {
        /// <summary>
        /// A function used to build the queue name for the implementation of <see cref="IReceiveBus"/>.
        /// </summary>
        Func<string> GetQueueName { get; }

        /// <summary>
        /// A function used to build the error queue name for the implementation of <see cref="IReceiveBus"/>.  Error queues are shared amongst logically related services.  Many Microservices can
        /// use the same audit queue as long as they belong to the same business domain.  Error queues are used by ServiceControl to report failures and allow message inspection and replay.
        /// </summary>
        Func<string> GetErrorQueueName { get; }

        /// <summary>
        /// A function used to build the audit queue name for the implementation of <see cref="IReceiveBus"/>.  Audit queues are shared amongst logically related services.  Many Microservices can
        /// use the same audit queue as long as they belong to the same business domain.  Audit queues are used by ServiceControl to store copies of messages.
        /// </summary>
        /// <value>
        /// The name of the get audit queue.
        /// </value>
        Func<string> GetSharedAuditQueueName { get; }

        /// <summary>
        /// A function used to build the monitoring queue name for the implementation of <see cref="IReceiveBus"/>. Monitoring queues are shared amongst logically related services.  Many Microservices can
        /// use the same audit queue as long as they belong to the same business domain.  Monitoring queues are used by ServiceControl to store metrics about your NServiceBus environment.
        /// </summary>
        /// <value>
        /// The name of the get monitoring queue.
        /// </value>
        Func<string> GetSharedMonitoringQueueName { get; }

        /// <summary>
        /// A function used to build the  service control queue name for the implementation of <see cref="IReceiveBus"/>.  The service control name is needed to send heartbeats and custom checks.  
        /// The ServiceControl input queue is equal to the ServiceControl service name as configured in the ServiceControl Management tool.  By convention, it should be {ServiceIdentity}.ServiceControl.
        /// </summary>
        /// <value>
        /// The name of the get service control.
        /// </value>
        Func<string> GetSharedServiceControlQueueName { get; }
    }
}