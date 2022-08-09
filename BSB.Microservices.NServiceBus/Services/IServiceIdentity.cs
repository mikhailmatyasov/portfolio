using System;
using System.Reflection;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// A mechanism for determing the name of the current service.  This is value will be used as part of our naming conventions for queues and exchanges.
    /// </summary>
    public interface IServiceIdentity
    {
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <returns></returns>
        string ServiceName { get; }

        /// <summary>
        /// When running multiple services in a single NServiceBus environment, set an ecosystem name.  This will be used to set the shared error,audit, monitor and serviceControl queues.  If null,
        /// ServiceName will be used.
        /// </summary>
        /// <value>
        /// The name of the ecosystem.
        /// </value>
        string EcosystemName { get; }
    }

    /// <summary>
    /// Uses the entry assembly as the source of the service name.
    /// </summary>
    /// <seealso cref="BSB.Microservices.NServiceBus.IServiceIdentity" />
    public class EntryAssemblyServiceIdentity : IServiceIdentity
    {
        public EntryAssemblyServiceIdentity()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                ServiceName = Assembly.GetEntryAssembly().FullName.Split(',')[0];
                EcosystemName = ServiceName;
            }
        }

        public string ServiceName
        {
            get; protected set;
        }

        public string EcosystemName { get; protected set; }
    }

    public class ServiceIdentity : EntryAssemblyServiceIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceIdentity" /> class.
        /// IF a null or empty string is passed, base class logic kicks in.
        /// </summary>
        /// <param name="serviceName">The name.</param>
        /// <param name="ecoSystemName">Name of the eco system.</param>
        public ServiceIdentity(string serviceName = null, string ecoSystemName = null) : base()
        {
            if (!string.IsNullOrEmpty(serviceName))
            {
                ServiceName = serviceName;
            }
            if (!string.IsNullOrEmpty(ecoSystemName))
            {
                EcosystemName = ecoSystemName;
            }

            if (string.IsNullOrEmpty(ServiceName))
            {
                throw new ArgumentException("ServiceName must be set.");
            }
        }
    }
}