using System;
using NServiceBus.ObjectBuilder.Common;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a service provider used for NServiceBus dependency registrations.
    /// </summary>
    public interface IServiceContainer : IContainer
    {
        TService GetRequiredService<TService>();

        object GetRequiredService(Type serviceType);

        void SetEndpointContext(EndpointData endpoint);
    }
}
