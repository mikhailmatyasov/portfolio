namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents a registered service that will be started when a <see cref="ISendBus"/> instance is started for the first time.
    /// </summary>
    public interface ISendBusStartupService : IBusStartupService
    {

    }
}
