using System;
using System.Linq;

namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    /// <summary>
    /// Represents a service used for determining the naming structure for RabbitMQ queues and exchanges.
    /// </summary>
    public interface INamingConventions : IEndpointNamingConventions
    {
        /// <summary>
        /// A function used to build exchange names based on the specified message type.
        /// </summary>
        Func<Type, string> GetExchangeName { get; }

        /// <summary>
        /// Returns true if <see cref="IEndpointNamingConventions.GetQueueName"/> has not been overriden.
        /// </summary>
        bool IsDefaultQueueName { get; }
    }

    public class NamingConventions : INamingConventions
    {
        private readonly IServiceIdentity _serviceIdentity;

        public NamingConventions(IServiceIdentity serviceIdentity = null)
        {
            _serviceIdentity = serviceIdentity ?? new EntryAssemblyServiceIdentity();
        }

        public virtual bool IsDefaultQueueName => GetQueueName() == _serviceIdentity.ServiceName;

        public virtual Func<string> GetQueueName => () => _serviceIdentity.ServiceName;

        public virtual Func<string> GetErrorQueueName => () => $"{_serviceIdentity.EcosystemName}.Error";

        public virtual Func<Type, string> GetExchangeName => messageType =>
        {
            if (messageType.IsGenericType)
            {
                var generics = messageType.GenericTypeArguments.Select(arg => GetExchangeName(arg));

                return $"{messageType.Namespace}{messageType.Name}[{string.Join(",", generics)}]";
            }

            return $"{messageType.FullName}";
        };

        public virtual Func<string> GetSharedAuditQueueName => () => $"{_serviceIdentity.EcosystemName}.Audit";

        public virtual Func<string> GetSharedMonitoringQueueName => () => $"{_serviceIdentity.EcosystemName}.Monitoring";

        public virtual Func<string> GetSharedServiceControlQueueName => () => $"{_serviceIdentity.EcosystemName}.ServiceControl";
    }
}