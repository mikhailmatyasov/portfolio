namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Represents the connection endpoint configuration.
    /// </summary>
    public enum EndpointType
    {
        /// <summary>
        /// Endpoint will be set to receive and send messsages.
        /// </summary>
        Default,

        /// <summary>
        /// Endpoint will be set to send messages.
        /// </summary>
        SendOnly,

        /// <summary>
        /// Endpoint will only handle messages for a given topic.
        /// </summary>
        Topic
    }
}
